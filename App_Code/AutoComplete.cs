using System;
//using System.Activities.Expressions;
using System.Collections.Generic;
using System.Web.Services;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Delve;
//using System.Web.Script.Services;


[WebService(Namespace = "http://simran.com/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[System.Web.Script.Services.ScriptService]

public class AutoComplete : WebService
{
    public AutoComplete()
    { 
        
    }



    //*********************  Search Items On Purchase ********//
    [WebMethod(EnableSession = true)]
    public string[] GetSearch_Items_On_Purchase(string prefixText, int count)
    {

        if (count == 0)
        {
            count = 10;
        }

        DataTable dt = GetItemsOnPurchase(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            string str = dt.Rows[i][0].ToString();
            items.Add(str);
        }

        return items.ToArray();

    }

    public DataTable GetItemsOnPurchase(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = @"SELECT [SearchItem] FROM [View_SearchItemOnPurchase] where upper([SearchItem]) like upper('%" +
                       strName + "%') ";

        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }

    [WebMethod(EnableSession = true)]
    public string[] GetInvoiceSearch(string prefixText, int count)
    {

        if (count == 0)
        {
            count = 10;
        }

        DataTable dt = GetGetInvoiceSearch(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            string str = dt.Rows[i][0].ToString();
            items.Add(str);
        }

        return items.ToArray();

    }

    public DataTable GetGetInvoiceSearch(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = @" Select Convert(nvarchar,OrderDate,103)+'-'+InvoiceNo from [Order] where upper (Convert(nvarchar,OrderDate,103)+'-'+InvoiceNo) like upper('%" +
                       strName + "%') ";

        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }

    ////Branch Invocie Search

    [WebMethod(EnableSession = true)]
    public string[] GetBranchInvoiceSearch(string prefixText, int count)
    {
        if (Session["BranchId"] != null)
        {

            if (count == 0)
            {
                count = 10;
            }

            DataTable dt = GetGetBranchInvoiceSearch(prefixText);

            List<string> items = new List<string>(count);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string str = dt.Rows[i][0].ToString();
                items.Add(str);
            }

            return items.ToArray();
        }

        return null;

    }
    public DataTable GetGetBranchInvoiceSearch(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = @" Select Convert(nvarchar,OrderDate,103)+'-'+InvoiceNo from [Order] where upper (Convert(nvarchar,OrderDate,103)+'-'+InvoiceNo) like upper('%" + strName + "%')";

        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }
    //




    //********************* Branch  Items Search ********//
    [WebMethod(EnableSession = true)]
    public string[] GetBranchItemsSearch(string prefixText, int count)
    {

        if (Session["BranchId"] != null)
        {
            if (count == 0)
            {
                count = 10;
            }

            DataTable dt = GetBranchAllitems(prefixText, Session["BranchId"].ToString());

            List<string> items = new List<string>(count);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string str = dt.Rows[i][0].ToString();
                items.Add(str);
            }

            return items.ToArray();
        }

        return null;

    }

    public DataTable GetBranchAllitems(string strName, string BranchId)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = @"SELECT [CodeWiseSearchItems] FROM [View_Search_Items_On_OutLetStock] where upper([CodeWiseSearchItems]) like upper('%" +
                       strName + "%') and BranchID='" + BranchId + "' ";

        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }

  














    [WebMethod(EnableSession = true)]
    public string[] GetItemDetails(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GetItems(prefixText, Session["user"].ToString());
        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            string str = dt.Rows[i][0].ToString();
            items.Add(str);
        }
        return items.ToArray();
    }

    public DataTable GetItems(string strName, string User)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = "";
        DataTable dt = null;
        Users usr = UsersManager.getUser(User.ToUpper());
        if (usr != null)
        {

            query =
                @"select SearchItem from View_SearchItemsInformation where upper(SearchItem) like upper('%" +
                strName + "%') and [StockQty]>0 And DeleteBy IS NULL and ItemType='F'";
            dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        }
        return dt;
    }






    //************** Search Expenses Details ***************//
    [WebMethod]
    public string[] GetItemExpensesSearch(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GetItemExpenses(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }

    public DataTable GetItemExpenses(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        //string query = "SELECT [USER_NAME]+' - '+[DESCRIPTION] FROM [dbo].[UTL_USERINFO]   where upper([USER_NAME]+' - '+[DESCRIPTION]) like upper('%" + strName + "%') ";
        string Query = @"SELECT [Search] FROM [View_ExpenseDetails] where upper([Search]) like upper('%" + strName + "%') ";
        DataTable dt = DataManager.ExecuteQuery(strConn, Query, "autoname");
        return dt;
    }

    //********************* Items Search ********//
    [WebMethod(EnableSession = true)]
    public string[] GetItemsSetupInfoSearch(string prefixText, int count)
    {

        if (count == 0)
        {
            count = 10;
        }

        DataTable dt = GetItemsSetupInfo(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            string str = dt.Rows[i][0].ToString();
            items.Add(str);
        }

        return items.ToArray();

    }

    public DataTable GetItemsSetupInfo(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = @"SELECT [Search] FROM [View_Search_ItemsSetupInfo] where upper([Search]) like upper('%" +
                       strName + "%') ";

        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }
    //***************  Search Setup Item ********//

    [WebMethod(EnableSession = true)]
    public string[] GetSetupItem(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GetGetSetupItem(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }

    public DataTable GetGetSetupItem(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = @"Select t1.Code+'-'+t1.Name from  ItemSetup t1  where t1.DeleteBy Is null and  upper( t1.Code+'-'+t1.Name) like upper('%" + strName + "%') ";
        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }
    //*************************************
    [WebMethod(EnableSession = true)]
    public string[] GetItemsSearchNullBrance(string prefixText, int count)
    {

        if (count == 0)
        {
            count = 10;
        }

        DataTable dt = GetAllitems(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            string str = dt.Rows[i][0].ToString();
            items.Add(str);
        }

        return items.ToArray();

    }

    public DataTable GetAllitems(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = @"SELECT [CodeWiseSearchItems] FROM [View_Search_Items_On_Stock] where upper([CodeWiseSearchItems]) like upper('%" +
                       strName + "%') and BranceId is Null ";

        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }
    //***********Branch Wish Search Items****************
    [WebMethod(EnableSession = true)]
    public string[] GetItemsSearchBrance(string prefixText, int count)
    {

        if (count == 0)
        {
            count = 10;
        }
        string BranchId ;

        if (Session["branchId"].ToString().Equals(""))
        {
            BranchId = "0";
        }
        else
        {
            BranchId=Session["branchId"].ToString();
        }
        DataTable dt = GetAllitem(prefixText, BranchId);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            string str = dt.Rows[i][0].ToString();
            items.Add(str);
        }

        return items.ToArray();

    }

    public DataTable GetAllitem(string strName, string BranceId)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        int BranchId = Convert.ToInt32(BranceId);
        if (BranchId == 0)
        {
            string query = @"SELECT [CodeWiseSearchItems] FROM [View_Search_Items_On_Stock] where upper([CodeWiseSearchItems]) like upper('%" + strName + "%') and BranceId is null and ItemID is Null";
            DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
            return dt;
        }
        else
        {
            string query1 = @"SELECT [CodeWiseSearchItems] FROM [View_Search_Items_On_Stock] where upper([CodeWiseSearchItems]) like upper('%" + strName + "%') and BranceId='" + BranchId + "'";
            DataTable dt = DataManager.ExecuteQuery(strConn, query1, "autoname");
            return dt;
        }
    }
    //***********************************************

    //********************* Items Search ********//
    [WebMethod(EnableSession = true)]
    public string[] GetItemsSearch(string prefixText, int count)
    {

        if (count == 0)
        {
            count = 10;
        }

        DataTable dt = GetAllitems(prefixText, Session["branchId"].ToString());

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            string str = dt.Rows[i][0].ToString();
            items.Add(str);
        }

        return items.ToArray();

    }

    public DataTable GetAllitems(string strName,string BranceId)
     {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        int BranchId = Convert.ToInt32(BranceId);
        if (BranchId==0)
        {
            string query = @"SELECT [CodeWiseSearchItems] FROM [View_Search_Items_On_Stock] where upper([CodeWiseSearchItems]) like upper('%" + strName + "%') and BranceId is null";
            DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
            return dt;
        }
        else
        {
            string query1 = @"SELECT [CodeWiseSearchItems] FROM [View_Search_Items_On_Stock] where upper([CodeWiseSearchItems]) like upper('%" + strName + "%') and BranceId='" + BranchId + "'";
            DataTable dt = DataManager.ExecuteQuery(strConn, query1, "autoname");
            return dt;
        }
        

       
        
    }


    //********************* Items Search ********//
    [WebMethod(EnableSession = true)]
    public string[] GetItemsSearchBranch(string prefixText, int count)
    {

        if (count == 0)
        {
            count = 10;
        }

        DataTable dt = GetAllitemsBranch(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            string str = dt.Rows[i][0].ToString();
            items.Add(str);
        }

        return items.ToArray();

    }

    public DataTable GetAllitemsBranch(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = @"SELECT [CodeWiseSearchItems] FROM [View_Search_Items_On_OutLetStock] where upper([CodeWiseSearchItems]) like upper('%" +
                       strName + "%') ";

        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }






   


    //********************* Shipment SenderList ********//
    [WebMethod(EnableSession = true)]
    public string[] GetShipmentSenderSearch(string prefixText, int count)
    {
        if (Session["ShipmentID"] != null)
        {
            if (count == 0)
            {
                count = 10;
            }
            DataTable dt = GetShipmentSender(prefixText, Session["ShipmentID"].ToString());

            List<string> items = new List<string>(count);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string str = dt.Rows[i][0].ToString();
                items.Add(str);
            }

            return items.ToArray();
        }
        return null;
    }
    public DataTable GetShipmentSender(string strName, string ShipmentID)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = @"SELECT [SearchSender] FROM [View_Search_ShipmentSender] where upper(SearchSender) like upper('%" +
                       strName + "%') and ShipmentID='" + ShipmentID + "' ";

        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }



    //********************* Items Search ********//
    [WebMethod(EnableSession = true)]
    public string[] GetSearchItemOnPurchase(string prefixText, int count)
    {

        if (count == 0)
        {
            count = 10;
        }

        DataTable dt = GetSearchPurchase(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            string str = dt.Rows[i][0].ToString();
            items.Add(str);
        }

        return items.ToArray();

    }

    public DataTable GetSearchPurchase(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = @"SELECT [Search] FROM [View_Search_ItemsSetupInfo] where upper([Search]) like upper('%" +
                       strName + "%') ";

        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }
    //***********************

    //******************* Search BNANK/CASH COA *****************//
    [WebMethod]
    public string[] GetSearchBankCashCoa(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GetBankCashCOA(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }

    public DataTable GetBankCashCOA(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = "EXEC SP_Cash_Bank_Code @SearchGlCoa='" + strName + "' ";

        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }

    //******************* Search Sender  name *****************//
    [WebMethod]
    public string[] GetSearchSenderDtl(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GetSearchSender(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }

    public DataTable GetSearchSender(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = "SELECT Search FROM View_Search_Sender where upper(Search) like upper('%" + strName + "%') ";

        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }
  

    // *********************** GET Employee ****************//
    [WebMethod]
    public string[] GetEmployeInfo(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = getGetEmployeInfo(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }
     public DataTable getGetEmployeInfo(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = @"Select t1.EMP_NO+'-'+t1.F_NAME+'-'+t2.DESIG_NAME from PMIS_PERSONNEL t1 left join PMIS_DESIG_CODE t2 on t2.DESIG_CODE=t1.JOIN_DESIG_CODE 
where upper(t1.EMP_NO+'-'+t1.F_NAME+'-'+t2.DESIG_NAME ) like UPPER ('%" + strName + "%')";

        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }

    //******************************************************//
    
    [WebMethod]
    //[ScriptMethodAttribute(ResponseFormat = ResponseFormat.Json)]
    public string getAllItems()
    {
        String connectionString = DataManager.OraConnString();
        //string query = @"SELECT * FROM [Item] t where ClosingStock>0 and Code LIKE '%"+pat+"%'";
        string query = @"SELECT * FROM [Item] t where ClosingStock>0";
        DataTable dt = DataManager.ExecuteQuery(connectionString, query, "PatInfos");
        System.Web.Script.Serialization.JavaScriptSerializer srl = new System.Web.Script.Serialization.JavaScriptSerializer();
        List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
        Dictionary<string, object> row = null;
        foreach (DataRow dr in dt.Rows)
        {
            row = new Dictionary<string, object>();
            foreach (DataColumn col in dt.Columns)
            {
                row.Add(col.ColumnName.Trim(), dr[col]);
            }
            rows.Add(row);
        }
        return srl.Serialize(rows);
    }
    //********************* PMIS_EXAM_GROUP ********//
    [WebMethod]
    public string[] GetPMIS_EXAM_GROUP(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = getPMIS_EXAM_GROUP(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }
    public DataTable getPMIS_EXAM_GROUP(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = @"SELECT [Name] FROM [PMIS_EXAM_GROUP] where upper(Name) like upper('%" + strName + "%')";

        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }
   // ***************** Brand Info ****************
    [WebMethod]
    public string[] GetBrandInfo(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = getGetBrandInfo(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }

    public DataTable getGetBrandInfo(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query =
            @"Select CONVERT(nvarchar,ID)+'-'+BrandName from Brand where  UPPER (CONVERT(nvarchar,ID)+'-'+BrandName) like upper('%" +
            strName + "%')";

        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }

    //************** Items Search for Bangladesh Stock ********//
    [WebMethod]
    public string[] GetShowBDItemsSearch(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GetSearchItem(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }
    public DataTable GetSearchItem(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = @"SELECT [SearchItems] FROM [View_SearchBDItems] where upper(SearchItems) like upper('%" + strName + "%')";

        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }



    //******************* Faculty  name *****************//
    [WebMethod]
    public string[] GetFacultySearch(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GetFaculty(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }

    public DataTable GetFaculty(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = "SELECT EMP_NO+' - '+dbo.InitCap(isnull(F_NAME,'')+' '+isnull(M_NAME,'')+' '+isnull(L_NAME,'')) AS Name FROM PMIS_PERSONNEL where upper(EMP_NO+' - '+dbo.InitCap(isnull(F_NAME,'')+' '+isnull(M_NAME,'')+' '+isnull(L_NAME,''))) like upper('%" + strName + "%') ";

        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }


    //********************* Carton List ShipmentWise ********//
    [WebMethod(EnableSession = true)]
    public string[] GetCartonListSearch(string prefixText, int count)
    {
        if (Session["ShipmentID"] != null)
        {
            if (count == 0)
            {
                count = 10;
            }
            DataTable dt = GetCartonList(prefixText, Session["ShipmentID"].ToString());

            List<string> items = new List<string>(count);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string str = dt.Rows[i][0].ToString();
                items.Add(str);
            }

            return items.ToArray();
        }
        return null;
    }
    public DataTable GetCartonList(string strName, string ShipmentID)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = @"SELECT [CartoonNo] FROM [View_ShipmentWiseCartonList] where upper(CartoonNo) like upper('%" +
                       strName + "%') and ShiftmentID='" + ShipmentID + "' ";

        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }
    //********************* Branch Search ********//

    [WebMethod]
    public string[] GetShowBranch(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GetBranch(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }
    public DataTable GetBranch(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = "SELECT [BranchSearch] FROM [View_BranchInfo] where UPPER(BranchSearch) like upper('%" + strName + "%') ";

        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }

    //********************* Supplier List on Search Purchase ********//
    [WebMethod]
    public string[] GetShowSupplierOnPurchase(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GetSupplierOnPurchase(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }
    public DataTable GetSupplierOnPurchase(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = "SELECT SearchSupplier  FROM [View_SearchSupplierOnPurchase] where UPPER(SearchSupplier) like upper('%" + strName + "%') ";

        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }

    //********************* Supplier List on Search Purchase TOP(1) ********//
    [WebMethod]
    public string[] GetShowSupplierOnPurchaseTop1(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GetSupplierOnPurchaseTop1(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }
    public DataTable GetSupplierOnPurchaseTop1(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query =
            "SELECT top(1) SearchSupplier  FROM [View_SearchSupplierOnPurchase] where UPPER(SearchSupplier) like upper('%" +
            strName + "%') order by ID desc ";

        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }
    //********************* Supplier List on Search Purchase IN BD ********//
    [WebMethod]
    public string[] GetShowSupplierOnPurchaseBD(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GetSupplierOnPurchaseBD(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }
    public DataTable GetSupplierOnPurchaseBD(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = "SELECT SearchSupplier  FROM [View_SearchSupplierOnPurchase] where UPPER(SearchSupplier) like upper('%" + strName + "%') ";

        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }

    //*********************** Get Search Customer ********************//
            //*********** Get Search Customet All *************//

    [WebMethod(EnableSession = true)]
    public string[] GetCustomername(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GetGetCustomername(prefixText);
        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }
    public DataTable GetGetCustomername(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = @"select SearchName from View_SearchCustomer Where UPPER(SearchName) like UPPER('%" + strName + "%')";
        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }

        //*********** Get Search Customet BD/PH *************//











        //*********************** Get Search Customer Branch Wise ********************//
        //*********** Get Search Customet All *************//

        [WebMethod(EnableSession = true)]
        public string[] GetBranchCustomername(string prefixText, int count)
        {
            if (count == 0)
            {
                count = 10;
            }
            DataTable dt = GetGetBranchCustomername(prefixText,Session["BranchId"].ToString());
            List<string> items = new List<string>(count);
            for (int i = 0; i < dt.Rows.Count; i++)
            {

                string str = dt.Rows[i][0].ToString();

                items.Add(str);
            }

            return items.ToArray();
        }
        public DataTable GetGetBranchCustomername(string strName, string BranchId)
        {
            string strConn = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(strConn);
            string query = @"select SearchName from View_SearchCustomer Where BranchId='" + BranchId + "'  or CommonCus='1'  and UPPER(SearchName) like UPPER('%" + strName + "%')";
            DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
            return dt;
        }

        //*********** Get Search Customet BD/PH  Branch *************//





       [WebMethod(EnableSession = true)]
        public string[] GetCustomername_BD_PH(string prefixText, int count)
        {
            if (count == 0)
            {
                count = 10;
            }
            DataTable dt = GetGetCustomername(prefixText);

            List<string> items = new List<string>(count);
            for (int i = 0; i < dt.Rows.Count; i++)
            {

                string str = dt.Rows[i][0].ToString();

                items.Add(str);
            }

            return items.ToArray();
        }

    public DataTable Customername_BD_PH(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = @"select SearchName from View_SearchCustomer Where UPPER(SearchName) like UPPER('%" + strName +
                       "%') and Country='" + Session["UserType"].ToString() + "' ";
        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }

    //**************************** GET Invoice No*************
    [WebMethod]
    public string[] GetInvoiceNo(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GetGetInvoiceNo(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }
    public DataTable GetGetInvoiceNo(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = @"select InvoiceNo from [Order] where UPPER(InvoiceNo) like UPPER('%" + strName + "%')";
        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }
    //**************** Local Purchase **************//
    [WebMethod]
    public string[] GetLocalGRN(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GetGetLocalGRN(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }
    public DataTable GetGetLocalGRN(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = @"SELECT t1.[GRN]
         
  FROM [ItemPurchaseLocalMst] t1 where UPPER(t1.GRN) like UPPER('%" + strName + "%') ";
        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }

    //**************************** GEt Finish Good Item*************
    [WebMethod]
    public string[] GetFGAndCmnItem(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GetGetFGAndCmnItem(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }
    public DataTable GetGetFGAndCmnItem(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = @"select Code+'-'+name from item where UPPER(Code+'-'+name) like UPPER('%" + strName + "%') and ItemType in(2,3)";
        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }

    //**************************** GEt Finish Good Item*************
    [WebMethod]
    public string[] GetRMAndCmnItem(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GetGetRMAndCmnItem(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }
    public DataTable GetGetRMAndCmnItem(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = @"select Code+'-'+name from item where UPPER(Code+'-'+name) like UPPER('%" + strName + "%') and ItemType in(1,3)";
        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }
        //*************Get PVReturn************************
         [WebMethod]
    public string[] GetPVReturn(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = PVReturn(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }
    public DataTable PVReturn(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = @" ";
        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }
    //********************* Remarks ********//
    [WebMethod]
    public string[] GetSearchGlCoa(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GetGlCoa(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }
    public DataTable GetGlCoa(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = @"SELECT [SEG_COA_CODE]+' - '+[SEG_COA_DESC] FROM [GL_SEG_COA] where ROOTLEAF='L' and upper([SEG_COA_CODE]+' - '+[SEG_COA_DESC]) like upper('%" + strName + "%') ";
        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }
    ///************** Local Purchase Return ********//
     [WebMethod]
    public string[] GetGReturnNo(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GetGetGReturnNo(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }
     public DataTable GetGetGReturnNo(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = @"  SELECT  [Return_No]  FROM  [PurReturnMstLocal]   where UPPER(Return_No) Like UPPER ('%" + strName + "%') ORDER BY Return_No DESC";
        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }

    [WebMethod]
    public string[] GetPONo(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = Po(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }
    public DataTable Po(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = @"  SELECT [PO] FROM [ItemPurOrderMst] where UPPER(PO) Like UPPER ('%"+strName+"%')";
        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }
    //******************** Supplier *************************
     [WebMethod(EnableSession = true)]
    public string[] GetSupplier(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        
             DataTable dt = Supplier(prefixText);
             List<string> items = new List<string>(count);
             for (int i = 0; i < dt.Rows.Count; i++)
             {

                 string str = dt.Rows[i][0].ToString();

                 items.Add(str);
             }
             return items.ToArray();
         
    }
     public DataTable Supplier(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
         
            string query =
                @"select SupplierSearch_Mobile from View_Search_Supplier where upper(SupplierSearch_Mobile) like UPPER('%" +
                strName + "%') ";
            DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
            return dt;
        
    }

     //******************** Supplier BD *************************
     [WebMethod(EnableSession = true)]
     public string[] GetSupplierBD(string prefixText, int count)
     {
         if (count == 0)
         {
             count = 10;
         }
         if (Session["user"].ToString() != null)
         {
             DataTable dt = SupplierBD(prefixText, Session["user"].ToString());
             List<string> items = new List<string>(count);
             for (int i = 0; i < dt.Rows.Count; i++)
             {

                 string str = dt.Rows[i][0].ToString();

                 items.Add(str);
             }
             return items.ToArray();
         }
         else
             return null;
     }
     public DataTable SupplierBD(string strName, string User)
     {
         string strConn = DataManager.OraConnString();
         SqlConnection sqlCon = new SqlConnection(strConn);
         string UserType = IdManager.GetShowSingleValueString(" t.UserType", "t.USER_NAME", "UTL_USERINFO t", User);
         if (!string.IsNullOrEmpty(UserType))
         {
             string query =
                 @"select isnull(Code,'')+' - '+ContactName from Supplier where upper(isnull(Code,'')+' - '+ContactName) like UPPER('%" +
                 strName + "%') ";
             DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
             return dt;
         }
         else
         {
             return null;
         }
     }
     //********************* Shipment AND Local Sale  Items With Current Stock Not Zero********//
     [WebMethod(EnableSession = true)]

     public string[] GetShowShiftmentID(string prefixText, int count)
     {
         if (count == 0)
         {
             count = 10;
         }
         DataTable dt = GetShowShiftment(prefixText);

         List<string> items = new List<string>(count);
         for (int i = 0; i < dt.Rows.Count; i++)
         {

             string str = dt.Rows[i][0].ToString();

             items.Add(str);
         }

         return items.ToArray();
     }
     private DataTable GetShowShiftment(string prefixText)
     {
         string strConn = DataManager.OraConnString();
         SqlConnection sqlCon = new SqlConnection(strConn);
         // Remove This t.Quantity>0 and  -- t.Quantity>0  AND --t.Quantity>0  AND

         DataTable dt = null;
         string query = "";
         
             query = @"SELECT [ItemsName] FROM [View_SalesItems]  
                where upper(ItemsName) like upper('%" + prefixText + "%') ";
             dt = DataManager.ExecuteQuery(strConn, query, "autoname");
             
         return dt;
     }

     //********************* Shipment AND Local Sale  Items With Current Stock Not Zero and Zero********//

     [WebMethod(EnableSession = true)]
     public string[] GetShowShiftmentIDItems(string prefixText, int count)
     {
         if (count == 0)
         {
             count = 10;
         }
         DataTable dt = GetShowShiftmentItems(prefixText);

         List<string> items = new List<string>(count);
         for (int i = 0; i < dt.Rows.Count; i++)
         {
             string str = dt.Rows[i][0].ToString();
             items.Add(str);
         }

         return items.ToArray();
     }
     private DataTable GetShowShiftmentItems(string prefixText)
     {
         string strConn = DataManager.OraConnString();
         SqlConnection sqlCon = new SqlConnection(strConn);
         // Remove This t.Quantity>0 and  -- t.Quantity>0  AND --t.Quantity>0  AND

         DataTable dt = null;
         string query = "";

         query = @"SELECT [ItemsName] FROM [View_SalesItems]  
                where upper(ItemsName) like upper('%" + prefixText + "%') and Quantity>0 ";
         dt = DataManager.ExecuteQuery(strConn, query, "autoname");

         return dt;
     }
    //********************* Remarks ********//
    [WebMethod]
    public string[] GetShowRemarks(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GetRarks(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }
    public DataTable GetRarks(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = @"SELECT Comment FROM [VoucherRemarks] where upper(Comment) like upper('%" + strName + "%') ";
        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }
    //*********************************


    //********************* Party Name  Info ********//
    [WebMethod]
    public string[] GetShowPartyName(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GetParty(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }
    public DataTable GetParty(string strName)
    {        
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = @"SELECT [PartyCode]+' - '+[PartyName] FROM [PartyInfo] where upper([PartyCode]+' - '+[PartyName]) like upper('%" + strName + "%') ";
        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;       
    }
    //*********************************


    //********************* Party Items Stock ********//
    [WebMethod]
    public string[] GetShowItemPartyStock(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GetAllItemPartyStock(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }
    public DataTable GetAllItemPartyStock(string strName)
    {
        if (Session["ID"] != null)
        {
            string strConn = DataManager.OraConnString();
            SqlConnection sqlCon = new SqlConnection(strConn);
            string query = @"SELECT t2.Code+' - '+t2.Name FROM [ItemPartyStock] t1 inner join Item t2 on t2.ID=t1.ItemsID where upper(t2.Code+' - '+t2.Name) like upper('%" + strName + "%') AND t1.[PartyID]='" + Session["ID"].ToString() + "'";

            DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
            return dt;
        }
        else
            return null;
    }
    //*********************************
    //********************* Party Information ********//
    [WebMethod]
    public string[] GetShowPartyInfo(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GetAllParty(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }
    public DataTable GetAllParty(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = @"SELECT [ContactName]+' - '+[Mobile]  FROM [Customer] where upper([ContactName]+' - '+[Mobile]) like upper('%" + strName + "%')";

        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }
    //*********************************
    //********************* Shipment Information ********//
    [WebMethod]
    public string[] GetShiftmentInfo(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GetAllShiftment(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }
    public DataTable GetAllShiftment(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = @"SELECT SearchShipment FROM [View_Search_Shipment] WHERE UPPER(SearchShipment) like upper('%" + strName + "%')";

        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }
    //********************* Shipment without Root ********//
    [WebMethod]
    public string[] GetShiftmentInfoWithoutRoot(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GetAllShiftmentWithoutRoot(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }
    public DataTable GetAllShiftmentWithoutRoot(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = @"SELECT [ShiftmentNO]+' - '+CONVERT(NVARCHAR,[ShiftmentDate],103) FROM [ShiftmentAssigen] WHERE UPPER([ShiftmentNO]+' - '+CONVERT(NVARCHAR,[ShiftmentDate],103)) like upper('%" + strName + "%') and ID not in (select distinct ParentShiftmentNO from ShiftmentAssigen where ParentShiftmentNO IS NOT NULL) ";

        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }
    //*********************************
    //********************* Items With Catagory ********//
    [WebMethod]
    public string[] GetShowItemsWithCat(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GetAllItem(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }
    public DataTable GetAllItem(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = @"SELECT t1.[Code]+' - '+t1.[Name]+' - '+Isnull(t2.Name,'')  FROM [Item] t1 Left  join Category t2 on t2.ID=t1.CategoryID where upper(t1.[Code]+' - '+t1.[Name]+' - '+Isnull(t2.Name,'')) like upper('%" + strName + "%')";

        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }


    //********************* ItemPurOrderMst List ****************//
    [WebMethod]
    public string[] GetShowItems(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GetItemsList(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }
    public DataTable GetItemsList(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = "SELECT t.Code+' - '+t.Name+' - '+Isnull(t1.BrandName,'')+' - '+Isnull(t2.Name,'') FROM [Item] t left join Brand t1 on t.Brand=t1.ID left join Category t2 on t2.ID=t.CategoryID where  t.ClosingStock>0 and upper(t.Code+' - '+t.Name+' - '+Isnull(t1.BrandName,'')+' - '+Isnull(t2.Name,'')) like upper('%" + strName + "%') ";

        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }

    //********************* ItemPurOrderMst List *************//
    [WebMethod]
    public string[] GetShowPONo(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GetAllPOno(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }
    public DataTable GetAllPOno(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = "SELECT SearchPoCode From View_SearchPurchaseOrder where upper(SearchPoCode) like upper('%" +
                       strName + "%') and OrderStatus in ('P')  order by [ID] desc ";

        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }


    //********************* ItemPurOrderMst ( [ApprovedBy] ) List *************//
    [WebMethod]
    public string[] GetShowPONoApprovedBy(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GetPOnoApprovedBy(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }
    public DataTable GetPOnoApprovedBy(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = "SELECT SearchPoCode From View_SearchPurchaseOrder where upper(SearchPoCode) like upper('%" +
                       strName + "%') and OrderStatus in ('P') and [ApprovedBy] IS NOT NULL  order by [ID] desc ";

        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }
    //********************* SUPPLIER List **************//
    [WebMethod]
    public string[] GetShowSUPPLIER(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GetSUPPLIERtt(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }
    public DataTable GetSUPPLIERtt(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = "SELECT [Code]+'-'+[ContactName]+'-'+[Mobile]  FROM [Supplier] where UPPER([Code]+'-'+[ContactName]+'-'+[Mobile]) like upper('%" + strName + "%') and Active='True' ";

        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }

    //********************* Customer List ******************//
    [WebMethod]
    public string[] GetShowCustomer(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GetCustomer(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }
    public DataTable GetCustomer(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = @"SELECT [Code]+'-'+[ContactName]+'-'+[Mobile]  FROM [Customer] where UPPER([Code]+'-'+[ContactName]+'-'+[Mobile]) like upper('%" + strName + "%') and Active='True' ";

        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }

    //********************* Cheque No Supplier *********************//
    [WebMethod]
    public string[] GetChequeNumber(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GetChequeNo(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }
    public DataTable GetChequeNo(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = "SELECT [ChequeNo] FROM [SupplierPayment] where upper([ChequeNo]) like upper('%" + strName + "%') and [Chk_Status]='P' ";

        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }
   

    //********************* Cheque No Customer ********//
    [WebMethod]
    public string[] GetChequeCustomerNumber(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GetChequeNoCus(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }
    public DataTable GetChequeNoCus(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = "SELECT [ChequeNo] FROM [CustomerPaymentReceive] where upper([ChequeNo]) like upper('%" + strName + "%') and [Chk_Status]='P' ";

        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }
    //********************* GRN Number (Purchase Voucher) *****************//

    [WebMethod]
    public string[] GetGRN(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GetGetGRNSup(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }

    private DataTable GetGetGRNSup(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = "SELECT [GRN] FROM [ItemPurchaseMst] WHERE UPPER([GRN]) like upper('%" + strName + "%') Order By GRN DESC ";

        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }

    //********************* Return Number (Purchase Return) ****************//

    [WebMethod]
    public string[] GetPurchaseReturnNo(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = PurchaseReturnNo(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }

    public DataTable PurchaseReturnNo(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = "SELECT [Return_No] FROM [PurReturnMst] WHERE UPPER([Return_No]) like upper('%" + strName + "%') Order By Return_No DESC ";

        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }

    //********************* Return GRN Number Local *****************//
    [WebMethod]
    public string[] GetGRNLocal(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GetGetGRNSupLocal(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }
    public DataTable GetGetGRNSupLocal(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = "SELECT [GRN] FROM [ItemPurchaseLocalMst] WHERE UPPER([GRN]) like upper('%" + strName + "%') Order By GRN DESC ";

        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }
    //********************* Return Invoice No Number ******************//
    [WebMethod(EnableSession = true)]
    public string[] GetInvoice(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GetGetInvoiceCus(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }
    public DataTable GetGetInvoiceCus(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);

        string query = "SELECT [InvoiceNo] FROM [Order] WHERE UPPER([InvoiceNo]) like upper('%" + strName + "%')  order by InvoiceNo desc ";
        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }



    //********************* Return Invoice No Number ******************//
    [WebMethod(EnableSession = true)]
    public string[] GetBranchInvoice(string prefixText, int count)
    {
        var BranchId = Session["BranchId"].ToString();
        if (!string.IsNullOrEmpty(BranchId))
        {

            if (count == 0)
            {
                count = 10;
            }

            DataTable dt = GetGetBranchInvoiceCus(prefixText,BranchId);

            List<string> items = new List<string>(count);
            for (int i = 0; i < dt.Rows.Count; i++)
            {

                string str = dt.Rows[i][0].ToString();

                items.Add(str);
            }

            return items.ToArray();
        }

        return null;
    }
    public DataTable GetGetBranchInvoiceCus(string strName,string BranchId)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);

        string query = "SELECT [InvoiceNo] FROM [Order] WHERE UPPER([InvoiceNo]) like upper('%" + strName + "%') and BranchId='"+BranchId+"' order by InvoiceNo desc ";
        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }


    //********************************* Gl COA CODE *********************//
    [WebMethod(EnableSession = true)]
    public string[] GetCompletionList(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        
            DataTable dt = GetItems(prefixText);

            List<string> items = new List<string>(count);
            for (int i = 0; i < dt.Rows.Count; i++)
            {

                string str = dt.Rows[i][0].ToString();

                items.Add(str);
            }

            return items.ToArray();
        
    }
    public DataTable GetItems(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = "";
        
            query = "select coa_desc from gl_coa where upper(coa_desc) like upper('%" + strName + "%') ";
       
        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;

    } 

    //*********************************
    [WebMethod]
    public string[] GetCompletionListSeg(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GetSegDesc(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }
    [WebMethod]
    public string[] GetCompletionListCost(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GetCostDesc(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }
    //***************** Shipment Item ******************//

    [WebMethod(EnableSession = true)]
    public string[] GetShipmentItem(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        string ShipmentID = Session["ShipmentID"].ToString();
        DataTable dt = GetShipmentItem(prefixText, ShipmentID);
     
        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }
    public DataTable GetShipmentItem(string strName, string ShipmentID)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = @"select SearchItem from View_ShipmentItem where upper(SearchItem) like upper('%" + strName +
                       "%') and [ShiftmentID]='" + ShipmentID + "' ";
        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }
    //***************** Item List for stock transfer ******************//
    [WebMethod]
    public string[] GetItemListStock(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GetAllItemsStock(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }
    public DataTable GetAllItemsStock(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = @"select convert(nvarchar,t1.ItemsID)+' - '+t2.Code+' - '+ t2.Name from ItemSalesStock t1
inner join Item t2 on t1.ItemsID=t2.ID
where upper(convert(nvarchar,t1.ItemsID)+ ' - '+t2.Code+' - '+ t2.Name) like upper('%" + strName + "%') ";

        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }
    //***************** Get Item Search ******************//

    [WebMethod]
    public string[] GetItemSearchAll(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GetItemsForAll(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }
    public DataTable GetItemsForAll(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = @"select SearchItems from View_SearchItems where upper(SearchItems) like upper('%" + strName + "%') ";

        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }

    [WebMethod(EnableSession=true)]
    public string[] GetItemSearchAll2(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        string ItemID = "";
        try
        {
            ItemID = Session["ItemIDOnly"].ToString();
        }
        catch
        {
            ItemID = "0";
        }
        DataTable dt = GetItemsForAll2(prefixText, ItemID);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }
    public DataTable GetItemsForAll2(string strName,string ItemID)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string parameter = "";
        if (ItemID != "0" && ItemID != "")
        {
            parameter = " and ItemSetupID='" + ItemID + "' ";
        }
        string query = @"select SearchItems from View_SearchItems where upper(SearchItems) like upper('%" + strName + "%') "+parameter+" ";

        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }

            //**** Get Items With Closing Stock *******//

    [WebMethod]
    public string[] GetItemSearchAllWithClosingStock(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GetItemsForAlllWithClosing(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }
    public DataTable GetItemsForAlllWithClosing(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = @"select SearchItems from View_SearchItems where upper(SearchItems) like upper('%" + strName + "%') and ClosingStock>0";

        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }

    //***************** Get Catagory Search ******************//

    [WebMethod(EnableSession = true)]
    public string[] GetItemCatagorySearchAll(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GetCatagoryForAll(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }
    public DataTable GetCatagoryForAll(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = @"select SearchCatagory from View_CatagorySearch where upper(SearchCatagory) like upper('%" + strName + "%') ";

        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }

    //***************** Get Catagory with department include Search ******************//
    [WebMethod(EnableSession = true)]
    public string[] GetItemCatagorySearchAllWithDepartment(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }

        string Dept = "";
        try
        {
            Dept = Session["Dept_Search"].ToString();
        }
        catch
        {
            Dept = "";
        }

        DataTable dt = GetItemCatagoryAllWithDepartment(prefixText, Dept);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }
    public DataTable GetItemCatagoryAllWithDepartment(string strName, string Dept)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string Parameter = "";
        if (!string.IsNullOrEmpty(Dept))
        {
            Parameter = " and DeptID='" + Dept + "' ";
        }

        string query = @"select SearchCatagory from View_CatagorySearch where upper(SearchCatagory) like upper('%" +
                       strName + "%') " + Parameter;

        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }

    //***************** Get Sub Catagory Search ******************//
    [WebMethod]
    public string[] GetItemSubCatagorySearchAll(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GetSubCatagoryForAll(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }
    public DataTable GetSubCatagoryForAll(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = @"select SearchSubCatagory from View_SubCatagorySearch where upper(SearchSubCatagory) like upper('%" + strName + "%') ";

        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }
    //***************** Get Brand Search ******************//

    [WebMethod]
    public string[] GetItemBrandSearchAll(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GetBrandForAll(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }
    public DataTable GetBrandForAll(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = @"select SearchBrand from View_BrandSearch where upper(SearchBrand) like upper('%" + strName + "%') ";

        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }
    //********************* Items List ***************//
    [WebMethod]
    public string[] GetItemList(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GetAllItems(prefixText);

        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {

            string str = dt.Rows[i][0].ToString();

            items.Add(str);
        }

        return items.ToArray();
    }
    public DataTable GetAllItems(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        //string query = "select Code+ ' - '+Name from Item where upper(Code+ ' - '+Name) like upper('%" + strName + "%') ";
        string query = @"select  ISNULL(t1.Code,'')+ ' - '+t1.StyleNo+' - '+ISNULL(t1.Name,'')+' - '+ISNULL(t2.BrandName,'')+' - '+convert(nvarchar,t1.UnitPrice) from Item t1
inner join Brand t2 on t1.Brand=t2.ID where upper( ISNULL(t1.Code,'')+ ' - '+t1.StyleNo+' - '+ISNULL(t1.Name,'')+' - '+ISNULL(t2.BrandName,'')+' - '+convert(nvarchar,t1.UnitPrice)) like upper('%" + strName + "%') ";

        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }
    //********************* Search Purchase Voucher ********//
    [WebMethod(EnableSession = true)]
    public string[] GetItemPurcaseWithGrn(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GetPurcaseWithGrn(prefixText);
        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            string str = dt.Rows[i][0].ToString();
            items.Add(str);
        }
        return items.ToArray();
    }
    public DataTable GetPurcaseWithGrn(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = "";
        DataTable dt = null;
        query = @"select SearchGrnNo from [View_Search_Purchase_Mst] where upper(SearchGrnNo) like upper('%" + strName + "%') ";
        dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }
    //********************* Items List Barcode ********//
    [WebMethod(EnableSession = true)]
    public string[] GetItemListBarcode(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GetAllItemsBarcode(prefixText);
        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            string str = dt.Rows[i][0].ToString();
            items.Add(str);
        }
        return items.ToArray();
    }
    public DataTable GetAllItemsBarcode(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = "";
        DataTable dt = null;
        query = @"select Items_Code_Name_Price_ExpDate from View_Search_Stock_Items where upper(Items_Code_Name_Price_ExpDate) like upper('%" + strName + "%') ";
        dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }







    //********************* Branch Items List Barcode ********//
    [WebMethod(EnableSession = true)]
    public string[] GetBranchItemListBarcode(string prefixText, int count)
    {
        var BranchId = Session["BranchId"].ToString();
        if (!string.IsNullOrEmpty(BranchId))
        {
            if (count == 0)
            {
                count = 10;
            }
            DataTable dt = GetAllBranchItemsBarcode(prefixText,BranchId);
            List<string> items = new List<string>(count);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string str = dt.Rows[i][0].ToString();
                items.Add(str);
            }
            return items.ToArray();
        }
        else
        {
            return null;  
        }
        
    }
    public DataTable GetAllBranchItemsBarcode(string strName,string BranchId)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = "";
        DataTable dt = null;
        query = @"select Items_Code_Name_Price_ExpDate from View_Search_OutLet_Stock_Items where upper(Items_Code_Name_Price_ExpDate) like upper('%" + strName + "%') and BranchID='"+BranchId+"' ";
        dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }






    //*********************************
    [WebMethod]
    public string[] GetClientList(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GetClients(prefixText);
        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            string str = dt.Rows[i][0].ToString();
            items.Add(str);
        }
        return items.ToArray();
    }

     [WebMethod]
    public string[] GetClientListname(string prefixText, int count)
    {
        if (count == 0)
        {
            count = 10;
        }
        DataTable dt = GetClientname(prefixText);
        List<string> items = new List<string>(count);
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            string str = dt.Rows[i][0].ToString();
            items.Add(str);
        }
        return items.ToArray();
    }  

    public DataTable GetClients(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = "select   client_id+'-'+ client_name+ '-'+fh_name from client_info where upper( client_id+'-'+client_name+'-'+fh_name) like upper('%" + strName + "%')";

        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }

    public DataTable GetClientname(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = "select RTRIM(LTRIM(client_name))client_name from client_info where upper(client_name) like upper('%" + strName + "%')";

        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }
    
    
    public DataTable GetSegDesc(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = "select seg_coa_desc from gl_seg_coa where lvl_code='02' and upper(seg_coa_desc) like upper('%" + strName + "%') ";

        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }
    public DataTable GetCostDesc(string strName)
    {
        string strConn = DataManager.OraConnString();
        SqlConnection sqlCon = new SqlConnection(strConn);
        string query = "select seg_coa_desc from gl_seg_coa where lvl_code='03' and upper(seg_coa_desc) like upper('%" + strName + "%') ";

        DataTable dt = DataManager.ExecuteQuery(strConn, query, "autoname");
        return dt;
    }
   

    
}
