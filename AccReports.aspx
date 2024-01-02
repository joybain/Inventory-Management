<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="AccReports.aspx.cs" Inherits="Reports" Title="Report Selection Form"  Theme="Themes"  MaintainScrollPositionOnPostback="true"%>
<%--<%@ Register Assembly="CrystalDecisions.Web, Version=13.0.2000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304"
    Namespace="CrystalDecisions.Web" TagPrefix="CR" %>--%>
    
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">  

<script src='<%= ResolveUrl("~/Scripts/valideDate.js") %>' type="text/javascript"></script>
<script language="javascript" type="text/javascript" >
    function OpenWindow() {
        var rept = document.getElementById('<%=ddlRepType.ClientID%>');        
        var reptype = rept.options[rept.selectedIndex].value;
        var StartDt = document.getElementById('<%=txtStartDt.ClientID%>').value;
        var EndDt = document.getElementById('<%=txtEndDt.ClientID%>').value;
        var repl = document.getElementById('<%=ddlRepLvl.ClientID%>'); 
        var RepLvl = repl.options[repl.selectedIndex].value;
        var segl = document.getElementById('<%=ddlSegLvl.ClientID%>'); 
        var SegLvl = segl.options[segl.selectedIndex].value;
        var vcht = document.getElementById('<%=ddlVchType.ClientID%>'); 
        var VchType = vcht.options[vcht.selectedIndex].value;
        var RptId = document.getElementById('<%=txtRptSysId.ClientID%>').value;
        var NotesNo = document.getElementById('<%=txtNotesNo.ClientID%>').value;
        var coa = document.getElementById('<%=dgSeg.ClientID%>');
        var Desc = document.getElementById('<%=txtdes.ClientID%>').value;
        var COACode = document.getElementById('<%=TextBox1.ClientID%>').value;
        //        alert(Desc);
        
        var rawcoa;
        for (var i = 0; i < coa.rows.length; i++) {
            if (coa.rows[i].cells[1].innerHTML != 'undefined') {
                rawcoa = rawcoa + coa.rows[i].cells[1].innerHTML + '-';
            }
        }
        var glcoa = rawcoa.replace('undefined','');
        var GlCoaCode = glcoa.substr(0,glcoa.length-1);
        if (reptype == 'B' | reptype == 'C' | reptype == 'I' | reptype == 'B1') {
            var testwindow1 = window.open('rptBalSheet.aspx?reptype='+reptype+'&glcoa='+GlCoaCode+'&replvl='+RepLvl+'&seglvl='+SegLvl+'&vchtyp='+VchType+'&startdt='+StartDt+'&enddt='+EndDt+'&rptsysid='+RptId+'&notes='+NotesNo+'');
        }
        else if (reptype == 'B1' | reptype=='C1' | reptype == 'I1') {
            var testwindow2 = window.open('rptBalSheet.aspx?reptype='+reptype+'&glcoa='+GlCoaCode+'&replvl='+RepLvl+'&seglvl='+SegLvl+'&vchtyp='+VchType+'&startdt='+StartDt+'&enddt='+EndDt+'&rptsysid='+RptId+'&notes=All');
        } 
        else if (reptype == '7') {
            var testwindow3 = window.open('rptBalSheet.aspx?reptype='+reptype+'&glcoa='+GlCoaCode+'&replvl='+RepLvl+'&seglvl='+SegLvl+'&vchtyp='+VchType+'&startdt='+StartDt+'&enddt='+EndDt+'&rptsysid='+RptId+'&notes=');
        } 
        else if (reptype == '3' | reptype=='4' | reptype == '5') {
            var testwindow4 = window.open('rptLedgerStat.aspx?reptype='+reptype+'&glcoa='+GlCoaCode+'&replvl='+RepLvl+'&seglvl='+SegLvl+'&vchtyp='+VchType+'&startdt='+StartDt+'&enddt='+EndDt+'&rptsysid='+RptId+'&notes='+ Desc.replace('&'," And "));
        }
        else if (reptype == 'DB' ) {
            //            var testwindow5 = window.open('rptDayBook.aspx?reptype=' + reptype + '&glcoa=' + GlCoaCode + '&replvl=' + RepLvl + '&seglvl=' + SegLvl + '&vchtyp=' + VchType + '&startdt=' + StartDt + '&enddt=' + EndDt + '&rptsysid=' + RptId + '&notes=');
            var testwindow5 = window.open('Changed_rptDayBook.aspx?reptype=' + reptype + '&glcoa=' + GlCoaCode + '&replvl=' + RepLvl + '&seglvl=' + SegLvl + '&vchtyp=' + VchType + '&startdt=' + StartDt + '&enddt=' + EndDt + '&rptsysid=' + RptId + '&notes=');
        }
        else if (reptype == 'DBS') {
            var testwindow7 = window.open('rptDayBookSum.aspx?reptype=' + reptype + '&glcoa=' + GlCoaCode + '&replvl=' + RepLvl + '&seglvl=' + SegLvl + '&vchtyp=' + VchType + '&startdt=' + StartDt + '&enddt=' + EndDt + '&rptsysid=' + RptId + '&notes=');
        }
        else if (reptype == 'CSL') {
//            var testwindow6 = window.open('rptCoa.aspx?reptype=' + reptype + '&glcoa=' + GlCoaCode + '&replvl=' + RepLvl + '&seglvl=' + SegLvl + '&vchtyp=' + VchType + '&startdt=' + StartDt + '&enddt=' + EndDt + '&rptsysid=' + RptId + '&notes=');
        }
        else if (reptype == '1COA') {
            var testwindow6 = window.open('rptCoa.aspx?reptype=' + reptype + '&glcoa=' + GlCoaCode + '&replvl=' + RepLvl + '&seglvl=' + SegLvl + '&vchtyp=' + VchType + '&startdt=' + StartDt + '&enddt=' + EndDt + '&rptsysid=' + RptId + '&notes=');
        }
        else if (reptype == 'SCB') {
            var testwindow7 = window.open('rptScheduleOfCashatBank.aspx?reptype=' + reptype + '&glcoa=' + GlCoaCode + '&replvl=' + RepLvl + '&seglvl=' + SegLvl + '&vchtyp=' + VchType + '&startdt=' + StartDt + '&enddt=' + EndDt + '&rptsysid=' + RptId + '&notes=' + Desc);
        }
        else if (reptype == 'TB') {
            var testwindow7 = window.open('NewTrialBlance.aspx?reptype=' + reptype + '&glcoa=' + GlCoaCode + '&replvl=' + RepLvl + '&seglvl=' + SegLvl + '&vchtyp=' + VchType + '&startdt=' + StartDt + '&enddt=' + EndDt + '&rptsysid=' + RptId + '&notes=' + Desc);
        }
        else if (reptype == 'IAES') {
            var testwindow7 = window.open('rptIncomeAndExpance.aspx?reptype=' + reptype + '&glcoa=' + GlCoaCode + '&replvl=' + RepLvl + '&seglvl=' + SegLvl + '&vchtyp=' + COACode + '&startdt=' + StartDt + '&enddt=' + EndDt + '&rptsysid=' + RptId + '&notes=' + Desc);
        }
        else if (reptype == 'BCS') {
            var testwindow7 = window.open('rptCashAndBankStatement.aspx?reptype=' + reptype + '&glcoa=' + GlCoaCode + '&replvl=' + RepLvl + '&seglvl=' + SegLvl + '&vchtyp=' + COACode + '&startdt=' + StartDt + '&enddt=' + EndDt + '&rptsysid=' + RptId + '&notes=' + Desc);
        }
        else if (reptype == 'RP') {
            var testwindow7 = window.open('rptReceivedAndPayment.aspx?reptype=' + reptype + '&glcoa=' + GlCoaCode + '&replvl=' + RepLvl + '&seglvl=' + SegLvl + '&vchtyp=' + COACode + '&startdt=' + StartDt + '&enddt=' + EndDt + '&rptsysid=' + RptId + '&notes=' + Desc);
        }
        else if (reptype == '8') {
            var testwindow7 = window.open('rptReveivedAndPaymentNew.aspx?reptype=' + reptype + '&glcoa=' + GlCoaCode + '&replvl=' + RepLvl + '&seglvl=' + SegLvl + '&vchtyp=' + COACode + '&startdt=' + StartDt + '&enddt=' + EndDt + '&rptsysid=' + RptId + '&notes=' + Desc);
        } 
    }
    </script>
<script type="text/javascript">

var prm = Sys.WebForms.PageRequestManager.getInstance();
prm.add_beginRequest(beginRequest);

function beginRequest()
{

prm._scrollPosition = null;
}

</script>
    <%--  <asp:ListItem Text="Cash Flow Statement Notes" Value="C1"></asp:ListItem>--%>
<div id="frmMainDiv" style="background-color:White; width:100%; height:auto !important; ">
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
<div style="height: 19px"></div>
<div>
    <table style="width: 100%">
        <tr>
            <td align="left" colspan="3">
                <asp:Label ID="Label10" runat="server" style=" box-shadow:5px 1px 1px #888888; background:linear-gradient(#DDD 2%,#FFF,#FFF)" 
                    Font-Bold="True" Font-Size="X-Large" Text="Accounts Report"
                    Height="40px"></asp:Label>
            </td>
        </tr>
        <tr>
            <td align="left" colspan="3" style="height: 7px">
            </td>
        </tr>
        <tr>
            <td style="width:37%" valign="top">
            
                  <div  style="width:100%; background:linear-gradient(#DDD 2%,#FFF,#FFF)">
                <fieldset style=" text-align:left; vertical-align: top; border: solid 1px #8BB381;"><legend style="color: maroon;"> <b>
        Report Chart Of Account </b></legend>
<asp:UpdatePanel ID="UpdatePanelTree" runat="server" UpdateMode="Conditional">
<ContentTemplate>
<asp:Panel ID="pnlTreeView" runat="server" Width="450px" ScrollBars="Auto" 
        Height="429px" HorizontalAlign="Left">
   <asp:TreeView ID="TreeView1" runat="server" AutoGenerateDataBindings="False" Width="100%"
             onselectednodechanged="TreeView1_SelectedNodeChanged" ImageSet="Msdn"
        ForeColor="Blue" ParentNodeStyle-ForeColor="Green" Font-Bold="True" 
        Font-Size="Medium" EnableTheming="True">           
            <ParentNodeStyle ForeColor="Green" />
            <SelectedNodeStyle Font-Size="Medium" Font-Underline="False" HorizontalPadding="3px" 
                VerticalPadding="2px" BackColor="White" BorderColor="#888888" 
                BorderStyle="Solid" BorderWidth="1px" />            
            <NodeStyle Font-Size="8pt" ForeColor="Black" HorizontalPadding="2px" 
                NodeSpacing="2px" VerticalPadding="3px" Font-Names="Verdana" />
   </asp:TreeView>
</asp:Panel>
</ContentTemplate>
</asp:UpdatePanel></fieldset>
</div>
            </td>
            <td style="width:3%" align="center">
                <img alt="" height="100%" src="img/box_bottom_hori.gif" width="2px" /></td>
            <td style="width:45%" valign="top">
            
                 <div style="width:100%;background:linear-gradient(#DDD 2%,#FFF,#FFF)">
             <fieldset style=" text-align:left; vertical-align: top; border: solid 1px #8BB381;"><legend style="color: maroon;"> <b>
        Report Selection Form</b></legend>
            <asp:UpdatePanel ID="UpdatePanelReport" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                <table style="width: 100%">
                    <tr>
                        <td style="width:25%; height: 32px;" >
                            <asp:Label ID="lblRepType" runat="server" Font-Size="8pt" Font-Bold="False" 
                                style="font-weight: 700">Report Type</asp:Label>
                        </td>
                        <td colspan="4" style="height: 32px">
    <asp:DropDownList SkinID="ddlPlain" ID="ddlRepType" runat="server" Width="88%"  
            AutoPostBack="False" Font-Size="8pt" 
            onselectedindexchanged="ddlRepType_SelectedIndexChanged">
    <asp:ListItem Text="------------------Select Report-------------------" Value=""></asp:ListItem>
    <asp:ListItem Value="5">Ledger Statement</asp:ListItem>
    <asp:ListItem Value="3">Cash Book</asp:ListItem>
    <asp:ListItem Value="4">Bank Book</asp:ListItem>
    <asp:ListItem Value="DB">Day Book</asp:ListItem>
    <asp:ListItem Value="TB">Root/Leaf Summery</asp:ListItem>
    <asp:ListItem Value="I">Income Statement</asp:ListItem>   
    <asp:ListItem Value="B1">Income Statement for shopping center</asp:ListItem>    
    <asp:ListItem Value="7">Trial Balance</asp:ListItem>
    <asp:ListItem Value="B">Balance Sheet</asp:ListItem>
    <asp:ListItem Value="C">Cash Flow Statement</asp:ListItem>
 <%--   <asp:ListItem Value="RP">Receipt & Payment Account</asp:ListItem>--%>
     <asp:ListItem Value="8">Receipts & Payments</asp:ListItem>
    <%--<asp:ListItem Text="Day Book Summary" Value="DBS"></asp:ListItem>   --%> 
   <%-- <asp:ListItem Text="Trial Balance Unposted" Value="6"></asp:ListItem>--%>
   <%-- <asp:ListItem Text="Income Statement Notes" Value="I1"></asp:ListItem>--%>
  <%--  <asp:ListItem Text="Balance Sheet Notes" Value="B1"></asp:ListItem>--%>
  <%--  <asp:ListItem Text="Cash Flow Statement Notes" Value="C1"></asp:ListItem>--%>
       <%-- <asp:ListItem Value="SCB">Schedule Report</asp:ListItem>--%>  
       <%--<asp:ListItem Value="IAES">Income and Expenses Statement</asp:ListItem>--%>
      <%--  <asp:ListItem Value="BCS">Cash &amp; Bank Summery</asp:ListItem>--%>
    </asp:DropDownList>
    </td>
                    </tr>
                    <tr>
                        <td style="font-weight: 700; width: 25%;">
                            <asp:Label ID="lblCountry" runat="server" Text="Select Country"></asp:Label>
                        </td>
                        <td colspan="4" >
                            <table style="width: 100%">
                                <tr>
                                    <td style="width: 4%">
                                        <asp:DropDownList ID="ddlUserType" runat="server" AutoPostBack="True" 
                                            Font-Size="8pt" onselectedindexchanged="ddlUserType_SelectedIndexChanged" 
                                            SkinID="ddlPlain" Width="100%">
                                            <asp:ListItem Value=""></asp:ListItem>
                                            <asp:ListItem Value="1">BD</asp:ListItem>
                                            <asp:ListItem Value="2">PH</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td style="width: 3%" align="right">
                                        <asp:Label ID="lblBranch" runat="server" Text="Branch" style="font-weight: 700"></asp:Label>
                                    </td>
                                    <td style="width: 10%">
                                        <asp:DropDownList ID="ddlBranchID" runat="server" AutoPostBack="True" 
                                            Font-Size="8pt" onselectedindexchanged="ddlBranchID_SelectedIndexChanged" 
                                            SkinID="ddlPlain" Width="80%">
                                            <asp:ListItem Value=""></asp:ListItem>
                                            <asp:ListItem Value="1">Bangladesh</asp:ListItem>
                                            <asp:ListItem Value="2">Manila</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td style="width:25%; height: 30px;" >
            <asp:Label ID="lblRepType0" runat="server" Font-Size="8pt" Font-Bold="False" 
                                style="font-weight: 700">Natural Accounts</asp:Label>
                        </td>
                        <td colspan="4" style="height: 30px">
            <asp:TextBox ID="TextBox1" runat="server" AutoPostBack="True" placeHolder="Search Natural Accounts"
                ontextchanged="TextBox1_TextChanged" Width="87%" Height="22px"></asp:TextBox>
            <ajaxToolkit:AutoCompleteExtender ID="autoComplete" runat="server" 
                CompletionInterval="20" CompletionSetCount="12" EnableCaching="true" 
                MinimumPrefixLength="1" ServiceMethod="GetCompletionList" 
                ServicePath="AutoComplete.asmx" TargetControlID="TextBox1" />
                </td>
                    </tr>
                    <tr>
                        <td style="width:25%;" >
                            &nbsp;</td>
                        <td style="width:24%;">
                            &nbsp;</td>
                        <td style="width:2%;">
                            &nbsp;</td>
                        <td style="width:21%;">
                            &nbsp;</td>
                        <td style="width:20%;">
                            &nbsp;</td>
                    </tr>
                    <tr>
                        <td colspan="5" >
            <asp:GridView ID="dgSeg" runat="server" AllowPaging="True" 
                AlternatingRowStyle-CssClass="alt" AutoGenerateColumns="false" 
                BackColor="White" BorderColor="LightGray" BorderStyle="Solid" BorderWidth="1px" 
                CellPadding="2" CellSpacing="0" CssClass="mGrid" Font-Size="8pt" 
                PagerStyle-CssClass="pgr" PageSize="3" RowStyle-Height="25px" 
                ShowHeader="false" Width="100%">
                <Columns>
                    <asp:BoundField DataField="lvl_desc" ItemStyle-BackColor="LightGray" 
                        ItemStyle-Height="21px" ItemStyle-HorizontalAlign="Left" 
                        ItemStyle-Width="120px" />
                    <asp:BoundField DataField="seg_code" ItemStyle-HorizontalAlign="Left" 
                        ItemStyle-Width="40px" />
                    <asp:BoundField DataField="seg_desc" ItemStyle-HorizontalAlign="Left" 
                        ItemStyle-Width="140px" />
                </Columns>
            </asp:GridView>
                        </td>
                    </tr>
                    <tr>
                        <td style="width:25%;" >
            <asp:Label ID="Label4" runat="server" Font-Size="8pt" Font-Bold="True">Report Level</asp:Label>
                        </td>
                        <td style="width:24%;">
	<asp:DropDownList SkinID="ddlPlain" ID="ddlRepLvl" runat="server" Width="105px"  
            AutoPostBack="False" Font-Size="8">    
    <asp:ListItem Text="" Value=""></asp:ListItem>
    <asp:ListItem Text="Top Level" Value="1"></asp:ListItem>
    <asp:ListItem Text="Second Level" Value="2"></asp:ListItem>
    <asp:ListItem Text="Third Level" Value="3"></asp:ListItem>
    <asp:ListItem Text="Fourth Level" Value="4"></asp:ListItem>
    <asp:ListItem Text="Fifth Level" Value="5"></asp:ListItem>
    <asp:ListItem Text="Last Level" Value="6"></asp:ListItem>
    </asp:DropDownList></td>
                        <td style="width:2%;">
                            &nbsp;</td>
                        <td style="width:21%;">
	    <asp:Label ID="Label11" runat="server" Font-Size="8pt" Font-Bold="True">Segment Level</asp:Label>
                        </td>
                        <td style="width:20%;">
        <asp:DropDownList ID="ddlSegLvl" runat="server" AutoPostBack="False" 
            Font-Size="8" SkinID="ddlPlain" Width="105px">
        </asp:DropDownList>
	                    </td>
                    </tr>
                    <tr>
                        <td style="width:25%;" >
            <asp:Label ID="Label6" runat="server" Font-Size="8pt" Font-Bold="True">Voucher Type</asp:Label>
                        </td>
                        <td style="width:24%;">
	<asp:DropDownList SkinID="ddlPlain" ID="ddlVchType" runat="server" Width="105px"  
            AutoPostBack="False" Font-Size="8">  
    <asp:ListItem></asp:ListItem>  
    <asp:ListItem Text="Debit Voucher" Value="01"></asp:ListItem>
    <asp:ListItem Text="Credit Voucher" Value="02"></asp:ListItem>
    <asp:ListItem Text="Contra Voucher" Value="03"></asp:ListItem>
    <asp:ListItem Text="Journal Voucher" Value="04"></asp:ListItem>
    </asp:DropDownList>
                        </td>
                        <td style="width:2%;">
                            &nbsp;</td>
                        <td style="width:21%;">
	    <asp:Label ID="Label12" runat="server" Font-Size="8pt" Font-Bold="True">Report Sys. No</asp:Label>
                        </td>
                        <td style="width:20%;">
        <asp:TextBox ID="txtRptSysId" runat="server" Font-Size="8" MaxLength="12" 
            SkinID="tbPlain" Width="100px"></asp:TextBox>
	                    </td>
                    </tr>
                    <tr>
                        <td style="width:25%;" >
            <asp:Label ID="Label7" runat="server" Font-Size="8pt" Font-Bold="True" 
                ForeColor="#CC3300">Start Date</asp:Label>
                        </td>
                        <td style="width:24%;">
	<asp:TextBox SkinID="tbPlain" ID="txtStartDt" runat="server" Font-Size="8" 
            Width="100px" MaxLength="11"></asp:TextBox>
    <ajaxtoolkit:calendarextender runat="server" ID="Calendarextender1" 
            TargetControlID="txtStartDt" Format="dd/MM/yyyy"/>
                        </td>
                        <td style="width:2%;">
                            &nbsp;</td>
                        <td style="width:21%;">
        <asp:Label ID="Label14" runat="server" Font-Bold="True" ForeColor="#CC3300" 
            Text="Upto Date"></asp:Label>
                        </td>
                        <td style="width:20%;">
	<asp:TextBox SkinID="tbPlain" ID="txtEndDt" runat="server" Font-Size="8" 
            Width="100px" Enabled="false" MaxLength="11"></asp:TextBox>
    <ajaxtoolkit:calendarextender runat="server" ID="Calendarextender2" 
            TargetControlID="txtEndDt" Format="dd/MM/yyyy"/>
                        </td>
                    </tr>
                    <tr>
                        <td style="width:25%; height: 26px;" >
            <asp:Label ID="Label13" runat="server" Font-Size="8pt" Font-Bold="True">Notes No</asp:Label>
                        </td>
                        <td style="width:24%; height: 26px;">
	    <asp:TextBox ID="txtNotesNo" runat="server" Font-Size="8" MaxLength="4" 
            SkinID="tbPlain" Width="100px"></asp:TextBox>
                        </td>
                        <td style="width:2%; height: 26px;">
                            </td>
                        <td style="width:21%; height: 26px;">
                            </td>
                        <td style="width:20%; height: 26px;">
                            <asp:TextBox ID="txtdes" runat="server" Enabled="False" Font-Size="8" 
                                 SkinID="tbPlain" style="border:0px;" Width="1px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="width:25%;" >
                            &nbsp;</td>
                        <td style="width:24%;">
                            &nbsp;</td>
                        <td style="width:2%;">
                            &nbsp;</td>
                        <td style="width:21%;">
                            &nbsp;</td>
                        <td style="width:20%;">
                            &nbsp;</td>
                    </tr>
                    <tr>
                        <td colspan="5" >
                            <table style="width:100%;">
                                <tr>
                                    <td style="width:20%;">
                                    </td>
                                    <td align="center" style="width:40%;">
                                        <asp:Button ID="lbRunReport" runat="server" Height="35px" 
                                            onclick="lbRunReport_Click" Text="Run Report" ToolTip="Run Report" 
                                            Width="120px" />
                                    </td>
                                    <td align="center" style="width:40%;">
                                        <asp:Button ID="lbReset" runat="server" Height="35px" onclick="lbReset_Click" 
                                            Text="Reset" ToolTip="Clear" Width="120px" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td style="width:25%;" align="right" >
            <asp:Label ID="Label9" runat="server" Visible="False"></asp:Label>
                        </td>
                        <td align="center" colspan="3">
                            <asp:Label ID="lblTranStatus" runat="server" Font-Size="8" />
                        </td>
                        <td style="width:20%;">
                            &nbsp;</td>
                    </tr>
                    <tr>
                        <td align="right" style="width:25%;">
                            &nbsp;</td>
                        <td align="center" colspan="3">
                            &nbsp;</td>
                        <td style="width:20%;">
                            &nbsp;</td>
                    </tr>
                </table>
                </ContentTemplate>
</asp:UpdatePanel>
</fieldset>
            </div>
            </td>
        </tr>
        <tr>
            <td style="width:37%">
                &nbsp;</td>
            <td style="width:3%">
                &nbsp;</td>
            <td style="width:45%" >
                &nbsp;</td>
        </tr>
        </table>
    </div>
<div style="height: 24px"></div>
<div></div>

 <table>
 <tr>
<td colspan="3" style="width:100%; text-align:center">

 </td></tr>
<tr>
<td colspan="3" style="width:100%; text-align:center">

 </td>
 </tr>
 </table>
 <br />
    <%--    <CR:CrystalReportViewer ID="CrystalReportViewer1" runat="server" 
    AutoDataBind="true" />--%>
</div>
<div id="divBackground" style="position: fixed; z-index: 999; height: 100%; width: 100%;
        top: 0; left:0; background-color: Black; filter: alpha(opacity=60); opacity: 0.6; -moz-opacity: 0.8;-webkit-opacity: 0.8;display:none">
    </div>
</asp:Content>

