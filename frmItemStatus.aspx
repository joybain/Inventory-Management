<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="frmItemStatus.aspx.cs" Inherits="frmItemStatus" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">

<div>
            <table style="width:100%;">
                <tr>
                    <td style="width:10%;">
                    </td>
                    <td style="width:80%;">
                        <fieldset style="vertical-align: top; border: solid 1px #8BB381;">
                            <legend style="color: maroon;"><b>Search Option</b> </legend>
                            <table style="width:100%;">
                            <tr>
                            <td>
                               
                               
                                
                                <div align="right" style="width:20%; height:20px;float: left;">
                                    <asp:Label ID="Label1" runat="server" Text="Search Items"></asp:Label>
                                </div>
                                <div style="width:5%;height:20px; float: left;">
                                </div>
                                <div style="width:55%;height:20px; float: left;">
                            <asp:HiddenField ID="hfItemsID" runat="server" />
                    <asp:TextBox ID="txtName" runat="server" Width="100%" style="text-indent:15px;display: inline-block;border: 1px solid #ccc;
    border-radius: 4px;box-sizing: border-box;background:transparent !important;"
                        placeholder="Search Code/Name.." 
                        Height="26px" AutoPostBack="True" ontextchanged="txtName_TextChanged"></asp:TextBox>
                        <ajaxToolkit:AutoCompleteExtender ServicePath="AutoComplete.asmx" runat="server" 
                                     ID="txtName_AutoCompleteExtender" TargetControlID="txtName"
           ServiceMethod="GetItemSearchAll" MinimumPrefixLength="1" 
                                     CompletionSetCount="12" DelimiterCharacters="" 
                        Enabled="True"/>
                                </div>
                                <div style="width:100%;height:10px; float: left;">
                                </div>
                                </td>
                            </tr>
                            </table>
                        </fieldset>
                    </td>
                    <td style="width:10%;">
                    </td>
                </tr>
        <tr>
            <td style="width:10%;"></td>
            <td style="width:80%;" align="center">

          
             <div style="width:35%;height:20px; float: left;" >
                 <asp:RadioButtonList ID="rbReportType" runat="server" BorderStyle="Double" 
                     RepeatDirection="Horizontal">
                     <asp:ListItem Selected="True" Value="P">Pdf</asp:ListItem>
                     <asp:ListItem Value="E">Excel</asp:ListItem>
                 </asp:RadioButtonList>
                </div>
                   <div style="width:17%;height:20px; float: left;" >
                       <asp:Button ID="btnPrint" runat="server" OnClick="btnPrint_Click" Text="Print" 
                           Width="120px" Height="35px" />
                </div>
             <div style="width:17%;height:20px; float: left;" >
                     &nbsp;&nbsp;&nbsp;&nbsp;
                </div>
             <div style="width:15%;height:20px; float: left;" >
                     <asp:Button ID="btnClear" runat="server" Text="Clear" Width="120px" 
                         onclick="btnClear_Click" Height="35px" />
                </div>
             <div style="width:35%;height:20px; float: left;" ></div>
             <div style="width:100%;height:5px; float: left;" ></div>
            </td>
            <td style="width:10%;"></td>
        </tr>
                <tr>
                    <td align="center" colspan="3">
                        <asp:GridView ID="dgItems" runat="server" AutoGenerateColumns="False" 
                            CssClass="mGrid" Width="95%" AllowPaging="True" 
                            onpageindexchanging="dgItems_PageIndexChanging">
                            <Columns>
                                <asp:BoundField DataField="ItemCode" HeaderText="Items Code">
                                <ItemStyle HorizontalAlign="Left" Width="13%" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Name" HeaderText="Item Name" >
                                 <ItemStyle HorizontalAlign="Left" Width="20%" />
                                </asp:BoundField>
                                <asp:BoundField DataField="SizeName" HeaderText="Size" />
                                <asp:BoundField DataField="UnitPrice" HeaderText="Cos.Price.">
                                
                                <ItemStyle HorizontalAlign="Center" Width="8%" />
                                </asp:BoundField>
                                <asp:BoundField DataField="SalesPrice" HeaderText="Sale.Price">
                                <ItemStyle HorizontalAlign="Right" Width="8%" />
                                </asp:BoundField>
                                <asp:BoundField DataField="ExpireDate" HeaderText="ExpireDate">
                                </asp:BoundField>
                                <asp:BoundField DataField="PurchaseQuantity" HeaderText="Purchase .Qty">
                                <ItemStyle HorizontalAlign="Left" Width="12%" />
                                </asp:BoundField>
                                  <asp:BoundField DataField="RetQty" HeaderText="Ret.Qty">
                                 <ItemStyle HorizontalAlign="Left" Width="5%" />
                                </asp:BoundField>  
                                
                                <asp:BoundField DataField="SalesQty" HeaderText="Sales QTY">
                                <ItemStyle HorizontalAlign="Left" Width="7%" />
                                <ItemStyle HorizontalAlign="Center" Width="8%" />
                                </asp:BoundField>
                                <asp:BoundField DataField="ClosingStock" HeaderText="Closing-Stock">
                                 <ItemStyle HorizontalAlign="Left" Width="10%" />
                                </asp:BoundField>

                                                             
                               
                            </Columns>
                            <PagerSettings PageButtonCount="2" />
                        </asp:GridView>
                    </td>
                </tr>
        <tr>
            <td style="width:10%;">&nbsp;</td>
            <td style="width:80%;">&nbsp;</td>
            <td style="width:10%;">&nbsp;</td>
        </tr>
            </table>
           </div>
</asp:Content>

