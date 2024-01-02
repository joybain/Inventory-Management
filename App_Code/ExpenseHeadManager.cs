using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Delve;

/// <summary>
/// Summary description for ExpenseHeadManager
/// </summary>
public class ExpenseHeadManager
{
	public ExpenseHeadManager()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public static void SaveColorInfo(string ColorID, string Name, string LoginBy, GlCoa glcoa, SegCoa segcoa)
    {
//        string connectionString = DataManager.OraConnString();
//        SqlConnection oracon = new SqlConnection(connectionString);
//        string query = @"INSERT INTO [ExpenseHead]
//           ([Name],AddBy,AddDate)
//     VALUES
//           ('" + Name + "','" + LoginBy + "',GETDATE())";
//        DataManager.ExecuteNonQuery(connectionString, query);

        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlTransaction transaction;
        try
        {
            connection.Open();
            transaction = connection.BeginTransaction();
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.Transaction = transaction;

            // Expenses Set Code......
            command.CommandText = @"INSERT INTO [ExpenseHead]
           ([Name],AddBy,AddDate,GL_COA_CODE)
     VALUES
           ('" + Name + "','" + LoginBy + "',GETDATE(),'" + segcoa.GlSegCode + "')";
            command.ExecuteNonQuery();
            // End Expenses

            // GL Seg Coa Set
            if (segcoa.ParentCode == "" || segcoa.ParentCode == null)
            {
                command.CommandText = " insert into gl_seg_coa (seg_coa_code,lvl_code,seg_coa_desc,bud_allowed, " +
                       " post_allowed,acc_type,open_date,rootleaf,status,taxable,book_name,entry_user,entry_date) values ( " +
                       " '" + segcoa.GlSegCode + "',  '" + segcoa.LvlCode + "',  '" + segcoa.SegCoaDesc + "', " +
                       " '" + segcoa.BudAllowed + "',  '" + segcoa.PostAllowed + "', " +
                        "  '" + segcoa.AccType + "', convert(datetime,case '" + segcoa.OpenDate + "' when '' then null else '" + segcoa.OpenDate + "' end,103),  '" + segcoa.RootLeaf + "', " +
                        "  '" + segcoa.Status + "',  '" + segcoa.Taxable + "',  '" + segcoa.BookName + "','" + segcoa.EntryUser + "',convert(datetime,'" + segcoa.EntryDate + "',103))";
                command.ExecuteNonQuery();
            }
            else
            {
                command.CommandText = " insert into gl_seg_coa (seg_coa_code,lvl_code,seg_coa_desc,parent_code,bud_allowed, " +
                       " post_allowed,acc_type,open_date,rootleaf,status,taxable,book_name,entry_user,entry_date) values ( " +
                       " '" + segcoa.GlSegCode + "',  '" + segcoa.LvlCode + "',  '" + segcoa.SegCoaDesc + "', " +
                       "  '" + segcoa.ParentCode + "',  '" + segcoa.BudAllowed + "',  '" + segcoa.PostAllowed + "', " +
                        "  '" + segcoa.AccType + "', convert(datetime,case '" + segcoa.OpenDate + "' when '' then null else '" + segcoa.OpenDate + "' end,103),  '" + segcoa.RootLeaf + "', " +
                        "  '" + segcoa.Status + "',  '" + segcoa.Taxable + "',  '" + segcoa.BookName + "','" + segcoa.EntryUser + "',convert(datetime,'" + segcoa.EntryDate + "',103))";
                command.ExecuteNonQuery();
            }
            // End Seg Coa

            // GL Coa Expenses
            command.CommandText = "insert into gl_coa (book_name,gl_coa_code,coa_enabled,effective_from,effective_to, " +
            " bud_allowed,post_allowed,taxable,acc_type,status,coa_desc,coa_curr_bal,coa_natural_code,ENTRY_USER,ENTRY_DATE) values ( " +
            " '" + glcoa.BookName + "',  '" + glcoa.GlCoaCode + "',  '" + glcoa.CoaEnabled + "', " +
            "  convert(datetime,case '" + glcoa.EffectiveFrom + "' when '' then null else '" + glcoa.EffectiveFrom + "' end ,103), convert(datetime,case '" + glcoa.EffectiveTo + "' when '' then null else '" + glcoa.EffectiveTo + "' end ,103), '" + glcoa.BudAllowed + "', " +
            "  '" + glcoa.PostAllowed + "',  '" + glcoa.Taxable + "',  '" + glcoa.AccType + "', " +
            "  '" + glcoa.Status + "',  '" + glcoa.CoaDesc.Replace("'", "") + "',  '" + glcoa.CoaCurrBal + "', " +
            "  '" + glcoa.CoaNaturalCode + "','" + glcoa.LoginBy + "',GETDATE()) ";
            command.ExecuteNonQuery();
            // End GL Coa

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


    public static void SaveBranchColorInfo(string ColorID, string Name, string LoginBy, GlCoa glcoa, SegCoa segcoa,string BranchId)
    {
        //        string connectionString = DataManager.OraConnString();
        //        SqlConnection oracon = new SqlConnection(connectionString);
        //        string query = @"INSERT INTO [ExpenseHead]
        //           ([Name],AddBy,AddDate)
        //     VALUES
        //           ('" + Name + "','" + LoginBy + "',GETDATE())";
        //        DataManager.ExecuteNonQuery(connectionString, query);

        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlTransaction transaction;
        try
        {
            connection.Open();
            transaction = connection.BeginTransaction();
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.Transaction = transaction;

            // Expenses Set Code......
            command.CommandText = @"INSERT INTO [ExpenseHead]
           ([Name],AddBy,AddDate,GL_COA_CODE)
     VALUES
           ('" + Name + "','" + LoginBy + "',GETDATE(),'" + segcoa.GlSegCode + "')";
            command.ExecuteNonQuery();
            // End Expenses

            // GL Seg Coa Set
            if (segcoa.ParentCode == "" || segcoa.ParentCode == null)
            {
                command.CommandText = " insert into gl_seg_coa (seg_coa_code,lvl_code,seg_coa_desc,bud_allowed, " +
                       " post_allowed,acc_type,open_date,rootleaf,status,taxable,book_name,entry_user,entry_date) values ( " +
                       " '" + segcoa.GlSegCode + "',  '" + segcoa.LvlCode + "',  '" + segcoa.SegCoaDesc + "', " +
                       " '" + segcoa.BudAllowed + "',  '" + segcoa.PostAllowed + "', " +
                        "  '" + segcoa.AccType + "', convert(datetime,case '" + segcoa.OpenDate + "' when '' then null else '" + segcoa.OpenDate + "' end,103),  '" + segcoa.RootLeaf + "', " +
                        "  '" + segcoa.Status + "',  '" + segcoa.Taxable + "',  '" + segcoa.BookName + "','" + segcoa.EntryUser + "',convert(datetime,'" + segcoa.EntryDate + "',103))";
                command.ExecuteNonQuery();
            }
            else
            {
                command.CommandText = " insert into gl_seg_coa (seg_coa_code,lvl_code,seg_coa_desc,parent_code,bud_allowed, " +
                       " post_allowed,acc_type,open_date,rootleaf,status,taxable,book_name,entry_user,entry_date) values ( " +
                       " '" + segcoa.GlSegCode + "',  '" + segcoa.LvlCode + "',  '" + segcoa.SegCoaDesc + "', " +
                       "  '" + segcoa.ParentCode + "',  '" + segcoa.BudAllowed + "',  '" + segcoa.PostAllowed + "', " +
                        "  '" + segcoa.AccType + "', convert(datetime,case '" + segcoa.OpenDate + "' when '' then null else '" + segcoa.OpenDate + "' end,103),  '" + segcoa.RootLeaf + "', " +
                        "  '" + segcoa.Status + "',  '" + segcoa.Taxable + "',  '" + segcoa.BookName + "','" + segcoa.EntryUser + "',convert(datetime,'" + segcoa.EntryDate + "',103))";
                command.ExecuteNonQuery();
            }
            // End Seg Coa

            // GL Coa Expenses
            command.CommandText = "insert into gl_coa (book_name,gl_coa_code,coa_enabled,effective_from,effective_to, " +
            " bud_allowed,post_allowed,taxable,acc_type,status,coa_desc,coa_curr_bal,coa_natural_code,ENTRY_USER,ENTRY_DATE) values ( " +
            " '" + glcoa.BookName + "',  '" + glcoa.GlCoaCode + "',  '" + glcoa.CoaEnabled + "', " +
            "  convert(datetime,case '" + glcoa.EffectiveFrom + "' when '' then null else '" + glcoa.EffectiveFrom + "' end ,103), convert(datetime,case '" + glcoa.EffectiveTo + "' when '' then null else '" + glcoa.EffectiveTo + "' end ,103), '" + glcoa.BudAllowed + "', " +
            "  '" + glcoa.PostAllowed + "',  '" + glcoa.Taxable + "',  '" + glcoa.AccType + "', " +
            "  '" + glcoa.Status + "',  '" + glcoa.CoaDesc.Replace("'", "") + "',  '" + glcoa.CoaCurrBal + "', " +
            "  '" + glcoa.CoaNaturalCode + "','" + glcoa.LoginBy + "',GETDATE()) ";
            command.ExecuteNonQuery();
            // End GL Coa

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



    public static DataTable GetColorDetails()
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection oracon = new SqlConnection(connectionString);
        string query = @"SELECT [ID],[Name],GL_COA_CODE FROM [ExpenseHead] order by id desc";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ColorInfo");
        return dt;
    }

    public static DataTable GetBranchExpenseHead(string BranchId)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection oracon = new SqlConnection(connectionString);
        string query = @"SELECT [ID],[Name],GL_COA_CODE FROM [ExpenseHead] where BranchId='"+BranchId+"' order by id desc";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ColorInfo");
        return dt;
    }

    public static void DeleteColorInfo(string ID, string p_2,string CoaCode)
    {
        //string connectionString = DataManager.OraConnString();
        //SqlConnection oracon = new SqlConnection(connectionString);
        //string query = @"DELETE FROM [ExpenseHead]  WHERE ID='" + ID + "'";
        //DataManager.ExecuteNonQuery(connectionString, query);

        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlTransaction transaction;
        try
        {
            connection.Open();
            transaction = connection.BeginTransaction();
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.Transaction = transaction;

            command.CommandText = @"DELETE FROM [ExpenseHead]  WHERE ID='" + ID + "'";
            command.ExecuteNonQuery();

            command.CommandText = @"delete from GL_SEG_COA where SEG_COA_CODE='" + CoaCode + "' ";
            command.ExecuteNonQuery();

            command.CommandText = @"delete from GL_COA where COA_NATURAL_CODE='" + CoaCode + "' ";
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

    public static void UpdateColorInfo(string ID, string Name,string LoginBy,string COADesc,string CoaCode)
    {
        //string connectionString = DataManager.OraConnString();
        //SqlConnection oracon = new SqlConnection(connectionString);
        //string query = @"UPDATE [ExpenseHead] SET [Name] ='" + Name + "',UpdateBy='" + LoginBy +
        //               "',UpdateDate=GETDATE()  WHERE ID='" + ID + "'";
        //DataManager.ExecuteNonQuery(connectionString, query);


        SqlConnection connection = new SqlConnection(DataManager.OraConnString());
        SqlTransaction transaction;
        try
        {
            connection.Open();
            transaction = connection.BeginTransaction();
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.Transaction = transaction;

            command.CommandText = @"UPDATE [ExpenseHead] SET [Name] ='" + Name + "',UpdateBy='" + LoginBy +
                                  "',UpdateDate=GETDATE()  WHERE ID='" + ID + "'";
            command.ExecuteNonQuery();

            command.CommandText = @"UPDATE [GL_SEG_COA] SET [SEG_COA_DESC] ='" + Name + " Expenses'  WHERE [SEG_COA_CODE]='" + CoaCode + "'";
            command.ExecuteNonQuery();

            command.CommandText = @"UPDATE [GL_COA] SET [COA_DESC] ='" + COADesc + "' where [GL_COA_CODE]='1-" + CoaCode + "'";
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

    public DataTable GetShowExpenses(string SearchExp, string Flag)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection oracon = new SqlConnection(connectionString);
        string query =
            @"SELECT [ID],[Name],[GL_COA_CODE],[Search] FROM [dbo].[View_ExpenseDetails] where UPPER(Search)=UPPER('" +
            SearchExp + "')";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ColorInfo");
        return dt;
    }

    //******************* Daily Expenses Entry ************************//

    public void SaveDailyExpenses(ExpensesInfo _aExpensesInfo, DataTable dtExp, VouchMst _aVouchMstCR,string BranchId)
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

            command.CommandText = @"INSERT INTO [dbo].[DailyExpensesMst]
           (BranchId,[Code],[ExpDate],[Remarks],[AddBy],[AddDate])
     VALUES
           ('"+BranchId+"','" + _aExpensesInfo.Code + "',convert(date,'" + _aExpensesInfo.Date + "',103),'" + _aExpensesInfo.Remarks + "','" + _aExpensesInfo.LoginBy + "',GETDATE())";
            command.ExecuteNonQuery();
            command.CommandText = @"SELECT top(1) [ID] FROM [dbo].[DailyExpensesMst] where BranchId='"+BranchId+"' ORDER BY ID desc";
            int MstID =Convert.ToInt32(command.ExecuteScalar());

            command.CommandText = VouchManager.SaveVoucherMst(_aVouchMstCR, 1);
            command.ExecuteNonQuery();

            VouchDtl vdtlCR;
           
            int sl = 1;
            foreach (DataRow drRow in dtExp.Rows)
            {
                if (!string.IsNullOrEmpty(drRow["ExpensesHead"].ToString()))
                {
                    command.CommandText = @"INSERT INTO [dbo].[DailyExpensesDtl]
           (BranchId,[MstID],[ExpensesHeadID],[Amount],[AddBy],[AddDate])
     VALUES
           ('" + BranchId + "','" + MstID + "','" + drRow["ID"].ToString() + "','" +
                                          drRow["Amount"].ToString().Replace(",", "") + "','" + _aExpensesInfo.LoginBy +
                                          "',GETDATE())";
                    command.ExecuteNonQuery();

                    vdtlCR = new VouchDtl();
                    vdtlCR.VchSysNo = _aVouchMstCR.VchSysNo;
                    vdtlCR.ValueDate = _aExpensesInfo.Date;
                    vdtlCR.LineNo = sl.ToString();
                    vdtlCR.GlCoaCode = "1-" + drRow["GL_COA_CODE"].ToString();
                    vdtlCR.Particulars = drRow["ExpensesHead"].ToString();
                    vdtlCR.AccType = VouchManager.getAccType("1-" + drRow["GL_COA_CODE"].ToString());
                    vdtlCR.AmountDr = drRow["Amount"].ToString().Replace(",", "");
                    vdtlCR.AmountCr = "0";
                    vdtlCR.Status = _aVouchMstCR.Status;
                    vdtlCR.BookName = _aVouchMstCR.BookName; //*********** Convert Rate ********//

                    vdtlCR.AUTHO_USER = _aVouchMstCR.EntryUser;
                    VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstCR, vdtlCR, command);
                    sl++;
                }
            }

            vdtlCR = new VouchDtl();
            vdtlCR.VchSysNo = _aVouchMstCR.VchSysNo;
            vdtlCR.ValueDate = _aExpensesInfo.Date;
            vdtlCR.LineNo = sl.ToString();
            vdtlCR.GlCoaCode = dtFixCode.Rows[0]["CashInHand_BD"].ToString(); //**** SalesCode *******//
            vdtlCR.AccType =
                VouchManager.getAccType(dtFixCode.Rows[0]["CashInHand_BD"]
                    .ToString()); //**** SalesCode *******//
            vdtlCR.Particulars = dtFixCode.Rows[0]["CashName_BD"].ToString();
            vdtlCR.AmountDr = "0";
            vdtlCR.AmountCr = _aVouchMstCR.ControlAmt.Replace(",", "");
            vdtlCR.Status = _aVouchMstCR.Status;
            vdtlCR.BookName = _aVouchMstCR.BookName;
            vdtlCR.AUTHO_USER = _aVouchMstCR.EntryUser;
            VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstCR, vdtlCR, command);

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

    public void UpdateDailyExpenses(ExpensesInfo _aExpensesInfo, DataTable dtExp, VouchMst _aVouchMstCR,string BranchId)
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

            command.CommandText = @"UPDATE [dbo].[DailyExpensesMst]
            SET [ExpDate] = convert(date,'" + _aExpensesInfo.Date + "',103) ,[Remarks] ='" + _aExpensesInfo.Remarks +"' ,[UpdateBy] ='" + _aExpensesInfo.LoginBy + "' ,[UpdateDate] =GETDATE()  WHERE ID=" +_aExpensesInfo.ID+" and BranchId='"+BranchId+"'";
            command.ExecuteNonQuery();

            command.CommandText = @"delete FROM [dbo].[DailyExpensesDtl] where MstID=" + _aExpensesInfo.ID+" and BranchId='"+BranchId+"'";
            command.ExecuteNonQuery();


            command.CommandText = VouchManager.SaveVoucherMst(_aVouchMstCR, 2);
            command.ExecuteNonQuery();

            command.CommandText = @"delete from gl_trans_dtl where vch_sys_no=convert(numeric,'" +
                                  _aVouchMstCR.VchSysNo + "')";
            command.ExecuteNonQuery();

            VouchDtl vdtlCR;
            
            int sl = 1;

            foreach (DataRow drRow in dtExp.Rows)
            {
                if (!string.IsNullOrEmpty(drRow["ExpensesHead"].ToString()))
                {
                    command.CommandText = @"INSERT INTO [dbo].[DailyExpensesDtl]
           (BranchId,[MstID],[ExpensesHeadID],[Amount],[AddBy],[AddDate])
     VALUES
           ('"+BranchId+"','" + _aExpensesInfo.ID + "','" + drRow["ID"].ToString() + "','" +
                                          drRow["Amount"].ToString().Replace(",", "") + "','" + _aExpensesInfo.LoginBy +
                                          "',GETDATE())";
                    command.ExecuteNonQuery();

                    vdtlCR = new VouchDtl();
                    vdtlCR.VchSysNo = _aVouchMstCR.VchSysNo;
                    vdtlCR.ValueDate = _aExpensesInfo.Date;
                    vdtlCR.LineNo = sl.ToString();
                    vdtlCR.GlCoaCode = "1-" + drRow["GL_COA_CODE"].ToString();
                    vdtlCR.Particulars = drRow["ExpensesHead"].ToString();
                    vdtlCR.AccType = VouchManager.getAccType("1-" + drRow["GL_COA_CODE"].ToString());
                    vdtlCR.AmountDr = drRow["Amount"].ToString().Replace(",", "");
                    vdtlCR.AmountCr = "0";
                    vdtlCR.Status = _aVouchMstCR.Status;
                    vdtlCR.BookName = _aVouchMstCR.BookName;
                    vdtlCR.AUTHO_USER = _aVouchMstCR.EntryUser;
                    VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstCR, vdtlCR, command);
                    sl++;
                }

            }

            vdtlCR = new VouchDtl();
            vdtlCR.VchSysNo = _aVouchMstCR.VchSysNo;
            vdtlCR.ValueDate = _aExpensesInfo.Date;
            vdtlCR.LineNo = sl.ToString();
            vdtlCR.GlCoaCode = dtFixCode.Rows[0]["CashInHand_BD"].ToString(); //**** SalesCode *******//
            vdtlCR.AccType =
                VouchManager.getAccType(dtFixCode.Rows[0]["CashInHand_BD"]
                    .ToString()); //**** SalesCode *******//
            vdtlCR.Particulars = dtFixCode.Rows[0]["CashName_BD"].ToString();
            vdtlCR.AmountDr = "0";
            vdtlCR.AmountCr = _aVouchMstCR.ControlAmt.Replace(",", "");
            vdtlCR.Status = _aVouchMstCR.Status;
            vdtlCR.BookName = _aVouchMstCR.BookName;
            vdtlCR.AUTHO_USER = _aVouchMstCR.EntryUser;
            VouchManager.CreateVouchDtlForAutoVoucher(_aVouchMstCR, vdtlCR, command);

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

    public DataTable getDailyExpences(string ID,string Flag,string BranchId)
    {
        DataTable dt = null;
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        sqlCon.Open();
        SqlDataAdapter da = new SqlDataAdapter();
        da.SelectCommand = new SqlCommand("SP_ExpensesDetails", sqlCon);
        da.SelectCommand.CommandType = CommandType.StoredProcedure;

        if (!string.IsNullOrEmpty(ID))
        {
            da.SelectCommand.Parameters.AddWithValue("@ID", ID);
        }
        else
        {
            da.SelectCommand.Parameters.AddWithValue("@ID", null);
        }

        if (!string.IsNullOrEmpty(BranchId))
        {
            da.SelectCommand.Parameters.AddWithValue("@BranchId", BranchId);
        }
        else
        {
            da.SelectCommand.Parameters.AddWithValue("@BranchId", null);
        }
        da.SelectCommand.Parameters.AddWithValue("@Type", Flag);
        da.SelectCommand.CommandTimeout = 6000;
        DataSet ds = new DataSet();
        da.Fill(ds, "SP_ExpensesDetails");
        dt = ds.Tables["SP_ExpensesDetails"];
        return dt;
    }

    public void DeleteExpensesDetails(string MstID, VouchMst _aVouchMstCR,string BranchId)
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

            command.CommandText = @"delete FROM [dbo].[DailyExpensesMst] where ID=" + MstID + " and BranchId='"+BranchId+"' ";
            command.ExecuteNonQuery();

            command.CommandText = @"delete FROM [dbo].[DailyExpensesDtl] where MstID=" + MstID+" and BranchId='"+BranchId+"'";
            command.ExecuteNonQuery();

            if (_aVouchMstCR.VchSysNo != null)
            {
                command.CommandText = @"delete from gl_trans_dtl where vch_sys_no=convert(numeric,'" +
                                      _aVouchMstCR.VchSysNo + "')";
                command.ExecuteNonQuery();

                command.CommandText = @"delete FROM [dbo].[GL_TRANS_MST] where vch_sys_no=convert(numeric,'" +
                                      _aVouchMstCR.VchSysNo + "')";
                command.ExecuteNonQuery();
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

    public DataTable GetShowwAllExpenses(string p)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection oracon = new SqlConnection(connectionString);
        string query = @"SELECT top(50) [ID],[Code],convert(nvarchar,[ExpDate],103) AS [ExpDate],[Remarks],[Total]
  FROM [dbo].[View_ExpensesMst] order by ID desc";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ColorInfo");
        return dt;
    }

    public DataTable GetShowwBranchAllExpenses(string BranchId)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection oracon = new SqlConnection(connectionString);
        string query = @"SELECT top(50) [ID],[Code],convert(nvarchar,[ExpDate],103) AS [ExpDate],[Remarks],[Total]
  FROM [dbo].[View_ExpensesMst] where BranchId='"+BranchId+"' order by ID desc";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "ColorInfo");
        return dt;
    }
}