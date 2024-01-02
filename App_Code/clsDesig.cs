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
/// Summary description for clsDesig
/// </summary>
public class clsDesig
{
    public string DesigCode;
    public string DesigName;
    public string MgrCode;
    public string GradeCode;
    public string Class;
    public string TechNtech;
    public string OfficerStaff;
    public string DesigAbb;
    public string Serial;

    public clsDesig()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public clsDesig(DataRow dr)
    {
        if (dr["Serial"].ToString() != string.Empty)
        {
            this.Serial = dr["Serial"].ToString();
        }
        if (dr["desig_code"].ToString() != string.Empty)
        {
            this.DesigCode = dr["desig_code"].ToString();
        }
        if (dr["desig_name"].ToString() != string.Empty)
        {
            this.DesigName = dr["desig_name"].ToString();
        }
        if (dr["desig_abb"].ToString() != string.Empty)
        {
            this.DesigAbb = dr["desig_abb"].ToString();
        }
        if (dr["mgr_code"].ToString() != string.Empty)
        {
            this.MgrCode = dr["mgr_code"].ToString();
        }
        if (dr["grade_code"].ToString() != string.Empty)
        {
            this.GradeCode = dr["grade_code"].ToString();
        }
        if (dr["class"].ToString() != string.Empty)
        {
            this.Class = dr["class"].ToString();
        }
        if (dr["tech_ntech"].ToString() != string.Empty)
        {
            this.TechNtech = dr["tech_ntech"].ToString();
        }
        if (dr["officer_staff"].ToString() != string.Empty)
        {
            this.OfficerStaff = dr["officer_staff"].ToString();
        }
    }
}
