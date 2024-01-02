<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="UserPermis.aspx.cs" Inherits="UserPermis" Title="User Permission Setup" Theme="Themes" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">

<div id="frmMainDiv" style="width:100%; background-color:White;">
<div style="vertical-align:top;">
<table  id="pageFooterWrapper">
   <tr>
   <td align="center">
       <asp:Button ID="btnDelete" runat="server" ToolTip="Delete" onclick="btnDelete_Click"  
           onclientclick="javascript:return window.confirm('Are u really want to delete these data')" 
           Text="Delete"
            
             Width="100px" Height="35px" BorderStyle="Outset" BorderWidth="1px"/> </td> 
   <td align="center"> 
    <asp:Button ID="btnFind" runat="server" ToolTip="Find" 
           onclick="btnFind_Click"  Text="Find"
            
             Width="100px" Height="35px" BorderStyle="Outset" BorderWidth="1px" />        
           </td> 
   <td align="center"> 
       <asp:Button ID="btnSave" runat="server" ToolTip="Save" onclick="btnSave_Click" 
       Text="Save"
            
             Width="100px" Height="35px" BorderStyle="Outset" BorderWidth="1px"  /></td>  
   <td align="center"> 
       <asp:Button ID="btnClear" runat="server" ToolTip="Clear" 
           onclick="btnClear_Click" Text="Clear" 
            
             Width="100px" Height="35px" BorderStyle="Outset" BorderWidth="1px"/>       
           </td>            
   </tr>
</table>
 </div> 
 <table style="width:100%;">
<tr>
<td style="width:1%;"></td>
<td style="width:98%;" align="center">
<br />
  
<table style="border:solid 1px LightGray; width:100%; line-height:2.5;">
<tr>
<td style="width:100px; vertical-align:middle;" align="left">
<asp:Label ID="lblUserId" runat="server" Font-Size="8pt">User ID</asp:Label>
</td>
<td> <asp:TextBox SkinID="tbGray" ID="txtUserName" runat="server"  Width="100px" Font-Size="8pt" Enabled="false" MaxLength="4"></asp:TextBox>

</td>
<td style="height: 27px" >&nbsp&nbsp</td>
<td style="width:100px; vertical-align:middle;" align="left">
<asp:Label ID="lblDesc" runat="server" Font-Size="8pt">Description</asp:Label>
</td>
<td  colspan="4"> <asp:TextBox SkinID="tbGray" ID="txtDesc" Enabled="false" runat="server" Width="362px" Font-Size="8" MaxLength="150"></asp:TextBox></td>
</tr>
<tr><td colspan="8" style="height:18px; vertical-align:middle;">
<asp:Label ID="lblTranStatus"  runat="server" Text="" Visible="false" Font-Size="X-Small"></asp:Label>
</td></tr>
</table>
<br />
<asp:GridView  RowStyle-Height="25px" Font-Names="Tahoma" CssClass="mGrid" PagerStyle-CssClass="pgr"  AlternatingRowStyle-CssClass="alt" ID="dgUserMst" runat="server" AutoGenerateColumns="false" 
        AllowPaging="True" Width="100%" BackColor="White" BorderWidth="1px" BorderStyle="Solid"
        CellPadding="2" CellSpaAMBg="0" BorderColor="LightGray" Font-Size="8pt" AllowSorting="true" PageSize="25" 
        onselectedindexchanged="dgUserMst_SelectedIndexChanged" ForeColor="#333333"  >
  <HeaderStyle Font-Size="9pt" Font-Names="Palatino" Font-Bold="True" HorizontalAlign="center" BackColor="Silver"/>
  <FooterStyle BackColor="Silver" Font-Bold="True" ForeColor="White" />
  <Columns>
  <asp:CommandField ShowSelectButton="True"  ItemStyle-Width="90px" ItemStyle-HorizontalAlign="Center" ItemStyle-ForeColor="Blue" ItemStyle-Height="25px"/>
  <asp:BoundField  HeaderText="User ID" DataField="user_name" ItemStyle-Width="120px" ItemStyle-HorizontalAlign="Left"/>
  <asp:BoundField  HeaderText="Description" DataField="description" ItemStyle-Width="350px" ItemStyle-HorizontalAlign="Left"/>
  <asp:BoundField  HeaderText="Group" DataField="user_grp" ItemStyle-Width="150px" ItemStyle-HorizontalAlign="Left"/>  
  </Columns>
                        <RowStyle BackColor="white" />
                        <EditRowStyle BackColor="" />
                        <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                        <HeaderStyle BackColor="Silver" Font-Bold="True" ForeColor="Black" />
                        <AlternatingRowStyle BackColor="#F5F5F5" />
</asp:GridView>
<asp:GridView  RowStyle-Height="25px" Font-Names="Tahoma" CssClass="mGrid" PagerStyle-CssClass="pgr"  AlternatingRowStyle-CssClass="alt" ID="dgPermis" runat="server" AutoGenerateColumns="False" 
        AllowPaging="True" Width="100%" BackColor="White" BorderWidth="1px" BorderStyle="Solid"
        CellPadding="2" BorderColor="LightGray" Font-Size="8pt" AllowSorting="True" PageSize="100" 
          OnRowUpdating="dgPermis_RowUpdating" OnRowEditing="dgBudget_RowEditing" 
          OnRowCancelingEdit="dgPermis_CancelingEdit" ShowFooter="false" 
          onselectedindexchanging="dgPermis_SelectedIndexChanging" 
          onrowdatabound="dgPermis_RowDataBound" 
        onrowcommand="dgPermis_RowCommand" 
        onpageindexchanging="dgPermis_PageIndexChanging" >
  <HeaderStyle Font-Size="9pt" ForeColor="White" Font-Bold="True" BackColor="Silver" HorizontalAlign="center"/> 
  <Columns>
 <asp:TemplateField ItemStyle-Font-Size="8pt" ItemStyle-Width="120px" ItemStyle-HorizontalAlign="Center" ItemStyle-Height="25px">
 <ItemTemplate>
 <asp:LinkButton ID="lbEdit" runat="server" CausesValidation="False" CommandName="Edit" Text="Edit" ></asp:LinkButton>  
  </ItemTemplate>
 <EditItemTemplate>
 <asp:LinkButton ID="lbUpdate" runat="server" CausesValidation="True" CommandName="Update" Text="Update" ></asp:LinkButton> 
  <asp:LinkButton ID="lbCancel" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel"></asp:LinkButton>
 </EditItemTemplate> 
 </asp:TemplateField>
  
  <asp:TemplateField HeaderText="Module Name" ItemStyle-Width="270px" ItemStyle-HorizontalAlign="Left">
  <ItemTemplate>  
  <asp:Label ID="lblModName" runat="server" Text='<%#Eval("mod_name") %>' Width="270px" Font-Size="8pt"></asp:Label>    
  </ItemTemplate>
  </asp:TemplateField>
  
  <asp:TemplateField  HeaderText="Add" ItemStyle-Width="50px" ItemStyle-HorizontalAlign="Center">
 <ItemTemplate>
 <asp:Label ID="lblAllowAdd" runat="server" Text='<% # Eval("allow_add") %>' Width="50px"></asp:Label>
 </ItemTemplate>
 <EditItemTemplate>
 <asp:DropDownList SkinID="ddlPlain" ID="ddlAllowAdd" runat="server" Width="50px" Font-Size="8pt">
<asp:ListItem Text="Yes" Value="Y"></asp:ListItem> 
<asp:ListItem Text="No" Value="N" Selected="True"></asp:ListItem>
</asp:DropDownlist>
 </EditItemTemplate>
 </asp:TemplateField>
 
 <asp:TemplateField  HeaderText="Edit" ItemStyle-Width="50px" ItemStyle-HorizontalAlign="Center">
 <ItemTemplate>
 <asp:Label ID="lblAllowEdit" runat="server" Text='<% # Eval("allow_edit") %>' Width="50px"></asp:Label>
 </ItemTemplate>
 <EditItemTemplate>
 <asp:DropDownList SkinID="ddlPlain" ID="ddlAllowEdit" runat="server" Width="50px" Font-Size="8pt">
<asp:ListItem Text="Yes" Value="Y"></asp:ListItem> 
<asp:ListItem Text="No" Value="N" Selected="True"></asp:ListItem>
</asp:DropDownlist>
 </EditItemTemplate>
 </asp:TemplateField>
 
 <asp:TemplateField  HeaderText="View" ItemStyle-Width="50px" ItemStyle-HorizontalAlign="Center">
 <ItemTemplate>
 <asp:Label ID="lblAllowView" runat="server" Text='<% # Eval("allow_view") %>' Width="50px"></asp:Label>
 </ItemTemplate>
 <EditItemTemplate>
 <asp:DropDownList SkinID="ddlPlain" ID="ddlAllowView" runat="server" Width="50px" Font-Size="8pt">
<asp:ListItem Text="Yes" Value="Y"></asp:ListItem> 
<asp:ListItem Text="No" Value="N" Selected="True"></asp:ListItem>
</asp:DropDownlist>
 </EditItemTemplate>
 <FooterTemplate>
 <asp:DropDownList SkinID="ddlPlain" ID="ddlAllowView" runat="server" Width="50px" Font-Size="8pt">
<asp:ListItem Text="Yes" Value="Y"></asp:ListItem> 
<asp:ListItem Text="No" Value="N" Selected="True"></asp:ListItem>
</asp:DropDownlist>
 </FooterTemplate>
 </asp:TemplateField>
 
 <asp:TemplateField  HeaderText="Delete" ItemStyle-Width="50px" ItemStyle-HorizontalAlign="Center">
 <ItemTemplate>
 <asp:Label ID="lblAllowDelete" runat="server" Text='<% # Eval("allow_delete") %>' Width="50px"></asp:Label>
 </ItemTemplate>
 <EditItemTemplate>
 <asp:DropDownList SkinID="ddlPlain" ID="ddlAllowDelete" runat="server" Width="50px" Font-Size="8pt">
<asp:ListItem Text="Yes" Value="Y"></asp:ListItem> 
<asp:ListItem Text="No" Value="N" Selected="True"></asp:ListItem>
</asp:DropDownList>
 </EditItemTemplate>
 </asp:TemplateField>
 
 <asp:TemplateField  HeaderText="Print" ItemStyle-Width="50px" ItemStyle-HorizontalAlign="Center">
 <ItemTemplate>
 <asp:Label ID="lblAllowPrint" runat="server" Text='<% # Eval("allow_print") %>' Width="50px"></asp:Label>
 </ItemTemplate>
 <EditItemTemplate>
 <asp:DropDownList SkinID="ddlPlain" ID="ddlAllowPrint" runat="server" Width="50px" Font-Size="8pt">
<asp:ListItem Text="Yes" Value="Y"></asp:ListItem> 
<asp:ListItem Text="No" Value="N" Selected="True"></asp:ListItem>
</asp:DropDownlist>
 </EditItemTemplate>
 </asp:TemplateField>
 
 <asp:TemplateField  HeaderText="Authorize" ItemStyle-Width="50px" ItemStyle-HorizontalAlign="Center">
 <ItemTemplate>
 <asp:Label ID="lblAllowAutho" runat="server" Text='<% # Eval("allow_autho") %>' Width="50px"></asp:Label>
 </ItemTemplate>
 <EditItemTemplate>
 <asp:DropDownList SkinID="ddlPlain" ID="ddlAllowAutho" runat="server" Width="50px" Font-Size="8pt">
<asp:ListItem Text="Yes" Value="Y"></asp:ListItem> 
<asp:ListItem Text="No" Value="N" Selected="True"></asp:ListItem>
</asp:DropDownlist>
 </EditItemTemplate>
 </asp:TemplateField>
  
</Columns>
                        <RowStyle BackColor="white" />
                        <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                        <HeaderStyle BackColor="Silver" Font-Bold="True" ForeColor="Black" />
                        <AlternatingRowStyle BackColor="#F5F5F5" />
  </asp:GridView>  
</td>
<td style="width:1%;"></td>
</tr>
</table>
</div>


</asp:Content>

