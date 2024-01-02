<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Coa.aspx.cs" Inherits="Coa" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">  
<div>
 	<table id="Table1"  >
	<tr >
	<td style="width: 100px; height: 27px;" align="left">
	<asp:Label ID="lblGlCoaCode" runat="server" Font-Size="8">COA Code</asp:Label></td>
	<td style="width: 140px; height: 27px;" align="left">
    <asp:TextBox ID="txtGlCoaCode" runat="server" Width="140px"  AutoPostBack="False" ></asp:TextBox></td>
    
    
    <td style="height: 27px">&nbsp&nbsp</td>
	
	<td style="width: 90px; height: 27px;" align="left">
	<asp:Label ID="lblCoaEnaled" runat="server" Font-Size="8">COA Enabled?</asp:Label></td>
	<td style="width: 166px; height: 27px;" align="left">
    <asp:TextBox ID="txtCoaEnabled" runat="server" Width="112px"  AutoPostBack="False"></asp:TextBox></td>    
    </tr>
    
    <tr >
	<td style="width: 100px; height: 27px;" align="left">
	<asp:Label ID="lblEffectiveFrom" runat="server" Font-Size="8">Effective From</asp:Label></td>
	<td style="width: 120px; height: 27px;" align="left">
    <asp:TextBox ID="txtEffectiveFrom" runat="server" Width="112px"  AutoPostBack="False" ></asp:TextBox></td>
    
    
    <td style="height: 27px">&nbsp&nbsp</td>
	
	<td style="width: 90px; height: 27px;" align="left">
	<asp:Label ID="lblEffectiveTo" runat="server" Font-Size="8">Effective To</asp:Label></td>
	<td style="width: 166px; height: 27px;" align="left">
    <asp:TextBox ID="txtEffectiveTo" runat="server" Width="112px"  AutoPostBack="False"></asp:TextBox></td>    
    </tr>
    
    <tr >
	<td style="width: 100px; height: 27px;" align="left">
	<asp:Label ID="lblBudAllowed" runat="server" Font-Size="8">Budget Allowed</asp:Label></td>
	<td style="width: 120px; height: 27px;" align="left">
    <asp:TextBox ID="txtBudAllowed" runat="server" Width="112px"  AutoPostBack="False" ></asp:TextBox></td>
    
    
    <td style="height: 27px">&nbsp&nbsp</td>
	
	<td style="width: 90px; height: 27px;" align="left">
	<asp:Label ID="lblPostAllowed" runat="server" Font-Size="8">Post Allowed</asp:Label></td>
	<td style="width: 166px; height: 27px;" align="left">
    <asp:TextBox ID="txtPostAllowed" runat="server" Width="112px"  AutoPostBack="False"></asp:TextBox></td>    
    </tr>
    
    <tr >
	<td style="width: 100px; height: 27px;" align="left">
	<asp:Label ID="lblTaxable" runat="server" Font-Size="8">Taxable</asp:Label></td>
	<td style="width: 120px; height: 27px;" align="left">
    <asp:TextBox ID="txtTaxable" runat="server" Width="112px"  AutoPostBack="False" ></asp:TextBox></td>
    
    
    <%-- <td style="height: 27px">&nbsp&nbsp</td> --%>
	
	   
    </tr>
    <tr>
    <td style="width: 90px; height: 27px;" align="left">
	<asp:Label ID="lblCoaDesc" runat="server" Font-Size="8" Width="90px">COA Desc</asp:Label></td>
	<td style="width: 367px; height: 27px;" align="left"  colspan="4">
    <asp:TextBox ID="txtCoaDesc" runat="server" Width="367px"  AutoPostBack="False"></asp:TextBox></td> 
    </tr>
    <tr>
    
    <td style="width: 100px; height: 27px;" align="left">
	<asp:Label ID="lblCoaCurrBal" runat="server" Font-Size="8">Current Balance</asp:Label></td>
	<td style="width: 120px; height: 27px;" align="left">
    <asp:TextBox ID="txtCoaCurrbal" runat="server" Width="112px"  AutoPostBack="False" ></asp:TextBox></td>
    
    <td style="height: 27px">&nbsp&nbsp</td>
	
	<td style="width: 90px; height: 27px;" align="left">
	<asp:Label ID="Label1" runat="server" Font-Size="8">Status</asp:Label></td>
	<td style="width: 166px; height: 27px;" align="left">
    <asp:TextBox ID="txtStatus" runat="server" Width="112px"  AutoPostBack="False"></asp:TextBox></td>
    
    </tr>
    </table>
    <br />
    <br />
    <table>
   <tr>
   <td>
   <asp:LinkButton ID="btnSave" runat="server" Text="Save" 
           Font-Size="10" onclick="btnSave_Click"></asp:LinkButton>
   </td>
   <td>&nbsp&nbsp</td>
   <td> 
       <asp:LinkButton ID="btnClear" runat="server" text="Clear" 
           onclick="btnClear_Click" Font-Size="10" ></asp:LinkButton></td>
   <td>&nbsp&nbsp</td>
   </tr>
   </table>
</div>   
<br />
<br />
 <asp:GridView CssClass="mGrid" PagerStyle-CssClass="pgr"  AlternatingRowStyle-CssClass="alt"  ID="dgGlCoa" runat="server" AutoGenerateColumns="false" 
        AllowPaging="True" Width="75%" BackColor="White" BorderWidth="1px" BorderStyle="Solid"
        CellPadding="2" CellSpacing="0" BorderColor="Salmon" Font-Size="8pt" AllowSorting="true" PageSize="5" 
        onselectedindexchanged="dgGlCoa_SelectedIndexChanged">
  <HeaderStyle Font-Size="9" Font-Names="Arial" Font-Bold="True" BackColor="Blue" HorizontalAlign="center" ForeColor="White"/>

  <Columns>
  <asp:CommandField ShowSelectButton="True"  />
  
  <asp:BoundField  HeaderText="COA Code" DataField="gl_coa_code"></asp:BoundField>  
  <asp:BoundField  HeaderText="COA Desc" DataField="coa_desc"></asp:BoundField>
  <asp:BoundField  HeaderText="Account Type" DataField="acc_type"></asp:BoundField>
  <asp:BoundField  HeaderText="Status" DataField="status">
  <ItemStyle Width="50px" />
  </asp:BoundField>
  </Columns>
  
</asp:GridView>

</asp:Content>

