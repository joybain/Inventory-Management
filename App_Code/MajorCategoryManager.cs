using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

using System.Data.SqlClient;
using Delve;

/// <summary>
/// Summary description for MajorCategoryManager
/// </summary>
public class MajorCategoryManager
{
	public MajorCategoryManager()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public static DataTable GetMajorCats(string Search)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string Para = "";
        if (!string.IsNullOrEmpty(Search))
        { Para = " and DeptID='" + Search + "'"; }
        string query = "select t1.ID AS mjr_code,Name AS mjr_desc,Description,Active,Code,DeptID,t2.Dept_Name from Category t1 left join Depertment_Type t2 on t2.ID=t1.DeptID WHERE DeleteBy IS NULL " + Para + " order by t1.ID  ";

        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Category");
        return dt;
    }

    public static MajorCategory GetMajorCat(string mjr)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string query = "SELECT [ID],[Name],[Description],[Active],Code FROM [Category] where ID='" + mjr + "' ";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Mjr");
        if (dt.Rows.Count == 0)
        {
            return null;
        }
        return new MajorCategory(dt.Rows[0]);
    }

    public static void DeleteMajorCat(MajorCategory majcat)
    {
        String connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string query = "delete from Category where ID= '" + majcat.ID + "'";
        DataManager.ExecuteNonQuery(connectionString, query);
    }

    public static void CreateMajorCat(MajorCategory majcat)
    {
        String connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);

        string query = " insert into Category(Name,Description,Active,Code,DeptID) values ('" + majcat.Name + "', '" +
                       majcat.Description + "', '" + majcat.Active + "','" + majcat.Code + "', '" + majcat.DeptID +
                       "')";

        DataManager.ExecuteNonQuery(connectionString, query);
    }

    public static void UpdateMajorCat(MajorCategory majcat)
    {
        String connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);

        string query = " update Category set Name='" + majcat.Name + "' , Description= '" + majcat.Description +
                       "',Active='" + majcat.Active + "',DeptID='" + majcat.DeptID + "' where ID= '" + majcat.ID + "'";

        DataManager.ExecuteNonQuery(connectionString, query);
    }

    //***************************** New Code Catagory And Sub Catagory Code **************************//


    public DataTable GetCatagoryOnSearch(string CatagorySearchID)
    {
        String connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);

        string query = @"SELECT [ID],[Code] ,[Name] ,[Description] ,[SearchCatagory]
  FROM [View_CatagorySearch] where SearchCatagory='" + CatagorySearchID.ToUpper() + "' ";

        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "View_CatagorySearch");
        return dt;
    }

    public DataTable GetCaregotyOnDepartment(string DeptID)
    {
        String connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string query = @"SELECT [ID],[Code],[Name],[Description],[SearchCatagory],[DeptID],[Dept_Name]
        FROM [dbo].[View_CatagorySearch] where DeptID='" + DeptID + "' ";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "View_CatagorySearch");
        return dt;
    }
}