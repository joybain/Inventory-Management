<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="ReportMap.aspx.cs" Inherits="ReportMap" Title="Dynamic Report Setup"  Theme="Themes"%>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">  <script src='<%= ResolveUrl("~/Scripts/valideDate.js") %>' type="text/javascript"></script>

<script language="javascript" type="text/javascript">
    function expandcollapse(obj,row)
    {
        var div = document.getElementById(obj);
        var img = document.getElementById('img' + obj);        
        
        if (div.style.display == "none")
        {
            div.style.display = "block";
            if (row == 'alt') 
            {                
                img.src = "img/minus.gif";
            }
            else
            {                
                img.src = "img/minus.gif";
            }
            img.alt = "Close to view other Map Breaks";            
        }
        else
        {
            div.style.display = "none";
            if (row == 'alt')
            {                
                img.src = "img/plus.gif";
            }
            else
            {                
                img.src = "img/plus.gif";
            }
            img.alt = "Expand to show Breaks";
        }
    } 
    function expandcollapse1(obj)
    {
        var div = document.getElementById(obj);
        var img = document.getElementById('img' + obj);    
         alert(obj);       
            div.style.display = "block";
                          
                img.src = "img/minus.gif";
           
    } 
</script>
<div id="frmMainDiv" style="background-color:White; width:100%; overflow:visible;">
    <table  id="pageFooterWrapper">
   <tr>   
   <td align="center"> 
       <asp:Button ID="btnDelete" runat="server" ToolTip="Delete" onclick="btnDelete_Click"  
           
           onclientclick="javascript:return window.confirm('Are u really want to delete these data')" Text="Delete" 
        Height="35px" Width="100px" BorderStyle="Outset"/>       
           </td>
   <td align="center"> 
       <asp:Button ID="btnNew" runat="server" ToolTip="Enter a New Dynamic Report" onclick="btnNew_Click" 
       Text="New" 
        Height="35px" Width="100px" BorderStyle="Outset" />
   </td> 
   <td align="center"> 
       <asp:Button ID="btnSave" runat="server" ToolTip="Save" onclick="btnSave_Click" 
       Text="Save" 
        Height="35px" Width="100px" BorderStyle="Outset"  /></td>  
   <td align="center"> 
       <asp:Button ID="btnClear" runat="server" ToolTip="Clear" 
           onclick="btnClear_Click" Text="Clear" 
        Height="35px" Width="100px" BorderStyle="Outset"/>       
           </td>   
   </tr>
   </table>
<table style="width:100%;">
<tr>
<td style="width:1%;"></td>
<td style="width:98%;" align="center"> 

   <br />
<asp:UpdatePanel ID="UpdatePanel1" runat="server"  UpdateMode="Conditional" >
<ContentTemplate>

<table style="width:100%; border:solid 1px gray;  padding-right:15px;">
<tr>
<td style="width:15%;" align="left">
<asp:Label ID="lblTypeCode" runat="server" Font-Size="8pt" Width="100%">Report Type Code</asp:Label>
</td>
<td style="width:25%;" align="left"> 
<asp:TextBox SkinID="tbGray" ID="txtReportType" runat="server"  Width="100%" Font-Size="8pt" MaxLength="3" CssClass="tbc"></asp:TextBox>
</td>
<td style=" width:10%;" ></td>
<td style="width:15%;" align="left">
<asp:Label ID="lblVerNo" runat="server" Font-Size="8pt" Width="100%">Version No</asp:Label>
</td>
<td  style="width:20%;" align="left"> 
<asp:TextBox SkinID="tbGray" ID="txtVerNo" runat="server" Width="100%" Font-Size="8pt" MaxLength="1" CssClass="tbc"></asp:TextBox></td>
</tr>
<tr>
<td style="width:15%;" align="left">
<asp:Label ID="lblMstDesc" runat="server" Font-Size="8pt" Width="100%">Description</asp:Label>
</td>
<td colspan="4" style="width:85%;" align="left">
<asp:TextBox SkinID="tbGray" ID="txtMstDesc" runat="server" Width="100%" Font-Size="8pt" MaxLength="200"></asp:TextBox>
</td>
</tr>
<tr>
<td style="width:15%;" align="left">
<asp:Label ID="lblRefType" runat="server" Font-Size="8pt" Width="100%">Ref. Type Code</asp:Label>
</td>
<td style="width:20%;" align="left"> 
<asp:TextBox SkinID="tbGray" ID="txtRefType" runat="server"  Width="100%" Font-Size="8pt" MaxLength="1" CssClass="tbc"></asp:TextBox></td>
<td style=" width:10%;" ></td>
<td style="width:15%;" align="left">
<asp:Label ID="lblRefVer" runat="server" Font-Size="8pt" Width="100%">Ref. Version No</asp:Label>
</td>
<td style="width:20%;" align="left" > 
<asp:TextBox SkinID="tbGray" ID="txtRefVer" runat="server" Width="100%" Font-Size="8pt" MaxLength="1" CssClass="tbc"></asp:TextBox></td>
</tr>

</table>
<%--<asp:UpdateProgress ID="udProgress" runat="server" DisplayAfter="100" Visible="true" DynamicLayout="true">
    <ProgressTemplate>
    <img src="img/loading.gif" alt="Process is running..."/>
    </ProgressTemplate>
</asp:UpdateProgress>--%>

<br />
<asp:Label ID="lblTranStatus" runat="server" Width="100%" Text="" Visible="false" Font-Size="8pt"></asp:Label>
<asp:GridView  RowStyle-Height="25px" CssClass="mGrid" PagerStyle-CssClass="pgr"  AlternatingRowStyle-CssClass="alt" ID="dgMapMst" runat="server" 
AutoGenerateColumns="false" Width="100%" 
        AllowPaging="True" BackColor="White" BorderWidth="1px" BorderStyle="Solid"
        CellPadding="2" CellSpacing="0" BorderColor="LightGray" Font-Size="8pt" AllowSorting="true" PageSize="5" 
        onselectedindexchanged="dgMapMst_SelectedIndexChanged" ForeColor="#333333"  >
  <HeaderStyle Font-Size="8pt"  Font-Bold="True" HorizontalAlign="center" BackColor="Silver"/>
  <FooterStyle BackColor="Silver" Font-Bold="True" ForeColor="White" />
  <Columns>
  <asp:CommandField ShowSelectButton="True"  ItemStyle-Width="40px" ItemStyle-HorizontalAlign="Center" ItemStyle-ForeColor="Blue"/>
  <asp:BoundField  HeaderText="Type Code" DataField="type_code" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center"/>
  <asp:BoundField  HeaderText="Version No" DataField="ver_no" ItemStyle-Width="90px" ItemStyle-HorizontalAlign="Center"/>
  <asp:BoundField  HeaderText="Description" DataField="description" ItemStyle-Width="350px" ItemStyle-HorizontalAlign="Left"/>  
  </Columns>
                        <RowStyle BackColor="white" />
                        <EditRowStyle BackColor="" />
                        <PagerStyle  HorizontalAlign="Center" />
                        <AlternatingRowStyle BackColor="#F5F5F5" />
  </asp:GridView>
  
<asp:GridView  RowStyle-Height="25px" CssClass="mGrid" PagerStyle-CssClass="pgr"  
        AlternatingRowStyle-CssClass="alt" ID="dgMapDtl" runat="server" AutoGenerateColumns="False" 
        AllowPaging="True" Width="100%" BackColor="White" BorderWidth="1px" BorderStyle="Solid"
        CellPadding="2" BorderColor="LightGray" Font-Size="8pt" 
        AllowSorting="True" PageSize="200" 
          OnRowUpdating="dgMapDtl_RowUpdating" OnRowEditing="dgMapDtl_RowEditing" 
          OnRowCancelingEdit="dgMapDtl_CancelingEdit" ShowFooter="false" 
          onselectedindexchanging="dgMapDtl_SelectedIndexChanging" 
          onrowdeleting="dgMapDtl_RowDeleting"
          onrowdatabound="dgMapDtl_RowDataBound" 
        onrowcommand="dgMapDtl_RowCommand" >
  <HeaderStyle Font-Size="8pt" Font-Bold="True" BackColor="Silver" HorizontalAlign="center"/> 
  <Columns>
 <%-- <asp:CommandField ShowSelectButton="true" />--%>
 <asp:TemplateField ItemStyle-Font-Size="8pt" ItemStyle-Width="150px" ItemStyle-HorizontalAlign="Center">
 <ItemTemplate>
 <asp:LinkButton ID="lbEdit" runat="server" CausesValidation="False" CommandName="Edit" Text="Edit" ></asp:LinkButton>  
  <asp:LinkButton ID="lbDelete" runat="server" CausesValidation="False" CommandName="Delete" Text="Delete"></asp:LinkButton>
  <asp:LinkButton ID="lbAddNew" runat="server" CausesValidation="False" CommandName="Add" Text="New"></asp:LinkButton>
  <ajaxToolkit:ConfirmButtonExtender ID="detdeleteconfirm" runat="server" ConfirmText="Are you sure to delete??" TargetControlID="lbDelete"></ajaxToolkit:ConfirmButtonExtender> 
 </ItemTemplate>
 <EditItemTemplate>
 <asp:LinkButton ID="lbUpdate" runat="server" CausesValidation="True" CommandName="Update" Text="Update" ></asp:LinkButton> 
  <asp:LinkButton ID="lbCancel" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel"></asp:LinkButton>
 </EditItemTemplate>
 <FooterTemplate>
 <asp:LinkButton ID="lbInsert" runat="server" CausesValidation="True" CommandName="Insert" Text="Insert" ></asp:LinkButton> 
  <asp:LinkButton ID="lbCancel" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel"></asp:LinkButton>
 </FooterTemplate>
 <ItemStyle HorizontalAlign="Center" Width="150px"></ItemStyle>
 </asp:TemplateField>
 
 <asp:TemplateField ItemStyle-Width="40px" ItemStyle-HorizontalAlign="Center">
 <ItemTemplate> 
 <asp:Panel ID="panel1" runat="server">
 <a href="javascript:expandcollapse('div<%# Eval("sl_no") %>', 'one');">
 <img id="imgdiv<%# Eval("sl_no") %>" alt="Click to show/hide Maps" style="height:9px; width:10px;" src="img/plus.gif"/>
 <%--<asp:Image ID="imgdiv<%# Eval("sl_no") %>" runat="server" AlternateText="Click to show/hide Maps"  width="9px" ImageUrl="img/plus.gif" />--%>
 </a>
 </asp:Panel>  
 </ItemTemplate>
 <EditItemTemplate>
 <asp:Panel ID="panel1" runat="server" Width="40px">
  <asp:Label ID="lblSlNo1" runat="server" Text=".  ." ForeColor="White"></asp:Label>  
  </asp:Panel>  
 </EditItemTemplate>
 <FooterTemplate>
 <asp:Panel ID="panel1" runat="server" Width="40px">
  <asp:Label ID="lblSlNo1" runat="server" Text=".  ." ForeColor="White" ></asp:Label>  
  </asp:Panel> 
 </FooterTemplate>
     <ItemStyle HorizontalAlign="Center" Width="40px" />
 </asp:TemplateField>
 
  <asp:TemplateField HeaderText="Sl#" ItemStyle-Width="40px" ItemStyle-HorizontalAlign="Center">
  <ItemTemplate>  
  <asp:Label ID="lblSlNo" runat="server" Text='<%#Eval("sl_no") %>' Width="40px"></asp:Label>    
  </ItemTemplate>
  <EditItemTemplate>  
  <asp:TextBox SkinID="tbGray" ID="txtSlNo" runat="server" Text='<%#Eval("sl_no") %>' CssClass="tbc" Width="40px" MaxLength="3" Font-Size="8pt"></asp:TextBox>   
  </EditItemTemplate>
  <FooterTemplate>
  <asp:TextBox SkinID="tbGray" ID="txtSlNo" runat="server" Text="" Width="40px" CssClass="tbc" MaxLength="3" Font-Size="8pt"></asp:TextBox>
  </FooterTemplate>
      <ItemStyle HorizontalAlign="Center" Width="40px" />
  </asp:TemplateField>
  
  <asp:TemplateField HeaderText="Segment Code" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center">
  <ItemTemplate>
  <asp:Label ID="lblGlSegCode" runat="server" Text='<%#Eval("gl_seg_code") %>' Width="80px"></asp:Label>  
  </ItemTemplate>
  <EditItemTemplate>
  <asp:TextBox SkinID="tbGray" ID="txtGlSegCode" runat="server" Text='<%#Eval("gl_seg_code") %>' CssClass="tbc" Width="70px" MaxLength="7" Font-Size="8pt"></asp:TextBox>
  </EditItemTemplate>
  <FooterTemplate>
  <asp:TextBox SkinID="tbGray" ID="txtGlSegCode" runat="server" Text="" Width="70px" MaxLength="7" CssClass="tbc" Font-Size="8pt"></asp:TextBox>
  </FooterTemplate>
<ItemStyle HorizontalAlign="Center" Width="100px"></ItemStyle>
  </asp:TemplateField>
  
  <asp:TemplateField HeaderText="Description" ItemStyle-Width="40%" ItemStyle-HorizontalAlign="Left" >
  <ItemTemplate>
  <asp:Label ID="lblGlDesc" runat="server" Text='<%#Eval("Description") %>' Width="310px"></asp:Label>  
  </ItemTemplate>
  <EditItemTemplate>
  <asp:TextBox SkinID="tbGray" ID="txtGlDesc" runat="server" Text='<%#Eval("Description") %>' Width="310px" MaxLength="200" Font-Size="8pt"></asp:TextBox>
  </EditItemTemplate>
  <FooterTemplate>
  <asp:TextBox SkinID="tbGray" ID="txtGlDesc" runat="server" Text="" Width="310px" MaxLength="200" Font-Size="8pt"></asp:TextBox>
  </FooterTemplate>
<ItemStyle HorizontalAlign="Left" Width="40%"></ItemStyle>
  </asp:TemplateField>
  
  <asp:TemplateField HeaderText="Period" ItemStyle-Width="80px" ItemStyle-HorizontalAlign="Center">
  <ItemTemplate>
  <asp:Label ID="lblBalFrom" runat="server" Text='<%#Eval("bal_from") %>'></asp:Label>  
  </ItemTemplate>
  <EditItemTemplate>
  <asp:DropDownList SkinID="ddlPlain" ID="ddlBalFrom" runat="server" Font-Size="8pt">
  <asp:ListItem Text="" Value=""></asp:ListItem>
  <asp:ListItem Text="Current" Value="C"></asp:ListItem>
  <asp:ListItem Text="Previous" Value="P"></asp:ListItem>
  </asp:DropDownList>
  </EditItemTemplate>
  <FooterTemplate>
  <asp:DropDownList SkinID="ddlPlain" ID="ddlBalFrom" runat="server" Font-Size="8pt">
  <asp:ListItem Text="" Value=""></asp:ListItem>
  <asp:ListItem Text="Current" Value="C"></asp:ListItem>
  <asp:ListItem Text="Previous" Value="P"></asp:ListItem>
  </asp:DropDownList>
  </FooterTemplate>

<ItemStyle HorizontalAlign="Center" Width="80px"></ItemStyle>
  </asp:TemplateField>
  
  <asp:TemplateField HeaderText="Add/Less" ItemStyle-Width="80px" ItemStyle-HorizontalAlign="Center">
  <ItemTemplate>
  <asp:Label ID="lblAddLess" runat="server" Text='<%#Eval("add_less") %>'></asp:Label>  
  </ItemTemplate>
  <EditItemTemplate>
  <%--<asp:TextBox SkinID="tbGray" ID="txtAddLess" runat="server" Text='<%#Eval("add_less") %>'></asp:TextBox>--%>
  <asp:DropDownList SkinID="ddlPlain" ID="ddlAddLess" runat="server" Font-Size="8pt" Width="80px">
  <asp:ListItem Text="" Value=""></asp:ListItem>
  <asp:ListItem Text="Add" Value="A"></asp:ListItem>
  <asp:ListItem Text="Less" Value="L"></asp:ListItem>
  </asp:DropDownList>
  </EditItemTemplate>
  <FooterTemplate>
  <asp:DropDownList SkinID="ddlPlain" ID="ddlAddLess" runat="server"  Font-Size="8pt" Width="80px">
  <asp:ListItem Text="" Value=""></asp:ListItem>
  <asp:ListItem Text="Add" Value="A"></asp:ListItem>
  <asp:ListItem Text="Less" Value="L"></asp:ListItem>
  </asp:DropDownList>
  </FooterTemplate>
      <ItemStyle HorizontalAlign="Center" Width="80px" />
  </asp:TemplateField>  

  <asp:TemplateField HeaderText="Fixed Amount" ItemStyle-Width="80px" ItemStyle-HorizontalAlign="Right">
  <ItemTemplate>
  <asp:Label ID="lblConsAmt" runat="server" Text='<%#Eval("cons_amt") %>' Width="80px"></asp:Label>  
  </ItemTemplate>
  <EditItemTemplate>
  <asp:TextBox SkinID="tbGray" ID="txtConsAmt" runat="server" Text='<%#Eval("cons_amt") %>' CssClass="tbc" Width="70px" MaxLength="7" Font-Size="8pt"></asp:TextBox>
  </EditItemTemplate>
  <FooterTemplate>
  <asp:TextBox SkinID="tbGray" ID="txtConsAmt" runat="server" Text="" Width="70px" MaxLength="7" CssClass="tbc" Font-Size="8pt"></asp:TextBox>
  </FooterTemplate>
<ItemStyle HorizontalAlign="Center" ></ItemStyle>
  </asp:TemplateField>
  

<asp:TemplateField  ItemStyle-Width="1px" ItemStyle-HorizontalAlign="Center">
<ItemTemplate>
<tr>
<td colspan="100%" align="center">
<div id="div<%# Eval("sl_no") %>" style="display:none;position:relative;left:15px;OVERFLOW:auto;WIDTH:97%" >
<asp:Panel Style="margin-left: 20px; margin-right: 20px;" ID="pnlMapBreak" runat="server">
<asp:UpdatePanel ID="pnlUpdateBreaks"  runat="server" UpdateMode="Conditional">
<ContentTemplate>
<asp:GridView  RowStyle-Height="25px" CssClass="mGrid" PagerStyle-CssClass="pgr"  AlternatingRowStyle-CssClass="alt" ID="dgMapBreak" runat="server" 
AutoGenerateColumns="false" Font-Size="8pt" Width="700px"
        onrowcancelingedit="dgMapBreak_RowCancelingEdit"   OnRowDataBound="dgMapBreak_RowDataBound"
        onrowdeleting="dgMapBreak_RowDeleting" onrowediting="dgMapBreak_RowEditing" 
        onrowupdating="dgMapBreak_RowUpdating" OnRowCommand = "dgMapBreak_RowCommand">
<Columns>
<asp:TemplateField  ItemStyle-Width="150px" ItemStyle-HorizontalAlign="Center">
<ItemTemplate>
<asp:LinkButton ID="lbBrkEdit" runat="server" CausesValidation="False" CommandName="Edit" Text="Edit" ></asp:LinkButton>  
  <asp:LinkButton ID="lbBrkDelete" runat="server" CausesValidation="False" CommandName="Delete" Text="Delete"></asp:LinkButton>
  <ajaxToolkit:ConfirmButtonExtender ID="detdeleteconfirm1" runat="server" ConfirmText="Are you sure to delete??" TargetControlID="lbBrkDelete"></ajaxToolkit:ConfirmButtonExtender>
<asp:LinkButton ID="lbBrkAddNew" runat="server" CausesValidation="False" CommandName="Add" Text="New"></asp:LinkButton>
</ItemTemplate>
<EditItemTemplate>
<asp:LinkButton ID="lbBrkUpdate" runat="server" CausesValidation="True" CommandName="Update" Text="Update" ></asp:LinkButton> 
  <asp:LinkButton ID="lbBrkCancel" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel"></asp:LinkButton>
</EditItemTemplate>
<FooterTemplate>
<asp:LinkButton ID="lbBrkInsert" runat="server" CausesValidation="True" CommandName="Insert" Text="Insert" ></asp:LinkButton> 
  <asp:LinkButton ID="lbBrkCancel" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel"></asp:LinkButton>
</FooterTemplate>
<ItemStyle Width="100px"></ItemStyle>
</asp:TemplateField>
 <asp:TemplateField  HeaderText="Ref. Sl No" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center">
 <ItemTemplate>
 <asp:Label ID="lblBrkRefSlNo" runat="server" Text='<% # Eval("ref_sl_no") %>' Width="70px" CssClass="tbc"></asp:Label>
 </ItemTemplate>
 <EditItemTemplate>
 <asp:Label ID="lblBrkRefSlNo" runat="server" Text='<% # Eval("ref_sl_no") %>' Width="70px" CssClass="tbc"></asp:Label>
 </EditItemTemplate>
 <FooterTemplate>
 <asp:Label ID="lblBrkRefSlNo" runat="server" Text="" Width="70px" CssClass="tbc"></asp:Label>
 </FooterTemplate>
 </asp:TemplateField>
 
 <asp:TemplateField  HeaderText="Sl No" ItemStyle-Width="70px" ItemStyle-HorizontalAlign="Center">
 <ItemTemplate>
 <asp:Label ID="lblBrkSlNo" runat="server" Text='<% # Eval("sl_no") %>' Width="70px"></asp:Label>
 </ItemTemplate>
 <EditItemTemplate>
 <asp:TextBox SkinID="tbGray" ID="txtBrkSlNo" runat="server" Text='<% # Eval("sl_no") %>' Width="70px" MaxLength="3" CssClass="tbc"></asp:TextBox>
 </EditItemTemplate>
 <FooterTemplate>
 <asp:TextBox SkinID="tbGray" ID="txtBrkSlNo" runat="server" Text='<% # Eval("sl_no") %>' Width="70px" MaxLength="3" CssClass="tbc"></asp:TextBox>
 </FooterTemplate>
 </asp:TemplateField>
 
  <asp:TemplateField  HeaderText="Segment Code" ItemStyle-Width="70px" ItemStyle-HorizontalAlign="Center">
 <ItemTemplate>
 <asp:Label ID="lblGl_Coa_Code" runat="server" Text='<% # Eval("gl_seg_code") %>' Width="70px"></asp:Label>
 </ItemTemplate>
 <EditItemTemplate>
 <asp:TextBox SkinID="tbGray" ID="txtGl_Coa_Code" runat="server" Text='<% # Eval("gl_seg_code") %>' Width="70px" CssClass="tbc"></asp:TextBox>
 </EditItemTemplate>
 <FooterTemplate>
 <asp:TextBox SkinID="tbGray" ID="txtGl_Coa_Code" runat="server" Text='<% # Eval("gl_seg_code") %>' Width="70px" CssClass="tbc"></asp:TextBox>
 </FooterTemplate>
 </asp:TemplateField>
 
 <asp:TemplateField HeaderText="Add/Less" ItemStyle-Width="80px" ItemStyle-HorizontalAlign="Center">
  <ItemTemplate>
   <%-- <asp:Label ID="lblAddLess" runat="server" Text='<%#Eval("ADD_LESS") %>'></asp:Label>  --%>
  </ItemTemplate>
  <EditItemTemplate>
  <%--<asp:TextBox SkinID="tbGray" ID="txtAddLess" runat="server" Text='<%#Eval("add_less") %>'></asp:TextBox>--%>
  <asp:DropDownList SkinID="ddlPlain" ID="ddlAddLess" runat="server" Font-Size="8pt" Width="80px">
  <asp:ListItem Text="" Value=""></asp:ListItem>
  <asp:ListItem Text="Add" Value="A"></asp:ListItem>
  <asp:ListItem Text="Less" Value="L"></asp:ListItem>
  </asp:DropDownList>
  </EditItemTemplate>
  <FooterTemplate>
  <asp:DropDownList SkinID="ddlPlain" ID="ddlAddLess" runat="server"  Font-Size="8pt" Width="80px">
  <asp:ListItem Text="" Value=""></asp:ListItem>
  <asp:ListItem Text="Add" Value="A"></asp:ListItem>
  <asp:ListItem Text="Less" Value="L"></asp:ListItem>
  </asp:DropDownList>
  </FooterTemplate>
      <ItemStyle HorizontalAlign="Center" Width="80px" />
  </asp:TemplateField> 

</Columns>
</asp:GridView>

</ContentTemplate>
</asp:UpdatePanel>

</asp:Panel>
</div>
</td>
</tr>
</ItemTemplate>
    <ItemStyle HorizontalAlign="Center" Width="1px" />
</asp:TemplateField>

  </Columns>
                        <RowStyle BackColor="white" />
                        <PagerStyle HorizontalAlign="Center" />
                        <AlternatingRowStyle BackColor="#F5F5F5" />
</asp:GridView>
<div style="display:none;">
<asp:GridView  RowStyle-Height="25px" CssClass="mGrid" PagerStyle-CssClass="pgr"  AlternatingRowStyle-CssClass="alt" ID="tmpMapDtl" runat="server" AutoGenerateColumns="true">
</asp:GridView>
<asp:GridView  RowStyle-Height="25px" CssClass="mGrid" PagerStyle-CssClass="pgr"  AlternatingRowStyle-CssClass="alt" ID="tmpMapBreak" runat="server" AutoGenerateColumns="true">
</asp:GridView>
</div>
</ContentTemplate>
</asp:UpdatePanel>
 </td>
 <td style="width:5%;"></td>
</tr>
</table>

</div>
</asp:Content>

