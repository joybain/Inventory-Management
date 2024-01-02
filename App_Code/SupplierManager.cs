using System;
using System.Data;
using System.Configuration;
using System.Linq;






using System.Xml.Linq;
using System.Data.SqlClient;
using sales;
using Delve;

/// <summary>
/// Summary description for SupplierManager
/// </summary>
/// 
namespace sales
{    
    public class SupplierManager
    {
        
        public static DataTable GetSuppliers(string Parameter)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "select t1.*,t2.COUNTRY_DESC from Supplier t1 inner join COUNTRY_INFO t2 on t2.COUNTRY_CODE=t1.Country " + Parameter + " order by ID DESC ";

            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Supplier");
            return dt;
        }
        public static Supplier GetSupplier(string sup)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "select * from Supplier where ID='" + sup + "' ";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Supplier");
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            return new Supplier(dt.Rows[0]);
        }
        public static void DeleteSupplier(Supplier sup)
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

                command.CommandText = @"delete from supplier where ID= '" + sup.ID + "'";
                command.ExecuteNonQuery();

                //*********** Auto Coa generate off **********//
                //command.CommandText = @"delete from GL_SEG_COA where SEG_COA_CODE='" + sup.GlCoa + "' ";
                //command.ExecuteNonQuery();

                //command.CommandText = @"delete from GL_COA where COA_NATURAL_CODE='" + sup.GlCoa + "' ";
                //command.ExecuteNonQuery();

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
        public static void CreateSupplier(Supplier sup, SegCoa segcoa, GlCoa glcoa)
        {
            String connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string variables = "";

            variables = @" [Code],[Name],[ContactName],[Designation],[Email],[Phone],[Fax],[Mobile],[Address1],[Address2],[City],[State],[PostalCode],[Country],[SupplierGroupID],[Active],[CreatedBy],[CreatedDate],Gl_CoaCode";
            string values = " '" + sup.SupCode + "','" + sup.ComName + "','" + sup.SupName + "','" + sup.Designation + "','" + sup.Email + "','" + sup.SupPhone + "','" + sup.Fax + "','" + sup.SupMobile + "','" + sup.SupAddr1 + "','" + sup.SupAddr2 + "','" + sup.City + "','" + sup.State + "','" + sup.PostCode + "','" + sup.Country + "','" + sup.SupGroup + "','" + sup.Active + "','" + sup.LoginBy + "',GETDATE(),'" + sup.GlCoa + "'";
           
         

            string query = "";
            if (sup.SupNIDImage != null)
            {
                if (sup.SupNIDImage.Length > 0)
                {
                    variables = variables + ",NIDImage";
                    values = values + ",@img1";
                }
            }

            if (sup.SupCurrentImage != null)
            {
                if (sup.SupCurrentImage.Length > 0)
                {
                    variables = variables + ",CurrentImage";
                    values = values + ",@img2";
                }
            }

            query = " INSERT INTO [Supplier] (" + variables + ")  values ( " + values + " )";


            SqlParameter img = new SqlParameter();
            img.SqlDbType = SqlDbType.VarBinary;
            img.ParameterName = "img1";
            img.Value = sup.SupNIDImage;

            SqlParameter img2 = new SqlParameter();
            img2.SqlDbType = SqlDbType.VarBinary;
            img2.ParameterName = "img2";
            img2.Value = sup.SupCurrentImage;

            using (SqlCommand cmnd = new SqlCommand(query, sqlCon))
            {
                cmnd.Parameters.Add(img);
                cmnd.Parameters.Add(img2);
                if (sup.SupNIDImage == null)
                {
                    cmnd.Parameters.Remove(img);
                }
                if (sup.SupCurrentImage == null)
                {
                    cmnd.Parameters.Remove(img2);
                }
                sqlCon.Open();
                cmnd.ExecuteNonQuery();
            }
           // DataManager.ExecuteNonQuery(connectionString, query);

            // GL Seg Coa Set
            query = "";
            if (segcoa.ParentCode == "" || segcoa.ParentCode == null)
            {
                query = " insert into gl_seg_coa (seg_coa_code,lvl_code,seg_coa_desc,bud_allowed, " +
                       " post_allowed,acc_type,open_date,rootleaf,status,taxable,book_name,entry_user,entry_date) values ( " +
                       " '" + segcoa.GlSegCode + "',  '" + segcoa.LvlCode + "',  '" + segcoa.SegCoaDesc + "', " +
                       " '" + segcoa.BudAllowed + "',  '" + segcoa.PostAllowed + "', " +
                        "  '" + segcoa.AccType + "', convert(datetime,case '" + segcoa.OpenDate + "' when '' then null else '" + segcoa.OpenDate + "' end,103),  '" + segcoa.RootLeaf + "', " +
                        "  '" + segcoa.Status + "',  '" + segcoa.Taxable + "',  '" + segcoa.BookName + "','" + segcoa.EntryUser + "',convert(datetime,'" + segcoa.EntryDate + "',103))";
                SqlCommand com1 = new SqlCommand(query, sqlCon);
                com1.ExecuteNonQuery();
            }
            else
            {
                query = " insert into gl_seg_coa (seg_coa_code,lvl_code,seg_coa_desc,parent_code,bud_allowed, " +
                       " post_allowed,acc_type,open_date,rootleaf,status,taxable,book_name,entry_user,entry_date) values ( " +
                       " '" + segcoa.GlSegCode + "',  '" + segcoa.LvlCode + "',  '" + segcoa.SegCoaDesc + "', " +
                       "  '" + segcoa.ParentCode + "',  '" + segcoa.BudAllowed + "',  '" + segcoa.PostAllowed + "', " +
                        "  '" + segcoa.AccType + "', convert(datetime,case '" + segcoa.OpenDate + "' when '' then null else '" + segcoa.OpenDate + "' end,103),  '" + segcoa.RootLeaf + "', " +
                        "  '" + segcoa.Status + "',  '" + segcoa.Taxable + "',  '" + segcoa.BookName + "','" + segcoa.EntryUser + "',convert(datetime,'" + segcoa.EntryDate + "',103))";
                SqlCommand com1 = new SqlCommand(query, sqlCon);
                com1.ExecuteNonQuery();
            }
            // End Seg Coa

            // GL Coa Suplier

            query = "insert into gl_coa (book_name,gl_coa_code,coa_enabled,effective_from,effective_to, " +
            " bud_allowed,post_allowed,taxable,acc_type,status,coa_desc,coa_curr_bal,coa_natural_code,ENTRY_USER,ENTRY_DATE) values ( " +
            " '" + glcoa.BookName + "',  '" + glcoa.GlCoaCode + "',  '" + glcoa.CoaEnabled + "', " +
            "  convert(datetime,case '" + glcoa.EffectiveFrom + "' when '' then null else '" + glcoa.EffectiveFrom + "' end ,103), convert(datetime,case '" + glcoa.EffectiveTo + "' when '' then null else '" + glcoa.EffectiveTo + "' end ,103), '" + glcoa.BudAllowed + "', " +
            "  '" + glcoa.PostAllowed + "',  '" + glcoa.Taxable + "',  '" + glcoa.AccType + "', " +
            "  '" + glcoa.Status + "',  '" + glcoa.CoaDesc.Replace("'", "") + "',  '" + glcoa.CoaCurrBal + "', " +
            "  '" + glcoa.CoaNaturalCode + "','" + glcoa.LoginBy + "',GETDATE()) ";
            SqlCommand com2 = new SqlCommand(query, sqlCon);
            com2.ExecuteNonQuery();
            // End GL Coa

        }

        public static void UpdateSupplier(Supplier sup)
        {
            String connectionString = DataManager.OraConnString();
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                string variables = "";
                variables = @" [Name] ='" + sup.ComName + "' ,[ContactName] ='" + sup.SupName + "' ,[Designation] ='" +
                            sup.Designation + "' ,[Email] ='" + sup.Email + "',[Phone] = '" + sup.SupPhone +
                            "',[Fax] ='" + sup.Fax + "' ,[Mobile] ='" + sup.SupMobile + "' ,[Address1] ='" +
                            sup.SupAddr1 + "' ,[Address2] ='" + sup.SupAddr2 + "' ,[City] ='" + sup.City +
                            "' ,[State] ='" + sup.State + "' ,[PostalCode] ='" + sup.PostCode + "',[Country] ='" +
                            sup.Country + "',[SupplierGroupID] ='" + sup.SupGroup + "' ,[Active] ='" + sup.Active +
                            "' ,[ModifiedBy] ='" + sup.LoginBy + "' ,[ModifiedDate] =GETDATE(),Gl_CoaCode='" +
                            sup.GlCoa + "' ";


                if (sup.SupNIDImage != null)
                {
                    if (sup.SupNIDImage.Length > 0)
                    {
                        variables = variables + ",NIDImage=@img1";

                    }
                }

                if (sup.SupCurrentImage != null)
                {
                    if (sup.SupCurrentImage.Length > 0)
                    {
                        variables = variables + ",CurrentImage=@img2";

                    }
                }

                string query = " UPDATE [Supplier] SET  " + variables + " WHERE ID='" + sup.ID + "' ";

                SqlParameter img = new SqlParameter();
                img.SqlDbType = SqlDbType.VarBinary;
                img.ParameterName = "img1";
                if (sup.SupNIDImage != null)
                {
                    img.Value = sup.SupNIDImage;
                }




                SqlParameter img2 = new SqlParameter();
                img2.SqlDbType = SqlDbType.VarBinary;
                img2.ParameterName = "img2";

                if (sup.SupCurrentImage != null)
                {
                    img2.Value = sup.SupCurrentImage;
                }

                using (SqlCommand cmnd = new SqlCommand(query, sqlCon))
                {
                    cmnd.Parameters.Add(img);
                    cmnd.Parameters.Add(img2);

                    if (sup.SupNIDImage == null)
                    {
                        cmnd.Parameters.Remove(img);
                    }
                    else
                    {
                        if (sup.SupNIDImage.Length == 0)
                        {
                            cmnd.Parameters.Remove(img);
                        }
                    }

                    if (sup.SupCurrentImage == null)
                    {
                        cmnd.Parameters.Remove(img2);
                    }
                    else
                    {
                        if (sup.SupCurrentImage.Length == 0)
                        {
                            cmnd.Parameters.Remove(img2);
                        }
                    }

                    sqlCon.Open();
                    cmnd.ExecuteNonQuery();
                }

                query = "";
                query = @"UPDATE [GL_SEG_COA] SET [SEG_COA_DESC] ='" + sup.CoaDesc +
                                      "'  WHERE [SEG_COA_CODE]='" + sup.GlCoa + "'";
                SqlCommand com11 = new SqlCommand(query, sqlCon);
                com11.ExecuteNonQuery();

                query = "";
                query = @"UPDATE [GL_COA] SET [COA_DESC] ='" + sup.CoaDesc + "' where [GL_COA_CODE]='1-" +
                                      sup.GlCoa + "'";
                SqlCommand com12 = new SqlCommand(query, sqlCon);
                com12.ExecuteNonQuery();
            }
        }

        public static string GetSupplierName(string sup)
        {
            String ConnectionString = DataManager.OraConnString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            string Query = "select sup_desc from supplier where sup_code='" + sup + "'";
            myConnection.Open();
            SqlCommand myCommand = new SqlCommand(Query, myConnection);
            object maxValue = myCommand.ExecuteScalar();
            myConnection.Close();
            if (maxValue != null)
            {
                return maxValue.ToString();
            }
            return "";
        }

        //********************* Supplier & Party Payment **************************//

        public static DataTable GetShowSupplierOnPayment(string SupplierName,string PatyName,string SearchType)
        {
            String connectionString = DataManager.OraConnString();
            string query = "";
            if (SearchType == "S")
            {
                query =
                    @"SELECT [ID],[GRN],[PODate],[ChallanNo],[SupplierID] ,[ContactName] ,[Remarks],[Total] ,[ShiftmentID],[PvOrder],Gl_CoaCode
 ,[SearchSupplier] FROM [View_SearchSupplierOnPurchase] where UPPER(SearchSupplier)=UPPER('" +
                    SupplierName + "') ";
            }
            else
            {
                query = @"SELECT ID,PartyCode AS Code,PartyName AS ContactName,Gl_CoaCode FROM PartyInfo where upper([PartyCode]+' - '+[PartyName])='" + PatyName + "' ";
            }

            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "party");
            return dt;
        }

        public static SupplierPayment GetShowSupplierPaymentInfo(string ID)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = @"SELECT t1.[ID],CONVERT(nvarchar,t1.[PmDate],103) AS PmDate,t1.[supplier_id],t1.[PayAmt],t1.[PayMethod],t1.[Bank_id],t1.[ChequeNo],CONVERT(nvarchar,t1.[ChequeDate],103) AS ChequeDate,t1.Chk_Status,t1.Payment_Type,t2.GRN,t1.[purchase_id] FROM [SupplierPayment] t1 left join ItemPurchaseMst t2 on t2.ID=t1.purchase_id where t1.ID='" + ID + "'";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Supplier");
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            return new SupplierPayment(dt.Rows[0]);
        }

        public static void SaveSupplierPayment(SupplierPayment Supp)
        {
            String connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);

            string query = @"INSERT INTO [SupplierPayment]
           ([PmDate],[purchase_id],[supplier_id],[PayAmt],[PayMethod],[Bank_id],[ChequeNo],[ChequeDate],Chk_Status,[entry_by],[entry_date],Payment_Type,PartyORSupplier)
     VALUES
           (CONVERT(DATE,'" + Supp.PmDate + "',103),'" + Supp.PvID + "','" + Supp.SupplierID + "','" + Supp.PayAmt + "','" + Supp.PaymentMethord + "','" + Supp.BankId + "','" + Supp.ChequeNo + "',CONVERT(DATE,'" + Supp.ChequeDate + "',103),'" + Supp.ChequeStatus + "','" + Supp.LoginBy + "',GETDATE(),'" + Supp.Payment_Type + "','" + Supp.PartyOrSupplier + "')";
            DataManager.ExecuteNonQuery(connectionString, query);
        }

        public static void UpdateSupplierPayment(SupplierPayment Supp)
        {
            String connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);

            string query = @"UPDATE  [SupplierPayment] SET
           [PmDate]=CONVERT(DATE,'" + Supp.PmDate + "',103),[PayAmt]='" + Supp.PayAmt + "',[PayMethod]='" + Supp.PaymentMethord + "',[Bank_id]='" + Supp.BankId + "',[ChequeNo]='" + Supp.ChequeNo + "',[ChequeDate]=CONVERT(DATE,'" + Supp.ChequeDate + "',103),[update_by]='" + Supp.LoginBy + "',[update_date]=GETDATE(),Chk_Status='" + Supp.ChequeStatus + "',[purchase_id]='" + Supp.PvID + "' where ID='" + Supp.ID + "' ";
            DataManager.ExecuteNonQuery(connectionString, query);
        }

        public static int GetShowPaymentID()
        {
            SqlConnection connection = new SqlConnection(DataManager.OraConnString());
            try
            {
                connection.Open();
                string Query = @"SELECT top(1)[ID]  FROM [SupplierPayment] order by [ID] desc";
                SqlCommand command = new SqlCommand(Query, connection);
                return Convert.ToInt32(command.ExecuteScalar());
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

        public static DataTable GetShowSupplierHistory(string ID,string P_Type,string SupplierOrParty)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string Parameter = "";
            string query="";
            if (SupplierOrParty == "S")
            {
                if (ID == "") { Parameter = "WHERE t1.Chk_Status not in ('A') and Payment_Type='" + P_Type + "' order by ID desc"; } else { Parameter = "Where t1.Chk_Status not in ('A') and t1.supplier_id='" + ID + "' and Payment_Type='" + P_Type + "' order by ID desc"; }
                query = @"SELECT top(50) t1.[ID]
              ,t2.Code
              ,t2.ContactName
              ,CONVERT(nvarchar,t1.[PmDate],103) AS PmDate           
              ,t1.[PayAmt]
              ,CASE WHEN t1.Chk_Status='P' THEN 'Pending' WHEN t1.Chk_Status='A' THEN 'Approved' WHEN t1.Chk_Status='B' THEN 'Bounce' ELSE '' END AS CHK_Status  
              ,t1.ChequeNo           
          FROM [SupplierPayment] t1 inner join Supplier t2 on t2.ID=t1.supplier_id " + Parameter;
            }
            else
            {
                if (ID == "") { Parameter = "WHERE t1.Chk_Status not in ('A') and Payment_Type='" + P_Type + "' order by ID desc"; } else { Parameter = "Where t1.Chk_Status not in ('A') and t1.supplier_id='" + ID + "' and Payment_Type='" + P_Type + "' order by ID desc"; }

                query = @"SELECT top(50) t1.[ID]
              ,t2.PartyCode AS Code
              ,t2.PartyName AS ContactName
              ,CONVERT(nvarchar,t1.[PmDate],103) AS PmDate           
              ,t1.[PayAmt]
              ,CASE WHEN t1.Chk_Status='P' THEN 'Pending' WHEN t1.Chk_Status='A' THEN 'Approved' WHEN t1.Chk_Status='B' THEN 'Bounce' ELSE '' END AS CHK_Status  
              ,t1.ChequeNo           
          FROM [SupplierPayment] t1 inner join PartyInfo t2 on t2.ID=t1.supplier_id  " + Parameter;
            }
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Supplier");
            return dt;
        }

        public static void DeteleSupplierPayment(SupplierPayment SP)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection connection = new SqlConnection(DataManager.OraConnString());
            SqlTransaction transaction;
            try
            {
                connection.Open();

                transaction = connection.BeginTransaction();
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.Transaction = transaction;

                command.CommandText = @"select t1.VCH_SYS_NO  from [GL_TRANS_MST] t1 where SERIAL_NO='" + SP.ID + "' and PAYEE='SP'";
                string VoucherID = command.ExecuteScalar().ToString();

                command.CommandText = @"DELETE FROM [GL_TRANS_MST]  WHERE SERIAL_NO='" + VoucherID + "'";
                command.ExecuteNonQuery();
                command.CommandText = @"DELETE FROM [GL_TRANS_DTL]  WHERE VCH_SYS_NO='" + VoucherID + "'";
                command.ExecuteNonQuery();

                command.CommandText = @"DELETE FROM [SupplierPayment] WHERE  ID='" + SP.ID + "'";
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

        public static DataTable GetShowCheckNubber(string ChkId)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = @"SELECT t1.[ID]
      ,convert(nvarchar,t1.[PmDate],103) AS PmDate
      ,t1.[purchase_id]
      ,t3.ContactName
      ,t3.Gl_CoaCode
      ,t1.[PayAmt]
      ,t1.[PayMethod]
      ,t1.[Bank_id]
      ,t1.[ChequeNo]
      ,convert(nvarchar,t1.[ChequeDate],103) AS [ChequeDate]      
  FROM [SupplierPayment] t1 inner join ItemPurchaseMst t2 on t2.ID=t1.[purchase_id] inner join Supplier t3 on t3.ID=t2.SupplierID where t1.[ChequeNo]='" + ChkId + "'";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "SupplierPayment");
            return dt;
        }

        public static DataTable GetShowChequeStatement(string Chk_Type, string StartDate, string EndDate, string SupplierID)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string Parameter = "";
            if (StartDate == "" && EndDate == "" && SupplierID == "0")
            {
                Parameter = "where t1.PayMethod='Q' AND t1.Chk_Status='" + Chk_Type + "' ";
            }
            else if (StartDate != "" && EndDate != "" && SupplierID == "0")
            { Parameter = "where t1.PayMethod='Q' AND t1.Chk_Status='" + Chk_Type + "' AND CONVERT(DATE,t1.PmDate,103) between CONVERT(DATE,'" + StartDate + "',103) and  CONVERT(DATE,'" + EndDate + "',103) "; }
            else if (StartDate != "" && EndDate != "" && SupplierID != "0")
            {
                Parameter = "where t1.PayMethod='Q' AND t1.Chk_Status='" + Chk_Type + "' AND t1.supplier_id='" + SupplierID + "' AND CONVERT(DATE,t1.PmDate,103) between CONVERT(DATE,'" + StartDate + "',103) and  CONVERT(DATE,'" + EndDate + "',103) ";
            }
            else if (StartDate == "" && EndDate == "" && SupplierID != "0")
            {
                Parameter = "where t1.PayMethod='Q' AND t1.Chk_Status='" + Chk_Type + "' AND t1.supplier_id='" + SupplierID + "' ";
            }
            string query = @"SELECT CONVERT(NVARCHAR,t1.[PmDate],103) AS [PmDate],t1.[supplier_id],t2.ContactName,t1.[PayAmt],t3.bank_name,t1.[ChequeNo],CONVERT(NVARCHAR,t1.[ChequeDate],103) AS ChequeDate,CASE WHEN  t1.[Chk_Status]='P' THEN 'Pending' WHEN  t1.[Chk_Status]='A' THEN 'Approved' WHEN  t1.[Chk_Status]='B' THEN 'Bounce' END AS Chk_Status,case when t4.ShiftmentID='' OR t4.ShiftmentID IS NULL then t4.GRN else t4.ShiftmentID end as [ShiftMent]
  FROM [SupplierPayment] t1 inner join Supplier t2 on t2.ID=t1.supplier_id inner join bank_info t3 on t3.bank_id=t1.Bank_id left join ItemPurchaseMst t4 on t4.ID=t1.purchase_id  " + Parameter;
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "SupplierPayment");
            return dt;
        }

        public static DataTable getSupRpt(string SupID)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string Query = @"SELECT t1.[ID]
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
                              ,t2.COUNTRY_DESC  
                                ,t3.Name as SuplierGroup
                              ,t1.[NIDImage]
                              ,t1.[CurrentImage]
                          FROM [Supplier] t1 LEFT join COUNTRY_INFO t2 on t2.COUNTRY_CODE=t1.[Country] LEFT join SupplierGroup t3 on t3.ID=t1.SupplierGroupID where t1.ID='" + SupID + "'";
            DataTable dt = DataManager.ExecuteQuery(connectionString, Query, "Supplier");
            return dt;

        }

        public DataTable GetPurchaseMst(string SearchID, int flag)
        {
            String connectionString = DataManager.OraConnString();
            string query = "";

            query = @"
SELECT [ID],[GRN],[POCode],[ReceivedDate],[ChallanNo],[ChallanDate],[Name],[Total],[SearchGrnNo]  FROM [dbo].[View_Search_Purchase_Mst] where Upper(SearchGrnNo)='" + SearchID.ToUpper() + "' ";
           

            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "party");
            return dt;
        }
    }
}