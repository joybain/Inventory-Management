<%@ Page Title="Sales/Invoice Return.-RD" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="SalesReturn.aspx.cs" Inherits="SalesReturn" %>
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

<div id="frmMainDiv" style="background-color:White; width:100%;"> 
<table  id="pageFooterWrapper">
  <tr>  
        <td align="center">
        <asp:Button ID="btnDelete" runat="server" ToolTip="Delete" onclick="Delete_Click"
            
                
                onclientclick="javascript:return window.confirm('are u really want to delete these data')" Text="Delete" 
        Height="35px" Width="110px" BorderStyle="Outset"  />
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
        Height="35px" Width="110px" BorderStyle="Outset"  /> 
        </td>
        <td align="center" >
        <asp:Button ID="btnClear" runat="server"  ToolTip="Clear" onclick="Clear_Click" Text="Clear" 
        Height="35px" Width="110px" BorderStyle="Outset"  />
        </td>
        <td align="center" >
        <asp:Button ID="btnPrint" runat="server" ToolTip="Print PO" Text="Print" 
        Height="35px" Width="110px" BorderStyle="Outset" onclick="btnPrint_Click"  />
        </td>            
        
   </tr>
   </table>
   <br />
     
 <table style="width:100%;">
<tr>
<td style="width:1%;" align="center"></td>
<td style="width:98%;" align="left" valign="top">
<asp:Label ID="lblBranchName" runat="server" Font-Size="10pt" ForeColor="Black" 
                   style="font-weight: 700" Font-Underline="True" Visible="False"></asp:Label>

 <asp:UpdatePanel ID="PVI_UP" runat="server" UpdateMode="Conditional"> 
 <ContentTemplate>
<fieldset style="vertical-align: top; border: solid 1px #8BB381; text-align:left;"><legend style="color: maroon;"><b>
    Sales Return </b> </legend>
<table id="Table1" style="width:100%;">
	<tr >
	<td style="width: 15%; height: 31px;" align="left">
	    <asp:Label ID="LblSuppNo0" runat="server" Font-Size="9pt">Search Invoice No</asp:Label>
        &nbsp;<asp:Label ID="lblPVID" runat="server" Font-Size="9pt" Visible="False"></asp:Label>
        </td>
	<td style="width: 32%; height: 31px;" align="left">
        <asp:TextBox ID="txtGoodsReceiveNo" runat="server" AutoPostBack="True" placeholder="Search By Invoice No."
            CssClass="txtVisibleAlign" Font-Size="8pt" SkinId="tbPlain" style="text-align:left;" 
            Width="80%" ontextchanged="txtGoodsReceiveNo_TextChanged" Height="18px"></asp:TextBox>
            <ajaxToolkit:AutoCompleteExtender ServicePath="AutoComplete.asmx" runat="server" 
                                     ID="autoComplete1" TargetControlID="txtGoodsReceiveNo"
           ServiceMethod="GetBranchInvoice" MinimumPrefixLength="1" CompletionInterval="20" EnableCaching="true" 
                                     CompletionSetCount="12"/>
    </td>
    <td style=" width:3%; height: 31px;">
        <asp:Label ID="lbLId" runat="server" Font-Size="9pt" Visible="False"></asp:Label>
        </td>
	<td style="width: 15%; height: 31px;" align="left">
	<asp:Label ID="lblQuotDate" runat="server" Font-Size="9pt">S. Return No.</asp:Label></td>
	<td style="width: 30%; height: 31px;" align="left">
    <asp:TextBox SkinId="tbPlain" ID="txtReturnNO" runat="server" Width="60%" style="text-align:center;"
            CssClass="tbc"  AutoPostBack="False"  Font-Size="8pt"></asp:TextBox>
    </td>    
    </tr>

    <tr >
	<td style="width: 15%; height: 29px;" align="left">
    <asp:Label ID="LblSuppNo" runat="server" Font-Size="9pt" >Customer</asp:Label>
        <asp:Label ID="lblGlCoa" runat="server" Font-Size="9pt" Visible="False"></asp:Label>
        <asp:Label ID="lblSupID" runat="server" Font-Size="9pt" Visible="False"></asp:Label>
        </td>
	<td style="width: 32%; height: 29px;" align="left">
        <asp:TextBox ID="txtSupplier" runat="server" AutoPostBack="False" 
            CssClass="txtVisible" Font-Size="8pt" SkinId="tbPlain" style="text-align:left;" 
            Width="80%" Enabled="False"></asp:TextBox>
    </td>
    <td style=" width:3%; height: 29px;">
        <asp:Label ID="lblBranchID" runat="server" Visible="False"></asp:Label>
        </td>
	<td style="width: 15%; height: 29px;" align="left">
	    <asp:Label ID="lblQuotDate0" runat="server" Font-Size="9pt">Return Date</asp:Label>
        </td>
	<td style="width: 30%; height: 29px;" align="left">
        <asp:TextBox ID="txtReturnDate" runat="server" AutoPostBack="False" 
            CssClass="tbc" Font-Size="8pt" SkinId="tbPlain" style="text-align:center;" 
            Width="60%"></asp:TextBox>
        <ajaxToolkit:CalendarExtender ID="txtReturnDate_CalendarExtender" 
            runat="server" Format="dd/MM/yyyy" TargetControlID="txtReturnDate" />
        </td>    
    </tr>

    <tr>
    <td style="width: 15%; font-weight: 700;" align="left" valign="top">
        Remarks/Particulars :
        </td>
    <td align="left" colspan="4">
        <asp:TextBox ID="txtRemarks" runat="server" AutoPostBack="False" 
                    Font-Size="8pt" Height="51px" SkinID="tbGray" TextMode="MultiLine" Width="99%"></asp:TextBox>
                <cc:AutoCompleteExtender ID="txtRemarks_AutoCompleteExtender" runat="server" 
                    CompletionInterval="20" CompletionSetCount="12" EnableCaching="true" 
                    MinimumPrefixLength="1" ServiceMethod="GetShowRemarks" 
                    ServicePath="AutoComplete.asmx" TargetControlID="txtRemarks" />
    </td>    
    </tr>    

    </table>  
    </fieldset>
    </ContentTemplate></asp:UpdatePanel>    
  <asp:GridView CssClass="mGrid" PagerStyle-CssClass="pgr"  
        AlternatingRowStyle-CssClass="alt" ID="dgPRNMst" runat="server" AutoGenerateColumns="False" 
        AllowPaging="True"  BackColor="White" BorderWidth="1px" BorderStyle="Solid"
        CellPadding="2" BorderColor="LightGray" Font-Size="9pt" 
        AllowSorting="True" PageSize="30" Width="100%" 
        onpageindexchanging="dgPRNMst_PageIndexChanging" 
        onrowdatabound="dgPRNMst_RowDataBound" 
        onselectedindexchanged="dgPRNMst_SelectedIndexChanged" >
  <HeaderStyle Font-Size="9pt"  Font-Bold="True" BackColor="LightGray" HorizontalAlign="center"  ForeColor="Black" />
  <Columns>  
  <asp:CommandField ShowSelectButton="True"  ItemStyle-Width="40px" 
          ItemStyle-HorizontalAlign="Center">
<ItemStyle HorizontalAlign="Center" Width="40px"></ItemStyle>
      </asp:CommandField>
  <asp:BoundField  HeaderText="Return No" DataField="Return_No" ItemStyle-Width="100px" 
          ItemStyle-HorizontalAlign="Center">    
<ItemStyle HorizontalAlign="Center" Width="100px"></ItemStyle>
      </asp:BoundField>
  <asp:BoundField  HeaderText="Return Date" DataField="ReturnDate" 
          DataFormatString="{0:dd/MM/yyyy}" ItemStyle-Width="100px" 
          ItemStyle-HorizontalAlign="Center">
          <ItemStyle HorizontalAlign="Center" Width="100px"></ItemStyle>
      </asp:BoundField>
      <asp:BoundField DataField="GRN" 
          HeaderText="Invoice" >
          <ItemStyle HorizontalAlign="Center" Width="100px"></ItemStyle>
      </asp:BoundField>
       <asp:BoundField DataField="ContactName" 
          HeaderText="Customer">
          <ItemStyle HorizontalAlign="Center" Width="100px"></ItemStyle>
      </asp:BoundField>
      <asp:BoundField HeaderText="Total" DataField="TotalAmount">
      <ItemStyle HorizontalAlign="Right" Width="80px"></ItemStyle>
      </asp:BoundField>
      <asp:BoundField  HeaderText="ID" DataField="ID"  ItemStyle-Width="80px" 
          ItemStyle-HorizontalAlign="Left" > 
<ItemStyle HorizontalAlign="Left" Width="80px"></ItemStyle>
      </asp:BoundField>
  <asp:BoundField  HeaderText="Remark's" DataField="Remarks" 
          ItemStyle-Height="20" ItemStyle-Width="200px" 
          ItemStyle-HorizontalAlign="Left">
<ItemStyle HorizontalAlign="Left" Height="20px" Width="200px"></ItemStyle>
      </asp:BoundField>
  </Columns>
                        <RowStyle BackColor="White" />
                        <SelectedRowStyle BackColor="" Font-Bold="True" />
                        <PagerStyle BackColor="LightGray" ForeColor="Black" HorizontalAlign="Center" />
                        <AlternatingRowStyle BackColor="" />
</asp:GridView>

<%--<CR:CrystalReportViewer ID="CrystalReportViewer1" runat="server" 
    AutoDataBind="true" />--%>
 </td>
 <td style="width:1%;" align="center"></td>
</tr>
<tr>
<td style="width:1%;" align="center">&nbsp;</td>
<td style="width:98%;" align="center"> 	
<%--<div runat="server" id="PVDetails">--%>
<asp:Panel ID="pnlVch" runat="server" Width="100%">
<div style="font-size: 8pt;" align="center">
<asp:UpdatePanel ID="PVIesms_UP" runat="server"  UpdateMode="Conditional">
 <ContentTemplate>
<%--<ajaxtoolkit:TabContainer ID="tabVch" runat="server" Width="99%" ActiveTabIndex="0" 
         Font-Size="8pt">
 <ajaxtoolkit:TabPanel ID="tpVchDtl" runat="server" HeaderText="Items Details">
     <ContentTemplate>--%>
         <table style="width:100%;"><tr>
             <td colspan="2" align="center" style="font-weight: 700">             
                 <asp:GridView ID="dgPODetailsDtl" runat="server" AutoGenerateColumns="False"   
            BorderColor="LightGray" CssClass="mGrid" Font-Size="9pt"  
            OnRowDataBound="dgPurDtl_RowDataBound" OnRowDeleting="dgPurDtl_RowDeleting" 
            Width="98%"><AlternatingRowStyle CssClass="alt" /><Columns>
                         <asp:TemplateField><ItemTemplate>
                             <asp:ImageButton ID="lbDelete" runat="server" CausesValidation="False" 
                                 CommandName="Delete" ImageAlign="Middle" ImageUrl="~/img/delete.png" 
                                 Text="Delete" /></ItemTemplate>
                             <ItemStyle Font-Size="8pt" HorizontalAlign="Center" Width="4%" /></asp:TemplateField>
                         <asp:TemplateField HeaderText="Item Code">
                             <ItemTemplate>
                                 <asp:TextBox ID="txtItemCode" runat="server" 
                                     AutoPostBack="true" Font-Size="8pt" MaxLength="15" SkinId="tbPlain" Text='<%#Eval("item_code")%>' Width="93%" onFocus="this.select()">
                                     </asp:TextBox>
                             </ItemTemplate>
                             <FooterStyle HorizontalAlign="Center" />
                             <ItemStyle HorizontalAlign="Center" Width="12%" /></asp:TemplateField>
                         <asp:TemplateField HeaderText="Description">
                             <ItemTemplate>
                                 <asp:DropDownList ID="DropDownList1" runat="server" Height="26px" 
                                     onselectedindexchanged="DropDownList1_SelectedIndexChanged" Width="95%"  
                                     DataSource="<%#PopulatePayType()%>" DataTextField="Items_Name" 
                            DataValueField="ItemID"
                                     SelectedValue='<%# Eval("item_desc") %>' AutoPostBack="True">
                                     <asp:ListItem></asp:ListItem>
                                 </asp:DropDownList>
                             </ItemTemplate>
                             <ItemStyle HorizontalAlign="Left" Width="30%" /></asp:TemplateField>
                         <asp:TemplateField HeaderText="Msr Unit">
                             <ItemTemplate>
                                 <asp:DropDownList ID="ddlMeasure" 
                                     runat="server" AutoPostBack="true" 
           DataSource="<%#PopulateMeasure()%>" DataTextField="Name" DataValueField="ID" Font-Size="8pt" 
           SelectedValue='<%#Eval("msr_unit_code")%>' SkinId="ddlPlain" Width="95%" Height="26px" 
                                     Enabled="False"></asp:DropDownList></ItemTemplate>
                             <ItemStyle HorizontalAlign="Center" Width="10%" /></asp:TemplateField>
                             <asp:TemplateField HeaderText="Sale Quantity">
                             <ItemTemplate>
                                 <asp:TextBox ID="txtSaleQnty" runat="server" 
                                     AutoPostBack="true" CssClass="tbc"  Enabled="false"
           Font-Size="8pt" SkinId="tbPlain" onkeypress="return isNumber(event)"
           Text='<%#Eval("salqnty")%>' Width="93%" onFocus="this.select()"></asp:TextBox></ItemTemplate>
                             <ItemStyle HorizontalAlign="Center" Width="10%" /></asp:TemplateField>
                             
                         <asp:TemplateField HeaderText="Discount(-)">
                             <ItemTemplate>
                                 <asp:TextBox ID="txtdiscountAmt" runat="server" 
                                              AutoPostBack="False" CssClass="tbr" Font-Size="8pt"  onkeypress="return isNumber(event)"
                                              SkinId="tbPlain" Text='<%#Eval("discountAmt")%>' Width="93%" onFocus="this.select()" Enabled="False"></asp:TextBox>
                                <asp:TextBox ID="txtdiscountAmtHf" runat="server" 
                                              AutoPostBack="False" CssClass="tbr" Font-Size="8pt"  onkeypress="return isNumber(event)"
                                              SkinId="tbPlain" Visible="False" Text='<%#Eval("discountAmtfix")%>' Width="93%" onFocus="this.select()" Enabled="False"></asp:TextBox>
                                
                             </ItemTemplate>
                             <ItemStyle HorizontalAlign="Right" Width="10%" />
                         </asp:TemplateField>
                         
                         
                         
                         
                         
                         
                          <asp:TemplateField HeaderText="Vat">
                             <ItemTemplate>
                                 <asp:TextBox ID="txtVat" runat="server" 
                                              AutoPostBack="False" CssClass="tbr" Font-Size="8pt"  onkeypress="return isNumber(event)"
                                              SkinId="tbPlain" Text='<%#Eval("TaxAmount")%>' Width="93%" onFocus="this.select()" Enabled="False"></asp:TextBox>
                               
                                <asp:TextBox ID="txtVatHf" runat="server" 
                                              AutoPostBack="False" CssClass="tbr" Font-Size="8pt"  onkeypress="return isNumber(event)"
                                              SkinId="tbPlain" Visible="False" Text='<%#Eval("TaxAmountfix")%>' Width="93%" onFocus="this.select()" Enabled="False"></asp:TextBox>
                               
                             </ItemTemplate>
                             <ItemStyle HorizontalAlign="Right" Width="10%" />
                         </asp:TemplateField>
                         
                         
                         
                         
                         

                         <asp:TemplateField HeaderText="Item Rate">
                             <ItemTemplate>
                                 <asp:TextBox ID="txtItemRate" runat="server" 
                                     AutoPostBack="False" CssClass="tbr" Font-Size="8pt"  onkeypress="return isNumber(event)"
           SkinId="tbPlain" Text='<%#Eval("item_rate")%>' Width="93%" onFocus="this.select()" Enabled="False"></asp:TextBox>
                             </ItemTemplate><ItemStyle HorizontalAlign="Right" Width="10%" />
                             </asp:TemplateField>

                         <asp:TemplateField HeaderText="Return Quantity">
                             <ItemTemplate>
                                 <asp:TextBox ID="txtQnty" runat="server" 
                                     AutoPostBack="true" CssClass="tbc" 
           Font-Size="8pt" OnTextChanged="txtQnty_TextChanged" SkinId="tbPlain" onkeypress="return isNumber(event)"
           Text='<%#Eval("qnty")%>' Width="93%" onFocus="this.select()"></asp:TextBox></ItemTemplate>
                             <ItemStyle HorizontalAlign="Center" Width="10%" /></asp:TemplateField>
                         <asp:TemplateField HeaderText="Total">
                             <ItemTemplate>
                                 <asp:Label ID="lblTotal" runat="server" 
                                     Font-Size="8pt" Width="95%">
           </asp:Label></ItemTemplate><FooterStyle HorizontalAlign="Center" />
                             <ItemStyle HorizontalAlign="Right" Width="10%" /></asp:TemplateField>
                         <asp:TemplateField HeaderText="ID">
                             <ItemTemplate>
                                 <asp:Label ID="lblid" runat="server" 
                                     Font-Size="8pt" Width="95%" Text='<%#Eval("ID")%>'></asp:Label></ItemTemplate>
                             <FooterStyle HorizontalAlign="Center" />
                             <ItemStyle HorizontalAlign="Right" Width="10%" /></asp:TemplateField></Columns>
                     <HeaderStyle Font-Bold="True" Font-Size="9pt" ForeColor="White" />
                     <PagerStyle CssClass="pgr" ForeColor="White" 
                         HorizontalAlign="Center" />
                     <RowStyle BackColor="White" /></asp:GridView></td></tr>
                 <tr>
                     <td style="width:50%;">
                     <fieldset style=" border:solid 1px #8BB381;text-align:left;"><legend>Payment Methord</legend>
           <asp:UpdatePanel ID="UPPaymentMtd" runat="server" UpdateMode="Conditional">
           <ContentTemplate>               
                         <table style="width: 100%" >
                             <tr>
                                 <td style="width:20%; height: 26px;" align="left">
                                     <asp:Label ID="Label26" runat="server" Font-Size="9pt">P. Methord</asp:Label>
                                 </td>
                                 <td style="width:30%; height: 26px;">
                                     <asp:DropDownList ID="ddlPaymentMethord" runat="server" AutoPostBack="True" 
                                         Font-Size="8pt" Height="24px" 
                                         onselectedindexchanged="ddlPaymentMethord_SelectedIndexChanged" 
                                         SkinID="ddlPlain" TabIndex="19" Width="100%">
                                         <asp:ListItem Text="Cash" Value="C"></asp:ListItem>
                                        <%-- <asp:ListItem Value="Q">Cheque</asp:ListItem>--%>
                                     </asp:DropDownList>
                                 </td>
                                 <td style="width:3%; height: 26px;"></td>
                                 <td style="width:20%; height: 26px;" align="left">
                                     <asp:Label ID="lblChequeNo" runat="server" Font-Size="9pt">Cheque/Card No.</asp:Label>
                                 </td>
                                 <td style="width:30%; height: 26px;">
                                     <asp:TextBox ID="txtChequeNo" runat="server" Font-Size="8pt" SkinID="tbPlain" 
                                         TabIndex="22" Width="94%"></asp:TextBox>
                                 </td>
                             </tr>
                             <tr>
                                 <td align="left" style="width:20%; height: 27px;">
                                     <asp:Label ID="lblBankName" runat="server" Font-Size="9pt">Bank Name</asp:Label>
                                 </td>
                                 <td style="width:30%; height: 27px;">
                                     <asp:DropDownList ID="ddlBank" runat="server" Font-Size="8pt" Height="24px" 
                                         SkinID="ddlPlain" TabIndex="21" Width="100%">
                                     </asp:DropDownList>
                                 </td>
                                 <td style="width:3%; height: 27px;">
                                     </td>
                                 <td align="left" style="width:20%; height: 27px;">
                                     <asp:Label ID="lblChequeDate" runat="server" Font-Size="9pt">Cheque date</asp:Label>
                                 </td>
                                 <td style="width:30%; height: 27px;">
                                     <asp:TextBox ID="txtChequeDate" runat="server" Font-Size="8pt" SkinId="tbPlain" 
                                         TabIndex="23" Width="94%"></asp:TextBox>
                                     <ajaxToolkit:CalendarExtender ID="txtChequeDate_CalendarExtender" 
                                         runat="server" Enabled="True" Format="dd/MM/yyyy" 
                                         TargetControlID="txtChequeDate">
                                     </ajaxToolkit:CalendarExtender>
                                 </td>
                             </tr>
                             <tr>
                                 <td align="left" style="width:20%; height: 26px;">
                                     <asp:Label ID="lblChequeStatus" runat="server" Font-Size="9pt">Check Status</asp:Label>
                                 </td>
                                 <td style="width:30%; height: 26px;">
                                     <asp:DropDownList ID="ddlChequeStatus" runat="server" Font-Size="8pt" 
                                         Height="24px" 
                                         SkinID="ddlPlain" TabIndex="24" Width="100%">
                                         <asp:ListItem></asp:ListItem>
                                         <asp:ListItem Value="P">Pending</asp:ListItem>
                                         <asp:ListItem Value="A">Approved</asp:ListItem>
                                         <asp:ListItem Value="B">Bounce</asp:ListItem>
                                     </asp:DropDownList>
                                 </td>
                                 <td style="width:3%; height: 26px;">
                                     </td>
                                 <td style="width:20%; height: 26px;">
                                     </td>
                                 <td style="width:30%; height: 26px;">
                                     </td>
                             </tr>
                         </table></ContentTemplate></asp:UpdatePanel></fieldset>
                     </td>
                     <td align="right" valign="top" style="width:35%;">
                      <fieldset style=" border:solid 1px #8BB381;text-align:left;"><legend>Payment Information</legend>
                         <table style="width: 100%">
                             <tr>
                                 <td style="width:50%; height: 22px;" align="right">
                                     <asp:Label ID="Label7" runat="server" Text="Total"></asp:Label>
                                 </td>
                                 <td style="width:3%; height: 22px;"></td>
                                 <td style="width:30%; height: 22px;">
                                     <asp:TextBox ID="txtTotal" runat="server" style="text-align: right;" onkeypress="return isNumber(event)"
                                         Width="98%"></asp:TextBox>
                                 </td>
                             </tr>                            
                             <tr>
                                 <td style="width: 50%;" align="right">
                                     <asp:Label ID="Label8" runat="server" Text="Total Payment"></asp:Label>
                                 </td>
                                 <td style="width: 3%;">
                                     &nbsp;</td>
                                 <td style="width: 30%;">
                                     <asp:TextBox ID="txtTotPayment" runat="server" style="text-align: right;" onkeypress="return isNumber(event)"
                                         Width="98%" AutoPostBack="True" ontextchanged="txtTotPayment_TextChanged"></asp:TextBox>
                                 </td>
                             </tr>
                             <tr>
                                 <td style="width: 50%;" align="right">
                                     <asp:Label ID="Label9" runat="server" Text="Total Due"></asp:Label>
                                 </td>
                                 <td style="width: 3%;">
                                     &nbsp;</td>
                                 <td style="width: 30%;">
                                     <asp:TextBox ID="txtDue" runat="server" style="text-align: right;" 
                                         onkeypress="return isNumber(event)" Width="98%" Enabled="False"></asp:TextBox>
                                 </td>
                             </tr>
                         </table></fieldset>
                     </td>
             </tr>
                 </table>
            </ContentTemplate><%--</ajaxtoolkit:TabPanel></ajaxtoolkit:TabContainer>--%>
        </asp:UpdatePanel>
    </div>
    </asp:Panel>
 </td>
 <td style="width:1%;" align="center">&nbsp;</td>
</tr>
</table>
</div>
</asp:Content>



