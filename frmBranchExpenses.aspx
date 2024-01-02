<%@ Page Title="" Language="C#" MasterPageFile="~/BranchMasterPage.master" AutoEventWireup="true" CodeFile="frmBranchExpenses.aspx.cs" Inherits="frmBranchExpenses" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <div id="frmMainDiv" style="background-color:White; width:100%;">
<table id="pageFooterWrapper">
    <tr>
        <td align="center" style="vertical-align:middle; height:100%;">
            <asp:Button ID="DeleteButton" runat="server" CssClass="buttonclass" 
                onclick="DeleteButton_Click" Text="Delete" Width="100px" Height="35px" />
        </td>
        <td align="center" style="vertical-align:middle;">
            &nbsp;</td>
        <td align="center" style="vertical-align:middle; height:100%;">
            <asp:Button ID="btnSave" runat="server" CssClass="buttonclass" 
                onclick="btnSave_Click" Text="Save" Width="100px" Height="35px" />
        </td>
        <td align="center" style="vertical-align:middle;">
            <asp:Button ID="CloseButton" runat="server" CssClass="buttonclass" 
                onclick="CloseButton_Click" Text="Clear" Width="100px" Height="35px" />
        </td>
    </tr>
</table>
        <table style="width:100%;">
            <tr>
                <td align="center">&nbsp;</td>
            </tr>
            <tr>
                <td align="right" class="style1" style="height:14px; ">
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 25%; height: 68px">
                            </td>
                            <td style="width:50%; height: 68px">
                            <fieldset style=" text-align:left; vertical-align: top; border: solid 1px #8BB381;"><legend style="color: maroon;">
                                <b>Expense Setting</b></legend>
                                <table style="width:100%; font-size:8pt;">
                                    <tr>
                                        <td align="right" style="width: 40%; height: 5px">
                                            </td>
                                        <td style="width: 17px; height: 5px">
                                        </td>
                                        <td align="left" style="height: 5px">
                                            <asp:TextBox ID="txtColorID" runat="server" CssClass="TextBox" Enabled="False" 
                                                Height="20px" ReadOnly="True" SkinID="tbPlain" style="text-align:center" 
                                                Width="220px" Visible="False"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right" style="width:40%; height: 34px;">
                                            Expense Name
                                        </td>
                                        <td style="width: 17px; height: 34px;">
                                        </td>
                                        <td align="left" style="height: 34px">
                                            <asp:TextBox ID="txtColorName" runat="server" CssClass="TextBox" Height="20px" 
                                                SkinID="tbPlain" Width="220px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right" style="width:40%; height: 34px;">
                                            Gl Coa Code
                                        </td>
                                        <td style="width: 17px; height: 34px;">
                                            &nbsp;</td>
                                        <td align="left" style="height: 34px">
                                            <asp:TextBox ID="txtGlCoaCode" runat="server" CssClass="TextBox" Height="20px" 
                                                SkinID="tbPlain" Width="220px" Enabled="False"></asp:TextBox>
                                        </td>
                                    </tr>
                                </table></fieldset>
                            </td>
                            <td style=" width:25%; height: 68px">
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td align="center" style="height:400px;" valign="top">
                    <asp:GridView ID="dgColor" runat="server" AllowPaging="True" 
                        AutoGenerateColumns="False" Caption="Expense History" CssClass="mGrid" 
                        onselectedindexchanged="dgColor_SelectedIndexChanged" PageSize="30" 
                        Width="50%" onrowdatabound="dgColor_RowDataBound">
                        <Columns>
                            <asp:CommandField ShowSelectButton="True" />
                            <asp:BoundField DataField="ID" HeaderText="ID" />
                            <asp:BoundField DataField="Name" HeaderText="Expense" />
                            <asp:BoundField DataField="GL_COA_CODE" HeaderText="GL COA CODE" />
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
        </table>
    </div>

</asp:Content>
