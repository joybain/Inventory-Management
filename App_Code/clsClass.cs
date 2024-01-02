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
/// Summary description for clsClass
/// </summary>
public class clsClass
{
    public string ClassId;
    public string ClassName;

    public clsClass()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public clsClass(DataRow dr)
    {
        if (dr["class_id"].ToString() != string.Empty)
        {
            this.ClassId = dr["class_id"].ToString();
        }
        if (dr["class_name"].ToString() != string.Empty)
        {
            this.ClassName = dr["class_name"].ToString();
        }
    }
}
