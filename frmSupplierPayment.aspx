<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="frmSupplierPayment.aspx.cs" Inherits="frmSupplierPayment" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">

    <table style="width: 100%;background-color: white">
        <tr>
            <td style="width: 20%">
                &nbsp;</td>
            <td style="width: 60%">
                &nbsp;</td>
            <td style="width: 20%">
                &nbsp;</td>
        </tr>
        <tr>
            <td style="width: 20%">
                &nbsp;</td>
            <td style="width: 60%">
                <fieldset style="vertical-align: top; border: solid 1px #8BB381;text-align:left;">
                    <legend style="color: maroon; font-weight: 700;">Supplier<b> Payment</b> </legend>
                <table style="width: 100%">
                    <tr>
                        <td style="width: 17%; height: 32px;" align="right">
                            <asp:Label ID="Label2" runat="server" Font-Bold="True" Text="Search Supplier"></asp:Label>
                        </td>
                        <td style="width: 2%; height: 32px;">
                            </td>
                        <td style="height: 32px;" colspan="4">
                            <asp:HiddenField ID="hfSupplier" runat="server" />
                            <asp:HiddenField ID="hfSupplierCoa" runat="server" />
                            <asp:TextBox ID="txtSupplierSearch" runat="server" AutoPostBack="True" CssClass="txtVisibleAlign"
            ontextchanged="txtSupplierSearch_TextChanged" placeholder="Search By Supplier.." 
            Width="100%" Height="18px"></asp:TextBox>
        <asp:HiddenField ID="hfSupplierID" runat="server" />
        <ajaxToolkit:autocompleteextender ID="txtSupplierSearch_AutoCompleteExtender" runat="server" 
            CompletionInterval="20" CompletionSetCount="12" EnableCaching="true" 
            MinimumPrefixLength="1" ServiceMethod="GetSupplier" 
            ServicePath="AutoComplete.asmx" TargetControlID="txtSupplierSearch" />
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 17%; height: 24px;" align="right">
                            Pay Date</td>
                        <td style="width: 2%; height: 24px;">
                            &nbsp;</td>
                        <td style="width: 15%; height: 24px;">
                            <asp:TextBox ID="txtPayDate" runat="server" placeholder="dd/MM/yyyy" autocomplete="off"
                                style="text-align:center;" TabIndex="11" Height="18px"></asp:TextBox>
                            <ajaxToolkit:CalendarExtender ID="Calendarextender2" runat="server" 
                                Format="dd/MM/yyyy" TargetControlID="txtPayDate" />
                        </td>
                        <td style="width: 10%; height: 24px;">
                            Due Amount</td>
                        <td style="width: 2%; height: 24px;">
                            &nbsp;</td>
                        <td style="width: 15%; height: 24px;">
                            <asp:TextBox ID="txtDueAmount" style="text-align: right" runat="server" 
                                Enabled="False" Height="18px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 17%; height: 24px;" align="right">
                            &nbsp;</td>
                        <td style="width: 2%; height: 24px;">
                            </td>
                        <td style="width: 15%; height: 24px;">
                            &nbsp;</td>
                        <td style="width: 10%; height: 24px;">
                            Pay Amount</td>
                        <td style="width: 2%; height: 24px;">
                            </td>
                        <td style="width: 15%; height: 24px;">
                            <asp:TextBox ID="txtPayAmount" style="text-align: right" runat="server" 
                                AutoPostBack="True" ontextchanged="txtPayAmount_TextChanged" Height="18px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 17%; height: 27px;" align="right">
                            Remarks</td>
                        <td style="width: 2%; height: 27px;">
                            </td>
                        <td style="height: 27px;" colspan="4">
                            <asp:TextBox ID="txtRemarks" runat="server" Width="98%" Height="18px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 17%">
                            &nbsp;</td>
                        <td style="width: 2%">
                            &nbsp;</td>
                        <td style="width: 15%">
                            &nbsp;</td>
                        <td style="width: 10%">
                            &nbsp;</td>
                        <td style="width: 2%">
                            &nbsp;</td>
                        <td style="width: 15%">
                            &nbsp;</td>
                    </tr>
                </table>
                </fieldset>
            </td>
            <td style="width: 20%">
                <asp:HiddenField ID="hfID" runat="server" />
            </td>
        </tr>
        <tr>
            <td style="width: 20%">
                &nbsp;</td>
            <td style="width: 60%;" align="center">
                <table >
                    <tr>
                        <td align="center">
                            <asp:Button ID="btnDelete" runat="server" BorderStyle="Outset" Height="35px" 
                                onclick="btnDelete_Click" 
                                onclientclick="javascript:return window.confirm('are u really want to delete these data')" 
                                Text="Delete" ToolTip="Delete" Width="110px" />
                        </td>
                        <td align="center">
                            &nbsp;</td>
                        <td align="center">
                            <asp:Button ID="btnSave" runat="server" BorderStyle="Outset" Height="35px" 
                                onclick="btnSave_Click" Text="Save" ToolTip="Save Purchase Record" 
                                Width="110px" />
                        </td>
                        <td align="center">
                            &nbsp;</td>
                        <td align="center">
                            <asp:Button ID="btnClear" runat="server" BorderStyle="Outset" Height="35px" 
                                onclick="btnClear_Click" Text="Clear" ToolTip="Clear" Width="110px" />
                        </td>
                        <td align="center">
                            &nbsp;</td>
                        <td align="center">
                            <asp:Button ID="btnPrint" runat="server" BorderStyle="Outset" Height="35px" 
                                onclick="btnPrint_Click" Text="Print" ToolTip="Print" Width="110px" />
                        </td>
                        <td align="center">
                            &nbsp;</td>
     <%-- onclick="btnChallan_Click" --%>
                
                        <td align="center">
                            &nbsp;</td>
                    </tr>
                </table>
            </td>
            <td style="width: 20%">
                &nbsp;</td>
        </tr>
        <tr>
            <td style="width: 20%">
                &nbsp;</td>
            <td style="width: 60%">
                &nbsp;</td>
            <td style="width: 20%">
                &nbsp;</td>
        </tr>
        <tr>
            <td style="width: 20%">
                &nbsp;</td>
            <td style="width: 60%">
    
  <asp:GridView CssClass="mGrid" PagerStyle-CssClass="pgr"  
        AlternatingRowStyle-CssClass="alt" ID="dgSVMst" runat="server" 
        AutoGenerateColumns="False"  BackColor="White" BorderWidth="1px" BorderStyle="Solid"
        CellPadding="2" BorderColor="LightGray" Font-Size="9pt" PageSize="30" 
        Width="100%" AllowPaging="True" onrowdatabound="dgSVMst_RowDataBound" 
        onselectedindexchanged="dgSVMst_SelectedIndexChanged" 
        onpageindexchanging="dgSVMst_PageIndexChanging" >
  <HeaderStyle Font-Size="9pt"  Font-Bold="True" BackColor="LightGray" HorizontalAlign="center"  ForeColor="Black" />
  <Columns>  
  <asp:CommandField ShowSelectButton="True"  ItemStyle-Width="40px" 
          ItemStyle-HorizontalAlign="Center">
<ItemStyle HorizontalAlign="Center" Width="40px"></ItemStyle>
      </asp:CommandField>
      <asp:BoundField  HeaderText="ID" DataField="ID"  ItemStyle-Width="80px" 
          ItemStyle-HorizontalAlign="Left" > 
<ItemStyle HorizontalAlign="Left" Width="80px"></ItemStyle>
      </asp:BoundField>
  <asp:BoundField  HeaderText="Date" ItemStyle-Width="100px" 
          ItemStyle-HorizontalAlign="Center" DataField="Date">
          <ItemStyle HorizontalAlign="Center" Width="80px"></ItemStyle>
      </asp:BoundField>
  <asp:BoundField  HeaderText="Supplier Name"  DataField="ContactName" ItemStyle-Width="100px" 
          ItemStyle-HorizontalAlign="Left">    
<ItemStyle HorizontalAlign="Center" Width="100px"></ItemStyle>
      </asp:BoundField>
      <asp:BoundField  HeaderText="Pay Amount" DataField="PayAmt" >
          <ItemStyle HorizontalAlign="Right" Width="80px"></ItemStyle>
      </asp:BoundField>
      <asp:BoundField DataField="Remarks" HeaderText="Remarks">
      <ItemStyle Height="20px" HorizontalAlign="Left" Width="200px" />
      </asp:BoundField>
  </Columns>
                        <RowStyle BackColor="White" />
                        <SelectedRowStyle BackColor="" Font-Bold="True" />
                        <PagerStyle BackColor="LightGray" ForeColor="Black" HorizontalAlign="Center" />
                        <AlternatingRowStyle BackColor="" />
</asp:GridView>

            </td>
            <td style="width: 20%">
                &nbsp;</td>
        </tr>
        <tr>
            <td style="width: 20%">
                &nbsp;</td>
            <td style="width: 60%">
                &nbsp;</td>
            <td style="width: 20%">
                &nbsp;</td>
        </tr>
        <tr>
            <td style="width: 20%">
                &nbsp;</td>
            <td style="width: 60%">
                &nbsp;</td>
            <td style="width: 20%">
                &nbsp;</td>
        </tr>
    </table>

</asp:Content>

