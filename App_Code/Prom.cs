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
/// Summary description for Prom
/// </summary>
public class Prom
{
    public string EmpNo;
    public string OffOrdNo;
    public string JoiningDate;
    public string JoiningBranch;
    public string JoiningDesig;
    public string BasicPay;
    public string PayScale;

    public Prom()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public Prom(DataRow dr)
    {
        if (dr["emp_no"].ToString() != String.Empty)
        {
            this.EmpNo = dr["emp_no"].ToString();
        }
        if (dr["off_ord_no"].ToString() != String.Empty)
        {
            this.OffOrdNo = dr["off_ord_no"].ToString();
        }
        if (dr["joining_date"].ToString() != String.Empty)
        {
            this.JoiningDate = dr["joining_date"].ToString();
        }
        if (dr["joining_branch"].ToString() != String.Empty)
        {
            this.JoiningBranch = dr["joining_branch"].ToString();
        }
        if (dr["joining_desig"].ToString() != String.Empty)
        {
            this.JoiningDesig = dr["joining_desig"].ToString();
        }
        if (dr["basic_pay"].ToString() != String.Empty)
        {
            this.BasicPay = dr["basic_pay"].ToString();
        }
        if (dr["pay_scale"].ToString() != String.Empty)
        {
            this.PayScale = dr["pay_scale"].ToString();
        }
    }
}
