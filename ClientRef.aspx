<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ClientRef.aspx.cs" Inherits="ClientRef" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Select Quotation</title>
<script language="javascript" type="text/javascript">
    function SubmitToParent(item) {
        window.opener.setValue(item);
        window.opener.document.forms[0].submit();        
        window.close();
        return false;
    }
</script>  
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <asp:GridView CssClass="mGrid" PagerStyle-CssClass="pgr"  AlternatingRowStyle-CssClass="alt" ID="dgQuot" runat="server" AutoGenerateColumns="false" 
        AllowPaging="True"  BackColor="White" BorderWidth="1px" BorderStyle="Solid"
        CellPadding="2" CellSpacing="0" BorderColor="LightGray" Font-Size="8pt" 
        AllowSorting="true" PageSize="30" 
        onselectedindexchanged="dgQuot_SelectedIndexChanged" 
         onpageindexchanging="dgQuot_PageIndexChanging" >
      <HeaderStyle Font-Size="9" Font-Names="Arial" Font-Bold="True" BackColor="LightGray" HorizontalAlign="center"  ForeColor="Black" />
      <Columns>  
      <asp:CommandField ShowSelectButton="True"  ItemStyle-Width="40px" ItemStyle-HorizontalAlign="Center"/>
      <asp:BoundField HeaderText="Client ID" DataField="client_id" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center"/>    
      <asp:BoundField HeaderText="Client Name" DataField="client_name"  ItemStyle-Width="200px" ItemStyle-HorizontalAlign="Left" DataFormatString="{0:dd/MM/yyyy}"/>
      <asp:BoundField HeaderText="Addres" DataField="address1" ItemStyle-Height="20" ItemStyle-Width="200px" ItemStyle-HorizontalAlign="Left"></asp:BoundField>
      <asp:BoundField HeaderText="Contact No" DataField="mobile" ItemStyle-Height="20" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Left"></asp:BoundField>  
      </Columns>
                        <RowStyle BackColor="White" />
                        <SelectedRowStyle BackColor="" Font-Bold="True" />
                        <PagerStyle BackColor="LightGray" ForeColor="Black" HorizontalAlign="Center" />
                        <AlternatingRowStyle BackColor="" />
</asp:GridView>
    </div>
    </form>
</body>
</html>
