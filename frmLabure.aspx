<%@ Page Title="Temp Employee.-DDP" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="frmLabure.aspx.cs" Inherits="frmLabure" Theme="Themes" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
<script>
    function SetImageNID() {
        document.getElementById('<% =lbImgUpload.ClientID %>').click();
    }
    function SetImage() {
        document.getElementById('<% =imgBtnsup.ClientID %>').click();
    }
</script>
    <div style="background-color:White; width:100%; min-height:700px; height:auto !important; height:700px; font-family:Tahoma;">
<div style="vertical-align:top;">
<table  id="pageFooterWrapper">
  <tr>  
        <td style="width:5%;"></td>
        <td align="center">
        <asp:Button ID="Delete" runat="server" ToolTip="Delete" onclick="btnDelete_Click"
            
                
                onclientclick="javascript:return window.confirm('are u really want to delete these data')" Text="Delete" 
        Height="35px" Width="120px" BorderStyle="Outset"  />
        </td>
        <td style="width:20px;"></td>
        <td align="center" >
        <asp:Button ID="btnSave" runat="server" ToolTip="Save Supplier Record" 
                onclick="btnSave_Click" Text="Save" 
        Height="35px" Width="120px" BorderStyle="Outset"  />
        </td>             
        <td style="width:20px;"></td>
        <td align="center" >
        <asp:Button ID="Clear" runat="server"  ToolTip="Clear" onclick="btnClear_Click" Text="Clear" 
        Height="35px" Width="120px" BorderStyle="Outset"  />
        </td>
         <td style="width:5%;"></td>
        <td style="width:5%;">
        <asp:Button ID="btnPrint" runat="server" ToolTip="Save Supplier Record" 
                onclick="btnPrint_Click" Text="Print" 
        Height="35px" Width="120px" BorderStyle="Outset"  />
        </td>
         <td style="width:5%;"></td>
   </tr>
   </table>
</div>
<table style="width:100%;"><tr>
<td style="width:1%;"></td>
<td style="width:98%;" align="center">
<table style="width:100%;">
<tr>
<td style="width:100%;" align="center"> 
<br />
<fieldset style="vertical-align: top; border: solid 1px #8BB381;text-align:left;"><legend style="color: maroon;"><b>Temporary Employee Information </b></legend>
<table style="width:100%;">

<td style="width:11%;" align="left">
<asp:Label ID="lblSupCode" runat="server" Font-Size="9pt">Employee Code</asp:Label>
</td>
<td style="width:25%;" align="left"> 
<asp:TextBox SkinID="tbPlain" ID="txtSupCode" runat="server"  Width="80%" 
        Font-Size="9pt" CssClass="tbc" Enabled="False"></asp:TextBox>
</td>
<td style="height: 27px; width:7%;" align="left" >
    <asp:CheckBox ID="CheckBox1" Checked="true" runat="server" Text="Active?" />
    </td>
<td style="width:10%;" align="left">
<asp:Label ID="lbLID" runat="server" Font-Size="9pt" Visible="False"></asp:Label>
    </td>
<td  style="width:25%;" align="left"> 
    &nbsp;</td>
<td  style="width:10%;" align="left"> 
            <asp:FileUpload ID="imgUploadNID" runat="server" Font-Size="8pt" Height="20px" 
            onchange="javascript:SetImageNID();" Size="15" Visible="true" />
        <asp:Button ID="lbImgUpload" runat="server" Font-Size="8pt" Height="20px" 
            onclick="lbImgUpload_Click" style="display:none;" Text="Upload" Width="50px" />
            &nbsp;<span style="color: #0033CC">&nbsp; NID Images</span></td>
</td>
</tr>
<tr>
<td style="width:11%;" align="left">
<asp:Label ID="lblSupName0" runat="server" Font-Size="9pt">Company Name</asp:Label>
</td>
<td style="width:20%;" align="left"> 
<asp:TextBox SkinID="tbPlain" ID="txtCompanyName" runat="server" Width="80%" 
        Font-Size="8pt"  CssClass="tbl"></asp:TextBox>
</td>
<td style="height: 27px; width:7%;" align="left" >&nbsp;</td>
<td style="width:15%;" align="left">
<asp:Label ID="lblSupAddr" runat="server" Font-Size="9pt">Office address </asp:Label>
</td>
<td  style="width:20%;" align="left"> 
<asp:TextBox SkinID="tbPlain" ID="txtAddress1" runat="server"  Width="80%" 
        Font-Size="9pt"  CssClass="tbc"></asp:TextBox>
    </td>
<td  style="width:25%;" align="left" rowspan="4" valign="top"> 
    <asp:Image ID="imgSupNID" runat="server" BackColor="#EFF3FB" 
        BorderStyle="Solid" BorderWidth="1px" Height="100px" Width="100%" />
    </td>
</tr>
<tr>
<td style="width:11%;" align="left">
<asp:Label ID="lblSupName" runat="server" Font-Size="9pt">Employee Name</asp:Label>
</td>
<td style="width:20%;" align="left"> 
<asp:TextBox SkinID="tbPlain" ID="txtSupplierName" runat="server" Width="80%" 
        Font-Size="8pt"  CssClass="tbl"></asp:TextBox>
&nbsp;
<asp:Label ID="lblSupAddr12" runat="server" Font-Size="9pt" Font-Bold="True" 
        ForeColor="#CC3300">*</asp:Label>
</td>
<td style="height: 27px; width:7%;" align="left" ></td>
<td style="width:15%;" align="left">
<asp:Label ID="lblSupAddr0" runat="server" Font-Size="9pt">Residential Address</asp:Label>
</td>
<td  style="width:20%;" align="left"> 
<asp:TextBox SkinID="tbPlain" ID="txtAddress2" runat="server" Width="80%" 
        Font-Size="8pt"></asp:TextBox>
    </td>
<td  style="width:20%;" align="left"> 
    &nbsp;</td>
</tr>
<tr>
<td style="width:11%;" align="left">
    <asp:Label ID="lbldateOfStart" runat="server" Text="Date of join "></asp:Label>
</td>
<td style="width:20%;" align="left"> 
<asp:TextBox SkinID="tbPlain" placeholder="dd/MM/yyyy" ID="txtJoinDate" runat="server" Width="80%" 
        Font-Size="8pt"  CssClass="tbl"></asp:TextBox>
        <ajaxToolkit:CalendarExtender ID="CalendarExtender1" runat="server" TargetControlID="txtJoinDate" Format="dd/MM/yyyy" />
</td>
<td style="height: 27px; width:7%;" align="left" >&nbsp;</td>
<td style="width:15%;" align="left">
    <asp:Label ID="lblDateofRetaired" runat="server" Text="Date Of Retired"></asp:Label>
</td>
<td  style="width:20%;" align="left"> 
<asp:TextBox SkinID="tbPlain" ID="txtRetairedDate" placeholder="dd/MM/yyyy" runat="server" Width="80%" 
        Font-Size="8pt"  CssClass="tbl"></asp:TextBox>
        <ajaxToolkit:CalendarExtender ID="AutoCompleteExtender1" runat="server" TargetControlID="txtRetairedDate" Format="dd/MM/yyyy" />
    </td>
<td  style="width:20%;" align="left"> 
    &nbsp;</td>
</tr>
<tr>
<td style="width:11%; height: 47px;" align="left">
<asp:Label ID="lblSupAddr1" runat="server" Font-Size="9pt">Designation</asp:Label>
</td>
<td style="width:20%; height: 47px;" align="left"> 
<asp:TextBox SkinID="tbPlain" ID="txtDesignation" runat="server" style="text-align:left;" Width="80%"
        Font-Size="9pt"  CssClass="tbc"></asp:TextBox>
</td>
<td style="height: 47px; width:7%;" align="left" ></td>
<td style="width:15%; height: 47px;" align="left">
<asp:Label ID="lblSupAddr2" runat="server" Font-Size="9pt">City</asp:Label>
</td>
<td style="width:20%; height: 47px;" align="left" > 
<asp:TextBox SkinID="tbPlain" ID="txtCity" runat="server" Width="80%" 
        Font-Size="8pt"  CssClass="tbl"></asp:TextBox> 
</td>
<td style="width:20%; height: 47px;" align="left" > 
    </td>
</tr>

<tr>
<td style="width:11%;" align="left">
<asp:Label ID="lblSupAddr3" runat="server" Font-Size="9pt">Mobile</asp:Label>
</td>
<td style="width:20%;" align="left"> 
<asp:TextBox SkinID="tbPlain" ID="txtMobile" runat="server"  Width="80%" style="text-align:left;"
        Font-Size="9pt"  CssClass="tbc"></asp:TextBox>
</td>
<td style="height: 27px; width:7%;" align="left" >&nbsp;</td>
<td style="width:15%;" align="left">
<asp:Label ID="lblSupAddr4" runat="server" Font-Size="9pt">State</asp:Label>
</td>
<td style="width:20%;" align="left" > 
<asp:TextBox SkinID="tbPlain" ID="txtState" runat="server" Width="80%" 
        Font-Size="8pt" CssClass="tbl" ></asp:TextBox> 
</td>
<td style="width:20%;" align="left" > 
    <asp:FileUpload ID="imgUploadsup" runat="server" Font-Size="8pt" Height="20px" 
        onchange="javascript:SetImage();" Size="15" Visible="true" />
    <asp:Button ID="imgBtnsup" runat="server" Font-Size="8pt" Height="20px" 
        onclick="imgBtnsup_Click" style="display:none;" Text="Upload" Width="50px" />
    </td>
</tr>

<tr>
<td style="width:11%;" align="left">
<asp:Label ID="lblSupAddr5" runat="server" Font-Size="9pt">Phone</asp:Label>
</td>
<td style="width:20%;" align="left"> 
<asp:TextBox SkinID="tbPlain" ID="txtPhone" runat="server"  Width="80%" style="text-align:left;"
        Font-Size="9pt" CssClass="tbc"></asp:TextBox>
</td>
<td style="height: 27px; width:7%;" align="left" >&nbsp;</td>
<td style="width:15%;" align="left">
<asp:Label ID="lblSupAddr6" runat="server" Font-Size="9pt">Postal Code</asp:Label>
</td>
<td style="width:20%;" align="left" > 
<asp:TextBox SkinID="tbPlain" ID="txtPostalCode" runat="server" Width="80%" 
        Font-Size="8pt"  CssClass="tbl"></asp:TextBox> 
</td>
<td style="width:25%;" align="left" rowspan="3" > 
    <asp:Image ID="imgSup" runat="server" BackColor="#EFF3FB" BorderStyle="Solid" 
        BorderWidth="1px" Height="90px" Width="100%" />
    </td>
</tr>

<tr>
<td style="width:11%;" align="left">
<asp:Label ID="lblSupAddr7" runat="server" Font-Size="9pt">National/Any valid ID</asp:Label>
</td>
<td style="width:20%;" align="left"> 
<asp:TextBox SkinID="tbPlain" ID="txtFax" runat="server"  Width="80%" style="text-align:left;"
        Font-Size="9pt"  CssClass="tbc"></asp:TextBox>
</td>
<td style="height: 22px; width:7%;" align="left" >&nbsp;</td>
<td style="width:15%;" align="left">
<asp:Label ID="lblSupAddr8" runat="server" Font-Size="9pt">Country</asp:Label>
</td>
<td style="width:20%;" align="left" > 
    <asp:DropDownList ID="ddlCountry" runat="server" Height="26px" Width="84%">
    </asp:DropDownList>
</td>
<td style="width:20%;" align="left" > 
    &nbsp;</td>
</tr>

<tr>
<td style="width:11%;" align="left">
<asp:Label ID="lblSupAddr9" runat="server" Font-Size="9pt">Email</asp:Label>
</td>
<td style="width:20%;" align="left"> 
<asp:TextBox SkinID="tbPlain" ID="txtEmail" runat="server"  Width="80%" style="text-align:left;"
        Font-Size="9pt"  CssClass="tbc"></asp:TextBox>
</td>
<td style="height: 38px; width:7%;" align="left" >&nbsp;</td>
<td style="width:15%;" align="left">
<asp:Label ID="lblSupAddr10" runat="server" Font-Size="9pt">Type</asp:Label>
</td>
<td style="width:20%;" align="left" > 
    <asp:DropDownList ID="ddlSupplierGroup" runat="server" Height="26px" 
        Width="84%">
        <asp:ListItem></asp:ListItem>
        <asp:ListItem Value="LP">Labour Person</asp:ListItem>
        <asp:ListItem Value="CP">Carriage Person</asp:ListItem>
        <asp:ListItem Value="OS">Other&#39;s</asp:ListItem>
    </asp:DropDownList>
&nbsp;
<asp:Label ID="lblSupAddr11" runat="server" Font-Size="9pt" Font-Bold="True" 
        ForeColor="#CC3300">*</asp:Label>
</td>
<td style="width:20%;" align="left" > 
    &nbsp;</td>
</tr>

</table>
</fieldset>
<br />
<asp:GridView CssClass="mGrid" PagerStyle-CssClass="pgr"  AlternatingRowStyle-CssClass="alt" 
ID="dgSup" runat="server" AutoGenerateColumns="False" Width="100%" 
        AllowPaging="True" BackColor="White" BorderWidth="1px" BorderStyle="Solid"
        CellPadding="2" BorderColor="LightGray" Font-Size="9pt" 
        AllowSorting="True" PageSize="20" 
        onselectedindexchanged="dgSup_SelectedIndexChanged" ForeColor="#333333" 
        onpageindexchanging="dgSup_PageIndexChanging" 
        onrowdatabound="dgSup_RowDataBound"  >
  <HeaderStyle Font-Size="9pt"  Font-Bold="True" HorizontalAlign="center" BackColor="Silver"/>
  <FooterStyle BackColor="Silver" Font-Bold="True" ForeColor="White" />
  <Columns>
  <asp:CommandField ShowSelectButton="True"  ItemStyle-Width="40px" 
          ItemStyle-HorizontalAlign="Center" ItemStyle-ForeColor="Blue" 
          ItemStyle-Height="25px">
<ItemStyle HorizontalAlign="Center" ForeColor="Blue" Height="25px" Width="40px"></ItemStyle>
      </asp:CommandField>
  <asp:BoundField  HeaderText="Labour Code" DataField="Code" ItemStyle-Width="100px" 
          ItemStyle-HorizontalAlign="Center">
<ItemStyle HorizontalAlign="Center" Width="100px"></ItemStyle>
      </asp:BoundField>
  <asp:BoundField  HeaderText="Name" DataField="ContactName" ItemStyle-Width="200px" 
          ItemStyle-HorizontalAlign="Left">
<ItemStyle HorizontalAlign="Left" Width="200px"></ItemStyle>
      </asp:BoundField>
  <asp:BoundField  HeaderText="Address" DataField="Address1" ItemStyle-Width="200px" 
          ItemStyle-HorizontalAlign="Left">  
<ItemStyle HorizontalAlign="Left" Width="200px"></ItemStyle>
      </asp:BoundField>
  <asp:BoundField  HeaderText="Mobile" DataField="Mobile" ItemStyle-Width="100px" 
          ItemStyle-HorizontalAlign="Left">
<ItemStyle HorizontalAlign="Left" Width="100px"></ItemStyle>
      </asp:BoundField>
   <asp:BoundField  HeaderText="ID" DataField="ID" ItemStyle-Width="100px" 
          ItemStyle-HorizontalAlign="Left">
<ItemStyle HorizontalAlign="Left" Width="100px"></ItemStyle>
      </asp:BoundField>
  </Columns>
                        <RowStyle BackColor="white" />
                        <EditRowStyle BackColor="" />
                        <PagerStyle  HorizontalAlign="Center" />
                        <AlternatingRowStyle BackColor="" />
  </asp:GridView>
</td>
</tr>
</table>
</td>
<td style="width:1%;"></td>
</tr></table>
</div>
</asp:Content>

