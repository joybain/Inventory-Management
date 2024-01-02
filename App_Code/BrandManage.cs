using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;
using System.Data.SqlClient;

using Delve;
using Delve;

/// <summary>
/// Summary description for CompanyManage
/// </summary>
/// 
namespace Delve
{
    public class BrandManage
    {
        public static DataTable GetCompanies()
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "select ID AS com_id,BrandName AS com_desc,[Active] AS [check] from Brand order by ID";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Brand");
            return dt;
        }
        public static void CreateCompany(Brand comp)
        {
            String connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);

            string query = " insert into Brand(BrandName,Active) values ('" + comp.CompanyDesc + "','" + comp.Active + "')";

            DataManager.ExecuteNonQuery(connectionString, query);
            sqlCon.Close();
        }

        public static void UpdateCompany(Brand comp)
        {
            String connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);

            string query = " update Brand set BrandName='" + comp.CompanyDesc + "',Active='" + comp.Active + "' where ID='" + comp.CompanyId + "'";

            DataManager.ExecuteNonQuery(connectionString, query);
            sqlCon.Close();
        }

        public static void DeleteCompany(Brand comp)
        {
            String connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);

            string query = " delete from Brand where ID='" + comp.CompanyId + "'";

            DataManager.ExecuteNonQuery(connectionString, query);
            sqlCon.Close();
        }

        public static Brand GetCompany(System.String comid)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "select * from Brand where ID = '" + comid + "'";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Brand");
            sqlCon.Close();
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            return new Brand(dt.Rows[0]);
        }

        public DataTable GetBrandOnSearch(string BrandID)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = @"SELECT [ID],[BrandName],[SearchBrand]
  FROM [View_BrandSearch] where SearchBrand='" + BrandID + "' ";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Brand");
            return dt;
        }
    }
}