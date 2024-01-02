<%@ Page Language="C#" AutoEventWireup="true" CodeFile="GlFinYearPopup.aspx.cs" Inherits="GlFinYearPopup" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script language="javascript" type="text/javascript">
        function SubmitToParent(finmon) {
            window.opener.document.forms[0].elements[1].value = finmon;            
            window.close();
            return false;
            
        }
</script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <asp:GridView CssClass="mGrid" PagerStyle-CssClass="pgr"  AlternatingRowStyle-CssClass="alt" ID="dgFinMonth" runat="server" AutoGenerateColumns="false" 
        AllowPaging="True" Width="100%" BackColor="White" BorderWidth="1px" BorderStyle="Solid"
        CellPadding="2" CellSpacing="0" BorderColor="Salmon" Font-Size="8pt" 
            AllowSorting="true" PageSize="5" onselectedindexchanged="dgFinMonth_SelectedIndexChanged" >
  <HeaderStyle Font-Size="9" Font-Names="Arial" Font-Bold="True" BackColor="Yellow" HorizontalAlign="center" />

  <Columns>
  <asp:CommandField ShowSelectButton="True"  ItemStyle-Width="40px" ItemStyle-HorizontalAlign="Center"/>
  <asp:BoundField  HeaderText="Financial Year" DataField="fin_year" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center"/>
  <asp:BoundField  HeaderText="Financial Month" DataField="fin_mon" ItemStyle-Width="200px" ItemStyle-HorizontalAlign="Center"/>
  <asp:BoundField  HeaderText="Year Flag" DataField="year_flag" ItemStyle-Width="90px" ItemStyle-HorizontalAlign="Center"/>
  
  </Columns>
                        <RowStyle BackColor="white" />
                        <EditRowStyle BackColor="" />
                        <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                        <HeaderStyle BackColor="Silver" Font-Bold="True" ForeColor="Black" />
                        <AlternatingRowStyle BackColor="gray" />
  </asp:GridView>
    </div>
    </form>
</body>
</html>
