<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Budget.aspx.cs" Inherits="Budget" Title="Budget Entry"  Theme="Themes"%>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">  
<script src='<%= ResolveUrl("~/Scripts/valideDate.js") %>' type="text/javascript"></script>

   
<div id="frmMainDiv" style="background-color:White; width:100%; overflow:visible;">
<table style="width:100%;">
<tr>
<td style="width:1%;"></td>
<td style="width:98%;" align="center"> 
<%--<div style="background: url('img/background.png') repeat-x; width: 100%; height: 90px; border: solid 1px #cccccc; padding: 5px; color: #666666">--%>
<table  id="pageFooterWrapper">
   <tr>   
   <td style="width:33%;" align="center"> 
       <asp:Button ID="btnClear" runat="server" ToolTip="Clear" onclick="btnClear_Click" 
       Text="Clear" 
        Width="100px"/>      
           </td>          
   
   <td style="width:33%;" align="center"> 
       <asp:Button ID="btnSave" runat="server" ToolTip="Save" onclick="btnSave_Click" 
       Text="Save" Width="100px"></asp:Button></td>        
   
   <td style="width:33%;" align="center"> 
       <asp:Button ID="btnDelete" runat="server" ToolTip="Delete" onclick="btnDelete_Click" 
           onclientclick="javascript:return window.confirm('Are u really want to delete these data?')" Text="Delete" 
        Width="100px" /></td>
      
   </tr>
   </table>
   <br />
<asp:UpdatePanel ID="UpdatePanel1" runat="server"  UpdateMode="Conditional" >
<ContentTemplate>
<table style="border:solid 1px gray; width:100%; padding-right:15px;">
<tr>
<td style="width:15%;">
<asp:Label ID="lblBudgetSysId" runat="server" Font-Size="8pt">Budget ID</asp:Label>
</td>
<td style="width:15%;"> 
<asp:TextBox SkinID="tbGray" ID="txtBudgetSysId" runat="server" CssClass="tbc" Width="100%" Font-Size="8" Enabled="false" MaxLength="4"></asp:TextBox>
</td>
<td style=" width:5%;" ></td>
<td style="width:15%;">
<asp:Label ID="lblDesc" runat="server" Font-Size="8pt">Description</asp:Label>
</td>
<td style="width:50%" colspan="4"> 
<asp:TextBox SkinID="tbGray" ID="txtDesc" runat="server" Width="100%" Font-Size="8" MaxLength="150"></asp:TextBox></td>
</tr>
<tr>
<td style="width:15%;">
<asp:Label ID="lblFinYear" runat="server" Font-Size="8pt">Financial Year</asp:Label>
</td>
<td style="width:15%;"> 
<asp:TextBox SkinID="tbGray" ID="txtFinYear" runat="server" CssClass="tbc"  Width="100%" Font-Size="8" MaxLength="9" 
        AutoPostBack="True" ontextchanged="txtFinYear_TextChanged"></asp:TextBox>
        <ajaxToolkit:TextBoxWatermarkExtender ID="WaterMark1" runat="server" TargetControlID="txtFinYear"
 WatermarkText="YYYY-YYYY" WatermarkCssClass="textBoxWatermark"></ajaxToolkit:TextBoxWatermarkExtender>
</td>
<td style=" width:5%;" ></td>
<td style="width:15%;">
<asp:Label ID="lblFinStartDt" runat="server" Font-Size="8pt">Start Date</asp:Label>
</td>
<td style="width:15%;"> <asp:TextBox SkinID="tbGray" ID="txtFinStartDt" runat="server" CssClass="tbc"  Width="100%" Font-Size="8" MaxLength="11"></asp:TextBox>
</td>
<td style=" width:5%;" ></td>
<td style="width:15%;">
<asp:Label ID="lblFinEndDt" runat="server" Font-Size="8pt">End Date</asp:Label>
</td>
<td style="width:15%;"> <asp:TextBox SkinID="tbGray" ID="txtFinEndDt" runat="server" CssClass="tbc"  Width="100%" Font-Size="8" MaxLength="11"></asp:TextBox>
</td>
</tr>
<tr>
<td style="width:15%;">
<asp:Label ID="lblBudTypeCode" runat="server" Font-Size="8">Budget Type</asp:Label>
</td>
<td style="width:15%;"> <asp:DropDownList SkinID="ddlPlain" ID="ddlBudTypeCode" runat="server"  Font-Size="8" Width="100%">
  <asp:ListItem Text="Revenue" Value="REV"></asp:ListItem>
  <asp:ListItem Text="Project" Value="PRJ"></asp:ListItem>
  </asp:DropDownList>
</td>
<td style=" width:5%;" ></td>
<td style="width:15%;">
<asp:Label ID="lblBudOpen" runat="server" Font-Size="8pt">Budget</asp:Label>
</td>
<td style="width:15%;"> <asp:DropDownList SkinID="ddlPlain" ID="ddlBudOpen" runat="server"  Font-Size="8" Width="101%">
  <asp:ListItem Text="Open" Value="O"></asp:ListItem>
  <asp:ListItem Text="Closed" Value="C"></asp:ListItem>
  </asp:DropDownList>
</td>
<td style=" width:5%;" ></td>
<td style="width:100px; vertical-align:middle;">
<asp:Label ID="lblStatus" runat="server" Font-Size="8pt">Status</asp:Label>
</td>
<td style="width:15%;"> <asp:TextBox SkinID="tbGray" ID="txtStatus" runat="server" CssClass="tbc"  Width="100%" Font-Size="8" Enabled="false" MaxLength="1"></asp:TextBox>
</td>
</tr>
</table>
<br />
<asp:Panel ID="pnlPopBud" runat="server" Visible="true">
<table style="border:solid 2px Maroon; width:100%; padding-right:15px;">
<tr>
<td style="width:15%;">
<asp:Label ID="lblPSegCode" runat="server" Font-Size="8pt">Segment Code</asp:Label>
</td>
<td style="width:15%;"> 
<asp:TextBox SkinID="tbGray" ID="txtPSegCode" runat="server"  Width="100%" CssClass="tbc" Font-Size="8" MaxLength="7"></asp:TextBox>
</td>
<td style=" width:10%;" ></td>

<td style="width:15%;">
<asp:Label ID="lblPCostCenter" runat="server" Font-Size="8pt">Cost Center</asp:Label>
</td>
<td style="width:15%;"> <asp:TextBox SkinID="tbGray" ID="txtPCostCenter" runat="server" CssClass="tbc"  Width="100%" Font-Size="8" MaxLength="3"></asp:TextBox>
</td>
<td style=" width:10%;" ></td>
<td style="width:15%;" align="center" >
<asp:Button ID="btnPopBudget" runat="server" ToolTip="Populate Budget" 
           onclick="btnPopBudget_Click"  Text="Populate"
        Width="100px"  />
</td>
 <td style=" width:10px;" ></td>          
<td style="vertical-align:middle; " align="right">
<asp:Button ID="btnAuth" runat="server" ToolTip="Authorize" onclick="btnAuth_Click"   
           Text="Authorize" Width="100px" /> 
    <ajaxToolkit:ModalPopupExtender ID="ModalPopupExtenderLogin" runat="server" 
             TargetControlID="btnAuth" PopupControlID="LoginPanel"
              BackgroundCssClass="modalBackground"/>
</td>
</tr>
</table>
</asp:Panel>
<br />
    <asp:UpdateProgress ID="udProgress" runat="server" DisplayAfter="100" Visible="true" DynamicLayout="true">
    <ProgressTemplate>
    <img src="img/loading.gif" alt="Process is running..."/>
    </ProgressTemplate>
    </asp:UpdateProgress>
<asp:Label ID="lblTranStatus" runat="server" Width="100%" Text="" Visible="false" Font-Size="8"></asp:Label>

<asp:Panel ID="LoginPanel" runat="server" CssClass="modalPopup" Style="display: none" Width="200px">
       
            <table style="width: 200px; font-size:10;">
                <tr>
                    <td style="width: 84px " align="left">
                        <asp:Label ID="lblUserName" runat="server" Font-Size="8pt" Height="23px" Text="User ID" Width="50px"></asp:Label>
                    </td>
                    <td style="width: 116px" >
                    <asp:TextBox SkinID="tbGray" ID="loginId" runat="server" Font-Size="8pt"  Width="115px"></asp:TextBox></td>
                </tr>
                <tr>
                    <td style="width: 84px " align="left">
                        <asp:Label ID="lblPassword" runat="server" Font-Size="8pt"  Height="23px" Text="Password" Width="50px"></asp:Label>
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
<asp:GridView  RowStyle-Height="25px" CssClass="mGrid" PagerStyle-CssClass="pgr"  AlternatingRowStyle-CssClass="alt" ID="dgBudMst" runat="server" AutoGenerateColumns="false" 
        AllowPaging="True" Width="100%" BackColor="White" BorderWidth="1px" BorderStyle="Solid"
        CellPadding="2" CellSpacing="0" BorderColor="LightGray" Font-Size="8pt" AllowSorting="true" PageSize="5" 
        onselectedindexchanged="dgBudMst_SelectedIndexChanged" ForeColor="#333333"  >
  <HeaderStyle Font-Size="9" Font-Names="Arial" Font-Bold="True" HorizontalAlign="center" BackColor="Silver"/>
  <FooterStyle BackColor="Silver" Font-Bold="True" ForeColor="White" />
  <Columns>
  <asp:CommandField ShowSelectButton="True"  ItemStyle-Width="40px" ItemStyle-HorizontalAlign="Center" ItemStyle-ForeColor="Blue"/>
  <asp:BoundField  HeaderText="Budget ID" DataField="bud_sys_id" ItemStyle-Width="90px" ItemStyle-HorizontalAlign="Center"/>
  <asp:BoundField  HeaderText="Financial Year" DataField="fin_year" ItemStyle-Width="90px" ItemStyle-HorizontalAlign="Center"/>
  <asp:BoundField  HeaderText="Description" DataField="bud_desc" ItemStyle-Width="350px" ItemStyle-HorizontalAlign="Left"/>  
  </Columns>
                        <RowStyle BackColor="white" />
                        <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                        <HeaderStyle BackColor="Silver" Font-Bold="True" ForeColor="Black" />
                        <AlternatingRowStyle BackColor="#F5F5F5" />
  </asp:GridView>
<asp:GridView  RowStyle-Height="25px" CssClass="mGrid" PagerStyle-CssClass="pgr"  AlternatingRowStyle-CssClass="alt" ID="dgBudget" runat="server" AutoGenerateColumns="False" 
        AllowPaging="True" Width="100%" BackColor="White" BorderWidth="1px" BorderStyle="Solid"
        CellPadding="2" BorderColor="LightGray" Font-Size="8pt" AllowSorting="True" PageSize="25" 
          OnRowUpdating="dgBudget_RowUpdating" OnRowEditing="dgBudget_RowEditing" 
          OnRowCancelingEdit="dgBudget_CancelingEdit" ShowFooter="false" 
          onselectedindexchanging="dgBudget_SelectedIndexChanging" 
          onrowdeleting="dgBudget_RowDeleting"
          onrowdatabound="dgBudget_RowDataBound" 
        onrowcommand="dgBudget_RowCommand" 
        onpageindexchanging="dgBudget_PageIndexChanging" >
  <HeaderStyle Font-Size="9" ForeColor="White" Font-Bold="True" BackColor="Silver" HorizontalAlign="center"/> 
  <Columns>
 <%-- <asp:CommandField ShowSelectButton="true" />--%>
 <asp:TemplateField ItemStyle-Font-Size="8pt" ItemStyle-Width="120px" ItemStyle-HorizontalAlign="Center">
 <ItemTemplate>
 <asp:LinkButton ID="lbEdit" runat="server" CausesValidation="False" CommandName="Edit" Text="Edit" ></asp:LinkButton>  
  <asp:LinkButton ID="lbDelete" runat="server" CausesValidation="False" CommandName="Delete" Text="Delete"></asp:LinkButton>  
  <ajaxToolkit:ConfirmButtonExtender ID="detdeleteconfirm" runat="server" ConfirmText="Are you sure to delete??" TargetControlID="lbDelete"></ajaxToolkit:ConfirmButtonExtender> 
 </ItemTemplate>
 <EditItemTemplate>
 <asp:LinkButton ID="lbUpdate" runat="server" CausesValidation="True" CommandName="Update" Text="Update" ></asp:LinkButton> 
  <asp:LinkButton ID="lbCancel" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel"></asp:LinkButton>
 </EditItemTemplate> 
 </asp:TemplateField>
 
  <asp:TemplateField HeaderText="COA Code" ItemStyle-Width="90px" ItemStyle-HorizontalAlign="Center">
  <ItemTemplate>  
  <asp:Label ID="lblGlCoaCode" runat="server" Text='<%#Eval("gl_coa_code") %>' Width="90px" Font-Size="8pt"></asp:Label>    
  </ItemTemplate>
  <EditItemTemplate>  
  <asp:TextBox SkinID="tbGray" ID="txtGlCoaCode" runat="server" Text='<%#Eval("gl_coa_code") %>' Width="90px" Font-Size="8pt" MaxLength="13"></asp:TextBox>   
  </EditItemTemplate>
  </asp:TemplateField>
  
  <asp:TemplateField HeaderText="Increased Pct(%)" ItemStyle-Width="70px" ItemStyle-HorizontalAlign="Center">
  <ItemTemplate>
  <asp:Label ID="lblBudIncPct" runat="server" Text='<%#Eval("bud_inc_pct") %>'></asp:Label>  
  </ItemTemplate>
  <EditItemTemplate>
  <asp:TextBox SkinID="tbGray" ID="txtBudIncPct" runat="server" Text='<%#Eval("bud_inc_pct") %>' Width="70px" AutoPostBack="true"
   ontextchanged="txtBudIncPct_TextChanged" Font-Size="8" MaxLength="3"></asp:TextBox>
  </EditItemTemplate>
  </asp:TemplateField>
  
  <asp:TemplateField HeaderText="Tolerance Pct(%)" ItemStyle-Width="70px" ItemStyle-HorizontalAlign="Center">
  <ItemTemplate>
  <asp:Label ID="lblBudTolPct" runat="server" Text='<%#Eval("bud_tol_pct") %>' Font-Size="8pt"></asp:Label>  
  </ItemTemplate>
  <EditItemTemplate>
  <asp:TextBox SkinID="tbGray" ID="txtBudTolPct" runat="server" Text='<%#Eval("bud_tol_pct") %>' Width="70px" AutoPostBack="true"
   ontextchanged="txtBudTolPct_TextChanged" Font-Size="8" MaxLength="3"></asp:TextBox>
  </EditItemTemplate>
  </asp:TemplateField>
  
  <asp:TemplateField HeaderText="Tolerance Amount" ItemStyle-Width="90px" ItemStyle-HorizontalAlign="Right">
  <ItemTemplate>  
  <asp:Label ID="lblBudTolAmnt" runat="server" Text='<%#Eval("bud_tol_amnt") %>' Width="90px" Font-Size="8pt"></asp:Label>    
  </ItemTemplate>
  <EditItemTemplate>  
  <asp:TextBox SkinID="tbGray" ID="txtBudTolAmnt" runat="server" Text='<%#Eval("bud_tol_amnt") %>' Width="90px" Font-Size="8" MaxLength="15"></asp:TextBox>   
  </EditItemTemplate>
  </asp:TemplateField>
  
  <asp:TemplateField HeaderText="Override Amount" ItemStyle-Width="90px" ItemStyle-HorizontalAlign="Right">
  <ItemTemplate>  
  <asp:Label ID="lblBudOverrideAmnt" runat="server" Text='<%#Eval("bud_Override_amnt") %>' Width="90px" Font-Size="8pt"></asp:Label>    
  </ItemTemplate>
  <EditItemTemplate>  
  <asp:TextBox SkinID="tbGray" ID="txtBudOverrideAmnt" runat="server" Text='<%#Eval("bud_Override_amnt") %>' Width="90px" Font-Size="8" MaxLength="15"></asp:TextBox>   
  </EditItemTemplate>
  </asp:TemplateField>
  
  <asp:TemplateField HeaderText="Budget Amount" ItemStyle-Width="120px" ItemStyle-HorizontalAlign="Right">
  <ItemTemplate>  
  <asp:Label ID="lblBudAmnt" runat="server" Text='<%#Eval("bud_amnt") %>' Width="120px" Font-Size="8pt" ></asp:Label>    
  </ItemTemplate>
  <EditItemTemplate>  
  <asp:TextBox SkinID="tbGray" ID="txtBudAmnt" runat="server" Text='<%#Eval("bud_amnt") %>' Width="120px" Font-Size="8" MaxLength="15"></asp:TextBox>   
  </EditItemTemplate>
  </asp:TemplateField>
  
  <asp:TemplateField HeaderText="Status" ItemStyle-Width="60px" ItemStyle-HorizontalAlign="Center">
  <ItemTemplate>
  <asp:Label ID="lblStatus" runat="server" Text='<%#Eval("status") %>' Width="60px" Font-Size="8pt"></asp:Label>  
  </ItemTemplate>
  <EditItemTemplate>
  <asp:TextBox SkinID="tbGray" ID="txtStatus" runat="server" Text='<%#Eval("status") %>' Width="60px" Font-Size="8" MaxLength="1"></asp:TextBox>
  </EditItemTemplate>
  </asp:TemplateField>
  
  <asp:TemplateField HeaderText="Desc" ItemStyle-Width="60px" ItemStyle-HorizontalAlign="Center" Visible="false">
  <ItemTemplate>
  <asp:Label ID="lblDesc" runat="server" Text='<%#Eval("seg_coa_desc") %>' Width="60px"></asp:Label>  
  </ItemTemplate>
  </asp:TemplateField>  
  </Columns>
                        <RowStyle BackColor="white" />
                        <PagerStyle BackColor="Silver" ForeColor="Blue" HorizontalAlign="Center" />
                        <HeaderStyle BackColor="Silver" Font-Bold="True" ForeColor="Black" />
                        <AlternatingRowStyle BackColor="#F5F5F5" />
  </asp:GridView>
  <br />
</ContentTemplate>
</asp:UpdatePanel>
</td>
<td style="width:1%;"></td>
</tr>
</table>
</div>
</asp:Content>

