<%@ Page Title="Brand Setup" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="frmBrand.aspx.cs" Inherits="frmBrand" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <div id="frmMainDiv" style="background-color:White; width:100%;">   

<table style="width:100%;">
<tr>
<td style="width:1%;">&nbsp;</td>
<td style="width:98%; margin-top:1em;" align="center"> 
    &nbsp;</td>
 <td style="width:1%;">&nbsp;</td>
</tr>
<tr>
<td style="width:1%;"></td>
<td style="width:98%; margin-top:1em;" align="center"> 
<asp:GridView CssClass="mGrid" PagerStyle-CssClass="pgr"  
        AlternatingRowStyle-CssClass="alt" ID="dgMsr" runat="server" 
        AutoGenerateColumns="false" Font-Size="9pt" ShowFooter="false"
        onrowcancelingedit="dgMsr_RowCancelingEdit" BorderColor="LightGray" 
        onrowdeleting="dgMsr_RowDeleting" onrowediting="dgMsr_RowEditing" 
        onrowupdating="dgMsr_RowUpdating" OnRowCommand = "dgMsr_RowCommand" 
        Width="60%" onrowdatabound="dgMsr_RowDataBound">
<Columns>
<asp:TemplateField  ItemStyle-Width="120px" ItemStyle-HorizontalAlign="Center" ItemStyle-Height="20px">
<ItemTemplate>
  <asp:LinkButton ID="lbBrkDelete" runat="server" CausesValidation="False" 
        CommandName="Delete" Text="Delete"
  
        onclientclick="javascript:return window.confirm('are u really want to delete these data')" 
        Font-Bold="True" Font-Size="10pt"></asp:LinkButton>
    &nbsp;
<asp:LinkButton ID="lbBrkEdit" runat="server" CausesValidation="False" 
        CommandName="Edit" Text="Edit" Font-Bold="True" Font-Size="10pt" ></asp:LinkButton>  
    &nbsp;  
<asp:LinkButton ID="lbAddNew" runat="server" CausesValidation="False" 
        CommandName="Add" Text="New" Font-Bold="True" Font-Size="10pt"></asp:LinkButton>  
</ItemTemplate>
<EditItemTemplate>
<asp:LinkButton ID="lbBrkCancel" runat="server" CausesValidation="False" 
        CommandName="Cancel" Text="Cancel" Font-Bold="True" Font-Size="10pt"></asp:LinkButton>
    &nbsp;
<asp:LinkButton ID="lbBrkUpdate" runat="server" CausesValidation="False" 
        CommandName="Update" Text="Update" Font-Bold="True" Font-Size="10pt"></asp:LinkButton> 
</EditItemTemplate>
<FooterTemplate>
  <asp:LinkButton ID="lbBrkCancel1" runat="server" CausesValidation="False" 
        CommandName="Cancel" Text="Cancel" Font-Bold="True" Font-Size="10pt"></asp:LinkButton>
    &nbsp;
  <asp:LinkButton ID="lbBrkInsert" runat="server" CausesValidation="True" 
        CommandName="Insert" Text="Insert" Font-Bold="True" Font-Size="10pt" ></asp:LinkButton>   
</FooterTemplate>

<ItemStyle HorizontalAlign="Center" Height="20px" Width="120px"></ItemStyle>
</asp:TemplateField>

 <asp:TemplateField  HeaderText="Brand Code" ItemStyle-Width="80px" ItemStyle-HorizontalAlign="Center"
  FooterStyle-HorizontalAlign="Center">
 <ItemTemplate>
 <asp:Label ID="lblComId" runat="server" Text='<% # Eval("com_id") %>' Width="80px"></asp:Label>
 </ItemTemplate>
 <EditItemTemplate>
 <asp:TextBox SkinID="tbPlain" ID="txtComId" runat="server" Text='<% # Eval("com_id") %>' Width="80px" Font-Size="8pt" CssClass="tbc" Enabled="false"></asp:TextBox>
 </EditItemTemplate>
 <FooterTemplate>
 <asp:TextBox SkinID="tbPlain" ID="txtComId" runat="server" Text="" Width="80px" CssClass="tbc" Enabled="false"></asp:TextBox>
 </FooterTemplate>

<FooterStyle HorizontalAlign="Center"></FooterStyle>

<ItemStyle HorizontalAlign="Center" Width="80px"></ItemStyle>
 </asp:TemplateField>
  
 <asp:TemplateField  HeaderText="Brand Name" ItemStyle-Width="230px" ItemStyle-HorizontalAlign="Left">
 <ItemTemplate>
 <asp:Label ID="lblComDesc" runat="server" Text='<% # Eval("com_desc") %>' Width="230px"></asp:Label>
 </ItemTemplate>
 <EditItemTemplate>
 <asp:TextBox SkinID="tbPlain" ID="txtComDesc" runat="server" Text='<% # Eval("com_desc") %>' Width="230px" Font-Size="8pt" AutoPostBack="false"></asp:TextBox>
 </EditItemTemplate> 
 <FooterTemplate>
 <asp:TextBox SkinID="tbPlain" ID="txtComDesc" runat="server" Text="" Width="230px" AutoPostBack="false"></asp:TextBox>
 </FooterTemplate> 

<ItemStyle HorizontalAlign="Left" Width="230px"></ItemStyle>
 </asp:TemplateField>
<asp:TemplateField HeaderText="Active?" ItemStyle-HorizontalAlign="Center"
  FooterStyle-HorizontalAlign="Center">        
        <ItemTemplate>
            <asp:CheckBox ID="chkSelect"  runat="server" Checked='<%# Eval("check").ToString().Equals("True") %>'/>
        </ItemTemplate>
        <FooterTemplate>
            <asp:CheckBox ID="chkSelect" Checked="true" runat="server" />
        </FooterTemplate>

<FooterStyle HorizontalAlign="Center"></FooterStyle>

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

