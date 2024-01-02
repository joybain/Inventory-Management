<%--<%@ Page Language="C#" AutoEventWireup="true" CodeFile="DebitVoucherUI.aspx.cs" Inherits="DebitVoucherUI" %>--%>

 <%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="DebitVoucherUI.aspx.cs" Inherits="DebitVoucherUI" Title="Debit Voucher.-DDP"  Theme="Themes" MaintainScrollPositionOnPostback="true"%>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxtoolkit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">  

<%--<script src='<%= ResolveUrl("~/Scripts/valideDate.js") %>' type="text/javascript"></script>--%>

<script language="javascript" type="text/javascript" >
    function setDecimal(abc) {
        var dt = document.getElementById(abc).value;
        if (dt.length > 0) {
            document.getElementById(abc).value = parseFloat(dt).toFixed(2);
        }
    }
    function isNumber(evt) {
        evt = (evt) ? evt : window.event;
        var charCode = (evt.which) ? evt.which : evt.keyCode;
        if (charCode != 46 && charCode > 31
            && (charCode < 48 || charCode > 57)) {
            return false;
        }
        return true;
    }

    function LoadModalDiv() {

        var bcgDiv = document.getElementById("divBackground");
        bcgDiv.style.display = "block";

    }
    function HideModalDiv() {

        var bcgDiv = document.getElementById("divBackground");
        bcgDiv.style.display = "none";

    }

</script>

<div id="frmMainDiv"  style="width:99.5%; background-color:transparent; padding:3px;">

<table  id="pageFooterWrapper">
  <tr> 
       <td align="center" >
       <asp:Button ID="Delete" runat="server" ToolTip="Delete" onclick="Delete_Click"
           
               
               onclientclick="javascript:return window.confirm('are u really want to delete these data')" Text="Delete" 
        Height="35px" Width="60%" BorderStyle="Outset"  />
        </td> 
               
       <td align="center" >
       <asp:Button ID="Find" runat="server" ToolTip="Find"  onclick="Find_Click" Text="Find" 
        Height="35px" Width="60%" BorderStyle="Outset"  />
       </td>                 
       
       <td align="center" >
       <asp:Button ID="btnSave" runat="server" ToolTip="Save Voucher" 
               onclick="btnSave_Click" Text="Save" 
        Height="35px" Width="60%" BorderStyle="Outset"  />
       </td>

         <td align="center" >
       <asp:Button ID="Clear" runat="server"  ToolTip="Clear" onclick="Clear_Click" Text="Clear" 
        Height="35px" Width="60%" BorderStyle="Outset"  />
       </td>

       <td align="center" style="cursor:hand;">
       <asp:Button ID="btnPrint" runat="server" ToolTip="Print"  onclick="btnPrint_Click" Text="Print" 
        Height="35px" Width="60%" BorderStyle="Outset"  />   </td>
  </tr>
   </table>

<table style="width:100%; font-family:Verdana; background-color:white;">
<tr>
<td style="width:1%;">&nbsp;</td>
 <td style="width: 98%; vertical-align: top; color: Maroon;"  align="left"><h2>>> Debit Or Payment Voucher.</h2></td>
<td style="width:1%;">&nbsp;</td>
</tr>
<tr>
<td style="width:1%;"></td>
<td style="width:98%;" align="center">
<%--<asp:UpdatePanel ID="UpdatePanel3" runat="server">
<ContentTemplate>--%>
   <asp:UpdatePanel ID="detailsupdatepanal" runat="server" UpdateMode="Conditional">
   <ContentTemplate>
    <table style="width: 100%; border: 1px solid silver;">

         <tr>
            <td align="left" colspan="9">
                <asp:Label ID="lblRefFileNo" runat="server" Font-Size="8pt" Visible="False"  style="display:none;">Voucher Type</asp:Label>
                <asp:DropDownList ID="txtVchCode" runat="server" AutoPostBack="False" Enabled="False" Font-Size="8pt" SkinID="ddlPlain" TabIndex="5" Visible="False" style="display:none;">
                    <asp:ListItem Text="Debit Voucher" Value="01"></asp:ListItem>
                </asp:DropDownList>
                <asp:Label ID="lblFinMon" runat="server" Font-Size="8pt" Visible="False" style="display:none;">Financial Year</asp:Label>
                <asp:DropDownList ID="ddlFinMon" runat="server" AutoPostBack="False" Enabled="False" Font-Size="8pt" SkinID="ddlPlain" Visible="False" style="display:none;">
                </asp:DropDownList>
                <asp:Label ID="lblMoneyRptNo" runat="server" Font-Size="8pt" style="display:none;">Money Rcpt No</asp:Label>
                <asp:TextBox ID="txtMoneyRptNo" runat="server" AutoPostBack="False" CssClass="tbc" Font-Size="8pt" MaxLength="15" SkinID="tbGray" style="display:none;"></asp:TextBox>
                <asp:Label ID="lblMoneyRptDate" runat="server" Font-Size="8pt" style="display:none;">Receipt Date</asp:Label>
                <asp:TextBox ID="txtMoneyRptDate" runat="server" AutoPostBack="False" CssClass="tbc" Font-Size="8pt" MaxLength="11" SkinID="tbGray" style="display:none;"></asp:TextBox>
                <ajaxToolkit:CalendarExtender ID="txtMoneyRptDate0_CalendarExtender" runat="server" Format="dd/MM/yyyy" TargetControlID="txtMoneyRptDate" />
                <asp:Label ID="lblControlAmt" runat="server" Font-Size="8pt" style="display:none;">Voucher Amount</asp:Label>
                <asp:TextBox ID="txtControlAmt" runat="server" AutoPostBack="False" CssClass="tbc" Font-Size="8pt" MaxLength="15" SkinID="tbGray" style="display:none;"></asp:TextBox>
                <asp:Label ID="lblPayee" runat="server" Font-Size="8pt" Visible="False" style="display:none;">Payee</asp:Label>
                <asp:TextBox ID="txtPayee" runat="server" AutoPostBack="False" Font-Size="8pt" MaxLength="200" SkinID="tbGray" Visible="False" style="display:none;"></asp:TextBox>
                <asp:Button ID="btnCheqPrint" runat="server" BorderStyle="Outset" Height="25px" OnClick="btnCheqPrint_Click" Text="Cheque Print" ToolTip="Cheque Print" Width="100px"  style="display:none;" />
                <asp:Button ID="lbSetNew" runat="server" BorderStyle="Outset" Height="25px" OnClick="lbSetNew_Click" Text="Set as New" ToolTip="Set as new voucher" Width="100px" style="display:none;" />
                <asp:Label ID="Label3" runat="server" Font-Size="8pt" Visible="False" style="display:none;" >Revenue/Project</asp:Label>
                <asp:DropDownList ID="ddlTransType" runat="server" AutoPostBack="False"  Font-Size="8pt" SkinID="ddlPlain" Visible="False" style="display:none;" >
                    <asp:ListItem></asp:ListItem>
                    <asp:ListItem Text="Revenue" Value="R"></asp:ListItem>
                    <asp:ListItem Text="Project" Value="P"></asp:ListItem>
                </asp:DropDownList>
                <asp:Label ID="lblVchCode" runat="server" Font-Size="8pt" Visible="False" style="display:none;" >Ref. File No</asp:Label>
                <asp:TextBox ID="txtRefFileNo" runat="server" AutoPostBack="False" CssClass="tbc" Font-Size="8pt" MaxLength="30" SkinID="tbGray" Visible="False" style="display:none;" ></asp:TextBox>
                <asp:Label ID="Label15" runat="server" Font-Size="8pt" Visible="False" style="display:none;" >Volume No</asp:Label>
                <asp:TextBox ID="txtVolumeNo" runat="server" AutoPostBack="False" CssClass="tbc" Font-Size="8pt" MaxLength="30" SkinID="tbGray" Visible="False" style="display:none;" ></asp:TextBox>
                <asp:DropDownList ID="ddlPayMode" runat="server" AutoPostBack="True" style="display:none;"
                    Font-Size="8pt" Height="18px" SkinID="ddlPlain" Width="50px">
                    <asp:ListItem></asp:ListItem>
                    <asp:ListItem Text="Cash" Value="C"></asp:ListItem>
                    <asp:ListItem Text="Cheque" Value="Q"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td align="left" style="width: 13%;">
                <asp:Label ID="lblVchSysNo" runat="server" Font-Size="8pt" Width="100%" 
                    style="font-weight: 700">Voucher No</asp:Label>
            </td>
            <td style="width: 15%;">
                <asp:TextBox ID="txtVchSysNo" runat="server" AutoPostBack="False" 
                    CssClass="tbc" Font-Size="8pt" MaxLength="20" SkinID="tbGray" Width="100%" 
                    Visible="False" Enabled="False"></asp:TextBox>
                <asp:TextBox ID="txtVchSysNo0" runat="server" AutoPostBack="False" 
                    CssClass="tbc" Enabled="False" Font-Size="8pt" MaxLength="20" SkinID="tbGray" 
                    Width="100%"></asp:TextBox>
            </td>
            <td style="width: 5%;" align="left" />
            &nbsp;<td align="left" style="width: 14%;">
                <asp:Label ID="LblValueDate0" runat="server" Font-Size="8pt" 
                    style="font-weight: 700">Voucher Date</asp:Label>
            </td>
            <td style="width: 15%;">
                <asp:TextBox ID="txtValueDate" runat="server" AutoPostBack="False" 
                    CssClass="tbc" Font-Size="8pt" MaxLength="11" SkinID="tbGray" Width="100%"></asp:TextBox>
                <ajaxToolkit:CalendarExtender ID="txtValueDate_CalendarExtender" runat="server" 
                    Format="dd/MM/yyyy" TargetControlID="txtValueDate" />
            </td>
            <td style="width: 4%;" align="left" />
            <td align="left" style="width: 15%;">
                <asp:Label ID="lblForStatus" runat="server" Font-Size="8pt" Width="100%">Total 
                Cash in Hand :</asp:Label>
            </td>
            <td align="right" colspan="2">
                <asp:Label ID="lblAmountStatus" runat="server" Font-Bold="True" 
                    Font-Size="Small" Text="0.00" ForeColor="#FF3300"></asp:Label>
            </td>
        </tr>

        <tr>
            <td style="width: 13%;" align="left">
                <asp:Label ID="LblVchRefNo" runat="server" Font-Size="8pt" Width="100%">Reference No</asp:Label>
            </td>
            <td style="width: 15%;">
                <asp:TextBox SkinID="tbGray" ID="txtVchRefNo" runat="server" Width="100%" CssClass="tbc"
                    AutoPostBack="False" Font-Size="8pt" MaxLength="30"></asp:TextBox>
            </td>
            <td style="width: 5%;" align="left" />
                &nbsp;<td style="width: 14%;" align="left">
                <asp:Label ID="Label11" runat="server" Font-Size="8pt" Width="100%">Manual 
                Voucher No.</asp:Label>
            </td>
            <td style="width: 15%;">
                <asp:TextBox ID="txtSerialNo" runat="server" AutoPostBack="False" 
                    CssClass="tbc" Font-Size="8pt" MaxLength="30" SkinID="tbGray" Width="100%"></asp:TextBox>
            </td>
            <td style="width: 4%;" align="left" />
            <td style="width: 15%;" align="left">
                <asp:Label ID="lblBankStatus" runat="server" Font-Size="8pt">Total Cash at Bank :</asp:Label>
            </td>
            <td align="right" colspan="2">
                <asp:Label ID="lblBankAmountStatus" runat="server" Font-Bold="True" 
                    Font-Size="Small" ForeColor="#CC3300" Text="0.00"></asp:Label>
            </td>
        </tr>
        <tr>
            <td style="width: 13%; vertical-align: top;" align="left">
                <asp:Label ID="lblParticulars" runat="server" Font-Size="8pt" Width="100%" 
                    style="font-weight: 700">Particulars</asp:Label>
            </td>
            <td colspan="8">
                <asp:TextBox SkinID="tbGray" ID="txtParticulars" runat="server" Width="99%" 
                    AutoPostBack="False" TextMode="MultiLine"  Font-Size="8pt" 
                    Height="50px"></asp:TextBox>
                     <ajaxToolkit:AutoCompleteExtender ID="autoComplete" runat="server" 
                                        CompletionInterval="20" CompletionSetCount="12" EnableCaching="true" 
                                        MinimumPrefixLength="1" ServiceMethod="GetShowRemarks" 
                                        ServicePath="AutoComplete.asmx" TargetControlID="txtParticulars" />
            </td>
        </tr>
        <tr>
            <td style="width: 13%;" align="left">
                <asp:Label ID="lblBank" runat="server" Font-Size="8pt">Bank Name</asp:Label>
            </td>
            <td style="width: 15%;">
                <asp:DropDownList ID="ddlBank" runat="server" AutoPostBack="True" 
                    Font-Size="8pt" Height="18px" SkinID="ddlPlain" Width="100%">
                    <asp:ListItem></asp:ListItem>
                    <asp:ListItem Text="Cash" Value="C"></asp:ListItem>
                    <asp:ListItem Text="Cheque" Value="Q"></asp:ListItem>
                </asp:DropDownList>
            </td>
            <td style="width: 5%;" align="center" />
            <asp:Label ID="lblColor" runat="server" Font-Bold="True" ForeColor="Red" 
                Text="*"></asp:Label>
            <td style="width: 14%;" align="left">
                <asp:Label ID="lblCheckNo" runat="server" Font-Size="8pt" Width="100%">Cheque No</asp:Label>
            </td>
            <td style="width: 15%;">
                <asp:TextBox ID="txtCheckNo" runat="server" AutoPostBack="True" Width="100%"></asp:TextBox>
            </td>
            <td style="width: 4%;" align="center" />
            <asp:Label ID="lblColor0" runat="server" Font-Bold="True" Font-Size="Medium" 
                ForeColor="Red" Text="*"></asp:Label>
            <td style="width: 15%;" align="left">
                <asp:Label ID="lblCheqDate" runat="server" Font-Size="8pt" Width="100%">Cheque 
                Date</asp:Label>
            </td>
            <td style="width: 15%;">
                <asp:TextBox ID="txtCheqDate" runat="server" AutoPostBack="False" 
                    CssClass="tbc" Font-Size="8pt" MaxLength="11" SkinID="tbGray" Width="100%"></asp:TextBox>
                <ajaxToolkit:CalendarExtender ID="txtCheqDate_CalendarExtender" runat="server" 
                    Format="dd/MM/yyyy" TargetControlID="txtCheqDate" />
            </td>
            <td></td>
        </tr>
        <tr>
            <td style="width: 13%; " align="left">
                <asp:Label ID="lblCheqAmnt" runat="server" Font-Size="8pt" Width="100%">Cheque 
                Amount</asp:Label>
            </td>
            <td style="width: 15%">
                <asp:TextBox ID="txtCheqAmnt" runat="server" AutoPostBack="False" 
                    CssClass="tbc" Enabled="False" Font-Size="8pt" MaxLength="15" SkinID="tbGray" 
                    Width="99%"></asp:TextBox>
            </td>
            <td style="width: 5%;" align="center" />
            &nbsp;<td align="left" style="width: 14%;">
                <asp:TextBox ID="txtAmt" runat="server" AutoPostBack="False" CssClass="tbc" 
                    Font-Size="8pt" MaxLength="30" SkinID="tbGray" Width="100%" 
                    Visible="False">0</asp:TextBox>
            </td>
            <td style="width: 15%;">
                &nbsp;</td>
            <td style="width: 4%;" align="left" />
            <asp:Label ID="lblStatus" runat="server" Font-Size="8pt" Width="100%">Status</asp:Label>
            <td align="left" style="width: 15%;">
                <asp:Label ID="txtStatus" runat="server" Enabled="False" Font-Size="8pt" 
                    ForeColor="Blue" MaxLength="1" Width="100%"></asp:Label>
            </td>
            <td align="left" style="width: 15%;">
                <asp:Button ID="lbAuth" runat="server" BorderStyle="Outset" Height="25px"  OnClientClick="LoadModalDiv();" 
                    OnClick="Autho_Click" Text="Authorize" ToolTip="Authorize" Width="100px" />
                <ajaxToolkit:ModalPopupExtender ID="ModalPopupExtenderLogin" runat="server" 
                    BackgroundCssClass="modalBackground" DropShadow="true" 
                    PopupControlID="LoginPanel" TargetControlID="lbAuth" />
            </td>
            <td></td>
        </tr>
        <tr>
            <td style="width: 100%" colspan="8"></td>
            <td align="left">

                <asp:Panel ID="LoginPanel" runat="server" CssClass="modalPopup" Style="display: none; padding: 15px 15px 15px 15px; background-color: White; border: 1px solid black;" Width="250px" Height="80px">

                    <table style="width: 250px;">
                        <tr>
                            <td style="width: 150px" align="left">
                                <asp:Label ID="lblUserName" runat="server" Font-Size="8pt" Height="23px" Text="Login ID" Width="100px"></asp:Label>
                            </td>
                            <td style="width: 116px">
                                <asp:TextBox SkinID="tbGray" ID="loginId" runat="server" Font-Size="8pt" Width="115px"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td style="width: 150px" align="left">
                                <asp:Label ID="lblPassword" runat="server" Font-Size="8pt" Height="23px" Text="Password" Width="100px"></asp:Label>
                            </td>
                            <td style="width: 116px">
                                <asp:TextBox SkinID="tbGray" ID="pwd" runat="server" Font-Size="8pt" Width="115px" TextMode="Password"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td style="width: 150px">
                                <asp:Button ID="CancelBtn" OnClientClick="HideModalDiv();" runat="server" Font-Size="8pt" Text="Cancel" Width="60px" OnClick="CancelBtn_Click" />
                            </td>
                            <td style="width: 116px">
                                <asp:Button ID="LoginBtn" OnClientClick="HideModalDiv();" runat="server" Font-Size="8pt" Text="Authorize" OnClick="LoginBtn_Click" /></td>
                        </tr>
                    </table>
                </asp:Panel>

            </td>
        </tr>
    </table>
    </ContentTemplate>
    </asp:UpdatePanel>

    <asp:Label ID="lblTranStatus" runat="server" Width="500px" Text="" Visible="false" Font-Size="8"></asp:Label>

   <br />
<asp:Panel ID="pnlVch" runat="server" Width="100%" BorderWidth="1px" BorderColor="LightGray">
    <div style="font-size: 8pt;" align="left">
     <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
        <ajaxtoolkit:TabContainer ID="tabVch" runat="server" Width="99%" ActiveTabIndex="1" 
                    Font-Size="8pt">
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <ajaxtoolkit:TabPanel ID="tpVchDtl" runat="server" HeaderText="Voucher Detail Information">
                <HeaderTemplate>
                    Voucher Detail Information
                </HeaderTemplate>
                <ContentTemplate>
                    <asp:GridView ID="dgVoucherDtl" runat="server" 
                        AllowSorting="True" 
                        AutoGenerateColumns="False" BackColor="White" BorderColor="LightGray" 
                        BorderStyle="Solid" BorderWidth="1px" CellPadding="2" 
                        CssClass="mGrid" Font-Size="8pt" onrowdatabound="dgVoucherDtl_RowDataBound" 
                        onrowdeleting="dgVoucherDtl_RowDeleting" Width="100%">
                        <Columns>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:ImageButton ID="lbDelete" runat="server" CausesValidation="False" 
                                        CommandName="Delete" ImageAlign="Middle" ImageUrl="~/img/delete.png" 
                                        Text="Delete" />
                                </ItemTemplate>
                                <ItemStyle Font-Size="8pt" HorizontalAlign="Center" Width="17px" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Line#">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtLineNo" runat="server" CssClass="tbc" MaxLength="4" Enabled="False"
                                        SkinID="tbGray" Text='<%#Eval("line_no") %>' Width="93%"></asp:TextBox>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" Width="70px" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="COA Code">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtGlCoaCode" runat="server" AutoPostBack="true" Enabled="False"
                                        CssClass="tbc" Font-Size="8" MaxLength="13" 
                                        ontextchanged="txtGlCode_TextChanged" SkinID="tbGray" 
                                        Text='<%#Eval("gl_coa_code") %>' Width="93%"></asp:TextBox>                                   
                                </ItemTemplate>
                                <ItemStyle Font-Size="8pt" Height="18px" Width="90px" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="COA Description">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtCoaDesc" runat="server" autocomplete="off" placeHolder="Search chart-of-accounts.."
                                        AutoPostBack="true" Font-Size="8" MaxLength="150" 
                                        ontextchanged="txtCoaDesc_TextChanged" SkinID="tbGray" 
                                        Text='<%#Eval("particulars") %>' Width="98%"></asp:TextBox>
                                    <ajaxToolkit:AutoCompleteExtender ID="autoComplete" runat="server" 
                                        CompletionInterval="20" CompletionSetCount="12" EnableCaching="true" 
                                        MinimumPrefixLength="1" ServiceMethod="GetCompletionList" 
                                        ServicePath="AutoComplete.asmx" TargetControlID="txtCoaDesc" />
                                </ItemTemplate>
                                <ItemStyle Font-Size="8pt" HorizontalAlign="Left" Width="300px" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Debit">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtDebit" runat="server" AutoPostBack="true" CssClass="tbr"  onFocus="this.select()" onkeypress="return isNumber(event)"
                                        MaxLength="25" ontextchanged="txtDebit_TextChanged" SkinID="tbGray" 
                                        Text='<%#Eval("amount_dr") %>' Width="95%"></asp:TextBox>
                                </ItemTemplate>
                                <FooterStyle Font-Bold="True" HorizontalAlign="Right" />
                                <HeaderStyle Wrap="False" />
                                <ItemStyle Font-Size="8pt" HorizontalAlign="Right" Width="100px" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Credit">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtCredit" runat="server" AutoPostBack="true" CssClass="tbr" onFocus="this.select()" onkeypress="return isNumber(event)"
                                        MaxLength="25" ontextchanged="txtCredit_TextChanged" SkinID="tbGray" 
                                        Text='<%#Eval("amount_cr") %>' Width="95%"></asp:TextBox>
                                </ItemTemplate>
                                <FooterStyle Font-Bold="True" HorizontalAlign="Right" />
                                <ItemStyle Font-Size="8pt" HorizontalAlign="Right" Width="100px" />
                            </asp:TemplateField>
                        </Columns>
                        <RowStyle BackColor="White" Height="25px" />
                        <PagerStyle HorizontalAlign="Center" CssClass="pgr" />
                        <HeaderStyle Font-Bold="True" ForeColor="Black" HorizontalAlign="Center" />
                        <AlternatingRowStyle BackColor="WhiteSmoke" CssClass="alt" />
                    </asp:GridView>
                </ContentTemplate>
</ajaxtoolkit:TabPanel> 
            <ajaxToolkit:TabPanel ID="tpVchHist" runat="server" 
                HeaderText="Voucher History">
                <HeaderTemplate>
                    Voucher History
                </HeaderTemplate>
                <ContentTemplate>
                    <table style="width: 100%">
                        <tr>
                            <td colspan="4">
                                <fieldset style="vertical-align: top; border: solid 1px #8BB381; text-align:left;width: 50%">
                                    <legend style="color: maroon;"><b>Search Option</b></legend>
                                    <table style="width: 100%">
                                        <tr>
                                            <td style="width: 44%">
                                                <asp:Label ID="Label14" runat="server" style="font-weight: 700" 
                                                    Text="Search Voucher No:"></asp:Label>
                                            </td>
                                            <td style="width: 60%">
                                                <asp:TextBox ID="txtVoucher" runat="server" onkeypress="return isNumber(event)" CssClass="txtVisibleAlign" 
                                                    Placeholder="Input Voucher No and click Find Button" Width="100%" 
                                                    Height="20px"></asp:TextBox>
                                            </td>
                                            <td style="width: 261px">
                                                &nbsp;</td>
                                            <td>
                                                &nbsp;</td>
                                        </tr>
                                    </table>
                                </fieldset>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="4">
                                <asp:GridView ID="dgVoucher" runat="server" AllowPaging="True" 
                                    AllowSorting="True" AutoGenerateColumns="False" BackColor="White" 
                                    BorderColor="LightGray" BorderStyle="Solid" BorderWidth="1px" CellPadding="2" 
                                    CssClass="mGrid" Font-Size="8pt" 
                                    OnPageIndexChanging="dgVoucher_PageIndexChanging1" 
                                    OnSelectedIndexChanged="dgVoucher_SelectedIndexChanged" PageSize="50" 
                                    Width="100%">
                                    <AlternatingRowStyle BackColor="WhiteSmoke" CssClass="alt" />
                                    <Columns>
                                        <asp:CommandField ShowSelectButton="True">
                                             <ItemStyle HorizontalAlign="Center" Width="55px" Font-Bold="True" 
                                            Font-Size="10pt" />
                                        </asp:CommandField>
                                        <asp:BoundField DataField="vch_sys_no" HeaderText="Voucher No">
                                        <ItemStyle HorizontalAlign="Center" Width="80px" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="serial_no" HeaderText="M.V. No">
                                        <ItemStyle HorizontalAlign="Center" Width="80px" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="VCH_REF_NO" HeaderText="Voucher Type">
                                        <ItemStyle HorizontalAlign="Center" Width="95px" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="value_date" DataFormatString="{0:dd/MM/yyyy}" 
                                            HeaderText="Voucher Date">
                                        <ItemStyle HorizontalAlign="Center" Width="92px" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="particulars" HeaderText="Particulars">
                                        <ItemStyle Height="20px" HorizontalAlign="Left" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="control_amt" DataFormatString="{0:N3}" 
                                            HeaderText="Amount">
                                        <ItemStyle HorizontalAlign="Right" Width="100px" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="status" HeaderText="Status">
                                        <ItemStyle HorizontalAlign="Center" Width="40px" />
                                        </asp:BoundField>
                                    </Columns>
                                    <HeaderStyle BackColor="Silver" Font-Bold="True" Font-Names="Arial" 
                                        Font-Size="8pt" ForeColor="Black" HorizontalAlign="Center" />
                                    <PagerStyle CssClass="pgr" HorizontalAlign="Center" />
                                    <RowStyle BackColor="White" Height="25px" />
                                </asp:GridView>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 250px">
                                &nbsp;</td>
                            <td style="width: 192px">
                                &nbsp;</td>
                            <td style="width: 264px">
                                &nbsp;</td>
                            <td>
                                &nbsp;</td>
                        </tr>
                    </table>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
</ajaxtoolkit:TabContainer>

</ContentTemplate>
        </asp:UpdatePanel>
          </div>
    </asp:Panel> 
  
</td>
<td style="width:1%;"></td>
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

