using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;
using System.Data.SqlClient;
using Delve;

/// <summary>
/// Summary description for UsersManager
/// </summary>
/// 
namespace Delve
{
    public class UsersManager
    {
        public static void CreateUser(Users usr)
        {
            String connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "insert into utl_userinfo (user_name,password,description,user_grp,status,emp_no,dept,UserType,BranchId) values ( " +
                " '" + usr.UserName + "', '" + usr.Password + "', '" + usr.Description + "', " +
                " '" + usr.UserGrp + "', '" + usr.Status + "', '" + usr.EmpNo + "' ,'" + usr.Dept + "','" + usr.UserType + "','"+usr.BranchId+"') ";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static void UpdateUser(Users usr)
        {
            String connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string Parameter = "";
            if (!string.IsNullOrEmpty(usr.Password))
            {
                Parameter = "password= '" + usr.Password + "',";
            }
            string query = "update utl_userinfo set " + Parameter + " description='" + usr.Description + "', " +
                " user_grp= '" + usr.UserGrp + "',BranchId='"+usr.BranchId+"', status= '" + usr.Status + "', emp_no= '" + usr.EmpNo + "',dept='" + usr.Dept + "',UserType='" + usr.UserType + "' where upper(user_name)=upper('" + usr.UserName + "')  ";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static void DeleteUser(Users usr)
        {
            String connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "delete from utl_userinfo where upper(user_name)=upper('"+usr.UserName+"')  ";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static Users getUser(string usr)
        {
            String connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);

            string query = "select * from utl_userinfo where upper(user_name)=upper('" + usr + "')";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "UserInfo");
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            return new Users(dt.Rows[0]);
        }
        public static DataTable GetUsers()
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "select upper(user_name) user_name,password,description,case user_grp when '1' then'Operator' when '2' then 'Supervisor' when '3' then 'Evaluator' when '4' then 'Administrator' when '5' then 'Director' end user_grp,user_grp usergrp,dept, " +
                " (select seg_coa_desc from gl_seg_coa where seg_coa_code=a.dept) deptname from  utl_userinfo a where status='A' order by description";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Users");
            return dt;
        }
        public static DataTable GetUserLevels()
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = "select  user_grp,case user_grp when '1' then'Operator' when '2' then 'Supervisor' when '3' then 'Evaluator' when '4' then 'Administrator' end user_grp from  utl_userinfo a where status='A' order by user_name";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Users");
            return dt;
        }
        public static string getUserName(string user)
        {
            String ConnectionString = DataManager.OraConnString();
            SqlConnection myConnection = new SqlConnection(ConnectionString);
            string Query = "select dbo.initcap(description)  from utl_userinfo where user_name='" + user + "' ";
            myConnection.Open();
            SqlCommand myCommand = new SqlCommand(Query, myConnection);
            object maxValue = myCommand.ExecuteScalar();
            myConnection.Close();
            string a = "";
            if (maxValue != null)
            {
                a = maxValue.ToString();
            }
            return a;
        }

        public static DataTable GetShowUser(string p)
        {
            String connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string Parameter = "";
            string query = @"SELECT t1.[USER_NAME],t1.[PASSWORD],t1.[DESCRIPTION],t1.[USER_GRP],t1.[STATUS],t1.[EMP_NO],t1.[Dept],t2.BranchName ,t1.[UserType],CASE WHEN t1.[UserType]=1 then 'Ban' WHEN t1.[UserType]=2 then 'Man' WHEN t1.[UserType]=3 then 'All' else '' END AS[UserTypeName]  FROM [UTL_USERINFO] t1 LEFT join BranchInfo t2 on t2.ID=t1.BranchId" + Parameter;
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Users");
            return dt;
        }

        public static void GetResetPassword(Users usr)
        {
            string OldPass = IdManager.GetShowSingleValueString("PASSWORD", "USER_NAME", "UTL_USERINFO", usr.UserName);
            String connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = @"UPDATE [UTL_USERINFO]
            SET ChangeBy='" + usr.LoginBy + "',ChangeDate=GETDATE(),OldPassword='" + OldPass + "',PASSWORD='" + usr.Password + "' WHERE USER_NAME='" + usr.UserName + "'";
            DataManager.ExecuteNonQuery(connectionString, query);
        }

        public static DataTable getShowUserInfo(string ID)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = @"SELECT [USER_GRP],[GROUP_DESC] FROM [UTL_GROUPINFO] " + ID;
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "Users");
            return dt;
        }

        public static void UpdateGroupInformation(Users aUsers)
        {
            String connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = @"UPDATE [UTL_GROUPINFO]
            SET [GROUP_DESC] ='" + aUsers.UserGrp + "',UpdateBy='" + aUsers.LoginBy + "',UpdateDate=GETDATE() WHERE USER_GRP='" + aUsers.GroupID + "'";
            DataManager.ExecuteNonQuery(connectionString, query);
        }

        public static void SaveGroupInformation(Users aUsers)
        {
            String connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = @"INSERT INTO [UTL_GROUPINFO]
           ([GROUP_DESC],AddBy,AddDate)
            VALUES
           ('" + aUsers.UserGrp + "','" + aUsers.LoginBy + "',GETDATE())";
            DataManager.ExecuteNonQuery(connectionString, query);
        }

        public static void DeleteGroupInformation(Users aUsers)
        {
            String connectionString = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(connectionString);
            string query = @"UPDATE [UTL_GROUPINFO]
            SET DeleteBy='" + aUsers.LoginBy + "',DeleteDate=GETDATE() WHERE USER_GRP='" + aUsers.GroupID + "'";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
    }
}

