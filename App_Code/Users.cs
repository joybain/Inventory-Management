using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;

/// <summary>
/// Summary description for Users
/// </summary>
public class Users
{
    public string UserName;
    public string Password;
    public string Description;
    public string UserGrp;
    public string Status;
    public string EmpNo;
    public string Dept, UserType;

    public string BranchId;

    public Users()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public Users(DataRow dr)
    {
        if (dr["user_name"].ToString() != String.Empty)
        {
            this.UserName = dr["user_name"].ToString();
        }
        if (dr["password"].ToString() != String.Empty)
        {
            this.Password = dr["password"].ToString();
        }
        if (dr["description"].ToString() != String.Empty)
        {
            this.Description = dr["description"].ToString();
        }
        if (dr["user_grp"].ToString() != String.Empty)
        {
            this.UserGrp = dr["user_grp"].ToString();
        }
        if (dr["status"].ToString() != String.Empty)
        {
            this.Status = dr["status"].ToString();
        }
        if (dr["emp_no"].ToString() != String.Empty)
        {
            this.EmpNo = dr["emp_no"].ToString();
        }
        if (dr["dept"].ToString() != String.Empty)
        {
            this.Dept = dr["dept"].ToString();
        }
        if (dr["UserType"].ToString() != String.Empty)
        {
            this.UserType = dr["UserType"].ToString();
        }
        if (dr["BranchId"].ToString() != String.Empty)
        {
            this.BranchId = dr["BranchId"].ToString();
        }
    }

    public string LoginBy { get; set; }

    public string GroupID { get; set; }

}
