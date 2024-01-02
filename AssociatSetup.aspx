<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="AssociatSetup.aspx.cs" Inherits="AssociatSetup" Title="Associations Info" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
<script src='<%= ResolveUrl("~/Scripts/valideDate.js") %>' type="text/javascript"></script>

<div style="height:580px; min-height:580px; height:auto !important; width:100%; text-align:center; margin-top:5em;">
<asp:Label ID ="lblTranStatus" runat="server" Font-Size="8pt" Text="" Visible="false"></asp:Label>
<asp:GridView CssClass="mGrid" PagerStyle-CssClass="pgr"  AlternatingRowStyle-CssClass="alt" ID="dgAss" runat="server" AutoGenerateColumns="False" 
        AllowPaging="True" Width="100%" BackColor="White" BorderWidth="1px" BorderStyle="Solid"
        CellPadding="2" BorderColor="LightGray" Font-Size="8pt" 
        AllowSorting="True" PageSize="20" onrowcancelingedit="dgAss_RowCancelingEdit" 
        onrowdeleting="dgAss_RowDeleting" onrowediting="dgAss_RowEditing" 
        onrowupdating="dgAss_RowUpdating" 
        onrowdatabound="dgAss_RowDataBound" onrowcommand="dgAss_RowCommand">
  <HeaderStyle Font-Size="8pt"  Font-Bold="True" BackColor="LightGray"
        HorizontalAlign="Center" ForeColor="Black"/>

  <Columns>
  <asp:TemplateField>
  <ItemTemplate> 
  <asp:LinkButton ID="lbEdit" runat="server" CausesValidation="False" CommandName="Edit" Text="Edit" ></asp:LinkButton>  
  <asp:LinkButton ID="lbDelete" runat="server" CausesValidation="False" CommandName="Delete" Text="Remove"
  onclientclick="javascript:return window.confirm('are u really want to delete  these data')"></asp:LinkButton>  
  <asp:LinkButton ID="btnAddDet" runat="server" CommandName="AddNew" Text="New"></asp:LinkButton>  
  </ItemTemplate>
  <EditItemTemplate>  
  <asp:LinkButton ID="lbUpdate" runat="server" CausesValidation="True" CommandName="Update" Text="Update" ></asp:LinkButton> 
  <asp:LinkButton ID="lbCancel" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel"></asp:LinkButton>  
  </EditItemTemplate>
  <FooterTemplate>  
  <asp:LinkButton ID="lbInsert" runat="server" CommandName="Insert" Text="Add" ></asp:LinkButton>   
  <asp:LinkButton ID="lbCancel" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel"></asp:LinkButton>    
  </FooterTemplate>
      <ItemStyle Font-Size="8pt" Width="130px" />
  </asp:TemplateField>
  
  <asp:TemplateField HeaderText="System ID">
  <ItemTemplate>
  <asp:Label ID="lblAssoId" runat="server" Text='<%#Eval("asso_id") %>' Width="80px"></asp:Label>  
  </ItemTemplate>
  <EditItemTemplate>
  <asp:TextBox SkinID="tbGray" ID="txtAssoId" CssClass="tbc" 
            runat="server" Width="80px" TabIndex="1" Text='<%#Eval("asso_id") %>' Font-Size="8pt" MaxLength="3"></asp:TextBox>
  </EditItemTemplate>
  <FooterTemplate>
  <asp:TextBox SkinID="tbGray" ID="txtAssoId"  CssClass="tbc"
            runat="server" Width="80px" TabIndex="2" Font-Size="8pt" MaxLength="3"></asp:TextBox>
  </FooterTemplate>
      <ItemStyle HorizontalAlign="Center" Width="80px" />
  </asp:TemplateField>
  
  <asp:TemplateField HeaderText="Association Name" ItemStyle-Height="20px">
  <ItemTemplate>
  <asp:Label ID="lblAssoName" runat="server" Text='<%#Eval("asso_name") %>' Width="550px"></asp:Label>  
  </ItemTemplate>
  <EditItemTemplate>
  <asp:TextBox SkinID="tbGray" ID="txtAssoName"  CssClass="tbl" 
            runat="server" Width="550px" TabIndex="3" Text='<%#Eval("asso_name") %>' Font-Size="8pt" MaxLength="35"></asp:TextBox>
  </EditItemTemplate>
  <FooterTemplate>
  <asp:TextBox SkinID="tbGray" ID="txtAssoName"  CssClass="tbl"
            runat="server" Width="550px" TabIndex="4" Font-Size="8pt" MaxLength="35"></asp:TextBox>
  </FooterTemplate>
      <ItemStyle HorizontalAlign="Left" Width="550px" />
  </asp:TemplateField>
  
  <asp:TemplateField HeaderText="Abbreviation">
  <ItemTemplate>
  <asp:Label ID="lblAssoAbvr" runat="server" Text='<%#Eval("asso_abvr") %>'></asp:Label>  
  </ItemTemplate>
  <EditItemTemplate>
  <asp:TextBox SkinID="tbGray" ID="txtAssoAbvr"  CssClass="tbc"
            runat="server" Width="100px" TabIndex="5" Text='<%#Eval("asso_abvr") %>' Font-Size="8pt" MaxLength="10"></asp:TextBox>
  </EditItemTemplate>
  <FooterTemplate>
  <asp:TextBox SkinID="tbGray" ID="txtAssoAbvr" CssClass="tbc" 
            runat="server" Width="100px" TabIndex="6" Font-Size="8pt" MaxLength="7"></asp:TextBox>
  </FooterTemplate>
      <ItemStyle HorizontalAlign="Center" Width="100px" />
  </asp:TemplateField>
  
</Columns>
<EditRowStyle BackColor="" />
                        <AlternatingRowStyle BackColor="" />
</asp:GridView>  
</div>
</asp:Content>

