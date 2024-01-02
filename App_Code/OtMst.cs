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
/// Summary description for OtMst
/// </summary>
public class OtMst
{
    public string OverId;
    public string ODate;
    public string BranchId;
    public string OverMon;

	public OtMst()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public OtMst(DataRow dr)
    {
        if (dr["over_id"].ToString() != String.Empty)
        {
            this.OverId = dr["over_id"].ToString();
        }
        if (dr["o_date"].ToString() != String.Empty)
        {
            this.ODate = dr["o_date"].ToString();
        }
        if (dr["BranchKey"].ToString() != String.Empty)
        {
            this.BranchId = dr["BranchKey"].ToString();
        }
        if (dr["over_mon"].ToString() != String.Empty)
        {
            this.OverMon = dr["over_mon"].ToString();
        }        
    }
}
