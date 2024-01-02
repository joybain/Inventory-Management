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
/// Summary description for clsAsso
/// </summary>
public class clsAsso
{
    public string AssoId;
    public string AssoName;
    public string AssoAbvr;
    public clsAsso()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public clsAsso(DataRow dr)
    {
        if (dr["asso_id"].ToString() != String.Empty)
        {
            this.AssoId = dr["asso_id"].ToString();
        }
        if (dr["asso_name"].ToString() != String.Empty)
        {
            this.AssoName = dr["asso_name"].ToString();
        }
        if (dr["asso_abvr"].ToString() != String.Empty)
        {
            this.AssoAbvr = dr["asso_abvr"].ToString();
        }
    }
}
