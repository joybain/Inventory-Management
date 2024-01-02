  <%@ Page Title="" Language="C#" MasterPageFile="~/BranchMasterPage.master" AutoEventWireup="true" CodeFile="BranchItemReceivedStock.aspx.cs" Inherits="BranchItemReceivedStock" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    
    
    <div style="background-color:White; width:100%; min-height:700px; height:auto !important; height:700px; font-family:Tahoma;">

    <br/>
    <br/>
    <fieldset style="vertical-align: top; border: solid 1px #8BB381;text-align:left;"><legend style="color: maroon;"><b>Search Items</b> </legend>
                <table style="width: 100%;">
                    <tr>
                        <td style="width: 15%; text-align: center; font-weight: 700; height: 30px;"></td>
                        <td style="width: 20%; height: 30px;">
                            </td>
                        <td style="width: 10%; text-align: center; font-weight: 700; height: 30px;"></td>
                        <td style="width: 20%; height: 30px;">
                            </td>
                        <td style="width: 5%; height: 30px;"></td>
                        <td style="width: 15%; text-align: center; height: 30px;">
                            </td>
                        <td style="width: 20%;text-align: center; height: 30px;">
                            </td>
                    </tr>
                    
                    <tr>
                            <td style="width: 15%; text-align: center; font-weight: 700; height: 48px;">Search By Date :</td>
                            <td style="width: 20%; height: 48px;">
                                <asp:TextBox ID="txtByDate" runat="server" placeholder="dd/MM/yyyy" autocomplete="off"
                                             style="text-align:center;" TabIndex="11" Width="99%" 
                                    Height="30px"></asp:TextBox>
                                <ajaxtoolkit:calendarextender runat="server" ID="Calendarextender1" 
                                                              TargetControlID="txtByDate" Format="dd/MM/yyyy"/>
                                    </td>
                            <td style="width: 10%; text-align: center; font-weight: 700; height: 48px;">To Date :</td>
                            <td style="width: 20%; height: 48px;">
                                <asp:TextBox ID="txtToDate" runat="server" placeholder="dd/MM/yyyy" autocomplete="off"
                                             style="text-align:center;" TabIndex="11" Width="99%" 
                                    Height="30px"></asp:TextBox>
                                <ajaxtoolkit:calendarextender runat="server" ID="Calendarextender2" 
                                                              TargetControlID="txtToDate" Format="dd/MM/yyyy"/>
                            </td>
                            <td style="width: 5%; height: 48px;"></td>
                            <td style="width: 15%; text-align: center; height: 48px;">
                                <asp:LinkButton ID="lbSearch" runat="server" Font-Bold="True" Font-Size="12pt" 
                                    Width="90%" BorderStyle="Solid" onclick="lbSearch_Click" Height="25px">Search</asp:LinkButton>
                            </td>
                            <td style="width: 20%;text-align: center; height: 48px;">
                                <table style="width: 100%;">
                                    <tr>
                                        <td style="width: 50%;text-align: center;">
                                            <asp:LinkButton ID="lbClear" runat="server" Font-Bold="True" Font-Size="12pt" 
                                                Width="90%" BorderStyle="Solid" onclick="lbClear_Click" Height="25px">Clear</asp:LinkButton>
                                        </td>
                                        <td style="width: 50%;">&nbsp;</td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 15%; text-align: center; font-weight: 700; height: 30px;"></td>
                            <td style="width: 20%; height: 30px;">
                                </td>
                            <td style="width: 10%; text-align: center; font-weight: 700; height: 30px;"></td>
                            <td style="width: 20%; height: 30px;">
                                </td>
                            <td style="width: 5%; height: 30px;"></td>
                            <td style="width: 15%; text-align: center; height: 30px;">
                                </td>
                            <td style="width: 20%;text-align: center; height: 30px;">
                                </td>
                        </tr>
                        </table>
      
        
      
                       
                        </fieldset>
    
    
    <br/>
    <br/>
    
         <div id="divItemMst" runat="server">
        <fieldset style="vertical-align: top; border: solid 1px #8BB381;text-align:left;"><legend style="color: maroon;"><b>Transfer/Received Stock</b> </legend>
    <table style="width: 100%">
            <tr>
                <td style="height: 35px;">
                     <asp:GridView ID="dgBranchItemReceivedMst" runat="server" 
                        AutoGenerateColumns="False" CssClass="mGrid" 
                        onrowdatabound="dgBranchItemReceivedMst_RowDataBound" 
                        onselectedindexchanged="dgBranchItemReceivedMst_SelectedIndexChanged" 
                         Width="100%" Font-Size="10pt">
                        <Columns>
                            <asp:CommandField ShowSelectButton="True">
                            <HeaderStyle Width="8%" />
                            <ItemStyle Width="8%" />
                            </asp:CommandField>
                            <asp:BoundField DataField="MStID" HeaderText="MstId">
                            <FooterStyle HorizontalAlign="Center" Width="5%" />
                            <HeaderStyle HorizontalAlign="Center" Width="5%" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Code" HeaderText="Code">
                            <HeaderStyle HorizontalAlign="Center" Width="12%" />
                            <ItemStyle HorizontalAlign="Left" Width="12%" />
                            </asp:BoundField>
                            <asp:BoundField DataField="TransferDate" HeaderText="Transfer Date">
                            <HeaderStyle HorizontalAlign="Center" Width="12%" />
                            <ItemStyle HorizontalAlign="Left" Width="12%" />
                            </asp:BoundField>
                            <asp:BoundField DataField="BranchName" HeaderText="Transfar From">
                            <HeaderStyle HorizontalAlign="Left" Width="10%" />
                            <ItemStyle HorizontalAlign="Left" Width="10%" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Remark" HeaderText="Remark">
                            <HeaderStyle HorizontalAlign="Left" />
                            <ItemStyle HorizontalAlign="Left" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Status" HeaderText="Status">
                            <HeaderStyle HorizontalAlign="Left" Width="12%" />
                            <ItemStyle HorizontalAlign="Left" />
                            </asp:BoundField>
                        </Columns>
                    </asp:GridView>
                    

                </td>
            </tr>
          
        </table>
            </fieldset>
        
        </div>
        
        
        
        <%--
        ****--%>
    <div id="divItemDtls" runat="server">
        <fieldset style="vertical-align: top; border: solid 1px #8BB381;text-align:left;"><legend style="color: maroon;"><b>Received Details Item </b> </legend>
    <table style="width: 100%; ">
            <tr>
                <td style="height: 11px;" colspan="4">
                    

                    <asp:GridView ID="dgBranchItemReceivedDtl" runat="server" 
                        AutoGenerateColumns="False" CssClass="mGrid" 
                        onrowdatabound="dgBranchItemReceivedDtl_RowDataBound" Visible="False" 
                        Width="100%" BorderStyle="None" CellPadding="2" Font-Size="11pt">
                        <Columns>
                            <asp:BoundField DataField="Name" HeaderText="Item Name">
                            <HeaderStyle HorizontalAlign="Left" Width="20%" />
                            <ItemStyle HorizontalAlign="Left" Width="30%" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Barcode" HeaderText="Barcode">
                            <HeaderStyle HorizontalAlign="Left" Width="10%" />
                            <ItemStyle HorizontalAlign="Left" Width="10%" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Code" HeaderText="Item Code">
                            <HeaderStyle HorizontalAlign="Left" />
                            <ItemStyle HorizontalAlign="Left" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Size" HeaderText="Size">
                            <HeaderStyle HorizontalAlign="Left" Width="5%" />
                            <ItemStyle HorizontalAlign="Left" Width="5%" />
                            </asp:BoundField>
                            <asp:BoundField DataField="StyleNo" HeaderText="Style">
                            <HeaderStyle HorizontalAlign="Left" Width="10%" />
                            <ItemStyle HorizontalAlign="Left" Width="10%" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Color" HeaderText="Color">
                            <HeaderStyle HorizontalAlign="Left" Width="5%" />
                            <ItemStyle HorizontalAlign="Left" Width="5%" />
                            </asp:BoundField>
                            <asp:BoundField DataField="TransferQuantity" HeaderText="Transfer Qty">
                            <HeaderStyle HorizontalAlign="Center" Width="10%" />
                            <ItemStyle HorizontalAlign="Right" Width="10%" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Discount" HeaderText="Discount">
                            <HeaderStyle HorizontalAlign="Center" Width="10%" />
                            <ItemStyle HorizontalAlign="Right" Width="10%" />
                            </asp:BoundField>
                            <asp:BoundField DataField="BranchSalesPrice" HeaderText="Sale Price">
                            <HeaderStyle HorizontalAlign="Center" Width="10%" />
                            <ItemStyle HorizontalAlign="Right" Width="10%" />
                            </asp:BoundField>
                            <asp:BoundField DataField="TransferQuantity" HeaderText="Recived  Qty">
                            <HeaderStyle HorizontalAlign="Center" Width="10%" />
                            <ItemStyle HorizontalAlign="Right" Width="10%" />
                            </asp:BoundField>
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
            <tr>
                <td style="width:10%; height: 35px;">
                    <asp:HiddenField ID="hfId" runat="server" />
                </td>
                <td style="width:70%; height: 35px;">
                    

                    <asp:HiddenField ID="hfStatus" runat="server" />
                </td>
                <td style="width:10%; height: 35px;">
                    <asp:Button ID="BtnSave" runat="server" onclick="BtnSave_Click" Text="Save" 
                        Visible="False" Width="129px" Height="43px" />
                </td>
                <td style="width:10%; height: 35px;">
                    <asp:Button ID="btnBack" runat="server" Text="Back" Width="129px" Height="43px" onclick="btnBack_Click" 
                       />
                </td>
            </tr>
        </table>
        </fieldset>
        </div>
  </div>
</asp:Content>

