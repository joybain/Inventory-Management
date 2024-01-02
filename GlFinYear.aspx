<%@ Page Title="Financial Calendar Setup" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="GlFinYear.aspx.cs" Inherits="GlFinYear"  Theme="Themes"%>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">  <script src='<%= ResolveUrl("~/Scripts/valideDate.js") %>' type="text/javascript"></script>

<script language="javascript" type="text/javascript">
    function clickButton(e) {
        if (navigator.appName.indexOf("Netscape") > (-1)) {
            if (e.keyCode == 13) {
                bt.click();
                return false;
            }
        }
        if (navigator.appName.indexOf("Microsoft Internet Explorer") > (-1)) {
            if (event.keyCode < 48 || event.keyCode > 57) {
                alert('Enter Numbers Only');
                return false;
            }
            if (event.Length > 3) {
                alert('Number Should be between 1 and 999');
                return false;
            }
        }
    }
    function clickButton1(e1) {
        if (navigator.appName.indexOf("Netscape") > (-1)) {
            if (e.keyCode == 13) {
                bt.click();
                return false;
            }
        }
        if (navigator.appName.indexOf("Microsoft Internet Explorer") > (-1)) {
            if (event.keyCode < 97 || event.keyCode > 122) {
                alert('Enter Characters Only');
                return false;
            }
           
        }
    }
   
   
</script>


<div id="frmMainDiv" style="background-color:White; width:100%; overflow:visible;"> 
<div>
<table id="pageFooterWrapper">
   <tr>
   <td align="center" >
       <asp:Button ID="btnClear" runat="server"  ToolTip="Clear" onclick="btnClear_Click" Text="Clear" 
        Width="100px"  />
       </td>
   <td align="center" >
       <asp:Button ID="btnSave" runat="server" ToolTip="Save" onclick="btnSave_Click" Text="Save" 
        Width="100px"  />
       </td>
   <td align="center" >
       <asp:Button ID="btnDelete" runat="server" ToolTip="Delete" onclick="btnDelete_Click"
           onclientclick="javascript:return window.confirm('are u really want to delete these data')" Text="Delete" 
        Width="100px"  />
        </td> 
   <td align="center">
   <asp:Button ID="btnPopMon" runat="server" ToolTip="Populate Month" 
           onclick="btnPopMonth_Click"  Text="Populate"
        Width="100px"  /></td>   
   </tr>
   </table>
</div>
<table style="width:100%;">
<tr>
<td style="width:1%;"></td>
<td style="width:98%;" align="center">

<asp:UpdatePanel ID="UpdatePanel1" runat="server">    
<ContentTemplate>   
<div id="YearSec" runat="server">

<div style="width: 98%; height: 90px; border: solid 1px gray; padding: 5px; color: #666666">
<table style="width:98%">
<tr>
<td style="width:15%;">
<asp:Label ID="lblFinYear" runat="server" Font-Size="8pt" Width="100%">Financial Year</asp:Label>
</td>
<td style="width:20%;"> <asp:TextBox SkinID="tbGray" ID="txtFinYear" runat="server"  Width="100%" Font-Size="8" 
        AutoPostBack="true" ontextchanged="txtFinYear_TextChanged" MaxLength="9"></asp:TextBox>
<ajaxToolkit:TextBoxWatermarkExtender ID="WaterMark1" runat="server" TargetControlID="txtFinYear"
 WatermarkText="YYYY or YYYY-YYYY" WatermarkCssClass="textBoxWatermark"></ajaxToolkit:TextBoxWatermarkExtender>

<%--<asp:CustomValidator ID="cvFinYear" runat="server" ControlToValidate="txtFinYear" 
ClientValidationFunction="ValidateFinYear"></asp:CustomValidator>--%>
<%--<asp:RegularExpressionValidator ID="vldFinYear" runat="server" ValidationExpression="(\d\d\d\d)|(\d\d\d\d-\d\d\d\d)" 
ErrorMessage="Financial year would be YYYY or YYYY-YYYY format" ControlToValidate="txtFinYear" Display="Dynamic"></asp:RegularExpressionValidator>--%>
</td>
<td style=" width:5%;" ></td>
<td style="width:15%;">
<asp:Label ID="lblDesc" runat="server" Font-Size="8pt" Width="100%">Description</asp:Label>
</td>
<td style="width:45%;" colspan="3"> <asp:TextBox SkinID="tbGray" ID="txtDesc" runat="server" Width="100%" Font-Size="8" MaxLength="200"></asp:TextBox></td>
</tr>
<tr>
<td style="width:15%;">
<asp:Label ID="lblStartDate" runat="server" Font-Size="8pt" Width="100%">Start Date</asp:Label>
</td>
<td> <asp:TextBox SkinID="tbGray" ID="txtStartDate" runat="server"  Width="100%" Font-Size="8" MaxLength="11"></asp:TextBox>
<ajaxtoolkit:calendarextender runat="server" ID="Calendarextender3" TargetControlID="txtStartDate" Format="dd/MM/yyyy"/>
</td>
<td style=" width:5%" ></td>
<td style="width:15%;">
<asp:Label ID="lblEndDate" runat="server" Font-Size="8pt" Width="100%">End Date</asp:Label>
</td>
<td style="width:20%;"> 
<asp:TextBox SkinID="tbGray" ID="txtEndDate" runat="server" Width="100%" Font-Size="8" MaxLength="11"></asp:TextBox>
<ajaxtoolkit:calendarextender runat="server" ID="Calendarextender1" TargetControlID="txtEndDate" Format="dd/MM/yyyy"/>
</td>
<td colspan="2" style="width:25%;" align="center">
<asp:Button ID="btnAuth" runat="server" ToolTip="Authorize" onclick="btnAuth_Click"   
           Text="Authorize" Width="100px" />          
<ajaxToolkit:ModalPopupExtender ID="ModalPopupExtenderLogin" runat="server" 
             TargetControlID="btnAuth" PopupControlID="LoginPanel"
              BackgroundCssClass="modalBackground"/>                         
</td>
</tr>
<tr>
<td style="width:15%;">
<asp:Label ID="lblYearFlag" runat="server" Font-Size="8pt" Width="100%">Year Flag</asp:Label>
</td>
<td style="width:20%;"> <asp:DropDownList SkinID="ddlPlain" ID="dlYearFlag" runat="server"  Width="100%" Font-Size="8pt">
<asp:ListItem Text="Open" Value="O" Selected="True"></asp:ListItem>
<asp:ListItem Text="Closed" Value="C"></asp:ListItem>
<asp:ListItem Text="Never Open" Value="N"></asp:ListItem>
</asp:DropDownlist></td>
<td style=" width:5%;" >&nbsp&nbsp</td>
<td style="width:15%;">
<asp:Label ID="Label2" runat="server" Font-Size="8pt" Width="100%">Week Allow</asp:Label>
</td>
<td style="width:20%;"> 
<asp:DropDownList SkinID="ddlPlain" ID="dlWeeklyFin" runat="server" Width="100%" Font-Size="8pt">
<asp:ListItem Text="Yes" Value="Y"></asp:ListItem> 
<asp:ListItem Text="No" Value="N" Selected="True"></asp:ListItem>
</asp:DropDownlist>
</td>
<td style="width:10%;" align="right">
<asp:Label ID="lblStatus" runat="server" Font-Size="8pt" Text="Status" CssClass="tbc" ></asp:Label>
</td>
<td style="width:15%;" align="center">
<asp:TextBox SkinID="tbGray" ID="txtStatus" runat="server" Width="100%" Enabled="False" Font-Size="8" MaxLength="1"></asp:TextBox>
</td>
</tr>
</table>
</div>

<asp:UpdateProgress ID="udProgress" runat="server" DisplayAfter="100" Visible="true" DynamicLayout="true">
    <ProgressTemplate>
    <img src="img/loading.gif" alt="Process is running..."/>
    </ProgressTemplate>
</asp:UpdateProgress>
</div>
<br />
<asp:Label ID="lblTranStatus" runat="server" Font-Size="8"></asp:Label>
<br />
<asp:Panel ID="LoginPanel" runat="server" CssClass="modalPopup" Style="display: none" Width="200px">
       
            <table style="width: 200px; font-size:10;">
                <tr>
                    <td style="width: 84px " align="left">
                        <asp:Label ID="lblUserName" runat="server" Font-Size="8pt" Height="23px" Text="UserName :" Width="50px"></asp:Label>
                    </td>
                    <td style="width: 116px" >
                    <asp:TextBox SkinID="tbGray" ID="loginId" runat="server" Font-Size="8pt"  Width="115px"></asp:TextBox></td>
                </tr>
                <tr>
                    <td style="width: 84px " align="left">
                        <asp:Label ID="lblPassword" runat="server" Font-Size="8pt"  Height="23px" Text="Password :" Width="50px"></asp:Label>
                    </td>
                    <td style="width: 116px">
                        <asp:TextBox SkinID="tbGray" ID="pwd" runat="server" Font-Size="8pt"  Width="115px" TextMode="Password"></asp:TextBox></td>
                </tr>
                <tr>
                    <td style="width: 84px">
                    <asp:Button ID="CancelBtn" runat="server" Font-Size="8pt"  Text="Cancel" Width="60px" OnClick="CancelBtn_Click"/>
                    </td>
                    <td style="width: 116px">                        
                    <asp:Button ID="LoginBtn" runat="server" Font-Size="8pt"  Text="Authorize" OnClick="LoginBtn_Click" /></td>
                </tr>
            </table>       
</asp:Panel>
<div>
<asp:GridView  RowStyle-Height="25px" CssClass="mGrid" PagerStyle-CssClass="pgr"  
        AlternatingRowStyle-CssClass="alt" ID="dgFinYear" runat="server" AutoGenerateColumns="false" 
        AllowPaging="True" Width="100%" BackColor="White" BorderWidth="1px" BorderStyle="Solid"
        CellPadding="2" CellSpacing="0" BorderColor="LightGray" Font-Size="8pt" 
        AllowSorting="true" PageSize="80" 
        onselectedindexchanged="dgFinYear_SelectedIndexChanged" 
        ForeColor="#333333"  >
  <HeaderStyle Font-Size="9pt" Font-Bold="True" HorizontalAlign="center" BackColor="Silver"/>
  <FooterStyle BackColor="Silver" Font-Bold="True" ForeColor="White" />
  <Columns>
  <asp:CommandField ShowSelectButton="True"  ItemStyle-Width="40px" ItemStyle-HorizontalAlign="Center" ItemStyle-ForeColor="Blue"/>
  <asp:BoundField  HeaderText="Financial Year" DataField="fin_year" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center"/>
  <asp:BoundField  HeaderText="Description" DataField="description" ItemStyle-Width="200px" ItemStyle-HorizontalAlign="Left"/>
  <asp:BoundField  HeaderText="Year Flag" DataField="year_flag" ItemStyle-Width="90px" ItemStyle-HorizontalAlign="Center"/>
  
  </Columns>
                        <RowStyle BackColor="white" />
                        <EditRowStyle BackColor="" />
                        <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                        <HeaderStyle BackColor="Silver" Font-Bold="True" ForeColor="Black" />
                        <AlternatingRowStyle BackColor="#F5F5F5" />
  </asp:GridView>

  </div>
  <div>
<asp:GridView  RowStyle-Height="25px" CssClass="mGrid" PagerStyle-CssClass="pgr"  AlternatingRowStyle-CssClass="alt" ID="dgFinMonth" runat="server" AutoGenerateColumns="False" 
        AllowPaging="True" Width="100%" BackColor="White" BorderWidth="1px" BorderStyle="Solid"
        CellPadding="2" BorderColor="LightGray" Font-Size="8pt" AllowSorting="True" PageSize="22" 
          OnRowUpdating="dgFinMonth_RowUpdating" OnRowEditing="dgFinMonth_RowEditing" OnRowCancelingEdit="dgFinMonth_CancelingEdit">
  <HeaderStyle Font-Size="9pt" ForeColor="White" Font-Bold="True" BackColor="Silver" HorizontalAlign="center" /> 
  <Columns>
  
  <asp:TemplateField ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center">
  <ItemTemplate>  
  <asp:LinkButton ID="lbEdit" runat="server" Text="Edit" CausesValidation="false" CommandName="Edit"/>  
  </ItemTemplate>
  <EditItemTemplate>
  <asp:LinkButton ID="lbUpdate" runat="server" Text="Update" CausesValidation="false" CommandName="Update" />
  <asp:LinkButton ID="lbCancel" runat="server" Text="Cancel" CausesValidation="false" CommandName="Cancel"/>
  </EditItemTemplate>
  </asp:TemplateField>
  
  <asp:TemplateField HeaderText="Sl No" ItemStyle-Width="60px" ItemStyle-HorizontalAlign="Center">
  <ItemTemplate>
  <asp:Panel ID="panel1" runat="server">
  <asp:Label ID="lblMonthSl" runat="server" Text='<%#Eval("month_sl") %>' ></asp:Label>  
  </asp:Panel>  
  </ItemTemplate>  
  </asp:TemplateField>
  
  <asp:TemplateField HeaderText="Month Name" ItemStyle-Width="70px" ItemStyle-HorizontalAlign="Center">
  <ItemTemplate>
  <asp:Label ID="lblFinMon" runat="server" Text='<%#Eval("fin_mon") %>'></asp:Label>  
  </ItemTemplate>
  </asp:TemplateField>
  
  <asp:TemplateField HeaderText="Quarter" ItemStyle-Width="30px" ItemStyle-HorizontalAlign="Center">
  <ItemTemplate>
  <asp:Label ID="lblQuarter" runat="server" Text='<%#Eval("quarter") %>'></asp:Label>  
  </ItemTemplate>

<ItemStyle HorizontalAlign="Center" Width="30px"></ItemStyle>
  </asp:TemplateField>
  
  <asp:TemplateField HeaderText="Start Date" ItemStyle-Width="30px" ItemStyle-HorizontalAlign="Center">
  <ItemTemplate>
  <asp:Label ID="lblMonthStartDt" runat="server" Text='<%#Eval("mon_start_dt") %>'></asp:Label>  
  </ItemTemplate>

<ItemStyle HorizontalAlign="Center" Width="30px"></ItemStyle>
  </asp:TemplateField>
  
  <asp:TemplateField HeaderText="End Date" ItemStyle-Width="30px" ItemStyle-HorizontalAlign="Center">
  <ItemTemplate>
  <asp:Label ID="lblMonthEndDt" runat="server" Text='<%#Eval("mon_end_dt") %>'></asp:Label>  
  </ItemTemplate>

  </asp:TemplateField>
  
  <asp:TemplateField HeaderText="Month Flag" ItemStyle-Width="50px" ItemStyle-HorizontalAlign="Center">
  <ItemTemplate>
  <asp:Label ID="lblMonYearFlag" runat="server" Text='<%#Eval("year_flag") %>'></asp:Label>  
  </ItemTemplate>
  <EditItemTemplate>
  <asp:DropDownList SkinID="ddlPlain" ID="txtMonYearFlag" runat="server" Font-Size="8pt">
  <asp:ListItem Text="Open" Value="O"></asp:ListItem>
  <asp:ListItem Text="Closed" Value="C"></asp:ListItem>
  <asp:ListItem Text="Never Open" Value="N"></asp:ListItem>
  </asp:DropDownList>
  <%--<asp:TextBox SkinID="tbGray" ID="txtMonYearFlag" runat="server" Text='<%# Bind("year_flag") %>' >></asp:TextBox> --%>
  </EditItemTemplate>

<ItemStyle HorizontalAlign="Center" Width="50px"></ItemStyle>
  </asp:TemplateField>
  </Columns>
                        <RowStyle BackColor="white" />
                        <EditRowStyle BackColor="" />
                        <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                        <HeaderStyle BackColor="Silver" Font-Bold="True" ForeColor="Black" />
                        <AlternatingRowStyle BackColor="#F5F5F5" />
</asp:GridView>
</div>
</ContentTemplate>
</asp:UpdatePanel>
</td>
<td style="width:1%;"></td>
</tr>
</table>
</div>

</asp:Content>

