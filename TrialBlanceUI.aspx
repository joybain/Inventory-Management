<%--<%@ Page Language="C#" AutoEventWireup="true" CodeFile="TrialBlanceUI.aspx.cs" Inherits="TrialBlanceUI" %>--%>

 <%@ Page Language="C#" AutoEventWireup="true" CodeFile="TrialBlanceUI.aspx.cs" Inherits="TrialBlanceUI" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <style type="text/css">
tHead
{
  display : table-header-group;
}
</style> 
    <title>Report</title>
    <link href="css/GrideView.css" rel="stylesheet" type="text/css" />
</head>
<body onload="javascript: addHeaders() " style="margin-top:0em;">
<form id="form1" runat="server"> 
   
<script language="JavaScript" type="text/javascript">
    function PrintContent(elementId) {
        var printContent = document.getElementById(elementId);
        var windowUrl = 'about:blank';
        var uniqueName = new Date();
        var windowName = 'Print' + uniqueName.getTime();
        document.getElementById("<%=lblOrg.ClientID %>").style.textAlign = "center";
        document.getElementById("<%=lblAddress1.ClientID %>").style.textAlign = "center";
        document.getElementById("<%=lblAddress2.ClientID %>").style.textAlign = "center";
        document.getElementById("<%=lblTitle.ClientID %>").style.textAlign = "center";
        var printWindow = window.open(windowUrl, windowName, 'width=800,height=600,top=0,left=10,toolbars=no,scrollbars=no,status=no,resizable=no');
        printWindow.document.write(printContent.innerHTML);
        printWindow.document.close();
        printWindow.focus();
        //printWindow.print();
        //printWindow.close();
    }
    function getNewUrl(newVal) {
        var testwindow1 = window.open(location.href + newVal);
    }
    function AddTHEAD(tableName) {
        var table = document.getElementById(tableName);
        if (table != null) {
            var head = document.createElement("THEAD");
            head.style.display = "table-header-group";
            head.appendChild(table.rows[0]);
            table.insertBefore(head, table.childNodes[0]);
        }
    }
    function addHeaders() {
        AddTHEAD('dgLedger');
    }
</script>
<div style="min-height:560px; height:auto !important; text-align:center; width:100%; margin-top:0em;">
<table style="width:100%;">
<tr>
<td style="width:10%;"></td>
<td style="width:80%;">

<table id ="tblOver" style="width:900px; margin-top:0em;" >
<thead>
<tr align="center">
<td style=" width:900px; text-align:center;" align="center">
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
<asp:Label ID="lblTitle" runat="server" Visible="True" Text="" Font-Size="8pt" Height="15px" Width="100%"></asp:Label>
</td>
</tr>
</table>
</td>
</tr>
</thead>
<tr>
<td >

&nbsp;</td>
</tr>
</table>
</td>
<td style="width:10%;"></td>
</tr>
<tr>
<td colspan="3">

<asp:GridView ID="dgLedger" runat="server" AutoGenerateColumns="False" CssClass="Grid"
        Visible="False" style="text-align:center; margin-top: 0px;" 
        AllowPaging="True" PageSize="50" Width="100%" BorderWidth="1px" BorderStyle="Solid"
        CellPadding="2" BorderColor="Black" Font-Size="8pt"
        Font-Names="Verdana" 
        onpageindexchanging="dgLedger_PageIndexChanging">
  <Columns> 
  <asp:BoundField HeaderText="Account"  
          ItemStyle-HorizontalAlign="Center" ItemStyle-Height="15px" 
          DataField="coa_desc">  
      <ItemStyle Height="15px" HorizontalAlign="center" Width="350px"/>
      </asp:BoundField>
      <asp:BoundField DataField="Openin_Cr" HeaderText="Opening Blance Cr">

      <ItemStyle HorizontalAlign="Right" Width="150px" />
      </asp:BoundField>

      <asp:BoundField DataField="Openin_Dr" HeaderText="Opening Blance Dr" >

      <ItemStyle HorizontalAlign="Right" Width="150px" />
      </asp:BoundField>

  <asp:BoundField HeaderText="Period Amount Cr" ItemStyle-Width="150px" 
          DataField="Period_amount_Cr"  ItemStyle-HorizontalAlign="Left" > 
      <ItemStyle HorizontalAlign="Right" Width="150px" />
      </asp:BoundField>
      <asp:BoundField HeaderText="Period Amount Dr" DataField="Period_amount_Dr" />
      <asp:BoundField HeaderText="Closing Cr" DataField="Closing_Cr" />
      <asp:BoundField HeaderText="Closing Dr" DataField="Closing_Dr" />
  </Columns>
                        <RowStyle BackColor="White" />
                        <PagerStyle HorizontalAlign="Center" />
                        <AlternatingRowStyle BackColor="" />
</asp:GridView>

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
<td style="width:80%;">

<asp:LinkButton ID="lbExp" runat="server" Text="Export Report to PDF" 
        Font-Size="X-Small" ForeColor="Blue" Width="150px" onclick="lbExp_Click"></asp:LinkButton>
</td>
<td style="width:10%;">&nbsp;</td>
</tr>
</table>
<br />
<%--    <CR:CrystalReportViewer ID="CrystalReportViewer1" runat="server" 
    AutoDataBind="true" />--%>
</div>
    </form>
</body>
</html>
