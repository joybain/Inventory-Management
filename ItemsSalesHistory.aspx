<%@ Page Title="Sales History.-DDP" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="ItemsSalesHistory.aspx.cs" Inherits="ItemsSalesHistory" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <script type = "text/javascript">
     function OnClose() {

         if (window.opener != null && !window.opener.closed) {
             window.opener.HideModalDiv();
         }
     }
     function OnOpen() {
         window.opener.LoadModalDiv();
     }
     window.onbeforeunload = OnClose;
     window.onload = OnOpen;


     function expandcollapse(obj, row) {
         var div = document.getElementById(obj);
         var img = document.getElementById('img' + obj);

         if (div.style.display == "none") {
             div.style.display = "block";
             if (row == 'alt') {
                 img.src = "img/minus.gif";
             }
             else {
                 img.src = "img/minus.gif";
             }
             img.alt = "Close to view other Map Breaks";
         }
         else {
             div.style.display = "none";
             if (row == 'alt') {
                 img.src = "img/plus.gif";
             }
             else {
                 img.src = "img/plus.gif";
             }
             img.alt = "Expand to show Breaks";
         }
     }
     function expandcollapse1(obj) {
         var div = document.getElementById(obj);

         //          alert(obj.valueOf());     
         if (div.style.display == "none") {
             div.style.display = "block";

         }
         else {
             div.style.display = "none";
         }
         div.style.display = "block";
     }

     function expandcollapse2(obj1, obj2) {
         var div1 = document.getElementById(obj1);
         var div2 = document.getElementById(obj2);

         //alert(obj.valueOf()); 
         if (obj1) {
             if (div1.style.display == "none") {
                 div1.style.display = "block";

             }
             else {
                 div1.style.display = "none";
             }
             div1.style.display = "block";
         }
         if (obj2) {
             if (div2.style.display == "none") {
                 div2.style.display = "block";

             }
             else {
                 div2.style.display = "none";
             }
             div2.style.display = "block";
         }
     }
</script>
    <div id="frmMainDiv" style="width:100%; background-color:White;">  
     

    <table style="width: 100%">
        <tr>
            <td style="width:10%;">
               
                &nbsp;</td>
            <td style="width:80%;">
                &nbsp;</td>
            <td style="width:10%;">
                &nbsp;</td>
        </tr>
        <tr>
            <td style="width:10%;">
                &nbsp;</td>
            <td style="width:80%;">
                &nbsp; <asp:Label ID="lblBranchName" runat="server" Font-Size="10pt" ForeColor="Black" 
                   style="font-weight: 700" Font-Underline="True"></asp:Label>
                &nbsp;<asp:Label ID="lblBranchID" runat="server" Visible="False"></asp:Label>
            </td>
            <td style="width:10%;">
                &nbsp;</td>
        </tr>
        <tr>
            <td style="width:10%; height: 43px;">
                </td>
            <td style="width:80%; height: 43px;">
             
             <fieldset style="vertical-align: top; border: solid 1px #8BB381; text-align:left;">
                            <legend style="color: maroon;"><b>Search Option</b></legend>
                <table style="width: 100%">
                    <tr>
                        <td style="width:10%; height: 31px;" align="right">
                            <asp:Label ID="Label3" runat="server" style="font-weight: 700" 
                                Text="Search Customer"></asp:Label>
                <asp:HiddenField ID="hfCustomerID" runat="server" />
                        </td>
                        <td style="width:1%; height: 31px; font-weight: 700;" align="center">
                            :</td>
                        <td style="height: 31px;" colspan="5">
                <asp:TextBox ID="txtCustomer" runat="server" AutoPostBack="True" CssClass="txtVisibleAlign"
                    ontextchanged="txtCustomer_TextChanged" placeHolder="Search Customer..." 
                    style="text-align:left;" TabIndex="11" Width="99%" Height="18px"></asp:TextBox>
                <ajaxToolkit:AutoCompleteExtender ID="AutoCompleteExtender2" runat="server" 
                    CompletionInterval="20" CompletionSetCount="12" EnableCaching="true" 
                    MinimumPrefixLength="1" ServiceMethod="GetCustomername" 
                    ServicePath="AutoComplete.asmx" TargetControlID="txtCustomer" />

                        </td>
                        <td style="height: 31px;" align="center" colspan="2">
                            <asp:Button ID="btnPrint" runat="server" Text="Print" Width="70%" 
                                onclick="btnSearch_Click" Visible="False" />
                            </td>
                    </tr>
                        
                   
                 <tr>
                        <td style="width:10%;" align="right">
                            <asp:Label ID="Label1" runat="server" style="font-weight: bold" 
                                Text="Start Date"></asp:Label>
                        </td>
                        <td style="width:1%; font-weight: 700;" align="center">
                            :</td>
                        <td style="width:10%;">
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
           <ContentTemplate> 
    <asp:TextBox SkinId="tbPlain" ID="txtStartDateDate" runat="server" Width="100%" CssClass="txtVisibleAlign"
                                style="text-align:center;" PlaceHolder="dd/MM/yyyy"
           AutoPostBack="True"  Font-Size="8pt" ontextchanged="txtStartDateDate_TextChanged" Height="18px" 
                               ></asp:TextBox>
    <ajaxtoolkit:calendarextender runat="server" ID="Calendarextender1" 
            TargetControlID="txtStartDateDate" Format="dd/MM/yyyy"/>
            </ContentTemplate>
          </asp:UpdatePanel>
                        </td>
                        <td  style="width:5%; font-weight: 700;" align="right">TO</td>
                        <td style="width:10%;" align="right">
                            <asp:Label ID="Label2" runat="server" style="font-weight: bold" Text="End Date"></asp:Label>
                        </td>
                        <td style="width:1%;">
                            &nbsp;</td>
                        <td style="width:10%;">
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
           <ContentTemplate> 
    <asp:TextBox SkinId="tbPlain" ID="txtEndDate" runat="server" Width="97%" 
                   style="text-align:center;" PlaceHolder="dd/MM/yyyy"
            CssClass="txtVisibleAlign"  AutoPostBack="True"  Font-Size="8pt" 
                   ontextchanged="txtEndDate_TextChanged" Height="18px"></asp:TextBox>
    <ajaxtoolkit:calendarextender runat="server" ID="txtEndDate_CalendarExtender" 
            TargetControlID="txtEndDate" Format="dd/MM/yyyy"/>
            </ContentTemplate> 
            </asp:UpdatePanel>
                        </td>
                        <td style="width:10%;" align="right">
                            <asp:Button ID="btnSearch" runat="server" Text="Search" Width="70%" 
                                onclick="btnSearch_Click" Height="35px" />
                        </td>
                        <td style="width:10%;" align="center">
                            <asp:Button ID="btnRefresh" runat="server" Text="Refresh" Width="70%" 
                                onclick="btnRefresh_Click" Height="35px" />
                        </td>
                    </tr>
               <%-- </ContentTemplate>  
                </asp:UpdatePanel>--%>
                </table></fieldset>
            </td>
            <td style="width:10%; height: 43px;">
                </td>
        </tr>
        <tr>
            <td style="width:10%;">
                &nbsp;</td>
            <td style="width:80%;">
                 <div id="divHitory" runat="server">
             <fieldset style="vertical-align: top; border: solid 1px #8BB381; text-align:left;">
                            <legend style="color: maroon;"><b>Sales History</b></legend>
            <asp:GridView ID="dgDepartment" runat="server" 
                  AlternatingRowStyle-CssClass="alt" AutoGenerateColumns="False" 
                  BackColor="White" BorderColor="LightGray" BorderStyle="Solid" BorderWidth="1px" 
                  CellPadding="2" CssClass="mGrid" Font-Size="8pt" PagerStyle-CssClass="pgr" 
                  PageSize="160" Width="100%" onrowdatabound="dgDepartment_RowDataBound" 
                                onrowcommand="dgDepartment_RowCommand">
                  <Columns>
                      <asp:TemplateField HeaderText="Select">
                            <ItemTemplate>
                                <asp:LinkButton ID="lblSelect" Visible="True"  runat="server" 
                                    CommandName="Select" Text="( Select )" Font-Bold="True" Font-Size="10pt" 
                                    Font-Underline="False"></asp:LinkButton>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" Width="8%" />
                          </asp:TemplateField>
                      <asp:TemplateField HeaderText="" ItemStyle-Width="40px">
                          <ItemTemplate>
                              <asp:Panel ID="panel1" runat="server">
                                 <a href="javascript:expandcollapse('div<%# Eval("InvoiceNo") %>', 'one');">
                                  <img id="imgdiv<%# Eval("InvoiceNo") %>" alt="Click to show/hide Maps"  width="25px"  src="img/plus.gif"/>
                                  </a>
                              </asp:Panel>
                          </ItemTemplate>
                          <EditItemTemplate>
                              <asp:Panel ID="panel1" runat="server" Width="40px">
                                  <asp:Label ID="lblSlNo1" runat="server" ForeColor="White" Text=".  ."></asp:Label>
                              </asp:Panel>
                          </EditItemTemplate>
                          <FooterTemplate>
                              <asp:Panel ID="panel1" runat="server" Width="40px">
                                  <asp:Label ID="lblSlNo1" runat="server" ForeColor="White" Text=".  ."></asp:Label>
                              </asp:Panel>
                          </FooterTemplate>
                          <ItemStyle HorizontalAlign="Center" Width="5%" />
                      </asp:TemplateField>
                      <asp:BoundField DataField="InvoiceNo" HeaderText="Invoice No">
                      <ItemStyle HorizontalAlign="Center" Width="15%" />
                      </asp:BoundField>
                      <asp:BoundField DataField="OrderDate" HeaderText="Order Date">
                      <ItemStyle HorizontalAlign="Center" Width="10%" />
                      </asp:BoundField>
                      <asp:BoundField HeaderText="Customer" DataField="CustomerName" >
                       <ItemStyle HorizontalAlign="Left" Width="15%" />
                      </asp:BoundField>
                      <asp:BoundField HeaderText="Branch" DataField="BranchName" >
                       <ItemStyle HorizontalAlign="Left" Width="15%" />
                      </asp:BoundField>
                      <asp:BoundField HeaderText="Sub Total" DataField="SubTotal" >
                       <ItemStyle HorizontalAlign="Right" Width="10%" />
                      </asp:BoundField>
                      <asp:BoundField DataField="ID" HeaderText="ID">
                      <ItemStyle HorizontalAlign="Left" Width="3%" />
                      </asp:BoundField>
                       
                       <asp:TemplateField>
                        <ItemTemplate>
                            <tr>
                                <td colspan="100%" align="center">
                                <div id="div<%# Eval("InvoiceNo") %>" style="display:none; position:relative; OVERFLOW:auto;" >
                                <asp:Panel Style="margin-left: 20px; margin-right: 5px;" ID="pnlSubMjrCat" runat="server" Visible="true">                                    
                                <asp:UpdatePanel ID="UpdatePanel1" runat="server"  UpdateMode="Conditional">
                                <ContentTemplate>
                                <asp:GridView  PagerStyle-CssClass="pgr"  AlternatingRowStyle-CssClass="alt" ID="dgSubMjrCat" runat="server" AutoGenerateColumns="false" Font-Size="9pt" ShowFooter="false" GridLines="None"
        BorderWidth="2px" BorderColor="DarkGray" BorderStyle="Double" Width="100%">
                                   <Columns>
                                    
                                    <%--DataField="DEPT_NAME"--%>

                                      <asp:BoundField DataField="Code"  HeaderText="Code">
                                          <ItemStyle HorizontalAlign="Center" Width="10%" />
                                      </asp:BoundField>  
                                      <asp:BoundField DataField="ItemsName"  HeaderText="Items Name">
                                          <ItemStyle HorizontalAlign="Left" Width="30%" />
                                      </asp:BoundField>                                       
                                       <asp:BoundField DataField="Quantity" DataFormatString="{0:n}"  HeaderText="Qty(Pic)">
                                          <ItemStyle HorizontalAlign="Right" Width="10%" />
                                      </asp:BoundField>  
                                       <asp:BoundField DataField="SalePrice"  HeaderText="Sales Price">
                                          <ItemStyle HorizontalAlign="Right" Width="10%" />
                                      </asp:BoundField>   
                                       <asp:BoundField DataField="TotalPrice"   HeaderText="Total Price">
                                          <ItemStyle HorizontalAlign="Right" Width="10%" />
                                      </asp:BoundField>                                        
                                  </Columns>
                                </asp:GridView>

                                </ContentTemplate>
                                </asp:UpdatePanel>
                                </asp:Panel>
                                </div>
                                </td>
                            </tr>                            
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Left" Width="10%" />
                        </asp:TemplateField>
                  </Columns>
                  <AlternatingRowStyle BackColor="" />
                  <EditRowStyle BackColor="" />
                  <HeaderStyle BackColor="" Font-Bold="True" Font-Size="8pt" ForeColor="Black" 
                      HorizontalAlign="Center" />
                  <PagerStyle CssClass="pgr" />
              </asp:GridView>
            </fieldset>
            </div>
            </td>
            <td style="width:10%;">
                &nbsp;</td>
        </tr>
        <tr>
            <td style="width:10%;">
                &nbsp;</td>
            <td style="width:80%;">
                &nbsp;</td>
            <td style="width:10%;">
                &nbsp;</td>
        </tr>
    </table>
    </div>
</asp:Content>

