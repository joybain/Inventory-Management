using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for MenueModel
/// </summary>
public class MenuModel
{
	public MenuModel()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public int Id { get; set; }
    public string Name { get; set; }

    public string Path { get; set; }
    public string LoginBy { get; set; }
}