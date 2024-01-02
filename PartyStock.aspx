<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="PartyStock.aspx.cs" Inherits="PartyStock" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">

    <div id="frmMainDiv" style="background-color:White; width:100%;">
        <table id="pageFooterWrapper">
            <tr>
                <td align="center" style="vertical-align:middle; height:100%;">
                    <asp:Button ID="DeleteButton" runat="server" CssClass="buttonclass" 
                        onclick="DeleteButton_Click" Text="Delete" Width="100px" />
                </td>
                <td align="center" style="vertical-align:middle;">
                    &nbsp;</td>
                <td align="center" style="vertical-align:middle; height:100%;">
                    <asp:Button ID="btnSave" runat="server" CssClass="buttonclass" 
                        onclick="btnSave_Click" Text="Save" Width="100px" />
                </td>
                <td align="center" style="vertical-align:middle;">
                    <asp:Button ID="CloseButton" runat="server" CssClass="buttonclass" 
                        onclick="CloseButton_Click" Text="Clear" Width="100px" />
                </td>
            </tr>
        </table>
        <table style="width:100%;">
            <tr>
                <td align="center">
                    &nbsp;</td>
            </tr>
            <tr>
                <td align="right" class="style1" style="height:14px; ">
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 25%; height: 68px">
                                <asp:TextBox ID="txtID" runat="server" Visible="False"></asp:TextBox>
                            </td>
                            <td style="width:50%; height: 68px">
                                <asp:UpdatePanel ID="UP1" runat="server" UpdateMode="Conditional"><ContentTemplate>
                                <fieldset style=" text-align:left; vertical-align: top; border: solid 1px #8BB381;">
                                    <legend style="color: maroon;"><b>Party Wise Product Stock</b></legend>
                                    <table style="width:100%; font-size:8pt;">
                                        <tr>
                                            <td align="right" style="width: 25%; height: 33px">
                                                <asp:Label ID="lblPartyID" runat="server" Visible="False"></asp:Label>
                                                Party Name</td>
                                            <td style="width: 17px; height: 33px">
                                            </td>
                                            <td align="left" style="height: 33px">
                                                <asp:TextBox ID="txtPartyName" runat="server" CssClass="TextBox" 
                                                    placeholder="Search Party" Height="20px" SkinID="tbPlain" 
                                                    style="text-align:left" Width="98%" AutoPostBack="True" 
                                                    ontextchanged="txtPartyName_TextChanged" Font-Bold="True"></asp:TextBox>
                                                     <ajaxtoolkit:AutoCompleteExtender ID="AutoCompleteExtender1"
                                                        runat="server" CompletionInterval="20" CompletionSetCount="30"
                                                        EnableCaching="true" MinimumPrefixLength="2"
                                                        ServiceMethod="GetShowPartyInfo" ServicePath="~/AutoComplete.asmx"
                                                        TargetControlID="txtPartyName">
                                                    </ajaxtoolkit:AutoCompleteExtender>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="right" style="width:25%; height: 34px;">
                                                <asp:Label ID="lblItemsID" runat="server" Visible="False"></asp:Label>
                                                Items Name</td>
                                            <td style="width: 17px; height: 34px;">
                                            </td>
                                            <td align="left" style="height: 34px">
                                                <asp:TextBox ID="txtItemsName" runat="server" CssClass="TextBox" Height="20px" placeholder="Search Items Name" 
                                                    SkinID="tbPlain" Width="98%" AutoPostBack="True" 
                                                    ontextchanged="txtItemsName_TextChanged" Font-Bold="True"></asp:TextBox>
                                                    <ajaxtoolkit:AutoCompleteExtender ID="txtPO_AutoCompleteExtender"
                                                        runat="server" CompletionInterval="20" CompletionSetCount="30"
                                                        EnableCaching="true" MinimumPrefixLength="2"
                                                        ServiceMethod="GetShowItemsWithCat" ServicePath="~/AutoComplete.asmx"
                                                        TargetControlID="txtItemsName">
                                                    </ajaxtoolkit:AutoCompleteExtender>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="right" style="width:25%; height: 34px;">
                                                Items Quantity</td>
                                            <td style="width: 17px; height: 34px;">
                                                &nbsp;</td>
                                            <td align="left" style="height: 34px">
                                                <asp:TextBox ID="txtQuantity" runat="server" CssClass="TextBox" Height="20px" placeholder="0.00" 
                                                 style="text-align:Center"  SkinID="tbPlain" Width="50%"></asp:TextBox>
                                            </td>
                                        </tr>
                                    </table>
                                </fieldset>
                                </ContentTemplate>
                                </asp:UpdatePanel>
                            </td>
                            <td style=" width:25%; height: 68px">
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td align="center" style="height:400px;" valign="top">
                    <asp:GridView ID="dgStock" runat="server" AllowPaging="True" 
                        AutoGenerateColumns="False" Caption="Party Stock" CssClass="mGrid" 
                        PageSize="30" Width="50%">
                        <Columns>
                            <asp:CommandField ShowSelectButton="True" />
                            <asp:BoundField DataField="ID" HeaderText="ID" />
                            <asp:BoundField DataField="ContactName" HeaderText="Party Name" />
                            <asp:BoundField DataField="Name" HeaderText="Items Name" />
                            <asp:BoundField DataField="Quantity" HeaderText="Items Quantity" />
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
        </table>
    </div>

</asp:Content>

