using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Delve;
using System.Data.SqlClient;

/// <summary>
/// Summary description for BankAndCashBlanceCheck
/// </summary>
public class BankAndCashBlanceCheck
{
	public BankAndCashBlanceCheck()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public static double GetCheckBlanceInBankAndCash(string GlCoaCode, int Flag,string UserType,string StartDate,string EndDate)
    {
        DataTable dtVch = VouchManager.GetVouchDtlForTotal(EndDate, "A", "AMB", UserType);
        string connectionString = DataManager.OraConnString();
        SqlConnection conn = new SqlConnection();
        conn.ConnectionString = connectionString;
        string criteria = GlCoaManager.getCoaCriteria(GlCoaCode, "-", "AMB");
        DataTable dtAlready = GlCoaManager.GetGlCoaCodes(criteria);
        string glcoacode = "";
        double bal = 0;
        foreach (DataRow drA in dtAlready.Rows)
        {  glcoacode += "'" + drA["gl_coa_code"].ToString() + "'" + ","; }
        if (glcoacode != "")
        {
            glcoacode = glcoacode.Remove(glcoacode.Length - 1, 1);
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = new SqlCommand("SP_CheckBlance", conn);
            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            da.SelectCommand.Parameters.AddWithValue("@GlCoaCode", glcoacode);
            if (!string.IsNullOrEmpty(UserType))
            {
                da.SelectCommand.Parameters.AddWithValue("@type", UserType);
            }
            else
            {
                da.SelectCommand.Parameters.AddWithValue("@type", null);
            }

            DataSet Ds = new DataSet();
            da.Fill(Ds, "SP_CheckBlance");       
            DataRow dr = Ds.Tables[0].NewRow();
            dr["vch_sys_no"] = "99999999";
            dr["acc_type"] = "";
            dr["coa_desc"] = "";
            dr["v_date"] = DataManager.DateEncode(DateTime.Now.ToString("dd/MM/yyyy"));
            dr["value_date"] = DateTime.Now.ToString("dd/MM/yyyy");
            dr["vch_manual_no"] = "";
            dr["particulars"] = "Opening Balance (REF. dt. " + DateTime.Now.ToString("dd/MM/yyyy") + ")";
            DateTime nowdate = DateTime.Now;
            decimal SumOpBal = GlCoaManager.getCoaBalanceFromExistingDataTable(dtVch, "AMB", GlCoaCode, "-", "A",
                StartDate, EndDate, null, 1);
            if (SumOpBal > 0)
            {
                dr["debit_amt"] = SumOpBal;
                dr["credit_amt"] = decimal.Zero;
            }
            else
            {
                dr["debit_amt"] = decimal.Zero;
                dr["credit_amt"] = SumOpBal * -1;
            }
            dr["bal"] = decimal.Zero;
            Ds.Tables[0].Rows.InsertAt(dr, 0);           
            foreach (DataRow drLedg in Ds.Tables[0].Rows)
            {
                drLedg.BeginEdit();
                bal += Convert.ToDouble(drLedg["debit_amt"].ToString()) - Convert.ToDouble(drLedg["credit_amt"].ToString());
                drLedg["bal"] = bal;
                drLedg.AcceptChanges();
            }

        }
        return bal;
    }

    public static double GetCheckBlanceInBankAndCash(string GlCoaCode, int Flag, string StartDate, string EndDate)
    {
        DataTable dtVch = VouchManager.GetVouchDtlForTotal(EndDate, "A", "AMB", "");
        string connectionString = DataManager.OraConnString();
        SqlConnection conn = new SqlConnection();
        conn.ConnectionString = connectionString;
        string criteria = GlCoaManager.getCoaCriteria(GlCoaCode, "-", "AMB");
        DataTable dtAlready = GlCoaManager.GetGlCoaCodes(criteria);
        string glcoacode = "";
        double bal = 0;
        foreach (DataRow drA in dtAlready.Rows)
        { glcoacode += "'" + drA["gl_coa_code"].ToString() + "'" + ","; }
        if (glcoacode != "")
        {
            glcoacode = glcoacode.Remove(glcoacode.Length - 1, 1);
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = new SqlCommand("SP_CheckBlance", conn);
            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            da.SelectCommand.Parameters.AddWithValue("@GlCoaCode", glcoacode);
            DataSet Ds = new DataSet();
            da.Fill(Ds, "SP_CheckBlance");
            DataRow dr = Ds.Tables[0].NewRow();
            dr["vch_sys_no"] = "99999999";
            dr["acc_type"] = "";
            dr["coa_desc"] = "";
            dr["v_date"] = DataManager.DateEncode(DateTime.Now.ToString("dd/MM/yyyy"));
            dr["value_date"] = DateTime.Now.ToString("dd/MM/yyyy");
            dr["vch_manual_no"] = "";
            dr["particulars"] = "Opening Balance (REF. dt. " + DateTime.Now.ToString("dd/MM/yyyy") + ")";
            DateTime nowdate = DateTime.Now;
            decimal SumOpBal = GlCoaManager.getCoaBalanceFromExistingDataTable(dtVch, "AMB", GlCoaCode, "-", "A",
                StartDate, EndDate, null, 1);
            if (SumOpBal > 0)
            {
                dr["debit_amt"] = Math.Ceiling(SumOpBal);
                dr["credit_amt"] = decimal.Zero;
            }
            else
            {
                dr["debit_amt"] = decimal.Zero;
                dr["credit_amt"] = Math.Ceiling(SumOpBal * -1);
            }
            dr["bal"] = decimal.Zero;
            Ds.Tables[0].Rows.InsertAt(dr, 0);
            foreach (DataRow drLedg in Ds.Tables[0].Rows)
            {
                drLedg.BeginEdit();
                bal += Convert.ToDouble(drLedg["debit_amt"].ToString()) - Convert.ToDouble(drLedg["credit_amt"].ToString());
                drLedg["bal"] = Math.Ceiling(bal);
                drLedg.AcceptChanges();
            }

        }
        return bal;
    }

    public static double GetCheckBlanceInBankAndCash(string GlCoaCode, int Flag)
    {
        DataTable dtVch = VouchManager.GetVouchDtlForTotal(DateTime.Now.ToString("dd/MM/yyyy"), "A", "AMB", "");
        string connectionString = DataManager.OraConnString();
        SqlConnection conn = new SqlConnection();
        conn.ConnectionString = connectionString;
        string criteria = GlCoaManager.getCoaCriteria(GlCoaCode, "-", "AMB");
        DataTable dtAlready = GlCoaManager.GetGlCoaCodes(criteria);
        string glcoacode = "";
        double bal = 0;
        foreach (DataRow drA in dtAlready.Rows)
        { glcoacode += "'" + drA["gl_coa_code"].ToString() + "'" + ","; }
        if (glcoacode != "")
        {
            glcoacode = glcoacode.Remove(glcoacode.Length - 1, 1);
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = new SqlCommand("SP_CheckBlance", conn);
            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            da.SelectCommand.Parameters.AddWithValue("@GlCoaCode", glcoacode);
            DataSet Ds = new DataSet();
            da.Fill(Ds, "SP_CheckBlance");
            DataRow dr = Ds.Tables[0].NewRow();
            dr["vch_sys_no"] = "99999999";
            dr["acc_type"] = "";
            dr["coa_desc"] = "";
            dr["v_date"] = DataManager.DateEncode(DateTime.Now.ToString("dd/MM/yyyy"));
            dr["value_date"] = DateTime.Now.ToString("dd/MM/yyyy");
            dr["vch_manual_no"] = "";
            dr["particulars"] = "Opening Balance (REF. dt. " + DateTime.Now.ToString("dd/MM/yyyy") + ")";
            DateTime nowdate = DateTime.Now;
            decimal SumOpBal = GlCoaManager.getCoaBalanceFromExistingDataTable(dtVch, "AMB", GlCoaCode, "-", "A", nowdate.AddDays(1).ToString("dd/MM/yyyy"), nowdate.AddDays(1).ToString("dd/MM/yyyy"), null, 1);
            if (SumOpBal > 0)
            {
                dr["debit_amt"] = SumOpBal;
                dr["credit_amt"] = decimal.Zero;
            }
            else
            {
                dr["debit_amt"] = decimal.Zero;
                dr["credit_amt"] = SumOpBal * -1;
            }
            dr["bal"] = decimal.Zero;
            Ds.Tables[0].Rows.InsertAt(dr, 0);
            foreach (DataRow drLedg in Ds.Tables[0].Rows)
            {
                drLedg.BeginEdit();
                bal += Convert.ToDouble(drLedg["debit_amt"].ToString()) - Convert.ToDouble(drLedg["credit_amt"].ToString());
                drLedg["bal"] = bal;
                drLedg.AcceptChanges();
            }

        }
        return bal;
    }

    public static DataTable GetCheckBlanceInAcc(string GlCoaCode, string Book, string StartDt, string EndDt)
    {
        SqlConnection conn = new SqlConnection(DataManager.OraConnString());
        DataTable dtVch = VouchManager.GetVouchDtlForTotal(EndDt, "A", Book);
        string coaexist = GlCoaManager.getCoaExistence(GlCoaCode);

        SqlDataAdapter da = new SqlDataAdapter();
        da.SelectCommand = new SqlCommand("SP_CheckBlance", conn);
        da.SelectCommand.CommandType = CommandType.StoredProcedure;
        da.SelectCommand.Parameters.AddWithValue("@GlCoaCode", GlCoaCode);
        DataSet Ds = new DataSet();
        da.Fill(Ds, "SP_CheckBlance");            
        string coaDesc = ""; string accTyp = "";
        if (Ds.Tables[0].Rows.Count > 0)
        {
            accTyp = ((DataRow)Ds.Tables[0].Rows[0])["acc_type"].ToString();
            coaDesc = ((DataRow)Ds.Tables[0].Rows[0])["coa_desc"].ToString();
        }
        else
        {
            accTyp = VouchManager.getAccType(GlCoaCode);
            coaDesc = GlCoaCode;
        }
        DataRow dr = Ds.Tables[0].NewRow();
        dr["vch_sys_no"] = "99999999";
        dr["acc_type"] = accTyp;
        dr["v_date"] = DataManager.DateEncode(StartDt);
        dr["value_date"] = StartDt;
        dr["coa_desc"] = "";
        dr["vch_manual_no"] = "";
        dr["particulars"] = "Opening Balance (REF. dt. " + StartDt + ")";
        if (accTyp == "A" | accTyp == "E")
        {
            dr["debit_amt"] = GlCoaManager.getCoaopeningBalanceFromExistingDataTable(dtVch, Book, GlCoaCode, "-", "A", StartDt, EndDt).ToString();
            dr["credit_amt"] = decimal.Zero;
        }
        else if (accTyp == "L" | accTyp == "I")
        {
            dr["debit_amt"] = decimal.Zero;
            dr["credit_amt"] = GlCoaManager.getCoaopeningBalanceFromExistingDataTable(dtVch, Book, GlCoaCode, "-", "A", StartDt, EndDt).ToString();
        }
        dr["bal"] = decimal.Zero;
        Ds.Tables[0].Rows.InsertAt(dr, 0);
        decimal bal = decimal.Zero;
        foreach (DataRow drLedg in Ds.Tables[0].Rows)
        {
            if (drLedg["debit_amt"].ToString() != "")
            {
                drLedg.BeginEdit();
                bal += decimal.Parse(drLedg["debit_amt"].ToString()) - decimal.Parse(drLedg["credit_amt"].ToString());
                drLedg["bal"] = bal;
                drLedg.AcceptChanges();
            }
        }
        DataTable ddt = Ds.Tables[0];
        return ddt;
    }

    public static void GetBanlanceConvertion(VouchDtl VchDtl, DataRow drdt)
    {
        VchDtl.Amount_DR_BD = drdt["amount_dr"].ToString();
        VchDtl.Amount_CR_DB = drdt["amount_cr"].ToString();

        VchDtl.Amount_DR_PH = drdt["amount_dr"].ToString();
        VchDtl.Amount_CR_PH = drdt["amount_cr"].ToString();
    }

    public static void GetBanlanceConvertion(VouchDtl VchDtl, string AmountDR, string AmountCR)
    {
        VchDtl.Amount_DR_BD = AmountDR;
        VchDtl.Amount_CR_DB = AmountCR;

        VchDtl.Amount_DR_PH = AmountDR;
        VchDtl.Amount_CR_PH = AmountCR;
    }



    public static double GetCurrency(System.Web.UI.WebControls.Button btnSave, System.Web.UI.WebControls.TextBox txtValueDate, int flag)
    {
        string date = "";
        if (flag.Equals(1))
        {
            date = txtValueDate.Text;
        }
        else
        {
            date = DateTime.Now.ToString("dd/MM/yyyy");
        }
        double CurrencyRate = IdManager.GetShowSingleValueCurrency("CurrencyRate", " DeleteBy IS NULL AND convert(nvarchar,CurrencyDate,103)", "CurrencySet", date);
        if (CurrencyRate > 0)
        {           
            btnSave.Visible = true;
        }
        else
        {
            btnSave.Visible = false;
            throw new Exception("Currency Not Set For today, So You cannot Enter any Voucher....!!");
            
        }
        return CurrencyRate;
    }
}