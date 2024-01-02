<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="UserGroupAndPrivilege.aspx.cs" Inherits="UserGroupAndPrivilege" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <div id="frmMainDiv" style="background-color:White; width:100%;">
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
<ajaxToolkit:TabContainer ID="tbcUserGroup" runat="server" Width="100%" ActiveTabIndex="1">
 <ajaxToolkit:tabpanel ID="TabPanel6" runat="server" HeaderText="User Group">
    <ContentTemplate>
        <div></div>        
<div>           

<table style="width:100%;">                    
<tr>
 <td style="width:20%;">       

</td>
   <td style="width:60%;"></td>    
   <td style="width:20%;"></td>                   
</tr>
    <tr>
        <td style="width:20%; height: 81px;">
            </td>
        <td style="width:60%; height: 81px;">
        <fieldset style=" text-align:left; vertical-align: top; border: solid 1px #8BB381;"><legend style="color: maroon;"><b>
                                USER GROUP</b></legend>
            <table style="width: 100%">
                <tr>
                    <td style="width: 35%; height: 28px;" align="right">
                        Group ID</td>
                    <td style="width: 3%; height: 28px;">
                        </td>
                    <td style="width: 100%; height: 28px;" align="left">
                        <asp:TextBox ID="txtGroupID" runat="server" CssClass="txtVisible" 
                            style="text-align:left;" Width="20%"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td align="right" style="width: 35%; height: 26px;">
                        <strong>Group Name</strong></td>
                    <td style="width: 3%; height: 26px;">
                        </td>
                    <td align="left" style="width: 100%; font-weight: 700; height: 26px;">
                        <asp:TextBox ID="txtGroupName" runat="server" CssClass="tbc" 
                            style="text-align:left;" Width="98%"></asp:TextBox>
                    </td>
                </tr>
            </table></fieldset>
        </td>
        <td style="width:20%; height: 81px;">
            </td>
    </tr>
    <tr>
        <td style="width:20%;">
            &nbsp;</td>
        <td style="width:50%;" align="center">
            <table style="width: 100%">
                <tr>
                    <td style="width:10%;">
                        &nbsp;</td>
                    <td style="width:10%;">
                        <asp:Button ID="btnDelete" runat="server" Height="35px" 
                            OnClick="btnDelete_Click" Text="Delete" Width="120px" />
                    </td>
                    <td style="width:10%;">
                        <asp:Button ID="btnSave" runat="server" Height="35px" OnClick="btnSave_Click" 
                            Text="Save" Width="120px" />
                    </td>
                    <td style="width:10%;">
                        <asp:Button ID="btnClear" runat="server" Height="35px" OnClick="btnClear_Click" 
                            Text="Clear" Width="120px" />
                    </td>
                    <td style="width:10%;">
                        &nbsp;</td>
                </tr>
            </table>
        </td>
        <td style="width:20%;">
            &nbsp;</td>
    </tr>
    <tr>
        <td style="width:20%;">
            &nbsp;</td>
        <td align="center" style="width:50%;">
            <asp:GridView ID="dgUserGroup" runat="server" AllowPaging="True" AllowSorting="True" 
                AutoGenerateColumns="False" BackColor="White" BorderColor="LightGray" 
                BorderStyle="Solid" BorderWidth="1px" CellPadding="2" CssClass="mGrid" 
                Font-Size="8pt" ForeColor="#333333" PageSize="40" Width="100%" 
                onrowdatabound="dgUserGroup_RowDataBound" 
                onselectedindexchanged="dgUserGroup_SelectedIndexChanged">
                <AlternatingRowStyle CssClass="alt" />
                <Columns>
                    <asp:CommandField ShowSelectButton="True">
                    <ItemStyle ForeColor="DeepSkyBlue" Height="25px" HorizontalAlign="Center" 
                        Width="7%" />
                    </asp:CommandField>
                    <asp:BoundField DataField="GROUP_DESC" HeaderText="User Group">
                    <ItemStyle HorizontalAlign="Left" Width="100%" />
                    </asp:BoundField>
                    <asp:BoundField DataField="USER_GRP" HeaderText="ID" />
                </Columns>
                <HeaderStyle Font-Bold="True" ForeColor="Black" HorizontalAlign="Center" />
                <PagerStyle CssClass="pgr" HorizontalAlign="Center" />
                <RowStyle BackColor="White" />
            </asp:GridView>
        </td>
        <td style="width:20%;">
            &nbsp;</td>
    </tr>
</table>           
</div> 
</ContentTemplate>    
</ajaxToolkit:tabpanel>
 <ajaxToolkit:tabpanel ID="TabPanel7" runat="server" HeaderText="Group Privilege">
    <ContentTemplate>
        
 <table style="width:100%; color:Black;" >
<tr>
<td style="width:1%;">&nbsp;</td>
<td style="width:98%;" align="center">
    <table style="width:100%;">
        <tr>
            <td align="left" style="width: 20%;">
            </td>
            <td align="center" style="width: 60%;">
                &nbsp;&nbsp;<asp:LinkButton ID="lbDelete0" runat="server" Font-Bold="True" 
                    Font-Size="18pt" ForeColor="Navy" onclick="lbDelete1_Click" Text="Delete"></asp:LinkButton>
                &nbsp; |&nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lbSave" runat="server" Font-Bold="True" Font-Size="18pt" 
                    ForeColor="Navy" OnClick="lbSave_Click" Text="Save"></asp:LinkButton>
                &nbsp; &nbsp;|&nbsp;
                <asp:LinkButton ID="lbClear" runat="server" Font-Bold="True" Font-Size="18pt" 
                    ForeColor="Navy" onclick="lbClear_Click" Text="Clear"></asp:LinkButton>
                &nbsp;</td>
            <td align="right" style="width: 20%;">
                &nbsp;</td>
        </tr>
    </table>
</td>
<td style="width:1%;">&nbsp;</td>
</tr>
     <tr>
         <td style="width:1%;">
         </td>
         <td align="center" style="width:98%;">
             <fieldset style="vertical-align: top; border: solid .5px #8BB381;text-align:left;line-height:1.5em;">
                 <legend style="color: maroon;"><b>Group Privilege</b></legend>
                 <asp:UpdatePanel ID="UP1" runat="server" UpdateMode="Conditional">
                     <ContentTemplate>
                         <table style="width:100%; font-size:8pt;">
                             <tr>
                                 <td align="right" colspan="5" style="font-size:8pt;">
                                     <asp:GridView ID="dgUserGroup0" runat="server" AutoGenerateColumns="False" 
                                         BackColor="White" BorderColor="LightGray" BorderStyle="Solid" BorderWidth="1px" 
                                         CellPadding="2" CssClass="mGrid" Font-Size="8pt" ForeColor="#333333" 
                                         OnRowDataBound="dgUserGroup_RowDataBound" 
                                         OnSelectedIndexChanged="dgUserGroup0_SelectedIndexChanged" PageSize="40" 
                                         Width="100%">
                                         <AlternatingRowStyle CssClass="alt" />
                                         <Columns>
                                             <asp:CommandField ShowSelectButton="True">
                                             <ItemStyle ForeColor="DeepSkyBlue" Height="25px" HorizontalAlign="Center" 
                                                 Width="7%" />
                                             </asp:CommandField>
                                             <asp:BoundField DataField="GROUP_DESC" HeaderText="User Group">
                                             <ItemStyle HorizontalAlign="Left" Width="100%" />
                                             </asp:BoundField>
                                             <asp:BoundField DataField="USER_GRP" HeaderText="ID" />
                                         </Columns>
                                         <HeaderStyle Font-Bold="True" ForeColor="Black" HorizontalAlign="Center" />
                                         <PagerStyle CssClass="pgr" HorizontalAlign="Center" />
                                         <RowStyle BackColor="White" />
                                     </asp:GridView>
                                 </td>
                             </tr>
                             <tr>
                                 <td align="right" style="width:15%; font-size:8pt;">
                                     <asp:Label ID="lblGroup" runat="server" style="font-weight: 700" 
                                         Text="Group Name : "></asp:Label>
                                 </td>
                                 <td>
                                     <asp:Label ID="lblGroupName" runat="server" Font-Bold="True" Font-Size="12pt" 
                                         ForeColor="#CC3300"></asp:Label>
                                 </td>
                                 <td align="center" style="height: 27px; width:5%;">
                                     &nbsp;</td>
                                 <td align="right" style="width:15%; font-size:8pt;">
                                     <asp:Label ID="lblGroupID" runat="server" Font-Bold="True" Font-Size="12pt" 
                                         ForeColor="#CC3300" Visible="False"></asp:Label>
                                 </td>
                                 <td style="width:15%;">
                                     &nbsp;</td>
                             </tr>
                         </table>
                     </ContentTemplate>
                 </asp:UpdatePanel>
             </fieldset>
         </td>
         <td style="width:1%;">
         </td>
     </tr>
<tr>
<td style="width:1%;">&nbsp;</td>
<td style="width:98%;" align="center">
<asp:UpdatePanel ID="UP3" runat="server" UpdateMode="Conditional">
<ContentTemplate>
    <table  style="width: 100%;">
        <tr>
            <td style="width: 35%" valign="top">
                <div id="ModelTab1" runat="server">
                    <fieldset style="vertical-align: top; border: solid .5px #8BB381;text-align:left;line-height:1.5em;">
                        <legend style="color: maroon;"><b>Module Name</b></legend>
                        <asp:GridView ID="dgModel" runat="server" AllowSorting="True" 
                            AlternatingRowStyle-CssClass="alt" AutoGenerateColumns="False" 
                            BackColor="White" BorderColor="LightGray" BorderStyle="Solid" BorderWidth="1px" 
                            CellPadding="2" CssClass="mGrid" Font-Size="8pt" PagerStyle-CssClass="pgr" 
                            PageSize="160" Width="100%" onrowcommand="dgModel_RowCommand" 
                            onrowdatabound="dgModel_RowDataBound">
                            <Columns>       
                                <asp:BoundField DataField="MOD_ID" HeaderText="ID">
                                    <ItemStyle HorizontalAlign="Center" Width="5%" />
                                </asp:BoundField>
                                                       
                                <asp:BoundField DataField="DESCRIPTION" HeaderText="Model Name">
                                <ItemStyle HorizontalAlign="Left" Width="65%" />
                                </asp:BoundField>
                                  <asp:TemplateField HeaderText=">>">
                                    <ItemTemplate>
                                       <asp:LinkButton ID="btnAdd" runat="server" CommandName="ADD" onclientclick="javascript:return window.confirm('are u really want to Set this module..!!')" 
                                Font-Bold="True" Text=">>" Font-Size="Large"></asp:LinkButton>
                                    </ItemTemplate>
                                    <ItemStyle Width="10%" />
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
            <td style="width: 65%" valign="top">
                <div id="ModelTab2" runat="server">
                    <fieldset style="vertical-align: top; border: solid .5px #8BB381;text-align:left;line-height:1.5em;">
                        <legend style="color: maroon;"><b>Module Set</b></legend>
                        <asp:GridView ID="dgModelAdd" runat="server" 
                            AlternatingRowStyle-CssClass="alt" AutoGenerateColumns="False" 
                            BackColor="White" BorderColor="LightGray" BorderStyle="Solid" BorderWidth="1px" 
                            CellPadding="2" CssClass="mGrid" Font-Size="8pt" PagerStyle-CssClass="pgr" 
                            PageSize="160" Width="100%" onrowdatabound="dgModelAdd_RowDataBound" 
                            onrowdeleting="dgModelAdd_RowDeleting">
                            <Columns>
                                <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:ImageButton ID="lbDelete" runat="server" CausesValidation="False" 
                                        CommandName="Delete" ImageAlign="Middle" ImageUrl="~/img/delete.png" 
                                        Text="Delete" />
                                </ItemTemplate>
                                <ItemStyle Font-Size="8pt" HorizontalAlign="Center" Width="5%" />
                            </asp:TemplateField>
                                <asp:BoundField DataField="ID" HeaderText="ID">
                                <ItemStyle HorizontalAlign="Center" Width="5%" />
                                </asp:BoundField>
                                <asp:BoundField DataField="ModelName" HeaderText="Model Name">
                                <ItemStyle HorizontalAlign="Left" Width="30%" />
                                </asp:BoundField>
                                 <asp:TemplateField HeaderText="Add">
                                    <ItemTemplate>
                                        <asp:DropDownList SkinID="ddlPlain" ID="ddlAllowAdd" runat="server" 
                                            Width="50px" Font-Size="8pt" SelectedValue='<%# Eval("Add") %>' 
                                            AutoPostBack="True" onselectedindexchanged="ddlAllowAdd_SelectedIndexChanged">
                                            <asp:ListItem Text="Yes" Value="Y"></asp:ListItem> 
                                            <asp:ListItem Text="No" Value="N"></asp:ListItem>
                                         </asp:DropDownList>                                      
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" Width="10%" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Edit" ItemStyle-Width="170px">
                                    <ItemTemplate>
                                        <asp:DropDownList SkinID="ddlPlain" ID="ddlAllowEdit" runat="server" 
                                            Width="50px" Font-Size="8pt" SelectedValue='<%# Eval("Edit") %>' 
                                            AutoPostBack="True" onselectedindexchanged="ddlAllowEdit_SelectedIndexChanged">
                                            <asp:ListItem Text="Yes" Value="Y"></asp:ListItem> 
                                            <asp:ListItem Text="No" Value="N"></asp:ListItem>
                                        </asp:DropDownList>                                        
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" Width="10%" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="View" ItemStyle-Width="120px">
                                    <ItemTemplate>
                                        <asp:DropDownList SkinID="ddlPlain" ID="ddlAllowView" runat="server" 
                                            Width="50px" Font-Size="8pt" SelectedValue='<%# Eval("View") %>' 
                                            AutoPostBack="True" onselectedindexchanged="ddlAllowView_SelectedIndexChanged">
                                            <asp:ListItem Text="Yes" Value="Y"></asp:ListItem> 
                                            <asp:ListItem Text="No" Value="N"></asp:ListItem>
                                        </asp:DropDownList>     
                                    </ItemTemplate>
                                     <ItemStyle HorizontalAlign="Center" Width="10%" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Delete" ItemStyle-Width="120px">
                                    <ItemTemplate>
                                         <asp:DropDownList SkinID="ddlPlain" ID="ddlAllowDelete" runat="server" 
                                             Width="50px" Font-Size="8pt" SelectedValue='<%# Eval("Delete") %>' 
                                             AutoPostBack="True" 
                                             onselectedindexchanged="ddlAllowDelete_SelectedIndexChanged">
                                            <asp:ListItem Text="Yes" Value="Y"></asp:ListItem> 
                                            <asp:ListItem Text="No" Value="N"></asp:ListItem>
                                        </asp:DropDownList> 
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" Width="10%" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Authorize" ItemStyle-Width="50px">
                                <ItemTemplate>
                                    <asp:DropDownList SkinID="ddlPlain" ID="ddlAllowAutho" runat="server" 
                                        Width="50px" Font-Size="8pt" SelectedValue='<%# Eval("Authoriz") %>' 
                                        AutoPostBack="True" onselectedindexchanged="ddlAllowAutho_SelectedIndexChanged">
                                        <asp:ListItem Text="Yes" Value="Y"></asp:ListItem> 
                                        <asp:ListItem Text="No" Value="N"></asp:ListItem>
                                    </asp:DropDownList> 
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" Width="10%" />
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
        </tr>
    </table>
    </ContentTemplate>
    </asp:UpdatePanel>
    </td>
<td style="width:1%;">&nbsp;</td>
</tr>
<tr>
<td style="width:1%;">&nbsp;</td>
<td style="width:98%;" align="center">
    &nbsp;</td>
<td style="width:1%;">&nbsp;</td>
</tr>
        </table>
    </ContentTemplate>
 </ajaxToolkit:tabpanel>
</ajaxToolkit:TabContainer>
            </td>
            <td style="width: 5%">
                &nbsp;</td>
        </tr>
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
                &nbsp;</td>
            <td style="width: 5%">
                &nbsp;</td>
        </tr>
    </table>
    </div>
</asp:Content>

