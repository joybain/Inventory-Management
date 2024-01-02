<%@ Page Title="Item's Setting.-RD" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="ItemsInformation.aspx.cs" Inherits="ItemsInformation" EnableEventValidation="true" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxtoolkit" %>
<%@ Import Namespace="System.Data" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">

    <script type="text/javascript">
    function SetImage() {
        document.getElementById('<% =lbImgUpload.ClientID %>').click();
    }

    function OpStockCal() {
        alert(txtUnitPrice.value);
        var txtUnitPrice = document.getElementById("<%=txtUnitPrice.ClientID %>");
        var txtOpeningStock = document.getElementById("<%=txtOpeningStock.ClientID %>");
        var txtOpeningAmount = document.getElementById("<%=txtOpeningAmount.ClientID %>");
        txtOpeningAmount.value = (txtUnitPrice.value * txtOpeningStock.value);
    }

</script>
<script language="javascript" type="text/javascript" >
    function setDecimal(abc) {
        var dt = document.getElementById(abc).value;
        if (dt.length > 0) {
            document.getElementById(abc).value = parseFloat(dt).toFixed(2);
        }
    }
    function isNumber(evt) {
        evt = (evt) ? evt : window.event;
        var charCode = (evt.which) ? evt.which : evt.keyCode;
        if (charCode != 46 && charCode > 31
            && (charCode < 48 || charCode > 57)) {
            return false;
        }
        return true;
    }

    onblur = function () {
        setTimeout('self.focus()', 100);
    }

    function SetImage1() {
        document.getElementById('<%=btnUpload.ClientID %>').click();
    }

</script>
<div id="frmMainDiv" style="background-color:White; width:100%;">
<table  id="pageFooterWrapper">
 <tr>
     <td align="center">
        &nbsp;</td>
	<td align="center">
	    <asp:Button  ID="BtnDelete" runat="server"  ToolTip="Delete Record"   
            OnClick="BtnDelete_Click"  
            onclientclick="javascript:return window.confirm('are u really want to delete  these data')" Text="Delete" 
        Height="35px" Width="100px" BorderStyle="Outset"  />
	 </td>
     <td align="center">
        &nbsp;</td>
	<td align="center">
    
	<asp:Button  ID="BtnFind" runat="server"  ToolTip="Find"  
            OnClick="BtnFind_Click"  Text="Find" 
        Height="35px" Width="100px" BorderStyle="Outset" Visible="False"  />
	</td>
    <td align="center">
        &nbsp;</td>
	
	<td align="center">
        <asp:Button ID="BtnSave" runat="server" ToolTip="Save or Update Record" 
            OnClick="BtnSave_Click" Text="Save"  
        Height="35px" Width="100px" BorderStyle="Outset"  /></td>
        <td align="center">
        &nbsp;</td>
	<td align="center">
        <asp:Button ID="BtnReset" runat="server" ToolTip="Clear Form" 
            OnClick="BtnReset_Click" Text="Clear" 
        Height="35px" Width="100px" BorderStyle="Outset"  /></td>
        <td align="center">
        &nbsp;</td>
    <td align="center">
       <asp:Button ID="btnPrint" runat="server" ToolTip="Print" 
            onclick="btnPrint_Click" Text="Print" 
        Height="35px" Width="100px" BorderStyle="Outset" Visible="False"  />
   </td>   
   <td align="center">
        &nbsp;</td>     
	</tr>		
</table>

    
    <table style="width: 100%">
        <tr>
            <td style="width: 26px">
                &nbsp;</td>
            <td style="width: 1166px">
                <asp:Label ID="lblBrandID" runat="server" Visible="False"></asp:Label>
            </td>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td style="width: 26px">
                &nbsp;</td>
            <td style="width: 1166px">     
        <ajaxtoolkit:TabContainer ID="tabVch" runat="server" Width="100%" ActiveTabIndex="0" 
                    Font-Size="8pt">
<ajaxtoolkit:TabPanel ID="tpVchDtl" runat="server"  HeaderText="Items Information">
    <ContentTemplate>
              <table style="width:100%;">
                     <tr>
            <td align="right" style="width: 101px; height: 27px;">
                <asp:Label ID="Label3" runat="server" Text="Code" style="font-weight: 700"></asp:Label>
            </td>
            <td style="width: 13px; height: 27px;"></td>
            <td style="width: 185px; height: 27px;" align="left">
                <asp:TextBox ID="txtCode" runat="server" Width="150px" Enabled="False"></asp:TextBox>
            </td>
            <td colspan="1" style="width: 156px; height: 27px;">
                <asp:Label ID="Label23" runat="server" Text="Department"></asp:Label>
                <asp:CheckBox ID="CheckBox0" runat="server" Checked="True" Text="Active?" 
                    Visible="False" />
            </td>
             <td colspan="1" style="height: 27px">
                 <asp:DropDownList ID="ddlDepart" runat="server" 
                     Width="102%" onselectedindexchanged="ddlDepart_SelectedIndexChanged" 
                     AutoPostBack="True">
                 </asp:DropDownList>
                         </td>
            <td align="center" style="height: 27px">
                <asp:Label ID="lblID" runat="server" Visible="False"></asp:Label>
<asp:FileUpload ID="imgUpload" runat="server" Size="15" Height="20px"  
                    onchange="javascript:SetImage();" Font-Size="8pt"/>
<asp:Button ID="lbImgUpload" runat="server" Text="Upload" Font-Size="8pt" Width="50px" Height="20px" onclick="lbImgUpload_Click" style="display:none;"></asp:Button>
            </td>
        </tr>
        <tr>
            <td align="right" style="width: 101px; height: 25px;">
                <asp:Label ID="Label4" runat="server" Text="Name" style="font-weight: 700"></asp:Label>
            </td>
            <td style="width: 13px; height: 25px;"></td>
            <td colspan="3" style="height: 25px">
                <table style="width: 100%;">
                    <tr>
                        <td style="width: 80%;" align="left" valign="middle">
                <asp:HiddenField ID="hfItemSetupID" runat="server" />
                <asp:TextBox ID="txtName" placeHolder="Search items...!!" runat="server" CssClass="txtVisibleAlign"
                    Width="98%" AutoPostBack="True" ontextchanged="txtName_TextChanged" 
                    Height="18px"></asp:TextBox>    
                <ajaxToolkit:AutoCompleteExtender ID="AutoCompleteExtender1" runat="server" 
                                                  CompletionInterval="5" CompletionSetCount="5" 
                                                  MinimumPrefixLength="1" ServiceMethod="GetItemsSetupInfoSearch" 
                                                  ServicePath="AutoComplete.asmx" 
                    TargetControlID="txtName" DelimiterCharacters="" Enabled="True" />
                
               

                        </td>
                        <td style="width: 80%;" align="center" valign="middle">
                            <asp:LinkButton ID="lbItemLink" runat="server" Font-Bold="True" 
                                onclick="lbItemLink_Click" BorderStyle="Solid" ToolTip="Create Items">..........</asp:LinkButton>
                        </td>
                    </tr>
                </table>
            </td>
            <td align="center" rowspan="8" valign="top">
    <asp:Image ID="imgEmp" runat="server" Height="150px" BorderStyle="Solid" 
                    BackColor="#EFF3FB" BorderWidth="1px"  />  
            </td>
        </tr>
        <tr>
            <td align="right" style="width: 101px; height: 27px;">
                <asp:Label ID="Label21" runat="server" Text="Short Name"></asp:Label>
            </td>
            <td style="width: 13px; height: 27px;"></td>
            <td style="width: 185px; height: 27px;">
                <asp:TextBox ID="txtStName" runat="server" Width="95%" placeholder="Short Name"></asp:TextBox>
            </td>
            <td align="right" style="width: 156px; height: 27px;">
                Style/Model/Serial No.</td>
            <td style="width: 232px; height: 27px;">
                <asp:TextBox ID="txtStyleNo" runat="server" Width="95%"></asp:TextBox>  
            </td>
        </tr>
        <tr>
            <td align="right" style="width: 101px; height: 30px;">
                <asp:Label ID="Label20" runat="server" Text="Brand"></asp:Label>
            </td>
            <td style="width: 13px; height: 30px;"></td>
            <td style="width: 185px; height: 30px;">
             <asp:UpdatePanel ID="UPBrand" runat="server" UpdateMode="Conditional">    
                 <ContentTemplate>
                <asp:TextBox ID="txtBrand" runat="server" Width="95%" placeHolder="Search Brand.." CssClass="txtVisibleAlign"
                     AutoPostBack="True" 
                    ontextchanged="txtBrand_TextChanged" Height="18px"></asp:TextBox>
                   <%-- <ajaxToolkit:AutoCompleteExtender ID="Brand" ServiceMethod="GetBrandInfo" ServicePath="AutoComplete.asmx" TargetControlID="txtBrand" runat=server/>
--%>
                     <ajaxToolkit:AutoCompleteExtender ID="txtSupplierSearch_AutoCompleteExtender" runat="server" 
            CompletionInterval="5" CompletionSetCount="5" EnableCaching="true" 
            MinimumPrefixLength="1" ServiceMethod="GetBrandInfo" 
            ServicePath="AutoComplete.asmx" TargetControlID="txtBrand" />
                    </ContentTemplate>
                    </asp:UpdatePanel>
            </td>
            <td align="right" style="width: 156px; height: 30px;">
                Size</td>
            <td style="width: 232px; height: 30px;">
                <asp:DropDownList ID="ddlSize" runat="server" Height="26px" Width="100%">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td align="right" style="width: 101px; height: 32px;">
                <asp:Label ID="Label5" runat="server" Text="UOM"></asp:Label>
            </td>
            <td style="width: 13px; height: 32px;"></td>
            <td style="width: 185px; height: 32px;">
                <asp:DropDownList ID="ddlUmo" runat="server" Width="100%" Height="26px">
                </asp:DropDownList>
            </td>
            <td align="right" style="width: 156px; height: 32px;">
                <asp:Label ID="Label11" runat="server" Text="Unit Price"></asp:Label>
            </td>
            <td style="width: 232px; height: 32px;">
                <asp:TextBox ID="txtUnitPrice" runat="server" Width="45%" style="text-align:right;"  onkeypress="return isNumber(event)" onfocus="this.select();"></asp:TextBox>
            &nbsp;<asp:DropDownList ID="ddlCurrency" runat="server" Width="35%" Height="26px">
                    <asp:ListItem>RM</asp:ListItem>
            <asp:ListItem>BDT</asp:ListItem>
                    <asp:ListItem>USD</asp:ListItem>
                    <asp:ListItem>EUR</asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td align="right" style="width: 101px; height: 29px;">
                <asp:Label ID="Label8" runat="server" Text="Category"></asp:Label>
            </td>
            <td style="width: 13px; height: 29px;">
            </td>
            <td style="width: 185px; height: 29px;" align="left">
                <asp:UpdatePanel ID="UPanel1Cat" runat="server" UpdateMode="Conditional">    
                 <ContentTemplate>      
                <asp:DropDownList ID="ddlCatagory" runat="server" AutoPostBack="True" 
                    Height="26px" onselectedindexchanged="ddlCatagory_SelectedIndexChanged" 
                    Width="100%">
                </asp:DropDownList>
                 </ContentTemplate>
            </asp:UpdatePanel>
            </td>
            <td align="right" style="width: 156px; height: 29px;">
                <asp:Label ID="Label15" runat="server" Text="Sub Category"></asp:Label>
            </td>
            <td style="width: 232px; height: 29px;">
                <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">    
                 <ContentTemplate>
                <asp:DropDownList ID="ddlSubCatagory" runat="server" Height="26px" Width="100%">
                </asp:DropDownList>
                </ContentTemplate>
            </asp:UpdatePanel>
            </td>
        </tr>
                     <tr>
                         <td align="right" style="width: 101px; height: 29px;">
                             <asp:Label ID="Label24" runat="server" Text="Warranty Year"></asp:Label>
                         </td>
                         <td style="width: 13px; height: 29px;">
                             &nbsp;</td>
                         <td align="left" style="width: 185px; height: 29px;">
                             <asp:DropDownList ID="ddlWarrantyYear" runat="server" Height="26px" 
                                 Width="100%">
                             </asp:DropDownList>
                         </td>
                         <td align="right" style="width: 156px; height: 29px;">
                             <asp:Label ID="Label28" runat="server" Text="Warranty Month"></asp:Label>
                         </td>
                         <td style="width: 232px; height: 29px;">
                             <asp:DropDownList ID="ddlWarrantyMonth" runat="server" Height="26px" 
                                 Width="100%">
                             </asp:DropDownList>
                         </td>
                     </tr>
        <tr>
            <td align="right" style="width: 101px; height: 34px;">
                <asp:Label ID="Label16" runat="server" Text="Tax Category"></asp:Label>
            </td>
            <td style="width: 13px; height: 34px;"></td>
            <td style="width: 185px; height: 34px;" align="left">  
                        
                <asp:DropDownList ID="ddlTextCatagory" runat="server" Height="26px" 
                    Width="100%">
                </asp:DropDownList>
                            
            </td>
            <td align="right" style="width: 156px; height: 34px;">
                <asp:CheckBox ID="CheckBox2" runat="server" AutoPostBack="True" 
                    OnCheckedChanged="CheckBox2_CheckedChanged" Text="Discounted?" />
            </td>
            <td style="width: 232px; height: 34px;">    
                            
                &nbsp;<asp:TextBox ID="txtDiscountAmount" runat="server" style="text-align:right;" 
                    Width="100px"></asp:TextBox>
                <asp:Label ID="Label17" runat="server" Font-Bold="True" ForeColor="#CC3300" 
                    Text="%"></asp:Label>
                            
            </td>
        </tr>
                     <tr>
                         <td align="right" style="width: 101px;">
                             <asp:Label ID="Label19" runat="server" Text="Description"></asp:Label>
                         </td>
                         <td style="width: 13px; height: 34px;">
                             &nbsp;</td>
                         <td align="left" colspan="3">
                             <asp:TextBox ID="txtDescription" runat="server" Width="98%"></asp:TextBox>
                         </td>
                     </tr>
                </table>
    </ContentTemplate>
</ajaxtoolkit:TabPanel>                                            
<ajaxtoolkit:TabPanel ID="tpVchHist" runat="server" HeaderText="Upload Image">
    <ContentTemplate>
        <table style="width: 100%">           
            <tr>
                <td align="center">
                    &nbsp;</td>
            </tr>
            <tr>
                <td align="center">
                    <asp:FileUpload ID="FileUpload1" runat="server" Font-Size="8pt" Height="25px" 
                        onchange="javascript:SetImage1();" Size="20%" BackColor="#CCCCCC" 
                        Width="300px" />
                    <asp:Button ID="btnUpload" runat="server" OnClick="btnUpload_Click" 
                        style="display:none;" Text="Update" Width="120px" />
                </td>
            </tr>
            <tr>
                <td align="center" style="vertical-align: top;">
                    <asp:Panel runat="server" ID="ScrollBer" ScrollBars="Both" Height="500px" Width="400px">
                    <asp:GridView ID="dgImage" runat="server" AutoGenerateColumns="False" 
                        CssClass="mGrid" OnRowDataBound="dgImage_RowDataBound" 
                        OnRowDeleting="dgImage_RowDeleting" PageSize="2" Width="40%">
                        <Columns>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:ImageButton ID="lbDelete" runat="server" CausesValidation="False" 
                                        CommandName="Delete" ImageAlign="Middle" ImageUrl="~/img/delete.png" 
                                        Text="Delete" />
                                </ItemTemplate>
                                <ItemStyle Font-Size="8pt" HorizontalAlign="Center" Width="2%" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Product Image">
                                <ItemTemplate>
                                    <asp:Image ID="Image1" runat="server" Height="150px" 
                                        ImageUrl='<%# "HTTPHandlerImageItems.ashx?ImID="+ Eval("ImageID") %>' Width="150px" />
                                </ItemTemplate>
                                <ItemStyle Font-Size="8pt" HorizontalAlign="Center" Width="4%" />
                            </asp:TemplateField>
                            <asp:BoundField DataField="ImageID" HeaderText="ID" />
                        </Columns>
                    </asp:GridView>
                    </asp:Panel>
                </td>
            </tr>
            <tr>
                <td align="left" style="vertical-align:top;">
                    &nbsp;</td>
            </tr>
        </table>
    </ContentTemplate>
</ajaxtoolkit:TabPanel>
</ajaxtoolkit:TabContainer>

            </td>
            <td>
                <asp:UpdatePanel ID="UPOPStock" style="width: 10px" runat="server" UpdateMode="Conditional">    
                 <ContentTemplate>
                <asp:TextBox ID="txtOpeningStock" runat="server" Width="10px"  onkeypress="return isNumber(event)"
                     onfocus="this.select();"
                    style="text-align:right;" AutoPostBack="True" 
                    ontextchanged="txtOpeningStock_TextChanged" Visible="False" ></asp:TextBox>
                  </ContentTemplate></asp:UpdatePanel>     
                   <asp:UpdatePanel ID="UPOPAmount" style="width: 10px" runat="server" UpdateMode="Conditional">    
                 <ContentTemplate>       
                <asp:TextBox ID="txtOpeningAmount" runat="server" Width="10px" 
                    style="text-align:right;" Visible="False"></asp:TextBox>  
                    </ContentTemplate>
                    </asp:UpdatePanel>   
                    <asp:TextBox ID="txtClosingStock" runat="server" Width="10px"  onkeypress="return isNumber(event)"
                    style="text-align:right;" Enabled="False" Visible="False"></asp:TextBox>
                <asp:TextBox ID="txtClosingAmount" runat="server" Width="10px" 
                    style="text-align:right;" Enabled="False" Visible="False"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td style="width: 26px; height: 4px;">
                &nbsp;</td>
            <td style="width: 1166px; height: 4px;">
                <fieldset style="vertical-align: top; border: solid 1px #8BB381;text-align:left;"><legend style="color: maroon;"><b>Search Items</b> </legend>
                <table style="width: 100%;">
                        <tr>
                            <td style="width: 24%; text-align: center; font-weight: 700;">Search By Code/Name/Brand :</td>
                            <td>
                                        <asp:TextBox ID="txtSearchItems" runat="server" placeHolder="Search By Code/Name/Brand.." Width="98%"></asp:TextBox>
                               <ajaxToolkit:AutoCompleteExtender ServicePath="AutoComplete.asmx" runat="server" ID="autoComplete1" TargetControlID="txtSearchItems"
                ServiceMethod="GetSearch_Items_On_Purchase" MinimumPrefixLength="1" CompletionInterval="20" EnableCaching="true" CompletionSetCount="12"/>
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
            </td>
            <td style="height: 4px">
                &nbsp;</td>
        </tr>
        <tr>
            <td style="width: 26px">
                &nbsp;</td>
            <td style="width: 1166px">
            
                <asp:GridView ID="dgHistory" runat="server" AllowPaging="True" CssClass="mGrid" 
                    onpageindexchanging="dgHistory_PageIndexChanging" 
                    onrowdatabound="dgHistory_RowDataBound" 
                    onselectedindexchanged="dgHistory_SelectedIndexChanged" PageSize="30" 
                    style="text-align:center;" Width="100%">
                    <Columns>
                        <asp:CommandField ShowSelectButton="True" />
                    </Columns>
                    <HeaderStyle Font-Bold="True" Height="25px" />
                </asp:GridView>
                 
            </td>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td style="width: 26px">
                &nbsp;</td>
            <td style="width: 1166px">
                &nbsp;</td>
            <td>
                &nbsp;</td>
        </tr>
    </table>

    
    </div>
</asp:Content>

