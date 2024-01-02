using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using Delve;

/// <summary>
/// Summary description for clsBranchManager
/// </summary>
/// 
namespace Delve
{
    public class clsBranchManager
    {
        public static DataTable GetBranchInfos()
        {
            string connectionString = DataManager.OraConnString();

            string query = "select ComKey, BranchKey, BranchName, ShortName, Address1, Phone, Mobile, Fax, EMail, IssuingPlace, Computerized, Status from BranchInfo order by BranchKey, ComKey";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "branch_info");
            return dt;
        }
        public static DataTable GetBranchInfosGrid(string branch)
        {
            string connectionString = DataManager.OraConnString();
            string Parameter = "";
            if (!string.IsNullOrEmpty(branch))
            {
                Parameter = " where ID = '" + branch + "' ";
            }
            string query = "select ID,ComKey, BranchKey, BranchName, ShortName, Address1, Phone, Mobile, Fax, EMail, IssuingPlace, Computerized, Status from BranchInfo " + Parameter;
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "branch_info");
            return dt;
        }
        public static DataTable GetClientInfosGridByBranchByName(string branch, string name)
        {
            string connectionString = DataManager.OraConnString();

            string query = "select ComKey, BranchKey, BranchName, ShortName, Address1, Phone, Mobile, Fax, EMail, IssuingPlace, Computerized, Status from BranchInfo a where BranchKey like '%" + branch + "%' and upper(ClientName) like upper('%" + name + "%') order by BranchKey, ComKey";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "branch_info");
            return dt;
        }
        public static void CreateBranchInfo(clsBranchSetup bi)
        {
            String connectionString = DataManager.OraConnString();
            string ParFiend = "", parVal = "";
            if(!string.IsNullOrEmpty(bi.Flag))
            {
                ParFiend=" , Flag";parVal= " , " +bi.Flag;
            }
            string query = " insert into BranchInfo(ComKey, BranchKey, BranchName, ShortName, Address1, Phone, Mobile, Fax, EMail, IssuingPlace, Computerized, Status " + ParFiend + ") values (" +
            " '" + bi.ComKey + "',  " +
            " '" + bi.BranchKey + "', " +
            " '" + bi.BranchName + "',  " +
            " '" + bi.ShortName + "',  " +
            " '" + bi.Address1 + "',  " +
            " '" + bi.Phone + "',  " +
            " '" + bi.Mobile + "',  " +
            " '" + bi.Fax + "',  " +
            " '" + bi.EMail + "',  " +
            " '" + bi.IssuingPlace + "',  " +
            " '" + bi.Computerized + "',  " +
            " '" + bi.Status + "' " + parVal + ")"; 
            DataManager.ExecuteNonQuery(connectionString, query);

        }
        public static void UpdateBranchInfo(clsBranchSetup bi)
        {
            String connectionString = DataManager.OraConnString();
            string parVal = "";
            if (!string.IsNullOrEmpty(bi.Flag))
            {
                parVal = " , Flag = '" + bi.Flag+"' ";
            }
            string query = @" update BranchInfo set BranchName= '" + bi.BranchName + "',ShortName='" + bi.ShortName + "',BranchKey= '" + bi.BranchKey + "',Address1= '" + bi.Address1 + "',Phone= '" + bi.Phone + "',  Mobile= '" + bi.Mobile + "', Fax= '" + bi.Fax + "', EMail= '" + bi.EMail + "', IssuingPlace= '" + bi.IssuingPlace + "', Computerized= '" + bi.Computerized + "',  Status= '" + bi.Status + "' " + parVal + " where ID= '" + bi.ID + "' ";
            DataManager.ExecuteNonQuery(connectionString, query);

        }
        public static void DeleteBranchInfo(clsBranchSetup bi)
        {
            String connectionString = DataManager.OraConnString();
            string query = " delete from BranchInfo where ID ='" + bi.BranchKey + "'";
            DataManager.ExecuteNonQuery(connectionString, query);

        }
        public static clsBranchSetup GetBranchInfo(string br)
        {
            string connectionString = DataManager.OraConnString();

            string query = "select ComKey, BranchKey, BranchName, ShortName, Address1, Phone, Mobile, Fax, EMail, IssuingPlace, Computerized, Status,Flag from BranchInfo where ID = '" + br + "' ";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Branch");

            if (dt.Rows.Count == 0)
            {
                return null;
            }
            return new clsBranchSetup(dt.Rows[0]);
        }
        public static clsBranchSetup GetBranchInfo1(string br)
        {
            string connectionString = DataManager.OraConnString();

            string query = "select ComKey, BranchKey, BranchName, ShortName, Address1, Phone, Mobile, Fax, EMail, IssuingPlace, Computerized, Status,Flag from BranchInfo where ID='" + br + "'";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Branch");

            if (dt.Rows.Count == 0)
            {
                return null;
            }
            return new clsBranchSetup(dt.Rows[0]);
        }
        public static string getBranchNameAddress(string key)
        {
            string val="";
            string connectionString = DataManager.OraConnString();
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                string query = " select branchname+'\n'+coalesce(Address1,'') branchname from branchinfo where branchkey='" + key + "' ";
                sqlCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, sqlCon))
                {
                    object maxValue = myCommand.ExecuteScalar();
                    if (maxValue == DBNull.Value) return "";
                    val = maxValue.ToString();
                }
            }
            return val;
        }

        public static string getBranchName(string key)
        {
            string val="";
            string connectionString = DataManager.OraConnString();
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                string query = " select branchname branchname from branchinfo where branchkey='" + key + "' ";
                sqlCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, sqlCon))
                {
                    object maxValue = myCommand.ExecuteScalar();
                    if (maxValue == DBNull.Value) return "";
                    val = maxValue.ToString();
                }
            }
            return val;
        }

        public static string getBranchShortName(string key)
        {
            string val="";
            string connectionString = DataManager.OraConnString();
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                string query = " select ShortName from branchinfo where branchkey='" + key + "' ";
                sqlCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, sqlCon))
                {
                    object maxValue = myCommand.ExecuteScalar();
                    if (maxValue == DBNull.Value) return "";
                    val = maxValue.ToString();
                }
            }
            return val;
        }

        public static DataTable getBranchRpt(string branch)
        {
            String connectionString = DataManager.OraConnString();
            string query = " select branchKey,initcap(branchname) branchname, address1 from branchInfo where branchKey like '%"+branch+"%' order by convert(numeric,branchKey)";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Branches");
            return dt;
        }

        public DataTable getBranch(string Prameter)
        {
            String connectionString = DataManager.OraConnString();
            string query = @"SELECT [ID]
                          ,[Gl_CoaCode]
                          ,[Gl_CoaDesc]
                          ,[BranchName]
                          ,[ShortName]
                          ,[Address1]
                          ,[Mobile]
                          ,[BranchSearch]
                          FROM [View_BranchInfo] " + Prameter;
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Branches");
            return dt;
        }
        public void SaveBranchDocument(string FileDescription, byte[] PasportFilbytes, string Mst_ID,string LoginBy)
        {
            String connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);

            string variables = "[Mst_ID],[FileDescription],AddBy,AddDate";
            string values = "'" + Mst_ID + "','" + FileDescription + "','" +
                            LoginBy + "',GETDATE()";

            string query = "";
            if (FileDescription != null)
            {
                if (FileDescription.Length > 0)
                {
                    variables = variables + ",[FileImage]";
                    values = values + ",@img";
                }
            }
            query = " insert into BranchInfoDocument (" + variables + ")  values (" + values + ")";
            SqlCommand cmnd;
            cmnd = new SqlCommand(query, sqlCon);
            SqlParameter file = new SqlParameter();
            file.SqlDbType = SqlDbType.VarBinary;
            file.ParameterName = "img";
            file.Value = PasportFilbytes;
            cmnd.Parameters.Add(file);
            if (PasportFilbytes == null)
            {
                cmnd.Parameters.Remove(file);
            }
            else
            {
                if (PasportFilbytes.Length == 0)
                {
                    cmnd.Parameters.Remove(file);
                }
            }
            sqlCon.Open();
            cmnd.ExecuteNonQuery();
            sqlCon.Close();
        }

        public DataTable getShowBranchDocument(string MstID,string dtlID)
        {
            String connectionString = DataManager.OraConnString();
            string Parameter = "";
            if (string.IsNullOrEmpty(dtlID))
            {
                Parameter = " and [Mst_ID]='" +MstID + "' ";
            }
            else
            {
                Parameter = " and [Mst_ID]='" + MstID + "' and ID='" + dtlID + "' ";
            }
            string query = @"SELECT [ID],[FileDescription],[FileImage] FROM [BranchInfoDocument] where  [DeleteBy] IS NULL " + Parameter;
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Branches");
            return dt;
        }

        public void DeleteBranchDocument(string ID,string LoginBy)
        {
            String connectionString = DataManager.OraConnString();
            string query = @"update BranchInfoDocument set DeleteBy='" + LoginBy + "',DeleteDate=GETDATE() where ID='" +
                           ID + "' ";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
    }
}