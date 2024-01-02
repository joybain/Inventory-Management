using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

using Delve;

/// <summary>
/// Summary description for PaymentMethodManager
/// </summary>
namespace OldColor
{
    public class PaymentMethodManager
    {
        public PaymentMethodManager()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public int Save(PaymentMethodModel payment)
        {
            SqlConnection connection = new SqlConnection(DataManager.OraConnString());
            connection.Open();
            string query =
                "insert into PaymentMethod (PaymentTypeId, AccountName, AccountNo, Gl_Code, AddBy, AddDate) values('" +
                payment.PaymentTypeId + "','" + payment.AccountName + "','" + payment.AccountNo + "','" +
                payment.gl_Code + "','" + payment.LoginBy + "',GETDATE())";
            var command = new SqlCommand(query, connection);
            int success = command.ExecuteNonQuery();
            return success;
        }

        public int Update(PaymentMethodModel payment)
        {
            SqlConnection connection = new SqlConnection(DataManager.OraConnString());
            connection.Open();
            string query = "update PaymentMethod set PaymentTypeId='" + payment.PaymentTypeId + "',AccountName='" +
                           payment.AccountName + "',AccountNo='" + payment.AccountNo + "',Gl_Code='" + payment.gl_Code +
                           "',UpdateBy='" + payment.LoginBy + "',UpdateDate=GETDATE() where Id='" + payment.Id + "'";
            var command = new SqlCommand(query, connection);
            int success = command.ExecuteNonQuery();
            return success;
        }

        public int Delete(PaymentMethodModel payment)
        {
            SqlConnection connection = new SqlConnection(DataManager.OraConnString());
            connection.Open();
            string query = "update PaymentMethod set DeleteBy='" + payment.LoginBy +
                           "',DeleteDate=GETDATE() where Id='" + payment.Id + "'";
            var command = new SqlCommand(query, connection);
            int success = command.ExecuteNonQuery();
            return success;
        }

        public DataTable GetAllPaymentType()
        {
            var connectionstring = DataManager.OraConnString();
            string query = "select Id,Name from PaymentType";
            var data = DataManager.ExecuteQuery(connectionstring, query, "PaymentType");
            return data;
        }

        public DataTable GetAll(string Id, string PaymentTypeId, string AccountNo)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            sqlCon.Open();
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = new SqlCommand("SP_GetAllPaymentMethod", sqlCon);
            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            if (string.IsNullOrEmpty(Id))
            {
                da.SelectCommand.Parameters.AddWithValue("@Id", null);
            }
            else
            {
                da.SelectCommand.Parameters.AddWithValue("@Id", Id);
            }

            if (string.IsNullOrEmpty(PaymentTypeId))
            {
                da.SelectCommand.Parameters.AddWithValue("@PaymentTypeId", null);
            }
            else
            {
                da.SelectCommand.Parameters.AddWithValue("@PaymentTypeId", PaymentTypeId);
            }

            if (string.IsNullOrEmpty(AccountNo))
            {
                da.SelectCommand.Parameters.AddWithValue("@AccountNo", null);
            }
            else
            {
                da.SelectCommand.Parameters.AddWithValue("@AccountNo", AccountNo);
            }

            da.SelectCommand.CommandTimeout = 6000;
            DataSet ds = new DataSet();
            da.Fill(ds, "SP_GetAllPaymentMethod");
            DataTable dt = ds.Tables["SP_GetAllPaymentMethod"];
            return dt;
        }
    }
}