<%@ Page Title="NSSL-SHOP.-Home Page"  Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Home.aspx.cs" Inherits="Home" EnableEventValidation="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <script language="javascript" type="text/javascript" >    

    function LoadModalDiv() {

        var bcgDiv = document.getElementById("divBackground");
        bcgDiv.style.display = "block";

    }
    function HideModalDiv() {

        var bcgDiv = document.getElementById("divBackground");
        bcgDiv.style.display = "none";

    }
</script>

<div id="frmMainDiv" style="background-color:White; width:100%; overflow:visible;">
<table style="width:100%;">
<tr>
<td style="width:100%; padding-left:10px;" align="center">
    &nbsp;</td>
</tr>
<tr>
<td style="width:100%; padding-left:10px; text-underline: black;" align="left">
   <h3> Quick Menu </h3>
</td>
</tr>
<tr>
<td style="width:100%; padding-left:10px;" align="center">
<asp:Panel ID="pnlTask" runat="server" Visible="True" Width="100%">
<div class="row">
    <br />
    <div class="col-xs-12 col-md-12">
    </div>
    <div class="col-xs-12 col-md-12">
    </div>
    
    <div>
        <table width="100%">
            <tr>
                <td style="width: 10%;" align="center">
                    <a class="mnu-box" href="#">
                        <asp:ImageButton ID="ibDailySalesStatus" runat="server" Width="70px" Height="50px"
                        ImageUrl="~/Logo/DailySalesStatus.PNG" 
                        onclick="ibDailySalesStatus_Click" />
                        <h4>Daily Sales Status</h4>
                    </a>
                </td>
                <td style="width: 10%;" align="center">
                    <a class="mnu-box" href="#">
                        <asp:ImageButton ID="ibItemPurchase" runat="server" Width="70px" Height="50px"
                        ImageUrl="~/Logo/ItemPurchase.png" onclick="ibItemPurchase_Click" />
                        <h4>Item Purchase</h4>
                    </a>
                </td>
                <td style="width: 10%;" align="center">
                    <a class="mnu-box" href="#">
                        <asp:ImageButton ID="ibSalesInvoice" runat="server" Width="70px" Height="50px"
                                         ImageUrl="~/Logo/sale-4.png" 
                        onclick="ibSalesInvoice_Click" />
                        <h4>Item Sales/Invoice</h4>
                    </a>
                </td> 
                <td style="width: 10%;" align="center">
                    <a class="mnu-box" href="#">
                        <asp:ImageButton ID="ibItemStock" runat="server" Width="70px" Height="50px"
                                         ImageUrl="~/Logo/ItemStock.PNG" 
                        onclick="ibItemStock_Click" />
                        <h4>Item Stock</h4>
                    </a>
                </td>
                <td style="width: 10%;" align="center">
                    <a class="mnu-box" href="#">
                        <asp:ImageButton ID="ibChangePassword" runat="server" Width="70px" Height="50px"
                                         ImageUrl="~/Logo/ChangePassword.PNG" 
                        onclick="ibChangePassword_Click" />
                        <h4>Change Password</h4>
                    </a>
                </td>
            </tr>
        </table>
    </div>
    <div>
        <table width="100%">
            <tr>
                <td style="width: 10%;">
                    &nbsp;</td>
                <td style="width: 70%;" align="center" valign="top">
                    --------------------------------------------------------------------------------</td>
                <td style="width: 10%;">&nbsp;</td>
            </tr>
            <tr>
                <td style="width: 10%;">
                </td>
                <td align="center" style="width: 70%;" valign="top">
                    <asp:Panel ID="pnlDailySalesStatus" runat="server" Width="50%">
                        <fieldset style="vertical-align:center; border: solid 1px #8BB381;">
                            <legend style="color: maroon; font-size: 10pt;"><b>Daily Sales Status </b>
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
                                    <td align="right" colspan="3" style="font-weight: 700;">
                                        Select Date :&nbsp;
                                    </td>
                                    <td align="left" colspan="2">
                                        <asp:TextBox ID="txtDate" runat="server" Height="18px" placeholder="dd/MM/yyyy" autocomplete="off"
                                            Width="80%"></asp:TextBox>
                                        <ajaxtoolkit:calendarextender runat="server" ID="Calendarextender1" 
                                                                      TargetControlID="txtDate" Format="dd/MM/yyyy"/>
                                    </td>
                                    <td style="width: 10%">
                                        <asp:Button ID="btnSearch" runat="server" onclick="btnSearch_Click" 
                                            Text="Search" />
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left" colspan="3" style="font-weight: 700;">
                                        &nbsp;</td>
                                    <td colspan="2">
                                        &nbsp;</td>
                                    <td style="width: 10%">
                                        &nbsp;</td>
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
                    <asp:LinkButton ID="lbChangePass" runat="server" Font-Size="8pt" 
                        Text="Change Password" Visible="False"></asp:LinkButton>
                    <br />
                    <asp:Panel ID="pnlChangePass" runat="server">
                        <table style="width:250px; border: 1px solid; font-size:8pt;">
                            <tr>
                                <td style="width:150px;">
                                    &nbsp;</td>
                                <td style="width:100px;">
                                    &nbsp;</td>
                            </tr>
                            <tr>
                                <td style="width:150px;">
                                    User ID</td>
                                <td style="width:100px;">
                                    <asp:TextBox ID="txtCpUserName" runat="server" Enabled="false" Font-Size="9" 
                                        MaxLength="18" SkinID="tbGray" Width="100px"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td style="width:150px;">
                                    Current Password</td>
                                <td style="width:100px;">
                                    <asp:TextBox ID="txtCpCurPass" runat="server" Font-Size="9" MaxLength="18" 
                                        SkinID="tbGray" TextMode="Password" Width="100px"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td style="width:150px;">
                                    New Password</td>
                                <td style="width:100px;">
                                    <asp:TextBox ID="txtCpNewPass" runat="server" Font-Size="9" MaxLength="18" 
                                        SkinID="tbGray" TextMode="Password" Width="100px"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td style="width:150px;">
                                    Confirm Password</td>
                                <td style="width:100px;">
                                    <asp:TextBox ID="txtCpConfPass" runat="server" Font-Size="9" MaxLength="18" 
                                        SkinID="tbGray" TextMode="Password" Width="100px"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2" style="height:10px;">
                                </td>
                            </tr>
                            <tr>
                                <td align="right" style="width:150px;">
                                    
                                    <asp:Button ID="lbChangePassword" runat="server" Font-Size="8pt" 
                                                OnClick="lbChangePassword_click" Text="Change" Width="100px" 
                                        Height="30px" />
                                </td>
                                <td align="center" style="width:100px;">
                                    <asp:Button ID="lbCancel" runat="server" Font-Size="8pt" 
                                                onclick="lbCancel_Click" Text="Cancel" Width="100px" 
                                        Height="30px" />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <asp:Label ID="lblTranStatus" runat="server" Font-Size="8pt" Text="" 
                                        Visible="false"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </td>
                <td style="width: 10%;">
                </td>
            </tr>
            <tr>
                <td style="width: 10%;">
                    &nbsp;</td>
                <td align="center" style="width: 70%;" valign="top">
                    &nbsp;</td>
                <td style="width: 10%;">
                    &nbsp;</td>
            </tr>
            <tr>
                <td style="width: 10%;">
                    &nbsp;</td>
                <td align="center" style="width: 70%;" valign="top">
                    &nbsp;</td>
                <td style="width: 10%;">
                    &nbsp;</td>
            </tr>
        </table>
    </div>

        
    </div>
    

</asp:Panel>

<%-- </ContentTemplate>
 </asp:UpdatePanel>--%>
</td>
</tr>
</table>
</div>    

</asp:Content>

