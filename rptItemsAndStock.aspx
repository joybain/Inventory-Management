<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="rptItemsAndStock.aspx.cs" Inherits="rptItemsAndStock" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    
     <div id="frmMainDiv"  style="width:98.5%; background-color:white;">

        <table style="width: 100%; background-color: transparent">
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
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 25%" valign="top">
                                &nbsp;</td>
                            <td style="width: 75%" valign="top">
                                &nbsp;</td>
                        </tr>
                        <tr>
                            <td style="width: 25%" valign="top">
                                <fieldset style="vertical-align: top;  border: solid .5px #8BB381;text-align:left;line-height:1.5em;">
                                <legend style="color: maroon;"><b>Report Criteria</b></legend>
                            <asp:UpdatePanel runat="server" UpdateMode="Conditional" ID="UP1">
                                <ContentTemplate>
                                    <asp:RadioButtonList ID="rbAttType" runat="server" AutoPostBack="True" OnSelectedIndexChanged="rbAttType_SelectedIndexChanged">
                                        <asp:ListItem Value="1" Selected="True">Items Details</asp:ListItem>
                                        <%--<asp:ListItem Value="2">Monthly </asp:ListItem>
                                        <asp:ListItem Value="3">Periodically</asp:ListItem>--%>
                                        <asp:ListItem Value="4">Stock Details</asp:ListItem>
                                    </asp:RadioButtonList>
                                </ContentTemplate>
                                    </asp:UpdatePanel>
                                </fieldset>
                            </td>
                            <td style="width: 75%" valign="top">
                                <fieldset style="vertical-align: top;  border: solid .5px #8BB381;text-align:left;line-height:1.5em;">
                                <legend style="color: maroon;"><b>Item&#39;s &amp; Stock Report</b></legend>
                            <asp:UpdatePanel runat="server" UpdateMode="Conditional" ID="UP2">
                            <ContentTemplate>
                                <table style="width: 100%">
                                    <tr>
                                        <td style="width: 10%; height: 29px;">
                                            <asp:Label ID="lblagency" runat="server" class="control-label mt-2" for="name" 
                                                style="font-weight: 700">Department</asp:Label>
                                        </td>
                                        <td align="center" style="width: 2%; height: 29px;">
                                            </td>
                                        <td colspan="4" style="height: 29px">
                                            <asp:DropDownList ID="ddlDepart" runat="server" AutoPostBack="True" 
                                                class="form-control dropdown-select" Height="26px" 
                                                Width="98%" onselectedindexchanged="ddlDepart_SelectedIndexChanged">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 10%; height: 28px;">
                                            <asp:label ID="lblProject" runat="server" class="control-label mt-2" 
                                                for="name" style="font-weight: 700">Search Category
                                            </asp:label>
                                        </td>
                                        <td style="width: 2%; height: 28px;" align="center">
                                            </td>
                                        <td colspan="4" style="height: 28px">
                                            <asp:HiddenField ID="hfCatagoryID" runat="server" />
                                            <asp:TextBox ID="txtCatagory" runat="server" AutoPostBack="True" Height="26px" 
                                                ontextchanged="txtCatagory_TextChanged" placeholder="Search Category.." 
                                                style="text-indent: 15px; display: inline-block; border: 1px solid #ccc; border-radius: 4px; box-sizing: border-box; background: transparent !important;" 
                                                Width="100%"></asp:TextBox>
                                            <ajaxToolkit:AutoCompleteExtender ID="txtName1_AutoCompleteExtender" 
                                                runat="server" CompletionSetCount="12" DelimiterCharacters="" Enabled="True" 
                                                MinimumPrefixLength="1" ServiceMethod="GetItemCatagorySearchAllWithDepartment" 
                                                ServicePath="AutoComplete.asmx" TargetControlID="txtCatagory" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 10%; height: 28px;">
                                            <label ID="lblCluster" runat="server" class="control-label mt-2" for="name">
                                            Search Sub Category
                                            </label>
                                        </td>
                                        <td style="width: 2%; height: 28px;" align="center">
                                            </td>
                                        <td colspan="4" style="height: 28px">
                                            <asp:HiddenField ID="hfSubCatagoryID" runat="server" />
                                            <asp:TextBox ID="txtSubCatagory" runat="server" AutoPostBack="True" 
                                                Height="26px" ontextchanged="txtSubCatagory_TextChanged" 
                                                placeholder="Search Sub Category.." 
                                                style="text-indent: 15px; display: inline-block; border: 1px solid #ccc; border-radius: 4px; box-sizing: border-box; background: transparent !important;" 
                                                Width="100%"></asp:TextBox>
                                            <ajaxToolkit:AutoCompleteExtender ID="txtName2_AutoCompleteExtender" 
                                                runat="server" CompletionSetCount="12" DelimiterCharacters="" Enabled="True" 
                                                MinimumPrefixLength="1" ServiceMethod="GetItemSubCatagorySearchAll" 
                                                ServicePath="AutoComplete.asmx" TargetControlID="txtSubCatagory" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 10%; font-weight: 700; height: 28px;">
                                            Search Brand</td>
                                        <td align="center" style="width: 2%; height: 28px;">
                                            </td>
                                        <td colspan="4" style="height: 28px">
                                            <asp:HiddenField ID="hfBrand" runat="server" />
                                            <asp:TextBox ID="txtBrand" runat="server" AutoPostBack="True" Height="26px" 
                                                ontextchanged="txtBrand_TextChanged" placeholder="Search Brand.." 
                                                style="text-indent: 15px; display: inline-block; border: 1px solid #ccc; border-radius: 4px; box-sizing: border-box; background: transparent !important;" 
                                                Width="100%"></asp:TextBox>
                                            <ajaxToolkit:AutoCompleteExtender ID="txtName3_AutoCompleteExtender" 
                                                runat="server" CompletionSetCount="12" DelimiterCharacters="" Enabled="True" 
                                                MinimumPrefixLength="1" ServiceMethod="GetItemBrandSearchAll" 
                                                ServicePath="AutoComplete.asmx" TargetControlID="txtBrand" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 10%; height: 30px;">
                                            <label ID="lblEmployeeName" runat="server" class="control-label mt-2" for="name">
                                            Search Item&#39;s</label></td>
                                        <td style="width: 2%; height: 30px;" align="center">
                                            </td>
                                        <td colspan="4" style="height: 30px">
                                            <asp:HiddenField ID="hfItemsID" runat="server" />
                                            <asp:TextBox ID="txtName" runat="server" 
                                                AutoPostBack="True" 
                                                OnTextChanged="txtName_TextChanged" placeHolder="Search Code/Name.." 
                                                Width="100%" Height="26px" 
                                                style="text-indent: 15px; display: inline-block; border: 1px solid #ccc; border-radius: 4px; box-sizing: border-box; background: transparent !important;"></asp:TextBox>
                                            <ajaxToolkit:AutoCompleteExtender ID="txtName_AutoCompleteExtender" 
                                                runat="server" CompletionSetCount="12" MinimumPrefixLength="1" ServiceMethod="GetItemSearchAll" 
                                                ServicePath="AutoComplete.asmx" TargetControlID="txtName" 
                                                DelimiterCharacters="" Enabled="True">
                                            </ajaxToolkit:AutoCompleteExtender>
                                        </td>
                                    </tr>
                                    <tr>
                                        <div id="lblDate" runat="server">
                                        <td style="width: 10%; height: 28px;">
                                            <asp:Label ID="lblFormDate" runat="server" class="control-label" for="emp_id">Form 
                                            Date :</asp:Label>
                                        </td>
                                        <td style="width: 2%; height: 28px;" align="center">
                                            &nbsp;</td>
                                        <td style="width: 15%; height: 28px;">
                                            <asp:TextBox ID="txtFormDate" runat="server" placeHolder="dd/MM/yyyy" 
                                                Width="93%" CssClass="txtVisibleAlign" Height="26px"></asp:TextBox>
                                            <ajaxToolkit:CalendarExtender ID="txtFormDate_CalendarExtender" runat="server" 
                                                Format="dd/MM/yyyy" TargetControlID="txtFormDate" />
                                        </td>
                                        <td style="width: 10%; height: 28px;" align="right">
                                            <asp:Label ID="lblToDate" runat="server" class="control-label" for="emp_id">To 
                                            Date :<span></span></asp:Label>
                                        </td>
                                        <td style="width: 2%; height: 28px;" align="center">
                                            </td>
                                        <td style="width: 15%; height: 28px;">
                                            <asp:TextBox ID="txtToDate" runat="server" placeHolder="dd/MM/yyyy" Width="93%" 
                                                CssClass="txtVisibleAlign" Height="26px"></asp:TextBox>
                                            <ajaxToolkit:CalendarExtender ID="txtToDate_CalendarExtender" runat="server" 
                                                Format="dd/MM/yyyy" TargetControlID="txtToDate" />
                                        </td>
                                        </div>
                                    </tr>
                                    <tr>
                                        <td style="width: 10%;">
                                            &nbsp;</td>
                                        <td align="center" style="width: 2%;">
                                            &nbsp;</td>
                                        <td style="width: 15%;">
                                            &nbsp;</td>
                                        <td align="right" style="width: 10%;">
                                            &nbsp;</td>
                                        <td align="center" style="width: 2%;">
                                            &nbsp;</td>
                                        <td style="width: 15%;">
                                            &nbsp;</td>
                                    </tr>
                                </table>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                                </fieldset>
                            </td>
                        </tr>
                    </table>
                </td>
                <td style="width: 5%">
                    &nbsp;</td>
            </tr>
            <tr>
                <td style="width: 5%">
                    &nbsp;</td>
                <td style="width: 90%" align="center">
                    <fieldset style="vertical-align: top;  border: solid .5px #8BB381;text-align:left;line-height:1.5em;">
                    
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 20%">
                                &nbsp;</td>
                            <td style="width: 10%">
                                &nbsp;</td>
                            <td style="width: 10%">
                                &nbsp;</td>
                            <td style="width: 10%" align="center">
            <asp:Button ID="btnPrint" runat="server" BorderStyle="Outset" Height="35px"  Text="Print" 
                                    ToolTip="Print" OnClick="btnPrint_Click" Width="95%" />
                            </td>
                            <td style="width: 10%" align="center">
            <asp:Button ID="btnClear" runat="server" BorderStyle="Outset" Height="35px"  Text="Clear" 
                                    OnClick="btnClear_Click" Width="95%" />
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
                <td style="width: 90%" align="center">
                    &nbsp;</td>
                <td style="width: 5%">
                    &nbsp;</td>
            </tr>
            <tr>
                <td style="text-align: center;" colspan="3">
                    </td>
            </tr>
        </table>

    </div>

</asp:Content>

