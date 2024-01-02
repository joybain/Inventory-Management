using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// Summary description for clsSession
/// </summary>
public class clsSession
{
    public string SessionId, SessionTime, Mac, UserId;
	public clsSession()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public clsSession(DataRow dr)
    {
        if (dr["session_id"].ToString() != string.Empty)
        {
            this.SessionId = dr["session_id"].ToString();
        }
        if (dr["session_time"].ToString() != string.Empty)
        {
            this.SessionTime = dr["session_time"].ToString();
        }
        if (dr["mac"].ToString() != string.Empty)
        {
            this.Mac = dr["mac"].ToString();
        }
        if (dr["user_id"].ToString() != string.Empty)
        {
            this.UserId = dr["user_id"].ToString();
        }
    }
}