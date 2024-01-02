<%@ Page Language="C#" AutoEventWireup="true" CodeFile="GetCoa.aspx.cs" Inherits="GetCoa" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script language="javascript" type="text/javascript">
        function SubmitToParent(gl,gldesc) {
            //window.opener.document.forms[0].elements[20].focus();
            window.opener.document.forms[0].elements[20].value = gl;
            window.opener.document.forms[0].elements[21].value = gldesc;
            //window.opener.document.forms[0].elements[16].reload();            
            //window.opener.document.getElementById("txtGlCoaCode").value = 
            //document.getElementById("txtGlCode").value;
            //window.opener.document.forms[0].submit();
            //self.close();
            
            window.close();
            return false;
            
        }
</script>
</head>
<body>

    <form id="form1" runat="server">
    <div style="background: url('img/background.png') repeat-x; width: 450px; height: 25px; border: solid 1px #cccccc; padding: 5px; color: #666666">
    <asp:Label ID="lblSearch" runat="server">COA Desc</asp:Label> &nbsp &nbsp &nbsp
    <asp:TextBox ID="txtGlCode" runat="server"></asp:TextBox> &nbsp &nbsp &nbsp
    <input type="button" id="btnSearch" value="Search" onclick="SubmitToParent()" />
    </div>
    <br />
    <br />
    <div style="width:550px">
    <asp:GridView CssClass="mGrid" PagerStyle-CssClass="pgr"  AlternatingRowStyle-CssClass="alt"  ID="dgGlCoa" runat="server" AutoGenerateColumns="false" Width="550px" 
        AllowPaging="True"  BackColor="White" BorderWidth="1px" BorderStyle="Solid"
        CellPadding="2" CellSpacing="0" BorderColor="Salmon" Font-Size="8pt" AllowSorting="true" PageSize="5" 
        onselectedindexchanged="dgGlCoa_SelectedIndexChanged">
  <HeaderStyle Font-Size="9" Font-Names="Arial" Font-Bold="True" BackColor="Blue" HorizontalAlign="center" ForeColor="White"/>

  <Columns>
  <asp:CommandField ShowSelectButton="True"  />
  
  <asp:BoundField  HeaderText="COA Code" DataField="gl_coa_code" ItemStyle-Width="100px"  ItemStyle-HorizontalAlign="Center"></asp:BoundField>  
  <asp:BoundField  HeaderText="COA Desc" DataField="coa_desc" ></asp:BoundField>
  <asp:BoundField  HeaderText="Account Type" DataField="acc_type" ItemStyle-Width="50px" ItemStyle-HorizontalAlign="Center"></asp:BoundField>
  <asp:BoundField  HeaderText="Status" DataField="status" >
  <ItemStyle Width="50px" />
  </asp:BoundField>
  </Columns>
                        <RowStyle BackColor="#EFF3FB" />
                        <EditRowStyle BackColor="" />
                        <SelectedRowStyle BackColor="#ADDFFF" Font-Bold="True" ForeColor="#333333" />
                        <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                        <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                        <AlternatingRowStyle BackColor="White" />
</asp:GridView>

    </div>
    </form>
</body>
</html>
