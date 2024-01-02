<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="frmClass.aspx.cs" Inherits="frmClass" Title="Employee Category Setup" Theme="Themes" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">

<div id="frmMainDiv" style="background-color:White; width:100%;">
<table style="width:100%;" >
<tr>
<td style="width:1%;"></td>
<td style="width:98%;" align="center">
<asp:Label ID ="lblTranStatus" runat="server" Font-Size="8pt" Text="" Visible="false"></asp:Label>
<asp:GridView CssClass="mGrid" PagerStyle-CssClass="pgr"  AlternatingRowStyle-CssClass="alt" ID="dgClass" runat="server" AutoGenerateColumns="False" 
        AllowPaging="True" Width="100%" BackColor="White" BorderWidth="1px" BorderStyle="Solid"
        CellPadding="2" BorderColor="LightGray" Font-Size="8pt" 
        AllowSorting="True" PageSize="20" onrowcancelingedit="dgClass_RowCancelingEdit" 
        onrowdeleting="dgClass_RowDeleting" onrowediting="dgClass_RowEditing" 
        onrowupdating="dgClass_RowUpdating" 
        onrowdatabound="dgClass_RowDataBound" onrowcommand="dgClass_RowCommand">
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
  <asp:Label ID="lblClassId" runat="server" Text='<%#Eval("class_id") %>' Width="150px"></asp:Label>  
  </ItemTemplate>
  <EditItemTemplate>
  <asp:TextBox SkinID="tbGray" ID="txtClassId" CssClass="tbc" 
            runat="server" Width="150px" TabIndex="1" Text='<%#Eval("class_id") %>' Font-Size="8pt" MaxLength="30"></asp:TextBox>
  </EditItemTemplate>
  <FooterTemplate>
  <asp:TextBox SkinID="tbGray" ID="txtClassId"  CssClass="tbc"
            runat="server" Width="150px" TabIndex="2" Font-Size="8pt" MaxLength="30"></asp:TextBox>
  </FooterTemplate>
      <ItemStyle HorizontalAlign="Center" Width="150px" />
  </asp:TemplateField>
  
  <asp:TemplateField HeaderText="Class Name">
  <ItemTemplate>
  <asp:Label ID="lblClassName" runat="server" Text='<%#Eval("class_name") %>' Width="200px"></asp:Label>  
  </ItemTemplate>
  <EditItemTemplate>
  <asp:TextBox SkinID="tbGray" ID="txtClassName" CssClass="tbl" 
            runat="server" Width="200px" TabIndex="1" Text='<%#Eval("class_name") %>' Font-Size="8pt" MaxLength="30"></asp:TextBox>
  </EditItemTemplate>
  <FooterTemplate>
  <asp:TextBox SkinID="tbGray" ID="txtClassName"  CssClass="tbl"
            runat="server" Width="200px" TabIndex="2" Font-Size="8pt" MaxLength="30"></asp:TextBox>
  </FooterTemplate>
      <ItemStyle HorizontalAlign="Center" Width="200px" />
  </asp:TemplateField>
  
</Columns>
<AlternatingRowStyle BackColor="" />
</asp:GridView>  
</td>
<td style="width:1%;"></td>
</tr>
</table>
</div>
</asp:Content>

