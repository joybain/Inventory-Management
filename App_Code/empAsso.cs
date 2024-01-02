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
/// Summary description for empAsso
/// </summary>
public class empAsso
{
    public string EmpNo;
    public string AssoId;
    public string MemberNo;

    public empAsso()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public empAsso(DataRow dr)
    {
        if (dr["emp_no"].ToString() != String.Empty)
        {
            this.EmpNo = dr["emp_no"].ToString();
        }
        if (dr["asso_id"].ToString() != String.Empty)
        {
            this.AssoId = dr["asso_id"].ToString();
        }
        if (dr["member_no"].ToString() != String.Empty)
        {
            this.MemberNo = dr["member_no"].ToString();
        }
    }
}
