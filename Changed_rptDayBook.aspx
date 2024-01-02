<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Changed_rptDayBook.aspx.cs" Inherits="rptDayBook" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Daybook report</title>
    <link href="css/GrideView.css" rel="stylesheet" type="text/css" />
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
<asp:Label ID="lblTitle" runat="server" Font-Size="15pt" 
        Font-Bold="True"></asp:Label>
</td>
</tr>
<tr style="width:100%;">
<td style="width:100%;">

<asp:Label ID="lblDate" runat="server" Font-Size="14pt" Height="25px" Width="100%" 
        style="font-weight: 700"></asp:Label>
</td>
</tr>
</table>
</td>
</tr>
</thead>
</table>
</td>
<td style="width:10%;">
    <asp:HiddenField ID="hfStartDate" runat="server" />
    <asp:HiddenField ID="hfEndDate" runat="server" />
    <asp:HiddenField ID="hfType" runat="server" />
    <asp:HiddenField ID="hfVchType" runat="server" />
    </td>
</tr>
<tr>
<td align="right" colspan="3">

 <table style="width:40%;border: solid 1px #8BB381;">
        <tr>
            <td align="left" colspan="2" >

                <asp:RadioButtonList ID="rbType" runat="server" AutoPostBack="True" 
                    BorderStyle="Solid" onselectedindexchanged="rbType_SelectedIndexChanged" 
                    RepeatDirection="Horizontal" Width="60%" Font-Bold="True" Font-Size="15pt">
                    <asp:ListItem Selected="True" Value="0">All</asp:ListItem>
                    <asp:ListItem Value="2">PH</asp:ListItem>
                    <asp:ListItem Value="1">BD</asp:ListItem>
                </asp:RadioButtonList>

            </td>
        </tr>
        <tr>
            <td align="center" >

<asp:LinkButton ID="lbExp" runat="server" Text="Export Report to PDF" 
        Font-Size="11pt" ForeColor="Blue" onclick="lbExp_Click" 
        style="height: 15px" Font-Bold="True" Enabled="False"></asp:LinkButton>

            </td>
            <td align="center">

<asp:LinkButton ID="lbExpExcel" runat="server" Text="Export Report to Excel" 
        Font-Size="11pt" ForeColor="Blue" 
        onclick="lbExpExcel_Click" Font-Bold="True"></asp:LinkButton>
            </td>
        </tr>
    </table>
</td>
</tr>
<tr>
<td colspan="3">

<asp:GridView ID="dgDb" runat="server" AutoGenerateColumns="False" Width="100%"  CssClass="Grid"
        BorderWidth="1px" BorderStyle="Solid"
        CellPadding="2" BorderColor="Black" Font-Size="8pt"
        Font-Names="Verdana" onrowdatabound="dgDb_RowDataBound" 
        onpageindexchanging="dgDb_PageIndexChanging" onprerender="dgDb_PreRender">
  <Columns> 
  <asp:BoundField HeaderText="Date" ItemStyle-Width="80px" DataField="value_date" 
          ItemStyle-HorizontalAlign="Center" ItemStyle-Height="15px">  
<ItemStyle HorizontalAlign="Center"  Height="15px" Width="10%"></ItemStyle>
      </asp:BoundField>
      <asp:BoundField DataField="Ref#" HeaderText="M.V.NO" />
  <asp:BoundField HeaderText="Head Of Account" ItemStyle-Width="400px" 
          DataField="particulars"  ItemStyle-HorizontalAlign="Left" > 
<ItemStyle HorizontalAlign="Left" Width="22%"></ItemStyle>
      </asp:BoundField>
      <asp:BoundField DataField="Descriptions" HeaderText="Particulars">
             <ItemStyle HorizontalAlign="Left" Height="15px" Width="25%"></ItemStyle>
      </asp:BoundField>
  <asp:BoundField HeaderText="Voucher No" ItemStyle-Width="100px" 
          DataField="vch_ref_no" ItemStyle-HorizontalAlign="Center" 
          ItemStyle-Height="10px"> 
<ItemStyle HorizontalAlign="Center" Height="15px" Width="15%"></ItemStyle>
      </asp:BoundField>
  <asp:BoundField HeaderText="Debit" ItemStyle-Width="100px" DataField="amount_dr" 
          DataFormatString="{0:N3}" ItemStyle-HorizontalAlign="Right" HtmlEncode="false">  
<ItemStyle HorizontalAlign="Right" Width="10%"></ItemStyle>
      </asp:BoundField>
  <asp:BoundField HeaderText="Credit" ItemStyle-Width="100px" DataField="amount_cr"
          DataFormatString="{0:N3}" ItemStyle-HorizontalAlign="Right" HtmlEncode="false"> 
<ItemStyle HorizontalAlign="Right" Width="10%"></ItemStyle>
      </asp:BoundField>
  </Columns>
                        <RowStyle BackColor="White" />
                        <PagerStyle HorizontalAlign="Center" />
</asp:GridView>

    </td>
</tr>
<tr>
<td style="width:10%;">&nbsp;</td>
<td style="width:80%;" align="left">

    &nbsp;</td>
<td style="width:10%;">&nbsp;</td>
</tr>
<tr>
<td style="width:10%;">&nbsp;</td>
<td style="width:80%;">

<asp:Label ID="lblTranStatus" runat="server" Visible="false" Text="" Font-Size="8pt" Width="100%"></asp:Label>
</td>
<td style="width:10%;">&nbsp;</td>
</tr>
</table>
<br />
</div>
    </form>
</body>
</html>
