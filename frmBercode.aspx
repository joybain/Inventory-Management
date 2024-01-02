<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="frmBercode.aspx.cs" Inherits="frmBercode" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <div id="frmMainDiv" style="background-color:White; width:100%;"> 
    <table style="width: 100%">
        <tr>
            <td style="width:4%;">
                &nbsp;</td>
            <td style="width:92%;">
                &nbsp;</td>
            <td style="width:4%;">
                &nbsp;</td>
        </tr>      
        <tr>
            <td style="width:4%;">
                &nbsp;</td>
            <td style="width:92%;">
                <asp:Label ID="lblBranchName" runat="server" Font-Size="10pt" ForeColor="Black" 
                   style="font-weight: 700" Font-Underline="True" Visible="False"></asp:Label>
                <asp:Label ID="lblBranchID" runat="server" Visible="False"></asp:Label>
                <asp:Label ID="lblUserType" runat="server" Visible="False"></asp:Label>
            </td>
            <td style="width:4%;">
                &nbsp;</td>
        </tr>      
        <tr>
            <td style="width:4%;">
                &nbsp;</td>
            <td style="width:92%;">
                 <fieldset style="vertical-align: top; border: solid 1px #8BB381;text-align:left;">
                     <legend style="color: maroon;"><b>Search Option</b> </legend>
                     <table style="width: 100%;">
                         <tr>
                             <td style="width: 24%; text-align: center; font-weight: 700;">
                                 Search By GRN No.&amp;Supplier&nbsp; :
                             </td>
                             <td>
                                 <asp:UpdatePanel ID="UPSearch" runat="server" UpdateMode="Conditional">
                                     <ContentTemplate>
                                         <asp:HiddenField ID="hfPurchaseID" runat="server" />
                                         <asp:TextBox ID="txtSupplierSearch" runat="server" CssClass="txtVisibleAlign"
                                             placeholder="Search By Supplier.." Width="90%" Height="22px"></asp:TextBox>
                                         <ajaxToolkit:AutoCompleteExtender ID="txtSupplierSearch_AutoCompleteExtender" 
                                             runat="server" CompletionInterval="20" CompletionSetCount="12" 
                                             EnableCaching="true" MinimumPrefixLength="1" ServiceMethod="GetItemPurcaseWithGrn" 
                                             ServicePath="AutoComplete.asmx" TargetControlID="txtSupplierSearch" />
                                     </ContentTemplate>
                                 </asp:UpdatePanel>
                             </td>
                             <td style="width: 15%; text-align: center">
                                 <asp:LinkButton ID="lbSearch" runat="server" BorderStyle="Solid" 
                                     Font-Bold="True" Font-Size="12pt" OnClick="lbSearch_Click" Width="90%">Search</asp:LinkButton>
                             </td>
                             <td style="width: 20%;text-align: center">
                                 <table style="width: 100%;">
                                     <tr>
                                         <td style="width: 50%;text-align: center;">
                                             <asp:LinkButton ID="lbClear" runat="server" BorderStyle="Solid" 
                                                 Font-Bold="True" Font-Size="12pt" OnClick="lbClear_Click" Width="90%">Clear</asp:LinkButton>
                                         </td>
                                         <td style="width: 50%;">
                                             &nbsp;</td>
                                     </tr>
                                 </table>
                             </td>
                         </tr>
                     </table>
                 </fieldset></td>
            <td style="width:4%;">
                &nbsp;</td>
        </tr>      
        <tr>
            <td style="width:4%;">
                &nbsp;</td>
            <td style="width:92%;">
                 <fieldset style="vertical-align: top; border: solid 1px #8BB381; text-align:left;"><legend style="color: maroon;">
                <b>Barcode Generate</b> </legend>
            <asp:UpdatePanel ID="PVIesms_UP" runat="server"  UpdateMode="Conditional">
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
                            <asp:TemplateField HeaderText="Item Code">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtItemCode" runat="server" AutoPostBack="true" 
                                        Font-Size="8pt" MaxLength="15" onFocus="this.select()" SkinId="tbPlain" 
                                        Text='<%#Eval("item_code")%>' Width="93%"></asp:TextBox>
                                </ItemTemplate>
                                <FooterStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" Width="12%" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Description">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtItemDesc" runat="server" autocomplete="off" placeholder="Search Items" Height="20px"
                                        AutoPostBack="true" Font-Size="8pt" onFocus="this.select()" CssClass="txtVisibleAlign"
                                        OnTextChanged="txtItemDesc_TextChanged" SkinId="tbPlain" 
                                        Text='<%#Eval("item_desc")%>' Width="97%">
                                    </asp:TextBox>
                                    <ajaxToolkit:AutoCompleteExtender ID="autoComplete1" runat="server"  CompletionInterval="20"
                                        CompletionSetCount="12" EnableCaching="true" 
                                        MinimumPrefixLength="1" ServiceMethod="GetItemListBarcode" 
                                        ServicePath="AutoComplete.asmx" TargetControlID="txtItemDesc" />                                     
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Left" Width="30%" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="StockQunatity">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtStkQty" runat="server" AutoPostBack="False" Enabled="false"
                                        CssClass="tbr" Font-Size="8pt" onFocus="this.select()" SkinId="tbPlain" 
                                        Text='<%#Eval("StkQty")%>' Width="93%">
                                    </asp:TextBox>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Right" Width="10%" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Item Rate">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtItemRate" runat="server" AutoPostBack="False"  Enabled="false"
                                        CssClass="tbr" Font-Size="8pt" onFocus="this.select()" SkinId="tbPlain" 
                                        Text='<%#Eval("item_rate")%>' Width="93%">
                                    </asp:TextBox>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Right" Width="10%" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Quantity">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtQnty" runat="server" AutoPostBack="true" CssClass="tbc" 
                                        Font-Size="8pt" onFocus="this.select()" OnTextChanged="txtQnty_TextChanged" 
                                        SkinId="tbPlain" Text='<%#Eval("qnty")%>' Width="93%"></asp:TextBox>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" Width="10%" />
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Tax">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtTax" runat="server" AutoPostBack="true" CssClass="tbc" 
                                        Font-Size="8pt" onFocus="this.select()" 
                                        SkinId="tbPlain" Text='<%#Eval("Tax")%>' Width="93%"></asp:TextBox>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" Width="10%" />
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Barcode">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtBarcode" runat="server" autocomplete="off" placeholder="Search Items" Height="20px"
                                                 AutoPostBack="true" Font-Size="8pt" onFocus="this.select()" CssClass="txtVisibleAlign"
                                                 SkinId="tbPlain" 
                                                 Text='<%#Eval("Barcode")%>' Width="97%">
                                    </asp:TextBox>
                                </ItemTemplate>
                            </asp:TemplateField>

                        </Columns>
                        <HeaderStyle Font-Bold="True" Font-Size="9pt" ForeColor="White" />
                        <PagerStyle CssClass="pgr" ForeColor="White" HorizontalAlign="Center" />
                        <RowStyle BackColor="White" />
                    </asp:GridView>
                </ContentTemplate>
        </asp:UpdatePanel></fieldset>
            </td>
            <td style="width:4%;">
                &nbsp;</td>
        </tr>      
        <tr>
            <td style="width:4%;">
                &nbsp;</td>
            <td style="width:92%;">
                 &nbsp;</td>
            <td style="width:4%;">
                &nbsp;</td>
        </tr>      
        <tr>
            <td style="width:4%;">
                &nbsp;</td>
            <td style="width:92%;" align="center">
                <asp:Button ID="btnPrint" runat="server" onclick="btnPrint_Click" Text="Print" 
                    Width="130px" Height="35px" />
&nbsp;
                <asp:Button ID="btnClear" runat="server" Text="Clear" Width="130px" 
                    onclick="btnClear_Click" Height="35px" />
            &nbsp;
                </td>
            <td style="width:4%;">
                &nbsp;</td>
        </tr>      
    </table>
    </div>
</asp:Content>

