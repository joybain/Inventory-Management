<%@ Page Title="Day Closing SaleSummary.-SDL" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="frmDayClosingSaleSummary.aspx.cs" Inherits="frmDayClosingSaleSummary" %>

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
<div id="frmMainDiv" style="width:100%; background-color:White;"> 
<table id="pageFooterWrapper">
         <tr>
              <td style="vertical-align:middle;" align="center">
                &nbsp;</td> 
	        <td style="vertical-align:middle; height:100%;" align="center">
                <asp:Button ID="btnPrint" runat="server" ToolTip="Print" TabIndex="904"   
                    Text="Print" Width="110px" Height="35px" BorderStyle="Outset" 
                    BorderWidth="1px" onclick="btnPrint_Click"  />
             </td>
	
	        <td style="vertical-align:middle;" align="center">
                &nbsp;</td>
                <td style="vertical-align:middle;" align="center">
                <asp:Button ID="btnDelete" 
                    runat="server" ToolTip="delete Record" TabIndex="901" Text="Delete"  onclientclick="javascript:return window.confirm('are u really want to delete this record')"         
                     Width="110px" Height="35px" BorderStyle="Outset" BorderWidth="1px" onclick="btnDelete_Click" 
                     /></td>
                    <td style="vertical-align:middle;" align="center">
                <asp:Button ID="BtnSave" 
                    runat="server" ToolTip="Save or Update Record" TabIndex="901" Text="Save"  onclientclick="javascript:return window.confirm('are u really want to received this record')"         
                     Width="110px" Height="35px" BorderStyle="Outset" BorderWidth="1px" 
                    onclick="BtnSave_Click" /></td>
	        <td style="vertical-align:middle;" align="center">
                &nbsp;</td> 
            <td style="vertical-align:middle;" align="center">
                &nbsp;</td>  
               <td style="vertical-align:middle;" align="center">
                <asp:Button ID="BtnReset" runat="server" ToolTip="Clear Form" TabIndex="904"   
                    Text="Clear" Width="110px" Height="35px" BorderStyle="Outset" 
                    BorderWidth="1px" onclick="BtnReset_Click" />
            </td>    
             <td style="vertical-align:middle;" align="center">
                &nbsp;</td>     
	        </tr>		
	  </table> 

    <table style="width: 100%">
        <tr>
            <td style="width: 5%">
                &nbsp;</td>
            <td style="width: 90%">
                &nbsp;</td>
            <td style="width: 5%">
                &nbsp;</td>
        </tr>
        <tr>
            <td style="width: 5%">
                &nbsp;</td>
            <td style="width: 90%">
                <fieldset style="vertical-align: top; border: solid 1px #8BB381; text-align:left;">
                            <legend style="color: maroon; font-weight: 700;"><b> Search Option</b></legend>
                            <asp:Panel ID="pnl" runat="server">
                <table  style="width: 100%">
                    <tr>
                        <td align="right" style="width: 8%; height: 29px; font-weight: 700;">
                            Search&nbsp; Date</td>
                        <td style="width: 1%; height: 29px; font-weight: 700;" align="center">
                            :</td>
                        <td style="width: 10%; height: 29px;" align="left" valign="middle">
                            <asp:TextBox ID="txtReceiveDate" CssClass="txtVisibleAlign" runat="server" Width="95%" Height="20px" placeHolder="dd/MM/yyyy"
                                BorderStyle="Ridge"></asp:TextBox>
                            
                             <ajaxtoolkit:calendarextender runat="server" ID="Calendarextender1" 
            TargetControlID="txtReceiveDate" Format="dd/MM/yyyy"/>
                        </td>
                        <td style="width: 13%; height: 29px;" valign="middle">
                            <asp:LinkButton ID="lbSearch" runat="server" Font-Bold="True" Height="22px" Style="text-align: center"
                                Width="120px" BorderColor="#009900" BorderStyle="Solid" Font-Size="13pt" 
                                onclick="ibSearch_Click" BackColor="#CCCCCC" ForeColor="Blue">Search</asp:LinkButton>
                        </td>
                        <td align="right" style="width: 5%; height: 29px;">
                            &nbsp;</td>
                    </tr>
                </table></asp:Panel></fieldset></td>
            <td style="width: 5%">
                &nbsp;</td>
        </tr>
        <tr>
            <td style="width: 5%">
                &nbsp;</td>
            <td style="width: 90%">

                <div id="dg" runat="server">
                    <asp:Panel ID="PnlMst" runat="server">
                    
                    <fieldset style="vertical-align: top; border: solid 1px #8BB381; text-align:left;">
                        <legend style="color: maroon;"><b>Day Closing Items History For Branch</b></legend>
                        <asp:HiddenField ID="hfBranchCoa" runat="server" />
                        <asp:GridView ID="dgHistory" runat="server" AutoGenerateColumns="False" 
                            CssClass="mGrid"  
                            PageSize="15" 
                            style="text-align:center;" Width="100%" 
                            onselectedindexchanged="dgHistory_SelectedIndexChanged" 
                            onrowdatabound="dgHistory_RowDataBound">
                            <Columns>
                                <asp:CommandField ShowSelectButton="True">
                                <ItemStyle HorizontalAlign="Center" Width="2%" Font-Bold="True" 
                                    Font-Size="10pt" />
                                </asp:CommandField>
                                <asp:BoundField DataField="ID" HeaderText="ID">
                                <ItemStyle HorizontalAlign="Center" Width="2%" />
                                </asp:BoundField>
                                <asp:BoundField DataField="ServerUpload" HeaderText="MstID">
                                <ItemStyle HorizontalAlign="Center" Width="5%" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="Branch Name" DataField="BranchName">
                                <ItemStyle HorizontalAlign="Left" Width="10%" />
                                </asp:BoundField>
                                <asp:BoundField DataField="ClosingDate" HeaderText="Day Closing Date">
                                <ItemStyle HorizontalAlign="Center" Width="10%" />
                                </asp:BoundField>
                                <asp:BoundField DataField="BranchID" HeaderText="BranchID">
                                <ItemStyle HorizontalAlign="Left" Width="1%" />
                                </asp:BoundField>
                                <asp:BoundField DataField="ReceivedBy" HeaderText="Status">
                                 <ItemStyle HorizontalAlign="Center" Width="10%" Font-Bold="True" 
                                    Font-Size="10pt" />
                                </asp:BoundField>
                                <asp:BoundField DataField="CostingPriceTotal" HeaderText="CostingPriceTotal">
                                <ItemStyle HorizontalAlign="Left" Width="1%" />
                                </asp:BoundField>
                                <asp:BoundField DataField="CashAmounTotal" FooterText="SaleAmt" 
                                    HeaderText="Total Amount">
                                <ItemStyle HorizontalAlign="Right" Width="10%" Font-Bold="True" 
                                    Font-Size="10pt" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Remark" FooterText="Remarks" HeaderText="Remarks">
                                  <ItemStyle HorizontalAlign="Left" Width="20%" />
                                </asp:BoundField>
                            </Columns>
                        </asp:GridView>
                    </fieldset>
                    </asp:Panel>
                    </div>
                    <asp:Panel ID="PnlDtl" runat="server">
                          <div>
                              <fieldset style="vertical-align: top; border: solid 1px #8BB381; text-align:left; width: 80%">
                                <legend style="color: maroon;"><b>Branch Info</b></legend>
                                <table width="100%">
                    <tr>
                        <td style="width:9%; height: 21px; font-weight: 700;" align="right">Branch Name</td>
                        <td style="width:1%; height: 21px;"></td>
                        <td style="width:15%; height: 21px;">
                            <asp:TextBox ID="txtBranchName" style="text-align: left" CssClass="txtVisible" runat="server" Width="100%"></asp:TextBox>
                        </td>
                          <td style="width:8%; height: 21px; font-weight: 700;" align="right">
                              <asp:HiddenField ID="hfBranchID" runat="server" />
                              Upload Status</td>
                            <td style="width:1%; height: 21px; font-weight: 700;" align="center">:</td>
                             <td style="width:10%; height: 21px;">
                                 <asp:TextBox ID="txtUploadStatus" CssClass="txtVisibleAlignNotBorder" runat="server" Width="80%"></asp:TextBox>
                                 <asp:Label ID="lblMstID" runat="server" Visible="False"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="width:9%; height: 21px;" align="right">
                            Transfer Date</td>
                        <td style="width:1%; height: 21px;">
                            </td>
                        <td style="width:15%; height: 21px;">
                            <asp:TextBox ID="txtDate" style="text-align: left" CssClass="txtVisible" runat="server" Width="100%"></asp:TextBox>
                              <ajaxtoolkit:calendarextender runat="server" ID="Calendarextender2" 
            TargetControlID="txtDate" Format="dd/MM/yyyy"/>
                        </td>
                        <td style="width:8%; height: 21px;">
                            <asp:HiddenField ID="hfCostingPrice" runat="server" />
                            </td>
                        <td style="width:1%; height: 21px;">
                            </td>
                        <td style="width:10%; height: 21px;">
                            <asp:HiddenField ID="hfSalesAmount" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td align="right" style="width:9%; height: 23px;">
                            <asp:Label ID="LblSuppNo0" runat="server" Font-Bold="False" Font-Size="9pt">Remarks/Particulars</asp:Label>
                        </td>
                        <td style="width:1%; height: 23px;">
                        </td>
                        <td colspan="4" style="height: 23px;">
                            <asp:TextBox ID="txtRemarks" runat="server" TextMode="MultiLine" Width="98%"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right" style="width:9%;">
                            &nbsp;</td>
                        <td style="width:1%;">
                            &nbsp;</td>
                        <td style="width:15%;">
                            &nbsp;</td>
                        <td style="width:8%;">
                            &nbsp;</td>
                        <td style="width:1%;">
                            &nbsp;</td>
                        <td style="width:10%;">
                            &nbsp;</td>
                    </tr>
                </table>
                             </fieldset>
                        </div>
                <div>
                <fieldset style="vertical-align: top; border: solid 1px #8BB381; text-align:left;">
                    <legend style="color: maroon;"><b>Select Items Details</b></legend>
                    <asp:UpdatePanel ID="Updtl" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:GridView ID="dgReceiveDtl" runat="server" AutoGenerateColumns="False" 
                                BorderColor="LightGray" CssClass="mGrid" Font-Size="9pt" 
                               
                                Width="100%">
                                <AlternatingRowStyle CssClass="alt" />
                                <Columns>
                                    <asp:BoundField DataField="Code" HeaderText="Code">
                                         <ItemStyle HorizontalAlign="Center" Width="10%"></ItemStyle>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="ItemName" HeaderText="Item Name" ItemStyle-Width="25%">
                                        <ItemStyle HorizontalAlign="Left" Width="30%"></ItemStyle>
                                    </asp:BoundField >
                                    <asp:BoundField DataField="SaleQty" HeaderText="Sales/Return/Exchange Qty" 
                                        ItemStyle-Width="15%">
                                         <ItemStyle HorizontalAlign="Center" Width="10%"></ItemStyle>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="SaleAmount" HeaderText="Amount" 
                                        ItemStyle-Width="15%">
                                             <ItemStyle HorizontalAlign="Right" Width="10%"></ItemStyle>
                                    </asp:BoundField>
                                     <asp:BoundField DataField="Type" ItemStyle-Width="5%" HeaderText="Type" >
                                    <ItemStyle Width="5%" />
                                    </asp:BoundField>
                                </Columns>
                                <HeaderStyle Font-Bold="True" Font-Size="9pt" ForeColor="White" />
                                <PagerStyle CssClass="pgr" ForeColor="White" HorizontalAlign="Center" />
                                <RowStyle BackColor="White" />
                            </asp:GridView>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </fieldset>
                </div>
                 </asp:Panel>
                </td>
            <td style="width: 5%">
                &nbsp;</td>
        </tr>
        <tr>
            <td style="width: 5%">
                &nbsp;</td>
            <td style="width: 90%">
            
                 </td>
            <td style="width: 5%">
                &nbsp;</td>
        </tr>
    </table>

    </div>
</asp:Content>

