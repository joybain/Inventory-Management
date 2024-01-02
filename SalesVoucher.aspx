<%@ Page Title="Sales Bill/Invoice.-RD." Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="SalesVoucher.aspx.cs" Inherits="SalesVoucher" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    
<script language="javascript" type="text/javascript" >
    $(document).ready(function () {

//        alert("Test");
//        $('#MainContent_txtItemsCode').focus();

    });

    $('#MainContent_txtItemsCode').keypress(function (event) {

        alert("Test");

        var keycode = (event.keyCode ? event.keyCode : event.which);
        if (keycode == '13') {
            alert('You pressed a enter key in textbox');
        }
    });

</script>

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
<script language="javascript" type="text/javascript" >
    function OpenWindow(Url)
    {
        var testwindow = window.open(Url, '', 'width=600px,height=400px,top=230,left=300,scrollbars=1');
    }
    function onListPopulated()
    {
        var completionList1 = $find("AutoCompleteEx1").get_completionList();
        completionList1.style.width = 'auto';
        var completionList2 = $find("AutoCompleteEx2").get_completionList();
        completionList2.style.width = 'auto';
        var completionList3 = $find("AutoCompleteEx3").get_completionList();
        completionList3.style.width = 'auto';
    }

    function setValue(myVal) {
        $("#MainContent_txtItemsCode").focus();
        document.getElementById('<%=txtItemsCode.ClientID%>').value = myVal;
    } 

    function pad(str, max) {
        return str.length < max ? pad("0" + str, max) : str;
    }

    function remLink() {
        if (window.testwindow && window.testwindow.open && !window.testwindow.closed)
            window.testwindow.opener = null;
    }
    function IsEmpty(aTextField) 
    {
        if ((aTextField.value.length == 0) || (aTextField.value == null)) {
            return true;
        }
        else {
            return false;
        }
    }

    function LoadModalDiv() {

        var bcgDiv = document.getElementById("divBackground");
        bcgDiv.style.display = "block";

    }
    function HideModalDiv() {

        var bcgDiv = document.getElementById("divBackground");
        bcgDiv.style.display = "none";

    }



    function TotSumation() {


        var txtSubTotal = document.getElementById("<%=txtSubTotal.ClientID %>");
        var txtVat = document.getElementById("<%=txtVat.ClientID %>");
        var txtDiscount = document.getElementById("<%=txtDiscount.ClientID %>");
        var txtPayment = document.getElementById("<%=txtPayment.ClientID %>");
        var txtDue = document.getElementById("<%=txtDue.ClientID %>"); 

        var totv = parseFloat(txtSubTotal.value) + ((parseFloat(txtSubTotal.value) * parseFloat(txtVat.value)) / 100);
        var todD = parseFloat(txtDiscount.value);
//            txtPayment.value = todD.toFixed(2);
            
    }

    function Due() {

        var txtSubTotal = document.getElementById("<%=txtSubTotal.ClientID %>");
        var txtVat = document.getElementById("<%=txtVat.ClientID %>");
        var txtDiscount = document.getElementById("<%=txtDiscount.ClientID %>");
        var txtPayment = document.getElementById("<%=txtPayment.ClientID %>");
        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ale", "alert('This Item already selected.!!');", true);
        var txtDue = document.getElementById("<%=txtDue.ClientID %>");

        var totv = parseFloat(txtSubTotal.value) + ((parseFloat(txtSubTotal.value) * parseFloat(txtVat.value)) / 100);
        var todD = parseFloat(totv) - ((parseFloat(totv) * parseFloat(txtDiscount.value)) / 100);          
        if (parseFloat(todD) < parseFloat(txtPayment.value)) {
            alert("Payment Amount Over This Total Charge.....!!!!!!");
            txtTotPayment.value = "0";
            txtTotPayment.focus();
        }
        else {
            var tt = parseFloat(todD) - parseFloat(txtPayment.value)
            txtDue.value = tt.toFixed(2);
        }
    }
       
</script>  
<script language="javascript" type="text/javascript">

    function LoadModalDiv() {

        var bcgDiv = document.getElementById("divBackground");
        bcgDiv.style.display = "block";

    }
    function HideModalDiv() {

        var bcgDiv = document.getElementById("divBackground");
        bcgDiv.style.display = "none";

    }
    
</script>
<div id="frmMainDiv" style="background-color:White; width:100%;">  
<table  id="pageFooterWrapper">
  <tr>  
        <td align="center">
        <asp:Button ID="btnDelete" runat="server" ToolTip="Delete"
            
                
                
                
                onclientclick="javascript:return window.confirm('are u really want to delete these data')" Text="Delete" 
        Height="35px" Width="110px" BorderStyle="Outset" onclick="btnDelete_Click"  />
        </td>       
        <td align="center" >
            &nbsp;</td>       
        <td align="center" >
        <asp:Button ID="btnSave" runat="server" ToolTip="Save Purchase Record" Text="Save" 
        Height="34px" Width="110px" BorderStyle="Outset" onclick="btnSave_Click"  />
        </td>
        <td align="center" >
        <asp:Button ID="btnNew" runat="server" ToolTip="New"  Text="New" 
        Height="35px" Width="110px" BorderStyle="Outset" onclick="btnNew_Click"  /> 
        </td>
        <td align="center" >
        <asp:Button ID="btnClear" runat="server"  ToolTip="Clear" Text="Clear" 
        Height="35px" Width="110px" BorderStyle="Outset" onclick="btnClear_Click"  />
        </td>
      <td align="center" >
          <asp:Button ID="btnFind" runat="server" ToolTip="Find" Text="Find" 
                      Height="35px" Width="110px" BorderStyle="Outset" 
              onclick="btnFind_Click1" />
      </td> 
        <td align="center" >
        <asp:Button ID="btnPrint" runat="server" ToolTip="Print PO" Text="Print Invoice" 
        Height="35px" Width="110px" BorderStyle="Outset" onclick="btnPrint_Click"   />
            <%--    <asp:BoundField DataField="Code" HeaderText="Code">
                                            <ItemStyle HorizontalAlign="Center" Width="10%" />
                                     </asp:BoundField>--%>
        </td>            
        
        <td align="center" >
            <asp:Button ID="btnChallan" runat="server" 
                Text="Print Challan" Height="35px" onclick="btnChallan_Click" 
                Visible="False" />
        </td>   
        <%--  <asp:TemplateField HeaderText="Barcode">
                                         
                                         <ItemTemplate>
                                             <asp:TextBox ID="txtBarcode" runat="server" AutoPostBack="True"  CssClass="txtVisibleAlign" Height="18px" autocomplete="off"
                                                          onfocus="this.select();" 
                                                          style="text-align:Left;" Text='<%# Eval("Barcode") %>'></asp:TextBox>
                                         </ItemTemplate>
                                         

                                     </asp:TemplateField>--%>
                
         <td align="center" >
            <asp:Button ID="btnPosPrint" runat="server" 
                Text="Pos Print" Height="35px" onclick="btnPosPrint_Click" 
                 Visible="False" />
        </td>   

   </tr>
   </table>

<table style="width:100%;">
<tr>
<td style="width:1%;">&nbsp;</td>
<td align="center" style="width: 98%" colspan="2">
<table width="100%">
<tr>
    <td style="width: 2%">&nbsp;</td>
    <td style="width: 96%">
                <table style="width: 100%">
                    <tr>
                        <td>
                <asp:Label ID="lblInvNo" runat="server" Visible="False" style="font-weight: 700"></asp:Label>
                                                     </td>
                        <td>
                                                     <asp:HiddenField ID="hfCustomerCoa" 
                    runat="server" />
                                                     </td>
                        <td>
                                                     <asp:HiddenField ID="hfCustomerID" 
                    runat="server" />
                        </td>
                        <td>
                                                     <asp:DropDownList ID="ddlDelevery" 
                    runat="server" Font-Size="8pt" Height="26px" 
                                                         onChange="changetextbox();" 
                    SkinID="ddlPlain" TabIndex="10" Visible="False">
                                                         <asp:ListItem Value="Y">Yes</asp:ListItem>
                                                         <asp:ListItem Value="N">No</asp:ListItem>
                                                     </asp:DropDownList>
                                                     </td>
                        <td>
                                                              
                                                     <asp:TextBox ID="txtChequeAmount" runat="server" Enabled="False" 
                                                         Font-Size="8pt" SkinID="tbPlain" 
                                style="text-align:right;" TabIndex="17" 
                                                         Visible="False" Width="20px"></asp:TextBox>
                                                 </td>
                        <td>
                                                         <asp:TextBox ID="txtLocalCustomer" runat="server" Font-Size="8pt" 
                                                             SkinID="tbPlain" TabIndex="15" Width="10px" 
                                Visible="False"></asp:TextBox>
                                                     </td>
                        <td>
                                                         <asp:TextBox ID="txtLocalCusPhone" runat="server" Font-Size="8pt" 
                                                             SkinID="tbPlain" TabIndex="15" Width="10px" 
                                Visible="False"></asp:TextBox>
                                                     </td>
                        <td>
                                                         <asp:TextBox ID="txtLocalCusAddress" 
                                runat="server" TabIndex="12" Width="10px" Visible="False"></asp:TextBox>
                                                     </td>
                        <td>
                            &nbsp;</td>
                        <td>
                            &nbsp;</td>
                    </tr>
                </table>
               </td>
    <td style="width: 2%">&nbsp;</td>
</tr>
<tr>
    <td style="width: 2%">&nbsp;</td>
    <td style="width: 96%">
        <fieldset style="vertical-align: top; border: solid 1px #8BB381;text-align:left;"><legend style="color: maroon;"><b>Customer Info</b> </legend>
                <table style="width: 100%">
                    <tr>
                        <td style="width: 10%; height: 29px;" align="right">
                                                     <asp:Label ID="Label35" runat="server" 
                                Text="Customer Name" style="font-weight: 700"></asp:Label>
                                                     </td>
                        <td style="width: 1%; height: 29px;">
                            </td>
                        <td style="width: 20%; height: 29px;">
                            <asp:TextBox ID="txtCustomerName" CssClass="txtVisibleAlign" Height="18px" 
                                placeHolder="Search by customer.." runat="server" Width="100%" 
                                AutoPostBack="True" ontextchanged="txtCustomerName_TextChanged"></asp:TextBox>
                            <ajaxToolkit:AutoCompleteExtender ServicePath="AutoComplete.asmx" runat="server" 
                                                              ID="AutoCompleteExtender3" TargetControlID="txtCustomerName"
                                                            
                                                               ServiceMethod="GetBranchCustomername"
                                                              MinimumPrefixLength="1" CompletionInterval="20" EnableCaching="true" 
                                                              CompletionSetCount="12"/>
                            <asp:Label ID="lblPreviousDue" runat="server" Font-Bold="True" Font-Size="10pt" ForeColor="#FF3300"></asp:Label>
                        </td>
                        <td style="width: 5%; height: 29px;" align="center">
                            <asp:LinkButton ID="lnkbCustomerLink" runat="server" Font-Bold="True" 
                                onclick="lnkbCustomerLink_Click">........</asp:LinkButton>
                            </td>
                        <td style="width: 10%; height: 29px;" align="right">
                            <strong>Note</strong></td>
                        <td style="width: 1%; height: 29px;">
                            </td>
                        <td style="width: 15%; height: 29px;">
                            <asp:TextBox ID="txtNote" runat="server" Width="100%"></asp:TextBox>
                        </td>
                        <td style="width: 10%; height: 29px;" align="right">
                            Delivery Date</td>
                        <td style="width: 1%; height: 29px;">
                            </td>
                        <td style="width: 10%; height: 29px;">
                            <asp:TextBox ID="txtDeleveryDate" runat="server" placeholder="dd/MM/yyyy" autocomplete="off"
                                style="text-align:center;" TabIndex="11" Width="99%"></asp:TextBox>
                            <cc:CalendarExtender ID="Calendarextender2" runat="server" Format="dd/MM/yyyy" TargetControlID="txtDeleveryDate" />
                       </td>
                    </tr>
                    <tr>
                        <td style="width: 10%; height: 58px;" align="right" valign="top">
                                                     Remark&#39;s</td>
                        <td style="width: 1%; height: 58px;">
                            </td>
                        <td colspan="8" style="height: 58px" valign="middle">
                           
                                                     <asp:TextBox ID="txtRemarks" runat="server" 
                                TabIndex="12" TextMode="MultiLine" 
                                                         Width="100%">Items Sales </asp:TextBox>
                           
                        </td>
                    </tr>
                </table>
        </fieldset>
               </td>
    <td style="width: 2%">&nbsp;</td>
</tr>
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
                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                <asp:Label ID="Label112" runat="server" style="font-weight: 700; text-align: center;" 
                                    Text="Search Brunch Number"></asp:Label>
                            </td>
                            <td style="width: 5%"> 
                                &nbsp;</td>
                            <td style="width: 40%">
                                <asp:Label ID="Label40" runat="server" style="font-weight: 700" 
                                    Text="Search Items/Size/Price/Exp.Date ."></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 40%">
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                <asp:DropDownList ID="ddlBranchId" runat="server" Height="25px" Width="50%" 
                                    AutoPostBack="True" Enabled="False" 
                                    onselectedindexchanged="ddlBranchId_SelectedIndexChanged">
                                </asp:DropDownList>
                            </td>
                            <td  style="width: 5%">
                                
                            </td>
                            <td  style="width: 40%">
                                
                <asp:TextBox ID="txtItemsCode" runat="server" Width="98%" AutoPostBack="True" 
                    CssClass="txtVisibleAlign" Height="18px" placeHolder="Search By Items.."
                    onfocus="this.select();" TabIndex="1"
                    ontextchanged="txtCode_TextChanged" ></asp:TextBox>
                <ajaxToolkit:AutoCompleteExtender ServicePath="AutoComplete.asmx" runat="server" 
                                                  ID="AutoCompleteExtender2" TargetControlID="txtItemsCode"
                                                  ServiceMethod="GetItemsSearch" 
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
    <td style="width: 2%">
                <asp:TextBox ID="txtItemsID" runat="server" Width="15px" ForeColor="Red"  
                    BorderWidth="0px" style="border-color:none;border:0px; background:transparent;"></asp:TextBox>
            </td>
</tr>
</table>   
</td>
<td style="width: 1%">&nbsp;</td>
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
                                                          onfocus="this.select();" ontextchanged="txtQty_TextChanged" onkeypress="return isNumber(event)" 
                                                          style="text-align:center;" Text='<%# Eval("Qty", "{0:0}") %>' Width="90%"></asp:TextBox>
                                         </ItemTemplate>
                                         <ItemStyle Height="20px" Width="6%" />
                                     </asp:TemplateField>
                                     
                                     <asp:TemplateField HeaderText="Sale Price">
                                         <ItemTemplate>
                                             <asp:TextBox ID="txtSalesPrice" runat="server" AutoPostBack="True"  
                                                 CssClass="txtVisibleAlign" Height="18px"
                                                          onfocus="this.select();" style="text-align:center;" 
                                                          Text='<%# Eval("SPrice") %>' Width="92%" onkeypress="return isNumber(event)" 
                                                          ontextchanged="txtSalesPrice_TextChanged" Enabled="False"></asp:TextBox>
                                         </ItemTemplate>
                                         <ItemStyle Height="20px" Width="8%" />
                                     </asp:TemplateField>

                                     <asp:BoundField DataField="Tax" HeaderText="Vat(%)">
                                             <ItemStyle Height="20px" HorizontalAlign="Center" Width="8%" />
                                     </asp:BoundField>                      

                                     <asp:TemplateField HeaderText="Dis(%)"> 
                                         <ItemTemplate>
                                             <asp:TextBox ID="txtDiscount" runat="server" AutoPostBack="True" 
                                                 ontextchanged="txtDiscount_TextChanged"  CssClass="txtVisibleAlign" Height="18px"
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
                                            <asp:Label ID="Label111" runat="server" Visible="False" Text=" Payment Type"></asp:Label>
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
                                                Width="100%" onselectedindexchanged="ddlBankNameFrom_SelectedIndexChanged">
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
                                                Width="100%" Visible="False" Enabled="True">
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
                                                style="text-align: right" Width="96%"  AutoPostBack="True" 
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
                                <asp:GridView ID="dgPaymentInfo" runat="server" Visible="True" AutoGenerateColumns="False" 
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
                                                             onfocus="this.select();" ontextchanged="txtVat_TextChanged" onkeypress="return isNumber(event)" 
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
                                onfocus="this.select();" ontextchanged="TotalDiscount_TextChange" onkeypress="return isNumber(event)" 
                                style="text-align:right;" TabIndex="6" Width="100%"></asp:TextBox>
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
<tr>
<td style="width:1%;">&nbsp;</td>
<td align="left" colspan="2">
    
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
  <asp:BoundField  HeaderText="Invoice No"  DataField="InvoiceNo" ItemStyle-Width="100px" 
          ItemStyle-HorizontalAlign="Center">    
<ItemStyle HorizontalAlign="Center" Width="100px"></ItemStyle>
      </asp:BoundField>
  <asp:BoundField  HeaderText="Invoice Date"  DataFormatString="{0:dd/MM/yyyy}" ItemStyle-Width="100px" 
          ItemStyle-HorizontalAlign="Center" DataField="OrderDate">
          <ItemStyle HorizontalAlign="Center" Width="100px"></ItemStyle>
      </asp:BoundField>
      <asp:BoundField  HeaderText="Customer Name" DataField="CustomerName" >
          <ItemStyle HorizontalAlign="Left" Width="200px"></ItemStyle>
      </asp:BoundField>
      <asp:BoundField DataField="GrandTotal" HeaderText="Grand Total">
      <ItemStyle Height="20px" HorizontalAlign="Right" Width="80px" />
      </asp:BoundField>
  <asp:BoundField  HeaderText="Received Amount" 
          ItemStyle-Height="20" ItemStyle-Width="80px" 
          ItemStyle-HorizontalAlign="Right" DataField="CashReceived">
<ItemStyle HorizontalAlign="Right" Height="20px" Width="80px"></ItemStyle>
      </asp:BoundField>
      <asp:BoundField  HeaderText="ID" DataField="ID"  ItemStyle-Width="80px" 
          ItemStyle-HorizontalAlign="Left" > 
<ItemStyle HorizontalAlign="Left" Width="80px"></ItemStyle>
      </asp:BoundField>
      <asp:BoundField  HeaderText="BranchId" 
          ItemStyle-Height="20" ItemStyle-Width="80px" 
          ItemStyle-HorizontalAlign="Right" DataField="BranchId">
      </asp:BoundField>
      <asp:BoundField DataField="BranchName" HeaderText="Branch Name">
      <ItemStyle Height="20px" HorizontalAlign="Center" Width="80px" />
      </asp:BoundField>
  </Columns>
                        <RowStyle BackColor="White" />
                        <SelectedRowStyle BackColor="" Font-Bold="True" />
                        <PagerStyle BackColor="LightGray" ForeColor="Black" HorizontalAlign="Center" />
                        <AlternatingRowStyle BackColor="" />
                        
</asp:GridView>

</td>
<td>&nbsp;</td>
</tr>
<tr>
<td style="width:1%;">&nbsp;</td>
<td style="width:41%;" align="center"> 
                                                     <asp:HiddenField ID="HidenBranchId" 
                    runat="server" />
                                                     </td>
<td style="width:62%;">&nbsp;</td>
<td>&nbsp;</td>
</tr>
<tr>
<td style="width:1%;">&nbsp;</td>
<td align="center" colspan="2">


</td>
<td>&nbsp;</td>
</tr>
<tr>
<td style="width:1%;">&nbsp;</td>
<td style="width:41%;" align="center"> 
    &nbsp;</td>
<td style="width:62%;">&nbsp;</td>
<td>&nbsp;</td>
</tr>
</table>
</div>
<div id="divBackground" style="position: fixed; z-index: 999; height: 100%; width: 100%; top: 0; left: 0; background-color: Black; filter: alpha(opacity=60); opacity: 0.6; -moz-opacity: 0.8; -webkit-opacity: 0.8; display: none">
</div>
</asp:Content>

