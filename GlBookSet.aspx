<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="GlBookSet.aspx.cs" Inherits="GlBookSet" Title="Account Book Setup"  Theme="Themes"%>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">  

    <%-- <script type="text/javascript">
    function SetImage()
    {        
        document.getElementById('<% =lbImgUpload.ClientID %>').click();
    }    
    </script>--%>
  <script type="text/javascript">

        function previewFile() {

            var preview = document.querySelector('#<%=imgLogo.ClientID %>');
            var file = document.querySelector('#<%=imgUpload.ClientID %>').files[0];
            var reader = new FileReader();

            reader.onloadend = function () {
                preview.src = reader.result;
                $("#lblWorning").hide();
            }

            if (file) {
                reader.readAsDataURL(file);
            } else {
                preview.src = "";
            }
        }
    </script>

<div id="frmMainDiv" style="width:98.5%; background-color:transparent; padding:10px;"> 
<table style="width:100%; font-family:Verdana; background-color:white;"><tr>
<td style="width:1%;"></td>
<td style="width:98%;" align="center">

<table id="pageFooterWrapper">
 <tr>
    <td align="center">
        <asp:Button   ID="btnDelete" runat="server"  ToolTip="Delete Book"  
            OnClick="btnDelete_Click" onclientclick="javascript:return window.confirm('are u really want to delete  these data')"                
            Text="Delete" 
             Width="50%" Height="25px" BorderStyle="Outset" BorderWidth="1px" /></td> 
    <td align="center">
	<asp:Button  ID="btnFind" runat="server"  ToolTip="Find Account Book" 
            OnClick="btnFind_Click"    
            Text="Find" Width="50%" Height="25px" BorderStyle="Outset" 
            BorderWidth="1px" />
	</td>        
	<td align="center"><asp:Button ID="btnSave" runat="server" ToolTip="Save or Update Record" 
            OnClick="btnSave_Click" Text="Save" Width="50%" Height="25px" 
            BorderStyle="Outset" BorderWidth="1px" /></td>
	
	<td align="center">
        <asp:Button ID="btnClear" runat="server" ToolTip="Clear Form" 
            OnClick="btnClear_Click"     
            Text="Clear" Width="50%" Height="30px" BorderStyle="Outset" 
            BorderWidth="1px"  /></td>
	          
	</tr>
	</table>
    
 <br />

<table style="border:solid 1px lightgray; width:99%;">
<tr style="height:10px;"><td colspan="8"></td></tr>
<tr style="height:35px;">
<td style="width:14%; vertical-align:middle;" align="left">
<asp:Label ID="lblBookName" runat="server" Font-Size="8pt">Book Name</asp:Label>
</td>
<td style="width:17%; vertical-align:middle;" align="left"> 
<asp:TextBox SkinID="tbGray" ID="txtBookName" runat="server"  Width="100%" Font-Size="8pt" Enabled="true" MaxLength="3"></asp:TextBox>
</td>
<td  style="width:5%; vertical-align:middle; height:27px;" align="left"></td>
<td style="width:14%; vertical-align:middle;" align="left">
<asp:Label ID="lblBookDesc" runat="server" Font-Size="8pt">Description</asp:Label>
</td>
<td style="width:51%; vertical-align:middle;" align="left" colspan="4"> 
<asp:TextBox SkinID="tbGray" ID="txtBookDesc" runat="server" Width="98%" 
        Font-Size="8pt" MaxLength="150" ></asp:TextBox></td>
</tr>
<tr style="height:35px;">
<td style="width:14%; vertical-align:middle;" align="left">
<asp:Label ID="lblFinYear" runat="server" Font-Size="8pt">Status</asp:Label>
</td>
<td style="width:17%; vertical-align:middle;" align="left"> 
  <asp:DropDownList SkinID="ddlPlain" ID="ddlBookStatus" runat="server"  Font-Size="8pt" Width="100%" >
  <asp:ListItem></asp:ListItem>
  <asp:ListItem Text="Open" Value="O"></asp:ListItem>
  <asp:ListItem Text="Close" Value="C"></asp:ListItem>
  </asp:DropDownList>
</td>
<td style="width:5%; vertical-align:middle; height:27px;" align="left"></td>
<td style="width:14%; vertical-align:middle;" align="left">
<asp:Label ID="lblFinStartDt" runat="server" Font-Size="8pt">Separator Type</asp:Label>
</td>
<td style="width:17%; vertical-align:middle;" align="left"> 
<asp:TextBox SkinID="tbGray" ID="txtSeparatorType" runat="server"  Width="100%" Font-Size="8pt" 
        MaxLength="1" ></asp:TextBox>
</td>
<td style="width:5%; vertical-align:middle; height:27px;" align="left"></td>
<td style="width:31%; vertical-align:top;" align="left" colspan="2" rowspan="3">
<table style="width:100%; vertical-align:top;">
<tr style="height:35px;">
<td colspan="2" style="width:100%; font-size:8pt; vertical-align:top; padding-right:5px;" align="right">
<asp:FileUpload ID="imgUpload" runat="server" Visible="true" Size="25%" Height="20px" Font-Size="8pt" onchange="previewFile()"/>
<asp:Button ID="lbImgUpload" runat="server" Text="Upload" Font-Size="XX-Small"  style="display:none;"
        Width="50px" Height="20px" onclick="lbImgUpload_Click"></asp:Button>
</td>
</tr>
<tr style="height:35px;">
<td style="width:50%; vertical-align:middle;" align="left">
<asp:Label ID="Label4" runat="server" Font-Size="8pt">Logo</asp:Label>
</td>
<td style="width: 50%; height: 54px; vertical-align:top; padding-right:5px;" align="right">
    <asp:Image ID="imgLogo" runat="server" Height="50px" Width="100px" BorderStyle="Solid" BackColor="#EFF3FB" BorderWidth="1px" />
</td>
</tr>
</table>
</td>
</tr>
<tr>
<td style="width:14%; vertical-align:middle;" align="left">
<asp:Label ID="lblAddress1" runat="server" Font-Size="8pt">Address (Line1)</asp:Label>
</td>
<td style="width:51%; vertical-align:middle;" align="left" colspan="4"> 
<asp:TextBox SkinID="tbGray" ID="txtCompanyAddress1" runat="server"  Width="100%" 
        Font-Size="8pt" MaxLength="100" ></asp:TextBox>
</td>
<td style="width:5%; vertical-align:middle; height:27px;" align="left"></td>
</tr>
<tr style="height:35px;">
<td style="width:14%; vertical-align:middle;" align="left">
<asp:Label ID="Label1" runat="server" Font-Size="8pt">Address (Line2)</asp:Label>
</td>
<td style="width:51%; vertical-align:middle;" align="left" colspan="4"> 
<asp:TextBox SkinID="tbGray" ID="txtCompanyAddress2" runat="server"  Width="100%" 
        Font-Size="8pt" MaxLength="100" ></asp:TextBox>
</td>
<td style="width:5%; vertical-align:middle; height:27px;" align="left"></td>
</tr>
<tr style="height:35px;">
<td style="width:14%; vertical-align:middle;" align="left">
<asp:Label ID="Label3" runat="server" Font-Size="8pt">URL</asp:Label>
</td>
<td style="width:17%; vertical-align:middle;" align="left"> 
<asp:TextBox SkinID="tbGray" ID="txtUrl" runat="server"  Width="100%" Font-Size="8pt" 
        Enabled="true" MaxLength="50" ></asp:TextBox>
</td>
<td style="width:5%; vertical-align:middle; height:27px;" align="left"></td>
<td style="width:14%; vertical-align:middle;" align="left">
<asp:Label ID="Label2" runat="server" Font-Size="8pt">Phone</asp:Label>
</td>
<td style="width:17%; vertical-align:middle;" align="left"> <asp:TextBox SkinID="tbGray" ID="txtPhone" runat="server"  Width="100%" Font-Size="8pt" 
        Enabled="true" MaxLength="20" ></asp:TextBox>
</td>
<td style="width:5%; vertical-align:middle; height:27px;" align="left"></td>
<td style="width:14%; vertical-align:middle;" align="left">
<asp:Label ID="lblFax" runat="server" Font-Size="8pt">Fax</asp:Label>
</td>
<td style="width:17%; vertical-align:middle;" align="left"> <asp:TextBox SkinID="tbGray" ID="txtFax" runat="server"  Width="95%" Font-Size="8pt" 
        Enabled="true" MaxLength="20" ></asp:TextBox>
</td>
<td style="width:5%; vertical-align:middle; height:27px;" align="left"></td>
</tr>
<tr style="height:35px;">
<td style="width:14%; vertical-align:middle;" align="left">
<asp:Label ID="Label5" runat="server" Font-Size="8pt">Retained Earning</asp:Label>
</td>
<td style="width:17%; vertical-align:middle;" align="left"> <asp:TextBox SkinID="tbGray" ID="txtRetdEarnAcc" runat="server"  
        Width="100%" Font-Size="8pt" Enabled="true" MaxLength="7" ></asp:TextBox></td>
<td style="width:5%; vertical-align:middle; height:27px;" align="left"></td>
<td style="width:14%; vertical-align:middle;" align="left">
<asp:Label ID="Label6" runat="server" Font-Size="8pt">Bank Book Code</asp:Label>
</td>
<td style="width:17%; vertical-align:middle;" align="left"> 
    <asp:TextBox SkinID="tbGray" ID="txtBankCode" runat="server"  Width="100%" Font-Size="8pt" 
        Enabled="true" MaxLength="7" ></asp:TextBox>
</td>
<td style="width:5%; vertical-align:middle; height:27px;" align="left"></td>
<td style="width:14%; vertical-align:middle;" align="left">
<asp:Label ID="Label7" runat="server" Font-Size="8pt">Cash Book Code</asp:Label>
</td>
<td style="width:17%; vertical-align:middle;" align="left"> 
    <asp:TextBox SkinID="tbGray" ID="txtCashCode" runat="server"  Width="95%" Font-Size="8pt" 
        Enabled="true" MaxLength="7" ></asp:TextBox>
</td>
</tr>
<tr style="height:35px;">
<td style="width:14%; vertical-align:middle;" align="left">
<asp:Label ID="Label8" runat="server" Font-Size="8pt">Tax Reg No.</asp:Label>
</td>
<td style="width:17%; vertical-align:middle;" align="left"> <asp:TextBox SkinID="tbGray" ID="txtTaxNo" runat="server"  
        Width="100%" Font-Size="8pt" Enabled="true" MaxLength="20" ></asp:TextBox></td>

</tr>
</table>
<br />
<asp:Label ID="lblTranStatus" runat="server" Font-Size="8pt" Text="" Visible="false"></asp:Label>
<br />
<asp:GridView  RowStyle-Height="25px" CssClass="mGrid" PagerStyle-CssClass="pgr"  AlternatingRowStyle-CssClass="alt" ID="dgGlBook" runat="server" AutoGenerateColumns="false" 
        AllowPaging="True" Width="100%" BackColor="White" BorderWidth="1px" BorderStyle="Solid"
        CellPadding="2" CellSpaAMBg="0" BorderColor="LightGray" Font-Size="8pt" 
        AllowSorting="true" PageSize="5" ForeColor="#333333" 
        onselectedindexchanged="dgGlBook_SelectedIndexChanged"  >
  <Columns>
  <asp:CommandField ShowSelectButton="True"  ItemStyle-Width="40px" ItemStyle-HorizontalAlign="Center" ItemStyle-ForeColor="Blue" ItemStyle-Height="25px"/>
  <asp:BoundField  HeaderText="Book Name" DataField="book_name" ItemStyle-Width="90px" ItemStyle-HorizontalAlign="Center"/>
  <asp:BoundField  HeaderText="Description" DataField="book_desc" ItemStyle-Width="200px" ItemStyle-HorizontalAlign="Left"/>
  <asp:BoundField  HeaderText="Address" DataField="company_address1" ItemStyle-Width="200px" ItemStyle-HorizontalAlign="Left"/>
  <asp:BoundField  HeaderText="Status" DataField="book_status" ItemStyle-Width="50px" ItemStyle-HorizontalAlign="Center"/>  
  </Columns>
                        <RowStyle BackColor="white" />
                        <EditRowStyle BackColor="" />
                        <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                        <HeaderStyle BackColor="Silver" Font-Bold="True" ForeColor="Black" />
                        <AlternatingRowStyle BackColor="#F5F5F5" />
  </asp:GridView>

</td>
<td style="width:1%;"></td>
</tr></table>   
</div>
</asp:Content>

