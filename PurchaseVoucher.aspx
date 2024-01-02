<%@ Page Title="Purchase Voucher.-RD" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="PurchaseVoucher.aspx.cs" Inherits="PurchaseVoucher" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <script src='<%= ResolveUrl("~/Scripts/valideDate.js") %>' type="text/javascript"></script>
<script language="javascript" type="text/javascript" >
    function OpenWindow(Url) {
        var testwindow = window.open(Url, '', 'width=600px,height=400px,top=100,left=300,scrollbars=1');
    }

    function setValueItem(item, iname, msr, rate) {
        $('input:text[id$=txtItemCode]').val(item);
        $('input:text[id$=txtQnty]').focus();

    }
        function remLink() {
            if (window.testwindow && window.testwindow.open && !window.testwindow.closed)
                window.testwindow.opener = null;
        }
        function IsEmpty(aTextField) {
            if ((aTextField.value.length == 0) || (aTextField.value == null)) {
                return true;
            }
            else {
                return false;
            }
        }
        function onListPopulated() {
            var completionList = $find("AutoCompleteEx").get_completionList();
            completionList.style.width = 'auto';
        }

        function changetextbox() {
            var valu = $('#<%=ddlPaymentMethord.ClientID%> option:selected').attr('value');
            if (valu === "C") {

                document.getElementById("<%=ddlBank.ClientID %>").disabled = true;
                document.getElementById("<%=txtChequeNo.ClientID %>").disabled = true;
                document.getElementById("<%=txtChequeDate.ClientID %>").disabled = true;
                document.getElementById("<%=txtChequeAmount.ClientID %>").disabled = true;
            }
            else {

                document.getElementById("<%=ddlBank.ClientID %>").disabled = false;
                document.getElementById("<%=txtChequeNo.ClientID %>").disabled = false;
                document.getElementById("<%=txtChequeDate.ClientID %>").disabled = false;
                document.getElementById("<%=txtChequeAmount.ClientID %>").disabled = false;

            }
        }
       
        function Remarks() {

            var txtRemarks = document.getElementById("<%=txtRemarks.ClientID %>");
            txtRemarks.focus();
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
         

        function LoadModalDiv() {

            var bcgDiv = document.getElementById("divBackground");
            bcgDiv.style.display = "block";           

        }
        function HideModalDiv() {

            var bcgDiv = document.getElementById("divBackground");
            bcgDiv.style.display = "none";
           
        }    
 </script>

<div id="frmMainDiv" style="background-color:White; width:100%;">  
<table  id="pageFooterWrapper">
  <tr>  
        <td align="center">
        <asp:Button ID="btnDelete" runat="server" ToolTip="Delete" onclick="Delete_Click"
                onclientclick="javascript:return window.confirm('are u really want to delete these data')" Text="Delete" 
        Height="35px" Width="110px" BorderStyle="Outset"  />
        </td>       
        <td >
            &nbsp;</td>       
        <td align="center" >
        <asp:Button ID="btnSave" runat="server" ToolTip="Save Purchase Record"   onclientclick="javascript:return window.confirm('Please check properly items and items expiration date then saved')"  Text="Save" 
                    Height="35px" Width="100px"
                onclick="btnSave_Click" 
       BorderStyle="Outset"  />
        </td>
        <td align="center" >
        <asp:Button ID="btnNew" runat="server" ToolTip="New" onclick="btnNew_Click"  Text="New" 
        Height="35px" Width="110px" BorderStyle="Outset"  /> 
        </td>
        <td align="center" >
        <asp:Button ID="btnClear" runat="server"  ToolTip="Clear" onclick="Clear_Click" Text="Clear" 
        Height="35px" Width="110px" BorderStyle="Outset"  />
        </td>
        <td align="center" >
        <asp:Button ID="btnPrint" runat="server" ToolTip="Print PO" Text="Print" 
        Height="35px" Width="110px" BorderStyle="Outset" onclick="btnPrint_Click"  />
        </td>   
       <td align="center" >
        <asp:Button ID="btnFind" runat="server" ToolTip="Find" Text="Find" 
        Height="35px" Width="110px" BorderStyle="Outset" onclick="btnFind_Click" 
               Visible="False" />
        </td>          
        
   </tr>
   </table>
   <%--<br />--%>
<table style="width:99%;">
<tr>
<td style="width:1%;">&nbsp;</td>
<td style="width:95%; " align="center">
        <asp:DropDownList ID="ddlParty" runat="server" AutoPostBack="True" 
            Font-Size="8pt" Height="26px" 
            onselectedindexchanged="ddlParty_SelectedIndexChanged" SkinId="ddlPlain" 
            TabIndex="7" Width="20px" Visible="False">
        </asp:DropDownList>
    
            <asp:CheckBox ID="chkAdvance" runat="server" Text="Use Advance" 
        Visible="False" />
        <asp:TextBox ID="txtShiftmentNo" runat="server" CssClass="tbc" Font-Size="8pt" 
            style="text-align:left" Placeholder="Search Shiftment No."
            SkinId="tbPlain" TabIndex="5" Width="20px" AutoPostBack="True" 
            ontextchanged="txtShiftmentNo_TextChanged" Visible="False"></asp:TextBox>
             <ajaxtoolkit:AutoCompleteExtender ID="AutoCompleteExtender2"
                                                        runat="server" 
            CompletionInterval="20" CompletionSetCount="30"
                                                        EnableCaching="true" MinimumPrefixLength="2"
                                                        
            ServiceMethod="GetShiftmentInfo" ServicePath="~/AutoComplete.asmx"
                                                        
            TargetControlID="txtShiftmentNo">
                                                    </ajaxtoolkit:AutoCompleteExtender>
    <asp:Label ID="lblShiftmentID" runat="server" Visible="False"></asp:Label>
    
</td>
<td style="width:1%;">&nbsp;</td>
</tr>
<tr>
<td style="width:1%;"></td>
<td style="width:95%; " align="center">
 <asp:UpdatePanel ID="PVI_UP" runat="server" UpdateMode="Conditional"> 
 <ContentTemplate>
<fieldset style="vertical-align: top; border: solid 1px #8BB381;text-align:left;"><legend style="color: maroon;"><b>Purchase Voucher</b> </legend>
<table id="Table1" style="width:100%"  cellpadding="2">
    <tr>
        <td align="left" style="width: 15%; height: 19px;">
            <asp:Label ID="lblPurNo" runat="server" Font-Size="9pt">Goods Receive No</asp:Label>
        </td>
        <td align="left" style="width: 29%; height: 19px;">
            <asp:TextBox ID="txtGRNO" runat="server" AutoPostBack="False" CssClass="tbc" 
                Font-Size="8pt" SkinId="tbPlain" TabIndex="0" Width="60%"></asp:TextBox>
        </td>
        <td style=" width:7%; height: 19px;">
            <asp:TextBox ID="txtID" runat="server" AutoPostBack="False" CssClass="tbc" 
                Font-Size="8pt" SkinId="tbPlain" Visible="False" Width="60%"></asp:TextBox>
        </td>
        <td align="left" style="width: 14%; height: 19px;">
            <asp:Label ID="lblPurDate" runat="server" Font-Size="9pt">Goods Receive Date</asp:Label>
        </td>
        <td align="left" style="width: 23%; height: 19px;">
            <asp:TextBox ID="txtGRNODate" runat="server" AutoPostBack="False" 
                Font-Size="8pt" placeholder="dd/MM/yyyy" SkinId="tbPlain" TabIndex="1" 
                Width="58%"></asp:TextBox>
            <cc:CalendarExtender ID="txtGRNODate_CalendarExtender" runat="server" 
                Format="dd/MM/yyyy" TargetControlID="txtGRNODate" />
        </td>
        <td style="height: 19px">
        </td>
    </tr>
    <tr >
	<td style="width: 15%; " align="left">
	<asp:Label ID="Label5" runat="server" Font-Size="9pt">Purchase Order No</asp:Label></td>
	<td style="width: 29%; " align="left">
    <asp:TextBox SkinId="tbPlain" ID="txtPO" runat="server" Width="100%" 
            CssClass="txtVisibleAlign" style="text-align: left;" 
            placeholder="Search By PO. No./PO.Code.." TabIndex="3"
            Font-Size="8pt" ontextchanged="txtPO_TextChanged" AutoPostBack="True" 
            Height="18px"></asp:TextBox>
            <ajaxtoolkit:AutoCompleteExtender ID="txtPO_AutoCompleteExtender"
                                                        runat="server" CompletionInterval="20" CompletionSetCount="30"
                                                        EnableCaching="true" MinimumPrefixLength="2"
                                                        ServiceMethod="GetShowPONoApprovedBy" ServicePath="~/AutoComplete.asmx"
                                                        TargetControlID="txtPO">
                                                    </ajaxtoolkit:AutoCompleteExtender>
            
        </td>
    <td style=" width:7%;">
        <asp:Label ID="lblOrNo" runat="server" Font-Size="9pt" Visible="False"></asp:Label>
        </td>
	<td style="width: 14%; " align="left">
	<asp:Label ID="Label9" runat="server" Font-Size="9pt"> Order Date</asp:Label></td>
	<td style="width: 23%; " align="left">
    <asp:TextBox SkinId="tbPlain" ID="txtPODate" runat="server" Width="58%"  
            placeholder="dd/MM/yyyy" TabIndex="4"
            AutoPostBack="False"  Font-Size="8pt"></asp:TextBox>

            <ajaxtoolkit:calendarextender runat="server" ID="Calendarextender3" 
            TargetControlID="txtPODate" Format="dd/MM/yyyy"/>
    </td>    
        <td>
        </td>
    </tr>

    <tr>
    <td style="width: 15%; height: 20px;" align="left">
	<asp:Label ID="Label1" runat="server" Font-Size="9pt">Challan No</asp:Label></td>
    <td style="width: 29%;" align="left">
    <asp:TextBox SkinId="tbPlain" ID="txtChallanNo" runat="server" Width="60%" TabIndex="5"
            CssClass="tbc" 
            Font-Size="8pt"></asp:TextBox>
    </td>    
    <td style=" width:7%;"></td>    
    <td style="width: 14%; height: 20px;" align="left">
    <asp:Label ID="lblLc" runat="server" Font-Size="9pt" >Challan Date</asp:Label></td>
    <td style="width: 23%; height: 20px;" align="left">
    <asp:TextBox SkinId="tbPlain" ID="txtChallanDate" runat="server" Width="58%"  
            placeholder="dd/MM/yyyy" TabIndex="6"
            AutoPostBack="False"  Font-Size="8pt"></asp:TextBox></td>  
             <td>
        </td>
             <ajaxtoolkit:calendarextender runat="server" ID="Calendarextender1" 
            TargetControlID="txtChallanDate" Format="dd/MM/yyyy"/> 
    </tr>
    
    <tr >
	<td style="width: 15%; height: 20px;" align="left">
    <asp:Label ID="LblSuppNo" runat="server" Font-Size="9pt" >Supplier</asp:Label></td>
	<td style="width: 29%; height: 20px;" align="left">
        <asp:TextBox ID="txtSupplierSearch" runat="server" AutoPostBack="True" CssClass="txtVisibleAlign"
            ontextchanged="txtSupplierSearch_TextChanged" placeholder="Search By Supplier.." 
            Width="100%" Height="18px"></asp:TextBox>
        <asp:HiddenField ID="hfSupplierID" runat="server" />
        <cc:AutoCompleteExtender ID="txtSupplierSearch_AutoCompleteExtender" runat="server" 
            CompletionInterval="20" CompletionSetCount="12" EnableCaching="true" 
            MinimumPrefixLength="1" ServiceMethod="GetSupplier" 
            ServicePath="AutoComplete.asmx" TargetControlID="txtSupplierSearch" />
    </td>
    <td style=" width:7%;" align="center">
        &nbsp;</td>
	<td style="width: 14%; height: 20px;" align="left" valign="middle">
	    <asp:Label ID="Label40" runat="server" BackColor="#CCCCCC" Font-Bold="True" 
            Height="22px" Text="Phone &amp; Mobile No." Width="100%"></asp:Label>
        </td>
	<td style="height: 20px;" align="left" colspan="2" valign="middle">
        <asp:Label ID="lblPhoneNo" runat="server" BackColor="#CCCCCC" Font-Bold="True" 
            Height="22px" Width="99%" style="margin-bottom: 0px"></asp:Label>
    
    </td>    
    </tr>
    <tr>
        <td align="left" style="width: 15%; height: 20px;" valign="top">
            <asp:Label ID="LblSuppNo0" runat="server" Font-Size="9pt">Remarks/Particulars</asp:Label>
        </td>
        <td align="left" colspan="5" style="height: 20px;">
            <asp:TextBox ID="txtRemarks" runat="server" AutoPostBack="False" 
                Font-Size="8pt" Height="51px" SkinID="tbGray" TextMode="MultiLine" Width="99%"></asp:TextBox>
            <cc:AutoCompleteExtender ID="autoComplete" runat="server" 
                CompletionInterval="20" CompletionSetCount="12" EnableCaching="true" 
                MinimumPrefixLength="1" ServiceMethod="GetShowRemarks" 
                ServicePath="AutoComplete.asmx" TargetControlID="txtRemarks" />
        </td>
    </tr>
    </table>
 </fieldset>
 </ContentTemplate>
    </asp:UpdatePanel>
    
</td>
<td style="width:1%;"></td>
</tr>
<tr>
<td style="width:1%;">&nbsp;</td>
<td style="width:95%; " align="center">
    <asp:Panel ID="PanelHistory" runat="server">
        <table style="width: 100%">
            <tr>
                <td>
                <fieldset style="vertical-align: top; border: solid 1px #8BB381;text-align:left;"><legend style="color: maroon;"><b>Search Option</b> </legend>
                    <table style="width: 100%">
                        <tr>
                            <td align="right">
                                <%--  <asp:UpdatePanel ID="Up1" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>--%>
                                <table style="width: 100%">
                                    <tr>
                                        <td align="left">
                                            <asp:Label ID="Label37" runat="server" style="font-weight: 700" 
                                                Text=" Goods Receive No (GRN)"></asp:Label>
                                        </td>
                                        <td align="left" style=" font-weight: 700;">
                                            Supplier</td>
                                        <td align="center" colspan="4" style="font-weight: 700">
                                            Search By Receive Date</td>
                                        <td>
                                            &nbsp;</td>
                                        <td>
                                            &nbsp;</td>
                                    </tr>
                                    <tr>
                                        <td align="left" style="width: 18%">
                                            <asp:TextBox ID="txtGrnNo" runat="server" CssClass="txtVisibleAlign" Height="18px"
                                                ontextchanged="txtGrnNo_TextChanged" placeholder="Search By GR. No" 
                                                Width="90%"></asp:TextBox>
                                            <cc:AutoCompleteExtender ID="txtGrnNo_AutoCompleteExtender" runat="server" 
                                                CompletionInterval="20" CompletionSetCount="12" EnableCaching="true" 
                                                MinimumPrefixLength="1" ServiceMethod="GetGRN" ServicePath="AutoComplete.asmx" 
                                                TargetControlID="txtGrnNo" />
                                        </td>
                                        <td align="left" style="width: 38%">
                                            <asp:UpdatePanel ID="UPSupplier" runat="server" UpdateMode="Conditional">
                                                <ContentTemplate>
                                                    <asp:TextBox ID="txtSupplier" runat="server" AutoPostBack="True" CssClass="txtVisibleAlign" Height="18px"
                                                        ontextchanged="txtSupplier_TextChanged" placeholder="Search By Supplier.." 
                                                        Width="90%"></asp:TextBox>
                                                    <cc:AutoCompleteExtender ID="AutoCompleteExtender1" runat="server" 
                                                        CompletionInterval="20" CompletionSetCount="12" EnableCaching="true" 
                                                        MinimumPrefixLength="1" ServiceMethod="GetSupplier" 
                                                        ServicePath="AutoComplete.asmx" TargetControlID="txtSupplier" />
                                                    <asp:Label ID="lblSupplier" runat="server" Visible="False"></asp:Label>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </td>
                                        <td align="left" style="width: 3%; ">
                                            &nbsp;</td>
                                        <td align="left" style="width: 11%">
                                            <asp:TextBox ID="txtFromDate" runat="server" Placeholder="Date From" CssClass="txtVisibleAlign" Height="18px" autocomplete="off"
                                                Width="90%"></asp:TextBox>
                                            <cc:CalendarExtender ID="Calender" runat="server" Format="dd/MM/yyyy" 
                                                TargetControlID="txtFromDate">
                                            </cc:CalendarExtender>
                                        </td>
                                        <td style="width: 2%">
                                            &nbsp;</td>
                                        <td align="left" style="width: 11%">
                                            <asp:TextBox ID="txtToDate" runat="server" Placeholder="Date TO" CssClass="txtVisibleAlign" Height="18px" Width="90%" autocomplete="off"></asp:TextBox>
                                            <cc:CalendarExtender ID="CalendarExtender2" runat="server" Format="dd/MM/yyyy" 
                                                TargetControlID="txtToDate">
                                            </cc:CalendarExtender>
                                        </td>
                                        <td align="center" style="width: 10%">
                                            <asp:LinkButton ID="lbSearch" runat="server" BorderStyle="Solid" 
                                                Font-Bold="True" Font-Size="12pt" onclick="BtnSearch_Click" Width="90%">Search</asp:LinkButton>
                                        </td>
                                        <td align="center" style="width: 10%">
                                            <asp:LinkButton ID="lbClear" runat="server" BorderStyle="Solid" 
                                                Font-Bold="True" Font-Size="12pt" onclick="Refresh_Click" Width="90%">Clear</asp:LinkButton>
                                        </td>
                                    </tr>
                                </table>
                                <%--  </ContentTemplate>
                                </asp:UpdatePanel>--%>
                            </td>
                        </tr>
                    </table></fieldset>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:GridView ID="dgPVMst" runat="server" AllowPaging="True" 
                        AllowSorting="True" AlternatingRowStyle-CssClass="alt" 
                        AutoGenerateColumns="False" BackColor="White" BorderColor="LightGray" 
                        BorderStyle="Solid" BorderWidth="1px" CellPadding="2" CssClass="mGrid" 
                        Font-Size="9pt" onpageindexchanging="dgPurMst_PageIndexChanging" 
                        onrowdatabound="dgPVMst_RowDataBound" 
                        onselectedindexchanged="dgPurMst_SelectedIndexChanged" 
                        PagerStyle-CssClass="pgr" PageSize="60" Width="100%">
                        <Columns>
                            <asp:CommandField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="40px" 
                                ShowSelectButton="True">
                            <ItemStyle HorizontalAlign="Center" Width="40px" />
                            </asp:CommandField>
                            <asp:BoundField DataField="GRN" HeaderText="G.R. No" 
                                ItemStyle-HorizontalAlign="Center" ItemStyle-Width="180px">
                            <ItemStyle HorizontalAlign="left" Width="180px" />
                            </asp:BoundField>
                            <asp:BoundField DataField="ReceivedDate" DataFormatString="{0:dd/MM/yyyy}" 
                                HeaderText="G.R. Date" ItemStyle-HorizontalAlign="Center" 
                                ItemStyle-Width="90px">
                            <ItemStyle HorizontalAlign="Center" Width="90px" />
                            </asp:BoundField>
                            <asp:BoundField DataField="POCode" HeaderText="P.O Code" ItemStyle-Height="20" 
                                ItemStyle-Width="100px">
                            <ItemStyle Height="20px" HorizontalAlign="Center" Width="100px" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Name" HeaderText="Supplier" 
                                ItemStyle-HorizontalAlign="Left" ItemStyle-Width="200px">
                            <ItemStyle HorizontalAlign="Left" Width="200px" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Party" HeaderText="Party" 
                                ItemStyle-HorizontalAlign="Left" ItemStyle-Width="200px">
                            <ItemStyle HorizontalAlign="Left" Width="200px" />
                            </asp:BoundField>
                            <asp:BoundField DataField="ChallanNo" HeaderText="Challan/LC No" 
                                ItemStyle-Height="20" ItemStyle-Width="100px">
                            <ItemStyle Height="20px" HorizontalAlign="Center" Width="100px" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Total" HeaderText="Total Amount" ItemStyle-Height="20" 
                                ItemStyle-Width="90px">
                            <ItemStyle Height="20px" HorizontalAlign="Right" Width="90px" />
                            </asp:BoundField>
                            <asp:BoundField DataField="ID" HeaderText="ID" ItemStyle-Height="20" 
                                ItemStyle-Width="100px">
                            <ItemStyle Height="20px" Width="100px" />
                            </asp:BoundField>
                        </Columns>
                        <RowStyle BackColor="White" />
                        <SelectedRowStyle BackColor="" Font-Bold="True" />
                        <PagerStyle BackColor="LightGray" ForeColor="Black" HorizontalAlign="Center" />
                        <AlternatingRowStyle BackColor="" />
                        <HeaderStyle Font-Size="9pt" />
                    </asp:GridView>
                </td>
            </tr>
        </table>
    </asp:Panel>
    </td>
<td style="width:1%;">&nbsp;</td>
</tr>
<tr>
<td style="width:1%;">&nbsp;</td>
<td style="width:98%; " align="left">
<%--<div runat="server" id="PVDetails">--%>
<asp:Panel ID="pnlVch" runat="server" Width="100%">
<div style="font-size: 8pt;" align="left">
<asp:UpdatePanel ID="PVIesms_UP" runat="server"  UpdateMode="Conditional">
 <ContentTemplate>
<ajaxtoolkit:TabContainer ID="tabVch" runat="server" Width="99%" ActiveTabIndex="0" Font-Size="8pt">
 <ajaxtoolkit:TabPanel ID="tpVchDtl" runat="server" HeaderText="Items Details"><ContentTemplate><table style="width:100%;">
 <tr>
   <td colspan="2" align="center">
   <asp:GridView ID="dgPVDetailsDtl" runat="server" AutoGenerateColumns="False" 
           BorderColor="LightGray" CssClass="mGrid" Font-Size="9pt"  
    OnRowDataBound="dgPurDtl_RowDataBound" OnRowDeleting="dgPurDtl_RowDeleting" 
           Width="98%"><AlternatingRowStyle CssClass="alt" />
    <Columns>            
            <asp:TemplateField>
                <ItemTemplate>
                <asp:ImageButton ID="lbDelete" runat="server" CausesValidation="False" CommandName="Delete" ImageAlign="Middle" ImageUrl="~/img/delete.png" Text="Delete" />
                </ItemTemplate>
                <ItemStyle Font-Size="8pt" HorizontalAlign="Center" Width="4%" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Code">
                <ItemTemplate>
                <asp:TextBox ID="txtItemCode" runat="server" AutoPostBack="true" Font-Size="8pt" 
                        Height="18px" MaxLength="15" ontextchanged="txtItemCode_TextChanged" 
                SkinId="tbPlain" Text='<%#Eval("item_code")%>' Width="100%" 
                        onFocus="this.select()"></asp:TextBox>
                </ItemTemplate>
                <FooterStyle HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" Width="8%" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Description">
                <ItemTemplate>
                <asp:TextBox ID="txtItemDesc" CssClass="txtVisibleAlign" runat="server" autocomplete="off" AutoPostBack="true" Font-Size="8pt" OnTextChanged="txtItemDesc_TextChanged" 
                SkinId="tbPlain" Text='<%#Eval("item_desc")%>' Width="95%" Height="18px" placeHolder="Search By Items Name.."  onFocus="this.select()">
                </asp:TextBox>
                <ajaxToolkit:AutoCompleteExtender ServicePath="AutoComplete.asmx" runat="server" ID="autoComplete1" TargetControlID="txtItemDesc"
                ServiceMethod="GetSearch_Items_On_Purchase" MinimumPrefixLength="1" CompletionInterval="20" EnableCaching="true" CompletionSetCount="12"/>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Left" Width="30%" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Barcode">
                
                
                <ItemTemplate>
                    
                    
                    
                    
                    <asp:TextBox ID="txtBarcode" CssClass="txtVisibleAlign" runat="server" autocomplete="off" AutoPostBack="true" Font-Size="8pt" OnTextChanged="txtBarcode_TextChanged" 
                                 SkinId="tbPlain" Text='<%#Eval("Barcode")%>' Width="95%" Height="18px" placeHolder="Item Barcode.."  onFocus="this.select()">
                    </asp:TextBox>
                    
                    

                </ItemTemplate>
                
                

            </asp:TemplateField>
            <asp:TemplateField HeaderText="Unit">
                <ItemTemplate>
                    
                <asp:DropDownList ID="ddlMeasure" runat="server" AutoPostBack="true" DataSource="<%#PopulateMeasure()%>" DataTextField="Name" DataValueField="ID" Font-Size="8pt" 
                SelectedValue='<%#Eval("msr_unit_code")%>' Enabled="False" SkinId="ddlPlain" Width="95%" Height="26px">
                </asp:DropDownList>

                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="8%" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Rate">
                <ItemTemplate>
                <asp:TextBox ID="txtItemRate" runat="server" AutoPostBack="True" CssClass="tbr" placeHolder="0.00"  Height="18px"
                        Font-Size="8pt" SkinId="tbPlain" Text='<%#Eval("item_rate")%>' Width="90%"  onkeypress="return isNumber(event)"
                        onFocus="this.select()" ontextchanged="txtItemsRate_TextChanged"></asp:TextBox>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Right" Width="8%" />
            </asp:TemplateField>
        <asp:TemplateField HeaderText="Sales Price">
            <ItemTemplate>
                <asp:TextBox ID="txtSalesPrice" runat="server" AutoPostBack="True" CssClass="tbr" placeHolder="0.00" Height="18px"
                             Font-Size="8pt" SkinId="tbPlain" Text='<%#Eval("item_sales_rate")%>' Width="90%"  onkeypress="return isNumber(event)"
                             onFocus="this.select()" ontextchanged="txtSalesRate_TextChanged"></asp:TextBox>
            </ItemTemplate>
            <ItemStyle HorizontalAlign="Right" Width="8%" />
        </asp:TemplateField>
        
        <asp:TemplateField HeaderText="Expire Date">
            <ItemTemplate>
                <asp:TextBox SkinId="tbPlain" ID="txtExpireDate" runat="server" Width="90%"  
                             placeholder="dd/MM/yyyy" Height="18px"
                             AutoPostBack="True"  Font-Size="8pt" ontextchanged="txtExpireDate_TextChanged" 
                             Text='<%# Eval("Expdate") %>'></asp:TextBox>
                <ajaxtoolkit:calendarextender runat="server" ID="Calendarextender3" 
                                              TargetControlID="txtExpireDate" Format="dd/MM/yyyy"/>
            </ItemTemplate>
            <FooterStyle HorizontalAlign="Center" />
            <ItemStyle HorizontalAlign="Right" Width="8%" />
        </asp:TemplateField>

            <asp:TemplateField HeaderText="Quantity">
                <ItemTemplate>
                <asp:TextBox ID="txtQnty" runat="server" AutoPostBack="true" placeHolder="0" CssClass="tbc" Font-Size="8pt" OnTextChanged="txtQnty_TextChanged" SkinId="tbPlain" 
                Text='<%#Eval("qnty")%>' Width="90%" Height="18px" onkeypress="return isNumber(event)" onFocus="this.select()">
                </asp:TextBox>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="8%" />
            </asp:TemplateField>
            
            <asp:TemplateField HeaderText="Item Total">
                <ItemTemplate>
                <asp:Label ID="lblTotal" runat="server" Font-Size="8pt" Width="95%"></asp:Label>
                </ItemTemplate>
                <FooterStyle HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Right" Width="8%" />
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Additinal(%)">
                <ItemTemplate>
                <asp:TextBox ID="txtAdditional" runat="server" AutoPostBack="true" CssClass="tbc"  onkeypress="return isNumber(event)"
                        Font-Size="8pt"  SkinId="tbPlain" 
                Text='<%#Eval("Additional")%>' Width="90%" onFocus="this.select()" 
                        ontextchanged="txtAdditional_TextChanged">
                </asp:TextBox>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="9%" />
            </asp:TemplateField>
            
            <asp:TemplateField HeaderText="Total Amount">
                <ItemTemplate>
                <asp:Label ID="lblAddTotal" runat="server" Font-Size="8pt" Width="95%"></asp:Label>
                </ItemTemplate>
                <FooterStyle HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Right" Width="8%" />
            </asp:TemplateField>

           <asp:TemplateField HeaderText="ID">
            <ItemTemplate><asp:Label ID="lblid" runat="server" Font-Size="8pt" Width="95%" Text='<%#Eval("ID")%>'>
            </asp:Label></ItemTemplate>
               <FooterStyle HorizontalAlign="Center" /><ItemStyle HorizontalAlign="Right" Width="10%" />
            </asp:TemplateField></Columns><HeaderStyle Font-Bold="True" Font-Size="9pt" ForeColor="White" />
            <PagerStyle CssClass="pgr" ForeColor="White" HorizontalAlign="Center" /><RowStyle BackColor="White" />
            </asp:GridView>
            </td>
          </tr>
          <tr>
              <td style="width: 45%"></td>
              <td></td>
          </tr>
          <tr>
          <td valign="top" style="width: 45%"  >           
           <fieldset style=" border:solid 1px #8BB381;text-align:left;"><legend><b>  Payment Methord </b> </legend>
            <asp:UpdatePanel runat="server" UpdateMode="Conditional" ID="UP1"><ContentTemplate>
           <table style="width: 100%; height: 206px;">
           <tr>
               <td style="width: 40%; height: 32px;">
               <asp:Label ID="Label26" runat="server" Font-Size="9pt">Payment Methord</asp:Label></td>
               <td style="width:5%; height: 32px;"></td>
               <td  style="width:55%; height: 32px;">
               <asp:DropDownList ID="ddlPaymentMethord" 
                       runat="server" Font-Size="8pt" 
                       SkinID="ddlPlain" Width="100%" Height="24px" AutoPostBack="True" 
                       onselectedindexchanged="ddlPaymentMethord_SelectedIndexChanged" 
                       TabIndex="19"><asp:ListItem Text="Cash" Value="C"></asp:ListItem>
                   <asp:ListItem Value="Q">Cheque</asp:ListItem>
               </asp:DropDownList>
               </td>
           </tr>
           <tr>
           <td style="width: 137px; height: 30px;"><asp:Label ID="lblAmount" runat="server" Font-Size="9pt">Cash Amount </asp:Label></td>
           <td style="width: 15px; height: 30px;"></td>
           <td style="height: 30px">
               <asp:TextBox ID="txtChequeAmount" runat="server" onfocus="this.select();"  
                    onkeypress="return isNumber(event)"
                   Font-Size="8pt" SkinID="tbPlain" style="text-align:right;" Width="94%" 
                   TabIndex="20" 
                   AutoPostBack="True" ontextchanged="txtChequeAmount_TextChanged"></asp:TextBox>
           </td>
           </tr>
               <tr>
                   <td style="width: 137px; height: 29px;">
                       <asp:Label ID="lblBankName" runat="server" Font-Size="9pt">Bank 
                       Name</asp:Label>
                   </td>
                   <td style="width: 15px; height: 29px;">
                       </td>
                   <td style="height: 29px">
                       <asp:DropDownList ID="ddlBank" runat="server" Font-Size="8pt" 
                           Height="24px" SkinID="ddlPlain" Width="100%" TabIndex="21">
                       </asp:DropDownList>
                   </td>
               </tr>
           <tr>
           <td style="width: 137px; height: 26px;"><asp:Label ID="lblChequeNo" runat="server" 
                   Font-Size="9pt">Cheque/Card No.</asp:Label></td>
           <td style="width: 15px; height: 26px;"></td>
           <td style="height: 26px">
               <asp:TextBox ID="txtChequeNo" runat="server" 
                   Font-Size="8pt" SkinID="tbPlain" Width="94%" TabIndex="22"></asp:TextBox></td>
           </tr>
           <tr><td style="width: 137px; height: 27px;"><asp:Label ID="lblChequeDate" 
                   runat="server" Font-Size="9pt">Cheque date</asp:Label></td>
           <td style="width: 15px; height: 27px;"></td>
           <td style="height: 27px">
               <asp:TextBox ID="txtChequeDate" runat="server" 
                   Font-Size="8pt" SkinId="tbPlain" 
                      Width="94%" TabIndex="23"></asp:TextBox>
                      <cc:CalendarExtender ID="txtChequeDate_CalendarExtender" runat="server"  Format="dd/MM/yyyy" Enabled="True" TargetControlID="txtChequeDate"></cc:CalendarExtender></td></tr>
                      <tr>
                      <td style="width: 137px; height: 26px;">
                          <asp:Label ID="lblChequeStatus" runat="server" Font-Size="9pt">Check 
                          Status</asp:Label>
                          </td>
                      <td style="width: 15px; height: 26px;"></td>
                      <td style="height: 26px">
                          <asp:DropDownList ID="ddlChequeStatus" runat="server" Font-Size="8pt" 
                              Height="24px" onselectedindexchanged="ddlChequeStatus_SelectedIndexChanged" 
                              SkinID="ddlPlain" Width="100%" TabIndex="24" AutoPostBack="True">
                              <asp:ListItem Value="A">Approved</asp:ListItem>
                              <asp:ListItem Value="P">Pending</asp:ListItem>
                          </asp:DropDownList>
                          </td>
                      </tr>
                </table>
               </ContentTemplate></asp:UpdatePanel>
              </fieldset>             
              &nbsp; &nbsp; &nbsp; &nbsp;&nbsp; &nbsp; &nbsp; &nbsp;             
            </td>
            <td valign="top">
            <fieldset style=" border:solid 1px #8BB381;text-align:left;"><legend><b> Payment 
                Information</b></legend>
            <asp:UpdatePanel runat="server" UpdateMode="Conditional" ID="UP2"><ContentTemplate>
            <table style="width: 100%;">
            <tr>
                <td style="width:20%; height: 30px;" align="right">
                    <asp:Label ID="Label36" runat="server" Font-Size="9pt" Visible="False">Items Charge</asp:Label>
                </td>
                <td style="width:3%; height: 30px;"></td>
                <td style="width:12%; height: 30px;">
                    <asp:TextBox ID="txtTotItems" runat="server" CssClass="tbc" Enabled="False" onkeypress="return isNumber(event)"
                        Font-Size="8pt" onfocus="this.select();" SkinID="tbPlain" 
                        style="text-align:right;" TabIndex="10" Width="50%" Visible="False"></asp:TextBox>
                </td>
                <td align="right" style="width:25%; height: 30px;"><asp:Label ID="Label30" runat="server" Font-Size="9pt">Grand Total</asp:Label></td>
                <td style="width:3%; height: 30px;"></td>
                <td style="width:20%; height: 30px;"><asp:TextBox ID="txtTotalAmount" runat="server" CssClass="tbc" Font-Size="8pt" onfocus="this.select();" SkinID="tbPlain" TabIndex="10"
             style="text-align:right;" Width="92%" Enabled="False" onkeypress="return isNumber(event)"></asp:TextBox></td>
            </tr>
                
                <tr>
                    <td align="right" style="width: 20%; height: 30px;"></td>
                    <td style="height: 30px"></td>
                    <td style="height: 30px; width: 12%;"></td>
                    <td align="right" style="height: 30px"><asp:Label ID="Label2" runat="server" Font-Size="9pt">Discount 
                        Amount</asp:Label></td>
                    <td style="height: 30px"></td>
                    <td style="height: 30px">
                        
                        <asp:TextBox ID="txtDiscountAmt" runat="server" onfocus="this.select();"  
                                     onkeypress="return isNumber(event)"
                                     Font-Size="8pt" SkinID="tbPlain" style="text-align:right;" Width="94%" 
                                     TabIndex="20" 
                                     AutoPostBack="True" ontextchanged="txtDiscountAmt_TextChanged"></asp:TextBox>
                        
                        
                    </td>
                </tr>
                

            <tr>
                <td align="right" style="width: 20%; height: 30px;">
                    <asp:Label ID="Label35" runat="server" Font-Size="9pt" Visible="False">Total Additional</asp:Label>
                </td>
                <td style="height: 30px"></td>
                <td style="height: 30px; width: 12%;">
                    <asp:TextBox ID="txtAddTot" runat="server" CssClass="tbc" Enabled="False" onkeypress="return isNumber(event)"
                        Font-Size="8pt" onfocus="this.select();" SkinID="tbPlain" 
                        style="text-align:right;" TabIndex="10" Width="50%" Visible="False"></asp:TextBox>
                </td>
                <td align="right" style="height: 30px"><asp:Label ID="Label31" runat="server" Font-Size="9pt">Total Payment</asp:Label></td>
                <td style="height: 30px"></td>
                <td style="height: 30px"><asp:TextBox ID="txtTotPayment" runat="server" CssClass="tbc" Font-Size="8pt" TabIndex="12" 
                        onChange="TotSumation();" onfocus="this.select();" SkinID="tbPlain" 
                              style="text-align:right;" Width="92%" Enabled="False" onkeypress="return isNumber(event)"></asp:TextBox></td>
                </tr>
                <tr>
                <td align="right" style="width: 20%; height: 30px;"></td>
                <td style="height: 30px"></td>
                <td style="height: 30px; width: 12%;"></td>
                <td align="right" style="height: 30px"><asp:Label ID="Label33" runat="server" Font-Size="9pt">Due Amount</asp:Label></td>
                <td style="height: 30px"></td>
                <td style="height: 30px"><asp:TextBox ID="txtDue" runat="server" CssClass="tbc" Font-Size="8pt" TabIndex="13"
                        onfocus="this.select();" SkinID="tbPlain" style="text-align:right;" 
                  Width="92%" Enabled="False" onkeypress="return isNumber(event)"></asp:TextBox></td>
           </tr>
                
              
           <tr>
               <td colspan="6">
          <table style="width:100%; border:1px solid lightgray;display: none"><tr>
               <td style="width:21%;"></td>
               <td style="width:3%;"></td>
               <td style="width:35%;"></td>
               <td style="width:20%;"></td>
               <td style="width:3%;"></td>
               <td style="width:20%;"></td>
              </tr>
              <tr>
                   <td align="right" style="width: 21%"><asp:Label ID="Label13" runat="server" Font-Size="9pt">Carriage Person</asp:Label></td>
                   <td>&nbsp;</td>
                   <td>
                       <asp:DropDownList ID="ddlCarriagePerson" runat="server" Font-Size="8pt" 
                                         Height="26px" SkinID="ddlPlain" Width="80%" TabIndex="14"></asp:DropDownList></td>
                   <td align="right"><asp:Label ID="Label21" runat="server" Font-Size="9pt">Carriage Charge</asp:Label></td>
                   <td>&nbsp;</td>
                   <td><asp:TextBox ID="txtCarriageCharge" runat="server" CssClass="tbc" 
                                 Font-Size="8pt" onfocus="this.select();" SkinID="tbPlain" 
                                 style="text-align:right;" Width="92%" TabIndex="15" onkeypress="return isNumber(event)"></asp:TextBox></td>
            </tr>
                   <tr>
                   <td align="right" style="width: 21%"><asp:Label ID="Label14" runat="server" Font-Size="9pt">Labour Person</asp:Label></td>
                   <td>&nbsp;</td>
                   <td>
                       <asp:DropDownList ID="ddlLaburePerson" runat="server" Font-Size="8pt" 
                                         Height="26px" SkinID="ddlPlain" Width="80%" TabIndex="16"></asp:DropDownList></td>
                   <td align="right"><asp:Label ID="Label22" runat="server" Font-Size="9pt">Labour Person</asp:Label></td>
                   <td>&nbsp;</td><td><asp:TextBox ID="txtLabureCharge" 
                           runat="server" CssClass="tbc" Font-Size="8pt" 
                                         onfocus="this.select();" SkinID="tbPlain" style="text-align:right;" 
                                 Width="92%" TabIndex="17" onkeypress="return isNumber(event)"></asp:TextBox></td>
             </tr>
                   <tr>
                   <td style="width: 21%">&nbsp;</td>
                   <td>&nbsp;</td>
                   <td>&nbsp;</td>
                   <td align="right"><asp:Label ID="Label32" runat="server" Font-Size="9pt">Other Charge</asp:Label></td>
                   <td>&nbsp;</td>
                   <td>
                   <asp:TextBox ID="txtOtherCharge" runat="server" CssClass="tbc" Font-Size="8pt" onkeypress="return isNumber(event)"
                                 onfocus="this.select();" SkinID="tbPlain" style="text-align:right;" 
                                 Width="92%" TabIndex="18"></asp:TextBox></td>
           </tr>
       </table>
       </td>
       </tr>
       </table>
                </ContentTemplate></asp:UpdatePanel>
       </fieldset>                
                &nbsp; &nbsp; &nbsp; &nbsp;&nbsp; &nbsp; &nbsp; &nbsp;                
                </td>
       </tr>
       </table>
       </ContentTemplate>
       </ajaxtoolkit:TabPanel>
</ajaxtoolkit:TabContainer>
</ContentTemplate>
        </asp:UpdatePanel>
        </div> 
       </asp:Panel>
     <%--</div>--%>
</td>
<td style="width:1%;">&nbsp;</td>
</tr>
<tr>
<td style="width:1%;">&nbsp;</td>
<td style="width:98%; " align="center">
 <asp:Panel ID="pnlClient" runat="server" CssClass="modalPopup1" Style="padding:15px 15px 15px 15px; display:none;  background-color:White; border:1px solid black;" Width="700px">
  <fieldset style="vertical-align: top; border: solid 1px #8BB381;">
                        <legend style="color: maroon;"><b>Save Data</b></legend>     
<table style="width:100%;">
<tr>
<td style="width:15%;" align="left">
    &nbsp;</td>
<td style="width:16%;" align="right"> 
    <asp:Label ID="Label3" runat="server" Text="Type"></asp:Label></td>
<td style=" width:4%;" >&nbsp;</td>
<td style="width:41%;" align="left">
    <asp:DropDownList SkinID="ddlPlain" ID="ddlType" runat="server"  
                    Font-Size="8" Width="100%" TabIndex="2" Height="26px">
  <asp:ListItem></asp:ListItem>
  <asp:ListItem Value="S">Supplier</asp:ListItem> 
  <asp:ListItem Value="P">Party</asp:ListItem>

  </asp:DropDownList></td>
<td style="width:25%;" align="left" > 
    &nbsp;</td>
</tr>
<tr>
<td style="width:15%;" align="left">
    &nbsp;</td>
<td style="width:16%;" align="right"> 
    <asp:Label ID="Label4" runat="server" Text="Name"></asp:Label>
    </td>
<td style=" width:4%;" >&nbsp;</td>
<td style="width:41%;" align="left">
    <asp:TextBox ID="txtvalue" runat="server" Width="100%"></asp:TextBox>
    
</td>
<td style="width:25%;" align="left" > 
    &nbsp;</td>
</tr>
    <tr>
        <td align="left" style="width:15%;">
            &nbsp;</td>
        <td align="right" style="width:16%;">
            Company Name</td>
        <td style=" width:4%;">
            &nbsp;</td>
        <td align="left" style="width:41%;">
            <asp:TextBox ID="txtCompanyName" runat="server" Width="100%"></asp:TextBox>
        </td>
        <td align="left" style="width:25%;">
            &nbsp;</td>
    </tr>
    <tr>
        <td align="left" style="width:15%;">
            &nbsp;</td>
        <td align="right" style="width:16%;">
            <asp:Label ID="Label38" runat="server" Text="Mobile"></asp:Label>
        </td>
        <td style=" width:4%;">
            &nbsp;</td>
        <td align="left" style="width:41%;">
            <asp:TextBox ID="txtMobile" runat="server" Width="100%"></asp:TextBox>
        </td>
        <td align="left" style="width:25%;">
            &nbsp;</td>
    </tr>
    <tr>
        <td align="left" style="width:15%;">
            &nbsp;</td>
        <td align="right" style="width:16%;">
            <asp:Label ID="Label39" runat="server" Text="E-mail"></asp:Label>
        </td>
        <td style=" width:4%;">
            &nbsp;</td>
        <td align="left" style="width:41%;">
            <asp:TextBox ID="txtEmail" runat="server" Width="100%"></asp:TextBox>
        </td>
        <td align="left" style="width:25%;">
            &nbsp;</td>
    </tr>
    <tr>
        <td align="left" style="width:15%;">
            &nbsp;</td>
        <td align="right" style="width:16%;">
            Country</td>
        <td style=" width:4%;">
            &nbsp;</td>
        <td align="left" style="width:41%;">
            <asp:DropDownList ID="ddlPopSupplier" runat="server" 
                Font-Size="8pt" Height="26px" SkinId="ddlPlain" 
                TabIndex="7" Width="100%">
            </asp:DropDownList>
        </td>
        <td align="left" style="width:25%;">
            &nbsp;</td>
    </tr>
<tr>
<td style="width:100%;" colspan="5">
<table style="width:100%;">
  <tr>  
  <td style="width:5%;"></td>
  <td align="right" >
       </td>   
       <td style="width:20px;"></td>
       <td align="left" >
       <asp:Button ID="btnClientQuit" runat="server" ToolTip="Quit Client" Text="Quit" OnClientClick="HideModalDiv();"
        Height="30px" Width="100px" BorderStyle="Outset"  Font-Size="9pt" />
       </td>        
       <td style="width:5%;"></td>       
   </tr>
   </table>
</td>
</tr>
</table>   
</fieldset> 
    </asp:Panel>
</td>
<td style="width:1%;">&nbsp;</td>
</tr>
</table>
</div>
<div id="divBackground" style="position: fixed; z-index: 999; height: 100%; width: 100%;
        top: 0; left:0; background-color: Black; filter: alpha(opacity=60); opacity: 0.6; -moz-opacity: 0.8;-webkit-opacity: 0.8;display:none">
    </div>
</asp:Content>



