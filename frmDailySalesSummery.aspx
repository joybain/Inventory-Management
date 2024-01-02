<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="frmDailySalesSummery.aspx.cs" Inherits="frmDailySalesSummery" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">

    <table style="width:100%">
        <tr>
            <td style="width:10%">

            </td>
            <td style="width:80%">

            </td>
            <td style="width:10%">

            </td>
        </tr>
        <tr>
            <td style="width:10%">

                &nbsp;</td>
            <td style="width:80%" align="center">

<asp:Panel ID="pnlDailySalesStatus" runat="server" Width="80%">
                        <fieldset style="vertical-align:center; border: solid 1px #8BB381;">
                            <legend style="color: maroon; font-size: 10pt;"><b>Sales Status </b>
                            </legend>
                            <table style="width: 100%">
                                <tr>
                                    <td align="left" colspan="3" style="font-weight: 700;">
                                        &nbsp;</td>
                                    <td colspan="2">
                                        &nbsp;</td>
                                    <td style="width: 10%">
                                        &nbsp;</td>
                                </tr>
                                <tr>
                                    <td align="right" colspan="1" style="font-weight: 700; height: 36px;">
                                        Start Date :&nbsp;
                                    </td>
                                    <td align="right" colspan="1" style="font-weight: 700; height: 36px;" 
                                        width="17%">
                                        <asp:TextBox ID="txtStartDate" runat="server" autocomplete="off" Height="18px" 
                                            placeholder="dd/MM/yyyy" Width="100%"></asp:TextBox>
                                        <ajaxToolkit:CalendarExtender ID="txtStartDate_CalendarExtender" runat="server" 
                                            Format="dd/MM/yyyy" TargetControlID="txtStartDate" />
                                    </td>
                                    <td align="right" colspan="1" style="font-weight: 700; height: 36px;" 
                                        width="10%">
                                        To Date</td>
                                    <td align="left" colspan="2" style="height: 36px">
                                        <asp:TextBox ID="txtDate" runat="server" Height="18px" placeholder="dd/MM/yyyy" autocomplete="off"
                                            Width="90%"></asp:TextBox>
                                        <ajaxtoolkit:calendarextender runat="server" ID="Calendarextender1" 
                                                                      TargetControlID="txtDate" Format="dd/MM/yyyy"/>
                                    </td>
                                    <td style="width: 10%; height: 36px;">
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left" colspan="3" style="font-weight: 700; height: 36px;">
                                        </td>
                                    <td colspan="2" style="height: 36px; text-align: left;">
                                        <asp:Button ID="btnSearch" runat="server" onclick="btnSearch_Click" 
                                            Text="Search" Height="34px" Width="86px" />
                                        &nbsp;&nbsp; &nbsp;
                                        <asp:Button ID="btnPrint" runat="server" onclick="btnPrint_Click" 
                                            Text="Print" Height="34px" Width="74px" />
                                        </td>
                                    <td style="width: 10%; height: 36px;">
                                        </td>
                                </tr>
                                <tr>
                                    <td align="left" colspan="3" style="font-weight: 700;">
                                        *Sales &amp; Return Quantity :</td>
                                    <td colspan="2">
                                        &nbsp;</td>
                                    <td style="width: 10%">
                                        &nbsp;</td>
                                </tr>
                                <tr>
                                    <td align="left" colspan="3">
                                        &nbsp;</td>
                                    <td colspan="2">
                                        &nbsp;</td>
                                    <td style="width: 10%">
                                        &nbsp;</td>
                                </tr>
                                <tr>
                                    <td style="height: 20px; font-weight: 700;" align="right" colspan="3">
                                        Total Sale Quantity :&nbsp;
                                    </td>
                                    <td align="left" colspan="2" style="height: 20px">
                                        <asp:Label ID="lblTotSaleQty" runat="server" style="font-weight: 700"></asp:Label>
                                    </td>
                                    <td style="width: 10%; height: 20px;">
                                    </td>
                                </tr>
                                <tr>
                                    <td style="height: 21px; font-weight: 700;" align="right" colspan="3">
                                        Total Return Quantity :&nbsp;
                                    </td>
                                    <td align="left" colspan="2" style="height: 21px">
                                        <asp:Label ID="lblTotReturnQty" runat="server" style="font-weight: 700"></asp:Label>
                                    </td>
                                    <td style="width: 10%; height: 21px;">
                                    </td>
                                </tr>
                                <tr>
                                    <td style="width: 10%">
                                        &nbsp;</td>
                                    <td align="left" colspan="2" style="font-weight: 700;">
                                        &nbsp;</td>
                                    <td align="left" colspan="2">
                                        &nbsp;</td>
                                    <td style="width: 10%">
                                        &nbsp;</td>
                                </tr>
                                <tr>
                                    <td align="left" colspan="3" style="font-weight: 700;">
                                        *Payment Information</td>
                                    <td align="left" colspan="2">
                                        &nbsp;</td>
                                    <td style="width: 10%">
                                        &nbsp;</td>
                                </tr>
                                <tr>
                                    <td style="width: 10%">
                                        &nbsp;</td>
                                    <td align="left" colspan="2" style="font-weight: 700;">
                                        &nbsp;</td>
                                    <td align="left" colspan="2">
                                        &nbsp;</td>
                                    <td style="width: 10%">
                                        &nbsp;</td>
                                </tr>
                                <tr>
                                    <td colspan="6" align="center">
                                        <asp:GridView ID="dgPayment" runat="server" AutoGenerateColumns="False" 
                                            CssClass="mGrid" Width="90%">
                                            <Columns>
                                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="5%" 
                                                    HeaderText="SL">
                                                    <ItemTemplate>
                                                        <%# Container.DataItemIndex + 1 %>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" Width="5%" />
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="Name" HeaderText="Account Type">
                                                <ItemStyle HorizontalAlign="Center" Width="60%" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Amount" HeaderText="Amount">
                                                <ItemStyle HorizontalAlign="Right" Width="25%" />
                                                </asp:BoundField>
                                            </Columns>
                                        </asp:GridView>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="width: 10%">
                                        &nbsp;</td>
                                    <td align="left" colspan="2" style="font-weight: 700;">
                                        &nbsp;</td>
                                    <td align="left" colspan="2">
                                        &nbsp;</td>
                                    <td style="width: 10%">
                                        &nbsp;</td>
                                </tr>
                                <tr>
                                    <td style="width: 10%">
                                        &nbsp;</td>
                                    <td style="width: 10%">
                                        &nbsp;</td>
                                    <td style="width: 10%">
                                        &nbsp;</td>
                                    <td style="width: 10%">
                                        &nbsp;</td>
                                    <td style="width: 10%">
                                        &nbsp;</td>
                                    <td style="width: 10%">
                                        &nbsp;</td>
                                </tr>
                            </table>
                        </fieldset>
                    </asp:Panel>

            </td>
            <td style="width:10%">

                &nbsp;</td>
        </tr>
        <tr>
            <td style="width:10%">

                &nbsp;</td>
            <td style="width:80%">

                &nbsp;</td>
            <td style="width:10%">

                &nbsp;</td>
        </tr>
    </table>
</asp:Content>

