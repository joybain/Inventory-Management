using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

/// <summary>
/// Summary description for Susp
/// </summary>
public class Susp
{
    public string EmpNo;
    public string OffOrderNo;
    public string SuspenDate;
    public string SuspenClause;
    public string WithdrawOrderNo;
    public string WithDate;
    public string Punishment;

    public Susp()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public Susp(DataRow dr)
    {
        if (dr["emp_no"].ToString() != String.Empty)
        {
            this.EmpNo = dr["emp_no"].ToString();
        }
        if (dr["off_order_no"].ToString() != String.Empty)
        {
            this.OffOrderNo = dr["off_order_no"].ToString();
        }
        if (dr["suspen_date"].ToString() != String.Empty)
        {
            this.SuspenDate = dr["suspen_date"].ToString();
        }
        if (dr["suspen_clause"].ToString() != String.Empty)
        {
            this.SuspenClause = dr["suspen_clause"].ToString();
        }
        if (dr["withdraw_order_no"].ToString() != String.Empty)
        {
            this.WithdrawOrderNo = dr["withdraw_order_no"].ToString();
        }
        if (dr["with_date"].ToString() != String.Empty)
        {
            this.WithDate = dr["with_date"].ToString();
        }
        if (dr["punishment"].ToString() != String.Empty)
        {
            this.Punishment = dr["punishment"].ToString();
        }
    }
}
