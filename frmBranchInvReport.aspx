<%@ Page Title="" Language="C#" MasterPageFile="~/BranchMasterPage.master" AutoEventWireup="true" CodeFile="frmBranchInvReport.aspx.cs" Inherits="frmBranchInvReport" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <div style="background-color:White; width:100%; min-height:700px; height:auto !important; height:700px; font-family:Tahoma;">
        
   <table style="width: 100%">
                
        <tr>
            <td style="width:10%; height: 50px;"></td>
            <td style="height: 50px; width: 80%;" align="right">
                <asp:ImageButton ID="ibHelp" Width="40px" runat="server" 
                    ImageUrl="~/Help/Help_Desk.jpg" onclick="ibHelp_Click" />
            </td>
            <td style="width:10%; height: 50px;"></td>
        </tr>
        
        <tr>
            <td style="width:10%;">&nbsp;</td>
            <td style="width: 80%">
                <asp:UpdatePanel ID="UPInvReport" runat="server" UpdateMode="Conditional">
                <ContentTemplate>                 
               <fieldset style="vertical-align: top; border: solid 1px #8BB381;"><legend style="color: maroon;"><b>Inventory Report </b></legend>
                <table style="width: 100%">
                    <tr>
                        <td style="height: 15px" valign="top" align="left">
                      <%--  <fieldset style="border:1px solid lightgray;">--%>
                           
                            <asp:RadioButtonList ID="RadioButtonList1" runat="server" AutoPostBack="True" 
                                onselectedindexchanged="RadioButtonList1_SelectedIndexChanged" 
                                RepeatDirection="Horizontal" Font-Bold="True" Font-Size="10pt" 
                                BorderStyle="Solid">
                             <%--   <asp:ListItem Value="Sales">Sales</asp:ListItem>--%>
                                <asp:ListItem Value="Purchase">Purchase</asp:ListItem>
                                <%--<asp:ListItem Value="IS">Item Stock</asp:ListItem>
                                <asp:ListItem Value="TIQ">Total Items Quantity</asp:ListItem>--%>
                               <%-- <asp:ListItem Value="TI">Transfer Item</asp:ListItem>
                                <asp:ListItem Value="SWIS">Shipment Wise Item Status</asp:ListItem>
                                <asp:ListItem Value="DSS">Dmage/Short Stock</asp:ListItem>--%>
                                <asp:ListItem Value="Sales">Sales</asp:ListItem>
                                <asp:ListItem Value="POL">Profit Or Loss</asp:ListItem>
                                <asp:ListItem Value="CPI">Customer Payment Info</asp:ListItem>
                                <asp:ListItem Value="CD">Customer Due</asp:ListItem>
                                <%--<asp:ListItem Value="SD">Supplier Due</asp:ListItem>--%>
                                <asp:ListItem Value="DSSR">Daily Sales Summery</asp:ListItem>
                                <asp:ListItem Value="DE">Daily Expanse</asp:ListItem>
                            </asp:RadioButtonList>
                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                            <%-- </fieldset>--%>
                        </td>
                    </tr>
                    <tr>
                        <td style="height: 15px" valign="top">
                            <%--Sales & Purchase Panel--%>
                            <asp:Panel ID="PnlSales" runat="server">
                                <table style="width: 99%;border:1px solid lightgray;">
                                    <tr>
                                        <td align="left" style="width: 226px">
                                            <asp:RadioButtonList ID="rbReportType" runat="server" BorderStyle="Solid" 
                                                RepeatDirection="Horizontal" Visible="False">
                                                <asp:ListItem Selected="True" Value="1">BD</asp:ListItem>
                                                <asp:ListItem Value="2">PH</asp:ListItem>
                                            </asp:RadioButtonList>
                                            &nbsp;</td>
                                        <td style="width: 51px">
                                            &nbsp;</td>
                                        <td align="center" style="width: 182px">
                                            <asp:RadioButtonList ID="rbPrintType" runat="server" BorderStyle="Solid" 
                                                RepeatDirection="Horizontal">
                                                <asp:ListItem Selected="True" Value="P">PDF</asp:ListItem>
                                                <asp:ListItem Value="E">Excel</asp:ListItem>
                                            </asp:RadioButtonList>
                                            <asp:DropDownList ID="ddlSupplier" runat="server" Height="27px" Width="60%">
                                            </asp:DropDownList>
                                        </td>
                                        <td style="width: 54px">
                                            &nbsp;</td>
                                        <td>
                                            <asp:RadioButtonList ID="rbItemWise" runat="server" BorderStyle="Solid" 
                                                RepeatDirection="Horizontal" Visible="False">
                                                <asp:ListItem Value="Item">Item Wise</asp:ListItem>
                                            </asp:RadioButtonList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center" style="width: 226px">
                                            <asp:RadioButton ID="rbCurDate" runat="server" AutoPostBack="True" 
                                                oncheckedchanged="rbCurDate_CheckedChanged" />
                                        </td>
                                        <td style="width: 51px">
                                            &nbsp;</td>
                                        <td align="center" style="width: 182px">
                                            <asp:RadioButton ID="rbByDate" runat="server" AutoPostBack="True" 
                                                oncheckedchanged="rbByDate_CheckedChanged" Text="By Date" />
                                        </td>
                                        <td style="width: 54px">
                                            &nbsp;</td>
                                        <td>
                                            &nbsp;</td>
                                    </tr>
                                    <tr>
                                        <td align="center" style="width: 226px; height: 4px;">
                                        </td>
                                        <td style="width: 51px; height: 4px;">
                                        </td>
                                        <td align="center" style="width: 182px; height: 4px;">
                                        </td>
                                        <td style="width: 54px; height: 4px;">
                                        </td>
                                        <td style="height: 4px">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right" style="width: 226px">
                                            <asp:TextBox ID="txtStartDate" runat="server" autocomplete="off" placeholder=" Date From ( dd/MM/yyyy )" style="text-align:center;"
                                                Width="90%"></asp:TextBox>
                                            <cc:CalendarExtender ID="txtStartDate_CalendarExtender" runat="server" 
                                                Format="dd/MM/yyyy" TargetControlID="txtStartDate" />
                                        </td>
                                        <td align="center" style="width: 51px">
                                            <asp:Label ID="lblTo" runat="server" style="font-weight: 700"></asp:Label>
                                        </td>
                                        <td align="left" style="width: 182px">
                                            <asp:TextBox ID="txtEndDate" runat="server" style="text-align:center;" autocomplete="off"
                                                placeholder=" Date To ( dd/MM/yyyy )" Width="100%"></asp:TextBox>
                                            <cc:CalendarExtender ID="Calendarextender1" runat="server" Format="dd/MM/yyyy" 
                                                TargetControlID="txtEndDate" />
                                        </td>
                                        <td style="width: 54px">
                                            &nbsp;</td>
                                        <td>
                                            &nbsp;</td>
                                    </tr>
                                    <tr>
                                        <td align="right" style="width: 226px; height: 3px;">
                                        </td>
                                        <td align="center" style="width: 51px; height: 3px;">
                                        </td>
                                        <td align="left" style="width: 182px; height: 3px;">
                                        </td>
                                        <td style="width: 54px; height: 3px;">
                                        </td>
                                        <td style="height: 3px">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" colspan="5">
                                            <table style="width: 100%">
                                                <tr>
                                                    <td  style="width: 22%">
                                                        <asp:Label ID="lblCustomeSupplier" runat="server" style="font-weight: 700"></asp:Label>
                                                        <asp:HiddenField ID="hfCustomerID" runat="server" />
                                                    </td>
                                                    <td style="width: 50%" valign="middle">
                                                        <asp:TextBox ID="txtCustomer" runat="server" AutoPostBack="True" 
                                                            CssClass="txtVisibleAlign" Height="18px" 
                                                            ontextchanged="txtCustomer_TextChanged" placeHolder="Search By Customer." 
                                                            style="text-align:left;" TabIndex="11" Width="100%"></asp:TextBox>
                                                        <cc:AutoCompleteExtender ID="AutoCompleteExtender2" runat="server" 
                                                            CompletionInterval="20" CompletionSetCount="12" EnableCaching="true" 
                                                            MinimumPrefixLength="1" ServiceMethod="GetBranchCustomername" 
                                                            ServicePath="AutoComplete.asmx" TargetControlID="txtCustomer" />
                                                        <asp:TextBox ID="txtSupplierSearch" runat="server" AutoPostBack="True" 
                                                            CssClass="txtVisibleAlign" Height="18px" 
                                                            ontextchanged="txtSupplierSearch_TextChanged" 
                                                            placeholder="Search By Supplier.." Visible="False" Width="100%"></asp:TextBox>
                                                        <asp:TextBox ID="txtBranchID" runat="server" AutoPostBack="True" 
                                                            CssClass="txtVisibleAlign" Height="18px" placeholder="Search By Supplier.." 
                                                            Visible="False" Width="100%"></asp:TextBox>
                                                        <cc:AutoCompleteExtender ID="txtBranchID_AutoCompleteExtender" 
                                                            runat="server" CompletionInterval="20" CompletionSetCount="12" 
                                                            EnableCaching="true" MinimumPrefixLength="1" ServiceMethod="GetSupplier" 
                                                            ServicePath="AutoComplete.asmx" TargetControlID="txtBranchID" />
                                                        <cc:AutoCompleteExtender ID="txtSupplierSearch_AutoCompleteExtender" 
                                                            runat="server" CompletionInterval="20" CompletionSetCount="12" 
                                                            EnableCaching="true" MinimumPrefixLength="1" ServiceMethod="GetSupplier" 
                                                            ServicePath="AutoComplete.asmx" TargetControlID="txtSupplierSearch" />
                                                    </td>
                                                    <td style="width: 5%">
                                                        &nbsp;</td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right" style="width: 226px; height: 3px;">
                                            &nbsp;</td>
                                        <td align="center" style="width: 51px; height: 3px;">
                                            &nbsp;</td>
                                        <td align="left" style="width: 182px; height: 3px;">
                                            &nbsp;</td>
                                        <td style="width: 54px; height: 3px;">
                                            &nbsp;</td>
                                        <td style="height: 3px">
                                            &nbsp;</td>
                                    </tr>
                                </table>
                            </asp:Panel>

                              <%-- Shipment Wise Items Status  --%>
                             <asp:Panel ID="pnlItemsStatus" runat="server">
                                 <table style="width: 99%;border:1px solid lightgray;">
                                    <tr>
                                        <td style="width:10%"></td>
                                        <td style="width:2%" align="center"></td>
                                        <td style="width:15%"></td>
                                        <td style="width:10%"></td>
                                        <td style="width:2%"></td>
                                        <td style="width:15%"></td>
                                    </tr> 
                                     <tr>
                                         <td style="width:10%; font-weight: 700; height: 31px;">
                                             Search Shipment<asp:Label ID="lblShiftmentID" runat="server" Visible="False"></asp:Label>
                                         </td>
                                         <td align="center" style="width:2%; height: 31px;">
                                             <b>:</b></td>
                                         <td colspan="4" style="height: 31px;">
                                             <asp:TextBox ID="txtShiftmentNo" runat="server" AutoPostBack="True" 
                                                 CssClass="txtVisibleAlign" Font-Size="8pt" Height="22px" 
                                                 ontextchanged="txtShiftmentNo_TextChanged" PlaceHoder="Search By Shiftment No." 
                                                 PlaceHolder="Search By Shipment No." SkinId="tbPlain" style="text-align:left;" 
                                                 Width="95%"></asp:TextBox>
                                             <cc:AutoCompleteExtender ID="AutoCompleteExtender3" runat="server" 
                                                 CompletionInterval="20" CompletionSetCount="30" EnableCaching="true" 
                                                 MinimumPrefixLength="2" ServiceMethod="GetShiftmentInfo" 
                                                 ServicePath="~/AutoComplete.asmx" TargetControlID="txtShiftmentNo">
                                             </cc:AutoCompleteExtender>
                                         </td>
                                     </tr>
                                     <tr>
                                         <td style="width:10%; font-weight: 700;">
                                             Search Items<asp:Label ID="lblItemsId" runat="server" Font-Size="9pt" 
                                                 Visible="False"></asp:Label>
                                         </td>
                                         <td align="center" style="width:2%">
                                             <b>:</b></td>
                                         <td colspan="4">
                                             <asp:TextBox ID="txtItemsName" runat="server" AutoPostBack="True" 
                                                 CssClass="txtVisibleAlign" Font-Size="8pt" Height="22px" 
                                                 ontextchanged="txtItemsName_TextChanged" PlaceHolder="Search By Item Name" 
                                                 SkinId="tbPlain" style="text-align:left;" Width="95%"></asp:TextBox>
                                             <cc:AutoCompleteExtender ID="txtPO_AutoCompleteExtender" runat="server" 
                                                 CompletionInterval="20" CompletionSetCount="30" EnableCaching="true" 
                                                 MinimumPrefixLength="2" ServiceMethod="GetShowBDItemsSearch" 
                                                 ServicePath="~/AutoComplete.asmx" TargetControlID="txtItemsName">
                                             </cc:AutoCompleteExtender>
                                         </td>
                                     </tr>
                                     <tr>
                                         <td style="width:10%">
                                             &nbsp;</td>
                                         <td align="center" style="width:2%">
                                             &nbsp;</td>
                                         <td style="width:15%">
                                             &nbsp;</td>
                                         <td style="width:10%">
                                             &nbsp;</td>
                                         <td style="width:2%">
                                             &nbsp;</td>
                                         <td style="width:15%">
                                             &nbsp;</td>
                                     </tr>
                                 </table>
                             </asp:Panel>
                            <%--      Items Stock Panel--%>
                            <asp:Panel ID="PnlItemsStock" runat="server">
                                <table style="width: 99%;border:1px solid lightgray;">
                                    <tr align="center">
                                        <td align="center" style="width: 122px">
                                            &nbsp;</td>
                                        <td style="width: 105px">
                                            <asp:RadioButton ID="rdbAll" runat="server" AutoPostBack="True" 
                                                oncheckedchanged="rdbAll_CheckedChanged" Text="All" />
                                        </td>
                                        <td align="center" style="width: 108px">
                                            <asp:RadioButton ID="rdbAvailable" runat="server" AutoPostBack="True" 
                                                oncheckedchanged="rdbAvailable_CheckedChanged" Text="Available" />
                                        </td>
                                        <td style="width: 117px">
                                            <asp:RadioButton ID="rdbNotAvailable" runat="server" AutoPostBack="True" 
                                                oncheckedchanged="rdbNotAvailable_CheckedChanged" Text="Unavailable" />
                                        </td>
                                        <td>
                                            &nbsp;</td>
                                    </tr>
                                    <tr>
                                        <td style="width: 122px">
                                            <cc:CalendarExtender ID="Calendarextender2" runat="server" Format="dd/MM/yyyy" 
                                                TargetControlID="txtStartDate" />
                                            <asp:Label ID="Label1" runat="server" Text="Category"></asp:Label>
                                        </td>
                                        <td align="center" colspan="4">
                                            <cc:CalendarExtender ID="Calendarextender3" runat="server" Format="dd/MM/yyyy" 
                                                TargetControlID="txtEndDate" />
                                            <asp:DropDownList ID="ddlCatagory" runat="server" AutoPostBack="True" 
                                                Height="26px" onselectedindexchanged="ddlCatagory_SelectedIndexChanged" 
                                                Width="100%">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 122px">
                                            <asp:Label ID="Label2" runat="server" Text="Sub Category"></asp:Label>
                                        </td>
                                        <td align="center" colspan="4">
                                            <asp:DropDownList ID="ddlSubCatagory" runat="server" Height="26px" Width="100%">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                            <%--      Items TranSaction--%>
                            <asp:Panel ID="pnlItemsTrans" runat="server">
                                <table style="width: 99%;border:1px solid lightgray;">
                                    <tr align="center">
                                        <td align="center" style="width: 122px; height: 3px;">
                                        </td>
                                        <td style="width: 105px; height: 3px;">
                                        </td>
                                        <td align="center" style="width: 108px; height: 3px;">
                                        </td>
                                        <td style="width: 117px; height: 3px;">
                                        </td>
                                        <td style="height: 3px">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 122px">
                                            <cc:CalendarExtender ID="Calendarextender4" runat="server" Format="dd/MM/yyyy" 
                                                TargetControlID="txtStartDate" />
                                            <asp:Label ID="Label3" runat="server" Text="Items Name"></asp:Label>
                                        </td>
                                        <td align="center" colspan="4">
                                            <cc:CalendarExtender ID="Calendarextender5" runat="server" Format="dd/MM/yyyy" 
                                                TargetControlID="txtEndDate" />
                                            <asp:DropDownList ID="ddlItemsName" runat="server" Height="26px" Width="100%">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 122px">
                                            <asp:Label ID="Label4" runat="server" Text="Date"></asp:Label>
                                        </td>
                                        <td align="left" colspan="4">
                                            <asp:TextBox ID="txtDate" runat="server" Width="50%"></asp:TextBox>
                                            <cc:CalendarExtender ID="txtDate_CalendarExtender" runat="server" 
                                                Format="dd/MM/yyyy" TargetControlID="txtDate" />
                                            <cc:CalendarExtender ID="txtDate_CalendarExtender2" runat="server" 
                                                Format="dd/MM/yyyy" TargetControlID="txtDate" />
                                            <cc:CalendarExtender ID="txtDate_CalendarExtender3" runat="server" 
                                                Format="dd/MM/yyyy" TargetControlID="txtDate" />
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                            <asp:Panel ID="PnlPayment" runat="server">
                                <table style="width: 99%;border:1px solid lightgray;">
                                    <tr>
                                        <td align="left" style="font-weight: 700;" colspan="4">
                                            &nbsp;</td>
                                    </tr>
                                    <tr>
                                        <td align="left" colspan="4" style="font-weight: 700;">
                                            <table style="width: 100%">
                                                <tr>
                                                    <td style="width: 20%" align="right">
                                                        Select Type</td>
                                                    <td style="width: 80%">
                                                        <asp:DropDownList ID="ddlReportType" runat="server" Width="58%">
                                                            <asp:ListItem Value="SR">Summery Report</asp:ListItem>
                                                            <asp:ListItem Value="DR">Details Report</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 51px">
                                            &nbsp;</td>
                                        <td align="center" style="width: 182px">
                                            <asp:RadioButton ID="rbCurDatePayment" runat="server" AutoPostBack="True" 
                                                oncheckedchanged="rbCurDatePayment_CheckedChanged" />
                                        </td>
                                        <td style="width: 54px">
                                            &nbsp;</td>
                                        <td>
                                            <asp:RadioButton ID="rbByDatePayment" runat="server" AutoPostBack="True" 
                                                oncheckedchanged="rbByDatePayment_CheckedChanged" Text="By Date" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 51px; height: 4px;">
                                        </td>
                                        <td align="center" style="width: 182px; height: 4px;">
                                        </td>
                                        <td style="width: 54px; height: 4px;">
                                        </td>
                                        <td style="height: 4px">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center" style="width: 51px">
                                            &nbsp;</td>
                                        <td align="left">
                                            <cc:CalendarExtender ID="Calendarextender6" runat="server" Format="dd/MM/yyyy" 
                                                TargetControlID="txtEndDate" />
                                            <asp:TextBox ID="txtStartDatePayment" runat="server" autocomplete="off" 
                                                placeholder=" Date From ( dd/MM/yyyy )" style="text-align:center;" Width="85%"></asp:TextBox>
                                        </td>
                                        <td style="width: 54px">
                                            <asp:Label ID="lblToPayment" runat="server" style="font-weight: 700">To</asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtEndDatePayment" runat="server" autocomplete="off" 
                                                placeholder=" Date To ( dd/MM/yyyy )" style="text-align:center;" 
                                                Width="50%"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center" style="width: 51px; height: 3px;">
                                        </td>
                                        <td align="left" style="width: 182px; height: 3px;">
                                        </td>
                                        <td style="width: 54px; height: 3px;">
                                        </td>
                                        <td style="height: 3px">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" colspan="4">
                                            <table style="width: 100%">
                                                <tr>
                                                    <td style="width: 20%" align="right">
                                                        
                                                        <asp:Label ID="Label5" runat="server" style="font-weight: 700" 
                                                            Text="Search Parameter"></asp:Label>
                                                    </td>
                                                    <td style="width: 50%" valign="middle">
                                                        <asp:TextBox ID="txtCustomerPayment" runat="server" AutoPostBack="True" 
                                                            CssClass="txtVisibleAlign" Height="18px" 
                                                            ontextchanged="txtCustomerPayment_TextChanged" 
                                                            placeHolder="Search By Customer." style="text-align:left;" TabIndex="11" 
                                                            Width="100%"></asp:TextBox>
                                                        <asp:TextBox ID="txtSupplierSearchDue" runat="server" AutoPostBack="True" 
                                                            CssClass="txtVisibleAlign" Height="18px" 
                                                            ontextchanged="txtSupplierSearchDue_TextChanged" 
                                                            placeholder="Search By Supplier.." Width="100%"></asp:TextBox>
                                                        <cc:AutoCompleteExtender ID="txtSupplierSearchDue_AutoCompleteExtender" 
                                                            runat="server" CompletionInterval="20" CompletionSetCount="12" 
                                                            EnableCaching="true" MinimumPrefixLength="1" ServiceMethod="GetSupplier" 
                                                            ServicePath="AutoComplete.asmx" TargetControlID="txtSupplierSearchDue" />
                                                        <cc:AutoCompleteExtender ID="AutoCompleteExtender4" runat="server" 
                                                            CompletionInterval="20" CompletionSetCount="12" EnableCaching="true" 
                                                            MinimumPrefixLength="1" ServiceMethod="GetCustomername" 
                                                            ServicePath="AutoComplete.asmx" TargetControlID="txtCustomerPayment" />
                                                    </td>
                                                    <td style="width: 70%">
                                                        <asp:HiddenField ID="hfCustomerIDPayment" runat="server" />
                                                        <asp:HiddenField ID="hfSupplierIDPayment" runat="server" />
                                                    </td>
                                                </tr>
                                            </table>
                                            <br />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center" style="width: 51px; height: 3px;">
                                            &nbsp;</td>
                                        <td align="left" style="width: 182px; height: 3px;">
                                            &nbsp;</td>
                                        <td style="width: 54px; height: 3px;">
                                            &nbsp;</td>
                                        <td style="height: 3px">
                                            &nbsp;</td>
                                    </tr>
                                </table>
                            </asp:Panel>
                            <asp:Panel ID="PnlExpanse" runat="server">
                                <table style="width: 99%;border:1px solid lightgray;">
                                    <tr>
                                        <td align="center" style="width: 226px">
                                            <asp:RadioButton ID="rbCurDateExpanse" runat="server" AutoPostBack="True" 
                                                oncheckedchanged="rbCurDateExpanse_CheckedChanged" />
                                        </td>
                                        <td style="width: 38px">
                                            &nbsp;</td>
                                        <td align="center" style="width: 182px">
                                            <asp:RadioButton ID="rbByDateExpanse" runat="server" AutoPostBack="True" 
                                                oncheckedchanged="rbByDateExpanse_CheckedChanged" Text="By Date" />
                                        </td>
                                        <td style="width: 54px">
                                            <asp:RadioButtonList ID="rbPrintTypeExpanse" runat="server" BorderStyle="Solid" 
                                                RepeatDirection="Horizontal">
                                                <asp:ListItem Selected="True" Value="P">PDF</asp:ListItem>
                                                <asp:ListItem Value="E">Excel</asp:ListItem>
                                            </asp:RadioButtonList>
                                        </td>
                                        <td>
                                            &nbsp;</td>
                                    </tr>
                                    <tr>
                                        <td align="center" style="width: 226px; height: 4px;">
                                        </td>
                                        <td style="width: 38px; height: 4px;">
                                        </td>
                                        <td align="center" style="width: 182px; height: 4px;">
                                        </td>
                                        <td style="width: 54px; height: 4px;">
                                        </td>
                                        <td style="height: 4px">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right" style="width: 226px">
                                            <asp:TextBox ID="txtStartDateExpanse" runat="server" autocomplete="off" 
                                                placeholder="Start Date" style="text-align:center;" Width="90%"></asp:TextBox>
                                            <cc:CalendarExtender ID="txtStartDate_CalendarExtender0" runat="server" 
                                                Format="dd/MM/yyyy" TargetControlID="txtStartDateExpanse" />
                                        </td>
                                        <td align="center" style="width: 38px">
                                            <asp:Label ID="lblToExpanse" runat="server"></asp:Label>
                                        </td>
                                        <td align="left" style="width: 182px">
                                            <asp:TextBox ID="txtEndDateExpanse" runat="server" autocomplete="off" 
                                                placeholder="End Date" style="text-align:center;" Width="90%"></asp:TextBox>
                                            <cc:CalendarExtender ID="Calendarextender7" runat="server" Format="dd/MM/yyyy" 
                                                TargetControlID="txtEndDateExpanse" />
                                        </td>
                                        <td style="width: 54px">
                                            &nbsp;</td>
                                        <td>
                                            &nbsp;</td>
                                    </tr>
                                    <tr>
                                        <td align="right" style="width: 226px; height: 3px;">
                                        </td>
                                        <td align="center" style="width: 38px; height: 3px;">
                                        </td>
                                        <td align="left" style="width: 182px; height: 3px;">
                                        </td>
                                        <td style="width: 54px; height: 3px;">
                                        </td>
                                        <td style="height: 3px">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" colspan="5">
                                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
                </fieldset>
                </ContentTemplate>
                </asp:UpdatePanel>
            </td>
          <%--  Popup Button Start--%>
            <td style="width:10%;" align="right">
                <asp:Panel ID="pnlChangePass" runat="server"  CssClass="modalPopup" 
                            Style=" display: none; background-color: White; width:1000px; height:580px; ">
                <asp:ImageButton ID="ibCancel" runat="server" ImageUrl="~/img/delete.png" 
                    onclick="ibCancel_Click" Width="30px" />
                <br/>
                           <asp:Literal ID="ltEmbed" runat="server" />
                        </asp:Panel>
                        <%--</ContentTemplate></asp:UpdatePanel>--%>
                         <asp:Button ID="Button1" style="display: none;" runat="server" Height="1px" Width="1px" />
                         <ajaxToolkit:ModalPopupExtender ID="ModalPopupExtenderLogin" runat="server" 
                    BackgroundCssClass="modalBackground" DropShadow="true" 
                    PopupControlID="pnlChangePass" TargetControlID="Button1" />
            </td>
              <%--  Popup Button End--%>
        </tr>
        
        <tr>
            <td style="width:10%;">&nbsp;</td>
            <td align="right" style="width: 80%">
                <asp:Button ID="btnPreview" runat="server" Text="Preview" Width="120px" 
                    Height="35px" onclick="btnPreview_Click" Visible="False" />
            &nbsp;
                <asp:Button ID="btnPrint" runat="server" Text="Print" Width="120px" 
                    Height="35px" onclick="btnPrint_Click" />
            &nbsp;&nbsp;
                <asp:Button ID="btnClear" runat="server" Text="Clear" Width="120px" 
                    onclick="btnClear_Click" Height="35px" />
            </td>
            <td style="width:10%;">&nbsp;</td>
        </tr>
        
        <tr>
            <td style="width:10%;">&nbsp;</td>
            <td align="right" style="width: 80%">&nbsp;</td>
            <td style="width:10%;">&nbsp;</td>
        </tr>
        
    </table>
    </div>
</asp:Content>

