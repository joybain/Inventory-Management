<%@ Page Title="Bulk Voucher Authorization" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="frmAccBulkAuth.aspx.cs" Inherits="frmAccBulkAuth" Theme="Themes" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
<script src='<%= ResolveUrl("~/Scripts/valideDate.js") %>' type="text/javascript"></script>
<script type="text/javascript">
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

<table  id="pageFooterWrapper">
  <tr>  
   <td align="center" >
               &nbsp;</td>
       <td align="center">
           <asp:Button ID="btnFind" runat="server" ToolTip="Find" onclick="btnFind_Click"  
               Text="Find" Width="100px" Height="35px"  /> 
       </td>
           <td align="center">
           <asp:Button ID="btnAuth" runat="server"  ToolTip="Clear" onclick="btnAuth_Click" 
                   Text="Authorize" Width="100px" Height="35px"  />
       </td>
        <td align="center" >
               &nbsp;</td>
           <td align="center">
           <asp:Button ID="btnClear" runat="server" ToolTip="Clear Form" 
                   onclick="btnClear_Click"  Text="Clear" Width="100px" Height="35px"  /> </td>
           <td align="center" >
               &nbsp;</td>
                
   </tr>
</table>

<table style="width:100%;">
<tr>
<td style="width:1%; height: 6px;"></td>
<td style="width:98%; height: 6px;" align="center">


<asp:TextBox SkinID="tbGray" CssClass="tbl" ID="txtPayee" runat="server" 
        Width="10px" Visible="False"></asp:TextBox>


</td>
<td style="width:1%; height: 6px;"></td>
</tr>
<tr>
<td style="width:1%; height: 6px;"></td>
<td style="width:98%; height: 6px;" align="center">


    </td>
<td style="width:1%; height: 6px;"></td>
</tr>
<tr>
<td style="width:1%;"></td>
<td style="width:98%;" align="center">

<fieldset style=" text-align:left; vertical-align: top; border: solid 1px #8BB381;"><legend style="color: maroon;"> 
<b> Search Voucher </b></legend>
<table style="width:99%; padding-right:10px;" >
<tr>
<td style="width:20%; font-size:8pt;" align="left">From Date</td>
<td style="width:25%; font-size:8pt;" align="left">
<asp:TextBox SkinID="tbGray" CssClass="tbc" ID="txtFromDt" runat="server" Width="100%" MaxLength="11" PlaceHolder="dd/MM/yyyy"></asp:TextBox>
<ajaxtoolkit:calendarextender runat="server" ID="Calendarextender2" TargetControlID="txtFromDt" Format="dd/MM/yyyy"/>
</td>
<td style="width:10%;"></td>
<td style="width:20%; font-size:8pt;" align="left">To Date</td>
<td style="width:25%; font-size:8pt;" align="left">
<asp:TextBox SkinID="tbGray" CssClass="tbc" ID="txtToDt" runat="server" Width="100%" MaxLength="11" PlaceHolder="dd/MM/yyyy"></asp:TextBox>
<ajaxtoolkit:calendarextender runat="server" ID="Calendarextender1" TargetControlID="txtToDt" Format="dd/MM/yyyy"/>
</td>
</tr>
<tr>
<td style="width:20%; font-size:8pt;" align="left">Particulars</td>
<td style="width:25%; font-size:8pt;" align="left" colspan="4">
<asp:TextBox SkinID="tbGray" CssClass="tbl" ID="txtParticulars" runat="server" Width="100%"></asp:TextBox>
</td>
</tr>
<tr>
<td style="width:20%; font-size:8pt;" align="left">Search User</td>
<td style="width:25%; font-size:8pt;" align="left">
    <asp:DropDownList ID="ddlUser" runat="server" Width="101%" Height="26px" 
        SkinID="ddlPlain" AutoPostBack="True" 
        onselectedindexchanged="ddlUser_SelectedIndexChanged">
    </asp:DropDownList>
</td>
</tr>
</table>
</fieldset>
<fieldset style=" text-align:left; vertical-align: top; border: solid 1px #8BB381;"><legend style="color: maroon;"> 
<b> Un-Authorization Voucher History </b></legend>
<table style="width:100%; padding-right:10px;">
<tr>
<td colspan="5">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional"><ContentTemplate>
        <asp:Label ID="lblMessage" runat="server" Font-Bold="True" ForeColor="#CC3300"></asp:Label>
        <asp:GridView  RowStyle-Height="25px" CssClass="mGrid" PagerStyle-CssClass="pgr"  
            AlternatingRowStyle-CssClass="alt"  ID="dgVoucher" runat="server" 
            AutoGenerateColumns="False" Width="100%" BackColor="White" 
            BorderWidth="1px" BorderStyle="Solid"
        CellPadding="2" BorderColor="LightGray" Font-Size="8pt" 
        AllowSorting="True" PageSize="100" 
         onpageindexchanging="dgVoucher_PageIndexChanging" 
            onrowcommand="dgVoucher_RowCommand" >
  <HeaderStyle Font-Size="9" Font-Names="Arial" Font-Bold="True" BackColor="Silver" HorizontalAlign="center"  ForeColor="Black" />

  <Columns>
  <asp:TemplateField ItemStyle-Width="40px" ItemStyle-HorizontalAlign="Center">
  <HeaderTemplate>
  <asp:CheckBox ID="chkSelect" runat="server" AutoPostBack="true" OnCheckedChanged="chkSelect_CheckedChanged" />
  </HeaderTemplate>
  <ItemTemplate>
  <asp:CheckBox ID="chkSelect" runat="server" />
  </ItemTemplate>
      <ItemStyle HorizontalAlign="Center" Width="40px" />
  </asp:TemplateField>
  <asp:TemplateField HeaderText="View" ItemStyle-Width="25px">
        <ItemTemplate>
            <asp:LinkButton ID="lblSelect" Visible="True"  runat="server"  OnClientClick="LoadModalDiv();"
                CommandName="View" Text="( View )" Font-Bold="True" Font-Size="10pt" 
                Font-Underline="False"></asp:LinkButton>
        </ItemTemplate>
        <ItemStyle HorizontalAlign="Center" Width="75px" />
      </asp:TemplateField>
  <asp:BoundField  HeaderText="Voucher No" DataField="vch_sys_no" 
          ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center"> 
      <ItemStyle HorizontalAlign="Center" Width="100px" />
      </asp:BoundField>
      <asp:BoundField  HeaderText="Manual V. No" DataField="serial_no" 
          ItemStyle-Width="70px" ItemStyle-HorizontalAlign="Center"> 
      <ItemStyle HorizontalAlign="Center" Width="70px" />
      </asp:BoundField>  
  <asp:BoundField  HeaderText="Voucher Date" DataField="value_date" 
          DataFormatString="{0:dd/MM/yyyy}" ItemStyle-Width="100px" 
          ItemStyle-HorizontalAlign="Center">
      <ItemStyle HorizontalAlign="Center" Width="100px" />
      </asp:BoundField>
  <asp:BoundField  HeaderText="Particulars" DataField="particulars" ItemStyle-Height="20" ItemStyle-Width="300px" ItemStyle-HorizontalAlign="Left">
      <ItemStyle Height="20px" HorizontalAlign="Left" Width="300px" />
      </asp:BoundField>
  <asp:BoundField  HeaderText="Amount" DataField="control_amt"  
          ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Right" 
          DataFormatString="{0:N3}">
  
      <ItemStyle HorizontalAlign="Right" Width="100px" />
      </asp:BoundField>
  
  </Columns>
                        <RowStyle BackColor="White" />
                        <EditRowStyle BackColor="" />
                        <PagerStyle HorizontalAlign="Center" />
                        <%--<HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />--%>
                        <AlternatingRowStyle BackColor="#F5F5F5" />
</asp:GridView> 
 
      <asp:Panel ID="pnlVoucher" runat="server"  CssClass="modalPopup" 
                            Style=" display:none; background-color: White; width:800px; height:auto; ">
                            <fieldset style=" text-align:left; vertical-align: top; border: solid 1px #8BB381;line-height:1.5em;"><legend style="color: maroon;"><b>View Voucher Information</b></legend>
                            <asp:Label runat="server" ID="lblPartuculars"></asp:Label>
                                <asp:GridView ID="dgVoucherDtl" runat="server" AllowSorting="True" 
                                    AutoGenerateColumns="False" BackColor="White" BorderColor="LightGray" 
                                    BorderStyle="Solid" BorderWidth="1px" CellPadding="2" CssClass="Grid" 
                                    Font-Size="8pt" Width="100%">
                                    <AlternatingRowStyle BackColor="WhiteSmoke" CssClass="alt" />
                                    <Columns>
                                        <asp:TemplateField HeaderText="Line#">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtLineNo" runat="server" CssClass="txtVisibleAlignNotBorder"  MaxLength="4" 
                                                    SkinID="tbGray" Text='<%#Eval("line_no") %>' Width="93%"></asp:TextBox>
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" Width="70px" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="COA Code">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtGlCoaCode" runat="server" AutoPostBack="true" 
                                                    CssClass="txtVisibleAlignNotBorder" Font-Size="8" MaxLength="13" 
                                                    Text='<%#Eval("gl_coa_code") %>' Width="93%"></asp:TextBox>
                                            </ItemTemplate>
                                            <ItemStyle Font-Size="8pt" Height="18px" Width="90px" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="COA Description">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtCoaDesc" runat="server" autocomplete="off" CssClass="txtVisibleAlignNotBorder"
                                                    AutoPostBack="true" Font-Size="8" MaxLength="150" 
                                                    Text='<%#Eval("particulars") %>' Width="98%"></asp:TextBox>
                                                <ajaxToolkit:AutoCompleteExtender ID="autoComplete" runat="server" 
                                                    CompletionInterval="20" CompletionSetCount="12" EnableCaching="true" 
                                                    MinimumPrefixLength="1" ServiceMethod="GetCompletionList" 
                                                    ServicePath="AutoComplete.asmx" TargetControlID="txtCoaDesc" />
                                            </ItemTemplate>
                                            <ItemStyle Font-Size="8pt" HorizontalAlign="Left" Width="300px" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Credit">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtCredit" runat="server" AutoPostBack="true" CssClass="txtVisibleAlignNotBorder" 
                                                    MaxLength="25" onFocus="this.select()" onkeypress="return isNumber(event)" 
                                                    Text='<%#Eval("amount_cr") %>' Width="95%" style="text-align:right;"></asp:TextBox>
                                            </ItemTemplate>
                                            <FooterStyle Font-Bold="True" HorizontalAlign="Right" />
                                            <ItemStyle Font-Size="8pt" HorizontalAlign="Right" Width="100px" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Debit">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtDebit" runat="server" AutoPostBack="true" CssClass="txtVisibleAlignNotBorder" 
                                                    MaxLength="25" onFocus="this.select()" onkeypress="return isNumber(event)" 
                                                    Text='<%#Eval("amount_dr") %>' Width="95%" style="text-align:right;" ></asp:TextBox>
                                            </ItemTemplate>
                                            <FooterStyle Font-Bold="True" HorizontalAlign="Right" />
                                            <HeaderStyle Wrap="False" />
                                            <ItemStyle Font-Size="8pt" HorizontalAlign="Right" Width="100px" />
                                        </asp:TemplateField>
                                    </Columns>
                                    <HeaderStyle Font-Bold="True" ForeColor="Black" HorizontalAlign="Center" />
                                    <PagerStyle CssClass="pgr" HorizontalAlign="Center" />
                                    <RowStyle BackColor="White" Height="25px" />
                                </asp:GridView>
                                <br/>
                                <asp:button id="btnCancel" runat="server" Text="Cancel" Font-Bold="True" OnClientClick="HideModalDiv();" 
                                Font-Size="20pt" />
                            </fieldset>
                        </asp:Panel>
        <asp:Button ID="Button1" runat="server" Height="1px" Width="1px" />
        <ajaxToolkit:ModalPopupExtender ID="ModalPopupExtenderLogin" runat="server" 
                                        BackgroundCssClass="modalBackground" DropShadow="true" 
                                        PopupControlID="pnlVoucher" TargetControlID="Button1" />
                        </ContentTemplate></asp:UpdatePanel>
                         


</td>
</tr>
</table></fieldset>
</td>
<td style="width:1%;"></td>
</tr>
<tr>
<td style="width:1%;">&nbsp;</td>
<td style="width:98%;" align="center">
 
</td>
<td style="width:1%;">&nbsp;</td>
</tr>
<tr>
<td style="width:1%;">&nbsp;</td>
<td style="width:98%;" align="center">


    &nbsp;</td>
<td style="width:1%;">&nbsp;</td>
</tr>
<tr>
<td style="width:1%;">&nbsp;</td>
<td style="width:98%;" align="center">


    &nbsp;</td>
<td style="width:1%;">&nbsp;</td>
</tr>
</table>
</div>
<div id="divBackground" style="position: fixed; z-index: 999; height: 100%; width: 100%;
        top: 0; left:0; background-color: Black; filter: alpha(opacity=60); opacity: 0.6; -moz-opacity: 0.8;-webkit-opacity: 0.8;display:none">
    </div>
</asp:Content>

