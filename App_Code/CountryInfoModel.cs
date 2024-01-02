using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for CountryInfoModel
/// </summary>
public class CountryInfoModel
{
	public CountryInfoModel()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public int Id { get; set; }
    public string Name { get; set; }

    public string ShortName { get; set; }
    public string LoginBy { get; set; }
}