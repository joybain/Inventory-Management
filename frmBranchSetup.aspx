<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="frmBranchSetup.aspx.cs" Inherits="frmBranchSetup" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">



<div id="frmMainDiv" style="background-color:White; width:100%;">
<div style="vertical-align:top;">
<table  id="pageFooterWrapper">
  <tr>  
        <td style="width:5%;"></td>
        <td align="center">
        <asp:Button ID="Delete" runat="server" ToolTip="Delete"
            
                
                
                
                onclientclick="javascript:return window.confirm('are u really want to delete these data')" Text="Delete" 
        Height="35px" Width="120px" BorderStyle="Outset" onclick="Delete_Click"  />
        </td>
        <td style="width:20px;"></td>
        <td align="center" >
        <asp:Button ID="btnSave" runat="server" ToolTip="Save Supplier Record" Text="Save" 
        Height="35px" Width="120px" BorderStyle="Outset" onclick="btnSave_Click"  />
        </td>             
        <td style="width:20px;"></td>
        <td align="center" >
        <asp:Button ID="btnClear" runat="server"  ToolTip="Clear" Text="Clear" 
        Height="35px" Width="120px" BorderStyle="Outset" onclick="btnClear_Click"  />
        </td>
        
      
    
   </tr>
   </table>
<asp:TextBox SkinId="tbPlain"  ID="txtFax" runat="server"  Width="20px" 
        Font-Size="8pt" MaxLength="20" CssClass="tbl" Visible="False"></asp:TextBox>
</div>
<table style="width:100%;">
<tr>
<td style="width:1%;" align="center"> </td>
<td style="width:98%;" align="center"> 
<fieldset style="vertical-align: top; border: solid 1px #8BB381; text-align:left;"><legend style="color: maroon;"><b>Branch Setup </b></legend>
<table style="width:100%;">


<tr>
<td style="width:16%; text-align: right; height: 30px;" align="left">
    <strong>Branch Name :</strong></td>
<td style="width:25%; height: 30px;" align="left"> 
    <asp:TextBox ID="txtBranchName" runat="server" Height="22px" Width="100%"></asp:TextBox>
    </td>
<td style=" width:8%; text-align: center; height: 30px;" >
    <asp:CheckBox ID="CheckBox1" Checked="true" runat="server" Text="Active?" />
    </td>
<td style="width:13%; text-align: right; height: 30px;" align="left">
    <strong>Short Name :</strong></td>
<td style="width:25%; height: 30px;" align="left" > 
    <asp:TextBox ID="txtShortName" runat="server" Height="22px" Width="100%"></asp:TextBox>
    </td>
<td style="width:25%; height: 30px;" align="left" > 
    </td>
</tr>


<tr>
<td style="width:16%; text-align: right; height: 30px;" align="left">
    <strong>Address (Line1):</strong></td>
<td style="height: 30px;" align="left" colspan="4"> 
    <asp:TextBox ID="txtAddress1" runat="server" Height="22px" Width="100%"></asp:TextBox>
    </td>
<td style="width:25%; height: 30px;" align="left" > 
    &nbsp;</td>
</tr>


<tr>
<td style="width:16%; text-align: right; height: 30px;" align="left">
    <strong>Address (Line2):</strong></td>
<td style="height: 30px;" align="left" colspan="4"> 
    <asp:TextBox ID="txtAddress2" runat="server" Height="22px" Width="100%"></asp:TextBox>
    </td>
<td style="width:25%; height: 30px;" align="left" > 
    &nbsp;</td>
</tr>


    <tr>
        <td style="width:16%; text-align: right; height: 30px;" align="left">
            <strong>Phone No :</strong></td>
        <td style="width:25%; height: 30px;" align="left"> 
            <asp:TextBox ID="txtPhoneNo" runat="server" Height="22px" Width="100%"></asp:TextBox>
        </td>
        <td style=" width:8%; text-align: center; height: 30px;" >
        
        </td>
        <td style="width:13%; text-align: right; height: 30px;" align="left">
            <strong>Mobile NO :</strong></td>
        <td style="width:25%; height: 30px;" align="left" > 
            <asp:TextBox ID="txtMobile" runat="server" Height="22px" Width="100%"></asp:TextBox>
        </td>
        <td style="width:25%; height: 30px;" align="left" > 
        </td>
    </tr>


    <tr>
        <td style="width:16%; text-align: right; height: 30px;" align="left">
            <strong>Email :</strong></td>
        <td style="width:25%; height: 30px;" align="left"> 
            <asp:TextBox ID="txtEmail" runat="server" Height="22px" Width="100%"></asp:TextBox>
        </td>
        <td style=" width:8%; text-align: center; height: 30px;" >
        
        </td>
        <td style="width:13%; text-align: right; height: 30px;" align="left">
            <strong>Vat Reg No :</strong></td>
        <td style="width:25%; height: 30px;" align="left" > 
            <asp:TextBox ID="txtVatRegNo" runat="server" Height="22px" Width="100%"></asp:TextBox>
        </td>
        <td style="width:25%; height: 30px;" align="left" > 
        </td>
    </tr>


    <tr>
        <td style="width:16%; text-align: right; height: 30px;" align="left">
            &nbsp;</td>
        <td style="width:25%; height: 30px;" align="left"> 
            <asp:HiddenField ID="hfId" runat="server" />
        </td>
        <td style=" width:8%; text-align: center; height: 30px;" >
        
            &nbsp;</td>
        <td style="width:13%; text-align: right; height: 30px;" align="left">
            &nbsp;</td>
        <td style="width:25%; height: 30px;" align="left" > 
            &nbsp;</td>
        <td style="width:25%; height: 30px;" align="left" > 
            &nbsp;</td>
    </tr>
</table>
</fieldset>

<br />
<asp:GridView CssClass="mGrid" PagerStyle-CssClass="pgr"  AlternatingRowStyle-CssClass="alt" 
ID="gdvBranch" runat="server" AutoGenerateColumns="False" Width="100%" 
        AllowPaging="True" BackColor="White" BorderWidth="1px" BorderStyle="Solid"
        CellPadding="2" BorderColor="LightGray" Font-Size="9pt" 
        AllowSorting="True" PageSize="50" ForeColor="#333333" 
        onrowdatabound="gdvBranch_RowDataBound" 
        onselectedindexchanged="gdvBranch_SelectedIndexChanged">
  <HeaderStyle Font-Size="9pt"  Font-Bold="True" HorizontalAlign="center" BackColor="Silver"/>
  <FooterStyle BackColor="Silver" Font-Bold="True" ForeColor="White" />
  <Columns>
  <asp:CommandField ShowSelectButton="True"  ItemStyle-Width="40px" 
          ItemStyle-HorizontalAlign="Center" ItemStyle-ForeColor="Blue">
<ItemStyle HorizontalAlign="Center" ForeColor="Blue" Width="40px"></ItemStyle>
      </asp:CommandField>
      <asp:BoundField DataField="ID" HeaderText="ID"  ItemStyle-Width="150px" 
          ItemStyle-HorizontalAlign="Left">
<ItemStyle HorizontalAlign="Left" Width="5%"></ItemStyle>
      </asp:BoundField>
  <asp:BoundField  HeaderText="BranchName" DataField="BranchName" 
          ItemStyle-Width="80px" ItemStyle-HorizontalAlign="Center">
      </asp:BoundField>
  <asp:BoundField  HeaderText="Address" DataField="Address1" ItemStyle-Width="200px" 
          ItemStyle-HorizontalAlign="Left">
<ItemStyle HorizontalAlign="Left" Width="35%"></ItemStyle>
      </asp:BoundField>
      <asp:BoundField  HeaderText="Phone No" DataField="Phone" ItemStyle-Width="100px" 
          ItemStyle-HorizontalAlign="Left">
<ItemStyle HorizontalAlign="Center" Width="20%"></ItemStyle>
      <ItemStyle HorizontalAlign="Left" Width="15%" />
      </asp:BoundField>
  <asp:BoundField  HeaderText="Vat Reg No" DataField="VatRegNo" ItemStyle-Width="200px" 
          ItemStyle-HorizontalAlign="Left">  
<ItemStyle HorizontalAlign="Left" Width="10%"></ItemStyle>
      </asp:BoundField>
  </Columns>
                        <RowStyle BackColor="white" />
                        <EditRowStyle BackColor="" />
                        <PagerStyle  HorizontalAlign="Center" />
                        <AlternatingRowStyle BackColor="" />
  </asp:GridView>
</td>
<td style="width:1%;" align="center"> </td>
</tr>
</table>
</div>

</asp:Content>



