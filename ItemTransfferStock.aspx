<%@ Page Title="Item Transffer.- Dorjibari." Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="ItemTransfferStock.aspx.cs" Inherits="ItemTransfferStock" %>

<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
<script language="javascript" type="text/javascript" >
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
    function LoadModalDiv() {

        var bcgDiv = document.getElementById("divBackground");
        bcgDiv.style.display = "block";

    }
    function HideModalDiv() {

        var bcgDiv = document.getElementById("divBackground");
        bcgDiv.style.display = "none";

    }
    	
</script>

    <div id="frmMainDiv" style="width:100%; background-color:White;">
    
   
    

    <table style="width:100%">
    <tr>
        <td style="width:05%">
        
        </td>
        <td style="width:90%">
        </td>
        <td style="width:5%"></td>
    </tr>
    <tr>
        <td style="width:05%">
        
            &nbsp;</td>
        <td style="width:90%">
            <asp:Panel ID="Panel2" runat="server">
                <ajaxToolkit:TabContainer ID="TabContainer1" runat="server" ActiveTabIndex="1" 
                    Width="100%">
                    <ajaxToolkit:TabPanel ID="TabPanel1" runat="server" HeaderText="Finish Goods">
                        <HeaderTemplate>
                            Finish Goods Item
                        </HeaderTemplate>
                        <ContentTemplate>
                            <table ID="pageFooterWrapper">
                                 <tr>
                                    <td align="center" style="vertical-align:middle; height:100%;">
                                        <asp:Button ID="BtnDelete" runat="server" BorderStyle="Outset" 
                                            BorderWidth="1px" Height="35px" onclick="BtnDelete_Click" 
                                            onclientclick="javascript:return window.confirm('are u really want to delete these data')" 
                                            TabIndex="903" Text="Delete" ToolTip="Delete Record" Visible="False" 
                                            Width="110px" />
                                    </td>
                                    <td align="center" style="vertical-align:middle;">
                                        <asp:Button ID="BtnSave" runat="server" BorderStyle="Outset" BorderWidth="1px" 
                                            Height="35px" onclick="BtnSave_Click" 
                                            onclientclick="javascript:return window.confirm('are u really want to save these data')" 
                                            TabIndex="901" Text="Save" ToolTip="Save or Update Record" Width="110px" />
                                    </td>
                                    <td align="center" style="vertical-align:middle;">
                                        <asp:Button ID="BtnReset" runat="server" BorderStyle="Outset" BorderWidth="1px" 
                                            Height="35px" OnClick="BtnReset_Click" TabIndex="904" Text="Clear" 
                                            ToolTip="Clear Form" Width="110px" />
                                    </td>
                                    <td align="center" style="vertical-align:middle;">
                                        &nbsp;</td>
                                    <td align="center" style="vertical-align:middle;">
                                        <asp:Button ID="btnPrint" runat="server" BorderStyle="Outset" BorderWidth="1px" 
                                            Height="35px" onclick="btnPrint_Click" TabIndex="904" Text="Preview" 
                                            ToolTip="Bill Print" Width="110px" Enabled="False" />

                                             <ajaxToolkit:ModalPopupExtender ID="ModalPopupExtenderLogin3" runat="server" 
                                                            BackgroundCssClass="modalBackground"  DynamicServicePath="" 
                                                            Enabled="True" PopupControlID="LoginPanel1" TargetControlID="btnCancel">
                                                        </ajaxToolkit:ModalPopupExtender>
                                    </td>
                                    <td align="center" style="vertical-align:middle;">
                                        <asp:Button ID="btnExcel" runat="server" BorderStyle="Outset" BorderWidth="1px" 
                                            Height="35px" onclick="btnExcel_Click" TabIndex="904" Text="Excel Export" 
                                            ToolTip="Excel Export" Width="110px" Enabled="False" />
                                    </td>
                                </tr>
                            </table>
                            <table style="width: 100%">
                                <tr>
                                    <td style="width:100%">
                                        &nbsp;</td>
                                </tr>
                                <tr>
                                    <td style="width:86%; height: 66px;">
                                        <fieldset style="vertical-align: top; border: solid 1px #8BB381; text-align:left;">
                                            <legend style="color: maroon;"><b>Transfer Information</b></legend>
                                            <asp:Panel ID="pnl" runat="server">
                                                <table style="width: 100%">
                                                    <tr>
                                                        <td style="width: 6%; height: 29px; font-weight: 700;">
                                                            Transfer Code</td>
                                                        <td align="center" style="width: 1%; height: 29px;">
                                                            :</td>
                                                        <td style="width: 13%; height: 29px;">
                                                            <asp:TextBox ID="txtTransferCode" runat="server" Width="60%"></asp:TextBox>
                                                            <asp:Label ID="lblPrintFlag" runat="server" Visible="False"></asp:Label>
                                                        </td>
                                                        <td align="right" style="width: 10%; height: 29px;">
                                                            <asp:Label ID="Label9" runat="server" Text="Challan No"></asp:Label>
                                                        </td>
                                                        <td align="center" style="width: 2%; height: 29px;">
                                                            &nbsp;</td>
                                                        <td style="width: 10%; height: 29px;">
                                                            <asp:TextBox ID="txtChallanNo" runat="server" Enabled="False" Width="53%"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="width: 6%; height: 29px;">
                                                            <asp:Label ID="Label5" runat="server" Text="Branch Name"></asp:Label>
                                                        </td>
                                                        <td align="center" style="width: 1%; height: 29px;">
                                                            :</td>
                                                        <td style="width: 13%; height: 29px;">
                                                            <asp:DropDownList ID="ddlBranch" runat="server" AutoPostBack="True" 
                                                                Font-Size="8pt" Height="26px" 
                                                                onselectedindexchanged="ddlBranch_SelectedIndexChanged" SkinId="ddlPlain" 
                                                                Width="93%">
                                                            </asp:DropDownList>
                                                        </td>
                                                        <td align="right" style="width: 10%; height: 29px;">
                                                            <table style="width: 100%">
                                                                <tr>
                                                                    <td style="width: 50%;text-align: center">
                                                                        <asp:CheckBox ID="chkAllItems" runat="server" Text="All Items" 
                                                                            Visible="False" />
                                                                    </td>
                                                                    <td style="width: 50%">
                                                                        <asp:Label ID="lblID" runat="server" Visible="False"></asp:Label>
                                                                        <asp:Label ID="Label8" runat="server" Text="Transfer Date"></asp:Label>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                        <td align="center" style="width: 2%; height: 29px;">
                                                            :</td>
                                                        <td style="width: 10%; height: 29px;">
                                                            <asp:TextBox ID="txtTfDate" runat="server" CssClass="tbc" Font-Size="8pt" 
                                                                PlaceHolder="dd/MM/yyyy" SkinId="tbPlain" style="text-align:center;" 
                                                                Width="93%"></asp:TextBox>
                                                            <ajaxToolkit:CalendarExtender ID="Calendarextender1" runat="server" 
                                                                Enabled="True" Format="dd/MM/yyyy" TargetControlID="txtTfDate" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="width: 6%; height: 25px;">
                                                            <asp:Label ID="Label7" runat="server" Text="Remarks"></asp:Label>
                                                        </td>
                                                        <td align="center" style="width: 1%; height: 25px;">
                                                            :</td>
                                                        <td colspan="4" style="height: 25px;">
                                                            <asp:TextBox ID="txtRemark" runat="server" Width="98%"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </asp:Panel>
                                        </fieldset>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="3">
                                        <asp:Panel ID="pnlItemDtl" runat="server">
                                            <fieldset style="vertical-align: top; border: solid 1px #8BB381;text-align:left;">
                                                <legend style="color: maroon;"><b>Items Details </b></legend>
                                                <div ID="Div2" runat="server">
                                                    <table style="width: 100%">
                                                        <tr>
                                                            <td align="right" style="width: 25%; height: 16px;">
                                                              </td>
                                                            <td style="width: 4%; height: 16px;">
                                                            </td>
                                                            <td style="width: 70%; height: 16px;">
                                                                <asp:TextBox ID="txtSearchCarton" runat="server" AutoPostBack="True" 
                                                                    CssClass="tbc" Font-Size="8pt" ontextchanged="txtSearchCarton_TextChanged" 
                                                                    PlaceHolder="Search Carton No.." SkinId="tbPlain" style="text-align:center;" 
                                                                    Width="53%" Visible="False"></asp:TextBox>
                                                                <ajaxToolkit:AutoCompleteExtender ID="AutoCompleteExtender4" runat="server" 
                                                                    CompletionInterval="20" CompletionSetCount="12" DelimiterCharacters="" 
                                                                    Enabled="True" MinimumPrefixLength="1" ServiceMethod="GetsearchCartonNo" 
                                                                    ServicePath="AutoComplete.asmx" TargetControlID="txtSearchCarton" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="right" style="width: 25%">
                                                                &nbsp;</td>
                                                            <td style="width: 4%">
                                                                &nbsp;</td>
                                                            <td style="width: 70%">
                                                                &nbsp;</td>
                                                        </tr>
                                                    </table>
                                                </div>
                                                <div ID="ItemsDetails" runat="server">
                                                    <asp:UpdatePanel ID="Up1" runat="server" UpdateMode="Conditional">
                                                        <ContentTemplate>
                                                            <asp:GridView ID="dgPODetailsDtl" runat="server" AutoGenerateColumns="False" 
                                                                BorderColor="LightGray" CssClass="mGrid" Font-Size="9pt" 
                                                                OnRowDataBound="dgPurDtl_RowDataBound" OnRowDeleting="dgPurDtl_RowDeleting" 
                                                                Width="100%">
                                                                <AlternatingRowStyle CssClass="alt" />
                                                                <Columns>
                                                                    <asp:TemplateField>
                                                                        <ItemTemplate>
                                                                            <asp:ImageButton ID="lbDelete" runat="server" CausesValidation="False" 
                                                                                CommandName="Delete" ImageAlign="Middle" ImageUrl="~/img/delete.png" 
                                                                                Text="Delete" />
                                                                        </ItemTemplate>
                                                                        <ItemStyle Font-Size="8pt" HorizontalAlign="Center" Width="4%" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Barcode">
                                                                        <ItemTemplate>
                                                                            <asp:TextBox ID="txtBarcode" runat="server" AutoPostBack="True" 
                                                                                Text='<%# Eval("Barcode") %>' Width="96%"></asp:TextBox>
                                                                        </ItemTemplate>
                                                                        <HeaderStyle HorizontalAlign="Center" />
                                                                        <ItemStyle HorizontalAlign="Center" Width="11%" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Style No">
                                                                        <ItemTemplate>
                                                                            <asp:TextBox ID="txtItemCode" runat="server" AutoPostBack="true" 
                                                                                Enabled="false" Font-Size="8pt" MaxLength="15" onFocus="this.select()" 
                                                                                ReadOnly="True" SkinId="tbPlain" Text='<%#Eval("StyleNo")%>' Width="90%"></asp:TextBox>
                                                                        </ItemTemplate>
                                                                        <FooterStyle HorizontalAlign="Center" />
                                                                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                        <ItemStyle HorizontalAlign="Center" Width="8%" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Items Name">
                                                                        <ItemTemplate>
                                                                            <asp:TextBox ID="txtItemName" runat="server" AutoPostBack="True" 
                                                                                ontextchanged="txtItemName_TextChanged" 
                                                                                placeHolder="Search Items Code/Style No./Name" Text='<%#Eval("ItemsName")%>' 
                                                                                Width="96%"></asp:TextBox>
                                                                            <ajaxToolkit:AutoCompleteExtender ID="autoComplete1" runat="server" 
                                                                                CompletionInterval="20" CompletionSetCount="12" EnableCaching="true" 
                                                                                MinimumPrefixLength="1" ServiceMethod="GetItemsSearchNullBrance" 
                                                                                ServicePath="AutoComplete.asmx" TargetControlID="txtItemName" />
                                                                            <asp:Label ID="lblItemsID" runat="server" style="display:none;" 
                                                                                Text='<%#Eval("ItemsID")%>'></asp:Label>
                                                                        </ItemTemplate>
                                                                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                        <ItemStyle HorizontalAlign="Center" Width="25%" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Stock Qty">
                                                                        <ItemTemplate>
                                                                            <asp:TextBox ID="txtStockQuantity" runat="server" AutoPostBack="False" 
                                                                                CssClass="txtVisibleAlign" Enabled="false" Font-Size="8pt" 
                                                                                onFocus="this.select()" onkeypress="return isNumber(event)" placeHolder="0.00" 
                                                                                SkinId="tbPlain" style="text-align:right;" Text='<%#Eval("StockQty")%>' 
                                                                                Width="90%">
           </asp:TextBox>
                                                                        </ItemTemplate>
                                                                        <ItemStyle HorizontalAlign="Right" Width="8%" />
                                                                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Transfer Price">
                                                                        <ItemTemplate>
                                                                            <asp:TextBox ID="txtPrice" runat="server" AutoPostBack="true" 
                                                                                CssClass="txtVisibleAlign" Enabled="False" Font-Size="8pt" 
                                                                                onFocus="this.select()" onkeypress="return isNumber(event)" placeHolder="0.00" 
                                                                                SkinId="tbPlain" style="text-align:right;" Text='<%#Eval("Price")%>' 
                                                                                Width="90%"></asp:TextBox>
                                                                        </ItemTemplate>
                                                                        <ItemStyle HorizontalAlign="Center" Width="10%" />
                                                                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Transfer Qty">
                                                                        <ItemTemplate>
                                                                            <asp:UpdatePanel ID="Uptr" runat="server" UpdateMode="Conditional">
                                                                                <ContentTemplate>
                                                                                    <asp:TextBox ID="txtTransferQuantity" runat="server" AutoPostBack="true" 
                                                                                        CssClass="tbc" Font-Size="8pt" onFocus="this.select()" 
                                                                                        onkeypress="return isNumber(event)" 
                                                                                        ontextchanged="txtTransferQuantity_TextChanged" placeHolder="0.00" 
                                                                                        SkinId="tbPlain" style="text-align:right;" Text='<%#Eval("TransferQty")%>' 
                                                                                        Width="90%"></asp:TextBox>
                                                                                </ContentTemplate>
                                                                            </asp:UpdatePanel>
                                                                        </ItemTemplate>
                                                                        <ItemStyle HorizontalAlign="Center" Width="10%" />
                                                                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Branch Sale P.">
                                                                        <ItemTemplate>
                                                                            <asp:TextBox ID="txtBranchSalePrice" runat="server" AutoPostBack="true" 
                                                                                CssClass="tbc" Enabled="False" Font-Size="8pt" onFocus="this.select()" 
                                                                                onkeypress="return isNumber(event)" ontextchanged="txtPrice_TextChanged" 
                                                                                placeHolder="0.00" SkinId="tbPlain" style="text-align:right;" 
                                                                                Text='<%#Eval("BranchSalesPrice")%>' Width="93%"></asp:TextBox>
                                                                            <%-- <asp:TextBox ID="txtBranchSalePrice" runat="server" 
                                     onkeypress="return isNumber(event)" placeHolder="0.00"
                                     AutoPostBack="true" CssClass="tbc" 
                                       Font-Size="8pt"  SkinId="tbPlain" 
                                       Text='<%#Eval("")%>' Width="93%" onFocus="this.select()" 
                                     style="text-align:right;" ontextchanged="txtPrice_TextChanged">
                                 </asp:TextBox>--%>
                                                                        </ItemTemplate>
                                                                        <ItemStyle HorizontalAlign="Center" Width="10%" />
                                                                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                    </asp:TemplateField>
                                                                    <asp:BoundField DataField="ReceivedQuantity" HeaderText="Received Qty.">
                                                                    <ItemStyle HorizontalAlign="Center" Width="10%" />
                                                                    <HeaderStyle HorizontalAlign="Right" VerticalAlign="Middle" />
                                                                    </asp:BoundField>
                                                                    <asp:TemplateField HeaderText="Code">
                                                                        <ItemTemplate>
                                                                            <asp:TextBox ID="txtCode" runat="server" AutoPostBack="true" Enabled="true" 
                                                                                Font-Size="8pt" MaxLength="15" onFocus="this.select()" 
                                                                                ontextchanged="txtCode_TextChanged" SkinId="tbPlain" 
                                                                                Text='<%#Eval("item_code")%>' Width="90%"></asp:TextBox>
                                                                            <ajaxToolkit:AutoCompleteExtender ID="autoCompletecode" runat="server" 
                                                                                CompletionInterval="20" CompletionSetCount="12" EnableCaching="true" 
                                                                                MinimumPrefixLength="1" ServiceMethod="GetItemDetailsCode" 
                                                                                ServicePath="AutoComplete.asmx" TargetControlID="txtCode" />
                                                                        </ItemTemplate>
                                                                        <FooterStyle HorizontalAlign="Center" />
                                                                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                        <ItemStyle HorizontalAlign="Center" />
                                                                    </asp:TemplateField>
                                                                </Columns>
                                                                <HeaderStyle Font-Bold="True" Font-Size="9pt" ForeColor="White" />
                                                                <PagerStyle CssClass="pgr" ForeColor="White" HorizontalAlign="Center" />
                                                                <RowStyle BackColor="White" />
                                                            </asp:GridView>
                                                        </ContentTemplate>
                                                    </asp:UpdatePanel>
                                                </div>
                                            </fieldset>
                                        </asp:Panel>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="3">
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center" colspan="3" style="height: 18px">
                                    </td>
                                </tr>
                                <tr>
                                    <td style="width:6%">
                                        &nbsp;</td>
                                    <td style="width:86%">
                                        &nbsp;</td>
                                    <td style="width:15%">
                                        &nbsp;</td>
                                </tr>
                            </table>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                  
                    <ajaxToolkit:TabPanel ID="TabPanel3" runat="server" HeaderText="TabPanel2">
                        <HeaderTemplate>
                            History
                        </HeaderTemplate>
                        <ContentTemplate>
                            <table style="width: 100%">
                                <tr>
                                    <td style="width: 5%">
                                        &nbsp;</td>
                                    <td style="width: 90%">
                                        &nbsp;</td>
                                    <td style="width: 5%">
                                        &nbsp;</td>
                                </tr>
                                <tr>
                                    <td style="width: 5%">
                                        &nbsp;</td>
                                    <td style="width: 90%">
                                        <fieldset style="vertical-align: top; border: solid 1px #8BB381;">
                                            <legend style="color: maroon;"><b>Search Option</b> </legend>
                                            <table style="width:100%;">
                                                <tr>
                                                    <td>
                                                        <div align="right" style="width:18;height:20px; float: left;">
                                                        </div>
                                                        <div align="right" style="width:20%;height:20px; float: left;">
                                                            Branch Name</div>
                                                        <div style="width:5%;height:20px; float: left;">
                                                        </div>
                                                        <div style="width:40%;height:20px; float: left;">
                                                            <asp:DropDownList ID="ddlBranchSearch" runat="server" Width="100%">
                                                            </asp:DropDownList>
                                                        </div>
                                                        <div style="width:100%;height:10px; float: left;">
                                                        </div>
                                                        <div align="right" style="width:20%; height:20px;float: left;">
                                                            <asp:Label ID="Label4" runat="server" Text="Search Date/Code"></asp:Label>
                                                        </div>
                                                        <div style="width:5%;height:20px; float: left;">
                                                        </div>
                                                        <div style="width:40%;height:20px; float: left;">
                                                            <asp:TextBox ID="txtSearchBydateCode" runat="server" Height="26px" 
                                                                placeholder="Search ** Transfer Date OR Code..." 
                                                                style="width: 100%; text-indent: 15px; display: inline-block; border: 1px solid #ccc; border-radius: 4px; box-sizing: border-box; background: transparent !important;" 
                                                                Width="100%"></asp:TextBox>
                                                            <ajaxToolkit:AutoCompleteExtender ID="AutoCompleteExtender3" runat="server" 
                                                                CompletionSetCount="12" DelimiterCharacters="" Enabled="True" 
                                                                MinimumPrefixLength="1" ServiceMethod="GetSearchTransferDateOrCode" 
                                                                ServicePath="AutoComplete.asmx" TargetControlID="txtSearchBydateCode">
                                                            </ajaxToolkit:AutoCompleteExtender>
                                                        </div>
                                                        <div align="center" style="width:15%;height:20px; float: left;">
                                                            <asp:Button ID="btnSearch" runat="server" Height="25px" 
                                                                onclick="btnSearch_Click" Text="Search" Width="90px" />
                                                        </div>
                                                        <div align="center" style="width:15%;height:20px; float: left;">
                                                            <asp:Button ID="btnClearSearch" runat="server" Height="25px" 
                                                                onclick="btnClearSearch_Click" Text="Clear" Width="90px" />
                                                        </div>
                                                        <div style="width:100%;height:10px; float: left;">
                                                        </div>
                                <br />
                                                    </td>
                                                </tr>
                                            </table>
                                        </fieldset>
                                    </td>
                                    <td style="width: 5%">
                                        &nbsp;</td>
                                </tr>
                                <tr>
                                    <td style="width: 5%">
                                        &nbsp;</td>
                                    <td style="width: 90%">
                                        <asp:GridView ID="dgHistory" runat="server" AllowPaging="True" 
                                            AutoGenerateColumns="False" CssClass="mGrid" 
                                            OnPageIndexChanging="dgHistory_PageIndexChanging" 
                                            OnRowDataBound="dgSundousBranch_RowDataBound" 
                                            OnSelectedIndexChanged="dgSundousBranch_SelectedIndexChanged" 
                                            style="text-align:center;" Width="100%">
                                            <Columns>
                                                <asp:CommandField ShowSelectButton="True">
                                                <ItemStyle Font-Bold="True" HorizontalAlign="Center" Width="5%" />
                                                </asp:CommandField>
                                                <asp:BoundField DataField="ID" HeaderText="ID">
                                                <ItemStyle HorizontalAlign="Center" Width="5%" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Code" HeaderText="Code">
                                                <ItemStyle HorizontalAlign="Center" Width="8%" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="BranchName" HeaderText="Branch Name">
                                                <ItemStyle HorizontalAlign="Left" Width="12%" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="TransferDate" HeaderText="Transfer Date">
                                                <ItemStyle HorizontalAlign="Center" Width="8%" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Qty" HeaderText="Transfer Qty.">
                                                <ItemStyle HorizontalAlign="Right" Width="8%" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Remark" HeaderText="Remark">
                                                <ItemStyle HorizontalAlign="Left" Width="30%" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Status" HeaderText="Status">
                                                <ItemStyle Font-Bold="True" Font-Size="Small" HorizontalAlign="Center" 
                                                    Width="10%" />
                                                </asp:BoundField>
                                            </Columns>
                                        </asp:GridView>
                                    </td>
                                    <td style="width: 5%">
                                        &nbsp;</td>
                                </tr>
                            </table>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                </ajaxToolkit:TabContainer>
            </asp:Panel>
        </td>
        <td style="width:5%">&nbsp;</td>
    </tr>
    <tr>
        <td style="width:05%">
        
            &nbsp;</td>
        <td style="width:90%" align="center">
            <asp:Panel ID="LoginPanel1"  runat="server" CssClass="modalPopup" 
                Style="display: none;  padding: 15px 15px 15px 15px; background-color: White; border: 1px solid black;" 
                Width="90%">
                <table style="width: 100%">
                    <tr>
                        <td colspan="4" align="left">
                            <rsweb:ReportViewer ID="ReportViewer1" runat="server" Width="90%">
                            </rsweb:ReportViewer>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 20%">
                        </td>
                       
                        <td style="width: 20%; text-align:center;">
                            <asp:Button ID="CancelBtn" runat="server" Font-Size="8pt" BorderWidth="1px" BorderStyle="Outset" Height="30px"
                                OnClick="CancelBtn_Click" OnClientClick="HideModalDiv();" Text="Cancel" 
                                Width="80px" />
                                
                        </td>
                         <td style="width: 20%">
                         <asp:Button runat="server" Font-Size="8pt"   Text="Bill Print" 
                                BorderWidth="1px" BorderStyle="Outset" Height="30px" 
   ToolTip="Print" Width="95px" ID="btnBillPrintr" onclick="btnBillPrintr_Click"></asp:Button>
                        </td>
                        <td align="left" style="width: 50%">
                            <asp:Button runat="server" Font-Size="8pt"   Text="Challan Print" 
                                BorderWidth="1px" BorderStyle="Outset" Height="30px" 
   ToolTip="Print" Width="95px" ID="btnChallan" 
    OnClick="btnChallan_Click" ></asp:Button>

                                    
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </td>
        <td style="width:5%">&nbsp;</td>
    </tr>
    <tr>
        <td style="width:05%">
        
            &nbsp;</td>
        <td style="width:90%">
              <asp:Panel ID="pnlVoucher" runat="server"  CssClass="modalPopup" 
                            Style=" display:none; background-color: White; width:800px; height:auto; ">
                            <fieldset style=" text-align:left; vertical-align: top; border: solid 1px #8BB381;line-height:1.5em;"><legend style="color: maroon;"><b>View Voucher Information</b></legend>
                            <asp:Label runat="server" ID="lblPartuculars"></asp:Label>
                                 
                                <br/>
                                <asp:button id="btnCancel" runat="server" Text="Cancel" Font-Bold="True" OnClientClick="HideModalDiv();" 
                                Font-Size="20pt" />
                            </fieldset>
                        </asp:Panel></td>
        <td style="width:5%">&nbsp;</td>
    </tr>
    <tr>
        <td style="width:05%">
        
            &nbsp;</td>
        <td style="width:90%">
            &nbsp;</td>
        <td style="width:5%">&nbsp;</td>
    </tr>
    </table> 
     
 </div>

 <div id="divBackground" style="position: fixed; z-index: 999; height: 100%; width: 100%;
        top: 0; left:0; background-color: white; filter: alpha(opacity=60); opacity: 0.6; -moz-opacity: 0.8;-webkit-opacity: 0.8;display:none">
    </div>
</asp:Content>

