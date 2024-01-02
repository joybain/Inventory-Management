<%@ Page Title="Employee Report" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="frmEmployeeReport.aspx.cs" Inherits="frmEmployeeReport" Theme="Themes" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxtoolkit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
     <script src='<%= ResolveUrl("~/Scripts/valideDate.js") %>' type="text/javascript"></script>
     <script language="javascript" type="text/javascript">
    function onListPopulated() {
        var completionList1 = $find("AutoCompleteEx1").get_completionList();
        completionList1.style.width = 'auto';
        var completionList2 = $find("AutoCompleteEx2").get_completionList();
        completionList2.style.width = 'auto';
        var completionList3 = $find("AutoCompleteEx3").get_completionList();
        completionList3.style.width = 'auto';
        var completionList4 = $find("AutoCompleteEx4").get_completionList();
        completionList4.style.width = 'auto';
        var completionList5 = $find("AutoCompleteEx5").get_completionList();
        completionList5.style.width = 'auto';
        var completionList6 = $find("AutoCompleteEx6").get_completionList();
        completionList6.style.width = 'auto';
        var completionList7 = $find("AutoCompleteEx7").get_completionList();
        completionList7.style.width = 'auto';
        var completionList8 = $find("AutoCompleteEx8").get_completionList();
        completionList8.style.width = 'auto';
        var completionList7 = $find("AutoCompleteEx9").get_completionList();
        completionList7.style.width = 'auto';
        var completionList8 = $find("AutoCompleteEx10").get_completionList();
        completionList8.style.width = 'auto';
    }
    </script>

    <div id="frmMainDiv" style="width:98.5%; background-color:transparent; padding:10px; height: auto !important;">

        <table style="width: 100%; padding-left: 5px; background-color:white;">
            <tr>
                <td style="width:20%;"></td>
                <td style="width: 60%; vertical-align: top;" align="center">
                    <table style="border: solid 1px gray; height: 100%; vertical-align: top;">
                        <tr>
                            <td colspan="5" style="width: 100%; height:30px;" align="center">
                                <b>Report Paraform </b>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 20%;" align="right">
                                <asp:Label ID="lblRepType" runat="server" Font-Size="8pt">Report Type</asp:Label></td>
                            <td style="width: 80%; vertical-align: middle;" align="left" colspan="4">
                                <asp:DropDownList SkinID="ddlPlain" ID="ddlRepType" runat="server" Width="100%"
                                    AutoPostBack="False" Font-Size="8pt"
                                    OnSelectedIndexChanged="ddlRepType_SelectedIndexChanged">
                                    <asp:ListItem Text="------------------Select Report-------------------" Value=""></asp:ListItem>
                                    <asp:ListItem Text="All Employee List" Value="AEL"></asp:ListItem>
                                    <asp:ListItem Text="Branch" Value="BR"></asp:ListItem>
                                    <asp:ListItem Text="Designation" Value="Des"></asp:ListItem>
                                    <asp:ListItem Text="Employee Type" Value="ET"></asp:ListItem>
                                    <asp:ListItem Text="Joining Date" Value="JD"></asp:ListItem>
                                    <asp:ListItem Value="OT">Others</asp:ListItem>
                                    

                                </asp:DropDownList></td>
                        </tr> 
                        <tr>
                             <td style="width: 20%;" align="right">
                                <asp:Label ID="Label4" runat="server" Font-Size="8pt" Width="100%">Branch Name</asp:Label>
                            </td>
                            <td style="width: 80%; vertical-align: middle;" align="left" colspan="4">
                                <asp:DropDownList SkinID="ddlPlain" ID="ddlBranchKey" runat="server" 
                                    Width="100%" Font-Size="8pt" 
                                    OnSelectedIndexChanged="ddlBranchKey_SelectedIndexChanged">
                                </asp:DropDownList>
                            </td>
                        </tr> 
                        <tr>
                             <td style="width: 20%;" align="right">
                                <asp:Label ID="Label1" runat="server" Font-Size="8pt" Width="100%">Designation Name</asp:Label>
                            </td>
                            <td style="width: 80%; vertical-align: middle;" align="left" colspan="4">
                                <asp:DropDownList SkinID="ddlPlain" ID="DesignationDrropDownList" 
                                    runat="server" Width="100%" Font-Size="8pt" 
                                    OnSelectedIndexChanged="ddlBranchKey_SelectedIndexChanged">
                                </asp:DropDownList>
                            </td>
                        </tr>   
                        <tr>
                             <td style="width: 20%;" align="right">
                                <asp:Label ID="Label2" runat="server" Font-Size="8pt" Width="100%">Employee Type</asp:Label>
                            </td>
                            <td style="width: 80%; vertical-align: middle;" align="left" colspan="4">
                                <asp:DropDownList SkinID="ddlPlain" ID="EmployeeTypeDrropDownList" 
                                    runat="server" Width="100%" Font-Size="8pt" 
                                    OnSelectedIndexChanged="ddlBranchKey_SelectedIndexChanged">
                                    <asp:ListItem>All</asp:ListItem>
                                    <asp:ListItem>Desk</asp:ListItem>
                                    <asp:ListItem >Marketing</asp:ListItem>
                                    <asp:ListItem>Desk &amp; Marketing</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>              
                        <tr>
                             <td style="width: 20%;" align="right">
                                 Joining Date Range</td>
                            <td style="width: 80%; vertical-align: middle;" align="left" colspan="4" 
                                 valign="top">
                                &nbsp;Start Date :<asp:TextBox ID="StartDtateTextBox" runat="server"></asp:TextBox>
                                <ajaxtoolkit:CalendarExtender ID="StartDate" TargetControlID="StartDtateTextBox" runat="server"></ajaxtoolkit:CalendarExtender>
                                &nbsp; End Date :<asp:TextBox ID="EndDateTextBox" runat="server"></asp:TextBox>
                                <ajaxtoolkit:CalendarExtender ID="EndDate" TargetControlID="EndDateTextBox" runat="server"></ajaxtoolkit:CalendarExtender>
                            </td>
                        </tr>          
                        <%--<tr>
                            <td style="width: 20%; " align="left">
                                <asp:Label ID="Label9" runat="server" Font-Size="8pt">Year</asp:Label></td>
                            <td style="width: 25%; " align="left">
                                <asp:TextBox SkinID="tbGray" ID="txtYear"  CssClass="tbc"
            runat="server" Width="100%" AutoPostBack="false" Font-Size="8pt" 
            MaxLength="4" ></asp:TextBox>
                            </td>
                            <td style="width:5%;"></td>
                            <td style="width: 20%; " align="left">
                               </td>
                            <td style="width: 25%; " align="left">
                                
                            </td>
                        </tr>--%>
                        <tr>
                            <td colspan="2" align="center">
                                <asp:Button ID="lbRunReport" runat="server" ToolTip="Run Report" OnClick="lbRunReport_Click"
                                    Height="25px" Width="100px" BorderStyle="Outset" Text="Run Report" />
                            </td>
                            <td style="width: 5%;"></td>
                            <td colspan="2" align="center">
                                <asp:Button ID="lbReset" runat="server" ToolTip="Clear" OnClick="lbReset_Click"
                                    Height="25px" Width="100px" BorderStyle="Outset" Text="Reset" />
                            </td>
                        </tr>   
                        
                    </table>
                </td>
                <td style="width:20%;"></td>
            </tr>
        </table>
        <!--
        <div class="loading" align="center">
            Loading. Please wait.<br />
            <br />
            <img src="img/loading.gif" alt="" />
        </div>
        -->
        <table>
            <tr>
                <td colspan="3" style="width: 100%; text-align: center"></td>
            </tr>
            <tr>
                <td colspan="3" style="width: 100%; text-align: center"></td>
            </tr>
        </table>

    </div>
</asp:Content>