<%@ Page Title="" Language="C#" MasterPageFile="~/BranchMasterPage.master" AutoEventWireup="true" CodeFile="BranchDownlodeInfoHeadOffice.aspx.cs" Inherits="BranchDownlodeInfoHeadOffice" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">

     <script src='<%= ResolveUrl("~/Scripts/valideDate.js") %>' type="text/javascript"></script>
    <div id="frmMainDiv" style="width:98.5%; background-color:transparent; padding:10px; height: auto !important;">

        <table style="width: 100%; padding-left: 5px; background-color:white;">
            <tr>
                <td style="width:20%;"></td>
                <td style="width: 60%; vertical-align: top;" align="center">
                    <table style="border: solid 1px gray; height: 100%; vertical-align: top;">
                        <tr>
                            <td colspan="4" style="width: 100%; height:30px;" align="center">
                                Download Head Office Data</td>
                        </tr>
                        <tr>
                            <td align="center" colspan="4">
                                <asp:Label ID="Errorlbl" runat="server" Text="Label" Visible="False"></asp:Label>
                                <asp:TextBox ID="txtbrow" runat="server" style="text-align:center;" 
                                    Width="269px" Visible="False">D</asp:TextBox>
                            &nbsp;<asp:LinkButton ID="LinkButton1" runat="server" onclick="LinkButton1_Click" 
                                    Visible="False">LinkButton</asp:LinkButton>
                            </td>
                        </tr>
                        
                        <tr>
                            <td style="width: 13%; height: 27px; font-size:10pt;" align="right"> 
                                &nbsp;</td>
	<td style="width: 27%; height: 27px;" align="left">
        &nbsp;</td>  
    <td style="width: 13%; height: 27px; font-size:10pt;" align="right"> 
        &nbsp;</td>
	<td style="width: 27%; height: 27px;" align="left">
        &nbsp;</td>
                        </tr> 
                        <tr>
                            <td style="height: 27px; font-size:8pt;" align="center" colspan="4">
                                <asp:Button ID="Button1" runat="server" Height="46px" onclick="Button1_Click" 
                                    Text="Downlode Data" Width="227px" />
                            </td>
                        </tr>
                        <tr>	
	<td style="width: 13%; height: 27px; font-size:8pt;" align="right">
        &nbsp;</td>
	<td style="width: 18%; height: 27px;" align="left">
        &nbsp;</td>        
     
  <td style="width: 13%; height: 27px; font-size:8pt;" align="right">
      &nbsp;</td>
	<td style="width: 18%; height: 27px;" align="left">
	    &nbsp;</td>     
</tr>                       
                       
                        <tr>
                            <td colspan="2" align="center">
                                &nbsp;</td>
                            <td style="width: 5%;"></td>
                            <td align="center">
                                &nbsp;</td>
                        </tr>   
                        
                    </table>
                </td>
                <td style="width:20%;"></td>
            </tr>
        </table>
        <!--
        <div class="loading" align="center">
            Loading. Please wait.<br />
            <br />
            <img src="img/loading.gif" alt="" />
        </div>
        -->
        <table>
            <tr>
                <td colspan="3" style="width: 100%; text-align: center"></td>
            </tr>
            <tr>
                <td colspan="3" style="width: 100%; text-align: center"></td>
            </tr>
        </table>

    </div>

</asp:Content>
