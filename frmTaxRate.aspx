<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="frmTaxRate.aspx.cs" Inherits="frmTaxRate" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
<div id="frmMainDiv" style="background-color:White; width:100%;">   

<table style="width:100%;">
<tr>
<td style="width:1%;"></td>
<td style="width:98%; margin-top:1em;" align="center"> 
<asp:GridView CssClass="mGrid" PagerStyle-CssClass="pgr"  
        AlternatingRowStyle-CssClass="alt" ID="dgTax" runat="server" 
        AutoGenerateColumns="False" Font-Size="9pt"
        onrowcancelingedit="dgTax_RowCancelingEdit" BorderColor="LightGray" 
        onrowdeleting="dgTax_RowDeleting" onrowediting="dgTax_RowEditing" 
        onrowupdating="dgTax_RowUpdating" OnRowCommand = "dgTax_RowCommand" 
        Caption="Tax Rate Setup" Width="80%" onrowdatabound="dgTax_RowDataBound">
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
  <asp:LinkButton ID="lbBrkCancel" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel"></asp:LinkButton>
  <asp:LinkButton ID="lbBrkInsert" runat="server" CausesValidation="True" CommandName="Insert" Text="Insert" ></asp:LinkButton>
</FooterTemplate>

<ItemStyle HorizontalAlign="Center" Height="20px" Width="120px"></ItemStyle>
</asp:TemplateField>

 <asp:TemplateField  HeaderText="TaxCode" ItemStyle-Width="80px" ItemStyle-HorizontalAlign="Center"
  FooterStyle-HorizontalAlign="Center">
 <ItemTemplate>
 <asp:Label ID="lblTaxCode" runat="server" Text='<% # Eval("TaxCode") %>' Width="80px"></asp:Label>
 </ItemTemplate>
 <EditItemTemplate>
 <asp:TextBox SkinID="tbPlain" ID="txtTaxCode" runat="server" Text='<% # Eval("TaxCode") %>' Enabled="false" Width="80px" Font-Size="8pt" CssClass="tbc"></asp:TextBox>
 </EditItemTemplate>
 <FooterTemplate>
 <asp:TextBox SkinID="tbPlain" ID="txtTaxCode" runat="server" Text="" Enabled="false" Width="80px" CssClass="tbc"></asp:TextBox>
 </FooterTemplate>

<FooterStyle HorizontalAlign="Center"></FooterStyle>

<ItemStyle HorizontalAlign="Center" Width="80px"></ItemStyle>
 </asp:TemplateField>
  
 <asp:TemplateField  HeaderText="Tax Type" ItemStyle-Width="230px" ItemStyle-HorizontalAlign="Left">
     <ItemTemplate>
     <asp:Label ID="lblTaxType" runat="server" Text='<% # Eval("TaxType") %>' Width="100px"></asp:Label>
     </ItemTemplate>
     <EditItemTemplate>
     <asp:TextBox SkinID="tbPlain" ID="txtTaxType" runat="server" Text='<% # Eval("TaxType") %>' Width="100px" Font-Size="8pt" AutoPostBack="false"></asp:TextBox>
     </EditItemTemplate> 
     <FooterTemplate>
     <asp:TextBox SkinID="tbPlain" ID="txtTaxType" runat="server" Text="" Width="100px" AutoPostBack="false"></asp:TextBox>
     </FooterTemplate> 
    <ItemStyle HorizontalAlign="Left" Width="230px"></ItemStyle> 
 </asp:TemplateField>
  <asp:TemplateField  HeaderText="Tax Rate" ItemStyle-Width="230px" ItemStyle-HorizontalAlign="Left">
     <ItemTemplate>
     <asp:Label ID="lblTaxRate" runat="server" Text='<% # Eval("TaxRate") %>' Width="100px"></asp:Label>
     </ItemTemplate>
     <EditItemTemplate>
     <asp:TextBox SkinID="tbPlain" ID="txtTaxRate" runat="server" Text='<% # Eval("TaxRate") %>' Width="100px" Font-Size="8pt" AutoPostBack="false"></asp:TextBox>
     </EditItemTemplate> 
     <FooterTemplate>
     <asp:TextBox SkinID="tbPlain" ID="txtTaxRate" runat="server" Text="" Width="100px" AutoPostBack="false"></asp:TextBox>
     </FooterTemplate> 
    <ItemStyle HorizontalAlign="Left" Width="230px"></ItemStyle> 
 </asp:TemplateField>
    <asp:TemplateField HeaderText="Active?" ItemStyle-HorizontalAlign="Center"
  FooterStyle-HorizontalAlign="Center">        
        <ItemTemplate>
            <asp:CheckBox ID="chkSelect" runat="server" Checked='<%# Eval("check").ToString().Equals("True") %>'/>
        </ItemTemplate>
        <FooterTemplate>
            <asp:CheckBox ID="chkSelect" Checked="true" runat="server" />
        </FooterTemplate>
        <ItemStyle HorizontalAlign="Center" Width="40px"></ItemStyle>
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
<tr>
<td style="width:1%;">&nbsp;</td>
<td style="width:98%; margin-top:1em;" align="center"> 
    &nbsp;</td>
 <td style="width:1%;">&nbsp;</td>
</tr>
</table>
</div>  
</asp:Content>



