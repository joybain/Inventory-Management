using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for ConfigurationModel
/// </summary>
public class ManuConfigurationModel
{
    public ManuConfigurationModel()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public int Id { get; set; }
    public int MenuId { get; set; }
    public int ParentMenuId { get; set; }
    public string Name { get; set; }
    public string Path { get; set; }
    public string Priority { get; set; }
  
    public string LoginBy { get; set; }
}