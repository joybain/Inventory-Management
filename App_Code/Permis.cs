using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;

/// <summary>
/// Summary description for Permis
/// </summary>
public class Permis
{
    public string UserName;
    public string ModId;
    public string AllowAdd;
    public string AllowEdit;
    public string AllowView;
    public string AllowDelete;
    public string AllowPrint;
    public string AllowAutho;

    public Permis()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public Permis(DataRow dr)
    {
        if (dr["user_name"].ToString() != String.Empty)
        {
            this.UserName = dr["user_name"].ToString();
        }
        if (dr["mod_id"].ToString() != String.Empty)
        {
            this.ModId = dr["mod_id"].ToString();
        }
        if (dr["allow_add"].ToString() != String.Empty)
        {
            this.AllowAdd = dr["allow_add"].ToString();
        }
        if (dr["allow_edit"].ToString() != String.Empty)
        {
            this.AllowEdit = dr["allow_edit"].ToString();
        }
        if (dr["allow_view"].ToString() != String.Empty)
        {
            this.AllowView = dr["allow_view"].ToString();
        }
        if (dr["allow_delete"].ToString() != String.Empty)
        {
            this.AllowDelete = dr["allow_delete"].ToString();
        }
        if (dr["allow_print"].ToString() != String.Empty)
        {
            this.AllowPrint = dr["allow_print"].ToString();
        }
        if (dr["allow_autho"].ToString() != String.Empty)
        {
            this.AllowAutho = dr["allow_autho"].ToString();
        }
    }
}
