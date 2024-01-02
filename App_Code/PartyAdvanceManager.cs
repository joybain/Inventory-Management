using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using Delve;
using System.Data;

/// <summary>
/// Summary description for PartyAdvanceManager
/// </summary>
public class PartyAdvanceManager
{
	public PartyAdvanceManager()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public static void SavePartyAdvance(PartyAdvance aPartyAdvance)
    {
        String connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);

        string query = @"INSERT INTO [PartyAdvance]
           ([PartySupID],[AdvanceAmount],[PaymentAmount],[VoucherNo],[AddDate],[AddBy],Flag,[PayMethod],[Bank_id],[ChequeNo],[ChequeDate],[Chk_Status],[PayDate])
         VALUES
           ('" + aPartyAdvance.MstID + "','" + aPartyAdvance.Advance + "','" + aPartyAdvance.PaymentAmount + "','" + aPartyAdvance.VoucherNo + "',GETDATE(),'" + aPartyAdvance.LoginBy + "','" + aPartyAdvance.Flag + "','" + aPartyAdvance.PayMethod + "','" + aPartyAdvance.Bank_id + "','" + aPartyAdvance.ChequeNo + "',convert(Date,'" + aPartyAdvance.ChequeDate + "',103),'" + aPartyAdvance.Chk_Status + "',convert(Date,'" + aPartyAdvance.PayDate + "',103))";

        DataManager.ExecuteNonQuery(connectionString, query);
        sqlCon.Close();
    }

    public static void UpdatePartyAdvance(PartyAdvance aPartyAdvance)
    {
        String connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);

        string query = "UPDATE [PartyAdvance]  SET [AdvanceAmount] = '" + aPartyAdvance.Advance + "' ,[PaymentAmount] = '" + aPartyAdvance.PaymentAmount + "',            [VoucherNo] ='" + aPartyAdvance.VoucherNo + "' ,[UpdateDate] =GETDATE() ,[UpdateBy] ='" + aPartyAdvance.LoginBy + "' ,Flag='" + aPartyAdvance.Flag + "',[PayMethod]='" + aPartyAdvance.PayMethod + "',[Bank_id]='" + aPartyAdvance.Bank_id + "',[ChequeNo]='" + aPartyAdvance.ChequeNo + "',[ChequeDate]=convert(Date,'" + aPartyAdvance.ChequeDate + "',103),[Chk_Status]='" + aPartyAdvance.Chk_Status + "',[PayDate]=convert(Date,'" + aPartyAdvance.PayDate + "',103)  WHERE  ID='" + aPartyAdvance.ID + "'";

        DataManager.ExecuteNonQuery(connectionString, query);
        sqlCon.Close();
    }

    public static void DeletePartyAdvance(string ID)
    {
        String connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);

        string query = "delete from [PartyAdvance]  WHERE  ID='" + ID + "'";

        DataManager.ExecuteNonQuery(connectionString, query);
        sqlCon.Close();
    }

    public static PartyAdvance GetShowPartyAdvanceDetails(string ID)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string query = "SELECT t1.[ID],t1.[PartySupID],t1.[AdvanceAmount],t1.[PaymentAmount],CONVERT(NVARCHAR,t1.[PayDate],103) AS [PayDate],t1.[VoucherNo],t1.[PayMethod],t1.[Bank_id],t1.[ChequeNo],CONVERT(NVARCHAR,t1.[ChequeDate],103) AS [ChequeDate],t1.[Chk_Status],t2.GRN  FROM [PartyAdvance] t1 left join ItemPurchaseMst t2 on t2.ID=t1.VoucherNo where t1.ID='" + ID + "' ";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "PartyAdvance");     
        if (dt.Rows.Count == 0)
        {
            return null;
        }
        return new PartyAdvance(dt.Rows[0]);
    }

    public static DataTable GetShowPartyDetails(string Flag)
    {
        string connectionString = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(connectionString);
        string query = @"SELECT t1.ID
,CASE WHEN t1.Flag='PA' then t2.PartyName WHEN t1.Flag='SAP' then t3.ContactName else '' end AS Name
,CASE WHEN t1.Flag='PA' then t2.[Mobile] WHEN t1.Flag='SAP' then t3.Mobile else '' end AS Phone
,t1.AdvanceAmount
,t1.PaymentAmount
,t4.GRN  AS VoucherNo
 FROM [PartyAdvance] t1 left join PartyInfo t2 on t2.ID=t1.PartySupID  left join Supplier t3 on t3.ID=t1.PartySupID left join ItemPurchaseMst t4 on t4.ID=t1.VoucherNo where t1.Flag='" + Flag + "' ";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "PartyAdvance");
        return dt;
    }
}