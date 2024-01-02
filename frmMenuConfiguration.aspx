<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="frmMenuConfiguration.aspx.cs" Inherits="frmMenuConfiguration" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    
     <script language="javascript" type="text/javascript" >
         function isNumber(evt) {
             evt = (evt) ? evt : window.event;
             var charCode = (evt.which) ? evt.which : evt.keyCode;
             if (charCode != 46 && charCode > 31
            && (charCode < 48 || charCode > 57)) {
                 return false;
             }
             return true;
         }
       
</script>
      <div id="frmMainDiv" style="background-color:White; width:100%;">
<table id="pageFooterWrapper">
    <tr>
        <td align="center" style="vertical-align:middle; height:100%;">
            <asp:Button ID="DeleteButton" runat="server" CssClass="buttonclass" 
                onclick="DeleteButton_Click" Text="Delete" Width="100px" Height="35px" />
        </td>
        <td align="center" style="vertical-align:middle;">
            &nbsp;</td>
        <td align="center" style="vertical-align:middle; height:100%;">
            <asp:Button ID="btnSave" runat="server" CssClass="buttonclass" 
                onclick="btnSave_Click" Text="Save" Width="100px" Height="35px" />
        </td>
        <td align="center" style="vertical-align:middle;">
            <asp:Button ID="CloseButton" runat="server" CssClass="buttonclass" 
                onclick="CloseButton_Click" Text="Clear" Width="100px" Height="35px" />
        </td>
    </tr>
</table>
        <table style="width:100%;">
            <tr>
                <td align="center">&nbsp;</td>
            </tr>
            <tr>
                <td align="right" class="style1" style="height:14px; ">
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 20%; height: 68px">
                            </td>
                            <td style="width:60%; height: 68px">
                            <fieldset style=" text-align:left; vertical-align: top; border: solid 1px #8BB381;"><legend style="color: maroon;">
                                <b>Menu Setting</b></legend>
                                <table style="width:100%; font-size:8pt;">
                                    <tr>
                                        <td align="right" style="width: 30%; height: 5px">
                                            </td>
                                        <td style="width: 5%; height: 5px; text-align: center;">
                                        </td>
                                        <td align="left"  style="height: 5px;width: 65%">
                                            <asp:HiddenField ID="hfId" runat="server" />
                                        </td>
                                    </tr>
                                      <tr>
                                        <td align="right" style="width: 30%; height: 5px; font-weight: 700;">
                                            Manu Name</td>
                                        <td style="width: 5%; height: 5px; text-align: center;">
                                            <strong>:</strong></td>
                                        <td align="left"  style="height: 5px;width: 65%">
                                            <asp:DropDownList ID="ddlManuName" runat="server" Width="230px" Height="30px">
                                            </asp:DropDownList>
                                          </td>
                                    </tr>
                                       <tr>
                                        <td align="right" style="width: 30%; height: 5px; font-weight: 700;">
                                          Parent  Manu Name</td>
                                        <td style="width: 5%; height: 5px; text-align: center;">
                                            <strong>:</strong></td>
                                        <td align="left"  style="height: 5px;width: 65%">
                                            <asp:DropDownList ID="ddlParentMenuId" runat="server" Width="230px" Height="30px">
                                            </asp:DropDownList>
                                          </td>
                                    </tr>
                                    <tr>
                                        <td align="right" style="width:30%; height: 34px; font-weight: 700;">
                                            Name
                                        </td>
                                        <td style="width:5%; height: 34px; text-align: center;">
                                            <strong>:</strong></td>
                                        <td align="left" style="height: 34px;width:65%;">
                                            <asp:TextBox ID="txtName" runat="server" CssClass="TextBox" Height="20px" 
                                                SkinID="tbPlain" Width="220px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right" style="width:30%; height: 34px;">
                                            <strong>Path</strong></td>
                                        <td style="width:5%; height: 34px; text-align: center;">
                                            <b>:</b></td>
                                        <td align="left" style="height: 34px;width:65%;">
                                            <asp:TextBox ID="txtPath" runat="server" CssClass="TextBox" Height="20px" 
                                                SkinID="tbPlain" Width="220px"></asp:TextBox>
                                        </td>
                                    </tr>
                                        <tr>
                                        <td align="right" style="width: 30%; height: 5px">
                                            <strong>Priority</strong></td>
                                        <td style="width: 5%; height: 5px; text-align: center;">
                                            :</td>
                                        <td align="left"  style="height: 5px;width: 65%">
                                            <asp:TextBox ID="txtPriority" runat="server" CssClass="TextBox" Height="20px" onkeypress="return isNumber(event)"
                                                SkinID="tbPlain" Width="220px"></asp:TextBox>
                                            </td>
                                    </tr>
                                </table></fieldset>
                            </td>
                            <td style=" width:20%; height: 68px">
                            </td>
                        </tr>
                        <tr>
                             <td style="width: 20%; height: 68px"></td>
                              <td style="width: 60%; height: 68px">
                                   <fieldset style=" text-align:left; vertical-align: top; border: solid 1px #8BB381;"><legend style="color: maroon;">
                                <b>Menu List</b></legend>
                               <table style="width: 100%">
                                   <tr>
                               <td style="width: 100%">
                                  <asp:GridView ID="gdvConfigruation" runat="server" AllowPaging="True" 
                        AutoGenerateColumns="False"
                       
                         CssClass="mGrid" 
                        onselectedindexchanged="dgColor_SelectedIndexChanged" PageSize="30" 
                        Width="100%" onrowdatabound="dgColor_RowDataBound">
                        <Columns>
                            <asp:CommandField ShowSelectButton="True" >
                            <ItemStyle Width="10%" />
                            </asp:CommandField>
                            <asp:BoundField DataField="Id" HeaderText="ID" >
                             <ItemStyle Width="5%" />
                            </asp:BoundField>
                             <asp:BoundField DataField="MenuId" HeaderText="Manu Id" >
                            <HeaderStyle Width="5%" />
                            </asp:BoundField>
                            <asp:BoundField DataField="MenuName" HeaderText="ManuName" >
                            <HeaderStyle HorizontalAlign="Left" />
                            <ItemStyle HorizontalAlign="Left" Width="30%" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Name" HeaderText="Name" >
                            <HeaderStyle HorizontalAlign="Left" />
                            <ItemStyle HorizontalAlign="Left" Width="30%" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Path" HeaderText="Path" >
                             <HeaderStyle HorizontalAlign="Left" />
                            <ItemStyle HorizontalAlign="Left" Width="25%" />
                            </asp:BoundField>
                             <asp:BoundField DataField="Priority" HeaderText="Priority" >
                            <ItemStyle Width="5%" />
                            </asp:BoundField>
                        </Columns>
                    </asp:GridView> 
                    </td>
                    </tr>
                               </table>
                                </fieldset>
                              </td>
                              <td style="width: 20%; height: 68px"></td>
                        </tr>
                    </table>
                </td>
            </tr>
       <%--     <tr>
                
               
                <td align="center" style="height:400px;" valign="top">
                     
                </td>
            </tr>--%>
        </table>
    
    </div>
</asp:Content>

