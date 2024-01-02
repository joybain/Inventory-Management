<%@ Page Language="C#" AutoEventWireup="true" CodeFile="rptLedgerStat.aspx.cs" Inherits="rptLedgerStat" %>
<%--<%@ Register Assembly="CrystalDecisions.Web, Version=13.0.2000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304"
    Namespace="CrystalDecisions.Web" TagPrefix="CR" %>--%>
    
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<style type="text/css">
tHead
{
  display : table-header-group;
}
    
.mGrid {   
    width: 100%;   
    background-color: #fff;   
    margin: 5px 0 10px 0;   
    border: solid 1px #525252;   
    border-collapse:collapse;   
}  
input.tbc
{
 text-align:center; 
}

input[type=text],textarea, input[type=password], select
{
	background: #ffffff url('images/bg_ip.png') repeat-x;
	padding: 3px;
	font-size: 10px;
	color: #000000;
	font-weight: bold;
	border: 1px solid #c0c0c0;
	height:18px;
    margin-left: 0;
    margin-right: 0;
    }

input.tbr
{
 text-align:right;
}

.mGrid .alt { background: #fcfcfc url('css/black/img/grd_alt.png') repeat-x 50% top;
    }  
    </style> 
    <title>Ledger Statement</title>
    <link href="css/GrideView.css" rel="stylesheet" type="text/css" />
</head>
<body onload="javascript: addHeaders() " style="margin-top:0em;">
<form id="form1" runat="server"> 
   
<script language="JavaScript" type="text/javascript">
function PrintContent(elementId)
{
 var printContent = document.getElementById(elementId); 
 var windowUrl = 'about:blank';
 var uniqueName = new Date();
 var windowName = 'Print' + uniqueName.getTime();
 document.getElementById("<%=lblOrg.ClientID %>").style.textAlign="center";
 document.getElementById("<%=lblAddress1.ClientID %>").style.textAlign="center";
 document.getElementById("<%=lblAddress2.ClientID %>").style.textAlign="center";   
 document.getElementById("<%=lblTitle.ClientID %>").style.textAlign="center";
 var printWindow = window.open(windowUrl, windowName, 'width=800,height=600,top=0,left=10,toolbars=no,scrollbars=no,status=no,resizable=no'); 
 printWindow.document.write(printContent.innerHTML);  
 printWindow.document.close();
 printWindow.focus();
 //printWindow.print();
 //printWindow.close();
}
function getNewUrl(newVal)
{
    var testwindow1 = window.open(location.href+newVal);
}
function AddTHEAD(tableName)
{
   var table = document.getElementById(tableName); 
   if(table != null) 
   {
    var head = document.createElement("THEAD");
    head.style.display = "table-header-group";
    head.appendChild(table.rows[0]);
    table.insertBefore(head, table.childNodes[0]); 
   }
}
function addHeaders()
{
    AddTHEAD('dgLedger');
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
<div style="min-height:560px; height:auto !important; text-align:center; width:100%; margin-top:0em;">
<table style="width:100%;">
<tr>
<td style="width:10%;">&nbsp;</td>
<td style="width:80%;">

    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
</td>
<td style="width:10%;">&nbsp;</td>
</tr>
<tr>
<td style="width:10%;"></td>
<td style="width:80%;">

<table id ="tblOver" style="width:900px; margin-top:0em;" >
<thead>
<tr align="center">
<td style=" width:900px; text-align:center;" align="center">
<table style="width:100%;">
<tr style="width:100%;">
<td style="width:100%;text-align:center;" align="center">
<asp:Label ID="lblOrg" runat="server" Visible="True" Text="" Font-Size="14pt" Height="20px" Width="100%"></asp:Label>
</td>
</tr>
<tr style="width:100%;">
<td style="width:100%;" align="center">
<asp:Label ID="lblAddress1" runat="server" Visible="True" Text="" Font-Size="8pt" Height="15px" Width="100%"></asp:Label>
</td>
</tr>
<tr style="width:100%;">
<td style="width:100%;" align="center">
<asp:Label ID="lblAddress2" runat="server" Visible="True" Text="" Font-Size="8pt" Height="15px" Width="100%"></asp:Label>
</td>
</tr>
<tr style="width:100%;">
<td style="width:100%;" align="center">
<asp:Label ID="lblTitle" runat="server" Font-Size="10pt" 
        Height="15px" Width="100%" style="font-size: small" Font-Bold="True"></asp:Label>
</td>
</tr>
</table>
</td>
</tr>
</thead>
<tr>
<td align="center" >

<asp:Label ID="lblDate" runat="server" Font-Size="Small" Width="100%" 
        style="font-size: small" Font-Bold="False"></asp:Label>
    </td>
</tr>
</table>
</td>
<td style="width:10%;"></td>
</tr>
<tr>
<td colspan="3" align="right">  
    <table style="width:40%;border: solid 1px #8BB381;">
        <tr>
            <td align="left" colspan="2" >

                <asp:RadioButtonList ID="rbType" runat="server" AutoPostBack="True" 
                    BorderStyle="Solid" onselectedindexchanged="rbType_SelectedIndexChanged" 
                    RepeatDirection="Horizontal" Width="60%" Font-Bold="True" Font-Size="15pt">
                    <asp:ListItem Selected="True" Value="A">All</asp:ListItem>
                    <asp:ListItem Value="D">Debit</asp:ListItem>
                    <asp:ListItem Value="C">Credit</asp:ListItem>
                </asp:RadioButtonList>

            </td>
        </tr>
        <tr>
            <td align="center" >

<asp:LinkButton ID="lbExp" runat="server" Text="Export Report to PDF" 
        Font-Size="11pt" ForeColor="Blue" onclick="lbExp_Click" 
        style="height: 15px" Font-Bold="True"></asp:LinkButton>

            </td>
            <td align="center">

<asp:LinkButton ID="lbExpExcel" runat="server" Text="Export Report to Excel" 
        Font-Size="11pt" ForeColor="Blue" 
        onclick="lbExpExcel_Click" Font-Bold="True">
    
</asp:LinkButton>
            </td>
        </tr>
    </table>
    </td>
</tr>
<tr>
<td colspan="3">
    <%--<asp:UpdatePanel ID="UP1" runat="server" UpdateMode="Conditional" ><ContentTemplate>--%>
<asp:GridView ID="dgLedger" runat="server" AutoGenerateColumns="False" 
        Visible="False" style="text-align:center;" 
        AllowPaging="True" PageSize="50" Width="100%" BorderWidth="1px" BorderStyle="Solid"
        CellPadding="2" BorderColor="Black" Font-Size="8pt"
        Font-Names="Verdana" onrowdatabound="dgLedger_RowDataBound" 
        onpageindexchanging="dgLedger_PageIndexChanging" CssClass="Grid" 
        onrowcommand="dgLedger_RowCommand" 
        onselectedindexchanged="dgLedger_SelectedIndexChanged">
  <Columns> 
      <asp:TemplateField HeaderText="Select" ItemStyle-Width="25px">
        <ItemTemplate>
            <asp:LinkButton ID="lblSelect" Visible="True"  runat="server" 
                CommandName="Select" Text="( Select )" Font-Bold="True" Font-Size="10pt" 
                Font-Underline="False"></asp:LinkButton>
        </ItemTemplate>
        <ItemStyle HorizontalAlign="Center" Width="75px" />
      </asp:TemplateField>
       <asp:BoundField DataField="vch_sys_no" HeaderText="Voucher No.">
            <ItemStyle HorizontalAlign="Center" Height="15px" Width="70px"></ItemStyle>
      </asp:BoundField>
      <asp:BoundField DataField="value_date" HeaderText="Date">
       <ItemStyle HorizontalAlign="Center" Height="10px" Width="50px"></ItemStyle>
      </asp:BoundField>
  <asp:BoundField HeaderText="M.V.NO" ItemStyle-Width="40px" ItemStyle-HorizontalAlign="Center" ItemStyle-Height="15px" DataField="vch_manual_no">  
                <ItemStyle HorizontalAlign="Center" Height="15px" Width="50px"></ItemStyle>
      </asp:BoundField>
      <asp:BoundField DataField="VCH_REF_NO" HeaderText="Vch Ref No" >
      <ItemStyle HorizontalAlign="Center" Height="15px" Width="100px"></ItemStyle>
      </asp:BoundField>
      <asp:BoundField HeaderText="Head Name" ItemStyle-Width="150px" 
          DataField="Part"  ItemStyle-HorizontalAlign="Left" > 
<ItemStyle HorizontalAlign="Left" Width="250px"></ItemStyle>
      </asp:BoundField>
  <asp:BoundField HeaderText="Particulars" ItemStyle-Width="250px" 
          DataField="particulars"  ItemStyle-HorizontalAlign="Left" > 
<ItemStyle HorizontalAlign="Left" Width="250px"></ItemStyle>
      </asp:BoundField>
  <asp:BoundField HeaderText="Debit" ItemStyle-Width="150px" DataField="debit_amt" 
          DataFormatString="{0:N3}" ItemStyle-HorizontalAlign="Right">  
<ItemStyle HorizontalAlign="Right" Width="110px"></ItemStyle>
      </asp:BoundField>
  <asp:BoundField HeaderText="Credit" ItemStyle-Width="150px" DataField="credit_amt" 
          DataFormatString="{0:N3}" ItemStyle-HorizontalAlign="Right" >  
<ItemStyle HorizontalAlign="Right" Width="110px"></ItemStyle>
      </asp:BoundField>
  <asp:BoundField HeaderText="Balance" ItemStyle-Width="150px" DataField="bal" 
          DataFormatString="{0:N3}" ItemStyle-HorizontalAlign="Right" >    
<ItemStyle HorizontalAlign="Right" Width="110px"></ItemStyle>
      </asp:BoundField>
  </Columns>
                        <RowStyle BackColor="White" />
                        <PagerStyle HorizontalAlign="Center" />
                        <AlternatingRowStyle BackColor="" />
</asp:GridView>

<br/>

  <asp:Panel ID="pnlChangePass" runat="server"  CssClass="modalPopup" 
                            Style=" display:none; background-color: White; width:800px; height:auto; ">
                            <fieldset style=" text-align:left; vertical-align: top; border: solid 1px #8BB381;line-height:1.5em;"><legend style="color: maroon;"><b>View Ledger Information</b></legend>

                                <asp:GridView ID="dgVoucherDtl" runat="server" AllowSorting="True" 
                                    AutoGenerateColumns="False" BackColor="White" BorderColor="LightGray" 
                                    BorderStyle="Solid" BorderWidth="1px" CellPadding="2" CssClass="Grid" 
                                    Font-Size="8pt" Width="100%" onrowdatabound="dgVoucherDtl_RowDataBound">
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
                                <asp:button id="btnCancel" runat="server" Text="Cancel" Font-Bold="True" 
                                    Font-Size="20pt"/>
                            </fieldset>
                        </asp:Panel>
                         <asp:Button ID="Button1" runat="server" Visible="False" />
    <ajaxToolkit:ModalPopupExtender ID="ModalPopupExtenderLogin" runat="server" 
                    BackgroundCssClass="modalBackground" DropShadow="true" 
                    PopupControlID="pnlChangePass" TargetControlID="Button1" />
    <%--</ContentTemplate>
</asp:UpdatePanel>--%>
    </td>
</tr>
<tr>
<td style="width:10%;">&nbsp;</td>
<td style="width:80%;">

<asp:Label ID="lblTranStatus" runat="server" Visible="false" Text="" Font-Size="8pt" Width="100%"></asp:Label>
</td>
<td style="width:10%;">&nbsp;</td>
</tr>
<tr>
<td style="width:10%;">&nbsp;</td>
<td style="width:80%; text-align: center; ">
    <asp:UpdatePanel ID="UP2" UpdateMode="Conditional" runat="server"><ContentTemplate>
  
        </ContentTemplate></asp:UpdatePanel>                
</td>
<td style="width:10%;">
    
<%--<ajaxToolkit:ModalPopupExtender ID="mpeApproval"
     backgroundcssclass="ModalBackground"
     runat="server"
     TargetControlId="pnlChangePass"
     cancelcontrolid="btnCancel"
     DropShadow="True"
     PopupControlID="dummy"/>--%>
</td>
</tr>
<tr>
<td style="width:10%;">&nbsp;</td>
<td style="width:80%;">
    
    &nbsp;</td>
<td style="width:10%;">
    
    &nbsp;</td>
</tr>
</table>
   
<br />
    <%--    <CR:CrystalReportViewer ID="CrystalReportViewer1" runat="server" 
    AutoDataBind="true" />--%>
</div>
    </form>
    
    <div id="divBackground" style="position: fixed; z-index: 999; height: 100%; width: 100%;
        top: 0; left:0; background-color: Black; filter: alpha(opacity=60); opacity: 0.6; -moz-opacity: 0.8;-webkit-opacity: 0.8;display:none">
    </div>

</body>
</html>
