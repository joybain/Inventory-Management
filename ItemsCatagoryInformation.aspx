<%@ Page Title="Items Catagory & SubCatagory.-DDP" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="ItemsCatagoryInformation.aspx.cs" Inherits="ItemsCatagoryInformation" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <script language="javascript" type="text/javascript" >
    function setDecimal(abc) {
        var dt = document.getElementById(abc).value;
        if (dt.length > 0) {
            document.getElementById(abc).value = parseFloat(dt).toFixed(2);
        }
    }
    function isNumber(evt) {
        evt = (evt) ? evt : window.event;
        var charCode = (evt.which) ? evt.which : evt.keyCode;
        if (charCode != 46 && charCode > 31
            && (charCode < 48 || charCode > 57)) {
            return false;
        }
        return true;
    }

    onblur = function () {
        setTimeout('self.focus()', 100);
    }
</script>
    <div id="frmMainDiv"  style="width:100%; background-color:transparent;">
 <table style="width:60%;" id="pageFooterWrapper">
  <tr>  
  <td style="width:5%;"></td>   
<td>
<asp:Button ID="Delete" runat="server" ToolTip="Delete" onclick="btnDelete_Click"
           
        
        onclientclick="javascript:return window.confirm('are u really want to delete these data')" Text="Delete" 
        Height="35px" Width="100px" BorderStyle="Outset"  />
</td>
 <td style="width:5%;"></td>   
<td style="vertical-align:middle;" align="center">
<asp:Button ID="btnSave" runat="server" ToolTip="Save Item" onclick="btnSave_Click" Text="Save" 
        Height="35px" Width="100px" BorderStyle="Outset"  />
</td>
<td style="width:5%;"></td>   
  <td style="vertical-align:middle;" align="center">
<asp:Button ID="btnClear" runat="server"  ToolTip="Clear" onclick="btnClear_Click" Text="Clear" 
        Height="35px" Width="100px" BorderStyle="Outset"  />
</td>
<td style="width:5%;"></td>   
   </tr>
</table> 

<table style="width:100%; background-color:White" >
<tr>
<td style="width:1%;"></td>
<td style="width:98%; padding-top:10px;" align="center"> 
<table style="width:100%;">
<tr>
<td style="width:42%;" align="left">
    &nbsp;</td>
<td style="width:1%;">&nbsp;</td>
<td style="width:60%; vertical-align:top;" align="left">
                <asp:HiddenField ID="HfdCat" runat="server" />
                <asp:HiddenField ID="HfdSubCat" runat="server" />
    </td>
</tr>
<tr>
<td align="left" valign="top" style="width: 42%" >
   
<%--<fieldset  ><legend>Select Catagory OR Sud-Catagory</legend>--%>
<table style="width:100%;"><tr>
<td valign="top" align="left" style="text-align: left;">
    
        <fieldset style="vertical-align: top; border: solid 1px #8BB381;"><legend style="color: maroon;"><b> Category & Subcategory Information </b></legend>
     <asp:UpdatePanel ID="UPTree" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
    <asp:TreeView ID="TreeView1" runat="server" Width="100%" 
             onselectednodechanged="TreeView1_SelectedNodeChanged" 
        Font-Bold="True" ForeColor="Blue" ImageSet="Contacts" 
        NodeIndent="10">
            <ParentNodeStyle Font-Bold="True" ForeColor="#5555DD" />
            <HoverNodeStyle Font-Underline="False" />
            <SelectedNodeStyle Font-Underline="True" HorizontalPadding="0px" 
                VerticalPadding="0px" />            
            <Nodes>
                <asp:TreeNode Text="Category" Value="C"></asp:TreeNode>
                <asp:TreeNode Text="Sub Category" Value="SC"></asp:TreeNode>
            </Nodes>
            <NodeStyle Font-Names="Verdana" Font-Size="8pt" ForeColor="Black" 
                HorizontalPadding="5px" NodeSpacing="0px" VerticalPadding="0px" />
   </asp:TreeView>
    </ContentTemplate>
    </asp:UpdatePanel>
    <div  style="height: 35%">
        ----------------------------------------------------------------------------------------------------</div>
    <asp:UpdatePanel ID="UPDetails" runat="server" UpdateMode="Conditional">
    <ContentTemplate>

    <table style="width: 100%;line-height:1.5em;" >
        <tr>
            <td align="right" style="width: 134px; height: 5px;">
                <asp:Label ID="lblID" runat="server" Font-Size="9pt" Visible="False"></asp:Label>
            </td>
            <td style="width: 14px; height: 5px;">
                </td>
            <td align="left" style="height: 5px">
            </td>
        </tr>
        <tr>
            <td align="right" style="width: 134px">
                <asp:Label ID="lblItemCode" runat="server" Font-Size="9pt">Item Code</asp:Label>
            </td>
            <td style="width: 14px">
                &nbsp;</td>
            <td align="left">
                <asp:TextBox ID="txtItemCode" runat="server" AutoPostBack="False" 
                    CssClass="tbc" Enabled="False" Font-Size="8pt" MaxLength="15" SkinID="tbPlain" 
                    Width="30%"></asp:TextBox>
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:CheckBox ID="CheckBox1" Checked="true"  runat="server" Text="Active?" 
                    Visible="False" />
            </td>
        </tr>
        <tr>
            <td align="right" style="width: 134px; height: 30px;">
                <asp:Label ID="lblCatagory" runat="server" Font-Size="9pt">Categoty</asp:Label>
            </td>
            <td style="width: 14px; height: 30px;">
                </td>
            <td align="left" style="height: 30px">
                <asp:TextBox ID="txtItemDesc" runat="server" AutoPostBack="False" TextMode="MultiLine"
                    Enabled="False" Font-Size="8pt" SkinID="tbPlain" Width="95%" Height="80px"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td align="right" style="width: 134px; height: 26px;">
	            <asp:Label ID="lblddlCat" runat="server" Text="Category"></asp:Label>
            </td>
            <td style="width: 14px; height: 26px;">
                </td>
            <td align="left" style="height: 26px">
                <asp:DropDownList ID="ddlCatagory" runat="server" AutoPostBack="True" 
                    Enabled="False" Font-Size="8pt" Height="26px" 
                    onselectedindexchanged="ddlCatagory_SelectedIndexChanged" SkinID="ddlPlain" 
                    Width="98%">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td align="right" style="width: 134px; height: 28px;">
	    <asp:Label ID="Label24" runat="server" Text="Department" Font-Size="9pt"></asp:Label>
            </td>
            <td style="width: 14px; height: 28px;">
                </td>
            <td align="left" style="height: 28px">
                <asp:DropDownList ID="ddlDepart" runat="server" Height="26px"  Width="98%" 
                    AutoPostBack="True" onselectedindexchanged="ddlDepart_SelectedIndexChanged">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td align="right" style="width: 134px; height: 30px;"> 
                Description</td>
            <td style="width: 14px; height: 30px;">
                </td>
            <td align="left" style="height: 30px">
    <asp:TextBox SkinID="tbPlain" ID="txtDesc" runat="server" Width="95%" TextMode="MultiLine" 
    AutoPostBack="False" Font-Size="8pt"  CssClass="tbc" Enabled="False" Height="80px"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td align="right" style="width: 134px; height: 4px;">
                &nbsp;</td>
            <td style="width: 14px; height: 4px;">
            </td>
            <td align="left" style="height: 4px">
                &nbsp;</td>
        </tr>
    </table>
    </ContentTemplate>
    </asp:UpdatePanel>
    </fieldset>

</td>
</tr>

</table>
   
   <%--</fieldset>--%>
  
    </td>
<td style="width:1%;" valign="top">&nbsp;</td>
<td style="width:60%;  vertical-align:top;" align="left">
    <fieldset style="vertical-align: top; border: solid 1px #8BB381;width: 95%;"><legend style="color: maroon;"><b> Category & Subcategory History  </b></legend>
    <asp:UpdatePanel ID="UPCatagory" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
<asp:GridView CssClass="mGrid" PagerStyle-CssClass="pgr"  style="text-align:center;"
        AlternatingRowStyle-CssClass="alt" ID="dgCatagory" runat="server" AutoGenerateColumns="False" 
        AllowPaging="True"  BackColor="White" BorderWidth="1px" BorderStyle="Solid"
        CellPadding="2" BorderColor="LightGray" Font-Size="9pt" 
        AllowSorting="True" PageSize="20" Width="100%" 
        onselectedindexchanged="dgCatagory_SelectedIndexChanged" 
        onpageindexchanging="dgCatagory_PageIndexChanging" 
            onrowdatabound="dgCatagory_RowDataBound" >
  <HeaderStyle Font-Size="9" Font-Names="Arial" Font-Bold="True" BackColor="LightGray" HorizontalAlign="center"  ForeColor="Black" />
  <Columns>
                        <asp:CommandField ItemStyle-Height="25px" ItemStyle-HorizontalAlign="Center" 
                            ItemStyle-Width="40px" ShowSelectButton="True">
                            <ItemStyle Height="25px" HorizontalAlign="Center" Width="40px" />
                        </asp:CommandField>
                        <asp:BoundField DataField="mjr_code" HeaderText="Item Code" 
                            ItemStyle-HorizontalAlign="Center" ItemStyle-Width="100px">
                            <ItemStyle HorizontalAlign="Center" Width="100px" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Dept_Name" HeaderText="Department" 
                            ItemStyle-HorizontalAlign="Center" ItemStyle-Width="100px">
                            <ItemStyle HorizontalAlign="Left" Width="100px" />
                        </asp:BoundField>
                         <asp:BoundField DataField="mjr_desc" HeaderText="Category" 
                            ItemStyle-HorizontalAlign="Center" ItemStyle-Width="100px">
                            <ItemStyle HorizontalAlign="Center" Width="100px" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Description" HeaderText="Description" 
                            ItemStyle-HorizontalAlign="Left" ItemStyle-Width="350px">
                            <ItemStyle HorizontalAlign="Left" Width="350px" />
                        </asp:BoundField>                       
                        <asp:BoundField DataField="Active" HeaderText="Active" />
                        <asp:BoundField DataField="Code" HeaderText="Code" />
                        <asp:BoundField DataField="DeptID" HeaderText="DeptID" />
                    </Columns>
                        <RowStyle BackColor="White" />
                        <SelectedRowStyle BackColor="" Font-Bold="True" />
                        <PagerStyle BackColor="LightGray" ForeColor="Black" HorizontalAlign="Center" />
                        <AlternatingRowStyle BackColor="" />
</asp:GridView>
 </ContentTemplate>
    </asp:UpdatePanel>
     <asp:UpdatePanel ID="UPSubCat" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
<asp:GridView CssClass="mGrid" PagerStyle-CssClass="pgr"  style="text-align:center;"
        AlternatingRowStyle-CssClass="alt" ID="dgSubCatagory" runat="server" AutoGenerateColumns="False" 
        AllowPaging="True"  BackColor="White" BorderWidth="1px" BorderStyle="Solid"
        CellPadding="2" BorderColor="LightGray" Font-Size="9pt" 
        AllowSorting="True" PageSize="30" Width="100%" 
        onselectedindexchanged="dgSubCatagory_SelectedIndexChanged" 
        onrowdatabound="dgSubCatagory_RowDataBound" 
        onpageindexchanging="dgSubCatagory_PageIndexChanging" >
  <HeaderStyle Font-Size="9" Font-Names="Arial" Font-Bold="True" BackColor="LightGray" HorizontalAlign="center"  ForeColor="Black" />
  <Columns>  
  <asp:CommandField ShowSelectButton="True"  ItemStyle-Width="40px" 
          ItemStyle-HorizontalAlign="Center" ItemStyle-Height="25px">
<ItemStyle HorizontalAlign="Center" Height="25px" Width="40px"></ItemStyle>
      </asp:CommandField>
  <asp:BoundField  HeaderText="Item Code" DataField="sub_mjr_code" 
          ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center">    
<ItemStyle HorizontalAlign="Center" Width="100px"></ItemStyle>
      </asp:BoundField>
      <asp:BoundField  HeaderText="Department" DataField="Dept_Name" 
          ItemStyle-Width="350px" ItemStyle-HorizontalAlign="Left">  
<ItemStyle HorizontalAlign="Left" Width="300px"></ItemStyle>
      </asp:BoundField>
      <asp:BoundField  HeaderText="Category" DataField="MJR_DESC" 
          ItemStyle-Width="350px" ItemStyle-HorizontalAlign="Left">  
<ItemStyle HorizontalAlign="Left" Width="300px"></ItemStyle>
      </asp:BoundField>
      <asp:BoundField  HeaderText="SubCategory" DataField="sub_mjr_desc" 
          ItemStyle-Width="350px" ItemStyle-HorizontalAlign="Left">  
<ItemStyle HorizontalAlign="Left" Width="300px"></ItemStyle>
      </asp:BoundField>
  <asp:BoundField  HeaderText="Description" DataField="Description" 
          ItemStyle-Width="350px" ItemStyle-HorizontalAlign="Left">  
<ItemStyle HorizontalAlign="Left" Width="300px"></ItemStyle>
      </asp:BoundField>
      <asp:BoundField DataField="Active" HeaderText="Active" />
      <asp:BoundField HeaderText="CatagoryID" DataField="mjr_code" />
      <asp:BoundField DataField="sub_mjr_code" HeaderText="SubcatID" />
      <asp:BoundField DataField="SubDeptID" HeaderText="SubDeptID" />      
      <asp:BoundField DataField="Code" HeaderText="Code" />
  </Columns>
                        <RowStyle BackColor="White" />
                        <SelectedRowStyle BackColor="" Font-Bold="True" />
                        <PagerStyle BackColor="LightGray" ForeColor="Black" HorizontalAlign="Center" />
                        <AlternatingRowStyle BackColor="" />
</asp:GridView>
</ContentTemplate>
</asp:UpdatePanel>
</fieldset>
    </td>
</tr>
<tr>
<td style="width:98%; padding-top:10px; padding-right:100px;" align="center" colspan="3">

</td>
</tr>
<tr>
<td style="width:98%; padding-top:10px; padding-right:100px;" align="center" colspan="3">

    &nbsp;</td>
</tr>
</table>
</td>
<td style="width:1%;"></td>
</tr>
</table>
</div>
</asp:Content>


