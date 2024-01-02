<%@ Page Language="C#" AutoEventWireup="true" CodeFile="rptCoa.aspx.cs" Inherits="rptDayBook" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Daybook report</title>
</head>
<body>
    <form id="form1" runat="server">
<div style="min-height:560px; height:auto !important; text-align:center; width:100%; margin-top:0em;">
<table style="width:100%;">
<tr>
<td style="width:10%;"></td>
<td style="width:80%;">

<table id ="tblOver" style="width:900px; margin-top:0em;" >
<thead>
<tr align="center">
<td style=" width:900px; text-align:center;" align="center">
<table style="width:100%;">
<tr style="width:100%;">
<td style="width:100%;">
<asp:Label ID="lblOrg" runat="server" Visible="True" Text="" Font-Size="14pt" Height="20px" Width="100%"></asp:Label>
</td>
</tr>
<tr style="width:100%;">
<td style="width:100%;">
<asp:Label ID="lblAddress1" runat="server" Visible="True" Text="" Font-Size="8pt" Height="15px" Width="100%"></asp:Label>
</td>
</tr>
<tr style="width:100%;">
<td style="width:100%;">
<asp:Label ID="lblAddress2" runat="server" Visible="True" Text="" Font-Size="8pt" Height="15px" Width="100%"></asp:Label>
</td>
</tr>
<tr style="width:100%;">
<td style="width:100%;">
<asp:Label ID="lblTitle" runat="server" Visible="True" Text="" Font-Size="8pt" Height="15px" Width="100%"></asp:Label>
</td>
</tr>
</table>
</td>
</tr>
</thead>
<tr>
<td >

<asp:GridView ID="dgDb" runat="server" AutoGenerateColumns="False" Width="100%" 
        BorderWidth="1px" BorderStyle="Solid"
        CellPadding="2" BorderColor="Black" Font-Size="8pt"
        Font-Names="Verdana" onrowdatabound="dgDb_RowDataBound" 
        onpageindexchanging="dgDb_PageIndexChanging">
  <Columns> 
  <%--<asp:BoundField HeaderText="Date" ItemStyle-Width="80px" 
          ItemStyle-HorizontalAlign="Center" ItemStyle-Height="15px">  
<ItemStyle HorizontalAlign="Center" Height="15px" Width="80px"></ItemStyle>
      </asp:BoundField>--%>
      <asp:TemplateField>
            <HeaderTemplate>
            Serial No.</HeaderTemplate>
            <ItemTemplate>
            <asp:Label ID="lblSRNO" runat="server" 
                Text='<%#Container.DataItemIndex+1 %>'></asp:Label>
            </ItemTemplate>
            </asp:TemplateField>
      <asp:BoundField DataField="GL_COA_CODE" HeaderText="Chart of Account " />
      <asp:BoundField DataField="COA_DESC" HeaderText="Descriptions"  ItemStyle-HorizontalAlign="Left"/>
  </Columns>
                        <RowStyle BackColor="White" />
                        <PagerStyle HorizontalAlign="Center" />
                        <AlternatingRowStyle BackColor="" />
</asp:GridView>

</td>
</tr>
</table>
<table style="width:100%;">
<%--<tr style="width:100%;">
<td style="width:50%; vertical-align:top;" align="left">
<asp:LinkButton ID="btnPrint" runat="server" Text="Print" Font-Size="X-Small" 
        OnClientClick='javascript:PrintContent("tblOver")' ForeColor="Blue"></asp:LinkButton>
</td>
<td style="width:50%; vertical-align:top;" align="right">       
<asp:LinkButton ID="lbExp" runat="server" Text="Export to Excel" Font-Size="X-Small" 
        ForeColor="Blue"></asp:LinkButton>        
</td>
</tr>--%>
<tr style="width:100%;">
<td style="width:50%; vertical-align:top;" align="left">
<asp:LinkButton ID="lbExp" runat="server" Text="Export Report to PDF" 
        Font-Size="X-Small" ForeColor="Blue" Width="150px" onclick="lbExp_Click"></asp:LinkButton>
</td>
<td style="width:50%; vertical-align:top;" align="right">           
</td>
</tr>
<tr style="width:100%;">
<td style="width:100%;" colspan="2">
<asp:Label ID="lblTranStatus" runat="server" Visible="false" Text="" Font-Size="8pt" Width="100%"></asp:Label>
</td>
</tr>
</table>
</td>
<td style="width:10%;"></td>
</tr>
</table>
<br />
</div>
    </form>
</body>
</html>
