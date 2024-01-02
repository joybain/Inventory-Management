<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="UserInfo.aspx.cs" Inherits="UserInfo" Title="User Administration"  Theme="Themes"%>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">  
    <div id="frmMainDiv" style="width:100%; background-color:White;">
    <table id="pageFooterWrapper">
   <tr>
   <td align="center">
       <asp:Button ID="btnDelete" runat="server" ToolTip="Delete" onclick="btnDelete_Click"  
           onclientclick="javascript:return window.confirm('Are u really want to delete these data')" 
           Text="Delete"
            
             Width="100px" Height="35px" BorderStyle="Outset" BorderWidth="1px"/> </td> 
   <td align="center"> 
    <asp:Button ID="btnFind" runat="server" ToolTip="Find" 
           onclick="btnFind_Click"  Text="Find"            
             Width="100px" Height="35px" BorderStyle="Outset" BorderWidth="1px" />        
           </td> 
   <td align="center"> 
       <asp:Button ID="btnSave" runat="server" ToolTip="Save" onclick="btnSave_Click" 
       Text="Save"
            
             Width="100px" Height="35px" BorderStyle="Outset" BorderWidth="1px"  /></td>  
   <td align="center"> 
       <asp:Button ID="btnClear" runat="server" ToolTip="Clear" 
           onclick="btnClear_Click" Text="Clear" 
            
             Width="100px" Height="35px" BorderStyle="Outset" BorderWidth="1px"/>       
           </td>            
   </tr>
</table>


<table style="width:100%; font-family:Verdana;">

<tr>
<td style="width:10%;">&nbsp;</td>
<td style="width:80%;" align="center">
    &nbsp;</td>
<td style="width:10%;">&nbsp;</td>
</tr>

<tr>
<td style="width:10%;">&nbsp;</td>
<td style="width:80%;" align="center">
                <asp:Label ID="Label29" runat="server" BackColor="#CCCCCC" BorderStyle="Inset" 
                    Font-Bold="True" Font-Size="X-Large" Text="User Information. " 
                    Width="80%"></asp:Label>
            </td>
<td style="width:10%;">&nbsp;</td>
</tr>

<tr>
<td style="width:10%;">&nbsp;</td>
<td style="width:80%;" align="center">
    &nbsp;</td>
<td style="width:10%;">&nbsp;</td>
</tr>

<tr>
<td style="width:10%;">&nbsp;</td>
<td style="width:80%;" align="center">
<fieldset style="vertical-align: top; border: solid 1px #8BB381;"><legend style="color: maroon; text-align:left;"><b>User Info</b></legend>

    
<table style="width:100%; line-height:2.5;">
<tr>
<td style="width:137px; vertical-align:middle;" align="right">
<asp:Label ID="lblUserIf" runat="server" Font-Size="8pt" style="font-weight: 700">User ID</asp:Label>
</td>
<td align="left" style="height: 27px; width:23%;"><asp:TextBox SkinID="tbGray" 
        ID="txtUserId" runat="server"  
        Width="100%" Font-Size="8pt" Enabled="true" MaxLength="20"></asp:TextBox>
</td>
<td style="height: 27px; width: 31px;" align="center" >
<asp:Label ID="lblDescription0" runat="server" Font-Size="8pt" ForeColor="#CC3300">*</asp:Label>
    </td>
<td style="height: 27px; width:15;" align="left">
<asp:Label ID="lblDescription" runat="server" Font-Size="8pt">User Name</asp:Label>
</td>
<td align="left" style="height: 27px; width:25%;"> <asp:TextBox SkinID="tbGray" ID="txtDescription" runat="server" Width="362px" 
        Font-Size="8"  ></asp:TextBox></td>
</tr>
<tr>
<td style="width:137px; vertical-align:middle;" align="right">
<asp:Label ID="Label3" runat="server" Font-Size="8pt" style="font-weight: 700">Password</asp:Label>
</td>
<td align="left" style="width: 23%"> 
    <asp:TextBox SkinID="tbGray" ID="txtPassword" TextMode="Password" 
        runat="server"  Width="100%" Font-Size="8" Enabled="true"></asp:TextBox></td>
<td style="height: 27px; width: 31px;" align="center" >
<asp:Label ID="lblDescription3" runat="server" Font-Size="8pt" ForeColor="#CC3300">*</asp:Label>
    </td>
<td style="width:100px; vertical-align:middle;" align="left">
<asp:Label ID="Label2" runat="server" Font-Size="8pt">User Group</asp:Label>
</td>
<td align="left"> 
    <asp:DropDownList SkinID="ddlPlain" ID="ddlUsrGrp" 
        runat="server"  Font-Size="8" Width="50%" >
  </asp:DropDownList>
</td>
</tr>
<tr>
<td style="width:137px;" align="right">
<asp:Label ID="lblFax" runat="server" Font-Size="8pt" style="font-weight: 700">Status</asp:Label>
</td>
<td align="left" style="width: 23%"> 
    <asp:DropDownList SkinID="ddlPlain" ID="ddlStatus" 
        runat="server"  Font-Size="8" Width="104%" >
  <asp:ListItem Text="Enabled" Value="A"></asp:ListItem>
  <asp:ListItem Text="Disabled" Value="U"></asp:ListItem>
  </asp:DropDownList>
</td>
<td style="height: 27px; width: 31px;" align="center" >
<asp:Label ID="lblDescription1" runat="server" Font-Size="8pt" ForeColor="#CC3300">*</asp:Label>
    </td>
<td style="width:100px; font-weight: 700;" align="left">
<asp:Label ID="Label1" runat="server" Font-Size="8pt">Emplyee ID</asp:Label>
</td>
<td align="left">
     <asp:UpdatePanel ID="UP1" runat="server" UpdateMode="Conditional"><ContentTemplate>
     <asp:TextBox SkinID="tbGray" ID="txtEmpNo" runat="server"  
        Width="362px" Font-Size="8" Enabled="true" MaxLength="15" AutoPostBack="True" 
             ontextchanged="txtEmpNo_TextChanged"></asp:TextBox>
                <ajaxToolkit:AutoCompleteExtender ID="txtTransfer_AutoCompleteExtender" runat="server" CompletionInterval="20" 
        CompletionSetCount="30" EnableCaching="true" MinimumPrefixLength="1" ServiceMethod="GetFacultySearch" 
        ServicePath="~/AutoComplete.asmx" TargetControlID="txtEmpNo">
        </ajaxToolkit:AutoCompleteExtender>
         <asp:Label ID="lblEmpID" runat="server" Font-Size="8pt" Visible="False"></asp:Label>
         </ContentTemplate></asp:UpdatePanel>
</td>
</tr>
    <tr>
        <td style="width:137px;" align="right">
            <strong>Branch Name :</strong></td>
        <td align="left" style="width: 23%"> 
    <asp:DropDownList SkinID="ddlPlain" ID="ddlBranch" 
        runat="server"  Font-Size="8" Width="104%" >
  </asp:DropDownList>
        </td>
        <td align="center" style="width: 31px">
            &nbsp;</td>
        <td align="left">
            &nbsp;</td>
        <td align="left">
            &nbsp;</td>
    </tr>
<tr>
<td style="width:137px;" align="right">
    &nbsp;</td>
<td align="left" style="width: 23%"> 
<asp:DropDownList SkinID="ddlPlain" ID="ddlDept" runat="server"  Font-Size="8" 
        Width="104%" Visible="False" >
    <asp:ListItem Value="1">Main</asp:ListItem>
 </asp:DropDownList>
</td>
    <td align="center" style="width: 31px">
        &nbsp;</td>
    <td align="left">
        &nbsp;</td>
    <td align="left">
        <asp:DropDownList SkinID="ddlPlain" ID="ddlUserType" runat="server"  
            Font-Size="8" Width="50%" Visible="False" >
  <asp:ListItem Value="1">Bangladesh</asp:ListItem> 
  <asp:ListItem Value="2">Manila</asp:ListItem>
            <asp:ListItem Value="3">All</asp:ListItem>
  </asp:DropDownList>
    </td>
</tr>

</table>

    
</fieldset>
</td>
<td style="width:10%;">&nbsp;</td>
</tr>

<tr>
<td style="width:10%;">&nbsp;</td>
<td style="width:80%;" align="center">

    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" 
        CssClass="mGrid" 
        onselectedindexchanged="GridView1_SelectedIndexChanged" Width="100%" 
        onrowcommand="GridView1_RowCommand" 
        onrowdatabound="GridView1_RowDataBound">
        <Columns>
            <asp:CommandField ShowSelectButton="True" />
            <asp:BoundField DataField="USER_NAME" HeaderText="UserID" />
            <asp:BoundField DataField="DESCRIPTION" HeaderText="User Name" />
            <asp:BoundField DataField="BranchName" HeaderText="Branch" />
            <asp:BoundField DataField="UserTypeName" HeaderText="User Type" />
            <asp:BoundField DataField="STATUS" HeaderText="Status" />
            <asp:BoundField DataField="BranchName" HeaderText="Branch Name" />
            <asp:TemplateField HeaderText="Reset Password" >
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lbProgram" runat="server" CommandName="Reset" onclientclick="javascript:return window.confirm('are u really want to Change Password.')"
                                            Text="( Reset )" Font-Bold="True" Font-Size="12pt"></asp:LinkButton>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" Width="15%" />
                       </asp:TemplateField>
        </Columns>
    </asp:GridView>
    </td>
<td style="width:10%;">&nbsp;</td>
</tr>

<tr>
<td style="width:10%;">&nbsp;</td>
<td style="width:80%;" align="center">
    &nbsp;</td>
<td style="width:10%;">&nbsp;</td>
</tr>

</table>

</div>
</asp:Content>

