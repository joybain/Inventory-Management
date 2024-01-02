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
/// Summary description for OtDtl
/// </summary>
public class OtDtl
{
    public string OverId;
    public string EmployeeId;
    public string OtSingle;
    public string OtDouble;
    public string OtAmount;

    public OtDtl()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public OtDtl(DataRow dr)
    {
        if (dr["over_id"].ToString() != String.Empty)
        {
            this.OverId = dr["over_id"].ToString();
        }
        if (dr["employee_id"].ToString() != String.Empty)
        {
            this.EmployeeId = dr["employee_id"].ToString();
        }
        if (dr["ot_single"].ToString() != String.Empty)
        {
            this.OtSingle = dr["ot_single"].ToString();
        }
        if (dr["ot_double"].ToString() != String.Empty)
        {
            this.OtDouble = dr["ot_double"].ToString();
        }
        if (dr["OtAmount"].ToString() != String.Empty)
        {
            this.OtAmount = dr["OtAmount"].ToString();
        }
    }
}
