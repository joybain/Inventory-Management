using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Delve;

namespace sales
{
    public class clsClientInfoManager
    {
        public static DataTable GetClientInfos()
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "select client_id,client_name,national_id,address1,address2,phone,mobile,fax,email,url,status from client_info order by client_id";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "client_info");
            return dt;
        }
        public static DataTable GetClientInfosGrid(string Parameter)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "select a.*,c.COUNTRY_DESC from Customer a left join COUNTRY_INFO c on c.COUNTRY_CODE=a.Country "+Parameter+" order by ID DESC ";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Customer");
            return dt;
        }
      
        public static void CreateClientInfo(clsClientInfo ci, GlCoa glcoa, SegCoa segcoa)
        {
            String connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string variables = @" [Code],BranchId           
           ,[ContactName],[Email],[Mobile],[Phone],[Fax],[Address1],[Address2],[City] ,State,[PostalCode],[Country],[Active],[CreatedBy],[CreatedDate],[Gl_CoaCode],CommonCus,NationalID ";
          string values="'" + ci.Code + "','"+ci.BranchId+"','" + ci.CustomerName + "','" + ci.Email + "','" + ci.Mobile + "','" + ci.Phone + "','" +
                           ci.Fax + "','" + ci.Address1 + "','" + ci.Address2 + "','" + ci.City + "','"+ci.State+"','" +
                           ci.PostalCode + "','" + ci.Country + "','" + ci.Active + "','" + ci.LoginBy + "',GETDATE(),'" +
                           ci.GlCoa + "','" + ci.CommonCus + "','" + ci.NationalId + "' ";
            string query = "";
            if (ci.NIDImage != null)
            {
                if (ci.NIDImage.Length > 0)
                {
                    variables = variables + ",NIDImage";
                    values = values + ",@img1";
                }
            }

            if (ci.CurrentImage != null)
            {
                if (ci.CurrentImage.Length > 0)
                {
                    variables = variables + ",CurrentImage";
                    values = values + ",@img2";
                }
            }

            query = " INSERT INTO [Customer]  (" + variables + ")  values ( " + values + " )";

            SqlParameter img = new SqlParameter();
            img.SqlDbType = SqlDbType.VarBinary;
            img.ParameterName = "img1";
            if (ci.NIDImage != null)
            {
                img.Value = ci.NIDImage;
            }
            SqlParameter img2 = new SqlParameter();
            img2.SqlDbType = SqlDbType.VarBinary;
            img2.ParameterName = "img2";

            if (ci.CurrentImage != null)
            {
                img2.Value = ci.CurrentImage;
            } 
            using (SqlCommand cmnd = new SqlCommand(query, sqlCon))
            {
                    cmnd.Parameters.Add(img);
                    cmnd.Parameters.Add(img2);

                    if (ci.NIDImage == null)
                {
                    cmnd.Parameters.Remove(img);
                }
                else
                {
                    if (ci.NIDImage.Length == 0)
                    {
                        cmnd.Parameters.Remove(img);
                    }
                }
                if (ci.CurrentImage == null)
                {
                    cmnd.Parameters.Remove(img2);
                }
                else
                {
                    if (ci.CurrentImage.Length == 0)
                    {
                        cmnd.Parameters.Remove(img2);
                    }
                }
                    sqlCon.Open();
                    cmnd.ExecuteNonQuery();
            }
           
           // sqlCon.Close();
           query = "";
            // Seg Coa Set.............
            if (segcoa.ParentCode == "" || segcoa.ParentCode == null)
            {
                query = " insert into gl_seg_coa (seg_coa_code,lvl_code,seg_coa_desc,bud_allowed, " +
                        " post_allowed,acc_type,open_date,rootleaf,status,taxable,book_name,entry_user,entry_date) values ( " +
                        " '" + segcoa.GlSegCode + "',  '" + segcoa.LvlCode + "',  '" + segcoa.SegCoaDesc + "', " +
                        " '" + segcoa.BudAllowed + "',  '" + segcoa.PostAllowed + "', " +
                        "  '" + segcoa.AccType + "', convert(datetime,case '" + segcoa.OpenDate +
                        "' when '' then null else '" + segcoa.OpenDate + "' end,103),  '" + segcoa.RootLeaf + "', " +
                        "  '" + segcoa.Status + "',  '" + segcoa.Taxable + "',  '" + segcoa.BookName + "','" +
                        segcoa.EntryUser + "',convert(datetime,'" + segcoa.EntryDate + "',103))";
                SqlCommand comm1 = new SqlCommand(query, sqlCon);
                comm1.ExecuteNonQuery();
            }
            else
            {
                query = "";
                query = " insert into gl_seg_coa (seg_coa_code,lvl_code,seg_coa_desc,parent_code,bud_allowed, " +
                        " post_allowed,acc_type,open_date,rootleaf,status,taxable,book_name,entry_user,entry_date) values ( " +
                        " '" + segcoa.GlSegCode + "',  '" + segcoa.LvlCode + "',  '" + segcoa.SegCoaDesc + "', " +
                        "  '" + segcoa.ParentCode + "',  '" + segcoa.BudAllowed + "',  '" + segcoa.PostAllowed + "', " +
                        "  '" + segcoa.AccType + "', convert(datetime,case '" + segcoa.OpenDate +
                        "' when '' then null else '" + segcoa.OpenDate + "' end,103),  '" + segcoa.RootLeaf + "', " +
                        "  '" + segcoa.Status + "',  '" + segcoa.Taxable + "',  '" + segcoa.BookName + "','" +
                        segcoa.EntryUser + "',convert(datetime,'" + segcoa.EntryDate + "',103))";

                SqlCommand comm2 = new SqlCommand(query, sqlCon);
                comm2.ExecuteNonQuery();
            }
            // End Seg Coa........

            // GL Coa Set........
            query = "";
            query = "insert into gl_coa (book_name,gl_coa_code,coa_enabled,effective_from,effective_to, " +
                    " bud_allowed,post_allowed,taxable,acc_type,status,coa_desc,coa_curr_bal,coa_natural_code,ENTRY_USER,ENTRY_DATE) values ( " +
                    " '" + glcoa.BookName + "',  '" + glcoa.GlCoaCode + "',  '" + glcoa.CoaEnabled + "', " +
                    "  convert(datetime,case '" + glcoa.EffectiveFrom + "' when '' then null else '" +
                    glcoa.EffectiveFrom + "' end ,103), convert(datetime,case '" + glcoa.EffectiveTo +
                    "' when '' then null else '" + glcoa.EffectiveTo + "' end ,103), '" + glcoa.BudAllowed + "', " +
                    "  '" + glcoa.PostAllowed + "',  '" + glcoa.Taxable + "',  '" + glcoa.AccType + "', " +
                    "  '" + glcoa.Status + "',  '" + glcoa.CoaDesc.Replace("'", "") + "',  '" + glcoa.CoaCurrBal +
                    "', " +
                    "  '" + glcoa.CoaNaturalCode + "','" + glcoa.LoginBy + "',GETDATE()) ";
            SqlCommand comm3 = new SqlCommand(query, sqlCon);
            comm3.ExecuteNonQuery();
            // GL Coa End


        }

        public static void UpdateClientInfo(clsClientInfo ci)
        {
            SqlConnection connection = new SqlConnection(DataManager.OraConnString());
            SqlTransaction transaction;
            try
            {
                if (string.IsNullOrEmpty(ci.PessoRate))
                {
                    ci.PessoRate = "0";
                }
                connection.Open();
                transaction = connection.BeginTransaction();
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.Transaction = transaction;


                string variables = @" ContactName='" + ci.CustomerName + "',BranchId='"+ci.BranchId+"',City='" + ci.City + "',State='" + ci.State +
                                   "',NationalID='" +
                                   ci.NationalId + "', Address1= '" + ci.Address1 + "', Address2= '" + ci.Address2 +
                                   "', Phone= '" + ci.Phone + "', Mobile='" + ci.Mobile + "',Fax='" + ci.Fax +
                                   "', Active= '" + ci.Active + "' ,ModifiedBy='" + ci.LoginBy +
                                   "',ModifiedDate=GETDATE(),CommonCus='" + ci.CommonCus + "',Gl_CoaCode='" +
                                   ci.GlCoa + "',[Email]='" + ci.Email + "',PostalCode='" + ci.PostalCode +
                                   "',PessoRate='" + ci.PessoRate + "' ";


                if (ci.NIDImage != null)
                {
                    if (ci.NIDImage.Length > 0)
                    {
                        variables = variables + ",NIDImage=@img1";

                    }
                }
                if (ci.CurrentImage != null)
                {
                    if (ci.CurrentImage.Length > 0)
                    {
                        variables = variables + ",CurrentImage=@img2";

                    }
                }
                command.CommandText = " update Customer   SET  " + variables + "   where ID='" + ci.ID + "' ";
                SqlParameter img = new SqlParameter();
                img.SqlDbType = SqlDbType.VarBinary;
                img.ParameterName = "img1";
                if (ci.NIDImage != null)
                {
                    img.Value = ci.NIDImage;
                }
                SqlParameter img2 = new SqlParameter();
                img2.SqlDbType = SqlDbType.VarBinary;
                img2.ParameterName = "img2";

                if (ci.CurrentImage != null)
                {
                    img2.Value = ci.CurrentImage;
                }
                command.Parameters.Add(img);
                command.Parameters.Add(img2);
                if (ci.NIDImage == null)
                {
                    command.Parameters.Remove(img);
                }
                else
                {
                    if (ci.NIDImage.Length == 0)
                    {
                        command.Parameters.Remove(img);
                    }
                }
                if (ci.CurrentImage == null)
                {
                    command.Parameters.Remove(img2);
                }
                else
                {
                    if (ci.CurrentImage.Length == 0)
                    {
                        command.Parameters.Remove(img2);
                    }
                }
                command.ExecuteNonQuery();

                //*********** Auto Coa generate off **********//

                command.CommandText = @"UPDATE [GL_SEG_COA] SET [SEG_COA_DESC] ='Accounts Receivable from " +
                                      ci.CustomerName + "'  WHERE [SEG_COA_CODE]='" + ci.GlCoa + "'";
                command.ExecuteNonQuery();

                command.CommandText = @"UPDATE [GL_COA] SET [COA_DESC] ='" + ci.GlCoaDesc +
                                      "' where [GL_COA_CODE]='1-" + ci.GlCoa + "'";
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

        public static void DeleteClientInfo(clsClientInfo ci)
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

                command.CommandText = @"delete from Customer where ID='" + ci.ID + "'";
                command.ExecuteNonQuery();

                //*********** Auto Coa generate off **********//
                //command.CommandText = @"delete from GL_SEG_COA where SEG_COA_CODE='" + ci.GlCoa + "' ";
                //command.ExecuteNonQuery();

                //command.CommandText = @"delete from GL_COA where COA_NATURAL_CODE='" + ci.GlCoa + "' ";
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

        public static clsClientInfo GetClientInfo(string ci)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "select * from Customer where ID = '" + ci + "' ";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "client_info");
            sqlCon.Close();
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            return new clsClientInfo(dt.Rows[0]);
        }
        public static clsClientInfo GetBranchClientInfo(string ci,string BranchId)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "select * from Customer where ID = '" + ci + "' and BranchId='"+BranchId+"' ";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "client_info");
            sqlCon.Close();
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            return new clsClientInfo(dt.Rows[0]);
        }
        public static clsClientInfo GetClientInfoIdName(string ci)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "select client_id,client_name,national_id,address1,address2,phone,mobile,fax,email,url,status from client_info where upper(client_id + ' - '+client_name) = upper('" + ci + "') ";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Client");
            sqlCon.Close();
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            return new clsClientInfo(dt.Rows[0]);
        }
        public static clsClientInfo GetClientInfoPp(string ci,string pp)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "select client_id,client_name,national_id,address1,address2,phone,mobile,fax,email,url,status from client_info where client_id = '" + ci + "' or passport='"+pp+"'";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Client");
            sqlCon.Close();
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            return new clsClientInfo(dt.Rows[0]);
        }
        public static string getClientName(string cid)
        {
            String ConnectionString = DataManager.OraConnString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            string Query = "select client_name from client_info where client_id='" + cid + "'";
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

        public static DataTable GetCommonClient()
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "select ID,ContactName,Gl_CoaCode from Customer where CommonCus='1'";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Client");
            return dt;
        }
        // *********************** Customer Payment Receive ******************//
        public static DataTable GetShowSupplierOnPayment(string p)
        {
            String connectionString = DataManager.OraConnString();
            string query = @"SELECT ID,Code,[ContactName],Gl_CoaCode  FROM [Customer] where UPPER([Code]+'-'+[ContactName]+'-'+[Mobile])=UPPER('" + p + "') and Active='True'";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Customer");
            return dt;
        }

        public static clsClientPaymentRec GetShowSupplierPaymentInfo(string ID)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = @"SELECT t1.[ID],CONVERT(nvarchar,t1.[Date],103) AS [Date],t1.[Customer_id],t1.[PayAmt],t1.[PayMethod],t1.[Bank_id],t1.[ChequeNo],CONVERT(nvarchar,t1.[ChequeDate],103) AS ChequeDate,t1.Chk_Status,t1.Invoice AS[INV_ID],t2.InvoiceNo FROM [CustomerPaymentReceive] t1 left join [Order] t2 on t2.ID=t1.Invoice where t1.ID='" + ID + "'";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "CustomerPaymentReceive");
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            return new clsClientPaymentRec(dt.Rows[0]);
        }
        public static void SaveCustomerPaymentRecevie(clsClientPaymentRec CP)
        {
            String connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);

            string query = @"INSERT INTO [CustomerPaymentReceive]
           ([Date],[Customer_id],[PayAmt],[PayMethod],[Bank_id],[ChequeNo],[ChequeDate],[entry_by],[entry_date],Chk_Status,Payment_Type,Invoice)
     VALUES
           (CONVERT(DATE,'" + CP.Date + "',103),'" + CP.Customer_id + "','" + CP.PayAmt + "','" + CP.PaymentMethord + "','" + CP.BankId + "','" + CP.ChequeNo + "',CONVERT(DATE,'" + CP.ChequeDate + "',103),'" + CP.LoginBy + "',GETDATE(),'" + CP.Chk_Status + "','" + CP.P_Type + "','" + CP.INV_ID + "')";
            DataManager.ExecuteNonQuery(connectionString, query);
        }

        public static void UpdateCustomerPaymentRecevie(clsClientPaymentRec CP,VouchMst _aVouchMstDV)
        {
           SqlConnection connection = new SqlConnection(DataManager.OraConnString());
            SqlTransaction transaction;
            DataTable dtFixCode = VouchManager.GetAllFixGlCode("");
            try
            {
                connection.Open();

                transaction = connection.BeginTransaction();
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.Transaction = transaction;
                string query = @"UPDATE  [CustomerPaymentReceive] SET
           [Date]=CONVERT(DATE,'" + CP.Date + "',103),[Customer_id]='" + CP.Customer_id + "',[PayAmt]='" + CP.PayAmt + "',[PayMethod]='" + CP.PaymentMethord + "',[Bank_id]='" + CP.BankId + "',[ChequeNo]='" + CP.ChequeNo + "',[ChequeDate]=CONVERT(DATE,'" + CP.ChequeDate + "',103),[update_by]='" + CP.LoginBy + "',[update_date]=GETDATE(),Chk_Status='" + CP.Chk_Status + "',Invoice='" + CP.INV_ID + "' where ID='" + CP.ID + "' ";

                command.CommandText = query;
                command.ExecuteNonQuery();

                if (_aVouchMstDV.RefFileNo.Equals("New"))
                {
                    _aVouchMstDV.ControlAmt = CP.PayAmt.Replace("'", "").Replace(",", "");
                    command.CommandText = VouchManager.SaveVoucherMst(_aVouchMstDV, 1);
                    command.ExecuteNonQuery();
                    VouchDtl vdtlCR;
                    for (int j = 0; j < 2; j++)
                    {
                        if (j == 0)
                        {
                            //DataRow 
                            vdtlCR = new VouchDtl();
                            vdtlCR.VchSysNo = _aVouchMstDV.VchSysNo;
                            vdtlCR.ValueDate = CP.Date;
                            vdtlCR.LineNo = "1";
                            vdtlCR.GlCoaCode = "1-" + CP.CustomerCoa;
                            vdtlCR.Particulars = "Supplier Payment Received";
                            vdtlCR.AccType = VouchManager.getAccType("1-" + CP.CustomerCoa);
                            vdtlCR.AmountCr = "0";
                            vdtlCR.AmountDr = _aVouchMstDV.ControlAmt.Replace(",", "");
                            vdtlCR.Status = _aVouchMstDV.Status;
                            vdtlCR.BookName = _aVouchMstDV.BookName; //*********** Convert Rate ********//

                            vdtlCR.AUTHO_USER = _aVouchMstDV.EntryUser;
                            VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstDV, vdtlCR, command);
                        }
                        else if (j == 1)
                        {
                            vdtlCR = new VouchDtl();
                            vdtlCR.VchSysNo = _aVouchMstDV.VchSysNo;
                            vdtlCR.ValueDate = CP.Date;
                            vdtlCR.LineNo = "2";
                            if (string.IsNullOrEmpty(CP.BankId))
                            {
                                vdtlCR.GlCoaCode = "1-" + dtFixCode.Rows[0]["CashInHand_BD"].ToString(); //**** SalesCode *******//
                                vdtlCR.AccType =
                                    VouchManager.getAccType("1-" + dtFixCode.Rows[0]["CashInHand_BD"]
                                        .ToString()); //**** SalesCode *******//
                                vdtlCR.Particulars = dtFixCode.Rows[0]["CashName_BD"].ToString();
                            }
                            else
                            {
                                //vdtlCR.GlCoaCode = "1-" + aSales.BankCoaCode; //**** SalesCode *******//
                                //vdtlCR.AccType =
                                //    VouchManager.getAccType("1-" + aSales.BankCoaCode); //**** SalesCode *******//
                                //vdtlCR.Particulars = aSales.BankName;
                            }

                            vdtlCR.AmountDr = _aVouchMstDV.ControlAmt.Replace(",", "");
                            vdtlCR.AmountCr = "0";
                            vdtlCR.Status = _aVouchMstDV.Status;
                            vdtlCR.BookName = _aVouchMstDV.BookName;
                            vdtlCR.AUTHO_USER = _aVouchMstDV.EntryUser;
                            //*********** Convert Rate ********//
                            VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstDV, vdtlCR, command);
                        }
                    }
                }
                else
                {
                    _aVouchMstDV.ControlAmt = CP.PayAmt.Replace("'", "").Replace(",", "");
                    command.CommandText = VouchManager.SaveVoucherMst(_aVouchMstDV, 2);
                    command.ExecuteNonQuery();

                    command.CommandText = @"delete from gl_trans_dtl where vch_sys_no=convert(numeric,'" +
                                          _aVouchMstDV.VchSysNo + "')";
                    command.ExecuteNonQuery();
                    VouchDtl vdtlCR;

                    for (int j = 0; j < 2; j++)
                    {
                        if (j == 0)
                        {
                            //DataRow 
                            vdtlCR = new VouchDtl();
                            vdtlCR.VchSysNo = _aVouchMstDV.VchSysNo;
                            vdtlCR.ValueDate = CP.Date;
                            vdtlCR.LineNo = "1";
                            vdtlCR.GlCoaCode = "1-" + CP.CustomerCoa;
                            vdtlCR.Particulars = "Supplier Payment Received";
                            vdtlCR.AccType = VouchManager.getAccType("1-" + CP.CustomerCoa);
                            vdtlCR.AmountCr = "0";
                            vdtlCR.AmountDr = _aVouchMstDV.ControlAmt.Replace(",", "");
                            vdtlCR.Status = _aVouchMstDV.Status;
                            vdtlCR.BookName = _aVouchMstDV.BookName; //*********** Convert Rate ********//

                            vdtlCR.AUTHO_USER = _aVouchMstDV.EntryUser;
                            VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstDV, vdtlCR, command);
                        }
                        else if (j == 1)
                        {
                            vdtlCR = new VouchDtl();
                            vdtlCR.VchSysNo = _aVouchMstDV.VchSysNo;
                            vdtlCR.ValueDate = CP.Date;
                            vdtlCR.LineNo = "2";
                            if (string.IsNullOrEmpty(CP.BankId))
                            {
                                vdtlCR.GlCoaCode = "1-" + dtFixCode.Rows[0]["CashInHand_BD"].ToString(); //**** SalesCode *******//
                                vdtlCR.AccType =
                                    VouchManager.getAccType("1-" + dtFixCode.Rows[0]["CashInHand_BD"]
                                        .ToString()); //**** SalesCode *******//
                                vdtlCR.Particulars = dtFixCode.Rows[0]["CashName_BD"].ToString();
                            }
                            else
                            {
                                //vdtlCR.GlCoaCode = "1-" + aSales.BankCoaCode; //**** SalesCode *******//
                                //vdtlCR.AccType =
                                //    VouchManager.getAccType("1-" + aSales.BankCoaCode); //**** SalesCode *******//
                                //vdtlCR.Particulars = aSales.BankName;
                            }

                            vdtlCR.AmountCr = _aVouchMstDV.ControlAmt.Replace(",", "");
                            vdtlCR.AmountDr = "0";
                            vdtlCR.Status = _aVouchMstDV.Status;
                            vdtlCR.BookName = _aVouchMstDV.BookName;
                            vdtlCR.AUTHO_USER = _aVouchMstDV.EntryUser;
                            //*********** Convert Rate ********//
                            VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstDV, vdtlCR, command);
                        }

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

        public static DataTable GetShowCustomerHistory(string P,string P_Type)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string Parameter = "";
            if (P != "") { Parameter = "Where t1.Chk_Status not in ('A') and t1.Customer_id='" + P + "' and Payment_Type='" + P_Type + "' order by ID desc"; } else { Parameter = "Where  t1.Chk_Status not in ('A') and  Payment_Type='" + P_Type + "' order by ID desc"; }
            string query = @"SELECT top(50) t1.[ID]
              ,t2.Code
              ,t2.ContactName
              ,CONVERT(nvarchar,t1.[Date],103) AS PmDate           
              ,t1.[PayAmt] 
              ,t1.ChequeNo
              ,CASE WHEN t1.Chk_Status='P' THEN 'Pending' WHEN t1.Chk_Status='A' THEN 'Approved' WHEN t1.Chk_Status='B' THEN 'Bounce' ELSE '' END AS[Chk_Status]
          FROM [CustomerPaymentReceive] t1 inner join Customer t2 on t2.ID=t1.Customer_id " + Parameter;
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "CustomerPaymentReceive");
            return dt;
        }

        public static DataTable GetShowCheckNubber(string ChkId)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = @"SELECT t1.[ID]
      ,convert(nvarchar,t1.Date,103) AS PmDate
      ,t1.Invoice
      ,t1.Customer_id
      ,t3.ContactName
      ,t3.Gl_CoaCode
      ,t1.[PayAmt]
      ,t1.[PayMethod]
      ,t1.[Bank_id]
      ,t1.[ChequeNo]
      ,convert(nvarchar,t1.[ChequeDate],103) AS [ChequeDate]      
      ,t1.Chk_Status
  FROM CustomerPaymentReceive t1 inner join [Order] t2 on t2.ID=t1.Invoice inner join Customer t3 on t3.ID=t2.CustomerID where t1.[ChequeNo]='" + ChkId + "'";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "SupplierPayment");
            return dt;
        }
     
        public static void DeteleCustomerPayment(clsClientPaymentRec CP)
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

                command.CommandText = @"select t1.VCH_SYS_NO  from [GL_TRANS_MST] t1 where SERIAL_NO='" + CP.ID + "' and PAYEE='CP'";
                string VoucherID = command.ExecuteScalar().ToString();

                command.CommandText = @"DELETE FROM [GL_TRANS_MST]  WHERE SERIAL_NO='" + VoucherID + "'";
                command.ExecuteNonQuery();
                command.CommandText = @"DELETE FROM [GL_TRANS_DTL]  WHERE VCH_SYS_NO='" + VoucherID + "'";
                command.ExecuteNonQuery();

                command.CommandText = @"DELETE FROM [SupplierPayment] WHERE  ID='" + CP.ID + "'";
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

        public static int GetShowPaymentIDCustomer()
        {
            SqlConnection connection = new SqlConnection(DataManager.OraConnString());
            try
            {
                connection.Open();
                string Query = @"SELECT top(1)[ID]  FROM [CustomerPaymentReceive] order by [ID] desc";
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
            string query = @"SELECT CONVERT(NVARCHAR,t1.[PmDate],103) AS [PmDate],t1.[supplier_id],t2.ContactName,t1.[PayAmt],t3.bank_name,t1.[ChequeNo],CONVERT(NVARCHAR,t1.[ChequeDate],103) AS ChequeDate,CASE WHEN  t1.[Chk_Status]='P' THEN 'Pending' WHEN  t1.[Chk_Status]='A' THEN 'Approved' WHEN  t1.[Chk_Status]='B' THEN 'Bounce' END AS Chk_Status    
  FROM [SupplierPayment] t1 inner join Supplier t2 on t2.ID=t1.supplier_id inner join bank_info t3 on t3.bank_id=t1.Bank_id  " + Parameter;
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "SupplierPayment");
            return dt;
        }

        public static DataTable GetShowPartyInfo(string p)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = @"SELECT  tt.ID,tt.ContactName FROM [Customer] tt where UPPER([ContactName]+' - '+[Mobile]) LIKE UPPER('%" + p + "%')";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "SupplierPayment");
            return dt;
        }


        public DataTable GetCustomerOnSearch(string SearchParameter,int flag)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "";
            if (flag.Equals(0))
            {
                query = @"SELECT [ID],[ContactName],[Gl_CoaCode],[SearchName], Address1, Address2 FROM [View_SearchCustomer] " +
                               SearchParameter;
            }
            if (flag.Equals(5))
            {
                query = @"SELECT [ID],[ContactName],[Gl_CoaCode],[SearchName], Address1, Address2 FROM [View_SearchCustomer] where CommonCus=1  "+SearchParameter;
            }
            else
            {
                query = @"SELECT [ID],[ContactName],[Gl_CoaCode],[SearchName], Address1, Address2 FROM [View_SearchCustomer] " +
                        SearchParameter;
            }
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "View_SearchCustomer");
            return dt;
        }


      

        public double GetDueAmount(string CustomerID)
        {
          //return  IdManager.GetShowSingleValueCurrency(
          //      "select tab.DueAmt-isnull((SELECT isnull(t1.[PayAmt],0) FROM [dbo].[CustomerPaymentReceive] t1 where t1.[Customer_id]='" +
          //      CustomerID +
          //      "' and Invoice IS NULL),0) from (SELECT isnull(t1.[PayAmt],0) AS [DueAmt] FROM [dbo].[CustomerPaymentReceive] t1 where t1.[Customer_id]='" +
          //      CustomerID + "' and Invoice IS NOT NULL) tab");


            return IdManager.GetShowSingleValueCurrency(
                  "Select SUM(isnull(Due,0)-isnull(Payment,0)) as Due  from View_GetCustomerPaymentDue  where CustomerID='" + CustomerID + "'  group by CustomerID ");
        }


        public double GetBranchDueAmount(string CustomerID,string BranchId)
        {
            //return  IdManager.GetShowSingleValueCurrency(
            //      "select tab.DueAmt-isnull((SELECT isnull(t1.[PayAmt],0) FROM [dbo].[CustomerPaymentReceive] t1 where t1.[Customer_id]='" +
            //      CustomerID +
            //      "' and Invoice IS NULL),0) from (SELECT isnull(t1.[PayAmt],0) AS [DueAmt] FROM [dbo].[CustomerPaymentReceive] t1 where t1.[Customer_id]='" +
            //      CustomerID + "' and Invoice IS NOT NULL) tab");


            return IdManager.GetShowSingleValueCurrency(
                "Select SUM(isnull(Due,0)-isnull(Payment,0)) as Due  from View_GetCustomerPaymentDue  where CustomerID='" + CustomerID + "' and BranchId='"+BranchId+"'  group by CustomerID ");
        }


        public double GetDueAmountSupplier(string CustomerID)
        {
           
            return IdManager.GetShowSingleValueCurrency(
                  "Select SUM(isnull(Due,0)-isnull(Payment,0)) as Due  from [dbo].[View_GetSupplierPaymentDue]  where [SupplierID]='" + CustomerID + "'  group by SupplierID ");
        }


        public DataTable GetPaymentDetails(string ID)
        {
            string parameter = "";
            if (!string.IsNullOrEmpty(ID))
            {
                parameter = " where [ID]='" + ID + "' ";
            }

            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);

            string query =
                @"SELECT top(100) [ID],[Date],[Customer_id],[ContactName],[Gl_CoaCode] ,[SearchName],[Mobile],[Phone],[Address1],[Invoice],[PayAmt],[PayMethod],[Bank_id],[ChequeNo],[ChequeDate],[Chk_Status]
      ,[Payment_Type],[Remarks]
  FROM [dbo].[View_Search_Customer_Payment] " + parameter + " order by id desc"; 
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "View_Search_Customer_Payment");
            return dt;
        }
        public DataTable GetBranchPaymentDetails(string ID,string BranchId)
        {
            string parameter = "where  BranchId='" + BranchId + "'";
            if (!string.IsNullOrEmpty(ID))
            {
                parameter =parameter+ "  and   [ID]='" + ID + "' ";
            }
            

            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);

            string query =
                @"SELECT top(100) [ID],[Date],[Customer_id],[ContactName],[Gl_CoaCode] ,[SearchName],[Mobile],[Phone],[Address1],[Invoice],[PayAmt],[PayMethod],[Bank_id],[ChequeNo],[ChequeDate],[Chk_Status]
      ,[Payment_Type],[Remarks]
  FROM [dbo].[View_Search_Customer_Payment] " + parameter + " order by id desc";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "View_Search_Customer_Payment");
            return dt;
        }

        public DataTable GetPaymentDetailsSupplier(string ID)
        {
            string parameter = "";
            if (!string.IsNullOrEmpty(ID))
            {
                parameter = " where [ID]='" + ID + "' ";
            }

            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);

            string query =
                @" SELECT top(100) [ID],[Date],[Supplier_id],[ContactName],[Gl_CoaCode] ,[SearchName],[Mobile],[Phone],[Address1],GRN,[PayAmt],[PayMethod],[Bank_id],[ChequeNo],[ChequeDate],[Chk_Status]
      ,[Payment_Type],[Remarks]
  FROM [dbo].[View_SearchSupplierPayment] " + parameter + " order by id desc";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "View_SearchSupplierPayment");
            return dt;
        }
//        public DataTable GetPaymentDetailsSupplier(string ID)
//        {
//            string parameter = "";
//            if (!string.IsNullOrEmpty(ID))
//            {
//                parameter = " where [ID]='" + ID + "' ";
//            }

//            string connectionString = DataManager.OraConnString();
//            SqlConnection sqlCon = new SqlConnection(connectionString);
//            string query =
//               @"SELECT top(100) [ID],[Date],[Supplier_id],[ContactName],[Gl_CoaCode] ,GRN,[SearchName],[Mobile],[Phone],[Address1],[PurchaseVoucherID],[PayAmt],[PayMethod],[Bank_id],[ChequeNo],[ChequeDate],[Chk_Status]
//      ,[Payment_Type],[Remarks]
//  FROM [dbo].[View_SearchSupplierPayment] " + parameter + " order by id desc";
          
//            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "View_Search_Customer_Payment");
//            return dt;
//        }
        public void SaveCustomerPayment(clsClientPaymentRec _aclsClientPaymentRec, VouchMst _aVouchMstCR)
        {
            SqlConnection connection = new SqlConnection(DataManager.OraConnString());
            SqlTransaction transaction;
            DataTable dtFixCode = VouchManager.GetAllFixGlCode("");
            connection.Open();
            transaction = connection.BeginTransaction();
            try
            {

                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.Transaction = transaction;

                string Query = @"INSERT INTO [dbo].[CustomerPaymentReceive]
                ([Date],[Customer_id],[PayAmt],[entry_by],[entry_date],[Remarks])
            VALUES
                (convert(date,'" + _aclsClientPaymentRec.Date + "'+' '+ substring(CONVERT(VARCHAR, GETDATE(), 108),0,6),103),'" + _aclsClientPaymentRec.Customer_id +
                             "','" + _aclsClientPaymentRec.PayAmt.Replace(",", "") + "','" +
                             _aclsClientPaymentRec.LoginBy + "',GETDATE(),'" +
                             _aclsClientPaymentRec.Remarks.Replace(",", "") + "')";
                command.CommandText = Query;
                command.ExecuteNonQuery();

                //***************************  Credit Voucher ********************************// 
                command.CommandText = @"SELECT top(1) [ID]  FROM [CustomerPaymentReceive] order by ID desc";
                string MstID = command.ExecuteScalar().ToString();

                //***************************  Devid Voucher ********************************// 
                if (Convert.ToDecimal(_aclsClientPaymentRec.PayAmt) > 0)
                {
                    _aVouchMstCR.ControlAmt = _aclsClientPaymentRec.PayAmt.Replace("'", "").Replace(",", "");
                    _aVouchMstCR.SerialNo = MstID;
                      command.CommandText = VouchManager.SaveVoucherMst(_aVouchMstCR, 1);
                    command.ExecuteNonQuery();
                    VouchDtl vdtlCR;
                    for (int j = 0; j < 2; j++)
                    {
                        if (j == 0)
                        {
                            //DataRow 
                            vdtlCR = new VouchDtl();
                            vdtlCR.VchSysNo = _aVouchMstCR.VchSysNo;
                            vdtlCR.ValueDate = _aclsClientPaymentRec.Date;
                            vdtlCR.LineNo = "1";
                            vdtlCR.GlCoaCode = "1-" + _aclsClientPaymentRec.CustomerCoa;
                            vdtlCR.Particulars = "Customer Payment Received";
                            vdtlCR.AccType = VouchManager.getAccType("1-" + _aclsClientPaymentRec.CustomerCoa);
                            vdtlCR.AmountDr = "0";
                            vdtlCR.AmountCr = _aVouchMstCR.ControlAmt.Replace(",", "");
                            vdtlCR.Status = _aVouchMstCR.Status;
                            vdtlCR.BookName = _aVouchMstCR.BookName; //*********** Convert Rate ********//

                            vdtlCR.AUTHO_USER = _aVouchMstCR.EntryUser;
                            VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstCR, vdtlCR, command);
                        }
                        else if (j == 1)
                        {
                            vdtlCR = new VouchDtl();
                            vdtlCR.VchSysNo = _aVouchMstCR.VchSysNo;
                            vdtlCR.ValueDate = _aclsClientPaymentRec.Date;
                            vdtlCR.LineNo = "2";
                            if (string.IsNullOrEmpty(_aclsClientPaymentRec.BankId))
                            {
                                vdtlCR.GlCoaCode = "1-" + dtFixCode.Rows[0]["CashInHand_BD"].ToString(); //**** SalesCode *******//
                                vdtlCR.AccType =
                                    VouchManager.getAccType("1-" + dtFixCode.Rows[0]["CashInHand_BD"]
                                        .ToString()); //**** SalesCode *******//
                                vdtlCR.Particulars = dtFixCode.Rows[0]["CashName_BD"].ToString();
                            }
                            else
                            {
                                //vdtlCR.GlCoaCode = "1-" + aSales.BankCoaCode; //**** SalesCode *******//
                                //vdtlCR.AccType =
                                //    VouchManager.getAccType("1-" + aSales.BankCoaCode); //**** SalesCode *******//
                                //vdtlCR.Particulars = aSales.BankName;
                            }

                            vdtlCR.AmountDr = _aVouchMstCR.ControlAmt.Replace(",", "");
                            vdtlCR.AmountCr = "0";
                            vdtlCR.Status = _aVouchMstCR.Status;
                            vdtlCR.BookName = _aVouchMstCR.BookName;
                            vdtlCR.AUTHO_USER = _aVouchMstCR.EntryUser;
                            //*********** Convert Rate ********//
                            VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstCR, vdtlCR, command);
                        }
                    }
                }
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception(ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }


        public void SaveBranchCustomerPayment(clsClientPaymentRec _aclsClientPaymentRec, VouchMst _aVouchMstCR)
        {
            SqlConnection connection = new SqlConnection(DataManager.OraConnString());
            SqlTransaction transaction;
            DataTable dtFixCode = VouchManager.GetAllFixGlCode("");
            connection.Open();
            transaction = connection.BeginTransaction();
            try
            {

                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.Transaction = transaction;

                string Query = @"INSERT INTO [dbo].[CustomerPaymentReceive]
                (BranchId,[Date],[Customer_id],[PayAmt],[entry_by],[entry_date],[Remarks])
            VALUES
                ('"+_aclsClientPaymentRec.BranchId+"',convert(date,'" + _aclsClientPaymentRec.Date + "'+' '+ substring(CONVERT(VARCHAR, GETDATE(), 108),0,6),103),'" + _aclsClientPaymentRec.Customer_id +
                             "','" + _aclsClientPaymentRec.PayAmt.Replace(",", "") + "','" +
                             _aclsClientPaymentRec.LoginBy + "',GETDATE(),'" +
                             _aclsClientPaymentRec.Remarks.Replace(",", "") + "')";
                command.CommandText = Query;
                command.ExecuteNonQuery();

                //***************************  Credit Voucher ********************************// 
                command.CommandText = @"SELECT top(1) [ID]  FROM [CustomerPaymentReceive] where BranchId='"+_aclsClientPaymentRec.BranchId+"' order by ID desc";
                string MstID = command.ExecuteScalar().ToString();

                //***************************  Devid Voucher ********************************// 
                if (Convert.ToDecimal(_aclsClientPaymentRec.PayAmt) > 0)
                {
                    _aVouchMstCR.ControlAmt = _aclsClientPaymentRec.PayAmt.Replace("'", "").Replace(",", "");
                    _aVouchMstCR.SerialNo = MstID;
                    command.CommandText = VouchManager.SaveVoucherMst(_aVouchMstCR, 1);
                    command.ExecuteNonQuery();
                    VouchDtl vdtlCR;
                    for (int j = 0; j < 2; j++)
                    {
                        if (j == 0)
                        {
                            //DataRow 
                            vdtlCR = new VouchDtl();
                            vdtlCR.VchSysNo = _aVouchMstCR.VchSysNo;
                            vdtlCR.ValueDate = _aclsClientPaymentRec.Date;
                            vdtlCR.LineNo = "1";
                            vdtlCR.GlCoaCode = "1-" + _aclsClientPaymentRec.CustomerCoa;
                            vdtlCR.Particulars = "Customer Payment Received";
                            vdtlCR.AccType = VouchManager.getAccType("1-" + _aclsClientPaymentRec.CustomerCoa);
                            vdtlCR.AmountDr = "0";
                            vdtlCR.AmountCr = _aVouchMstCR.ControlAmt.Replace(",", "");
                            vdtlCR.Status = _aVouchMstCR.Status;
                            vdtlCR.BookName = _aVouchMstCR.BookName; //*********** Convert Rate ********//

                            vdtlCR.AUTHO_USER = _aVouchMstCR.EntryUser;
                            VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstCR, vdtlCR, command);
                        }
                        else if (j == 1)
                        {
                            vdtlCR = new VouchDtl();
                            vdtlCR.VchSysNo = _aVouchMstCR.VchSysNo;
                            vdtlCR.ValueDate = _aclsClientPaymentRec.Date;
                            vdtlCR.LineNo = "2";
                            if (string.IsNullOrEmpty(_aclsClientPaymentRec.BankId))
                            {
                                vdtlCR.GlCoaCode = "1-" + dtFixCode.Rows[0]["CashInHand_BD"].ToString(); //**** SalesCode *******//
                                vdtlCR.AccType =
                                    VouchManager.getAccType("1-" + dtFixCode.Rows[0]["CashInHand_BD"]
                                        .ToString()); //**** SalesCode *******//
                                vdtlCR.Particulars = dtFixCode.Rows[0]["CashName_BD"].ToString();
                            }
                            else
                            {
                                //vdtlCR.GlCoaCode = "1-" + aSales.BankCoaCode; //**** SalesCode *******//
                                //vdtlCR.AccType =
                                //    VouchManager.getAccType("1-" + aSales.BankCoaCode); //**** SalesCode *******//
                                //vdtlCR.Particulars = aSales.BankName;
                            }

                            vdtlCR.AmountDr = _aVouchMstCR.ControlAmt.Replace(",", "");
                            vdtlCR.AmountCr = "0";
                            vdtlCR.Status = _aVouchMstCR.Status;
                            vdtlCR.BookName = _aVouchMstCR.BookName;
                            vdtlCR.AUTHO_USER = _aVouchMstCR.EntryUser;
                            //*********** Convert Rate ********//
                            VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstCR, vdtlCR, command);
                        }
                    }
                }
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception(ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }



        public void SaveSupplierPayment(clsSupplierPaymentRec _aclsClientPaymentRec, VouchMst _aVouchMstCR)
        {
            SqlConnection connection = new SqlConnection(DataManager.OraConnString());
            SqlTransaction transaction;
            DataTable dtFixCode = VouchManager.GetAllFixGlCode("");
            connection.Open();
            transaction = connection.BeginTransaction();
            try
            {

                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.Transaction = transaction;

                string Query = @"INSERT INTO [dbo].[SupplierPaymentReceive]
                ([Date],[Supplier_id],[PayAmt],[entry_by],[entry_date],[Remarks])
            VALUES
                (convert(date,'" + _aclsClientPaymentRec.Date + "'+' '+ substring(CONVERT(VARCHAR, GETDATE(), 108),0,6),103),'" + _aclsClientPaymentRec.Supplier_id +
                             "','" + _aclsClientPaymentRec.PayAmt.Replace(",", "") + "','" +
                             _aclsClientPaymentRec.LoginBy + "',GETDATE(),'" +
                             _aclsClientPaymentRec.Remarks.Replace(",", "") + "')";
                command.CommandText = Query;
                command.ExecuteNonQuery();

                command.CommandText = @"SELECT top(1) [ID]  FROM [SupplierPaymentReceive] order by ID desc";
              string MstID = command.ExecuteScalar().ToString();

                //***************************  Devid Voucher ********************************// 
                if (Convert.ToDecimal(_aclsClientPaymentRec.PayAmt) > 0)
                {
                    _aVouchMstCR.ControlAmt = _aclsClientPaymentRec.PayAmt.Replace("'", "").Replace(",", "");
                    _aVouchMstCR.SerialNo = MstID;
                    command.CommandText = VouchManager.SaveVoucherMst(_aVouchMstCR, 1);
                    command.ExecuteNonQuery();
                    VouchDtl vdtlCR;
                    for (int j = 0; j < 2; j++)
                    {
                        if (j == 0)
                        {
                            //DataRow 
                            vdtlCR = new VouchDtl();
                            vdtlCR.VchSysNo = _aVouchMstCR.VchSysNo;
                            vdtlCR.ValueDate = _aclsClientPaymentRec.Date;
                            vdtlCR.LineNo = "1";
                            vdtlCR.GlCoaCode = "1-" + _aclsClientPaymentRec.SupplierCoa;
                            vdtlCR.Particulars = "Supplier Payment Received";
                            vdtlCR.AccType = VouchManager.getAccType("1-" + _aclsClientPaymentRec.SupplierCoa);
                            vdtlCR.AmountCr = "0";
                            vdtlCR.AmountDr = _aVouchMstCR.ControlAmt.Replace(",", "");
                            vdtlCR.Status = _aVouchMstCR.Status;
                            vdtlCR.BookName = _aVouchMstCR.BookName; //*********** Convert Rate ********//

                            vdtlCR.AUTHO_USER = _aVouchMstCR.EntryUser;
                            VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstCR, vdtlCR, command);
                        }
                        else if (j == 1)
                        {
                            vdtlCR = new VouchDtl();
                            vdtlCR.VchSysNo = _aVouchMstCR.VchSysNo;
                            vdtlCR.ValueDate = _aclsClientPaymentRec.Date;
                            vdtlCR.LineNo = "2";
                            if (string.IsNullOrEmpty(_aclsClientPaymentRec.BankId))
                            {
                                vdtlCR.GlCoaCode = "1-" + dtFixCode.Rows[0]["CashInHand_BD"].ToString(); //**** SalesCode *******//
                                vdtlCR.AccType =
                                    VouchManager.getAccType("1-" + dtFixCode.Rows[0]["CashInHand_BD"]
                                        .ToString()); //**** SalesCode *******//
                                vdtlCR.Particulars = dtFixCode.Rows[0]["CashName_BD"].ToString();
                            }
                            else
                            {
                                //vdtlCR.GlCoaCode = "1-" + aSales.BankCoaCode; //**** SalesCode *******//
                                //vdtlCR.AccType =
                                //    VouchManager.getAccType("1-" + aSales.BankCoaCode); //**** SalesCode *******//
                                //vdtlCR.Particulars = aSales.BankName;
                            }

                            vdtlCR.AmountCr = _aVouchMstCR.ControlAmt.Replace(",", "");
                            vdtlCR.AmountDr = "0";
                            vdtlCR.Status = _aVouchMstCR.Status;
                            vdtlCR.BookName = _aVouchMstCR.BookName;
                            vdtlCR.AUTHO_USER = _aVouchMstCR.EntryUser;
                            //*********** Convert Rate ********//
                            VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstCR, vdtlCR, command);
                        }
                    }
                }
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception(ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }


        public void UpdateCustomerPayment(clsClientPaymentRec _aclsClientPaymentRec, VouchMst _aVouchMstDV)
        {
            SqlConnection connection = new SqlConnection(DataManager.OraConnString());
            SqlTransaction transaction;
            DataTable dtFixCode = VouchManager.GetAllFixGlCode("");
            try
            {
                connection.Open();

                transaction = connection.BeginTransaction();
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.Transaction = transaction;
            string Query = @"UPDATE [dbo].[CustomerPaymentReceive]
             SET [Date] = convert(date,'" + _aclsClientPaymentRec.Date + "'+' '+ substring(CONVERT(VARCHAR, GETDATE(), 108),0,6),103),[Customer_id] = '" +
                           _aclsClientPaymentRec.Customer_id +
                           "',[PayAmt] ='" + _aclsClientPaymentRec.PayAmt.Replace(",", "") + "' ,[update_by] ='" +
                           _aclsClientPaymentRec.LoginBy + "' ,[update_date] =GETDATE(),[Remarks] = '" +
                           _aclsClientPaymentRec.Remarks.Replace(",", "") + "' WHERE ID='" +
                           _aclsClientPaymentRec.ID + "'";
            command.CommandText = Query;
            command.ExecuteNonQuery();

            if (_aVouchMstDV.RefFileNo.Equals("New"))
            {
                _aVouchMstDV.ControlAmt = _aclsClientPaymentRec.PayAmt.Replace("'", "").Replace(",", "");
                command.CommandText = VouchManager.SaveVoucherMst(_aVouchMstDV, 1);
                command.ExecuteNonQuery();
                VouchDtl vdtlCR;
                for (int j = 0; j < 2; j++)
                {
                    if (j == 0)
                    {
                        //DataRow 
                        vdtlCR = new VouchDtl();
                        vdtlCR.VchSysNo = _aVouchMstDV.VchSysNo;
                        vdtlCR.ValueDate = _aclsClientPaymentRec.Date;
                        vdtlCR.LineNo = "1";
                        vdtlCR.GlCoaCode = "1-" + _aclsClientPaymentRec.CustomerCoa;
                        vdtlCR.Particulars = "Customer Payment Received";
                        vdtlCR.AccType = VouchManager.getAccType("1-" + _aclsClientPaymentRec.CustomerCoa);
                        vdtlCR.AmountDr = "0";
                        vdtlCR.AmountCr = _aVouchMstDV.ControlAmt.Replace(",", "");
                        vdtlCR.Status = _aVouchMstDV.Status;
                        vdtlCR.BookName = _aVouchMstDV.BookName; //*********** Convert Rate ********//

                        vdtlCR.AUTHO_USER = _aVouchMstDV.EntryUser;
                        VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstDV, vdtlCR, command);
                    }
                    else if (j == 1)
                    {
                        vdtlCR = new VouchDtl();
                        vdtlCR.VchSysNo = _aVouchMstDV.VchSysNo;
                        vdtlCR.ValueDate = _aclsClientPaymentRec.Date;
                        vdtlCR.LineNo = "2";
                        if (string.IsNullOrEmpty(_aclsClientPaymentRec.BankId))
                        {
                            vdtlCR.GlCoaCode = "1-" + dtFixCode.Rows[0]["CashInHand_BD"].ToString(); //**** SalesCode *******//
                            vdtlCR.AccType =
                                VouchManager.getAccType("1-" + dtFixCode.Rows[0]["CashInHand_BD"]
                                    .ToString()); //**** SalesCode *******//
                            vdtlCR.Particulars = dtFixCode.Rows[0]["CashName_BD"].ToString();
                        }
                        else
                        {
                            //vdtlCR.GlCoaCode = "1-" + aSales.BankCoaCode; //**** SalesCode *******//
                            //vdtlCR.AccType =
                            //    VouchManager.getAccType("1-" + aSales.BankCoaCode); //**** SalesCode *******//
                            //vdtlCR.Particulars = aSales.BankName;
                        }

                        vdtlCR.AmountDr = _aVouchMstDV.ControlAmt.Replace(",", "");
                        vdtlCR.AmountCr = "0";
                        vdtlCR.Status = _aVouchMstDV.Status;
                        vdtlCR.BookName = _aVouchMstDV.BookName;
                        vdtlCR.AUTHO_USER = _aVouchMstDV.EntryUser;
                        //*********** Convert Rate ********//
                        VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstDV, vdtlCR, command);
                    }
                }
            }
            else
            {
                _aVouchMstDV.ControlAmt = _aclsClientPaymentRec.PayAmt.Replace("'", "").Replace(",", "");
                command.CommandText = VouchManager.SaveVoucherMst(_aVouchMstDV, 2);
                command.ExecuteNonQuery();

                command.CommandText = @"delete from gl_trans_dtl where vch_sys_no=convert(numeric,'" +
                                      _aVouchMstDV.VchSysNo + "')";
                command.ExecuteNonQuery();
                VouchDtl vdtlCR;

                for (int j = 0; j < 2; j++)
                {
                    if (j == 0)
                    {
                        //DataRow 
                        vdtlCR = new VouchDtl();
                        vdtlCR.VchSysNo = _aVouchMstDV.VchSysNo;
                        vdtlCR.ValueDate = _aclsClientPaymentRec.Date;
                        vdtlCR.LineNo = "1";
                        vdtlCR.GlCoaCode = "1-" + _aclsClientPaymentRec.CustomerCoa;
                        vdtlCR.Particulars = "Customer Payment Received";
                        vdtlCR.AccType = VouchManager.getAccType("1-" + _aclsClientPaymentRec.CustomerCoa);
                        vdtlCR.AmountDr = "0";
                        vdtlCR.AmountCr = _aVouchMstDV.ControlAmt.Replace(",", "");
                        vdtlCR.Status = _aVouchMstDV.Status;
                        vdtlCR.BookName = _aVouchMstDV.BookName; //*********** Convert Rate ********//

                        vdtlCR.AUTHO_USER = _aVouchMstDV.EntryUser;
                        VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstDV, vdtlCR, command);
                    }
                    else if (j == 1)
                    {
                        vdtlCR = new VouchDtl();
                        vdtlCR.VchSysNo = _aVouchMstDV.VchSysNo;
                        vdtlCR.ValueDate = _aclsClientPaymentRec.Date;
                        vdtlCR.LineNo = "2";
                        if (string.IsNullOrEmpty(_aclsClientPaymentRec.BankId))
                        {
                            vdtlCR.GlCoaCode = "1-" + dtFixCode.Rows[0]["CashInHand_BD"].ToString(); //**** SalesCode *******//
                            vdtlCR.AccType =
                                VouchManager.getAccType("1-" + dtFixCode.Rows[0]["CashInHand_BD"]
                                    .ToString()); //**** SalesCode *******//
                            vdtlCR.Particulars = dtFixCode.Rows[0]["CashName_BD"].ToString();
                        }
                        else
                        {
                            //vdtlCR.GlCoaCode = "1-" + aSales.BankCoaCode; //**** SalesCode *******//
                            //vdtlCR.AccType =
                            //    VouchManager.getAccType("1-" + aSales.BankCoaCode); //**** SalesCode *******//
                            //vdtlCR.Particulars = aSales.BankName;
                        }

                        vdtlCR.AmountCr = _aVouchMstDV.ControlAmt.Replace(",", "");
                        vdtlCR.AmountDr = "0";
                        vdtlCR.Status = _aVouchMstDV.Status;
                        vdtlCR.BookName = _aVouchMstDV.BookName;
                        vdtlCR.AUTHO_USER = _aVouchMstDV.EntryUser;
                        //*********** Convert Rate ********//
                        VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstDV, vdtlCR, command);
                    }
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


        public void UpdateBranchCustomerPayment(clsClientPaymentRec _aclsClientPaymentRec, VouchMst _aVouchMstDV)
        {
            SqlConnection connection = new SqlConnection(DataManager.OraConnString());
            SqlTransaction transaction;
            DataTable dtFixCode = VouchManager.GetAllFixGlCode("");
            try
            {
                connection.Open();

                transaction = connection.BeginTransaction();
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.Transaction = transaction;
                string Query = @"UPDATE [dbo].[CustomerPaymentReceive]
             SET [Date] = convert(date,'" + _aclsClientPaymentRec.Date + "'+' '+ substring(CONVERT(VARCHAR, GETDATE(), 108),0,6),103),[Customer_id] = '" +
                               _aclsClientPaymentRec.Customer_id +
                               "',[PayAmt] ='" + _aclsClientPaymentRec.PayAmt.Replace(",", "") + "' ,[update_by] ='" +
                               _aclsClientPaymentRec.LoginBy + "' ,[update_date] =GETDATE(),[Remarks] = '" +
                               _aclsClientPaymentRec.Remarks.Replace(",", "") + "' WHERE ID='" +
                               _aclsClientPaymentRec.ID + "' and BranchId='"+_aclsClientPaymentRec.BranchId+"'";
                command.CommandText = Query;
                command.ExecuteNonQuery();

                if (_aVouchMstDV.RefFileNo.Equals("New"))
                {
                    _aVouchMstDV.ControlAmt = _aclsClientPaymentRec.PayAmt.Replace("'", "").Replace(",", "");
                    command.CommandText = VouchManager.SaveVoucherMst(_aVouchMstDV, 1);
                    command.ExecuteNonQuery();
                    VouchDtl vdtlCR;
                    for (int j = 0; j < 2; j++)
                    {
                        if (j == 0)
                        {
                            //DataRow 
                            vdtlCR = new VouchDtl();
                            vdtlCR.VchSysNo = _aVouchMstDV.VchSysNo;
                            vdtlCR.ValueDate = _aclsClientPaymentRec.Date;
                            vdtlCR.LineNo = "1";
                            vdtlCR.GlCoaCode = "1-" + _aclsClientPaymentRec.CustomerCoa;
                            vdtlCR.Particulars = "Customer Payment Received";
                            vdtlCR.AccType = VouchManager.getAccType("1-" + _aclsClientPaymentRec.CustomerCoa);
                            vdtlCR.AmountDr = "0";
                            vdtlCR.AmountCr = _aVouchMstDV.ControlAmt.Replace(",", "");
                            vdtlCR.Status = _aVouchMstDV.Status;
                            vdtlCR.BookName = _aVouchMstDV.BookName; //*********** Convert Rate ********//

                            vdtlCR.AUTHO_USER = _aVouchMstDV.EntryUser;
                            VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstDV, vdtlCR, command);
                        }
                        else if (j == 1)
                        {
                            vdtlCR = new VouchDtl();
                            vdtlCR.VchSysNo = _aVouchMstDV.VchSysNo;
                            vdtlCR.ValueDate = _aclsClientPaymentRec.Date;
                            vdtlCR.LineNo = "2";
                            if (string.IsNullOrEmpty(_aclsClientPaymentRec.BankId))
                            {
                                vdtlCR.GlCoaCode = "1-" + dtFixCode.Rows[0]["CashInHand_BD"].ToString(); //**** SalesCode *******//
                                vdtlCR.AccType =
                                    VouchManager.getAccType("1-" + dtFixCode.Rows[0]["CashInHand_BD"]
                                        .ToString()); //**** SalesCode *******//
                                vdtlCR.Particulars = dtFixCode.Rows[0]["CashName_BD"].ToString();
                            }
                            else
                            {
                                //vdtlCR.GlCoaCode = "1-" + aSales.BankCoaCode; //**** SalesCode *******//
                                //vdtlCR.AccType =
                                //    VouchManager.getAccType("1-" + aSales.BankCoaCode); //**** SalesCode *******//
                                //vdtlCR.Particulars = aSales.BankName;
                            }

                            vdtlCR.AmountDr = _aVouchMstDV.ControlAmt.Replace(",", "");
                            vdtlCR.AmountCr = "0";
                            vdtlCR.Status = _aVouchMstDV.Status;
                            vdtlCR.BookName = _aVouchMstDV.BookName;
                            vdtlCR.AUTHO_USER = _aVouchMstDV.EntryUser;
                            //*********** Convert Rate ********//
                            VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstDV, vdtlCR, command);
                        }
                    }
                }
                else
                {
                    _aVouchMstDV.ControlAmt = _aclsClientPaymentRec.PayAmt.Replace("'", "").Replace(",", "");
                    command.CommandText = VouchManager.SaveVoucherMst(_aVouchMstDV, 2);
                    command.ExecuteNonQuery();

                    command.CommandText = @"delete from gl_trans_dtl where vch_sys_no=convert(numeric,'" +
                                          _aVouchMstDV.VchSysNo + "')";
                    command.ExecuteNonQuery();
                    VouchDtl vdtlCR;

                    for (int j = 0; j < 2; j++)
                    {
                        if (j == 0)
                        {
                            //DataRow 
                            vdtlCR = new VouchDtl();
                            vdtlCR.VchSysNo = _aVouchMstDV.VchSysNo;
                            vdtlCR.ValueDate = _aclsClientPaymentRec.Date;
                            vdtlCR.LineNo = "1";
                            vdtlCR.GlCoaCode = "1-" + _aclsClientPaymentRec.CustomerCoa;
                            vdtlCR.Particulars = "Customer Payment Received";
                            vdtlCR.AccType = VouchManager.getAccType("1-" + _aclsClientPaymentRec.CustomerCoa);
                            vdtlCR.AmountDr = "0";
                            vdtlCR.AmountCr = _aVouchMstDV.ControlAmt.Replace(",", "");
                            vdtlCR.Status = _aVouchMstDV.Status;
                            vdtlCR.BookName = _aVouchMstDV.BookName; //*********** Convert Rate ********//

                            vdtlCR.AUTHO_USER = _aVouchMstDV.EntryUser;
                            VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstDV, vdtlCR, command);
                        }
                        else if (j == 1)
                        {
                            vdtlCR = new VouchDtl();
                            vdtlCR.VchSysNo = _aVouchMstDV.VchSysNo;
                            vdtlCR.ValueDate = _aclsClientPaymentRec.Date;
                            vdtlCR.LineNo = "2";
                            if (string.IsNullOrEmpty(_aclsClientPaymentRec.BankId))
                            {
                                vdtlCR.GlCoaCode = "1-" + dtFixCode.Rows[0]["CashInHand_BD"].ToString(); //**** SalesCode *******//
                                vdtlCR.AccType =
                                    VouchManager.getAccType("1-" + dtFixCode.Rows[0]["CashInHand_BD"]
                                        .ToString()); //**** SalesCode *******//
                                vdtlCR.Particulars = dtFixCode.Rows[0]["CashName_BD"].ToString();
                            }
                            else
                            {
                                //vdtlCR.GlCoaCode = "1-" + aSales.BankCoaCode; //**** SalesCode *******//
                                //vdtlCR.AccType =
                                //    VouchManager.getAccType("1-" + aSales.BankCoaCode); //**** SalesCode *******//
                                //vdtlCR.Particulars = aSales.BankName;
                            }

                            vdtlCR.AmountCr = _aVouchMstDV.ControlAmt.Replace(",", "");
                            vdtlCR.AmountDr = "0";
                            vdtlCR.Status = _aVouchMstDV.Status;
                            vdtlCR.BookName = _aVouchMstDV.BookName;
                            vdtlCR.AUTHO_USER = _aVouchMstDV.EntryUser;
                            //*********** Convert Rate ********//
                            VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstDV, vdtlCR, command);
                        }
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

        public void UpdateSupplierPayment(clsSupplierPaymentRec _aclsClientPaymentRec, VouchMst _aVouchMstDV)
        {
            SqlConnection connection = new SqlConnection(DataManager.OraConnString());
            SqlTransaction transaction;
            DataTable dtFixCode = VouchManager.GetAllFixGlCode("");
            try
            {
                connection.Open();

                transaction = connection.BeginTransaction();
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.Transaction = transaction;

                string Query = @"UPDATE [dbo].[SupplierPaymentReceive]
             SET [Date] = convert(date,'" + _aclsClientPaymentRec.Date + "'+' '+ substring(CONVERT(VARCHAR, GETDATE(), 108),0,6),103),[Supplier_id] = '" +
                               _aclsClientPaymentRec.Supplier_id +
                               "',[PayAmt] ='" + _aclsClientPaymentRec.PayAmt.Replace(",", "") + "' ,[update_by] ='" +
                               _aclsClientPaymentRec.LoginBy + "' ,[update_date] =GETDATE(),[Remarks] = '" +
                               _aclsClientPaymentRec.Remarks.Replace(",", "") + "' WHERE ID='" +
                               _aclsClientPaymentRec.ID + "'";
                command.CommandText = Query;
                command.ExecuteNonQuery();
               
                _aVouchMstDV.ControlAmt = _aclsClientPaymentRec.PayAmt.Replace("'", "").Replace(",", "");
                command.CommandText = VouchManager.SaveVoucherMst(_aVouchMstDV, 2);
                command.ExecuteNonQuery();

                command.CommandText = @"delete from gl_trans_dtl where vch_sys_no=convert(numeric,'" +
                                      _aVouchMstDV.VchSysNo + "')";
                command.ExecuteNonQuery();
                VouchDtl vdtlCR;
                 if (_aVouchMstDV.RefFileNo.Equals("New"))
                {
                    _aVouchMstDV.ControlAmt = _aclsClientPaymentRec.PayAmt.Replace("'", "").Replace(",", "");
                    command.CommandText = VouchManager.SaveVoucherMst(_aVouchMstDV, 1);
                    command.ExecuteNonQuery();                   
                    for (int j = 0; j < 2; j++)
                    {
                        if (j == 0)
                        {
                            //DataRow 
                            vdtlCR = new VouchDtl();
                            vdtlCR.VchSysNo = _aVouchMstDV.VchSysNo;
                            vdtlCR.ValueDate = _aclsClientPaymentRec.Date;
                            vdtlCR.LineNo = "1";
                            vdtlCR.GlCoaCode = "1-" + _aclsClientPaymentRec.SupplierCoa;
                            vdtlCR.Particulars = "Supplier Payment Received";
                            vdtlCR.AccType = VouchManager.getAccType("1-" + _aclsClientPaymentRec.SupplierCoa);
                            vdtlCR.AmountCr = "0";
                            vdtlCR.AmountDr = _aVouchMstDV.ControlAmt.Replace(",", "");
                            vdtlCR.Status = _aVouchMstDV.Status;
                            vdtlCR.BookName = _aVouchMstDV.BookName; //*********** Convert Rate ********//

                            vdtlCR.AUTHO_USER = _aVouchMstDV.EntryUser;
                            VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstDV, vdtlCR, command);
                        }
                        else if (j == 1)
                        {
                            vdtlCR = new VouchDtl();
                            vdtlCR.VchSysNo = _aVouchMstDV.VchSysNo;
                            vdtlCR.ValueDate = _aclsClientPaymentRec.Date;
                            vdtlCR.LineNo = "2";
                            if (string.IsNullOrEmpty(_aclsClientPaymentRec.BankId))
                            {
                                vdtlCR.GlCoaCode = "1-" + dtFixCode.Rows[0]["CashInHand_BD"].ToString(); //**** SalesCode *******//
                                vdtlCR.AccType =
                                    VouchManager.getAccType("1-" + dtFixCode.Rows[0]["CashInHand_BD"]
                                        .ToString()); //**** SalesCode *******//
                                vdtlCR.Particulars = dtFixCode.Rows[0]["CashName_BD"].ToString();
                            }
                            else
                            {
                                //vdtlCR.GlCoaCode = "1-" + aSales.BankCoaCode; //**** SalesCode *******//
                                //vdtlCR.AccType =
                                //    VouchManager.getAccType("1-" + aSales.BankCoaCode); //**** SalesCode *******//
                                //vdtlCR.Particulars = aSales.BankName;
                            }

                            vdtlCR.AmountDr = _aVouchMstDV.ControlAmt.Replace(",", "");
                            vdtlCR.AmountCr = "0";
                            vdtlCR.Status = _aVouchMstDV.Status;
                            vdtlCR.BookName = _aVouchMstDV.BookName;
                            vdtlCR.AUTHO_USER = _aVouchMstDV.EntryUser;
                            //*********** Convert Rate ********//
                            VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstDV, vdtlCR, command);
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < 2; j++)
                    {
                        if (j == 0)
                        {
                            //DataRow 
                            vdtlCR = new VouchDtl();
                            vdtlCR.VchSysNo = _aVouchMstDV.VchSysNo;
                            vdtlCR.ValueDate = _aclsClientPaymentRec.Date;
                            vdtlCR.LineNo = "1";
                            vdtlCR.GlCoaCode = "1-" + _aclsClientPaymentRec.SupplierCoa;
                            vdtlCR.Particulars = "Supplier Payment Received";
                            vdtlCR.AccType = VouchManager.getAccType("1-" + _aclsClientPaymentRec.SupplierCoa);
                            vdtlCR.AmountCr = "0";
                            vdtlCR.AmountDr = _aVouchMstDV.ControlAmt.Replace(",", "");
                            vdtlCR.Status = _aVouchMstDV.Status;
                            vdtlCR.BookName = _aVouchMstDV.BookName; //*********** Convert Rate ********//

                            vdtlCR.AUTHO_USER = _aVouchMstDV.EntryUser;
                            VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstDV, vdtlCR, command);
                        }
                        else if (j == 1)
                        {
                            vdtlCR = new VouchDtl();
                            vdtlCR.VchSysNo = _aVouchMstDV.VchSysNo;
                            vdtlCR.ValueDate = _aclsClientPaymentRec.Date;
                            vdtlCR.LineNo = "2";
                            if (string.IsNullOrEmpty(_aclsClientPaymentRec.BankId))
                            {
                                vdtlCR.GlCoaCode = "1-" + dtFixCode.Rows[0]["CashInHand_BD"].ToString(); //**** SalesCode *******//
                                vdtlCR.AccType =
                                    VouchManager.getAccType("1-" + dtFixCode.Rows[0]["CashInHand_BD"]
                                        .ToString()); //**** SalesCode *******//
                                vdtlCR.Particulars = dtFixCode.Rows[0]["CashName_BD"].ToString();
                            }
                            else
                            {
                                //vdtlCR.GlCoaCode = "1-" + aSales.BankCoaCode; //**** SalesCode *******//
                                //vdtlCR.AccType =
                                //    VouchManager.getAccType("1-" + aSales.BankCoaCode); //**** SalesCode *******//
                                //vdtlCR.Particulars = aSales.BankName;
                            }

                            vdtlCR.AmountCr = _aVouchMstDV.ControlAmt.Replace(",", "");
                            vdtlCR.AmountDr = "0";
                            vdtlCR.Status = _aVouchMstDV.Status;
                            vdtlCR.BookName = _aVouchMstDV.BookName;
                            vdtlCR.AUTHO_USER = _aVouchMstDV.EntryUser;
                            //*********** Convert Rate ********//
                            VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstDV, vdtlCR, command);
                        }
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
        public void DeleteCustomerPayment(clsClientPaymentRec _aclsClientPaymentRec)
        {
            String connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string Query = @"UPDATE [dbo].[CustomerPaymentReceive]
             SET [DeleteBy] ='" +
                           _aclsClientPaymentRec.LoginBy + "' ,[DeleteDate] =GETDATE() WHERE ID='" +
                           _aclsClientPaymentRec.ID + "'";
            DataManager.ExecuteNonQuery(connectionString, Query);
            sqlCon.Close();
        }
        public void DeleteBranchCustomerPayment(clsClientPaymentRec _aclsClientPaymentRec)
        {
            String connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string Query = @"UPDATE [dbo].[CustomerPaymentReceive]
             SET [DeleteBy] ='" +
                           _aclsClientPaymentRec.LoginBy + "' ,[DeleteDate] =GETDATE() WHERE ID='" +
                           _aclsClientPaymentRec.ID + "' AND BranchId='"+_aclsClientPaymentRec.BranchId+"'";
            DataManager.ExecuteNonQuery(connectionString, Query);
            sqlCon.Close();
        }

        public void DeleteSupplierPayment(clsSupplierPaymentRec _aclsClientPaymentRec)
        {
            String connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string Query = @"UPDATE [dbo].[SupplierPaymentReceive]
             SET [DeleteBy] ='" +
                           _aclsClientPaymentRec.LoginBy + "' ,[DeleteDate] =GETDATE() WHERE ID='" +
                           _aclsClientPaymentRec.ID + "'";
            DataManager.ExecuteNonQuery(connectionString, Query);
            sqlCon.Close();
        }

        public string CustomerCoa { get; set; }

        public static DataTable GetClientInfosGrid()
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "select * from Customer a where a.DeleteBy IS NULL order by ID";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Customer");
            return dt;
        }
    }
}
