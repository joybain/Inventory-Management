using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;
using System.Data.SqlClient;
using Delve;


/// <summary>
/// Summary description for MapManager
/// </summary>
namespace Delve
{
    public class MapManager
    {
        public static void DeleteMapMaster(MapMst mapmst)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection oracon = new SqlConnection(connectionString);
            string query = "delete from gl_st_map_mst where type_code='" + mapmst.TypeCode + "'";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static void CreateMapMaster(MapMst mapmst)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection oracon = new SqlConnection(connectionString);
            string query="insert into gl_st_map_mst (book_name,type_code,ver_no,description,ref_type_code,ref_ver_no,active,entry_user,entry_date) "+
                " values ('"+mapmst.BookName+"', '"+mapmst.TypeCode+"', '"+mapmst.VerNo+"', '"+mapmst.Description+"', "+
            "  '"+mapmst.RefTypeCode+"', '"+mapmst.RefVerNo+"','"+mapmst.Active+"','"+mapmst.EntryUser+"',convert(datetime,'"+mapmst.EntryDate+"',103))";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static void UpdateMapMaster(MapMst mapmst)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection oracon = new SqlConnection(connectionString);
            string query = "update gl_st_map_mst set ver_no='" + mapmst.VerNo + "',description='" + mapmst.Description + "', " +
                " ref_type_code='" + mapmst.RefTypeCode + "', ref_ver_no='" + mapmst.RefVerNo + "',active='" + mapmst.Active + "',autho_user='" + mapmst.AuhtoUser + "',autho_date=convert(datetime,case '" + mapmst.AuthoDate + "' when '' then null else '" + mapmst.AuthoDate + "' end,103) where type_code='" + mapmst.TypeCode + "'";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static MapMst GetMapMaster(string typecode)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection oracon = new SqlConnection(connectionString);
            string query = "select book_name,type_code,ver_no,description,ref_type_code,ref_ver_no,active,entry_user,convert(varchar,entry_date,103) entry_date,autho_user,convert(varchar,autho_date,103) autho_date from gl_st_map_mst where type_code= + '" + typecode + "'";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "st_map_mst");
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            return new MapMst(dt.Rows[0]);
        }
        public static DataTable GetMapMasters()
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection oracon = new SqlConnection(connectionString);
            string query = "select type_code,ver_no,description,ref_type_code,ref_ver_no,active from gl_st_map_mst order by ver_no ";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "st_map_mst");
            return dt;
        }
        public static void DeleteMapDetails(string typecode)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection oracon = new SqlConnection(connectionString);
            string query = "delete from gl_st_map_dtl where type_code='" + typecode + "'";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static void DeleteMapDetail(MapDtl mapdtl)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection oracon = new SqlConnection(connectionString);
            string query = "delete from gl_st_map_dtl where type_code='" + mapdtl.TypeCode + "' and sl_no=" + mapdtl.SlNo + " ";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static void CreateMapDetail(MapDtl mapdtl)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection oracon = new SqlConnection(connectionString);
            string query = "insert into gl_st_map_dtl (book_name,type_code,ver_no,sl_no,gl_seg_code,description,add_less,bal_from,cons_amt) " +
                " values ('" + mapdtl.BookName + "','" + mapdtl.TypeCode + "','" + mapdtl.VerNo + "', convert (numeric, case '" + mapdtl.SlNo + "' when '' then null else '" + mapdtl.SlNo + "' end), " +
                    " '" + mapdtl.GlSegCode + "','" + mapdtl.Description + "','" + mapdtl.AddLess + "','" + mapdtl.BalFrom + "', convert(numeric, case '" + mapdtl.ConsAmt.Replace(",", "") + "'  when '' then null else '" + mapdtl.ConsAmt.Replace(",", "") + "' end ))";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static void UpdateMapDetail(MapDtl mapdtl)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection oracon = new SqlConnection(connectionString);
            string query = "update gl_st_map_dtl set gl_seg_code='" + mapdtl.GlSegCode + "', " +
                " description='" + mapdtl.Description + "',add_less='" + mapdtl.AddLess + "',bal_from='" + mapdtl.BalFrom + "', " +
                " cons_amt= convert(numeric,case '" + mapdtl.ConsAmt.Replace(",", "") + "' when '' then null else '" + mapdtl.ConsAmt.Replace(",", "") + "' end ) where type_code='" + mapdtl.TypeCode + "' and sl_no= convert (numeric, case '" + mapdtl.SlNo + "' when '' then null else '" + mapdtl.SlNo + "' end )";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static DataTable GetMapDetails(string typecode)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection oracon = new SqlConnection(connectionString);
            string query = "select sl_no,gl_seg_code,description,bal_from,bal_from,add_less,convert(varchar,cons_amt) cons_amt from gl_st_map_dtl where type_code= + '" + typecode + "' order by sl_no";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "st_map_dtl");
            return dt;
        }
        public static MapDtl GetMapDetail(string typecode,string sl)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection oracon = new SqlConnection(connectionString);
            string query = "select book_name,type_code,ver_no,sl_no,gl_seg_code,description,bal_from,add_less,convert(varchar,cons_amt) cons_amt from gl_st_map_dtl where type_code= + '" + typecode + "' and sl_no='" + sl + "' ";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "st_map_dtl");
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            return new MapDtl(dt.Rows[0]);
        }
        public static DataTable GetMapBreaks(string typecode, string refslno)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection oracon = new SqlConnection(connectionString);
            string query = "select ref_sl_no,sl_no,gl_seg_code,ADD_LESS from gl_st_break_dtl where type_code='" + typecode + "' and ref_sl_no='" + refslno + "' order by sl_no";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "st_break_dtl");
            return dt;
        }
        public static DataTable GetMapBreakAll(string typecode)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection oracon = new SqlConnection(connectionString);
            string query = "select ref_sl_no,sl_no,gl_seg_code from gl_st_break_dtl where type_code='" + typecode + "' order by sl_no";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "st_break_dtl");
            return dt;
        }
        public static MapBreak GetMapBreak(string typecode, string refslno,string slno)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection oracon = new SqlConnection(connectionString);
            string query = "select book_name,type_code,ver_no,ref_sl_no,sl_no,gl_seg_code from gl_st_break_dtl where type_code='" + typecode + "' and ref_sl_no='" + refslno + "' and sl_no='" + slno + "' ";
            DataTable dt = DataManager.ExecuteQuery(connectionString, query, "st_break_dtl");
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            return new MapBreak(dt.Rows[0]);
        }
        public static void DeleteMapBreak(string typecode,string refsl)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection oracon = new SqlConnection(connectionString);
            string query = "delete from gl_st_break_dtl where type_code= '" + typecode + "' and ref_sl_no= convert (numeric, case '" + refsl + "' when '' then null else '" + refsl + "' end ) ";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static void DeleteMapBreaks(string typecode)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection oracon = new SqlConnection(connectionString);
            string query = "delete from gl_st_break_dtl where type_code= '" + typecode + "' ";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static void UpdateMapBreak(MapBreak mapbrk)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection oracon = new SqlConnection(connectionString);
            string query = "update gl_st_break_dtl set sl_no='" + mapbrk.SlNo + "', gl_seg_code='" + mapbrk.GlSegCode + "',ADD_LESS='" + mapbrk.ADD_LESS + "' where type_code= '" + mapbrk.TypeCode + "' ";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
        public static void CreateMapBreak(MapBreak mapbrk)
        {
            string connectionString = DataManager.OraConnString();
            SqlConnection oracon = new SqlConnection(connectionString);
            string query = "insert into gl_st_break_dtl (book_name,type_code,ver_no,ref_sl_no,sl_no,gl_seg_code,ADD_LESS) " +
                " values ('" + mapbrk.BookName + "','" + mapbrk.TypeCode + "','" + mapbrk.VerNo + "', convert (numeric, case '" + mapbrk.RefSlNo + "' when '' then null else '" + mapbrk.RefSlNo + "' end) , " +
            " convert (numeric, case '" + mapbrk.SlNo + "' when '' then null else '" + mapbrk.SlNo + "' end),'" + mapbrk.GlSegCode + "','" + mapbrk.ADD_LESS + "')";
            DataManager.ExecuteNonQuery(connectionString, query);
        }
    }
}