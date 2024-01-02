<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="SegCoaSetup.aspx.cs" Inherits="SegCoaSetup" Title="Chart-of-Accounts"  Theme="Themes" EnableEventValidation="false" MaintainScrollPositionOnPostback="true"%>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">  
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
<div id="frmMainDiv" style="background-color:White; width:100%; "> 
<table style="width:100%; font-family:Verdana; font-size:8pt;">
<tr>
<td style="width:1%; height: 4px;" align="center"></td>
<td style="width:98%; height: 4px;" align="left">
</td>
<td style="width:1%; height: 4px;" align="center"></td>
</tr>
<tr>
<td style="width:1%;" align="center"></td>
<td style="width:98%;" align="left">
<asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional">
<ContentTemplate>
<table style="width:100%;">
<tr>
<td colspan="3" style="width:100%;" align="left">
<table style="width:100%;" border="0" cellpadding="0" cellspacing="0" >
<tr>
<td style="width:100%; vertical-align:top;"  align="left">
<asp:GridView  RowStyle-Height="25px" CssClass="mGrid" PagerStyle-CssClass="pgr"  
        AlternatingRowStyle-CssClass="alt" ID="dgLevel" runat="server" 
        AutoGenerateColumns="False" CaptionAlign="Top"
        AllowPaging="True" Width="100%" BackColor="White" BorderWidth="1px" BorderStyle="Solid"
        CellPadding="2" BorderColor="LightGray" Font-Size="8pt" AllowSorting="True" 
        PageSize="5" onrowcancelingedit="dgLevel_RowCancelingEdit" 
        onrowediting="dgLevel_RowEditing" onrowupdating="dgLevel_RowUpdating" 
        onrowcommand="dgLevel_RowCommand" onrowdeleting="dgLevel_RowDeleting" 
        onrowdatabound="dgLevel_RowDataBound" >
  <HeaderStyle Font-Size="10pt" ForeColor="Black" Font-Bold="True" BackColor="LightGray" HorizontalAlign="center" /> 
  <Columns>
  
  <asp:TemplateField ItemStyle-Width="130px" ItemStyle-HorizontalAlign="Center">
  <ItemTemplate>  
  <asp:LinkButton ID="lbAddNew" runat="server" CausesValidation="False" CommandName="AddNew" Text="AddNew" Font-Size="8pt"></asp:LinkButton>
  <asp:LinkButton ID="lbEdit" runat="server" Text="Edit" CausesValidation="false" CommandName="Edit" Font-Size="8pt"/>
  <asp:LinkButton ID="lbDelete" runat="server" CausesValidation="False" CommandName="Delete" Text="Delete" Font-Size="8pt"></asp:LinkButton>  
  <ajaxToolkit:ConfirmButtonExtender ID="detdeleteconfirm" runat="server" ConfirmText="Are you sure to delete??" TargetControlID="lbDelete"></ajaxToolkit:ConfirmButtonExtender>
  </ItemTemplate>
  <EditItemTemplate>
  <asp:LinkButton ID="lbUpdate" runat="server" Text="Update" CausesValidation="false" CommandName="Update" Font-Size="8pt"/>
  <asp:LinkButton ID="lbCancel" runat="server" Text="Cancel" CausesValidation="false" CommandName="Cancel" Font-Size="8pt"/>
  </EditItemTemplate>
  <FooterTemplate>
  <asp:LinkButton ID="lbInsert" runat="server" Text="Insert" CausesValidation="false" CommandName="Insert" Font-Size="8pt"/>
  <asp:LinkButton ID="lbCancel" runat="server" Text="Cancel" CausesValidation="false" CommandName="Cancel" Font-Size="8pt"/>
  </FooterTemplate>
  </asp:TemplateField>
  
  <asp:TemplateField HeaderText="Level Code" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center">
  <ItemTemplate>  
  <asp:Label ID="lblLevelCode" runat="server" Text='<%#Eval("lvl_code") %>' Font-Size="8pt"></asp:Label>     
  </ItemTemplate>
  <EditItemTemplate>  
  <asp:TextBox SkinID="tbGray" ID="txtLevelCode" runat="server" Text='<%#Eval("lvl_code") %>' Width="100px" CssClass="tbc" Font-Size="8pt" MaxLength="2" ></asp:TextBox>   
  </EditItemTemplate>
  <FooterTemplate>
  <asp:TextBox SkinID="tbGray" ID="txtLevelCode" runat="server" Text="" Font-Size="8pt" Width="100px" CssClass="tbc" MaxLength="2" ></asp:TextBox>   
  </FooterTemplate>
  </asp:TemplateField>
  
  <asp:TemplateField HeaderText="Description" ItemStyle-Width="150px" ItemStyle-HorizontalAlign="Left">
  <ItemTemplate>
  <asp:Label ID="lblLevelDesc" runat="server" Text='<%#Eval("lvl_desc") %>' Font-Size="8pt"></asp:Label>  
  </ItemTemplate>
  <EditItemTemplate>  
  <asp:TextBox SkinID="tbGray" ID="txtLevelDesc" runat="server" Text='<%#Eval("lvl_Desc") %>' Width="150px" CssClass="tbl" Font-Size="8pt" MaxLength="50" ></asp:TextBox>   
  </EditItemTemplate>
  <FooterTemplate>
  <asp:TextBox SkinID="tbGray" ID="txtLevelDesc" runat="server" Text="" Font-Size="8pt" MaxLength="50"  CssClass="tbc"  Width="150px"></asp:TextBox>   
  </FooterTemplate>
  </asp:TemplateField>  
  
  <asp:TemplateField HeaderText="Length" ItemStyle-Width="80px" ItemStyle-HorizontalAlign="Center">
  <ItemTemplate>  
  <asp:Label ID="lblLevelMaxSize" runat="server" Text='<%#Eval("lvl_max_size") %>' Font-Size="8pt"></asp:Label>     
  </ItemTemplate>
  <EditItemTemplate>  
  <asp:TextBox SkinID="tbGray" ID="txtLevelMaxSize" runat="server" Text='<%#Eval("lvl_max_size") %>' Width="80px" CssClass="tbc" Font-Size="8pt" MaxLength="1" ></asp:TextBox>   
  </EditItemTemplate>
  <FooterTemplate>
  <asp:TextBox SkinID="tbGray" ID="txtLevelMaxSize" runat="server" Text="" Font-Size="8pt" MaxLength="1"  Width="80px" CssClass="tbc"></asp:TextBox>   
  </FooterTemplate>
  </asp:TemplateField>
  
  <asp:TemplateField HeaderText="Enabled" ItemStyle-Width="80px" ItemStyle-HorizontalAlign="Center">
  <ItemTemplate>  
  <asp:Label ID="lblLevelEnabled" runat="server" Text='<%#Eval("lvl_enabled") %>' Font-Size="8pt"></asp:Label>     
  </ItemTemplate>
  <EditItemTemplate>  
  <asp:DropDownList SkinID="ddlPlain" ID="ddlLevelEnabled" runat="server" Font-Size="8pt" Width="80px" CssClass="tbc" Height="18px"  >
    <asp:ListItem></asp:ListItem>
    <asp:ListItem Text="Yes" Value="Y"></asp:ListItem>
    <asp:ListItem Text="No" Value="N"></asp:ListItem>
    </asp:DropDownList>  
  </EditItemTemplate>
  <FooterTemplate>
  <asp:DropDownList SkinID="ddlPlain" ID="ddlLevelEnabled" runat="server" Font-Size="8pt" Width="80px" CssClass="tbc" Height="18px"  >
    <asp:ListItem></asp:ListItem>
    <asp:ListItem Text="Yes" Value="Y"></asp:ListItem>
    <asp:ListItem Text="No" Value="N"></asp:ListItem>
    </asp:DropDownList>
  </FooterTemplate>
  </asp:TemplateField>
  
  <asp:TemplateField HeaderText="Type" ItemStyle-Width="120px" ItemStyle-HorizontalAlign="Center">
  <ItemTemplate>  
  <asp:Label ID="lblLevelSegType" runat="server" Text='<%#Eval("lvl_seg_type") %>' Font-Size="8pt"></asp:Label>     
  </ItemTemplate>
  <EditItemTemplate>  
  <asp:DropDownList SkinID="ddlPlain" ID="ddlLevelSegType" runat="server" Font-Size="8pt" Width="120px" Height="18px"  >
    <asp:ListItem></asp:ListItem>
    <asp:ListItem Text="Natural Segments" Value="N"></asp:ListItem>
    <asp:ListItem Text="Cost Center" Value="X"></asp:ListItem>
    <asp:ListItem Text="Balance Segments" Value="B"></asp:ListItem>
    </asp:DropDownList>   
  </EditItemTemplate>
  <FooterTemplate>
  <asp:DropDownList SkinID="ddlPlain" ID="ddlLevelSegType" runat="server" Font-Size="8pt" Width="120px" Height="18px"  >
    <asp:ListItem></asp:ListItem>
    <asp:ListItem Text="Natural Segments" Value="N"></asp:ListItem>
    <asp:ListItem Text="Cost Center" Value="X"></asp:ListItem>
    <asp:ListItem Text="Balance Segments" Value="B"></asp:ListItem>
    </asp:DropDownList>
  </FooterTemplate>
  </asp:TemplateField>
  
  <asp:TemplateField HeaderText="Order" ItemStyle-Width="70px" ItemStyle-HorizontalAlign="Center">
  <ItemTemplate>  
  <asp:Label ID="lblLevelOrder" runat="server" Text='<%#Eval("lvl_order") %>' Font-Size="8pt"></asp:Label>     
  </ItemTemplate>
  <EditItemTemplate>  
  <asp:TextBox SkinID="tbGray" ID="txtLevelOrder" runat="server" Text='<%#Eval("lvl_order") %>' Width="70px" CssClass="tbc" Font-Size="8pt" MaxLength="2" ></asp:TextBox>   
  </EditItemTemplate>
  <FooterTemplate>
  <asp:TextBox SkinID="tbGray" ID="txtLevelOrder" runat="server" Text="" Font-Size="8pt" Width="70px" CssClass="tbc" MaxLength="2" ></asp:TextBox>   
  </FooterTemplate>
  </asp:TemplateField>
  
  </Columns>
                        <RowStyle BackColor="white" />
                        <EditRowStyle BackColor="" />
                        <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                        <HeaderStyle BackColor="Silver" Font-Bold="True" ForeColor="Black" />
                        <AlternatingRowStyle BackColor="#F5F5F5" />
</asp:GridView>
 
</td>
</tr>
</table>
</td>
</tr>
<tr><td style="height:10px; line-height:normal; vertical-align:baseline;" colspan="3"></td></tr>
<tr>
<td style="width:38%;" valign="top" align="left">
<asp:Panel ID="pnlTreeView" runat="server" Width="400px" ScrollBars="Auto" Height="450px" HorizontalAlign="Left">
   <asp:TreeView ID="TreeView1" runat="server" AutoGenerateDataBindings="False" Width="100%"
             onselectednodechanged="TreeView1_SelectedNodeChanged" ImageSet="Msdn"
        ForeColor="Blue" ParentNodeStyle-ForeColor="Green" Font-Bold="True" 
        Font-Size="12pt">           
            <SelectedNodeStyle Font-Size="Medium" Font-Underline="False" HorizontalPadding="3px" 
                VerticalPadding="2px" BackColor="White" BorderColor="#888888" 
                BorderStyle="Solid" BorderWidth="1px" />            
            <NodeStyle Font-Size="8pt" ForeColor="Black" HorizontalPadding="2px" 
                NodeSpacing="2px" VerticalPadding="3px" Font-Names="Verdana" />
   </asp:TreeView>
</asp:Panel>
</td>
<td style="width:2%; border-right:solid 1px gray;" align="center">
</td>
<td style="width:60%; vertical-align:top;" align="left">
<table style="vertical-align:top; width:100%;">
    <tr>
        <td align="center" colspan="5" 
            style="width:100%; vertical-align:top; font-size:10pt; color:Maroon;">
             <fieldset style="vertical-align: top; border: solid 1px #8BB381; text-align:left;">
                                <legend style="color: maroon;"><b>Search Option</b></legend>
            <table style="width: 100%">
                <tr>
                    <td style="width: 34%; height: 30px;" align="left">
                        <asp:Label ID="lblSegCode0" runat="server" Font-Size="8pt" 
                            style="font-weight: 700">Search Code / Description</asp:Label>
                    </td>
                    <td style="width: 5%; font-weight: 700; height: 30px;" align="center">
                        :</td>
                    <td style="width: 80%; height: 30px;" align="left">
                        <asp:TextBox ID="txtSearchSegCoa" runat="server" AutoPostBack="True" 
                            Font-Size="8pt" MaxLength="150" SkinID="tbGray" Width="85%" 
                            ontextchanged="txtSearchSegCoa_TextChanged"></asp:TextBox>
                             <ajaxtoolkit:AutoCompleteExtender ID="AutoCompleteExtender2"
                                                        runat="server" CompletionInterval="20" CompletionSetCount="30"
                                                        EnableCaching="true" MinimumPrefixLength="2"
                                                        ServiceMethod="GetSearchGlCoa" ServicePath="~/AutoComplete.asmx"
                                                        TargetControlID="txtSearchSegCoa">
                                                    </ajaxtoolkit:AutoCompleteExtender>
                    </td>
                </tr>
            </table>
            </fieldset>
        </td>
    <tr>
        <td align="center" 
            style="width: 100%; vertical-align: top; font-size: 10pt; color: Maroon; font-weight: 700;" 
            colspan="5">
            Step:2 - Segment Code Setup</td>
        <tr>
            <td align="left" style="width: 20%;">
                <asp:Label ID="lblSegCode" runat="server" Font-Size="8pt">Segment Code</asp:Label>
            </td>
            <td align="left" style="width: 28%;">
                <asp:TextBox ID="txtSegCode" runat="server" AutoPostBack="False" 
                    Font-Size="8pt" MaxLength="7" SkinID="tbGray" Width="100%"></asp:TextBox>
            </td>
            <td style="width:4%;">
            </td>
            <td align="left" style="width: 20%;">
                <asp:Label ID="lblLvlCode" runat="server" Font-Size="8pt" Width="100%">Level</asp:Label>
            </td>
            <td align="left" style="width: 28%;">
                <%--<asp:TextBox ID="txtLvlCode" runat="server" AutoPostBack="False" 
                Font-Size="8pt" MaxLength="2" SkinID="tbGray"  Width="94%"></asp:TextBox>--%>
                <asp:DropDownList ID="ddlLvlcode" runat="server" AppendDataBoundItems="True" 
                    DataTextField="LVL_DESC" DataValueField="LVL_CODE" Font-Size="8pt" 
                    SkinID="ddlPlain" Width="100%">
                    <asp:ListItem Value="0">---Select---</asp:ListItem>
                </asp:DropDownList>
                <%--            <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:cinConnectionString %>" SelectCommand="SELECT [LVL_CODE], [LVL_DESC] FROM [GL_level_type]"></asp:SqlDataSource>
--%>
            </td>
        </tr>
        <tr>
            <td align="left" style="width: 20%;">
                <asp:Label ID="lblSegDesc" runat="server" Font-Size="8pt" Width="100%">Description</asp:Label>
            </td>
            <td align="left" colspan="3">
                <asp:TextBox ID="txtSegDesc" runat="server" AutoPostBack="False" 
                    Font-Size="8pt" MaxLength="150" SkinID="tbGray" Width="85%"></asp:TextBox>
            </td>
            <td align="left" style="width: 28%;">
                <asp:CheckBox ID="CheckBox1" runat="server" AutoPostBack="True" 
                    oncheckedchanged="CheckBox1_CheckedChanged" style="display:none" 
                    Text="Client" />
                &nbsp;&nbsp;&nbsp; &nbsp;<asp:CheckBox ID="CheckBox2" runat="server" AutoPostBack="True" 
                    oncheckedchanged="CheckBox2_CheckedChanged" style="display:none" Text="Bank" />
            </td>
        </tr>
        <tr>
            <td align="left" style="width: 20%;">
                <asp:Label ID="lblClient" runat="server" Font-Size="8pt" Width="100%">Client 
                Name</asp:Label>
            </td>
            <td align="left" colspan="4" style="width: 80%;">
                <asp:DropDownList ID="ddlClientName" runat="server" AutoPostBack="True" 
                    Font-Size="8pt" onselectedindexchanged="ddlClientName_SelectedIndexChanged" 
                    SkinID="ddlPlain" Width="100%">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td align="left" style="width: 20%;">
                <asp:Label ID="lblParentCode" runat="server" Font-Size="8pt">Parent Code</asp:Label>
            </td>
            <td align="left" style="width: 28%;">
                <asp:TextBox ID="txtParentCode" runat="server" AutoPostBack="False" 
                    Font-Size="8pt" MaxLength="7" SkinID="tbGray" Width="100%"></asp:TextBox>
            </td>
            <td style=" width:4%;">
            </td>
            <td align="left" style="width: 20%;">
                <asp:Label ID="lblBudAllowed" runat="server" Font-Size="8pt">Budget Allowed</asp:Label>
            </td>
            <td align="left" style="width: 28%;">
                <asp:DropDownList ID="ddlBudAllowed" runat="server" AutoPostBack="False" 
                    Font-Size="8pt" SkinID="ddlPlain" Width="99%">
                    <asp:ListItem Text="Yes" Value="Y"></asp:ListItem>
                    <asp:ListItem Text="No" Value="N"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td align="left" style="width: 20%;">
                <asp:Label ID="lblPostAllowed" runat="server" Font-Size="8pt">Post Allowed</asp:Label>
            </td>
            <td align="left" style="width: 28%;">
                <asp:DropDownList ID="ddlPostAllowed" runat="server" AutoPostBack="False" 
                    Font-Size="8pt" SkinID="ddlPlain" Width="105%">
                    <asp:ListItem Text="Yes" Value="Y"></asp:ListItem>
                    <asp:ListItem Text="No" Value="N"></asp:ListItem>
                </asp:DropDownList>
            </td>
            <td style=" width:4%;">
            </td>
            <td align="left" style="width: 20%;">
                <asp:Label ID="lblAccType" runat="server" Font-Size="8pt">Account Type</asp:Label>
            </td>
            <td align="left" style="width: 28%;">
                <asp:DropDownList ID="ddlAccType" runat="server" AutoPostBack="False" 
                    Font-Size="8pt" SkinID="ddlPlain" Width="99%">
                    <asp:ListItem></asp:ListItem>
                    <asp:ListItem Text="Asset" Value="A"></asp:ListItem>
                    <asp:ListItem Text="Liability" Value="L"></asp:ListItem>
                    <asp:ListItem Text="Income" Value="I"></asp:ListItem>
                    <asp:ListItem Text="Expense" Value="E"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td align="left" style="width: 20%;">
                <asp:Label ID="lblOpenDate" runat="server" Font-Size="8pt">Open Date</asp:Label>
            </td>
            <td align="left" style="width: 28%;">
                <asp:TextBox ID="txtOpenDate" runat="server" AutoPostBack="False" 
                    Font-Size="8pt" MaxLength="11" SkinID="tbGray" Width="100%"></asp:TextBox>
                <ajaxToolkit:CalendarExtender ID="Calendarextender2" runat="server" 
                    Format="dd/MM/yyyy" TargetControlID="txtOpenDate" />
            </td>
            <td style="width:4%;">
            </td>
            <td align="left" style="width: 20%;">
                <asp:Label ID="lblRootLeaf" runat="server" Font-Size="8pt">Root/Leaf?</asp:Label>
            </td>
            <td align="left" style="width: 28%;">
                <asp:DropDownList ID="ddlRootLeaf" runat="server" AutoPostBack="False" 
                    Font-Size="8pt" SkinID="ddlPlain" Width="99%">
                    <asp:ListItem Text="Root" Value="R"></asp:ListItem>
                    <asp:ListItem Text="Leaf" Value="L"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td align="left" style="width: 20%;">
                <asp:Label ID="lblTaxable" runat="server" Font-Size="8pt">Taxable</asp:Label>
            </td>
            <td align="left" style="width: 28%;">
                <asp:DropDownList ID="ddlTaxable" runat="server" AutoPostBack="False" 
                    Font-Size="8pt" SkinID="ddlPlain" Width="105%">
                    <asp:ListItem Text="Yes" Value="Y"></asp:ListItem>
                    <asp:ListItem Text="No" Value="N"></asp:ListItem>
                </asp:DropDownList>
            </td>
            <td style=" width:4%;">
            </td>
            <td align="left" style="width: 20%;">
                <asp:Label ID="lblStatus" runat="server" Font-Size="8pt">Status</asp:Label>
            </td>
            <td align="left" style="width: 28%;">
                <asp:DropDownList ID="ddlStatus" runat="server" AutoPostBack="False" 
                    Font-Size="8pt" SkinID="ddlPlain" Width="99%">
                    <asp:ListItem Text="Active" Value="A"></asp:ListItem>
                    <asp:ListItem Text="Inactive" Value="U"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td colspan="5" style="width:100%; height:20px; vertical-align:bottom;">
                <table style="width:100%;">
                    <tr>
                        <td align="center" colspan="6">
                            
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td style="width:100px;">
                            &nbsp;</td>
                        <td align="center">
                            <asp:Button ID="btnDelete" runat="server" Height="35px" 
                                onclick="btnDelete_Click" 
                                onclientclick="javascript:return window.confirm('are u really want to delete these data')" 
                                Text="Delete" ToolTip="Delete" Width="100px" />
                        </td>
                        <td style="width:20px;">
                        </td>
                        <td align="center">
                            <asp:Button ID="btnSave" runat="server" Height="35px" onclick="btnSave_Click" 
                                Text="Save" ToolTip="Save" Width="100px" />
                        </td>
                        <td style="width:20px;">
                            <asp:Button ID="btnClear" runat="server" Height="35px" onclick="btnClear_Click" 
                                Text="Clear" ToolTip="Clear" Width="100px" />
                        </td>
                        <td align="center">
                            &nbsp;</td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center" colspan="5" 
                style="width:100%; height:23px; vertical-align:top;">
                <asp:Label ID="lblTransStatus" runat="server" Font-Size="8pt" Text=""></asp:Label>
                <asp:UpdateProgress ID="updateProgress" runat="server" 
                    AssociatedUpdatePanelID="UpdatePanel3">
                    <progresstemplate>
                        <div ID="progressBackgroundFilter">
                        </div>
                        <div ID="processMessage">
                            <img src="img/loading.gif" alt="" />
                        </div>
                    </progresstemplate>
                </asp:UpdateProgress>
            </td>
        </tr>
        <tr>
            <td colspan="5" style="width:100%; height:150px; vertical-align:bottom;">
                <table style="width:100%;">
                    <tr>
                        <td align="center" colspan="5" 
                            
                            style="width:100%; vertical-align:bottom; font-size:10pt; color:Maroon; font-weight: 700;">
                            Step:3 - Chart-of-Account Code Generation</td>
                    </tr>
                    <tr>
                        <td colspan="5" style="width:100%; height:20px; vertical-align:bottom;">
                            <asp:GridView ID="dgGlCoaGen" runat="server" AllowPaging="True" 
                                AllowSorting="true" AlternatingRowStyle-CssClass="alt" 
                                AutoGenerateColumns="false" BackColor="White" BorderColor="LightGray" 
                                BorderStyle="Solid" BorderWidth="1px" CellPadding="2" CellSpacing="0" 
                                CssClass="mGrid" Font-Size="8pt" PagerStyle-CssClass="pgr" PageSize="6" 
                                RowStyle-Height="25px" ShowHeader="false" Width="100%">
                                <Columns>
                                    <asp:BoundField DataField="lvl_desc" ItemStyle-BackColor="LightGray" 
                                        ItemStyle-Font-Bold="true" ItemStyle-Height="18px" 
                                        ItemStyle-HorizontalAlign="Left" ItemStyle-Width="120px" />
                                    <asp:BoundField DataField="seg_code" ItemStyle-HorizontalAlign="Left" 
                                        ItemStyle-Width="40px" />
                                    <asp:BoundField DataField="seg_desc" ItemStyle-HorizontalAlign="Left" 
                                        ItemStyle-Width="140px" />
                                </Columns>
                            </asp:GridView>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="5" style="width:100%; height:20px; vertical-align:bottom;">
                            <table style="width:100%; text-align:center;">
                                <tr>
                                    <td style="width:100px;">
                                        <asp:Button ID="btnShowCoa" runat="server" Height="35px" 
                                            ImageUrl="~/img/show.jpg" onclick="btnShowCoa_Click" Text="Show COA" 
                                            ToolTip="Show COA" Width="100px" />
                                    </td>
                                    <td style="width:100px;">
                                        <asp:Button ID="btnGenCoa" runat="server" Height="35px" 
                                            ImageUrl="~/img/generate.jpg" onclick="btnGenCoa_Click" Text="Generate COA" 
                                            ToolTip="Geneate COA" Width="100px" />
                                    </td>
                                    <td style="width:20px;">
                                    </td>
                                    <td align="center" style="width:100px;">
                                        <asp:Button ID="btnRefresh" runat="server" Height="35px" 
                                            ImageUrl="~/img/generate.jpg" onclick="btnRefresh_Click" Text="Refresh" 
                                            ToolTip="Refresh" Width="100px" />
                                    </td>
                                    <td style="width:20px;">
                                    </td>
                                    <td style="width:100px;">
                                        <asp:Button ID="btnPrint" runat="server" onclick="btnPrint_Click" 
                                            Text="Print (COA)" Height="35px" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </tr>
    </table>
 
</td>
</tr>
<tr><td style="height:10px;" colspan="3">
              <asp:Panel ID="pnlChangePass" runat="server"  CssClass="modalPopup" 
                            Style="display: none; background-color: White; width:500px; height:auto; ">
                            <fieldset style=" text-align:left; vertical-align: top; border: solid 1px #8BB381;">
                                <legend style="color: maroon;"><b>Information Message</b></legend>
                                <table style="width: 100%">
                                    <tr>
                                        <td style="width: 72%;" align="right">
                                            <asp:Label ID="Message" runat="server" 
                                                Text="This Segment Code alrady exist ...!!!" Font-Bold="True" 
                                                ForeColor="#FF3300"></asp:Label>
                                        </td>
                                        <td style="width: 10%;">
                                            &nbsp;</td>
                                        <td style="width: 10%;">
                                            &nbsp;</td>
                                    </tr>
                                    <tr>
                                        <td align="right" style="width: 72%;">
                                            <asp:Label ID="Label2" runat="server" Text="Do you want to Update ?????" 
                                                Font-Bold="True" ForeColor="#336600"></asp:Label>
                                        </td>
                                        <td style="width: 10%;">
                                            &nbsp;</td>
                                        <td style="width: 10%;">
                                            &nbsp;</td>
                                    </tr>
                                    <tr>
                                        <td style="width: 72%;">
                                            &nbsp;</td>
                                        <td align="center" style="width: 10%;">
                                            <asp:LinkButton ID="lbYes" runat="server" Font-Bold="True" ForeColor="#CC3300" 
                                                onclick="lbYes_Click" Width="100%" Font-Underline="True">YES</asp:LinkButton>
                                        </td>
                                        <td align="center" style="width: 10%;">
                                            <asp:LinkButton ID="lbNo" runat="server" Font-Bold="True" onclick="lbNo_Click" 
                                                Width="100%" Font-Underline="True">NO</asp:LinkButton>
                                        </td>
                                    </tr>
                                </table>
                                <br/>
                            </fieldset>
                        </asp:Panel>
                        <%--</ContentTemplate></asp:UpdatePanel>--%>
                         <asp:Button ID="Button1" runat="server" Height="50px" Width="50px" Style="display: none"  />
                         <ajaxToolkit:ModalPopupExtender ID="ModalPopupExtenderLogin" runat="server" 
                    BackgroundCssClass="modalBackground" DropShadow="true" 
                    PopupControlID="pnlChangePass" TargetControlID="Button1" />

    </td>
    </tr>
    <tr>
        <td colspan="3" style="height:10px;">
            &nbsp;</td>
    </tr>
<tr><td style="height:10px;" colspan="3" align="center">
<asp:Button ID="btnSaveCoa" runat="server" Text="Save COA" Visible="false" 
        SkinID="lbPlain" onclick="btnSaveCoa_Click" Width="100px" Height="35px"></asp:Button>
</td></tr>
<tr>
<td  colspan="3" style="width:100%;" align="center">
<asp:GridView  RowStyle-Height="25px" CssClass="mGrid" PagerStyle-CssClass="pgr"  
        AlternatingRowStyle-CssClass="alt" ID="dgGlCoa" runat="server" AutoGenerateColumns="False" 
        Caption="Chart-of-Account Code" CaptionAlign="Top"
        AllowPaging="True" Width="100%" BackColor="White" BorderWidth="1px" BorderStyle="Solid"
        CellPadding="2" BorderColor="LightGray" Font-Size="8pt" 
        AllowSorting="True" PageSize="20" 
        onpageindexchanging="dgGlCoa_PageIndexChanging" 
        onrowcancelingedit="dgGlCoa_RowCancelingEdit" 
        onrowdeleting="dgGlCoa_RowDeleting" onrowediting="dgGlCoa_RowEditing" 
        onrowupdating="dgGlCoa_RowUpdating" onrowdatabound="dgGlCoa_RowDataBound">
  <HeaderStyle Font-Size="9" Font-Names="Arial" Font-Bold="True" BackColor="Blue" HorizontalAlign="center" ForeColor="White"/>

  <Columns>
  <asp:TemplateField ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center">
  <ItemTemplate>  
  <asp:CheckBox ID="chkInc" runat="server" Checked="true" Visible="false" AutoPostBack="true" OnCheckedChanged="chkIncCheck_Changed" />
  <asp:LinkButton ID="lbEdit" runat="server" Text="Edit" CausesValidation="false" CommandName="Edit"/>
  <asp:LinkButton ID="lbDelete" runat="server" CausesValidation="False" CommandName="Delete" Text="Delete"></asp:LinkButton>
  <ajaxToolkit:ConfirmButtonExtender ID="detdeleteconfirm" runat="server" ConfirmText="Are you sure to delete??" TargetControlID="lbDelete"></ajaxToolkit:ConfirmButtonExtender>
  </ItemTemplate>
  <EditItemTemplate>
  <asp:LinkButton ID="lbUpdate" runat="server" Text="Update" CausesValidation="false" CommandName="Update" />
  <asp:LinkButton ID="lbCancel" runat="server" Text="Cancel" CausesValidation="false" CommandName="Cancel"/>
  </EditItemTemplate>
      <ItemStyle HorizontalAlign="Center" Width="100px" />
  </asp:TemplateField>
   
  <asp:TemplateField HeaderText="COA Code" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center" ItemStyle-Height="30px">
  <ItemTemplate>
  <asp:Label ID="lblGlCoaCode" runat="server" Text='<%#Eval("gl_coa_code") %>' Width="100px" Font-Size="8pt"></asp:Label>  
  </ItemTemplate>
  <EditItemTemplate>  
  <asp:TextBox SkinID="tbGray" ID="txtGlCoaCode" runat="server" Text='<%#Eval("gl_coa_code") %>' Width="100px" Font-Size="8" MaxLength="13"></asp:TextBox>   
  </EditItemTemplate>
      <ItemStyle Height="30px" HorizontalAlign="Center" Width="100px" />
  </asp:TemplateField>
  
  <asp:TemplateField HeaderText="COA Code" ItemStyle-Width="420px" ItemStyle-HorizontalAlign="Left">
  <ItemTemplate>
  <asp:Label ID="lblGlCoaDesc" runat="server" Text='<%#Eval("coa_desc") %>' Font-Size="8pt"></asp:Label>  
  </ItemTemplate>
  <EditItemTemplate>  
  <asp:TextBox SkinID="tbGray" ID="txtGlCoaDesc" runat="server" Text='<%#Eval("coa_desc") %>' Width="320px" Font-Size="8pt" MaxLength="150"></asp:TextBox>   
  </EditItemTemplate>
      <ItemStyle HorizontalAlign="Left" Width="420px" />
  </asp:TemplateField>
  
  <asp:TemplateField HeaderText="Account Type" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center">
  <ItemTemplate>
  <asp:Label ID="lblGlAccType" runat="server" Text='<%#Eval("acc_type") %>' Width="100px" Font-Size="8pt"></asp:Label>  
  </ItemTemplate>
  <EditItemTemplate>  
  <asp:DropDownList SkinID="ddlPlain" ID="ddlGlAccType" runat="server" Width="100px"  AutoPostBack="False" Font-Size="8">
    <asp:ListItem Text="Asset" Value="A"></asp:ListItem>
    <asp:ListItem Text="Liability" Value="L"></asp:ListItem>
    <asp:ListItem Text="Income" Value="I"></asp:ListItem>
    <asp:ListItem Text="Expense" Value="E"></asp:ListItem>
    </asp:DropDownList>   
  </EditItemTemplate>
      <ItemStyle HorizontalAlign="Center" Width="100px" />
  </asp:TemplateField>
  
  <asp:TemplateField HeaderText="Status" ItemStyle-Width="80px" ItemStyle-HorizontalAlign="Center">
  <ItemTemplate>
  <asp:Label ID="lblGlStatus" runat="server" Text='<%#Eval("status") %>' Width="80px" Font-Size="8pt"></asp:Label>  
  </ItemTemplate>
  <EditItemTemplate>  
  <asp:DropDownList SkinID="ddlPlain" ID="ddlGlStatus" runat="server" Width="80px"  AutoPostBack="False" Font-Size="8">
    <asp:ListItem Text="Active" Value="A"></asp:ListItem>
    <asp:ListItem Text="Inactive" Value="U"></asp:ListItem>
    </asp:DropDownList>   
  </EditItemTemplate>
      <ItemStyle HorizontalAlign="Center" Width="80px" />
  </asp:TemplateField>  
  <asp:BoundField HeaderText="Natural Code" ItemStyle-Width="70px" 
          DataField="coa_natural_code"  ItemStyle-HorizontalAlign="Center" 
          ItemStyle-Height="15px">  
      <ItemStyle Height="15px" HorizontalAlign="Center" Width="70px" />
      </asp:BoundField>
      <asp:TemplateField HeaderText="Op.Balance">
      <ItemTemplate>
      <asp:Label ID="lblOPbalance" runat="server" Text='<%#Eval("opening_balance") %>' Width="80px" Font-Size="8pt"></asp:Label>  
      </ItemTemplate>
      <EditItemTemplate>  
        <asp:TextBox ID="txtOpBalance" runat="server" style="text-align:right;"  Font-Size="8"></asp:TextBox>
      </EditItemTemplate>
      <ItemStyle HorizontalAlign="Center" Width="80px" />
      </asp:TemplateField>
  </Columns>
                        <RowStyle BackColor="white" />
                        <EditRowStyle BackColor="" />
                        <PagerStyle BackColor="" HorizontalAlign="Center" />
                        <HeaderStyle BackColor="" Font-Bold="True" ForeColor="Black" />
                        <AlternatingRowStyle BackColor="#F5F5F5" />
</asp:GridView>
</td>
</tr>
</table>
</ContentTemplate>
</asp:UpdatePanel>  
</td>
<td style="width:1%;" align="center"></td>
</tr>
</table> 
</div> 

<div id="divBackground" style="position: fixed; z-index: 999; height: 100%; width: 100%;
        top: 0; left:0; background-color: Black; filter: alpha(opacity=60); opacity: 0.6; -moz-opacity: 0.8;-webkit-opacity: 0.8;display:none">
    </div>

<script language="javascript" type="text/javascript">
    function disposeTree(sender, args) {
        var elements = args.get_panelsUpdating();
        for (var i = elements.length - 1; i >= 0; i--) {
            var element = elements[i];
            var allnodes = element.getElementsByTagName('*'),
                length = allnodes.length;
            var nodes = new Array(length)
            for (var k = 0; k < length; k++) {
                nodes[k] = allnodes[k];
            }
            for (var j = 0, l = nodes.length; j < l; j++) {
                var node = nodes[j];
                if (node.nodeType === 1) {
                    if (node.dispose && typeof (node.dispose) === "function") {
                        node.dispose();
                    }
                    else if (node.control && typeof (node.control.dispose) === "function") {
                        node.control.dispose();
                    }

                    var behaviors = node._behaviors;
                    if (behaviors) {
                        behaviors = Array.apply(null, behaviors);
                        for (var k = behaviors.length - 1; k >= 0; k--) {
                            behaviors[k].dispose();
                        }
                    }
                }
            }
            element.innerHTML = "";
        }
    }
    Sys.WebForms.PageRequestManager.getInstance().add_pageLoading(disposeTree);
</script>


</asp:Content>

