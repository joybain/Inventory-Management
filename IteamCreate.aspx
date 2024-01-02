<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="IteamCreate.aspx.cs" Inherits="IteamCreate" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">

<script type="text/javascript">
    //   description

    

</script>
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
<table style="width:100%;background-color:White;">
<tr>
<td style="width:5%"></td>
<td style="width:95%">
    
       
              
                    <table style="width:100%; background-color:White;">
                        <tr>
                            <td style="width:2%;">
                                &nbsp;</td>
                            <td align="center" style="width:80%;">
                                <table style="width:100%;">
                                    <tr>
                                        <td align="left" style="width: 20%;">
                                            &nbsp;</td>
                                        <td align="center" style="width: 60%;">
                                            &nbsp;</td>
                                        <td align="right" style="width: 20%;">
                                            &nbsp;</td>
                                    </tr>
                                    <tr>
                                        <td align="left" style="width: 20%;">
                                            <asp:Label ID="lblItemSetupID" runat="server" Visible="False"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" colspan="3">
                                            <fieldset style="vertical-align: top; border: solid .5px #8BB381;text-align:left;line-height:1.5em;">
                                                <legend style="color: maroon;"><b>Item setup Information</b></legend>
                                                <asp:UpdatePanel ID="UP1" runat="server" UpdateMode="Conditional">
                                                    <ContentTemplate>
                                                        <table style="width:100%">
                                                            <tr>
                                                                <td align="left" style="width:15%; height: 26px;" width="15%">
                                                                    <asp:Label ID="Label22" runat="server" Text="Code"></asp:Label>
                                                                </td>
                                                                <td style="height: 26px">
                                                                    <asp:TextBox ID="txtCode" runat="server" Width="90%" Enabled="False"></asp:TextBox>
                                                                </td>
                                                                <td style="width:5%; height: 26px;">
                                                                </td>
                                                                <td style="width:5%; height: 26px;">
                                                                    <asp:CheckBox ID="chkActiveSetup" runat="server" Text="Active?" 
                                                                        Visible="False" Checked="True" />
                                                                </td>
                                                                <td style="height: 26px">
                                                                    </td>
                                                                <td align="center" width="10%" style="height: 26px">
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td align="left" style="width:15%; height: 26px; font-weight: 700;" width="15%">
                                                                    Item Name</td>
                                                                <td colspan="4" style="height: 26px">
                                                                    <asp:TextBox ID="txtName" runat="server" Width="96%" Height="18px"></asp:TextBox>
                                                                    &nbsp;
                                                                    </td>
                                                                <td align="center" width="10%" style="height: 26px">
                                                                    </td>
                                                            </tr>
                                                       
                                                            <tr>
                                                                <td align="left" style="width:15%; height: 27px;">
                                                                    Short Name</td>
                                                                <td style="width:30%; height: 27px;">
                                                                    <asp:TextBox ID="txtShortName" runat="server" Width="90%"></asp:TextBox>
                                                                </td>
                                                                <td align="center" style="width:5%; height: 27px;">
                                                                    </td>
                                                                <td style="height: 27px;" align="center" colspan="2">
                                                                    <asp:RadioButtonList ID="rdbItemType" runat="server" 
                                                                        RepeatDirection="Horizontal" Visible="False">
                                                                        <asp:ListItem Selected="True" Value="0">Product</asp:ListItem>
                                                                        <asp:ListItem Value="1">Fabrics</asp:ListItem>
                                                                        <asp:ListItem Value="2">Accessories/Matrials</asp:ListItem>
                                                                    </asp:RadioButtonList>
                                                                </td>
                                                                <td align="center" style="width:5%; height: 27px;" width="10%">
                                                                    </td>
                                                            </tr>
                                                           
                                                        </table>
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
                                            </fieldset>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" colspan="3">
                                            <table ID="pageFooterWrapper">
                                                <tr>
                                                    <td align="center">
                                                        <asp:Button ID="btnDeleteSetup" runat="server" BorderStyle="Outset" Height="35px" 
                                                            onclientclick="javascript:return window.confirm('are u really want to delete  these data')" 
                                                            Text="Delete" ToolTip="Delete Record" Width="100px" 
                                                            onclick="btnDeleteSetup_Click" />
                                                    </td>
                                                    <td align="center">
                                                        &nbsp;</td>
                                                    <td align="center">
                                                        &nbsp;</td>
                                                    <td align="center">
                                                        <asp:Button ID="BtnSaveSetup" runat="server" BorderStyle="Outset" Height="35px" 
                                                            OnClick="lblSave_Click" Text="Save" ToolTip="Save or Update Record" 
                                                            Width="100px" />
                                                    </td>
                                                    <td align="center">
                                                        <asp:Button ID="BtnResetSetup" runat="server" BorderStyle="Outset" Height="35px" 
                                                            Text="Clear" ToolTip="Clear Form" Width="100px" 
                                                            onclick="BtnResetSetup_Click1" />
                                                    </td>
                                                    <td align="center">
                                                        <asp:Button ID="btnSetupPrint" runat="server" BorderStyle="Outset" 
                                                            Height="35px" Text="Print" ToolTip="Print" 
                                                            Width="100px" Visible="False" />
                                                    </td>
                                                </tr>
                                            </table>
           <fieldset style="vertical-align: top; border: solid 1px #8BB381;"><legend style="color: maroon;"><b> Search Option</b></legend>
                <table style="width: 100%;">
                        <tr>
                            <td style="width: 25%; text-align: center">Search By Code/Name :</td>
                            <td >
                                        <asp:TextBox ID="txtSearchSetupItem" placeholder="search by items..!!" runat="server" Width="98%"></asp:TextBox>
                                <ajaxtoolkit:AutoCompleteExtender ID="AutoCompleteExtender1"
                                                        runat="server" CompletionInterval="20" CompletionSetCount="30" MinimumPrefixLength="1"
                                                        ServiceMethod="GetSetupItem" ServicePath="~/AutoComplete.asmx"
                                                        TargetControlID="txtSearchSetupItem" DelimiterCharacters="" 
                                            Enabled="True">
                                                    </ajaxtoolkit:AutoCompleteExtender>
                                    </td>
                            <td style="width: 15%; text-align: center">
                                <asp:LinkButton ID="lbSearch" runat="server" Font-Bold="True" Font-Size="12pt" OnClick="lbSearch_Click" Width="90%" BorderStyle="Solid">Search</asp:LinkButton>
                            </td>
                            <td style="width: 20%;text-align: center">
                                <table style="width: 100%;">
                                    <tr>
                                        <td style="width: 80%;text-align: center;">
                                            <asp:LinkButton ID="lbClear" runat="server" Font-Bold="True" Font-Size="12pt" OnClick="lbClear_Click" Width="90%" BorderStyle="Solid">Clear</asp:LinkButton>
                                        </td>
                                        <td style="width: 20%;">&nbsp;</td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        </table>
                        </fieldset>                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" colspan="3">
                                            <asp:GridView ID="dgItemSetupHistory" runat="server" AllowPaging="True" 
                                                CssClass="mGrid" Font-Size="Small" 
                                                onpageindexchanging="dgHistory_PageIndexChanging" 
                                                onrowdatabound="dgHistory_RowDataBound" 
                                                onselectedindexchanged="dgHistory_SelectedIndexChanged" PageSize="50" 
                                                style="text-align:center;" Width="100%">
                                                <Columns>
                                                    <asp:CommandField ShowSelectButton="True" />
                                                </Columns>
                                                <HeaderStyle Font-Bold="True" Height="25px" />
                                            </asp:GridView>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" style="width: 20%;">
                                            &nbsp;</td>
                                        <td style="width: 60%;">
                                            <table style="width: 100%">
                                                <tr>
                                                    <td style="width:33%">
                                                        &nbsp;</td>
                                                    
                                                    <td style="width:33%">
                                                        &nbsp;</td>
                                                    <td style="width:33%">
                                                        &nbsp;</td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td style="width: 20%;">
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td style="width:18%;">
                                &nbsp;</td>
                        </tr>
                    </table>
                
    
    </td>
<td style="width:10%"></td>
</tr>
</table>
</asp:Content>

