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
/// Summary description for Edu
/// </summary>
public class Edu
{
    public string EmpNo;
    public string ExamCode;
    public string GroupName;
    public string Institute;
    public string PassYear;
    public string MainSub;
    public string DivClass;

    public Edu()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public Edu(DataRow dr)
    {
        if (dr["emp_no"].ToString() != String.Empty)
        {
            this.EmpNo = dr["emp_no"].ToString();
        }
        if (dr["exam_code"].ToString() != String.Empty)
        {
            this.ExamCode = dr["exam_code"].ToString();
        }
        if (dr["group_name"].ToString() != String.Empty)
        {
            this.GroupName = dr["group_name"].ToString();
        }
        if (dr["institute"].ToString() != String.Empty)
        {
            this.Institute = dr["institute"].ToString();
        }
        if (dr["pass_year"].ToString() != String.Empty)
        {
            this.PassYear = dr["pass_year"].ToString();
        }
        if (dr["main_sub"].ToString() != String.Empty)
        {
            this.MainSub = dr["main_sub"].ToString();
        }
        if (dr["div_class"].ToString() != String.Empty)
        {
            this.DivClass = dr["div_class"].ToString();
        }
    }
}
