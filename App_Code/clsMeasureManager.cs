using System;
using System.Data;
using System.Configuration;
using System.Linq;






using System.Xml.Linq;
using System.Data.SqlClient;
using Delve;

/// <summary>
/// Summary description for clsMeasureManager
/// </summary>
/// 
namespace Delve
{
    public class clsMeasureManager
    {
        public static DataTable GetMeasures()
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "select ID AS msr_unit_code, Name AS msr_unit_desc from UOM order by 1";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "UOM");
            return dt;
        }
        public static void CreateMeasure(clsMeasure msr)
        {
            String connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = " insert into UOM (Name,Active) values ('" + msr.MsrUnitDesc + "','True')";
            DataManager.ExecuteNonQuery(connectionString, query);
            sqlCon.Close();
        }

        public static void UpdateMeasure(clsMeasure msr)
        {
            String connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = " update UOM set Name='" + msr.MsrUnitDesc + "' where ID='" + msr.MsrUnitCode + "'";
            DataManager.ExecuteNonQuery(connectionString, query);
            sqlCon.Close();
        }

        public static void DeleteMeasure(clsMeasure msr)
        {
            String connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = " delete from UOM where ID='" + msr.MsrUnitCode + "'";
            DataManager.ExecuteNonQuery(connectionString, query);
            sqlCon.Close();
        }

        public static clsMeasure GetMeasure(System.String msr)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "select * from UOM where ID = '" + msr + "'";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "UOM");
            sqlCon.Close();
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            return new clsMeasure(dt.Rows[0]);
        }
    }
}