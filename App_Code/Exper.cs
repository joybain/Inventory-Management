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
/// Summary description for Exper
/// </summary>
public class Exper
{
    public string EmpNo;
    public string OrgaName;
    public string PositionHeld;
    public string FromDt;
    public string ToDt;
    public string PayScale;

    public Exper()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public Exper(DataRow dr)
    {
        if (dr["emp_code"].ToString() != String.Empty)
        {
            this.EmpNo = dr["emp_code"].ToString();
        }
        if (dr["orga_name"].ToString() != String.Empty)
        {
            this.OrgaName = dr["orga_name"].ToString();
        }
        if (dr["position_held"].ToString() != String.Empty)
        {
            this.PositionHeld = dr["position_held"].ToString();
        }
        if (dr["from_dt"].ToString() != String.Empty)
        {
            this.FromDt = dr["from_dt"].ToString();
        }
        if (dr["to_dt"].ToString() != String.Empty)
        {
            this.ToDt = dr["to_dt"].ToString();
        }
        if (dr["pay_scale"].ToString() != String.Empty)
        {
            this.PayScale = dr["pay_scale"].ToString();
        }
    }
}
