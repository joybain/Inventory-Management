using System;
using System.Data;
using System.Configuration;
//using System.Linq;






//using System.Xml.Linq;
using System.Data.SqlClient;
/// <summary>
/// Summary description for CountryManager
/// </summary>
/// 
using Delve;

namespace KHSC
{
    public class clsCountryManager
    {
        public static DataTable GetCountries()
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "select country_code,country_desc,country_abvr from country_info order by country_code";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "CountryInfo");
            return dt;
        }
        public static void CreateCountry(clsCountry cnt)
        {
            String connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);

            string query = " insert into country_info(country_code,country_abvr,country_desc) values ('" + cnt.CountryCode + "','" + cnt.CountryAbvr + "','"+cnt.CountryDesc+"')";

            DataManager.ExecuteNonQuery(connectionString, query);
            sqlCon.Close();
        }

        public static void UpdateCountry(clsCountry cnt)
        {
            String connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);

            string query = " update country_info set country_abvr='" + cnt.CountryAbvr + "',country_desc='"+cnt.CountryDesc+"' where country_code='" + cnt.CountryCode + "'";

            DataManager.ExecuteNonQuery(connectionString, query);
            sqlCon.Close();
        }

        public static void DeleteCountry(clsCountry cnt)
        {
            String connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);

            string query = " delete from country_info where country_code='" + cnt.CountryCode + "'";

            DataManager.ExecuteNonQuery(connectionString, query);
            sqlCon.Close();
        }

        public static clsCountry GetCountry(System.String cntid)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "select * from country_info where country_code = '" + cntid + "'";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Country");
            sqlCon.Close();
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            return new clsCountry(dt.Rows[0]);
        }
        public static string GetCountryAbvr(string cc)
        {
            String ConnectionString = DataManager.OraConnString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            string Query = "select country_abvr from country_info where country_code='"+cc+"'" ;
            myConnection.Open();
            SqlCommand myCommand = new SqlCommand(Query, myConnection);
            object maxValue = myCommand.ExecuteScalar();
            myConnection.Close();
            return maxValue.ToString();
        }
    }
}