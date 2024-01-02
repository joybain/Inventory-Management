<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="frmDesignation.aspx.cs" Inherits="frmDesignation" Title="Designation Setup.-DDP" Theme="Themes" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <script src='<%= ResolveUrl("~/Scripts/valideDate.js") %>' type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        function isNumber(evt) {
            evt = (evt) ? evt : window.event;
            var charCode = (evt.which) ? evt.which : evt.keyCode;
            if (charCode != 46 && charCode > 31
            && (charCode < 48 || charCode > 57)) {
                return false;
            }
            return true;
        }
       
</script> 
<div id="frmMainDiv" style="background-color:White; width:100%;">
<table  id="pageFooterWrapper">
 <tr>
	<td align="center"><asp:Button   ID="BtnDelete" runat="server"  ToolTip="Delete Record"  
            OnClick="BtnDelete_Click"  TabIndex="903" 
            
            onclientclick="javascript:return window.confirm('are u really want to delete  these data')" Text="Delete" 
        Height="35px" Width="100px" BorderStyle="Outset"  /></td>
	<td align="center">
	<asp:Button  ID="BtnFind" runat="server"  ToolTip="Find" 
            OnClick="BtnFind_Click"  TabIndex="902" Text="Find" 
        Height="35px" Width="100px" BorderStyle="Outset" Visible="False"  />
	</td>
	<td align="center">
        <asp:Button ID="BtnSave" runat="server" ToolTip="Save or Update Record" 
            OnClick="BtnSave_Click" TabIndex="901" Text="Save" 
        Height="35px" Width="100px" BorderStyle="Outset"  /></td>
	<td align="center">
        <asp:Button ID="BtnReset" runat="server" ToolTip="Clear Form" 
            OnClick="BtnReset_Click"  TabIndex="904" Text="Clear" 
        Height="35px" Width="100px" BorderStyle="Outset"  /></td>           
	</tr>
	</table>	
<table style="width:100%;">
<tr>
<td style="width:1%;"></td>
<td style="width:98%;" align="center">
 
<table style="width:100%; font-size:8pt;">
<tr>
  <td style="width: 150px; height: 9px;" align="left"></td>
	<td style="width: 200px; height: 9px;" align="left">
        </td>
	
	<td style="width: 50px; height: 9px;" align="left"></td>	
	
	<td style="width: 100px; height: 9px;" align="left"></td>
	<td style="width: 200px; height: 9px;" align="left">
        </td>        
</tr>
<tr>
  <td style="width: 150px; height: 9px;" align="left">&nbsp;</td>
	<td style="width: 200px; height: 9px;" align="left">
        &nbsp;</td>
	
	<td style="width: 50px; height: 9px;" align="left">&nbsp;</td>	
	
	<td style="width: 100px; height: 9px;" align="left">&nbsp;</td>
	<td style="width: 200px; height: 9px;" align="left">
        &nbsp;</td>        
</tr>
<tr>
  <td style="width: 150px; height: 9px;" align="left">&nbsp;</td>
	<td style="height: 9px;" align="left" colspan="3">
	     <fieldset style="vertical-align: top; border: solid 1px #8BB381; text-align:left;">
                    <legend style="color: maroon;"><b>Designation Setup</b></legend> 
        <table style="width: 100%">
            <tr>
                <td style="width: 30%; height: 27px;" align="right">
                    Serial No.</td>
                <td style="width: 5%; height: 27px;">
                    </td>
                <td style="width: 75%; height: 27px;">
        <asp:TextBox SkinID="tbGray" ID="txtDesigCode"  CssClass="tbc"
            runat="server" Width="40%" Font-Size="8pt" MaxLength="3" TabIndex="1" 
            Enabled="False" Visible="False" ></asp:TextBox>
        <asp:TextBox SkinID="tbGray" ID="txtSerial"  CssClass="tbc" onkeypress="return isNumber(event)" 
            runat="server" Width="40%" Font-Size="8pt" MaxLength="3" TabIndex="1" ></asp:TextBox></td>
            </tr>
            <tr>
                <td style="width: 30%; height: 26px;" align="right">
                    Name</td>
                <td style="width: 5%; height: 26px;">
                    </td>
                <td style="width: 75%; height: 26px;">
        <asp:TextBox SkinID="tbGray" ID="txtDesigName"  CssClass="tbl" Enabled="true" Text=""
            runat="server" Width="97%" TabIndex="2" Font-Size="8pt" MaxLength="40"></asp:TextBox> </td>
            </tr>
        </table>
        </fieldset>
    </td>
	
	<td style="width: 200px; height: 9px;" align="left">
        &nbsp;</td>        
</tr>
<tr>
  <td style="width: 150px; height: 9px;" align="left">&nbsp;</td>
	<td style="height: 9px;" align="left" colspan="3">
<asp:GridView CssClass="mGrid" PagerStyle-CssClass="pgr"  
        AlternatingRowStyle-CssClass="alt" ID="dgDesig" runat="server" AutoGenerateColumns="False"
        AllowPaging="True" Width="100%" BackColor="White" BorderWidth="1px" BorderStyle="Solid"
        CellPadding="2" BorderColor="LightGray" Font-Size="8pt" 
        AllowSorting="True" PageSize="20" ForeColor="#333333" 
        onselectedindexchanged="dgDesig_SelectedIndexChanged" 
        onpageindexchanging="dgDesig_PageIndexChanging" 
        onrowdatabound="dgDesig_RowDataBound">
  <Columns>
  <asp:CommandField ShowSelectButton="True"  ItemStyle-Width="5%" 
          ItemStyle-HorizontalAlign="Center" ItemStyle-ForeColor="Blue" 
          ItemStyle-Height="20px">
<ItemStyle HorizontalAlign="Center" ForeColor="Blue" Height="20px" Width="5%" 
          Font-Bold="True" Font-Size="10pt"></ItemStyle>
      </asp:CommandField>
  <asp:BoundField  HeaderText="Desig Id" DataField="desig_code" ItemStyle-Width="0%" 
          ItemStyle-HorizontalAlign="Center">
<ItemStyle HorizontalAlign="Center" Width="0%"></ItemStyle>
      </asp:BoundField>
      <asp:BoundField  HeaderText="Serial No." DataField="Serial" ItemStyle-Width="0%" 
          ItemStyle-HorizontalAlign="Center">
<ItemStyle HorizontalAlign="Center" Width="8%"></ItemStyle>
      </asp:BoundField>
  <asp:BoundField  HeaderText="Designation Name" DataField="desig_name" 
          ItemStyle-Width="30%" ItemStyle-HorizontalAlign="Left">  
<ItemStyle HorizontalAlign="Left" Width="30%"></ItemStyle>
      </asp:BoundField>
  </Columns>
                        <RowStyle BackColor="white" />
                        <PagerStyle BackColor="" HorizontalAlign="Center" />
                        <HeaderStyle BackColor="Silver" Font-Bold="True" ForeColor="Black" />
                        <AlternatingRowStyle BackColor="" />
  </asp:GridView>
    </td>
	
	<td style="width: 200px; height: 9px;" align="left">
        &nbsp;</td>        
</tr>
<tr>
  <td style="height: 27px;" align="left" colspan="5" id="DesignationID" runat="server">
      
  <table style="width:100%">
    <tr>
  <td style="width: 150px; height: 27px;" align="left">Designation ID</td>
	<td style="width: 200px; height: 27px;" align="left">
        &nbsp;</td>
	
	<td style="width: 50px; height: 27px;" align="left"></td>	
	
	<td style="width: 100px; height: 27px;" align="left">&nbsp;</td>
	<td style="width: 200px; height: 27px;" align="left">
        &nbsp;</td>        
</tr>
<tr>
  <td style="width: 150px; height: 27px;" align="left">
      <asp:Label ID="Label3" runat="server" Text="Supervisor" Visible="False"></asp:Label>
    </td>
	<td style="width: 200px; height: 27px;" align="left">
        <asp:DropDownList SkinID="ddlPlain" ID="ddlMgrCode" runat="server" 
            Font-Size="8pt" Width="100%" Height="18px" TabIndex="8" Visible="False" >
     </asp:DropDownList></td>
	
	<td style="width: 50px; height: 27px;" align="left"></td>
	
	
	<td style="width: 100px; height: 27px;" align="left">
        <asp:Label ID="Label6" runat="server" Text="Grade Code" Visible="False"></asp:Label>
    </td>
	<td style="width: 200px; height: 27px;" align="left">
        <asp:DropDownList SkinID="ddlPlain" ID="ddlGradeCode" runat="server" 
            Font-Size="8pt" Width="100%" Height="18px" TabIndex="9" Visible="False" >
     </asp:DropDownList> </td>        
</tr>

<tr>
  <td style="width: 150px; height: 27px;" align="left">
      <asp:Label ID="Label4" runat="server" Text="Category 1" Visible="False"></asp:Label>
    </td>
	<td style="width: 200px; height: 27px;" align="left">
     <asp:DropDownList SkinID="ddlPlain" ID="ddlOfficerOrStaff" runat="server" 
            Font-Size="8pt" Width="100%" Height="18px" TabIndex="10" Visible="False" >
     <asp:ListItem></asp:ListItem>
     <asp:ListItem Text="Managing Director" Value="T"></asp:ListItem>      
     <asp:ListItem Text="Officer" Value="O"></asp:ListItem> 
     <asp:ListItem Text="Staff" Value="S"></asp:ListItem>
     </asp:DropDownList></td>
	
	<td style="width: 50px; height: 27px;" align="left"></td>
	
	
	<td style="width: 100px; height: 27px;" align="left">
        <asp:Label ID="Label7" runat="server" Text="Category 2" Visible="False"></asp:Label>
    </td>
	<td style="width: 200px; height: 27px;" align="left">
     <asp:DropDownList SkinID="ddlPlain" ID="ddlClass" runat="server" Font-Size="8pt" 
            Width="100%" Height="18px" TabIndex="11" Visible="False" >
     </asp:DropDownList> </td>        
</tr>

<tr>
  <td style="width: 150px; height: 27px;" align="left">
      <asp:Label ID="Label5" runat="server" Text="Category 3" Visible="False"></asp:Label>
    </td>
	<td style="width: 200px; height: 27px;" align="left">
        <asp:DropDownList SkinID="ddlPlain" ID="ddlTectNtech" runat="server" Font-Size="8pt" 
            Width="100%" Height="18px" TabIndex="12" Visible="False" >
     <asp:ListItem></asp:ListItem>       
     <asp:ListItem Text="Non Technical" Value="N"></asp:ListItem> 
     <asp:ListItem Text="Technical" Value="T"></asp:ListItem>
     </asp:DropDownList></td>
	
	<td style="width: 50px; height: 27px;" align="left"></td>	
	
	<td style="width: 100px; height: 27px;" align="left">
        <asp:Label ID="Label8" runat="server" Text="Short Name" Visible="False"></asp:Label>
    </td>
	<td style="width: 200px; height: 27px;" align="left">
       <asp:TextBox SkinID="tbGray" ID="txtDesigAbb"  CssClass="tbc" 
            runat="server" Width="97%"  Font-Size="8pt" Enabled="true"
            MaxLength="11" TabIndex="35" Visible="False" ></asp:TextBox></td>        
</tr>
  </table>
  </td>
</tr>

</table>
<br />
</td>
<td style="width:1%;"></td>
</tr>
</table>
</div>
</asp:Content>

