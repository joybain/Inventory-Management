<%@ Page Title="" Language="C#" MasterPageFile="~/BranchMasterPage.master" AutoEventWireup="true" CodeFile="frmBranchSalesExchange.aspx.cs" Inherits="frmBranchSalesExchange" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
<script language="javascript" type="text/javascript" >
    function isNumber(evt) {
        evt = (evt) ? evt : window.event;
        var charCode = (evt.which) ? evt.which : evt.keyCode;
        if (charCode != 46 && charCode > 31
            && (charCode < 48 || charCode > 57)) {
            return false;
        }
        return true;
    }
       
</script>  
    <table id="pageFooterWrapper">
    <tr>
        <td align="center" style="vertical-align:middle; height:100%;">
            <asp:Button ID="DeleteButton" runat="server" CssClass="buttonclass" 
                       Text="Delete" Width="100px" Height="35px" Visible="False" />
        </td>
   
        <td align="center" style="vertical-align:middle; height:100%;">
            <asp:Button ID="btnSave" runat="server" CssClass="buttonclass" 
                        onclick="btnSave_Click" Text="Save" Width="100px" Height="35px" />
        </td>

        <td align="center" style="vertical-align:middle;">
            <asp:Button ID="CloseButton" runat="server" CssClass="buttonclass" 
                        Text="Clear" Width="100px" Height="35px" 
                onclick="CloseButton_Click" />
        </td>
    </tr>
</table>
    <table style="width: 100%">
        
        <tr>
            <td style="width: 10%">
                &nbsp;</td>
            <td style="width: 80%">
            <fieldset style="vertical-align: top; border: solid 1px #8BB381; text-align: left">
                    <legend style="color: maroon;"><b>Search Invoice </b></legend>
                <table style="width: 100%">
                    <tr>
                          <td style="width: 5%">
                            &nbsp;</td>
                        <td style="width: 15%">
                            &nbsp;</td>
                        <td style="width: 2%">
                            &nbsp;</td>
                        <td style="width: 51%">
                            &nbsp;</td>
                              <td style="width: 2%">
                            &nbsp;</td>
                        <td style="width: 10%">
                            &nbsp;</td>
                        <td style="width: 5%">
                            &nbsp;</td>
                    </tr>
                    <tr>
                          <td style="width: 5%">
                            &nbsp;</td>
                        <td style="width: 15%; font-weight: 700;" align="right">
                            Search Invoice</td>
                        <td style="width: 2%">
                            &nbsp;</td>
                        <td style="width: 51%">
                            <asp:TextBox ID="txtSearchInvoice" runat="server" Width="100%"></asp:TextBox>
                              <ajaxToolkit:AutoCompleteExtender ServicePath="AutoComplete.asmx" runat="server" 
                                                  ID="AutoCompleteExtender2" TargetControlID="txtSearchInvoice"
                                                  ServiceMethod="GetBranchInvoiceSearch" 
                    MinimumPrefixLength="1" CompletionInterval="20" EnableCaching="true" 
                                                  CompletionSetCount="12"/>
                        </td>
                          <td style="width: 2%">
                            &nbsp;</td>
                        <td style="width: 10%; text-align: center;">
                                                        <asp:LinkButton ID="btnInvoiceSearch" 
                                runat="server" BorderStyle="Double" 
                                                            Font-Bold="True" Font-Size="10pt" 
                                Height="20px" OnClick="btnInvoiceSearch_Click" 
                                                            style="text-align: center" Width="70%">Search</asp:LinkButton>
                                                    </td>
                        <td style="width: 5%">
                    
                        </td>
                    </tr>
                
                   
                
                    <tr>
                          <td style="width: 5%">
                              &nbsp;</td>
                        <td style="width: 15%; font-weight: 700;" align="right">
                            Customer Name</td>
                        <td style="width: 2%; text-align: center;">
                            :</td>
                        <td style="width: 51%">
                            <asp:Label ID="lblCustomerName" runat="server"></asp:Label>
                          </td>
                          <td style="width: 2%">
                              &nbsp;</td>
                        <td style="width: 10%; text-align: center;">
                                                        &nbsp;</td>
                        <td style="width: 5%">
                    
                            &nbsp;</td>
                    </tr>
                
                   
                
                    <tr>
                          <td style="width: 5%">
                              &nbsp;</td>
                        <td style="font-weight: 700; text-align: center; margin-left: 80px;" 
                              align="right" colspan="5">
                         <asp:GridView ID="dgSV1" runat="server" AutoGenerateColumns="False" 
                                    BackColor="White" BorderColor="LightGray" BorderStyle="Solid" BorderWidth="1px" 
                                    CellPadding="2" CssClass="mGrid" Font-Size="9pt" 
                                    onrowdatabound="dgPVMst1_RowDataBound" PageSize="30" Width="100%">
                                    <Columns>
                                        <asp:BoundField DataField="ID" HeaderText="ID">
                                        <ItemStyle HorizontalAlign="Center" Width="5%" />
                                        </asp:BoundField>
                                      
                                        <%--    <asp:BoundField DataField="Code" HeaderText="Code">
                                            <ItemStyle HorizontalAlign="Center" Width="10%" />
                                     </asp:BoundField>--%>
                                        <asp:TemplateField HeaderText="Item Id">
                                            
                                            
                                            
                                            
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtItemId" runat="server" AutoPostBack="true" 
                                                             CssClass="txtVisibleAlign" Enabled="False" Font-Size="8pt" Height="18px" 
                                                             onFocus="this.select()"  SkinId="tbPlain" 
                                                             Text='<%#Eval("ItemID")%>' Width="97%"></asp:TextBox>
                                              
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Left" Width="30%" />
                                            


                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Item Code">
                                             <ItemTemplate>
                                                <asp:TextBox ID="txtItemCode" runat="server" AutoPostBack="true" 
                                                             CssClass="txtVisibleAlign" Enabled="False" Font-Size="8pt" Height="18px" 
                                                             onFocus="this.select()"  SkinId="tbPlain" 
                                                             Text='<%#Eval("ItemCode")%>' Width="97%"></asp:TextBox>
                                              
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Left" Width="30%" />
                                            
                                        </asp:TemplateField>
                                        
                                        
                                        
                                        <asp:TemplateField HeaderText="Unit Price">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtCostPrice" runat="server" AutoPostBack="true" 
                                                             CssClass="txtVisibleAlign" Enabled="False" Font-Size="8pt" Height="18px" 
                                                             onFocus="this.select()"  SkinId="tbPlain" 
                                                             Text='<%#Eval("CostPrice")%>' Width="97%"></asp:TextBox>
                                              
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Left" Width="30%" />
                                            
                                        </asp:TemplateField>
                                        
                                        
                                        

                                        <asp:TemplateField HeaderText="Item Name">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtItems" runat="server" AutoPostBack="true" 
                                                    CssClass="txtVisibleAlign" Enabled="False" Font-Size="8pt" Height="18px" 
                                                    onFocus="this.select()"  SkinId="tbPlain" 
                                                    Text='<%#Eval("ItemsName")%>' Width="97%"></asp:TextBox>
                                              
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Left" Width="30%" />
                                        </asp:TemplateField>
                                      <%--  <asp:TemplateField HeaderText="Model/Serial">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtSerial" runat="server" autocomplete="off" 
                                                    AutoPostBack="True" CssClass="txtVisibleAlign" Enabled="False" Font-Size="8pt" 
                                                    Height="18px" onFocus="this.select()"
                                                    SkinId="tbPlain" Text='<%# Eval("item_Serial") %>' Width="95%"></asp:TextBox>
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" Width="15%" />
                                        </asp:TemplateField>--%>
                                       
                                        <asp:BoundField DataField="TotalClosingStock" HeaderText="Stock">
                                        <ItemStyle Height="10px" HorizontalAlign="Right" Width="6%" />
                                        </asp:BoundField>
                                        <asp:TemplateField HeaderText="Qty">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtQty" Enabled="False" runat="server" autocomplete="off" AutoPostBack="True" 
                                                    CssClass="txtVisibleAlign" Height="18px" onfocus="this.select();"  style="text-align:center;" 
                                                    Text='<%# Eval("Qty", "{0:0}") %>' Width="90%"></asp:TextBox>
                                            </ItemTemplate>
                                            <ItemStyle Height="20px" Width="6%" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Sale Price">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtSalesPrice" Enabled="False" runat="server" AutoPostBack="True" 
                                                    CssClass="txtVisibleAlign" Height="18px" onfocus="this.select();" style="text-align:center;" 
                                                    Text='<%# Eval("SPrice") %>' Width="92%"></asp:TextBox>
                                            </ItemTemplate>
                                            <ItemStyle Height="20px" Width="8%" />
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="Tax" HeaderText="Vat(%)">
                                        <ItemStyle Height="20px" HorizontalAlign="Center" Width="8%" />
                                        </asp:BoundField>
                                        <asp:TemplateField HeaderText="Dis(TK)">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtDiscount" Enabled="False" runat="server" AutoPostBack="True" 
                                                    CssClass="txtVisibleAlign" Height="18px" onfocus="this.select();" ReadOnly="True" 
                                                    style="text-align:center;" Text='<%# Eval("DiscountAmount") %>' Width="90%"></asp:TextBox>
                                            </ItemTemplate>
                                            <ItemStyle Height="20px" Width="8%" />
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="Total" HeaderText="Total">
                                        <ItemStyle Height="20px" HorizontalAlign="Right" Width="8%" />
                                        </asp:BoundField>
                                        <asp:TemplateField HeaderText="ChangeQty">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtChangeQty" runat="server" autocomplete="off" AutoPostBack="True" 
                                                    CssClass="txtVisibleAlign" Height="18px" onfocus="this.select();" 
                                                     style="text-align:center;" 
                                                    Text='<%# Eval("ChangeQty", "{0:0}") %>' Width="90%"   onkeypress="return isNumber(event)"
                                                    ontextchanged="txtChangeQty_TextChanged"></asp:TextBox>
                                            </ItemTemplate>
                                            <ItemStyle Height="20px" Width="6%" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Barcode">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtBarcode" runat="server" autocomplete="off" 
                                                    CssClass="txtVisibleAlign" Enabled="False" Height="18px" 
                                                    onFocus="this.select();" style="text-align: center;" 
                                                    Text='<%# Eval("Barcode") %>' Width="90%"></asp:TextBox>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <RowStyle BackColor="White" />
                                    <SelectedRowStyle Font-Bold="True" />
                                    <PagerStyle BackColor="LightGray" CssClass="pgr" ForeColor="Black" 
                                        HorizontalAlign="Center" />
                                    <AlternatingRowStyle CssClass="alt" />
                                    <HeaderStyle Font-Size="9pt" />
                                </asp:GridView>
                         </td>
                        <td style="width: 5%">
                    
                            &nbsp;</td>
                    </tr>
                
                   
                
                    <tr>
                          <td style="width: 5%">
                              </td>
                        <td style="font-weight: 700; text-align: center; margin-left: 80px;" 
                              align="right" colspan="5">
                            <table style="width: 100%">
                                <tr>
                                    <td style="width: 16%; text-align: right;">
                                        <asp:Label ID="Label113" runat="server" Text="Sub Total :  "></asp:Label>
                                    </td>
                                     <td style="width: 16%; text-align: left;">
                                         <asp:Label ID="lblSubTotal" runat="server" Text="0"></asp:Label>
                                    </td>
                                      <td style="width: 16%; text-align: right;">
                                        <asp:Label ID="Label114" runat="server" Text="Vat Total :  "></asp:Label>
                                    </td>
                                       <td style="width: 16%; text-align: left;">
                                        
                                           <asp:Label ID="lblVat" runat="server" Text="0"></asp:Label>
                                        
                                    </td>
                                        <td style="width: 16%; text-align: right;"><asp:Label ID="Label1" runat="server" 
                                                Text="Total :  "></asp:Label></td>
                                       <td style="width: 16%; text-align: left;">
                                           <asp:Label ID="lblTotal" runat="server" Text="0"></asp:Label>
                                    </td>
                                        <td style="width: 4%"></td>
                                </tr>
                            </table>

                        </td>
                        <td style="width: 5%">
                    
                            &nbsp;</td>
                    </tr>
                
                   
                
                </table>
                </fieldset>
            </td>
            <td style="width: 10%">
                &nbsp;</td>
        </tr>
        <tr>
            <td style="width: 10%">
                &nbsp;</td>
                
                
            <td style="width: 80%">
                 <fieldset style="vertical-align: top; border: solid 1px #8BB381; text-align: left">
                    <legend style="color: maroon;"><b>New Sales List </b></legend>
                
                <asp:Panel ID="Panel1" runat="server">
                    <table style="width: 100%">
                     
                      
                      
                     
                        <tr>
                            <td style="width: 2%">
                                &nbsp;</td>
                              <td style="width: 93%">
                
        <table style="width: 100%">
            <tr>
                <td  style="width: 60%">
                    <fieldset style="vertical-align: top; border: solid 1px #8BB381;text-align:left;"><legend style="color: maroon;"><b>Search Items</b> </legend>
                    <table style="width: 100%">
                        <tr>
                            <td>
                                <asp:Label ID="Label40" runat="server" style="font-weight: 700" 
                                    Text="Search Items/Size/Price/Exp.Date ."></asp:Label>
                            </td>
                            <td style="width: 40%">
                                &nbsp;</td>
                        </tr>
                        <tr>
                            <td colspan="2">
                <asp:TextBox ID="txtItemsCode" runat="server" Width="98%" AutoPostBack="True" 
                    CssClass="txtVisibleAlign" Height="18px" placeHolder="Search By Items.."
                    onfocus="this.select();" TabIndex="1"
                    ontextchanged="txtCode_TextChanged" ></asp:TextBox>
                <ajaxToolkit:AutoCompleteExtender ServicePath="AutoComplete.asmx" runat="server" 
                                                  ID="AutoCompleteExtender1" TargetControlID="txtItemsCode"
                                                  ServiceMethod="GetItemsSearchBranch" 
                    MinimumPrefixLength="1" CompletionInterval="20" EnableCaching="true" 
                                                  CompletionSetCount="12"/>
                            </td>
                        </tr>
                    </table>
                    </fieldset>
                </td>
                <td style="width: 40%">
                    <fieldset style="vertical-align: top; border: solid 1px #8BB381;"><legend style="color: maroon;"><b>Invoice No</b> </legend>
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 40%">
                                <asp:Label ID="Label42" runat="server" style="font-weight: 700" 
                                    Text="Invoice No."></asp:Label>
                            </td>
                            <td style="width: 5%">
                                &nbsp;</td>
                            <td style="width: 40%">
                                <asp:Label ID="Label43" runat="server" style="font-weight: 700" Text="Date"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 40%">
                <asp:TextBox ID="txtInvoiceNo" runat="server" Width="100%"  CssClass="txtVisibleAlign" Height="18px"
                    style="text-align:center;" TabIndex="2"></asp:TextBox>

                     <ajaxToolkit:AutoCompleteExtender ServicePath="AutoComplete.asmx" runat="server" 
                                     ID="autoComplete1" TargetControlID="txtInvoiceNo"
           ServiceMethod="GetInvoice" MinimumPrefixLength="1" CompletionInterval="1000" EnableCaching="true" 
                                     CompletionSetCount="12"/>
                            </td>
                            <td style="width: 5%">
                                &nbsp;</td>
                            <td style="width: 40%">
                <asp:TextBox ID="txtDate" runat="server" Width="98%"  CssClass="txtVisibleAlign" Height="18px" style="text-align:center;" 
                    TabIndex="3"></asp:TextBox>
                <ajaxtoolkit:calendarextender runat="server" ID="Calendarextender1" 
                TargetControlID="txtDate" Format="dd/MM/yyyy"/>
                            </td>
                        </tr>
                    </table>
                    </fieldset>
                </td>
            </tr>
           
            </table>
      
    </td>
    <td style="width: 5%">
              
            </td>
                        </tr>
                    
                        <tr>
                            <td style="width: 2%">
                                &nbsp;</td>
                            <td style="width: 93%">
                             
                                <asp:GridView ID="dgSV" runat="server" 
                                 AutoGenerateColumns="False" BackColor="White" BorderColor="LightGray" 
                                 BorderStyle="Solid" BorderWidth="1px" CellPadding="2" CssClass="mGrid" 
                                 Font-Size="9pt" onrowdatabound="dgPVMst_RowDataBound" 
                                 onrowdeleting="dgSV_RowDeleting" PageSize="30" 
                                 Width="100%">
                                 <Columns>
                                     <asp:BoundField DataField="ID" HeaderText="ID">
                                     <ItemStyle HorizontalAlign="Center" Width="5%" />
                                     </asp:BoundField>
                                     <asp:TemplateField>
                                         <ItemTemplate>
                                             <asp:ImageButton ID="lbDelete" runat="server" CausesValidation="False" 
                                                 CommandName="Delete" ImageAlign="Middle" ImageUrl="~/img/delete.png" 
                                                 Text="Delete" />
                                         </ItemTemplate>
                                         <ItemStyle Font-Size="8pt" HorizontalAlign="Center" Width="4%" />
                                     </asp:TemplateField>
                                 <%--    <asp:BoundField DataField="Code" HeaderText="Code">
                                            <ItemStyle HorizontalAlign="Center" Width="10%" />
                                     </asp:BoundField>--%>
                                      <asp:TemplateField HeaderText="Item Name">
                                         <ItemTemplate>
                                          <asp:TextBox ID="txtItems" runat="server" Enabled="False"
                                          AutoPostBack="true" Font-Size="8pt" OnTextChanged="txtItems_TextChanged"  CssClass="txtVisibleAlign" Height="18px"
                                          SkinId="tbPlain" Text='<%#Eval("CodeWiseSearchItems")%>' Width="97%" onFocus="this.select()"></asp:TextBox>
                                           <ajaxToolkit:AutoCompleteExtender ServicePath="AutoComplete.asmx" runat="server" 
                                                                                  ID="AutoCompleteExtender1" TargetControlID="txtItems"
                                                                                  ServiceMethod="GetItemList" MinimumPrefixLength="1" 
                                                                     CompletionInterval="20" EnableCaching="true" 
                                                  CompletionSetCount="12"/>
                                       </ItemTemplate>
                                          <ItemStyle HorizontalAlign="Left" Width="30%" />
                                      </asp:TemplateField>

                                     <asp:TemplateField HeaderText="Barcode">
                                        <ItemTemplate>
                                            <asp:TextBox ID="txtSerial" runat="server" Enabled="False" autocomplete="off"
                                                         AutoPostBack="True" Font-Size="8pt" 
                                                OnTextChanged="txtItems_TextChanged"  CssClass="txtVisibleAlign" Height="18px"
                                                         SkinId="tbPlain" Text='<%# Eval("Barcode") %>' Width="95%" 
                                                onFocus="this.select()"></asp:TextBox>

                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" Width="15%" />
                                    </asp:TemplateField>
                                     <asp:TemplateField HeaderText="Remarks">
                                         <ItemTemplate>
                                             <asp:TextBox ID="txtRemarks" runat="server" Enabled="True" autocomplete="off"
                                                          AutoPostBack="true" Font-Size="8pt" OnTextChanged="txtRemarks_TextChanged"  CssClass="txtVisibleAlign" Height="18px"
                                                          SkinId="tbPlain" Text='<%#Eval("Remarks")%>' Width="95%" onFocus="this.select()"></asp:TextBox>
                                         </ItemTemplate>
                                         <ItemStyle HorizontalAlign="Left" Width="10%" />
                                     </asp:TemplateField>

                                     <asp:BoundField DataField="TotalClosingStock" HeaderText="Stock">
                                         <ItemStyle Height="10px" HorizontalAlign="Right" Width="6%" />
                                     </asp:BoundField>
                                     <asp:TemplateField HeaderText="Qty">
                                         <ItemTemplate>
                                             <asp:TextBox ID="txtQty" runat="server" AutoPostBack="True"  CssClass="txtVisibleAlign" Height="18px" autocomplete="off"
                                                          onfocus="this.select();" ontextchanged="txtQty_TextChanged"  onkeypress="return isNumber(event)"
                                                          style="text-align:center;" Text='<%# Eval("Qty", "{0:0}") %>' Width="90%"></asp:TextBox>
                                         </ItemTemplate>
                                         <ItemStyle Height="20px" Width="6%" />
                                     </asp:TemplateField>
                                     
                                     <asp:TemplateField HeaderText="Sale Price">
                                         <ItemTemplate>
                                             <asp:TextBox ID="txtSalesPrice" runat="server" AutoPostBack="True"  
                                                 CssClass="txtVisibleAlign" Height="18px"
                                                          onfocus="this.select();" style="text-align:center;" 
                                                          Text='<%# Eval("SPrice") %>' Width="92%" Enabled="False" 
                                                          ontextchanged="txtSalesPrice_TextChanged"></asp:TextBox>
                                         </ItemTemplate>
                                         <ItemStyle Height="20px" Width="8%" />
                                     </asp:TemplateField>

                                     <asp:BoundField DataField="Tax" HeaderText="Vat(%)">
                                             <ItemStyle Height="20px" HorizontalAlign="Center" Width="8%" />
                                     </asp:BoundField>                      

                                     <asp:TemplateField HeaderText="Dis(TK)"> 
                                         <ItemTemplate>
                                             <asp:TextBox ID="txtDiscount" runat="server" AutoPostBack="True" 
                                                 ontextchanged="txtDiscount_TextChanged"  CssClass="txtVisibleAlign" Height="18px"
                                                 onfocus="this.select();" style="text-align:center;"  Enabled="False"
                                                 Text='<%# Eval("DiscountAmount") %>' Width="90%" ReadOnly="True"></asp:TextBox>
                                         </ItemTemplate>
                                         <ItemStyle Height="20px" Width="8%" />
                                     </asp:TemplateField>
                                     
                                     <asp:BoundField DataField="Total" HeaderText="Total">
                                          <ItemStyle Height="20px" HorizontalAlign="Right" Width="8%" />
                                     </asp:BoundField>
                                    
                                   <%--  <asp:TemplateField HeaderText="Barcode">
                                         
                                         <ItemTemplate>
                                             <asp:TextBox ID="txtBarcode" runat="server" AutoPostBack="True"  CssClass="txtVisibleAlign" Height="18px" autocomplete="off"
                                                          onfocus="this.select();" 
                                                          style="text-align:Left;" Text='<%# Eval("Barcode") %>'></asp:TextBox>
                                         </ItemTemplate>
                                         

                                     </asp:TemplateField>--%>
                                    
                                 </Columns>
                                 <RowStyle BackColor="White" />
                                 <SelectedRowStyle Font-Bold="True" />
                                 <PagerStyle BackColor="LightGray" ForeColor="Black" HorizontalAlign="Center" 
                                     CssClass="pgr" />
                                 <AlternatingRowStyle CssClass="alt" />
                                 <HeaderStyle Font-Size="9pt" />
                             </asp:GridView>
                                </td>
                            <td style="width: 5%">
                                &nbsp;</td>
                        </tr>
                     
                        <tr>
                            <td style="width: 2%">
                                &nbsp;</td>
                            <td style="width: 93%">
                                <asp:Label ID="lblInvNo" runat="server" style="font-weight: 700" 
                                    Visible="False"></asp:Label>
                            </td>
                            <td style="width: 5%">
                                &nbsp;</td>
                        </tr>
                    </table>
                </asp:Panel>
                </fieldset>
            </td>
         
            <td style="width: 10%">
                &nbsp;</td>
        </tr>
        <tr>
            <td style="width: 10%">
                &nbsp;</td>
            <td style="width: 80%">
                &nbsp;&nbsp;&nbsp; </td>
            <td style="width: 10%">
                &nbsp;</td>
        </tr>
        <tr>
            <td style="width: 10%">
                &nbsp;</td>
            <td style="width: 80%">
 <table style="width: 100%">
        <tr>
            <td style="width: 65%" align="left" valign="top">
                <fieldset style="vertical-align: top; border: solid 1px #8BB381; text-align: left">
                    <legend style="color: maroon;"><b>Payment Status </b></legend>
                      <table style="width: 100%">
                        <tr>
                            <td style="width: 50%" valign="top">
                                <table style="width: 100%">
                                    <tr>
                                        <td align="right" style="width: 15%">
                                            <asp:Label ID="Label111" runat="server" Text=" Payment Type" Visible="False"></asp:Label>
                                           </td>
                                      
                                        <td align="center" style="width: 2%">
                                            &nbsp;</td>
                                        <td style="width: 20%">
                                            <asp:DropDownList ID="ddlPaymentTypeFrom" runat="server" AutoPostBack="True" 
                                                EnableTheming="True" 
                                                OnSelectedIndexChanged="ddlPaymentTypeFrom_SelectedIndexChanged" 
                                                Width="100%" Visible="False">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right" style="width: 15%">
                                            <asp:Label ID="lblBankNameFrom" runat="server" Text="Bank From"></asp:Label>
                                        </td>
                                        <td align="center" style="width: 2%">
                                            &nbsp;</td>
                                        <td style="width: 20%">
                                            <asp:DropDownList ID="ddlBankNameFrom" runat="server" EnableTheming="True" 
                                                Width="100%">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right" style="width: 15%">
                                            <asp:Label ID="lblAcountNoFrom" runat="server" Text="Account No"></asp:Label>
                                        </td>
                                        <td align="center" style="width: 2%">
                                            &nbsp;</td>
                                        <td style="width: 20%">
                                            <asp:TextBox ID="txtAccountNo" runat="server" autocomplete="off" CssClass="tbc" 
                                                Font-Size="8pt" onkeypress="return isNumber(event)" SkinID="tbPlain" 
                                                style="text-align: right" Width="96%"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right" style="width: 15%">
                                            <asp:Label ID="lblChekNo" runat="server" Text=" Card/Cheek No"></asp:Label>
                                        </td>
                                        <td align="center" style="width: 2%">
                                            &nbsp;</td>
                                        <td style="width: 20%">
                                            <asp:TextBox ID="txtcheeckNo" runat="server" Width="96%"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right" style="width: 15%">
                                            <asp:Label ID="lblApprovedDate" runat="server" Text="Approved Date"></asp:Label>
                                        </td>
                                        <td align="center" style="width: 2%">
                                            &nbsp;</td>
                                        <td style="width: 20%">
                                            <asp:TextBox ID="txtApprovedDate" runat="server" CssClass="tbc" Font-Size="8pt" 
                                                placeholder="dd/MM/yyyy" SkinID="tbPlain" style="text-align:center;" 
                                                Width="96%"></asp:TextBox>
                                            <cc:CalendarExtender ID="txtApprovedDate_CalendarExtender" runat="server" 
                                                Enabled="True" Format="dd/MM/yyyy" TargetControlID="txtApprovedDate">
                                            </cc:CalendarExtender>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right" style="width: 15%">
                                            <asp:Label ID="lblStatus" runat="server" Text="Status"></asp:Label>
                                        </td>
                                        <td align="center" style="width: 2%">
                                            &nbsp;</td>
                                        <td style="width: 20%">
                                            <asp:DropDownList ID="ddlPaymentStatus" runat="server" EnableTheming="True" 
                                                Width="100%">
                                                <asp:ListItem Value="0">Pending</asp:ListItem>
                                                <asp:ListItem Value="1">Approved</asp:ListItem>
                                                <asp:ListItem Value="2">bounced
                                                </asp:ListItem>
                                                <asp:ListItem Value="3">Cencel</asp:ListItem>
                                                <asp:ListItem Selected="True"></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td style="width: 50%" valign="top">
                                <table style="width: 100%">
                                    <tr>
                                        <td align="right" style="width: 15%">
                                            Received Type</td>
                                        <td align="center" style="width: 2%">
                                            <asp:Label ID="Label46" runat="server" ForeColor="#FF3300" Text="*"></asp:Label>
                                        </td>
                                        <td style="width: 20%">
                                            <asp:DropDownList ID="ddlPaymentTypeTo" runat="server" AutoPostBack="True" 
                                                EnableTheming="True" 
                                                OnSelectedIndexChanged="ddlPaymentType_SelectedIndexChanged" Width="100%">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    
                                    <tr>
                                        <td align="right" style="width: 15%">
                                            <asp:Label ID="lblBankNameTo" runat="server" Text="Receive Bank " 
                                                Visible="False"></asp:Label>
                                        </td>
                                        <td align="center" style="width: 2%">
                                            <asp:Label ID="lblRcbBankPoint" runat="server" ForeColor="#FF3300" Text="*"></asp:Label>
                                        </td>
                                        <td style="width: 20%">
                                            <asp:DropDownList ID="ddlBankName" runat="server" AutoPostBack="True" 
                                                EnableTheming="True" OnSelectedIndexChanged="ddlBankName_SelectedIndexChanged" 
                                                Width="100%" Visible="False">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right" style="width: 15%">
                                            <asp:Label ID="lblAcountNo" runat="server" Text="Account No" Visible="False"></asp:Label>
                                        </td>
                                        <td align="center" style="width: 2%">
                                            <asp:Label ID="lblAccNoPint" runat="server" ForeColor="#FF3300" Text="*"></asp:Label>
                                        </td>
                                        <td style="width: 20%">
                                            <asp:DropDownList ID="ddlAccountNo" runat="server" EnableTheming="True" 
                                                Width="100%" Visible="False">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right" style="width: 15%">
                                            Amount</td>
                                        <td align="center" style="width: 2%">
                                            <asp:Label ID="Label44" runat="server" ForeColor="#FF3300" Text="*"></asp:Label>
                                        </td>
                                        <td style="width: 20%">
                                            <asp:TextBox ID="txtAmount" runat="server" autocomplete="off" CssClass="tbc" 
                                                Font-Size="8pt" onkeypress="return isNumber(event)" SkinID="tbPlain" 
                                                style="text-align: right" Width="96%" AutoPostBack="True" 
                                                ontextchanged="txtAmount_TextChanged"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right" style="width: 15%">
                                            &nbsp;</td>
                                        <td align="center" style="width: 2%">
                                            &nbsp;</td>
                                        <td style="width: 20%">
                                            <table style="width: 100%">
                                                <tr>
                                                    <td align="center" style="width: 50%">
                                                        <asp:LinkButton ID="btnPaymentAdd" runat="server" BorderStyle="Double" 
                                                            Font-Bold="True" Font-Size="10pt" Height="20px" OnClick="btnPaymentAdd_Click" 
                                                            style="text-align: center" Width="90%" Visible="False">Add</asp:LinkButton>
                                                    </td>
                                                    <td align="center" style="width: 50%">
                                                        <asp:LinkButton ID="btnClearAll" runat="server" BorderStyle="Double" 
                                                            Font-Bold="True" Font-Size="10pt" Height="20px" OnClick="btnClearAll_Click" 
                                                            style="text-align: center" Width="90%" Visible="False">Clear</asp:LinkButton>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td align="right" colspan="2">
                                <asp:GridView ID="dgPaymentInfo" runat="server" AutoGenerateColumns="False" 
                                    BorderColor="LightGray" CssClass="mGrid" Font-Size="9pt" 
                                    OnRowDataBound="dgPaymentInfo_RowDataBound" Width="60%">
                                    <AlternatingRowStyle CssClass="alt" />
                                    <Columns>
                                        <asp:BoundField DataField="DtlID" HeaderText="DtlID">
                                            <ItemStyle HorizontalAlign="Center" Width="5%" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="PaymentypeFrom" HeaderText="Payment Type">
                                            <ItemStyle HorizontalAlign="Center" Width="15%" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="Amount" HeaderText="BD Amount">
                                            <ItemStyle HorizontalAlign="Right" Width="8%" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="Paymentype" HeaderText="ReceivedType">
                                            <ItemStyle HorizontalAlign="Right" Width="15%" />
                                        </asp:BoundField>
                                    </Columns>
                                    <HeaderStyle Font-Bold="True" Font-Size="9pt" ForeColor="White" />
                                    <PagerStyle CssClass="pgr" ForeColor="White" HorizontalAlign="Center" />
                                    <RowStyle BackColor="White" />
                                </asp:GridView>
                            </td>
                        </tr>
                    </table>
                </fieldset></td>
          <td style="width: 25%" align="left" valign="top">
                <fieldset style="vertical-align: top; border: solid 1px #8BB381;"><legend style="color: maroon;"><b>Payment Information</b> </legend>  
                <table style="width: 100%">
                    <tr>
                        <td style="width: 38%; height: 23px;" align="right">

                                                         <asp:Label ID="lblAmount" runat="server" 
                                Visible="False"></asp:Label>
                                                         Sub Total
                        </td>
                        <td style="width: 2%; height: 23px;">
                            </td>
                        <td style="width: 60%; height: 23px;">
                                                         <asp:TextBox ID="txtSubTotal" runat="server" Enabled="False" 
                                                             style="text-align:right;" TabIndex="4" 
                                Width="100%"></asp:TextBox>
                                                     </td>
                    </tr>
                    <tr>
                        <td style="width: 38%; height: 23px;" align="right">
                                                         VAT (%)</td>
                        <td style="width: 2%; height: 23px;">
                            </td>
                        <td style="width: 60%; height: 23px;">
                                                         <asp:TextBox ID="txtVat" runat="server" AutoPostBack="True" 
                                                             onfocus="this.select();" ontextchanged="txtVat_TextChanged"  onkeypress="return isNumber(event)"
                                                             style="text-align: right" Width="100%"></asp:TextBox>
                                                     </td>
                    </tr>
                    <tr>
                        <td style="width: 38%; height: 23px;" align="right">
                                                         Discount(TK)</td>
                        <td style="width: 2%; height: 23px;">
                            </td>
                        <td style="width: 60%; height: 23px;">
                            <asp:TextBox ID="txtDiscount" runat="server" AutoPostBack="True" 
                                onfocus="this.select();" ontextchanged="TotalDiscount_TextChange"   onkeypress="return isNumber(event)"
                                style="text-align:right;" TabIndex="6" Width="100%"></asp:TextBox>
                        </td>
                    </tr>
                    
                     <tr>
                        <td style="width: 38%; height: 27px;" align="right">
                            Exc Amount</td>
                        <td style="width: 2%; height: 27px;">
                            &nbsp;</td>
                        <td style="width: 60%; height: 27px;">
                            <asp:TextBox ID="txtExAmount" runat="server" 
                                         style="text-align:right;" TabIndex="8" 
                                         Width="100%" Enabled="False"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 38%; height: 24px;" align="right">
                                                         Grand Total</td>
                        <td style="width: 2%; height: 24px;">
                            </td>
                        <td style="width: 60%; height: 24px;">
                            <asp:TextBox ID="txtGrandTotal" runat="server" AutoPostBack="True" 
                                Enabled="False" onfocus="this.select();" style="text-align:right;" TabIndex="6" 
                                Width="100%"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 38%; height: 23px;" align="right">
                                                         Total Payment</td>
                        <td style="width: 2%; height: 23px;">
                            </td>
                        <td style="width: 60%; height: 23px;">
                            <asp:TextBox ID="txtPayment" runat="server" AutoPostBack="True" 
                                onfocus="this.select();" ontextchanged="TotalDiscount_TextChange" 
                                style="text-align:right;" TabIndex="7" Width="100%" Enabled="False"></asp:TextBox>
                            
                            <asp:TextBox ID="txtLastFigarAmount" runat="server" AutoPostBack="True" 
                                         onfocus="this.select();" 
                                         style="text-align:right;" TabIndex="7" Width="100%" 
                                Enabled="False" Visible="False" 
                                         ></asp:TextBox>
                          

                        </td>
                    </tr>
                    <tr>
                        <td style="width: 38%; height: 27px;" align="right">
                                                         <asp:Label ID="Label39" runat="server" 
                                Text="Due Amount"></asp:Label>
                                                     </td>
                        <td style="width: 2%; height: 27px;">
                            </td>
                        <td style="width: 60%; height: 27px;">
                                                         <asp:TextBox ID="txtDue" runat="server" 
                                style="text-align:right;" TabIndex="8" 
                                                             Width="100%" Enabled="False"></asp:TextBox>
                                                     </td>
                    </tr>
                   
                </table>
                </fieldset>
            </td>
        
        

        </tr>
    </table>
            </td>
            <td style="width: 10%">
                &nbsp;</td>
        </tr>
        <tr>
            <td style="width: 10%">
                &nbsp;</td>
            <td style="width: 80%">
                <table style="width: 100%">
                    <tr>
                        
                         <td style="width: 10%"> <asp:HiddenField ID="hfLocalCusAddress" runat="server" /></td>
                         <td style="width: 10%"><asp:HiddenField ID="hfLocalCusPhone" runat="server" /></td>
                          <td style="width: 10%"><asp:HiddenField ID="hfNote" runat="server" /></td>
                           <td style="width: 10%"><asp:HiddenField ID="hfLocalCustomer" runat="server" /></td>
                            <td style="width: 10%"><asp:HiddenField ID="hfGl_CoaCode" runat="server" /></td>
                             <td style="width: 10%"><asp:HiddenField ID="GrandTotal" runat="server" /></td>
                             <td style="width: 10%"><asp:HiddenField ID="hfsubTotal" runat="server" /></td>
                         <td style="width: 10%"><asp:HiddenField ID="hfDiscount" runat="server" /></td>
                          <td style="width: 10%"><asp:HiddenField ID="hfTotalOrderquantity" runat="server"/></td>
                           <td style="width: 10%"><asp:HiddenField ID="hfVat" runat="server"/></td>   
                    </tr>
                    <tr>
                                
                             
                              
                               
                               
                        <td style="width: 10%"><asp:HiddenField ID="hfInvoiceNo" runat="server" />
                        </td>
                        <td style="width: 10%"> <asp:HiddenField ID="hfCustomerID" runat="server" />
                        </td>
                        <td style="width: 10%"> <asp:HiddenField ID="hfCustomerName" runat="server" />
                        </td>
                        <td style="width: 10%"> <asp:HiddenField ID="hfRemark" runat="server" />
                        </td>
                        <td style="width: 10%"> <asp:HiddenField ID="hfOrderId" runat="server" />
                        </td>
                        <td style="width: 10%">
                              <asp:TextBox ID="txtItemsID" runat="server" Width="16px" ForeColor="Red"  
                    BorderWidth="0px" style="border-color:none;border:0px; background:transparent;"></asp:TextBox>
                        </td>
                        <td style="width: 10%">
                            <asp:HiddenField ID="hfCommonCus" runat="server" />
                        </td>
                        <td style="width: 10%">
                            <asp:HiddenField ID="hfSetupTotalDiscount" runat="server" />
                        </td>
                        <td style="width: 10%">
                        </td>
                        <td style="width: 10%">
                        </td>
                    </tr>
                </table>
                
                
                
            </td>
            <td style="width: 10%">
                &nbsp;</td>
        </tr>
        <tr>
            <td style="width: 10%">
                &nbsp;</td>
            <td style="width: 80%">
                &nbsp;</td>
            <td style="width: 10%">
                &nbsp;</td>
        </tr>
    </table>


</asp:Content>
