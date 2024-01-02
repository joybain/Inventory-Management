<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="EmpReport.aspx.cs" Inherits="EmpReport" Title="Personnel Report ParaForm" Theme="Themes" MaintainScrollPositionOnPostback="true" %>
<%--<%@ Register Assembly="CrystalDecisions.Web, Version=13.0.2000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304"
    Namespace="CrystalDecisions.Web" TagPrefix="CR" %>--%>

   
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
    <div id="frmMainDiv" style="background-color:White; width:100%;">
<table style="width:100%;">
<tr>
<td style="width:10%;"></td>
<td style="width:80%;" align="center">
<table style="width:600px;">
<tr>
<td style="width:200px; text-align:left;">
<table>
<tr><td style="font-size:9pt; color:Blue; font-weight: 700; width: 194px;">Select Criteria :</td></tr>
<tr>
<td style="width: 194px">
    <asp:RadioButtonList ID="rdoSelectCriteria" runat="server" Font-Size="8pt" AutoPostBack="true"
        BorderWidth="1" onselectedindexchanged="rdoSelectCriteria_SelectedIndexChanged">
  <%--  <asp:ListItem Text="Select Division" Value="DIV"></asp:ListItem>
    <asp:ListItem Text="Select District" Value="DIS"></asp:ListItem>
    <asp:ListItem Text="Select Thana" Value="THN"></asp:ListItem>
    <asp:ListItem Text="Select Branch" Value="BRN"></asp:ListItem>--%>
    <asp:ListItem Text="Designation" Value="DES"></asp:ListItem>
    <asp:ListItem Text="Emplyoee &amp; Documents" Value="EMP"></asp:ListItem>
        <asp:ListItem Value="BR">Branch</asp:ListItem>
    <asp:ListItem Text="All Employee" Value="All" Selected="True"></asp:ListItem>
        <asp:ListItem Value="Blank">Employee Information Form (Blank)</asp:ListItem>
    </asp:RadioButtonList>
</td>
</tr>
</table>
</td>
<td style="width:200px; text-align:left;">
<table>
<tr><td style="font-size:9pt; color:Blue; font-weight: 700;">Select Report Type :</td></tr>
<tr>
<td>
    <asp:RadioButtonList ID="rdoReportType" runat="server" Font-Size="8pt" BorderWidth="1">
    <asp:ListItem Text="Resume" Value="R"></asp:ListItem>
    <asp:ListItem Text="Detail Information" Value="D"></asp:ListItem>
    <asp:ListItem Text="Simple List" Value="SL" Selected="True"></asp:ListItem>
    </asp:RadioButtonList>
</td>
</tr>
</table>
</td>
<td style="width:200px; text-align:left;">
<asp:UpdateProgress AssociatedUpdatePanelID="upLogin" ID="updateProgress" runat="server">
    <ProgressTemplate>
        <div id="progressBackgroundFilter"></div>
        <div id="processMessage" style="height:30px;padding-right:0px;"><img src="img/loading.gif" alt="" style="border:2px solid lightgray;" /></div>
    </ProgressTemplate>
    </asp:UpdateProgress>

<table>
<tr>
<td align="center">
<asp:Button ID="btnShow" runat="server" ToolTip="Run Report" 
        onclick="btnShow_Click" TabIndex="100" Text="Run Report" 
        Height="35px" Width="100px" BorderStyle="Outset"   />
</td>
</tr>
<tr>
<td align="center">
<asp:Button ID="btnReset" runat="server"  ToolTip="Clear" onclick="btnReset_Click" Text="Clear" 
        Height="35px" Width="100px" BorderStyle="Outset"   />
</td>
</tr>
</table>
<asp:UpdatePanel ID="upLogin" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
</ContentTemplate>
        </asp:UpdatePanel>
</td>
</tr>
</table>
<img alt="" height="1px" src="img/box_bottom_filet.gif" width="100%" />
<asp:Label ID="lblTranStatus" runat="server" Font-Size="8pt" Text="" Visible="false"></asp:Label>
<asp:Panel runat="server" ID="pnlEmp" Visible="false">
<asp:Label ID="Label1" runat="server" Font-Size="8pt" 
        Text="Search Employee by Name" style="font-weight: 700" Visible="False"></asp:Label>&nbsp; &nbsp;
<asp:TextBox ID ="txtName" runat="server" Text="" placeholder="Search By Name.." 
        Font-Size="8pt" Width="300px" 
        Height="22px" Visible="False"></asp:TextBox>
        
        <%--<ajaxToolkit:AutoCompleteExtender ID="autocomplieteExtrender" runat="server" ServiceMethod="GetEmployeInfo" ServicePath="AutoComplete.asmx" TargetControlID="txtName"  MinimumPrefixLength="1" CompletionInterval="20" EnableCaching="true" CompletionSetCount="12" />--%>
        &nbsp; &nbsp;<asp:Label ID="lblempID" runat="server" Visible="False"></asp:Label>
&nbsp;<asp:LinkButton ID="lbSearch" runat="server" Font-Size="10pt" 
        ForeColor="Blue" Text="Find" 
        onclick="lbSearch_Click" Font-Bold="True" Font-Strikeout="False" 
        Font-Underline="True" Visible="False"></asp:LinkButton>
<img alt="" height="1px" src="img/box_bottom_filet.gif" width="100%" />       
<asp:GridView CssClass="mGrid" PagerStyle-CssClass="pgr"  
        AlternatingRowStyle-CssClass="alt" ID="dgEmp" runat="server" 
        AutoGenerateColumns="False" Width="790px" BackColor="White" BorderWidth="1px" BorderStyle="Solid"
        CellPadding="2" CellSpaAMBg="0" BorderColor="LightGray" Font-Size="8pt" 
        AllowSorting="True" PageSize="15" ForeColor="#333333" 
        onpageindexchanging="dgEmp_PageIndexChanging" 
        onrowdatabound="dgEmp_RowDataBound">
  <Columns>
  <asp:TemplateField ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center">
  <ItemTemplate>  
  <asp:CheckBox ID="chkInc" runat="server" Checked="false" Visible="true" />
  </ItemTemplate>
      <ItemStyle HorizontalAlign="Center" Width="100px" />
  </asp:TemplateField>
  <asp:BoundField  HeaderText="ID" DataField="ID" ItemStyle-Width="200px" 
          ItemStyle-HorizontalAlign="Left">
      <ItemStyle HorizontalAlign="Left" Width="200px" />
      </asp:BoundField>
        <asp:TemplateField HeaderText="" ItemStyle-Width="40px">
                          <ItemTemplate>
                              <asp:Panel ID="panel1" runat="server">
                                 <a href="javascript:expandcollapse('div<%# Eval("ID") %>', 'one');">
                                  <img id="imgdiv<%# Eval("ID") %>" alt="Click to show/hide Maps"  width="25px"  src="img/plus.gif"/>
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
  <asp:BoundField  HeaderText="Emplyee No" DataField="emp_no" ItemStyle-Width="90px" 
          ItemStyle-HorizontalAlign="Center">
      <ItemStyle HorizontalAlign="Center" Width="90px" />
      </asp:BoundField>
  <asp:BoundField  HeaderText="Name" DataField="name" ItemStyle-Width="200px" 
          ItemStyle-HorizontalAlign="Left">
      <ItemStyle HorizontalAlign="Left" Width="200px" />
      </asp:BoundField>
  <asp:BoundField  HeaderText="DOB" DataField="dob" ItemStyle-Width="90px" 
          ItemStyle-HorizontalAlign="Center">
      <ItemStyle HorizontalAlign="Center" Width="90px" />
      </asp:BoundField>
  <asp:BoundField  HeaderText="Branch" DataField="branch" ItemStyle-Width="200px" 
          ItemStyle-HorizontalAlign="Left">  
      <ItemStyle HorizontalAlign="Left" Width="200px" />
      </asp:BoundField>
  <asp:BoundField  HeaderText="Designation" DataField="desig" ItemStyle-Width="200px" 
          ItemStyle-HorizontalAlign="Left">   
      <ItemStyle HorizontalAlign="Left" Width="200px" />
      </asp:BoundField>
        <asp:TemplateField>
                        <ItemTemplate>
                            <tr>
                                <td colspan="100%" align="center">
                                <div id="div<%# Eval("ID") %>" style=" display:none; position:relative; OVERFLOW:auto;" >
                                <asp:Panel Style="margin-left: 20px; margin-right: 5px;" ID="pnlSubMjrCat" runat="server" Visible="true">                                    
                                <asp:UpdatePanel ID="UpdatePanel1" runat="server"  UpdateMode="Conditional">
                                <ContentTemplate>
                               
                                    <asp:GridView ID="dgQuestion" runat="server" AutoGenerateColumns="False" 
                                                     CssClass="mGrid" onrowcommand="dgQuestion_RowCommand" onrowdatabound="dgSubject_RowDataBound" 
                                                     Width="100%">
                                                     <HeaderStyle BackColor="#DF5015" />
                                                     <Columns>
                                                         <asp:BoundField DataField="ID" HeaderText="Id">
                                                         <ItemStyle HorizontalAlign="Center" Width="8%" />
                                                         </asp:BoundField>
                                                         <asp:TemplateField HeaderText="SL No.">
                                                             <ItemTemplate>
                                                                 <%#Container.DataItemIndex+1%>
                                                             </ItemTemplate>
                                                             <ItemStyle HorizontalAlign="Center" Width="10%" />
                                                         </asp:TemplateField>
                                                         <asp:BoundField DataField="FileDescription" HeaderText="File Description">
                                                         <ItemStyle HorizontalAlign="Left" Width="50%" />
                                                         </asp:BoundField>
                                                         <asp:TemplateField>
                                                             <ItemTemplate>
                                                                 <asp:LinkButton ID="lnkDownload" runat="server" Font-Bold="True" CommandName="Download"
                                                                      Text="Download"></asp:LinkButton>
                                                             </ItemTemplate>
                                                             <ItemStyle HorizontalAlign="Center" Width="10%" /> 
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
                        <ItemStyle HorizontalAlign="Left" Width="10%" />
                        </asp:TemplateField>
  </Columns>
                        <RowStyle BackColor="white" />
                        <EditRowStyle BackColor="" />
                        <PagerStyle BackColor="" HorizontalAlign="Center" />
                        <HeaderStyle BackColor="Silver" Font-Bold="True" ForeColor="Black" />
                        <AlternatingRowStyle BackColor="" />
  </asp:GridView>
  </asp:Panel>
<asp:Panel runat="server" ID="pnlDiv" Visible="false">
<asp:LinkButton ID="lbDivIncl" runat="server" Font-Size="8pt" ForeColor="Blue" Text="Add selected" 
        onclick="lbDivIncl_Click"></asp:LinkButton>
<asp:GridView CssClass="mGrid" PagerStyle-CssClass="pgr"  AlternatingRowStyle-CssClass="alt" ID="dgDiv" runat="server" AutoGenerateColumns="false"
        AllowPaging="True" Width="790px" BackColor="White" BorderWidth="1px" BorderStyle="Solid"
        CellPadding="2" CellSpaAMBg="0" BorderColor="LightGray" Font-Size="8pt" 
        AllowSorting="true" PageSize="15" ForeColor="#333333" 
        onpageindexchanging="dgDiv_PageIndexChanging">
  <Columns>
  <asp:TemplateField ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center">
  <ItemTemplate>  
  <asp:CheckBox ID="chkInc" runat="server" Checked="false" Visible="true" />
  </ItemTemplate>
  </asp:TemplateField>
  <asp:BoundField  HeaderText="Division Code" DataField="division_code" ItemStyle-Width="90px" ItemStyle-HorizontalAlign="Center"/>
  <asp:BoundField  HeaderText="Name" DataField="division_name" ItemStyle-Width="200px" ItemStyle-HorizontalAlign="Left"/>
  </Columns>
                        <RowStyle BackColor="white" />
                        <EditRowStyle BackColor="" />
                        <PagerStyle BackColor="" HorizontalAlign="Center" />
                        <HeaderStyle BackColor="Silver" Font-Bold="True" ForeColor="Black" />
                        <AlternatingRowStyle BackColor="" />
  </asp:GridView>
  </asp:Panel>
<asp:Panel runat="server" ID="pnlDis" Visible="false">
<asp:LinkButton ID="lblDisIncl" runat="server" Font-Size="8pt" ForeColor="Blue" Text="Add selected" 
        onclick="lbDisIncl_Click"></asp:LinkButton>
<asp:GridView CssClass="mGrid" PagerStyle-CssClass="pgr"  AlternatingRowStyle-CssClass="alt" ID="dgDis" runat="server" AutoGenerateColumns="false"
        AllowPaging="True" Width="790px" BackColor="White" BorderWidth="1px" BorderStyle="Solid"
        CellPadding="2" CellSpaAMBg="0" BorderColor="LightGray" Font-Size="8pt" 
        AllowSorting="true" PageSize="15" ForeColor="#333333" 
        onpageindexchanging="dgDis_PageIndexChanging">
  <Columns>
  <asp:TemplateField ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center">
  <ItemTemplate>  
  <asp:CheckBox ID="chkInc" runat="server" Checked="false" Visible="true" />
  </ItemTemplate>
  </asp:TemplateField>
  <asp:BoundField  HeaderText="Division Code" Visible="false" DataField="division_code" ItemStyle-Width="200px" ItemStyle-HorizontalAlign="Center"/>
  <asp:BoundField  HeaderText="Division Name" DataField="division_name" ItemStyle-Width="200px" ItemStyle-HorizontalAlign="Center"/>
  <asp:BoundField  HeaderText="District Code" DataField="district_code" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center"/>
  <asp:BoundField  HeaderText="Name" DataField="district_name" ItemStyle-Width="200px" ItemStyle-HorizontalAlign="Left"/>
  </Columns>
                        <RowStyle BackColor="white" />
                        <EditRowStyle BackColor="" />
                        <PagerStyle BackColor="" HorizontalAlign="Center" />
                        <HeaderStyle BackColor="Silver" Font-Bold="True" ForeColor="Black" />
                        <AlternatingRowStyle BackColor="" />
  </asp:GridView>
    <table style="width: 100%">
        <tr>
            <td style="width: 80%">
                &nbsp;</td>
            <td style="width: 20%">
                &nbsp;</td>
        </tr>
    </table>
  </asp:Panel> 
<asp:Panel runat="server" ID="pnlThana" Visible="false">
<asp:LinkButton ID="lblThanaIncl" runat="server" Font-Size="8pt" ForeColor="Blue" Text="Add selected" 
        onclick="lblThanaIncl_Click"></asp:LinkButton>
<asp:GridView CssClass="mGrid" PagerStyle-CssClass="pgr"  AlternatingRowStyle-CssClass="alt" ID="dgThana" runat="server" AutoGenerateColumns="false"
        AllowPaging="True" Width="790px" BackColor="White" BorderWidth="1px" BorderStyle="Solid"
        CellPadding="2" CellSpaAMBg="0" BorderColor="LightGray" Font-Size="8pt" 
        AllowSorting="true" PageSize="15" ForeColor="#333333"
        onpageindexchanging="dgThana_PageIndexChanging">
  <Columns>
  <asp:TemplateField ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center">
  <ItemTemplate>  
  <asp:CheckBox ID="chkInc" runat="server" Checked="false" Visible="true" />
  </ItemTemplate>
  </asp:TemplateField>
  <asp:BoundField  HeaderText="Division Code" Visible="false" DataField="division_code" ItemStyle-Width="200px" ItemStyle-HorizontalAlign="Center"/>
  <asp:BoundField  HeaderText="Division Name" DataField="division_name" ItemStyle-Width="200px" ItemStyle-HorizontalAlign="Center"/>
  <asp:BoundField  HeaderText="District Code" Visible="false" DataField="district_code" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center"/>
  <asp:BoundField  HeaderText="District Name" DataField="district_name" ItemStyle-Width="150px" ItemStyle-HorizontalAlign="Center"/>
  <asp:BoundField  HeaderText="Thana Code" DataField="thana_code" ItemStyle-Width="90px" ItemStyle-HorizontalAlign="Center"/>
  <asp:BoundField  HeaderText="Name" DataField="thana_name" ItemStyle-Width="200px" ItemStyle-HorizontalAlign="Left"/>
  </Columns>
                        <RowStyle BackColor="white" />
                        <EditRowStyle BackColor="" />
                        <PagerStyle BackColor="" HorizontalAlign="Center" />
                        <HeaderStyle BackColor="Silver" Font-Bold="True" ForeColor="Black" />
                        <AlternatingRowStyle BackColor="" />
  </asp:GridView>
  </asp:Panel>  
<asp:Panel runat="server" ID="pnlBranch" Visible="false">
<asp:LinkButton ID="lbBranchIncl" runat="server" Font-Size="8pt" ForeColor="Blue" Text="Add selected" 
        onclick="lbBranchIncl_Click" Visible="False"></asp:LinkButton>
<asp:GridView CssClass="mGrid" PagerStyle-CssClass="pgr"  
        AlternatingRowStyle-CssClass="alt" ID="dgBranch" runat="server" AutoGenerateColumns="false"
        AllowPaging="True" Width="40%" BackColor="White" BorderWidth="1px" BorderStyle="Solid"
        CellPadding="2" CellSpaAMBg="0" BorderColor="LightGray" Font-Size="8pt" 
        AllowSorting="true" PageSize="15" ForeColor="#333333" 
        onpageindexchanging="dgBranch_PageIndexChanging" 
        onrowdatabound="dgBranch_RowDataBound">
  <Columns>
  <asp:TemplateField ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center">
  <ItemTemplate>  
  <asp:CheckBox ID="chkInc" runat="server" Checked="false" Visible="true" />
  </ItemTemplate>
      <ItemStyle HorizontalAlign="Center" Width="10%" />
  </asp:TemplateField>
  <asp:BoundField  HeaderText="Branch Code" DataField="ID" 
          ItemStyle-Width="150px" ItemStyle-HorizontalAlign="Center">
      <ItemStyle HorizontalAlign="Center" Width="150px" />
      </asp:BoundField>
  <asp:BoundField  HeaderText="Name" DataField="BranchName" ItemStyle-Width="200px" 
          ItemStyle-HorizontalAlign="Left">
      <ItemStyle HorizontalAlign="Left" Width="200px" />
      </asp:BoundField>
  </Columns>
                        <RowStyle BackColor="white" />
                        <EditRowStyle BackColor="" />
                        <PagerStyle BackColor="" HorizontalAlign="Center" />
                        <HeaderStyle BackColor="Silver" Font-Bold="True" ForeColor="Black" />
                        <AlternatingRowStyle BackColor="" />
  </asp:GridView>
  </asp:Panel> 
<asp:Panel runat="server" ID="pnlDesig" Visible="false">
<asp:LinkButton ID="lbDesigIncl" runat="server" Font-Size="8pt" ForeColor="Blue" Text="Add selected" 
        onclick="lbDesigIncl_Click"></asp:LinkButton>
    <table style="width: 100%">
        <tr>
            <td style="width: 60%" align="right" valign="top">
                <asp:GridView ID="dgDesig" runat="server" AllowPaging="True" 
                    AllowSorting="true" AlternatingRowStyle-CssClass="alt" 
                    AutoGenerateColumns="false" BackColor="White" BorderColor="LightGray" 
                    BorderStyle="Solid" BorderWidth="1px" CellPadding="2" CellSpaAMBg="0" 
                    CssClass="mGrid" Font-Size="8pt" ForeColor="#333333" 
                    onpageindexchanging="dgDesig_PageIndexChanging" 
                    onrowdatabound="dgDesig_RowDataBound" PagerStyle-CssClass="pgr" PageSize="15" 
                    Width="40%">
                    <Columns>
                        <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="100px">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkInc" runat="server" AutoPostBack="True" Checked="false" 
                                    Visible="true" oncheckedchanged="lbDesigIncl_Click" />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" Width="100px" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="desig_code" HeaderText="Desig Code" 
                            ItemStyle-HorizontalAlign="Center" ItemStyle-Width="150px">
                        <ItemStyle HorizontalAlign="Center" Width="150px" />
                        </asp:BoundField>
                        <asp:BoundField DataField="desig_name" HeaderText="Name" 
                            ItemStyle-HorizontalAlign="Left" ItemStyle-Width="200px">
                        <ItemStyle HorizontalAlign="Left" Width="200px" />
                        </asp:BoundField>
                    </Columns>
                    <RowStyle BackColor="white" />
                    <EditRowStyle BackColor="" />
                    <PagerStyle BackColor="" HorizontalAlign="Center" />
                    <HeaderStyle BackColor="Silver" Font-Bold="True" ForeColor="Black" />
                    <AlternatingRowStyle BackColor="" />
                </asp:GridView>
            </td>
            <td style="width: 40%" valign="top" align="left">
                <asp:GridView ID="dgSelectDesignation" runat="server" AllowPaging="True" 
                    AllowSorting="True" AlternatingRowStyle-CssClass="alt" 
                    AutoGenerateColumns="False" BackColor="White" BorderColor="LightGray" 
                    BorderStyle="Solid" BorderWidth="1px" CellPadding="2" CellSpaAMBg="0" 
                    CssClass="mGrid" Font-Size="8pt" ForeColor="#333333" PagerStyle-CssClass="pgr" 
                    PageSize="15" Width="40%" Visible="False">
                    <Columns>
                        <asp:BoundField DataField="desig_name" HeaderText="Name" 
                            ItemStyle-HorizontalAlign="Left" ItemStyle-Width="200px">
                        <ItemStyle HorizontalAlign="Left" Width="200px" />
                        </asp:BoundField>
                    </Columns>
                    <RowStyle BackColor="white" />
                    <EditRowStyle BackColor="" />
                    <PagerStyle BackColor="" HorizontalAlign="Center" />
                    <HeaderStyle BackColor="Silver" Font-Bold="True" ForeColor="Black" />
                    <AlternatingRowStyle BackColor="" />
                </asp:GridView>
            </td>
        </tr>
    </table>
  </asp:Panel>    
</td>
<td style="width:10%;"></td>
</tr>
</table>    
</div>
<%--<CR:CrystalReportViewer ID="CrystalReportViewer1" runat="server" 
    AutoDataBind="true" />--%>
</asp:Content>

