﻿<%@ Page Title="Purchase Return.-DDP" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="PurchaseReturn.aspx.cs" Inherits="PurchaseReturn" %>

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
<td style="width:98%;" align="center" valign="top"> 	
<br />
 <asp:UpdatePanel ID="PVI_UP" runat="server" UpdateMode="Conditional"> 
 <ContentTemplate>
<fieldset style="vertical-align: top; border: solid 1px #8BB381; text-align:left;"><legend style="color: maroon;"><b>
    Purchase Return </b> </legend>
<table id="Table1" style="width:100%;">
	<tr >
	<td style="width: 15%; height: 27px;" align="left">
	    <asp:Label ID="LblSuppNo0" runat="server" Font-Size="9pt">Goods Receive No</asp:Label>
        &nbsp;<asp:Label ID="lblPVID" runat="server" Font-Size="9pt" Visible="False"></asp:Label>
        <asp:Label ID="lblType" runat="server" Font-Size="9pt" Visible="False"></asp:Label>
        </td>
	<td style="width: 32%; height: 27px;" align="left">
        <asp:TextBox ID="txtGoodsReceiveNo" runat="server" AutoPostBack="True" placeholder="Search By Goods Receive No"
            CssClass="tbc" Font-Size="8pt" SkinId="tbPlain" style="text-align:left;" 
            Width="60%" ontextchanged="txtGoodsReceiveNo_TextChanged"></asp:TextBox>
            <ajaxToolkit:AutoCompleteExtender ServicePath="AutoComplete.asmx" runat="server" 
                                     ID="autoComplete1" TargetControlID="txtGoodsReceiveNo"
           ServiceMethod="GetGRN" MinimumPrefixLength="1" CompletionInterval="20" EnableCaching="true" 
                                     CompletionSetCount="12"/>
    </td>
    <td style=" width:3%; height: 27px;">
        <asp:Label ID="lbLId" runat="server" Font-Size="9pt" Visible="False"></asp:Label>
        </td>
	<td style="width: 15%; height: 27px;" align="left">
	<asp:Label ID="lblQuotDate" runat="server" Font-Size="9pt">P. Return No.</asp:Label></td>
	<td style="width: 30%; height: 27px;" align="left">
    <asp:TextBox SkinId="tbPlain" ID="txtReturnNO" runat="server" Width="50%" style="text-align:center;"
            CssClass="tbc"  AutoPostBack="False"  Font-Size="8pt"></asp:TextBox>
    </td>    
    </tr>

    <tr >
	<td style="width: 15%; height: 27px;" align="left">
    <asp:Label ID="LblSuppNo" runat="server" Font-Size="9pt" >Supplier</asp:Label>
        <asp:Label ID="lblGlCoa" runat="server" Font-Size="9pt" Visible="False"></asp:Label>
        <asp:Label ID="lblSupID" runat="server" Font-Size="9pt" Visible="False"></asp:Label>
        </td>
	<td style="width: 32%; height: 27px;" align="left">
        <asp:TextBox ID="txtSupplier" runat="server" AutoPostBack="False" placeholder="Search By Supplier.."
            CssClass="tbc" Font-Size="8pt" SkinId="tbPlain" style="text-align:left;" 
            Width="80%"></asp:TextBox>
    </td>
    <td style=" width:3%; height: 27px;"></td>
	<td style="width: 15%; height: 27px;" align="left">
	    <asp:Label ID="lblQuotDate0" runat="server" Font-Size="9pt">Return Date</asp:Label>
        </td>
	<td style="width: 30%; height: 27px;" align="left">
        <asp:TextBox ID="txtReturnDate" runat="server" AutoPostBack="False" 
            CssClass="tbc" Font-Size="8pt" SkinId="tbPlain" style="text-align:center;" 
            Width="50%"></asp:TextBox>
        <ajaxToolkit:CalendarExtender ID="txtReturnDate_CalendarExtender" 
            runat="server" Format="dd/MM/yyyy" TargetControlID="txtReturnDate" />
        </td>    
    </tr>

    <tr>
    <td style="width: 15%; " align="left" valign="top">
        <asp:Label ID="LblSuppNo1" runat="server" Font-Size="9pt">Remarks/Particulars</asp:Label>
        </td>
    <td align="left" colspan="4">
    <asp:TextBox SkinId="tbGray" ID="txtRemarks" runat="server" Width="99%"  
            AutoPostBack="False"  Font-Size="8pt" Height="51px" TextMode="MultiLine"></asp:TextBox>
        <ajaxToolkit:AutoCompleteExtender ID="txtRemarks_AutoCompleteExtender" 
            runat="server" CompletionInterval="20" CompletionSetCount="12" 
            EnableCaching="true" MinimumPrefixLength="1" ServiceMethod="GetShowRemarks" 
            ServicePath="AutoComplete.asmx" TargetControlID="txtRemarks" />
    </td>    
    </tr>    

    </table>  
  
    </fieldset>
    </ContentTemplate></asp:UpdatePanel>
    <div id="SearchID" runat="server">
       <fieldset style="vertical-align: top; border: solid 1px #8BB381; text-align:left;"><legend style="color: maroon;"><b>
    Search Option </b> </legend>
                <table style="width: 100%">
                    <tr>
                        <td>
                            <asp:Label ID="Label37" runat="server" style="font-weight: 700" 
                                Text="Purchase Return No"></asp:Label>
                        </td>
                        <td align="left" style="font-weight: 700;" width="15%">
                            Search By Supplier</td>
                        <td align="center" colspan="4"  width="5%" style="font-weight: 700">
                            Search By Return Date</td>
                        <td style="width: 11%">
                            &nbsp;</td>
                        <td style="width: 15%">
                            &nbsp;</td>
                    </tr>
                    <tr>
                        <td style="width: 18%">
                            <asp:TextBox ID="txtPurReturn" runat="server" 
                                placeholder="Search By Return No." Width="90%"></asp:TextBox>
                            <%-- <autocompleteextender id="txtGrnNo_AutoCompleteExtender" runat="server" 
                                completioninterval="1000" completionsetcount="12" enablecaching="true" 
                                minimumprefixlength="1" servicemethod="GetGRN" servicepath="AutoComplete.asmx" 
                                targetcontrolid="txtPurReturn" />--%>
                            <ajaxToolkit:AutoCompleteExtender ID="AutoCompleteExtender2" runat="server" 
                                CompletionInterval="20" CompletionSetCount="12" EnableCaching="true" 
                                MinimumPrefixLength="1" ServiceMethod="GetPurchaseReturnNo" ServicePath="AutoComplete.asmx" 
                                TargetControlID="txtPurReturn" />
                        </td>
                        <td align="left" style="width: 35%" width="15%">
                            <asp:UpdatePanel runat="server" UpdateMode="Conditional" ID="UP2"><ContentTemplate>
                            <asp:TextBox ID="txtSupplierSearch" runat="server" AutoPostBack="True" 
                                ontextchanged="txtSupplier_TextChanged" placeholder="Search By Supplier.." 
                                Width="90%"></asp:TextBox>
                     <%--       <autocompleteextender id="AutoCompleteExtender1" runat="server" 
                                completioninterval="1000" completionsetcount="12" enablecaching="true" 
                                minimumprefixlength="1" servicemethod="GetSupplier" 
                                servicepath="AutoComplete.asmx" targetcontrolid="txtSupplierSearch" />--%>

                                 <ajaxToolkit:AutoCompleteExtender ID="AutoCompleteExtender3" 
            runat="server" CompletionInterval="20" CompletionSetCount="12" 
            EnableCaching="true" MinimumPrefixLength="1" ServiceMethod="GetSupplier" 
            ServicePath="AutoComplete.asmx" TargetControlID="txtSupplierSearch" />
                            <asp:Label ID="lblSupplier" runat="server" Visible="False"></asp:Label>
                            </ContentTemplate></asp:UpdatePanel>
                        </td>
                        <td align="left" style="width: 3%; ">
                            &nbsp;</td>
                        <td align="left" style="width: 10%" width="35%">
                            <asp:TextBox ID="txtFromDate" runat="server" Placeholder="Date From" 
                                Width="90%"></asp:TextBox>
                            <ajaxToolkit:CalendarExtender ID="Calender" runat="server" Format="dd/MM/yyyy" 
                                TargetControlID="txtFromDate">
                            </ajaxToolkit:CalendarExtender>
                        </td>
                        <td style="width: 2%" width="10%">
                            &nbsp;</td>
                        <td align="left" style="width: 10%">
                            <asp:TextBox ID="txtToDate" runat="server" Placeholder="Date To" Width="90%"></asp:TextBox>
                            <ajaxToolkit:CalendarExtender ID="CalendarExtender1" runat="server" 
                                Format="dd/MM/yyyy" TargetControlID="txtToDate">
                            </ajaxToolkit:CalendarExtender>
                        </td>
                        <td style="width: 10%" align="center">
                                            <asp:LinkButton ID="lbSearch" runat="server" BorderStyle="Solid" 
                                                Font-Bold="True" Font-Size="12pt" 
                                onclick="BtnSearch_Click" Width="90%">Search</asp:LinkButton>
                        </td>
                        <td align="center" style="width: 10%">
                                            <asp:LinkButton ID="lbClear" runat="server" BorderStyle="Solid" 
                                                Font-Bold="True" Font-Size="12pt" onclick="Refresh_Click" 
                                Width="90%">Clear</asp:LinkButton>
                        </td>
                    </tr>
                </table>
           </fieldset>
     </div>          
    
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
  <asp:BoundField  HeaderText="Return No" DataField="Return_No" ItemStyle-Width="80px" 
          ItemStyle-HorizontalAlign="Center">    
<ItemStyle HorizontalAlign="Center" Width="80px"></ItemStyle>
      </asp:BoundField>
  <asp:BoundField  HeaderText="Return Date" DataField="ReturnDate" 
          DataFormatString="{0:dd/MM/yyyy}" ItemStyle-Width="80px" 
          ItemStyle-HorizontalAlign="Center">
          <ItemStyle HorizontalAlign="Center" Width="80px"></ItemStyle>
      </asp:BoundField>
      <asp:BoundField DataField="GRN" 
          HeaderText="GRN" >
          <ItemStyle HorizontalAlign="Center" Width="80px"></ItemStyle>
      </asp:BoundField>
      <asp:BoundField DataField="ContactName" 
          HeaderText="Supplier" >
          <ItemStyle HorizontalAlign="Left" Width="120px"></ItemStyle>
      </asp:BoundField>
      <asp:BoundField DataField="TotalAmount" 
          HeaderText="Total Amount" >
          <ItemStyle HorizontalAlign="Right" Width="90px"></ItemStyle>
      </asp:BoundField>
  <asp:BoundField  HeaderText="Remark's" DataField="Remarks" 
          ItemStyle-Height="20" ItemStyle-Width="150px" 
          ItemStyle-HorizontalAlign="Left">
<ItemStyle HorizontalAlign="Left" Height="20px" Width="150px"></ItemStyle>
      </asp:BoundField>
      <asp:BoundField  HeaderText="ID" DataField="ID"  ItemStyle-Width="80px" 
          ItemStyle-HorizontalAlign="Left" > 
<ItemStyle HorizontalAlign="Left" Width="10px"></ItemStyle>
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
             <td colspan="2" align="center">
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
                         <asp:TemplateField HeaderText="Item Rate">
                             <ItemTemplate>
                                 <asp:TextBox ID="txtItemRate" runat="server"  placeHolder="0.00"
                                     AutoPostBack="False" CssClass="tbr" Font-Size="8pt" onkeypress="return isNumber(event)"
           SkinId="tbPlain" Text='<%#Eval("item_rate")%>' Width="93%" onFocus="this.select()" Enabled="False"></asp:TextBox></ItemTemplate><ItemStyle HorizontalAlign="Right" Width="10%" /></asp:TemplateField>
                         <asp:TemplateField HeaderText="Quantity">
                             <ItemTemplate>
                                 <asp:TextBox ID="txtQnty" runat="server"  placeHolder="0"
                                     AutoPostBack="true" CssClass="tbc" 
           Font-Size="8pt" OnTextChanged="txtQnty_TextChanged" SkinId="tbPlain" onkeypress="return isNumber(event)"
           Text='<%#Eval("qnty")%>' Width="93%" onFocus="this.select()"></asp:TextBox></ItemTemplate>
                             <ItemStyle HorizontalAlign="Center" Width="10%" /></asp:TemplateField>
                         <asp:TemplateField HeaderText="Total Amount">
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
                             <ItemStyle HorizontalAlign="Right" Width="10%" />
                         </asp:TemplateField>
                         <asp:BoundField DataField="PvQty"  HeaderText="Closing Qty.">
                             <ItemStyle HorizontalAlign="Right" Width="15%" />
                         </asp:BoundField>
                        <%-- ReturnQty--%>
                         <asp:BoundField DataField="ReturnQty"  HeaderText="ReturnQty">
                             <ItemStyle HorizontalAlign="Right" Width="15%" />
                         </asp:BoundField>
                         
                           <asp:BoundField DataField="PurchaseQty"  HeaderText="Purchase Qty.">
                             <ItemStyle HorizontalAlign="Right" Width="15%" />
                         </asp:BoundField>
                         <asp:BoundField DataField="ExpireDate" HeaderText="ExpireDate" />
                     </Columns>
                     <HeaderStyle Font-Bold="True" Font-Size="9pt" ForeColor="White" />
                     <PagerStyle CssClass="pgr" ForeColor="White" 
                         HorizontalAlign="Center" />
                     <RowStyle BackColor="White" /></asp:GridView></td></tr>
                 <tr>
                     <td style="width:50%;">
                     <fieldset style=" border:solid 1px #8BB381;text-align:left;"><legend>Payment Methord</legend>
           <asp:UpdatePanel ID="UPPaymentMtd" runat="server" UpdateMode="Conditional">
           <ContentTemplate>               
                         <table style="width: 100%">
                             <tr>
                                 <td style="width:20%; height: 26px;" align="left">
                                     <asp:Label ID="Label26" runat="server" Font-Size="9pt">Payment 
                                     Methord</asp:Label>
                                 </td>
                                 <td style="width:30%; height: 26px;">
                                     <asp:DropDownList ID="ddlPaymentMethord" runat="server" AutoPostBack="True" 
                                         Font-Size="8pt" Height="24px" 
                                         onselectedindexchanged="ddlPaymentMethord_SelectedIndexChanged" 
                                         SkinID="ddlPlain" TabIndex="19" Width="100%">
                                         <asp:ListItem Text="Cash" Value="C"></asp:ListItem>
                                         <asp:ListItem Value="Q">Cheque</asp:ListItem>
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
                                     &nbsp;</td>
                                 <td style="width:30%; height: 26px;">
                                     &nbsp;</td>
                                 <td style="width:3%; height: 26px;">
                                     </td>
                                 <td style="width:20%; height: 26px;">
                                     <asp:Label ID="lblChequeStatus" runat="server" Font-Size="9pt">Check Status</asp:Label>
                                     </td>
                                 <td style="width:30%; height: 26px;">
                                     <asp:DropDownList ID="ddlChequeStatus" runat="server" Font-Size="8pt" 
                                         Height="24px" onselectedindexchanged="ddlPaymentMethord_SelectedIndexChanged" 
                                         SkinID="ddlPlain" TabIndex="24" Width="100%">
                                         <asp:ListItem Value="A">Approved</asp:ListItem>
                                         <asp:ListItem Value="P">Pending</asp:ListItem>
                                     </asp:DropDownList>
                                     </td>
                             </tr>
                         </table></ContentTemplate></asp:UpdatePanel></fieldset>
                     </td>
                     <td align="right" style="width:35%;">
                      <fieldset style=" border:solid 1px #8BB381;text-align:left;"><legend>Payment Information</legend>
                         <table style="width: 100%">
                             <tr>
                                 <td style="width:50%; height: 31px;" align="right">
                                     <asp:Label ID="Label7" runat="server" Text="Grand Total"></asp:Label>
                                 </td>
                                 <td style="width:3%; height: 31px;"></td>
                                 <td style="width:30%; height: 31px;">
                                     <asp:TextBox ID="txtTotal" runat="server" style="text-align: right;" onkeypress="return isNumber(event)"
                                         Width="98%"></asp:TextBox>
                                 </td>
                             </tr>                            
                             <tr>
                                 <td style="width: 50%; height: 25px;" align="right">
                                     <asp:Label ID="Label8" runat="server" Text="Total Payment"></asp:Label>
                                 </td>
                                 <td style="width: 3%; height: 25px;">
                                     </td>
                                 <td style="width: 30%; height: 25px;">
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
                                     <asp:TextBox ID="txtDue" runat="server" style="text-align: right;" onkeypress="return isNumber(event)" Width="98%"></asp:TextBox>
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
<tr>
<td style="width:1%;" align="center">&nbsp;</td>
<td style="width:98%;" align="center"> 	
    &nbsp;</td>
 <td style="width:1%;" align="center">&nbsp;</td>
</tr>
<tr>
<td style="width:1%;" align="center">&nbsp;</td>
<td style="width:98%;" align="center"> 	
    &nbsp;</td>
 <td style="width:1%;" align="center">&nbsp;</td>
</tr>
<tr>
<td style="width:1%;" align="center">&nbsp;</td>
<td style="width:98%;" align="center"> 	
    &nbsp;</td>
 <td style="width:1%;" align="center">&nbsp;</td>
</tr>
<tr>
<td style="width:1%;" align="center">&nbsp;</td>
<td style="width:98%;" align="center"> 	
    &nbsp;</td>
 <td style="width:1%;" align="center">&nbsp;</td>
</tr>
</table>
</div>
</asp:Content>


