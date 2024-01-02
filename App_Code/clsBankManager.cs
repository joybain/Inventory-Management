using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;
using System.Data.SqlClient;


using Delve;


/// <summary>
/// Summary description for clsBankManager
/// </summary>
/// 
namespace Delve
{
    public class clsBankManager
    {
        public static void CreateBankMst(clsBankMst mst)
        {
            String connectionString = DataManager.OraConnString();
            string query = " insert into bank_info(bank_name,Gl_Code,Active,BankType,ParentCode) values ('" + mst.BankName + "','" + mst.SegmentCoaCode + "','True','" + mst.bankType + "','" + mst.ParentCode + "')";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static void UpdateBankMst(clsBankMst mst)
        {
            String connectionString = DataManager.OraConnString();
            string query = " update bank_info set bank_name = '" + mst.BankName + "',BankType='" + mst.bankType + "',ParentCode='" + mst.ParentCode + "',[LocalUploadBranchID]='0,',Gl_Code='" + mst.SegmentCoaCode + "' where bank_id= '" + mst.BankId + "' ";
            DataManager.ExecuteNonQuery(connectionString, query);



            //string query1 = @"UPDATE [GL_SEG_COA] SET [SEG_COA_DESC] ='" + mst.BankName + "'  WHERE [SEG_COA_CODE]='" + mst.BankId + "'";
            //DataManager.ExecuteNonQuery(connectionString, query1);

            //string query2 = @"UPDATE [GL_COA] SET [COA_DESC] ='SDL," + mst.BankName + "' where [GL_COA_CODE]='1-" + mst.BankId + "'";
            //DataManager.ExecuteNonQuery(connectionString, query2);

        }
        public static void DeleteBankMst(clsBankMst mst)
        {
            String connectionString = DataManager.OraConnString();
            string query = " delete from bank_info where bank_id= '" + mst.BankId + "' ";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static clsBankMst getBankMst(string mst)
        {
            String connectionString = DataManager.OraConnString();
            string query = " select bank_id, bank_name from bank_info where bank_id='" + mst + "' ";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "BankMaster");
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            return new clsBankMst(dt.Rows[0]);
        }
        public static DataTable getBankMsts()
        {
            String connectionString = DataManager.OraConnString();
            string query = "select bank_id,bank_name,Gl_Code,BankType,ParentCode,case when bankType='B' then 'Bank' when bankType='M' then 'Mobile Bank' when bankType='C' then 'Card Visa/Masters' end AS[BankTypeName] from bank_info";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "BankMasters");
            return dt;
        }
        public static void CreateBankDtl(clsBankDtl dtl)
        {
            String connectionString = DataManager.OraConnString();
            string query =
                "insert into bank_branch (bank_id,branch_name,addr1,addr2,PHONE1,PHONE2,AccountNo,Gl_Code,AddBy,AddDate,AccountName,[AccountType],[Active],BankCharge,BankType) values (" +
                "  '" + dtl.BankId + "','" + dtl.BranchName + "', '" + dtl.Addr1 + "', " +
                "  '" + dtl.Addr2 + "', '" + dtl.Phone + "', '" + dtl.Phone1 + "', '" + dtl.AccountNo + "', '" +
                dtl.Gl_Code + "', '" + dtl.LoginBy + "',GETDATE(),'" + dtl.AccountName + "','" + dtl.AccountType + "','" + dtl.Status + "','" + dtl.BankCharge + "','" + dtl.BankType + "')";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static void UpdateBankDtl(clsBankDtl dtl)
        {
            String connectionString = DataManager.OraConnString();
            string query = " update bank_branch set branch_name = '" + dtl.BranchName + "',[BankType]='" + dtl.BankType + "',BankCharge='" + dtl.BankCharge + "',AccountName='" + dtl.AccountName + "',addr1 = '" + dtl.Addr1 + "', " +
                " addr2 = '" + dtl.Addr2 + "',[AccountType]='" + dtl.AccountType + "',[Active]='" + dtl.Status + "',PHONE1 = '" + dtl.Phone + "',Gl_Code='" + dtl.Gl_Code + "',PHONE2 = '" + dtl.Phone1 + "',AccountNo='" + dtl.AccountNo + "',UpdateBy='" + dtl.LoginBy + "',UpdateDate=GETDATE() where ID='" + dtl.ID + "' ";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static void DeleteBankDtl(clsBankDtl dtl)
        {
            String connectionString = DataManager.OraConnString();
            string query = " update bank_branch set DeleteBy='" + dtl.LoginBy + "',DeleteDate=GETDATE() where ID='" + dtl.ID + "' ";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static clsBankDtl getBankDtl(string mst, string dtl)
        {
            String connectionString = DataManager.OraConnString();
            string query = " select bank_id, branch_id,branch_name,addr1,addr2,phone from bank_branch where bank_id='" + mst + "' and branch_id='" + dtl + "' ";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "BankDtl");
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            return new clsBankDtl(dt.Rows[0]);
        }

        public static DataTable getBankDtls(string mst)
        {
            String connectionString = DataManager.OraConnString();
            string Parameter = "";
            if (!string.IsNullOrEmpty(mst))
            {
                Parameter = " and t1.bank_id='" + mst + "' order by t1.branch_name";
            }
            string query = @"select t1.ID,t1.bank_id,t2.BANK_NAME,t1.Gl_Code,t1.branch_name,t1.AccountNo,t1.addr1,t1.addr2,t1.phone1 from bank_branch t1
            left join [dbo].[BANK_INFO] t2 on t2.BANK_ID=t1.BANK_ID where t1.DeleteBy IS NULL " + Parameter;
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "bank_branch");
            return dt;
        }

        public static string GatShowAutoId()
        {
            SqlConnection connection = new SqlConnection(DataManager.OraConnString());
            connection.Open();
            string selectQuery = @"SELECT '1102'+RIGHT('000'+CONVERT(VARCHAR,ISNULL(MAX(CONVERT(INTEGER,RIGHT([bank_id],3))),0)+1),3) FROM [bank_info]";
            SqlCommand command = new SqlCommand(selectQuery, connection);
            return command.ExecuteScalar().ToString();
        }

        public string GetShowCoaCode(string p)
        {
            SqlConnection connection = new SqlConnection(DataManager.OraConnString());

            string selectQuery = @"SELECT [gl_coa_code]  FROM [bank_info] where [bank_id]='" + p + "'";
            connection.Open();
            SqlCommand myCommand = new SqlCommand(selectQuery, connection);
            object maxValue = myCommand.ExecuteScalar();
            connection.Close();
            if (maxValue == null)
            {
                maxValue = "0";
            }
            return maxValue.ToString();
        }

        public static string GetshowBankCoa(string CoaCode, string CheckBank)
        {
            SqlConnection connection = new SqlConnection(DataManager.OraConnString());
            try
            {
                connection.Open();
                // string Query = @"SELECT count(*) FROM [GL_COA] where [GL_COA_CODE] ='" + CoaCode + "'";
                string Query = "";
                if (CheckBank.Equals("1"))
                {
                    Query =
                        "select t.PARENT_CODE from GL_SEG_COA t where t.SEG_COA_CODE in (SELECT t2.PARENT_CODE FROM [GL_COA] t1 inner join GL_SEG_COA t2 on t2.SEG_COA_CODE=t1.COA_NATURAL_CODE where t1.GL_COA_CODE='" +
                        CoaCode + "')";
                }
                else
                {
                    Query = @"SELECT t2.PARENT_CODE FROM [GL_COA] t1 inner join GL_SEG_COA t2 on t2.SEG_COA_CODE=t1.COA_NATURAL_CODE where t1.GL_COA_CODE='" + CoaCode + "'";
                }
                SqlCommand command = new SqlCommand(Query, connection);

                return command.ExecuteScalar().ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                connection.Close();
            }

        }
        public static string GetshowBankCoa(string CoaCode)
        {
            SqlConnection connection = new SqlConnection(DataManager.OraConnString());
            try
            {
                connection.Open();
                // string Query = @"SELECT count(*) FROM [GL_COA] where [GL_COA_CODE] ='" + CoaCode + "'";
                string Query = "";
                
                {
                    Query = @"SELECT t2.PARENT_CODE FROM [GL_COA] t1 inner join GL_SEG_COA t2 on t2.SEG_COA_CODE=t1.COA_NATURAL_CODE where t1.GL_COA_CODE='" + CoaCode + "'";
                }
                SqlCommand command = new SqlCommand(Query, connection);

                return command.ExecuteScalar().ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                connection.Close();
            }

        }

        public static clsBankDtl getBranchDtl(string ID)
        {
            String connectionString = DataManager.OraConnString();
            string query = "SELECT [ID],[BANK_ID],[BRANCH_NAME],[ADDR1],[AccountType],[Active],[AccountName],[ADDR2],[ADDR3],[PHONE1],[PHONE2],[AccountNo],[Gl_Code],BankType  FROM [dbo].[BANK_BRANCH] where  [ID]='" + ID + "' ";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "BankDtl");
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            return new clsBankDtl(dt.Rows[0]);
        }

        public static void UpdatePaymentmethod(PaymentMethod mst)
        {
            String connectionString = DataManager.OraConnString();
            string query = @"Update [dbo].[PaymentMethod]  set [Name]='" + mst.Name + "',[LocalUploadBranchID]='0,' ,UpdateBy='" + mst.LoginBy + "',UpdateDate=getdate(),Active='"+mst.Status+"',[LocalUploadFlag]=case when  LocalUploadFlag=1 then 2 else LocalUploadFlag end  where ID='" + mst.Id + "' ";
            DataManager.ExecuteNonQuery(connectionString, query);
        }

        public static void CreatePaymentmethod(PaymentMethod mst)
        {
            String connectionString = DataManager.OraConnString();
            string query = @"INSERT INTO [dbo].[PaymentMethod]   ([Name],[Active],[LocalUploadFlag],AddBy,AddDate)  VALUES ('" + mst.Name + "','" + mst.Status + "',0,'" + mst.LoginBy + "',GetDate())";
            DataManager.ExecuteNonQuery(connectionString, query);
        }

        public static object GetPaymentMethodInfo()
        {
            String connectionString = DataManager.OraConnString();
            string query = " select ID, Name,case when Active=1 then 'Active' else 'In Active' end as Active from PaymentMethod where  DeleteBy is null order by id desc";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "BankMaster");
            return dt;
        }

        public static PaymentMethod GetPaymentMethodInfodtls(string mst)
        {
            String connectionString = DataManager.OraConnString();
            string query = " select ID, Name from PaymentMethod where ID='" + mst + "' and DeleteBy is null ";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "BankMaster");
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            return new PaymentMethod(dt.Rows[0]);
        }

        public static void DeletePaymentMethod(PaymentMethod mst)
        {
            String connectionString = DataManager.OraConnString();
            string query = @"Update [dbo].[PaymentMethod]  set DeleteBy='" + mst.LoginBy + "',DeleteDate=getdate(),[LocalUploadFlag]=case when  LocalUploadFlag=1 then 2 else LocalUploadFlag end where ID='" + mst.Id + "'";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
    }
}