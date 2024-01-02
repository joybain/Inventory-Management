<%@ Page Title="Department Setup.-RD" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="frmDepertmentEntry.aspx.cs" Inherits="frmDepertmentEntry" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <div id="frmMainDiv" style="background-color:White; width:100%;">
      
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
                                <asp:Label ID="lbDeptID" runat="server" Visible="False"></asp:Label>
                            </td>
                            <td style="width:50%; height: 68px">
                                <fieldset style=" text-align:left; vertical-align: top; border: solid 1px #8BB381;">
                                    <legend style="color: maroon;"><b>Department Setting</b></legend>
                                    <table style="width:100%; font-size:8pt;">
                                        <tr>
                                            <td align="right" style="width:40%; height: 34px;">
                                                Dept Name
                                            </td>
                                            <td style="width: 17px; height: 34px;">
                                            </td>
                                            <td align="left" style="height: 34px">
                                                <asp:TextBox ID="txtDeptName" runat="server" CssClass="TextBox" Height="20px" 
                                                SkinID="tbPlain" Width="220px"></asp:TextBox>
                                            </td>
                                        </tr>
                                    </table>
                                </fieldset>
                            </td>
                            <td style=" width:25%; height: 68px">
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 25%;">
                                &nbsp;</td>
                            <td style="width:50%;">
                                <table>
                                    <tr>
                                        
                                        <td style="width:37%"></td>
                                        <td align="center" style="vertical-align:middle; height:100%;">
                                            <asp:Button ID="btnDelete" runat="server" CssClass="buttonclass" Height="35px" 
                                                onclick="btnDelete_Click" Text="Delete" Width="100px" />
                                        </td>
                                        <td align="center" style="vertical-align:middle;"></td>
                                        <td align="center" style="vertical-align:middle; height:100%;">
                                            <asp:Button ID="btnSave" runat="server" CssClass="buttonclass" Height="35px" 
                                                onclick="btnSave_Click" Text="Save" Width="100px" />
                                        </td>
                                        <td align="center" style="vertical-align:middle;">
                                            <asp:Button ID="btnClear" runat="server" CssClass="buttonclass" Height="35px" 
                                                onclick="btnClear_Click" Text="Clear" Width="100px" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td style=" width:25%;">
                                &nbsp;</td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td align="center" style="height:400px;" valign="top">
                    <asp:GridView ID="dgDept" runat="server" 
                        AutoGenerateColumns="False" Caption="Department History" CssClass="mGrid" PageSize="50" 
                        Width="50%" 
                        Font-Bold="False" Font-Size="Small" 
                        onselectedindexchanged="dgDept_SelectedIndexChanged" 
                        onrowdatabound="dgDept_RowDataBound">
                        <Columns>
                            <asp:CommandField ShowSelectButton="True">
                            <ItemStyle HorizontalAlign="Center" Width="10%"></ItemStyle>
                            </asp:CommandField>
                            <asp:BoundField DataField="ID" HeaderText="ID">
                            <ItemStyle HorizontalAlign="Center" Width="5%"></ItemStyle>
                            </asp:BoundField>                           
                            <asp:BoundField DataField="Dept_Name" HeaderText="Department">
                            <ItemStyle HorizontalAlign="Center" Width="85%"></ItemStyle>
                            </asp:BoundField>
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>

