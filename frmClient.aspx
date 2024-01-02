<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="frmClient.aspx.cs" Inherits="frmClient" Title="Customer Setup" Theme="Themes" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">

    <script type="text/javascript">
    function SetImageNID() {
        document.getElementById('<% =lbImgUpload.ClientID %>').click();
    }
    function SetImage() {
        document.getElementById('<% =imgBtnsup.ClientID %>').click();
    }
</script>

<div id="frmMainDiv" style="background-color:White; width:100%;">
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
<asp:TextBox SkinId="tbPlain"  ID="txtFax" runat="server"  Width="20px" 
        Font-Size="8pt" MaxLength="20" CssClass="tbl" Visible="False"></asp:TextBox>
</div>
<table style="width:100%;">
<tr>
<td style="width:1%;" align="center"> </td>
<td style="width:98%;" align="center"> 
<fieldset style="vertical-align: top; border: solid 1px #8BB381; text-align:left;"><legend style="color: maroon;"><b>Customer/Buyer Information </b></legend>
<table style="width:100%;">
<tr>
<td style="width:16%;" align="left">
<asp:Label ID="lblCid" runat="server" Font-Size="8pt">Code</asp:Label>
</td>
<td style="width:25%;" align="left"> 
<asp:TextBox SkinId="tbPlain"  ID="txtClientId" runat="server"  Width="80%" 
        Font-Size="8pt" MaxLength="4" CssClass="tbc" Enabled="false"></asp:TextBox>
</td>
<td style=" width:8%;" align="left" >
    <asp:CheckBox ID="CheckBox1" Checked="true" runat="server" Text="Active?" />
    </td>
<td align="left">
    <asp:CheckBox ID="CheckBox2"  runat="server" 
        Text="Common Customer?" Visible="False" />
    Segment Code</td>
    <td> 
<asp:TextBox SkinID="tbPlain" ID="txtGlCoa" MaxLength="7" style="text-align:left;" 
        runat="server"  Width="40%" 
        Font-Size="9pt"  CssClass="tbc"></asp:TextBox>
<asp:Label ID="lblGlCoa" runat="server" Font-Size="8pt" Visible="False"></asp:Label>
<asp:Label ID="lbLId" runat="server" Font-Size="8pt" Visible="False"></asp:Label>
                            </td>
    
    <td> 
        <asp:FileUpload ID="imgUploadNID" runat="server" Font-Size="8pt" Height="20px" 
            onchange="javascript:SetImageNID();" Size="15" Visible="true" />
        <asp:Button ID="lbImgUpload" runat="server" Font-Size="8pt" Height="20px" 
            onclick="lbImgUpload_Click" style="display:none;" Text="Upload" Width="50px" />
                            
&nbsp;<span style="color: #0033CC">&nbsp; NID Images</span></td>
    
</tr>
<tr>
<td style="width:16%; font-weight: 700; height: 29px;" align="left">
<asp:Label ID="lblCaddress" runat="server" Font-Size="8pt">Customer/Buyer Name</asp:Label>
</td>
<td style="height: 29px;" align="left" colspan="4"> 
<asp:TextBox SkinId="tbPlain"  ID="txtClientName" runat="server" Width="93%" 
        Font-Size="8pt" ></asp:TextBox>
</td>
<td  style="width:25%;" align="left" rowspan="4" valign="top"> 
    <asp:Image ID="imgSupNID" runat="server" BackColor="#EFF3FB" 
        BorderStyle="Solid" BorderWidth="1px" Height="100px" Width="100%" />
    </td>
</tr>
<tr>
<td style="width:16%; height: 30px;" align="left">
<asp:Label ID="Label15" runat="server" Font-Size="8pt">Office Address </asp:Label>
</td>
<td  style="height: 30px;" align="left" colspan="4">
<asp:TextBox SkinId="tbPlain"  ID="txtAddress1" runat="server" Width="93%" 
        Font-Size="8pt" ></asp:TextBox>
</td>
<td  style="width:25%; height: 30px;" align="left"> 
    </td>
</tr>
<tr>
<td style="width:16%; height: 30px;" align="left">
<asp:Label ID="Label16" runat="server" Font-Size="8pt">Residential Address</asp:Label>
</td>
<td style="height: 30px;" align="left" colspan="4">
<asp:TextBox SkinId="tbPlain"  ID="txtAddress2" runat="server" Width="93%" 
        Font-Size="8pt" ></asp:TextBox>
</td>
<td  style="width:25%; height: 30px;" align="left"> 
    </td>
</tr>
<tr>
<td style="width:16%; font-weight: 700; height: 27px;" align="left">
<asp:Label ID="lblCname" runat="server" Font-Size="8pt">National /Any Valid ID</asp:Label>
</td>
<td style="width:25%; height: 27px;" align="left">
<asp:TextBox SkinId="tbPlain"  ID="txtNationalId" runat="server" Width="79.5%" 
        Font-Size="8pt" MaxLength="50" CssClass="tbl"></asp:TextBox>
</td>
<td style=" width:8%; height: 27px;" > 
    </td>
<td style="width:13%; height: 27px;" align="left">
<asp:Label ID="lblSpId0" runat="server" Font-Size="8pt">Postal Code</asp:Label>
    </td>
<td  style="width:25%; height: 27px;" align="left"> 
<asp:TextBox SkinId="tbPlain"  ID="txtPostalCode" runat="server" Width="80%" 
        Font-Size="8pt" MaxLength="20"></asp:TextBox>
    </td>
<td  style="width:25%; height: 27px;" align="left"> 
    </td>
</tr>
<tr>
<td style="width:16%; height: 30px;" align="left">
    City</td>
<td style="width:25%; height: 30px;" align="left"> 
<asp:TextBox SkinId="tbPlain"  ID="txtCity" runat="server" Width="79.5%" 
        Font-Size="8pt" MaxLength="50" CssClass="tbl"></asp:TextBox>
    </td>
<td style=" width:8%; height: 30px;" ></td>
<td style="width:13%; height: 30px;" align="left">
<asp:Label ID="lblSupAddr8" runat="server" Font-Size="9pt">Country</asp:Label>
</td>
<td style="width:25%; height: 30px;" align="left" > 
    <asp:DropDownList ID="ddlCountry" runat="server" Height="26px" Width="81.5%">
    </asp:DropDownList>
    </td>
<td style="width:25%; height: 30px;" align="left" > 
    <asp:FileUpload ID="imgUploadsup" runat="server" Font-Size="8pt" Height="20px" 
        onchange="javascript:SetImage();" Size="15" Visible="true" />
    <asp:Button ID="imgBtnsup" runat="server" Font-Size="8pt" Height="20px" 
        onclick="imgBtnsup_Click" style="display:none;" Text="Upload" Width="50px" />
    &nbsp;&nbsp; </td>
</tr>
<tr>
<td style="width:16%;" align="left">
    Company Name</td>
<td style="width:25%;" align="left"> 
<asp:TextBox SkinId="tbPlain"  ID="txtState" runat="server" Width="79.5%" 
        Font-Size="8pt" MaxLength="50" CssClass="tbl"></asp:TextBox></td>
<td style=" width:8%;" ></td>
<td style="width:13%;" align="left">
<asp:Label ID="Label13" runat="server" Font-Size="8pt">Email</asp:Label>
</td>
<td style="width:25%;" align="left" > 
<asp:TextBox SkinId="tbPlain"  ID="txtEmail" runat="server" Width="79.5%" 
        Font-Size="8pt"  CssClass="tbc"></asp:TextBox></td>
<td style="width:25%;" align="left" rowspan="4" > 
    <asp:Image ID="imgSup" runat="server" BackColor="#EFF3FB" BorderStyle="Solid" 
        BorderWidth="1px" Height="100px" Width="100%" />
    </td>
</tr>
<tr>
<td style="width:16%;" align="left">
<asp:Label ID="Label11" runat="server" Font-Size="8pt">Mobile</asp:Label>
</td>
<td style="width:25%;" align="left"> 
<asp:TextBox SkinId="tbPlain"  ID="txtMobile" runat="server" Width="80%" 
        Font-Size="8pt" MaxLength="20" CssClass="tbc"></asp:TextBox>
</td>
<td style=" width:8%;" >&nbsp;</td>
<td style="width:13%;" align="left">
<asp:Label ID="Label10" runat="server" Font-Size="8pt">Phone</asp:Label>
</td>
<td style="width:25%;" align="left" > 
<asp:TextBox SkinId="tbPlain"  ID="txtPhone" runat="server"  Width="79.5%" 
        Font-Size="8pt" MaxLength="20" CssClass="tbl"></asp:TextBox>
    </td>
<td style="width:25%;" align="left" > 
    &nbsp;</td>
</tr>
<tr>
<td style="width:16%;" align="left">
    &nbsp;</td>
<td style="width:25%;" align="left"> 
    &nbsp;</td>
<td style=" width:8%;" >&nbsp;</td>
<td style="width:13%;" align="left">
    &nbsp;</td>
<td style="width:25%;" align="left" > 
<asp:TextBox SkinId="tbPlain"  ID="txtPessoRate" runat="server"  Width="79.5%" 
        style="text-align: center" placeHolder="0.00"
        Font-Size="8pt" MaxLength="20" CssClass="tbl" Visible="False"></asp:TextBox>
    </td>
<td style="width:25%;" align="left" > 
    &nbsp;</td>
</tr>
<tr>
<td style="width:16%;" align="left">
    &nbsp;</td>
<td style="width:25%;" align="left"> 
    &nbsp;</td>
<td style=" width:8%;" >&nbsp;</td>
<td style="width:13%;" align="left">
    &nbsp;</td>
<td style="width:25%;" align="left" > 
    &nbsp;</td>
<td style="width:25%;" align="left" > 
    &nbsp;</td>
</tr>
</table>
</fieldset>
<fieldset style="vertical-align: top; border: solid 1px #8BB381;text-align:left;"><legend style="color: maroon;"><b>Search Option</b> </legend>
                <table style="width: 100%;">
                        <tr>
                            <td style="width: 24%; text-align: center; font-weight: 700;">Search By Customer/Buyer Name :
                            </td>
                            <td>
                                <asp:UpdatePanel ID="UPSearch" runat="server" UpdateMode="Conditional"><ContentTemplate>
                                <asp:HiddenField ID="hfCustomerID" runat="server" />
                                    <asp:TextBox ID="txtCustomer" runat="server" AutoPostBack="True" Height="22px" 
                                        ontextchanged="txtCustomer_TextChanged" placeHolder="Search Customer..." 
                                        style="text-align:left;" TabIndex="11" Width="90%"></asp:TextBox>
                                    <ajaxToolkit:AutoCompleteExtender ID="AutoCompleteExtender2" runat="server" 
                                        CompletionInterval="20" CompletionSetCount="12" EnableCaching="true" 
                                        MinimumPrefixLength="1" ServiceMethod="GetCustomername" 
                                        ServicePath="AutoComplete.asmx" TargetControlID="txtCustomer" />
            </ContentTemplate></asp:UpdatePanel>
                                    </td>
                            <td style="width: 15%; text-align: center">
                                <asp:LinkButton ID="lbSearch" runat="server" Font-Bold="True" Font-Size="12pt" OnClick="lbSearch_Click" Width="90%" BorderStyle="Solid">Search</asp:LinkButton>
                            </td>
                            <td style="width: 20%;text-align: center">
                                <table style="width: 100%;">
                                    <tr>
                                        <td style="width: 50%;text-align: center;">
                                            <asp:LinkButton ID="lbClear" runat="server" Font-Bold="True" Font-Size="12pt" OnClick="lbClear_Click" Width="90%" BorderStyle="Solid">Clear</asp:LinkButton>
                                        </td>
                                        <td style="width: 50%;">&nbsp;</td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        </table>
                       
                        </fieldset>
<br />
<asp:GridView CssClass="mGrid" PagerStyle-CssClass="pgr"  AlternatingRowStyle-CssClass="alt" 
ID="dgClient" runat="server" AutoGenerateColumns="False" Width="100%" 
        AllowPaging="True" BackColor="White" BorderWidth="1px" BorderStyle="Solid"
        CellPadding="2" BorderColor="LightGray" Font-Size="9pt" 
        AllowSorting="True" PageSize="50" 
        onselectedindexchanged="dgClient_SelectedIndexChanged" ForeColor="#333333" 
        onpageindexchanging="dgClient_PageIndexChanging" 
        onrowdatabound="dgClient_RowDataBound"  >
  <HeaderStyle Font-Size="9pt"  Font-Bold="True" HorizontalAlign="center" BackColor="Silver"/>
  <FooterStyle BackColor="Silver" Font-Bold="True" ForeColor="White" />
  <Columns>
  <asp:CommandField ShowSelectButton="True"  ItemStyle-Width="40px" 
          ItemStyle-HorizontalAlign="Center" ItemStyle-ForeColor="Blue">
<ItemStyle HorizontalAlign="Center" ForeColor="Blue" Width="40px"></ItemStyle>
      </asp:CommandField>
  <asp:BoundField  HeaderText="Customer/Buyer Code" DataField="Code" 
          ItemStyle-Width="80px" ItemStyle-HorizontalAlign="Center">
<ItemStyle HorizontalAlign="Center" Width="80px"></ItemStyle>
      </asp:BoundField>
  <asp:BoundField  HeaderText="Customer/Buyer Name" DataField="ContactName" ItemStyle-Width="200px" 
          ItemStyle-HorizontalAlign="Left">
<ItemStyle HorizontalAlign="Left" Width="200px"></ItemStyle>
      </asp:BoundField>
  <asp:BoundField  HeaderText="Address" DataField="Address1" ItemStyle-Width="200px" 
          ItemStyle-HorizontalAlign="Left">  
<ItemStyle HorizontalAlign="Left" Width="200px"></ItemStyle>
      </asp:BoundField>
  <asp:BoundField  HeaderText="Mobile" DataField="Mobile" ItemStyle-Width="150px" 
          ItemStyle-HorizontalAlign="Left">
<ItemStyle HorizontalAlign="Left" Width="150px"></ItemStyle>
      </asp:BoundField>
      <asp:BoundField DataField="ID" HeaderText="ID"  ItemStyle-Width="150px" 
          ItemStyle-HorizontalAlign="Left">
<ItemStyle HorizontalAlign="Left" Width="150px"></ItemStyle>
      </asp:BoundField>
      <asp:BoundField  HeaderText="Country" DataField="COUNTRY_DESC" ItemStyle-Width="100px" 
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
<td style="width:1%;" align="center"> </td>
</tr>
</table>
</div>

</asp:Content>

