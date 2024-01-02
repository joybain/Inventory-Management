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
/// Summary description for Fam
/// </summary>
public class Fam
{
    public string EmpNo;
    public string RelName;
    public string Relation;
    public string BirthDt;
    public string Age;
    public string Occupation;
    public Fam()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public Fam(DataRow dr)
    {
        if (dr["emp_no"].ToString() != String.Empty)
        {
            this.EmpNo = dr["emp_no"].ToString();
        }
        if (dr["rel_name"].ToString() != String.Empty)
        {
            this.RelName = dr["rel_name"].ToString();
        }
        if (dr["relation"].ToString() != String.Empty)
        {
            this.Relation = dr["relation"].ToString();
        }
        if (dr["birth_dt"].ToString() != String.Empty)
        {
            this.BirthDt = dr["birth_dt"].ToString();
        }
        if (dr["age"].ToString() != String.Empty)
        {
            this.Age = dr["age"].ToString();
        }
        if (dr["occupation"].ToString() != String.Empty)
        {
            this.Occupation = dr["occupation"].ToString();
        }
    }
}
