<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Voucher.aspx.cs" Inherits="Voucher" Title="Currency Gain/Loss.-SDL"  Theme="Themes" MaintainScrollPositionOnPostback="true"%>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxtoolkit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">  

<script src='<%= ResolveUrl("~/Scripts/valideDate.js") %>' type="text/javascript"></script>


<script language="javascript" type="text/javascript" >
    function setDecimal(abc) {
        var dt = document.getElementById(abc).value;
        if (dt.length > 0) {
            document.getElementById(abc).value = parseFloat(dt).toFixed(2);
        }
    }
</script>

<div id="frmMainDiv"  style="width:98.5%; background-color:transparent; padding:10px;">
<div style="vertical-align:top;">
<table  id="pageFooterWrapper">
  <tr>  
      <!--
  <td align="center">
       <asp:Button ID="btnNew" runat="server" ToolTip="New" onclick="btnNew_Click" TabIndex="100" Text="New" 
        Height="25px" Width="100%" BorderStyle="Outset"  /> </td>
       -->
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
                <asp:DropDownList SkinID="ddlPlain" ID="ddlFinMon" runat="server" 
                    AutoPostBack="False" Font-Size="8pt" Visible="False"></asp:DropDownList>
                <asp:TextBox ID="txtVolumeNo" runat="server" AutoPostBack="False" 
                    CssClass="tbc" Font-Size="8pt" MaxLength="30" SkinID="tbGray" 
        Visible="False"></asp:TextBox>
                <asp:DropDownList SkinID="ddlPlain" ID="ddlTransType" runat="server" 
                    AutoPostBack="False" Font-Size="8pt" Visible="False" >
                    <asp:ListItem></asp:ListItem>
                    <asp:ListItem Text="Revenue" Value="R"></asp:ListItem>
                    <asp:ListItem Text="Project" Value="P"></asp:ListItem>
                </asp:DropDownList>
                <asp:TextBox SkinID="tbGray" ID="txtPayee" runat="server" AutoPostBack="False"
                    Font-Size="8pt" MaxLength="200" Visible="False"></asp:TextBox>
</div>
<table style="width:100%; font-family:Verdana; background-color:white;">
<tr>
<td style="width:1%;"></td>
<td style="width:98%;" align="center">
<%--<asp:UpdatePanel ID="UpdatePanel3" runat="server">
<ContentTemplate>--%>
<h2 style="width: 98%; vertical-align: top; color: Maroon;"  align="left">>> Currency Gain/Loss.</h2>
 
   <asp:UpdatePanel ID="detailsupdatepanal" runat="server" UpdateMode="Conditional">
   <ContentTemplate>
    <table style="width: 100%; border: 1px solid silver; padding-right: 10px;">
        <tr>
            <td style="width: 15%;" align="left">
                <asp:Label ID="lblVchSysNo" runat="server" Font-Size="8pt" Width="100%">Voucher No</asp:Label>
            </td>
            <td style="width: 15%;">
                <asp:TextBox SkinID="tbGray" ID="txtVchSysNo" runat="server" Width="100%" CssClass="tbc"
                    AutoPostBack="False" Font-Size="8pt" MaxLength="6"></asp:TextBox>
            </td>
            <td style="width: 5%;" align="left" />
            <td style="width: 15%;" align="left">
                <asp:Label ID="lblRefFileNo1" runat="server" Font-Size="8pt" Width="100%">Voucher 
                Type</asp:Label>
            </td>
            <td style="width: 15%;">
                <asp:DropDownList ID="txtVchCode" runat="server" AutoPostBack="False" 
                    Font-Size="8pt" SkinID="ddlPlain" TabIndex="5" Width="105%">
                    <asp:ListItem></asp:ListItem>
                    <asp:ListItem Text="Journal Voucher" Value="03"></asp:ListItem>
                    <asp:ListItem Text="Debit Voucher" Value="01"></asp:ListItem>
                    <asp:ListItem Text="Credit Voucher" Value="02"></asp:ListItem>
                    <asp:ListItem Text="Contra Voucher" Value="04"></asp:ListItem>
                </asp:DropDownList>
            </td>
            <td style="width: 5%;" align="left" />
            <td style="width: 15%;" align="left">
                <asp:Label ID="LblValueDate" runat="server" Font-Size="8pt" Width="100%">Voucher Date</asp:Label>
            </td>
            <td style="width: 15%;">
                <asp:TextBox SkinID="tbGray" ID="txtValueDate" runat="server" Width="100%" CssClass="tbc"
                    AutoPostBack="False" Font-Size="8pt" MaxLength="11"></asp:TextBox>
                <ajaxtoolkit:CalendarExtender runat="server" ID="Calendarextender2" TargetControlID="txtValueDate" Format="dd/MM/yyyy" />
            </td>
        </tr>

        <tr>
            <td style="width: 15%;" align="left">
                <asp:Label ID="LblVchRefNo" runat="server" Font-Size="8pt" Width="100%">Reference No</asp:Label>
            </td>
            <td style="width: 15%;">
                <asp:TextBox SkinID="tbGray" ID="txtVchRefNo" runat="server" Width="100%" CssClass="tbc"
                    AutoPostBack="False" Font-Size="8pt" MaxLength="30"></asp:TextBox>
            </td>
            <td style="width: 5%;" align="left" />
                &nbsp;<td style="width: 15%;" align="left">
                <asp:Label ID="lblVchCode2" runat="server" Font-Size="8pt" Width="100%">Ref. 
                File No</asp:Label>
            </td>
            <td style="width: 15%;">
                <asp:TextBox ID="txtRefFileNo" runat="server" AutoPostBack="False" 
                    CssClass="tbc" Font-Size="8pt" MaxLength="30" SkinID="tbGray" Width="100%"></asp:TextBox>
            </td>
            <td style="width: 5%;" align="left" />
            <td style="width: 15%;" align="left">
                <asp:Label ID="Label2" runat="server" Font-Size="8pt" Width="100%">Serial No</asp:Label>
            </td>
            <td style="width: 15%;">
                <asp:TextBox SkinID="tbGray" ID="txtSerialNo" runat="server" Width="100%" CssClass="tbc"
                    AutoPostBack="False" Font-Size="8pt" MaxLength="30"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td style="width: 15%; vertical-align: top;" align="left">
                <asp:Label ID="lblParticulars" runat="server" Font-Size="8pt" Width="100%">Particulars</asp:Label>
            </td>
            <td style="width: 88%;" colspan="7">
                <asp:TextBox SkinID="tbGray" ID="txtParticulars" runat="server" Width="100.3%" 
                    AutoPostBack="False" TextMode="MultiLine" MaxLength="1000" Font-Size="8pt"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td style="width: 15%;" align="left">
                <asp:Label ID="lblCheckNo" runat="server" Font-Size="8pt" Width="100%" 
                    Visible="False">Cheque No</asp:Label>
            </td>
            <td style="width: 15%;">
                <asp:TextBox SkinID="tbGray" ID="txtCheckNo" runat="server" Width="100%" CssClass="tbc"
                    AutoPostBack="False" Font-Size="8pt" MaxLength="25" Visible="False"></asp:TextBox>
            </td>
            <td style="width: 5%;" align="left" />
            <td style="width: 15%;" align="left">
                <asp:Label ID="lblCheqDate" runat="server" Font-Size="8pt" Width="100%" 
                    Visible="False">Cheque Date</asp:Label>
            </td>
            <td style="width: 15%;">
                <asp:TextBox SkinID="tbGray" ID="txtCheqDate" runat="server" Width="100%" CssClass="tbc"
                    AutoPostBack="False" Font-Size="8pt" MaxLength="11" Visible="False"></asp:TextBox>
                <ajaxtoolkit:CalendarExtender runat="server" ID="Calendarextender1" TargetControlID="txtCheqDate" Format="dd/MM/yyyy" />
            </td>
            <td style="width: 5%;" align="left" />
            <td style="width: 15%;" align="left">
                <asp:Label ID="lblCheqAmnt" runat="server" Font-Size="8pt" Width="100%" 
                    Visible="False">Cheque Amount</asp:Label>
            </td>
            <td style="width: 15%;">
                <asp:TextBox SkinID="tbGray" ID="txtCheqAmnt" runat="server" Width="100%" CssClass="tbc"
                    AutoPostBack="False" Font-Size="8pt" MaxLength="15" Visible="False"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td style="width: 15%;" align="left">
                <asp:Label ID="lblMoneyRptNo" runat="server" Font-Size="8pt" Width="100%" 
                    Visible="False">Money Rcpt No</asp:Label>
            </td>
            <td style="width: 15%;">
                <asp:TextBox SkinID="tbGray" ID="txtMoneyRptNo" runat="server" Width="100%" CssClass="tbc"
                    AutoPostBack="False" Font-Size="8pt" MaxLength="15" Visible="False"></asp:TextBox>
            </td>
            <td style="width: 5%;" align="left" />
            <td style="width: 15%;" align="left">
                <asp:Label ID="lblMoneyRptDate" runat="server" Font-Size="8pt" Width="100%" 
                    Visible="False">Receipt Date</asp:Label>
            </td>
            <td style="width: 15%;">
                <asp:TextBox SkinID="tbGray" ID="txtMoneyRptDate" runat="server" Width="100%" CssClass="tbc"
                    AutoPostBack="False" Font-Size="8pt" MaxLength="11" Visible="False"></asp:TextBox>
                <ajaxtoolkit:CalendarExtender runat="server" ID="Calendarextender3" 
                    TargetControlID="txtMoneyRptDate" Format="dd/MM/yyyy" />
            </td>
            <td style="width: 5%;" align="left" />
            <td style="width: 15%;" align="left">
                <asp:Label ID="lblControlAmt" runat="server" Font-Size="8pt" Width="100%" 
                    Visible="False">Voucher Amount</asp:Label>
            </td>
            <td style="width: 15%;">
                <asp:TextBox SkinID="tbGray" ID="txtControlAmt" runat="server" Width="100%" CssClass="tbc"
                    AutoPostBack="False" Font-Size="8pt" MaxLength="15" Visible="False"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td style="width: 15%; height: 30px;" align="left">
            </td>
            <td style="width: 15%;" align="left">
                <asp:Label ID="lblStatus" runat="server" Font-Size="8pt">Status</asp:Label>
                <asp:Label ID="txtStatus" runat="server" Enabled="False" Font-Size="8pt" 
                    ForeColor="Blue" MaxLength="1"></asp:Label>
            </td>
            <td style="width: 5%;" align="left" />
            <td style="width: 68%;" align="right" colspan="5">
                <table style="width: 100%;">
                    <tr>
                        <td align="left" style="margin-left: 0em; width: 30%;">
                            <asp:Button ID="lbAuth" runat="server" BorderStyle="Outset" Height="25px" 
                                OnClick="Autho_Click" Text="Authorize" ToolTip="Authorize" Width="100px" />
                            <ajaxToolkit:ModalPopupExtender ID="ModalPopupExtenderLogin" runat="server" 
                                BackgroundCssClass="modalBackground" DropShadow="true" 
                                PopupControlID="LoginPanel" TargetControlID="lbAuth" />
                        </td>
                        <td align="left" style="margin-left: 0em; width: 30%;">
                            <asp:Button ID="lbSetNew" runat="server" BorderStyle="Outset" Height="25px" 
                                OnClick="lbSetNew_Click" Text="Set as New" ToolTip="Set as new voucher" 
                                Visible="False" Width="100px" />
                        </td>
                        <td align="left" style="margin-left: 0em; width: 30%;">
                            <asp:Button ID="btnCheqPrint" runat="server" BorderStyle="Outset" Height="25px" 
                                OnClick="btnCheqPrint_Click" Text="Cheque Print" ToolTip="Cheque Print" 
                                Visible="False" Width="100px" />
                        </td>
                    </tr>
                </table>
            </td>
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
                                <asp:Button ID="CancelBtn" runat="server" Font-Size="8pt" Text="Cancel" Width="60px" OnClick="CancelBtn_Click" />
                            </td>
                            <td style="width: 116px">
                                <asp:Button ID="LoginBtn" runat="server" Font-Size="8pt" Text="Authorize" OnClick="LoginBtn_Click" /></td>
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
    <div style="font-size: 8pt;">
     <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
        <ajaxtoolkit:TabContainer ID="tabVch" runat="server" Width="100%" ActiveTabIndex="1" 
                    Font-Size="8pt">
            <ajaxtoolkit:TabPanel ID="tpVchDtl" runat="server" HeaderText="Voucher Detail Information">
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
                                    <asp:TextBox ID="txtLineNo" runat="server" CssClass="tbc" MaxLength="4" 
                                        SkinID="tbGray" Text='<%#Eval("line_no") %>' Width="93%"></asp:TextBox>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" Width="70px" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="COA Code">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtGlCoaCode" runat="server" AutoPostBack="true"
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
                                    <asp:TextBox ID="txtDebit" runat="server" AutoPostBack="true" CssClass="tbr" 
                                        MaxLength="15" ontextchanged="txtDebit_TextChanged" SkinID="tbGray" 
                                        Text='<%#Eval("amount_dr") %>' Width="95%"></asp:TextBox>
                                </ItemTemplate>
                                <FooterStyle Font-Bold="True" HorizontalAlign="Right" />
                                <HeaderStyle Wrap="False" />
                                <ItemStyle Font-Size="8pt" HorizontalAlign="Right" Width="100px" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Credit">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtCredit" runat="server" AutoPostBack="true" CssClass="tbr" 
                                        MaxLength="15" ontextchanged="txtCredit_TextChanged" SkinID="tbGray" 
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
<ajaxtoolkit:TabPanel ID="tpVchHist" runat="server" HeaderText="Voucher History">
    <ContentTemplate>
        <asp:GridView ID="dgVoucher" runat="server" AllowPaging="True" 
            AllowSorting="true" AlternatingRowStyle-CssClass="alt" 
            AutoGenerateColumns="false" BackColor="White" BorderColor="LightGray" 
            BorderStyle="Solid" BorderWidth="1px" CellPadding="2" CellSpacing="0" 
            CssClass="mGrid" Font-Size="8pt" 
            onpageindexchanging="dgVoucher_PageIndexChanging1" 
            onselectedindexchanged="dgVoucher_SelectedIndexChanged" 
            PagerStyle-CssClass="pgr" PageSize="10" RowStyle-Height="25px" Width="100%"><HeaderStyle 
                BackColor="Silver" Font-Bold="True" Font-Names="Arial" Font-Size="8" 
                ForeColor="Black" HorizontalAlign="center" /><Columns><asp:CommandField 
                    ItemStyle-HorizontalAlign="Center" ItemStyle-Width="40px" 
                    ShowSelectButton="True" /><asp:BoundField DataField="vch_sys_no" 
                    HeaderText="Voucher No" ItemStyle-HorizontalAlign="Center" 
                    ItemStyle-Width="100px" /><asp:BoundField DataField="value_date" 
                    DataFormatString="{0:dd/MM/yyyy}" HeaderText="Voucher Date" 
                    ItemStyle-HorizontalAlign="Center" ItemStyle-Width="100px" /><asp:BoundField 
                    DataField="particulars" HeaderText="Particulars" ItemStyle-Height="20" 
                    ItemStyle-HorizontalAlign="Left"></asp:BoundField><asp:BoundField 
                    DataField="control_amt" DataFormatString="{0:N3}" HeaderText="Amount" 
                    ItemStyle-HorizontalAlign="Right" ItemStyle-Width="100px" /><asp:BoundField 
                    DataField="status" HeaderText="Status" ItemStyle-HorizontalAlign="Center" 
                    ItemStyle-Width="40px" />
            </Columns>
            <RowStyle BackColor="white" />
            <EditRowStyle BackColor="" />
            <PagerStyle HorizontalAlign="Center" />
            <AlternatingRowStyle BackColor="#F5F5F5" />
        </asp:GridView>
    </ContentTemplate>
</ajaxtoolkit:TabPanel>
</ajaxtoolkit:TabContainer>

</ContentTemplate>
        </asp:UpdatePanel>
    </asp:Panel> 
    </div>
</td>
<td style="width:1%;"></td>
</tr>
</table>
</div>
</asp:Content>

