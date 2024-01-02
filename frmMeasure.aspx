<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="frmMeasure.aspx.cs" Inherits="frmMeasure" Title="Measurement Unit Setup.-DDP" Theme="Themes" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
<div id="frmMainDiv" style="background-color:White; width:100%;">   

<table style="width:100%;">
<tr>
<td style="width:1%;"></td>
<td style="width:98%; margin-top:1em;" align="center"> 
<asp:GridView CssClass="mGrid" PagerStyle-CssClass="pgr"  AlternatingRowStyle-CssClass="alt" ID="dgMsr" runat="server" AutoGenerateColumns="false" Font-Size="9pt" ShowFooter="false"
        onrowcancelingedit="dgMsr_RowCancelingEdit" BorderColor="LightGray" 
        onrowdeleting="dgMsr_RowDeleting" onrowediting="dgMsr_RowEditing" 
        onrowupdating="dgMsr_RowUpdating" OnRowCommand = "dgMsr_RowCommand">
<Columns>
<asp:TemplateField  ItemStyle-Width="120px" ItemStyle-HorizontalAlign="Center" ItemStyle-Height="20px">
<ItemTemplate>
<asp:LinkButton ID="lbBrkDelete" runat="server" CausesValidation="False" CommandName="Delete" Text="Delete"
  onclientclick="javascript:return window.confirm('are u really want to delete these data')"></asp:LinkButton>
<asp:LinkButton ID="lbBrkEdit" runat="server" CausesValidation="False" CommandName="Edit" Text="Edit" ></asp:LinkButton>  
<asp:LinkButton ID="lbAddNew" runat="server" CausesValidation="False" CommandName="Add" Text="New"></asp:LinkButton>  
</ItemTemplate>
<EditItemTemplate>
<asp:LinkButton ID="lbBrkUpdate" runat="server" CausesValidation="False" CommandName="Update" Text="Update"></asp:LinkButton> 
<asp:LinkButton ID="lbBrkCancel" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel"></asp:LinkButton>
</EditItemTemplate>
<FooterTemplate> 
  <asp:LinkButton ID="lbBrkCancel1" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel"></asp:LinkButton>
   <asp:LinkButton ID="lbBrkInsert" runat="server" CausesValidation="True" CommandName="Insert" Text="Insert" ></asp:LinkButton> 
</FooterTemplate>
</asp:TemplateField>

 <asp:TemplateField  HeaderText="Measure Code" ItemStyle-Width="80px" ItemStyle-HorizontalAlign="Center"
  FooterStyle-HorizontalAlign="Center">
 <ItemTemplate>
 <asp:Label ID="lblMsrUnitCode" runat="server" Text='<% # Eval("msr_unit_code") %>' Width="80px"></asp:Label>
 </ItemTemplate>
 <EditItemTemplate>
 <asp:TextBox SkinID="tbPlain" ID="txtMsrUnitCode" runat="server" Text='<% # Eval("msr_unit_code") %>' Width="80px" Font-Size="8pt" CssClass="tbc"></asp:TextBox>
 </EditItemTemplate>
 <FooterTemplate>
 <asp:TextBox SkinID="tbPlain" ID="txtMsrUnitCode" runat="server" Text="" Width="80px" CssClass="tbc"></asp:TextBox>
 </FooterTemplate>
 </asp:TemplateField>
  
 <asp:TemplateField  HeaderText="Measure Name" ItemStyle-Width="230px" ItemStyle-HorizontalAlign="Left">
 <ItemTemplate>
 <asp:Label ID="lblMsrUnitDesc" runat="server" Text='<% # Eval("msr_unit_desc") %>' Width="230px"></asp:Label>
 </ItemTemplate>
 <EditItemTemplate>
 <asp:TextBox SkinID="tbPlain" ID="txtMsrUnitDesc" runat="server" Text='<% # Eval("msr_unit_desc") %>' Width="230px" Font-Size="8pt" AutoPostBack="false"></asp:TextBox>
 </EditItemTemplate> 
 <FooterTemplate>
 <asp:TextBox SkinID="tbPlain" ID="txtMsrUnitDesc" runat="server" Text="" Width="230px" AutoPostBack="false"></asp:TextBox>
 </FooterTemplate>
 
 </asp:TemplateField>
 </Columns>
                        <RowStyle BackColor="White" />                      
                        <PagerStyle BackColor="LightGray" ForeColor="Black" HorizontalAlign="Center" />
                        <HeaderStyle BackColor="LightGray" Font-Bold="True" ForeColor="Black" />
                        <AlternatingRowStyle BackColor="" />
 </asp:GridView>
 </td>
 <td style="width:1%;"></td>
</tr>
</table>
</div>  
</asp:Content>

