using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Delve;


/// <summary>
/// Summary description for ExceptionLogging
/// </summary>
public static class ExceptionLogging
{
    private static String exepurl;
    static SqlConnection con;
   
    public static void SendExcepToDB(Exception exdb)
    {

        SqlConnection con = new SqlConnection(DataManager.OraConnString());
        con.Open();
        exepurl = HttpContext.Current.Request.Url.AbsolutePath;

        SqlCommand com = new SqlCommand("ExceptionLoggingToDataBase", con);
        com.CommandType = CommandType.StoredProcedure;
        com.Parameters.AddWithValue("@ExceptionMsg", exdb.Message.ToString());
        com.Parameters.AddWithValue("@ExceptionType", exdb.GetType().Name.ToString());
        com.Parameters.AddWithValue("@ExceptionURL", exepurl);
        com.Parameters.AddWithValue("@ExceptionSource", exdb.StackTrace.ToString());
        com.ExecuteNonQuery();

        con.Close();
    } 
}