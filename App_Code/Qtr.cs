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
/// Summary description for Qtr
/// </summary>
public class Qtr
{
    public string EmpNo;
    public string AllotRef;
    public string RefDate;
    public string PostDate;
    public string Locat;
    public string Road;
    public string Build;
    public string Flat;
    public string FlatTyp;
    public string Sizee;

    public Qtr()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public Qtr(DataRow dr)
    {
        if (dr["emp_no"].ToString() != String.Empty)
        {
            this.EmpNo = dr["emp_no"].ToString();              
        }
        if (dr["allot_ref"].ToString() != String.Empty)
        {
            this.AllotRef = dr["allot_ref"].ToString();
        }
        if (dr["ref_date"].ToString() != String.Empty)
        {
            this.RefDate = dr["ref_date"].ToString();
        }
        if (dr["post_date"].ToString() != String.Empty)
        {
            this.PostDate = dr["post_date"].ToString();
        }
        if (dr["locat"].ToString() != String.Empty)
        {
            this.Locat = dr["locat"].ToString();
        }
        if (dr["road"].ToString() != String.Empty)
        {
            this.Road = dr["road"].ToString();
        }
        if (dr["build"].ToString() != String.Empty)
        {
            this.Build = dr["build"].ToString();
        }
        if (dr["flat"].ToString() != String.Empty)
        {
            this.Flat = dr["flat"].ToString();
        }
        if (dr["flat_typ"].ToString() != String.Empty)
        {
            this.FlatTyp = dr["flat_typ"].ToString();
        }
        if (dr["sizee"].ToString() != String.Empty)
        {
            this.Sizee = dr["sizee"].ToString();
        }
    }
}
