<%@ Page Title="" Language="C#" MasterPageFile="~/BranchMasterPage.master" AutoEventWireup="true" CodeFile="frmRejectItem.aspx.cs" Inherits="frmRejectItem" %>
<%@ Register TagPrefix="cc" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit, Version=3.0.30512.20315, Culture=neutral, PublicKeyToken=28f01b0e84b6d53e" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    
    <table  id="pageFooterWrapper">
  <tr>  
        <td align="center">
        <asp:Button ID="btnDelete" runat="server" ToolTip="Delete"
            
                
                
                
                onclientclick="javascript:return window.confirm('are u really want to delete these data')" Text="Delete" 
        Height="35px" Width="110px" BorderStyle="Outset" Visible="False"  />
        </td>       
        <td align="center" >
            &nbsp;</td>       
        <td align="center" >
        <asp:Button ID="btnSave" runat="server" ToolTip="Save Purchase Record" 
                onclick="btnSave_Click" Text="Save" 
        Height="35px" Width="110px" BorderStyle="Outset"  />
        </td>
        <td align="center" >
        <asp:Button ID="btnNew" runat="server" ToolTip="New" onclick="btnNew_Click"  Text="New" 
        Height="35px" Width="110px" BorderStyle="Outset" Visible="False"  /> 
        </td>
        <td align="center" >
        <asp:Button ID="btnClear" runat="server"  ToolTip="Clear"  Text="Clear" 
        Height="35px" Width="110px" BorderStyle="Outset" onclick="btnClear_Click"  />
        </td>
        <td align="center" >
        <asp:Button ID="btnPrint" runat="server" ToolTip="Print PO" Text="Print" 
        Height="35px" Width="110px" BorderStyle="Outset" 
                Visible="False"  />
        </td>            
        
   </tr>
   </table>
    <table style="width: 100%">
        <tr>
    <td style="width: 2%">&nbsp;</td>
    <td style="width: 96%">
       
        <table style="width: 100%">
            <tr>
                <td  style="width: 60%">
                    <fieldset style="vertical-align: top; border: solid 1px #8BB381;text-align:left;"><legend style="color: maroon;"><b>Search Items</b> </legend>
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 40%">
                                <asp:Label ID="Label40" runat="server" style="font-weight: 700" 
                                    Text="Search Items/Size/Price/Exp.Date ."></asp:Label>
                            </td>
                            <td style="width: 5%"> 
                                &nbsp;</td>
                            <td style="width: 40%">
                                &nbsp;</td>
                        </tr>
                        <tr>
                            <td colspan="3">
                <asp:TextBox ID="txtItemsCode" runat="server" Width="98%" AutoPostBack="True" 
                    CssClass="txtVisibleAlign" Height="18px" placeHolder="Search By Items.."
                    onfocus="this.select();" TabIndex="1"
                    ontextchanged="txtCode_TextChanged" ></asp:TextBox>
                <ajaxToolkit:AutoCompleteExtender ServicePath="AutoComplete.asmx" runat="server" 
                                                  ID="AutoCompleteExtender2" TargetControlID="txtItemsCode"
                                                  ServiceMethod="GetBranchItemsSearch" 
                    MinimumPrefixLength="1" CompletionInterval="20" EnableCaching="true" 
                                                  CompletionSetCount="12"/>
                            </td>
                        </tr>
                    </table>
                    </fieldset>
                </td>
                <td style="width: 40%">
                    <fieldset style="vertical-align: top; border: solid 1px #8BB381;"><legend style="color: maroon;">
                        <strong>Date</strong><b> No</b> </legend>
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 40%">
                                <asp:Label ID="Label43" runat="server" style="font-weight: 700" Text="Date"></asp:Label>
                            </td>
                            <td style="width: 5%">
                                &nbsp;</td>
                            <td style="width: 40%">
                                &nbsp;</td>
                        </tr>
                        <tr>
                            <td style="width: 40%">
                                <asp:TextBox ID="txtDate" runat="server" CssClass="txtVisibleAlign" 
                                    Height="18px" style="text-align:center;" TabIndex="3" Width="98%"></asp:TextBox>
                                <ajaxToolkit:CalendarExtender ID="Calendarextender1" runat="server" 
                                    Format="dd/MM/yyyy" TargetControlID="txtDate" />
                            </td>
                            <td style="width: 5%">
                                &nbsp;</td>
                            <td style="width: 40%">
              
                            </td>
                        </tr>
                    </table>
                    </fieldset>
                </td>
            </tr>
           
            </table>
        
    </td>
    <td style="width: 2%">
                <asp:TextBox ID="txtItemsID" runat="server" Width="15px" ForeColor="Red"  
                    BorderWidth="0px" 
                    style="border-color:none;border:0px; background:transparent;" Height="18px"></asp:TextBox>
            </td>
</tr>

<tr>
<td style="width:1%;">&nbsp;</td>
<td align="center" style="width: 98%" colspan="2"> 
    <asp:Panel ID="Panel1" runat="server" style="vertical-align: top; border: solid 1px #8BB381;">
   
        <%--<table style="width:100%;">
            <tr>
             <td align="center">--%>
    <asp:UpdatePanel ID="UpItemsDetails" runat="server">
    <ContentTemplate>
                 <table style="width: 100%">                
                     <tr>
                         <td>
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
                                                          AutoPostBack="true" Font-Size="8pt" CssClass="txtVisibleAlign" Height="18px"
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
                                                          onfocus="this.select();" ontextchanged="txtQty_TextChanged" 
                                                          onkeypress="return isNumber(event)"    style="text-align:center;" Text='<%# Eval("Qty", "{0:0}") %>' Width="90%"></asp:TextBox>
                                         </ItemTemplate>
                                         <ItemStyle Height="20px" Width="6%" />
                                     </asp:TemplateField>
                                     
                                     <asp:TemplateField HeaderText="Sale Price">
                                         <ItemTemplate>
                                             <asp:TextBox ID="txtSalesPrice" runat="server" AutoPostBack="True"  
                                                 CssClass="txtVisibleAlign" Height="18px" onkeypress="return isNumber(event)" 
                                                          onfocus="this.select();" style="text-align:center;" 
                                                          Text='<%# Eval("SPrice") %>' Width="92%" 
                                                          Enabled="False"></asp:TextBox>
                                         </ItemTemplate>
                                         <ItemStyle Height="20px" Width="8%" />
                                     </asp:TemplateField>

                                     <asp:BoundField DataField="Tax" HeaderText="Vat(%)">
                                             <ItemStyle Height="20px" HorizontalAlign="Center" Width="8%" />
                                     </asp:BoundField>                      

                                     <asp:TemplateField HeaderText="Dis(%)"> 
                                         <ItemTemplate>
                                             <asp:TextBox ID="txtDiscount" runat="server" AutoPostBack="True" 
                                                 CssClass="txtVisibleAlign" Height="18px"
                                                 onfocus="this.select();" style="text-align:center;" onkeypress="return isNumber(event)"  
                                                 Text='<%# Eval("DiscountAmount") %>' Width="90%" ReadOnly="True" 
                                                 Enabled="False"></asp:TextBox>
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
                     </tr>
                     <tr>
                         <td valign="top">
                            <asp:UpdatePanel ID="UPPaymentMtd" runat="server" UpdateMode="Conditional">
<ContentTemplate> 

    </ContentTemplate>
</asp:UpdatePanel>
                         </td>
                     </tr>
                 </table>
                            
         </ContentTemplate>
     </asp:UpdatePanel>
     </asp:Panel>
 </td>
<td style="width: 1%">&nbsp;</td>
</tr>
    </table>

</asp:Content>

