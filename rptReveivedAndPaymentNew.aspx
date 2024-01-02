<%@ Page Language="C#" AutoEventWireup="true" CodeFile="rptReveivedAndPaymentNew.aspx.cs" Inherits="rptReveivedAndPaymentNew" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<style type="text/css">
tHead
{
  display : table-header-group;
}
    .style1
    {
        width: 11%;
        height: 4px;
    }
    .style2
    {
        width: 2%;
        height: 4px;
    }
    .style3
    {
        width: 20%;
        height: 4px;
    }
    .style4
    {
        height: 4px;
    }
    .style5
    {
        width: 11%;
        font-weight: 700;
    }
    </style> 
    <title>Accounts Report</title>
    <link href="css/GrideView.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/valideDate.js" type="text/javascript"></script>
    <script src="Scripts/validate12.js" type="text/javascript"></script>
    <script src="Scripts/Timeout.js" type="text/javascript"></script>
</head>
<body onload="javascript: addHeaders()" style="margin-top:0em;">
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
    AddTHEAD('dgBalNotes');
    AddTHEAD('dgBal');
    AddTHEAD('dgAccountBal');
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
<td style="width:20%;">&nbsp;</td>
<td style="width:60%;">

    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
</td>
<td style="width:20%;">&nbsp;</td>
</tr>
<tr>
<td style="width:20%;">&nbsp;</td>
<td style="width:60%;">

    <asp:HiddenField ID="hfSegLvl" runat="server" />
    <asp:HiddenField ID="hfRepLvl" runat="server" />
    </td>
<td style="width:20%;">&nbsp;</td>
</tr>
<tr>
<td style="width:20%;"></td>
<td style="width:60%;">

<table id ="tblOver" style="width:700px; margin-top:0em;" >
<thead>
<tr align="center">
<td style=" width:680px; text-align:center;" align="center">
<table style="width:100%;">
<tr style="width:100%;">
<td style="width:100%;">
<asp:Label ID="lblOrg" runat="server" Visible="True" Text="" Font-Size="14pt" Height="20px" Width="100%"></asp:Label>
</td>
</tr>
<tr style="width:100%;">
<td style="width:100%;">
<asp:Label ID="lblAddress1" runat="server" Visible="True" Text="" Font-Size="8pt" Height="15px" Width="100%"></asp:Label>
</td>
</tr>
<tr style="width:100%;">
<td style="width:100%;">
<asp:Label ID="lblAddress2" runat="server" Visible="True" Text="" Font-Size="8pt" Height="15px" Width="100%"></asp:Label>
</td>
</tr>
<tr style="width:100%;">
<td style="width:100%;">
<asp:Label ID="lblTitle" runat="server" Font-Size="15pt" Width="100%" 
        Font-Bold="True"></asp:Label>
</td>
</tr>
<tr style="width:100%;">
<td style="width:100%;">
<asp:Label ID="lblDate" runat="server" Font-Size="14pt" Height="25px" Width="100%" 
        style="font-weight: 700"></asp:Label>
</td>
</tr>
</table>
</td>
</tr>
</thead>
<tr>
<td>
    <%-- <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional"><ContentTemplate>--%>
 <asp:Panel ID="pnlChangePass" runat="server"  CssClass="modalPopup" 
                            Style="background-color: White; width:500px; height:auto; ">
                            <fieldset style=" text-align:left; vertical-align: top; border: solid 1px #8BB381;line-height:1.5em;"><legend style="color: maroon;"><b>
                                Select Date</b></legend>
                                <table style="width: 100%;">
                                    <tr>
                                        <td align="right" class="style5">
                                            Ledger</td>
                                        <td style="width: 2%;" align="center">
                                            :</td>
                                        <td colspan="4">
                                            <asp:Label ID="lblCoaCode" runat="server"></asp:Label>
                                            -<asp:Label ID="lblCoaName" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="style5" align="right">
                                            <asp:Label ID="Label1" runat="server" Text="From Date"></asp:Label>
                                            </td>
                                        <td align="center" style="width: 2%;">
                                            :</td>
                                        <td style="width: 20%;">
                                            <asp:TextBox ID="txtStartDt" runat="server" Font-Size="8pt" PlaceHolder="dd/MM/yyyy"
                                                SkinID="tbPlain" Width="80%" Font-Bold="True"></asp:TextBox>
                                            <ajaxToolkit:CalendarExtender ID="Calendarextender1" runat="server" 
                                                Format="dd/MM/yyyy" TargetControlID="txtStartDt" />
                                        </td>
                                        <td align="right" style="width: 10%;">
                                            <asp:Label ID="Label2" runat="server" style="font-weight: 700" Text="To Date"></asp:Label>
                                        </td>
                                        <td align="center" style="width: 2%;">
                                            :</td>
                                        <td style="width: 20%;">
                                            <asp:TextBox ID="txtEndDt" runat="server" Font-Size="8pt" PlaceHolder="dd/MM/yyyy"
                                                SkinID="tbPlain" Width="80%" Font-Bold="True"></asp:TextBox>
                                            <ajaxToolkit:CalendarExtender ID="Calendarextender2" runat="server" 
                                                Format="dd/MM/yyyy" TargetControlID="txtEndDt" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="style1">
                                        </td>
                                        <td class="style2">
                                        </td>
                                        <td class="style3">
                                            &nbsp;</td>
                                        <td align="center" class="style4" colspan="3">
                                            &nbsp;</td>
                                    </tr>
                                    <tr>
                                        <td class="style5">
                                            <asp:HiddenField ID="hfRootLeft" runat="server" />
                                        </td>
                                        <td style="width: 2%;">
                                            &nbsp;</td>
                                        <td style="width: 20%;">
                                            &nbsp;</td>
                                        <td align="center" colspan="3">
                                            <table style="width: 100%">
                                                <tr>
                                                    <td style="width: 60%">
                                                        <asp:Button ID="btnShow" runat="server" onclick="btnShow_Click" 
                                                            style="font-weight: 700" Text="Run" Width="90%" Height="35px" />
                                                    </td>
                                                    <td style="width: 40%">
                                                        <asp:Button ID="btnCancel" runat="server" Font-Bold="True" Font-Size="10pt" 
                                                            OnClientClick="HideModalDiv();" Text="Cancel" Width="90%" Height="35px" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </fieldset>
                        </asp:Panel>
    <%--</ContentTemplate></asp:UpdatePanel>--%>
                         <asp:Button ID="Button1" style="display: none;" runat="server" Height="1px" Width="1px" />
                         <ajaxToolkit:ModalPopupExtender ID="ModalPopupExtenderLogin" runat="server" 
                    BackgroundCssClass="modalBackground" DropShadow="true" 
                    PopupControlID="pnlChangePass" TargetControlID="Button1" />
    <%--  </ContentTemplate>
    </asp:UpdatePanel>--%>
</td>
</tr>
<tr>
    <td align="right">
          <table style="width:60%;border: solid 1px #8BB381;">
        <tr>
            <td align="center" >

<asp:LinkButton ID="lbPrint" runat="server" Text="Export Report to PDF" 
        Font-Size="11pt" ForeColor="Blue" onclick="lbPrint_Click" 
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
<td>
<asp:GridView ID="dgAccountBal" runat="server" AutoGenerateColumns="false" 
        Visible="false" CssClass="Grid"
        AllowPaging="true" PageSize="50" Width="100%" BorderWidth="1px" BorderStyle="Solid" 
        CellPadding="2" CellSpacing="0" BorderColor="Black" Font-Size="8pt" 
        Font-Names="Verdana" onrowdatabound="dgAccountBal_RowDataBound" 
        onpageindexchanging="dgAccountBal_PageIndexChanging" 
        onrowcommand="dgAccountBal_RowCommand">
  <Columns> 
  <asp:BoundField ShowHeader="false" ItemStyle-Width="50px" DataField="seg_coa_code"  ItemStyle-HorizontalAlign="Center" ItemStyle-Height="15px"/>  
  <asp:BoundField ShowHeader="false" ItemStyle-Width="350px" DataField="seg_coa_desc"  ItemStyle-HorizontalAlign="Left" /> 
  <asp:BoundField ShowHeader="false" ItemStyle-Width="120px" DataField="p_db_amt" DataFormatString="{0:N3}" ItemStyle-HorizontalAlign="Right"/>  
  <asp:BoundField ShowHeader="false" ItemStyle-Width="120px" DataField="p_cr_amt" DataFormatString="{0:N3}" ItemStyle-HorizontalAlign="Right" /> 
  <asp:BoundField ShowHeader="false" ItemStyle-Width="120px" DataField="u_db_amt" DataFormatString="{0:N3}" ItemStyle-HorizontalAlign="Right"/>  
  <asp:BoundField ShowHeader="false" ItemStyle-Width="120px" DataField="u_cr_amt" DataFormatString="{0:N3}" ItemStyle-HorizontalAlign="Right" />   
  <asp:BoundField ShowHeader="false" ItemStyle-Width="120px" DataField="Status" ItemStyle-HorizontalAlign="Right" />
  <asp:BoundField ShowHeader="false" ItemStyle-Width="120px" DataField="rootleaf" DataFormatString="{0:N3}" ItemStyle-HorizontalAlign="Right" /> 
  <asp:TemplateField ItemStyle-Width="25px" HeaderText="****">
        <ItemTemplate>
            <asp:LinkButton ID="lblSelect" Visible="True"  runat="server" 
                CommandName="View" Text="....." Font-Bold="True" Font-Size="10pt" 
                Font-Underline="False"></asp:LinkButton>
        </ItemTemplate>
        <ItemStyle HorizontalAlign="Center" Width="75px" />
      </asp:TemplateField> 
  </Columns>
                        <RowStyle BackColor="White" />
                        <PagerStyle HorizontalAlign="Center" />
                        <AlternatingRowStyle BackColor="" />
</asp:GridView>

</td>
</tr>
</table>
<table style="width:100%;">
    <%--<tr style="width:100%;">
<td style="width:50%; vertical-align:top;" align="left">
<asp:LinkButton ID="btnPrint" runat="server" Text="Print" Font-Size="X-Small" 
        OnClientClick='javascript:PrintContent("tblOver")' ForeColor="Blue"></asp:LinkButton>
</td>
<td style="width:50%; vertical-align:top;" align="right">       
<asp:LinkButton ID="lbExp" runat="server" Text="Export to Excel" Font-Size="X-Small" 
        ForeColor="Blue"></asp:LinkButton>        
</td>
</tr>--%>

<tr style="width:100%;">
<td style="width:100%;">
    &nbsp;</td>
</tr>
</table>
</td>
<td style="width:20%;"></td>
</tr>
</table>
</div>
    </form>
    <div id="divBackground" style="position: fixed; z-index: 999; height: 100%; width: 100%;
        top: 0; left:0; background-color: Black; filter: alpha(opacity=60); opacity: 0.6; -moz-opacity: 0.8;-webkit-opacity: 0.8;display:none">
    </div>
</body>

</html>
