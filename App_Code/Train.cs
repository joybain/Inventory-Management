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
/// Summary description for Train
/// </summary>
public class Train
{
	public string EmpNo;
    public string TrainTitle;
    public string Year;
    public string Place;
    public string Country;
    public string Finan;
    public string Amount;
    public string DuYear;
    public string DuMonth;
    public string DuDay;

    public Train()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public Train(DataRow dr)
    {
        if (dr["emp_no"].ToString() != String.Empty)
        {
            this.EmpNo = dr["emp_no"].ToString();
        }
        if (dr["train_title"].ToString() != String.Empty)
        {
            this.TrainTitle = dr["train_title"].ToString();
        }
        if (dr["year"].ToString() != String.Empty)
        {
            this.Year = dr["year"].ToString();
        }
        if (dr["place"].ToString() != String.Empty)
        {
            this.Place = dr["place"].ToString();
        }
        if (dr["country"].ToString() != String.Empty)
        {
            this.Country = dr["country"].ToString();
        }
        if (dr["finan"].ToString() != String.Empty)
        {
            this.Finan = dr["finan"].ToString();
        }
        if (dr["amount"].ToString() != String.Empty)
        {
            this.Amount = dr["amount"].ToString();
        }
        if (dr["du_year"].ToString() != String.Empty)
        {
            this.DuYear = dr["du_year"].ToString();
        }
        if (dr["du_month"].ToString() != String.Empty)
        {
            this.DuMonth = dr["du_month"].ToString();
        }
        if (dr["du_day"].ToString() != String.Empty)
        {
            this.DuDay = dr["du_day"].ToString();
        }
    }
}
