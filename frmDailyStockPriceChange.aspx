<%@ Page Title="" Language="C#" MasterPageFile="~/BranchMasterPage.master" AutoEventWireup="true" CodeFile="frmDailyStockPriceChange.aspx.cs" Inherits="frmDailyStockPriceChange" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    




    <script language="javascript" type="text/javascript">
        function onListPopulated() {
            var completionList1 = $find("AutoCompleteEx1").get_completionList();
            completionList1.style.width = 'auto';
        }
        //    function OpenWindow(Url) {
        //        var testwindow = window.open(Url, '', 'width=800px,height=620px,top=150,left=300,scrollbars=1');

        function OpenWindow(Url) {
            var popUpObj;
            //        var testwindow = window.open(Url, '', 'width=500px,height=420,top=200,left=500,scrollbars=1');
            //        testwindow.blur();

            popUpObj = window.open(Url, "ModalPopUp", "toolbar=no," + "scrollbars=no," + "location=no," + "statusbar=no," + "menubar=no," + "resizable=0," + "width=800px," +

    "height=620px," + "left = 300," + "top=150");
            popUpObj.focus();
            LoadModalDiv();
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


<table  id="pageFooterWrapper">
    <tr>  
        <td style="width:5%;"></td>
        <td align="center">
            <asp:Button ID="Delete" runat="server" ToolTip="Delete"
                        
                onclientclick="javascript:return window.confirm('are u really want to delete these data')" Text="Delete" 
                        Height="35px" Width="120px" BorderStyle="Outset" 
                Visible="False"  />
        </td>
        <td style="width:20px;"></td>
        <td align="center" >
            <asp:Button ID="btnSave" runat="server" ToolTip="Save Supplier Record" 
                        onclick="btnSave_Click" Text="Save" 
                        Height="35px" Width="120px" BorderStyle="Outset"  />
        </td>             
        <td style="width:20px;"></td>
        <td align="center" >
            <asp:Button ID="Clear" runat="server"  ToolTip="Clear" Text="Clear" 
                        Height="35px" Width="120px" BorderStyle="Outset" 
                onclick="Clear_Click"  />
        </td>
        <td style="width:20px;"></td>
        <td style="width:5%;">
            <asp:Button ID="Button1" runat="server" ToolTip="Save Supplier Record" 
                        onclick="btnPrint_Click" Text="Print" 
                        Height="35px" Width="120px" BorderStyle="Outset" 
                Visible="False"  />
        </td>
        <td style="width:5%;"></td>
    </tr>
</table>
<div id="frmMainDiv" style="background-color:White; width:100%;">  
    <table style="width: 100%">
        
        <tr>
            <td colspan="3">
                &nbsp;</td>
        </tr>
        
        <tr>
            <td colspan="3">
                <fieldset style="vertical-align: top; border: solid 1px #8BB381; text-align:left;">
                <legend style="color: maroon;"><b>Search Option</b></legend> 
                <table style="width: 100%">
                    <tr>
                        <td style="width: 9%; font-weight: 700;">
                            Department</td>
                        <td style="width: 1%" align="center">
                            &nbsp;</td>
                        <td style="width: 10%; font-weight: 700;">
                            Item&#39;s Name</td>
                        <td style="width: 1%">
                            &nbsp;</td>
                        <td style="width: 9%; font-weight: 700;">
                            Item&#39;s Details</td>
                        <td style="width: 10%">
                            <table style="width: 100%">
                                <tr>
                                    <td style="width: 50%" align="center">
                                        <b>Form Date</b></td>
                                    <td style="width: 50%" align="center">
                                        <b>To Date</b></td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 9%">
                 <asp:DropDownList runat="server" Width="102%" ID="ddlDepart" 
                                OnSelectedIndexChanged="ddlDepart_SelectedIndexChanged" Height="26px"></asp:DropDownList>

                         </td>
                        <td style="width: 1%" align="center">
                            &nbsp;</td>
                        <td colspan="1">
                    <asp:TextBox ID="txtNameOnly" runat="server" Width="100%" style="text-indent:15px;display: inline-block;border: 1px solid #ccc;
    border-radius: 4px;box-sizing: border-box;background:transparent !important;"
                        placeholder="Search Name.." 
                        Height="26px" AutoPostBack="True" ontextchanged="txtNameOnly_TextChanged"></asp:TextBox>
                         <ajaxtoolkit:AutoCompleteExtender ID="AutoCompleteExtender1"
                                                        runat="server" CompletionInterval="20" CompletionSetCount="30" MinimumPrefixLength="1"
                                                        ServiceMethod="GetSetupItem" ServicePath="~/AutoComplete.asmx"
                                                        TargetControlID="txtNameOnly" DelimiterCharacters="" 
                                            Enabled="True">
                                                    </ajaxtoolkit:AutoCompleteExtender>
                            <asp:HiddenField ID="hfItemsIDOnly" runat="server" />
                            </td>
                            <td>
                            </td>
                            <td>
                            <asp:HiddenField ID="hfItemsID" runat="server" />
                    <asp:TextBox ID="txtName" runat="server" Width="100%" style="text-indent:15px;display: inline-block;border: 1px solid #ccc;
    border-radius: 4px;box-sizing: border-box;background:transparent !important;"
                        placeholder="Search Code/Name.." 
                        Height="26px" AutoPostBack="True" ontextchanged="txtName_TextChanged"></asp:TextBox>
                        <ajaxToolkit:AutoCompleteExtender ServicePath="AutoComplete.asmx" runat="server" 
                                     ID="txtName_AutoCompleteExtender" TargetControlID="txtName"
           ServiceMethod="GetItemSearchAll2" MinimumPrefixLength="1" 
                                     CompletionSetCount="12" DelimiterCharacters="" 
                        Enabled="True"/>
                            </td>
                        <td style="width: 10%">
                            <table style="width: 100%">
                                <tr>
                                    <td style="width: 50%" align="center">
                                        <asp:TextBox ID="txtFormDate" CssClass="txtVisibleAlign" 
                                            placeHolder="dd/MM/yyyy" runat="server" Height="26px" Width="93%"></asp:TextBox>
                                        <ajaxToolkit:CalendarExtender runat="server" ID="cal1" Format="dd/MM/yyyy" TargetControlID="txtFormDate"/>
                                    </td>
                                    <td style="width: 50%" align="center">
                                        <asp:TextBox ID="txtToDate" runat="server"  CssClass="txtVisibleAlign" 
                                            placeHolder="dd/MM/yyyy" Height="26px" Width="93%"></asp:TextBox>
                                        <ajaxToolkit:CalendarExtender runat="server" ID="CalendarExtender1" Format="dd/MM/yyyy" TargetControlID="txtToDate"/>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 9%">
                                        <strong>Category</strong></td>
                        <td style="width: 1%" align="center">
                            &nbsp;</td>
                        <td style="width: 10%">
                                        <strong>Sub Category</strong></td>
                        <td style="width: 1%">
                            &nbsp;</td>
                        <td style="width: 9%">
                            <strong>Brand Name</strong></td>
                        <td style="width: 10%">
                           
                            <strong>Barcode</strong></td>
                    </tr>
                    <tr>
                        <td style="width: 9%">
                          
                            <asp:HiddenField ID="hfCatagoryID" runat="server" />
                            <asp:TextBox ID="txtCatagory" runat="server" Width="100%" style="text-indent:15px;display: inline-block;border: 1px solid #ccc;
    border-radius: 4px;box-sizing: border-box;background:transparent !important;"
                                         placeholder="Search Category.." 
                                         Height="26px" AutoPostBack="True" ontextchanged="txtCatagory_TextChanged"></asp:TextBox>
                            <ajaxToolkit:AutoCompleteExtender ServicePath="AutoComplete.asmx" runat="server" 
                                                              ID="txtName1_AutoCompleteExtender" TargetControlID="txtCatagory"
                                                              ServiceMethod="GetItemCatagorySearchAll" MinimumPrefixLength="1" 
                                                              CompletionSetCount="12" DelimiterCharacters="" 
                                                              Enabled="True"/>

                            </td>
                        <td style="width: 1%" align="center">
                            &nbsp;</td>
                        <td style="width: 10%">
                            
                            <asp:HiddenField ID="hfSubCatagoryID" runat="server" />
                            <asp:TextBox ID="txtSubCatagory" runat="server" Width="100%" style="text-indent:15px;display: inline-block;border: 1px solid #ccc;
    border-radius: 4px;box-sizing: border-box;background:transparent !important;"
                                         placeholder="Search Sub Category.." 
                                         Height="26px" AutoPostBack="True" ontextchanged="txtSubCatagory_TextChanged"></asp:TextBox>
                            <ajaxToolkit:AutoCompleteExtender ServicePath="AutoComplete.asmx" runat="server" 
                                                              ID="txtName2_AutoCompleteExtender" TargetControlID="txtSubCatagory"
                                                              ServiceMethod="GetItemSubCatagorySearchAll" MinimumPrefixLength="1" 
                                                              CompletionSetCount="12" DelimiterCharacters="" 
                                                              Enabled="True"/>
                            

                           
                            </td>
                        <td style="width: 1%">
                            &nbsp;</td>
                        <td style="width: 9%">
                            <asp:HiddenField ID="hfBrand" runat="server" />
                    <asp:TextBox ID="txtBrand" runat="server" Width="100%" style="text-indent:15px;display: inline-block;border: 1px solid #ccc;
    border-radius: 4px;box-sizing: border-box;background:transparent !important;"
                        placeholder="Search Brand.." 
                        Height="26px" AutoPostBack="True" ontextchanged="txtBrand_TextChanged"></asp:TextBox>
                        <ajaxToolkit:AutoCompleteExtender ServicePath="AutoComplete.asmx" runat="server" 
                                     ID="txtName3_AutoCompleteExtender" TargetControlID="txtBrand"
           ServiceMethod="GetItemBrandSearchAll" MinimumPrefixLength="1" 
                                     CompletionSetCount="12" DelimiterCharacters="" 
                        Enabled="True"/>
                            </td>
                        <td style="width: 10%" valign="bottom">
                           
                            
                            
                            <asp:TextBox ID="txtItemDesc" runat="server" Width="100%" style="text-indent:15px;display: inline-block;border: 1px solid #ccc;
    border-radius: 4px;box-sizing: border-box;background:transparent !important;"
                                         placeholder="Search Brand.." 
                                         Height="26px" AutoPostBack="True" ontextchanged="txtItemDesc_TextChanged"></asp:TextBox>

                            <ajaxToolkit:AutoCompleteExtender ID="autoComplete1" runat="server"  CompletionInterval="20"
                                                              CompletionSetCount="12" EnableCaching="true" 
                                                              MinimumPrefixLength="1" ServiceMethod="GetItemListBarcode" 
                                                              ServicePath="AutoComplete.asmx" TargetControlID="txtItemDesc" /> 
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 9%">
                            &nbsp;</td>
                        <td style="width: 1%" align="center">
                            &nbsp;</td>
                        <td style="width: 10%">
                            &nbsp;</td>
                        <td style="width: 1%">
                            &nbsp;</td>
                        <td style="width: 9%">
                            &nbsp;</td>
                        <td style="width: 10%" valign="bottom">
                            <table style="width: 100%">
                                <tr>
                                    <td style="width: 33%" align="center">
                                        <asp:Button ID="btnSearch" runat="server" OnClick="btnSearch_Click" Text="Search" 
                                                    Width="80%" Height="35px" />
                                    </td>
                                    <td style="width: 33%" align="center">
                                        <asp:Button ID="btnClear" runat="server" Text="Clear" Width="80%" 
                                                    onclick="btnClear_Click" Height="35px" />
                                    </td>
                                    <td style="width: 33%" align="center">
                                        <asp:Button ID="btnPrint" runat="server" OnClick="btnPrint_Click" Text="Print" 
                                                    Width="70%" Height="35px" />
                                    </td>
                                </tr>
                            </table></td>
                    </tr>
                    </table>
                </fieldset>
            </td>
        </tr>
        
        <tr>
            <td colspan="3">
                 <div>
            <table style="width:100%;">
                <tr>
            <td align="left" colspan="3">
            </td>
        </tr>
        <tr>
            <td align="center" colspan="3">
                <asp:GridView ID="dgItems" runat="server" AutoGenerateColumns="False" 
                    CssClass="mGrid" Width="100%" AllowPaging="True" PageSize="100" 
                    onpageindexchanging="dgItems_PageIndexChanging" 
                    onrowdatabound="GridView1_RowDataBound" 
                    onselectedindexchanged="dgItems_SelectedIndexChanged">
                    <Columns>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:CheckBox ID="chkSelect" runat="server" AutoPostBack="true" 
                                    OnCheckedChanged="chkSelect_CheckedChanged" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="chkSelect" runat="server" 
                                    Checked='<%# Eval("Generate").ToString()!="0" %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:CommandField HeaderText="Image" ShowSelectButton="True" 
                            SelectText="Image" >
                        <ItemStyle HorizontalAlign="Center" Width="5%"></ItemStyle>
                        </asp:CommandField>
                        <asp:BoundField DataField="ID" HeaderText="ID">
                         <ItemStyle HorizontalAlign="Right" Width="5%"></ItemStyle>
                        </asp:BoundField>
                        <asp:BoundField DataField="Dept_Name" HeaderText="Department">
                        </asp:BoundField>
                        <asp:BoundField DataField="Barcode" HeaderText="Barcode">
                            <ItemStyle HorizontalAlign="Left" Width="8%"></ItemStyle>
                        </asp:BoundField>
                        <asp:BoundField DataField="Name" HeaderText="Name">    
                        <ItemStyle HorizontalAlign="Left" Width="15%"></ItemStyle>
                        </asp:BoundField>
                        <asp:BoundField DataField="BrandName" HeaderText="Brand">    
                        <ItemStyle HorizontalAlign="Left" Width="8%" />
                        </asp:BoundField>
                        <asp:BoundField DataField="SizeName" HeaderText="Size">    
                        <ItemStyle HorizontalAlign="Left" Width="8%"></ItemStyle>
                        </asp:BoundField>
                        <asp:BoundField DataField="Category" HeaderText="Category">    
                        <ItemStyle HorizontalAlign="Left" Width="8%"></ItemStyle>
                        </asp:BoundField>
                        <asp:BoundField DataField="ExpireDate" HeaderText="Expiration date">
                            <ItemStyle HorizontalAlign="Center" Width="8%" Font-Bold="True" 
                            Font-Size="11pt"></ItemStyle>
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="Sales Price">
                            <ItemTemplate>
                                <asp:TextBox ID="txtSealsPrice" runat="server" Style="text-align: right" 
                                             Text='<%#Eval("ItemsPrice") %>' Width="90%"></asp:TextBox>
                            </ItemTemplate>
                            <ItemStyle Width="8%" />
                        </asp:TemplateField>
                         <asp:BoundField DataField="CostPrice" HeaderText="Purchase Price">    
                        <ItemStyle HorizontalAlign="right" Width="8%" Font-Bold="True" Font-Size="11pt"></ItemStyle>
                        </asp:BoundField>

                        <asp:BoundField DataField="TotalClosingStock" HeaderText="Closing Stock">    
                        <ItemStyle HorizontalAlign="Right" Width="8%" Font-Bold="True" Font-Size="12pt"></ItemStyle>
                        </asp:BoundField>
                       
                        <asp:BoundField DataField="ClosingAmount" HeaderText="Closing Amount">
                        <ItemStyle Width="0%" />
                        </asp:BoundField>
                        <asp:BoundField DataField="SalesClosingQty" HeaderText="Sales Closing Qty">
                        <ItemStyle Width="0%" />
                        </asp:BoundField>
                        <asp:BoundField DataField="ItemCode" HeaderText="ItemCode">
                        <ItemStyle Width="0%" />
                        </asp:BoundField>
                        <asp:BoundField DataField="GRN_ID" HeaderText="GRN_ID">
                        <ItemStyle Width="0%" />
                        </asp:BoundField>
                        <asp:BoundField DataField="ItemID" HeaderText="ItemID" SortExpression="ItemID">
                        <ItemStyle Width="0%" />
                        </asp:BoundField>
                        <asp:BoundField DataField="CostPrice" HeaderText="CostPrice">
                        <ItemStyle Width="0%" />
                        </asp:BoundField>
                       
                    </Columns>
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td style="width:20%;">&nbsp;</td>
            <td>&nbsp;</td>
            <td style="width:20%;">&nbsp;</td>
        </tr>
            </table>
           </div>
            </td>
        </tr>
        <tr>
            <td style="width:20%;">&nbsp;</td>
            <td>&nbsp;</td>
            <td style="width:20%;">&nbsp;</td>
        </tr>
        <tr>
            <td style="width:20%;">&nbsp;</td>
            <td>&nbsp;</td>
            <td style="width:20%;">&nbsp;</td>
        </tr>
    </table>
    </div>
     <div id="divBackground" style="position: fixed; z-index: 999; height: 100%; width: 100%;
        top: 0; left:0; background-color: Black; filter: alpha(opacity=60); opacity: 0.6; -moz-opacity: 0.8;-webkit-opacity: 0.8;display:none">
    </div>
    

</asp:Content>

