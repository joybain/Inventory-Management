using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// Summary description for clsSetAuthLevel
/// </summary>
public class clsSetAuthLevel
{
    public string Dept,ModId, AuthLevel;

	public clsSetAuthLevel()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public clsSetAuthLevel(DataRow dr)
    {
        if (dr["dept"].ToString() != string.Empty)
        {
            this.Dept = dr["dept"].ToString();
        }
        if (dr["mod_id"].ToString() != string.Empty)
        {
            this.ModId = dr["mod_id"].ToString();
        }
        if (dr["auth_level"].ToString() != string.Empty)
        {
            this.AuthLevel = dr["auth_level"].ToString();
        }
    }

}