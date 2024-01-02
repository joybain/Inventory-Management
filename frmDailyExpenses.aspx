<%@ Page Title=" Daily Expenses. Riders" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="frmDailyExpenses.aspx.cs" Inherits="frmDailyExpenses" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
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
    <table style="width: 100%">
        <tr>
            <td style="width: 17%; height: 17px;">
                </td>
            <td style="width: 68%; height: 17px;">
                </td>
            <td style="width: 20%; height: 17px;">
                </td>
        </tr>
        <tr>
            <td style="width: 17%">
                &nbsp;</td>
            <td style="width: 68%; color: maroon;"> <h3>
                &gt;&gt; Daily Expenses Entry </h3></td>
            <td style="width: 20%">
                &nbsp;</td>
        </tr>
        <tr>
            <td style="width: 17%">
                &nbsp;</td>
            <td style="width: 68%">
        <ajaxtoolkit:TabContainer ID="tabVch" runat="server" Width="99%" ActiveTabIndex="1" 
                    Font-Size="8pt">
            <ajaxtoolkit:TabPanel ID="tpVchDtl" runat="server" 
                HeaderText="Items Information">
                <HeaderTemplate>
                    Expenses Entry
                </HeaderTemplate>
                <ContentTemplate>
                    <fieldset style="vertical-align: top; border: solid 1px #8BB381;text-align:left;"><legend style="color: maroon;"><b>Expenses Info</b> </legend>
                        <table style="width: 100%">
                            <tr>
                                <td style="width: 18%; height: 27px; font-weight: 700;" align="right">
                                    Code</td>
                                <td style="width: 2%; height: 27px; font-weight: 700;" align="center" ID=":">
                                    :</td>
                                <td style="width: 24%; height: 27px;">
                                    <asp:TextBox ID="txtCode" runat="server" Enabled="False"></asp:TextBox>
                                    <asp:HiddenField ID="hfID" runat="server" />
                                </td>
                                <td style="width: 17%; height: 27px;" align="right">
                                    Expenses
                                    Date</td>
                                <td style="width: 5%; height: 27px; font-weight: 700;" align="center">
                                    :</td>
                                <td style="width: 20%; height: 27px;">
                                    <asp:TextBox ID="txtDate" runat="server" Width="93%"></asp:TextBox>
                                    <ajaxtoolkit:calendarextender runat="server" ID="txtGRNODate_CalendarExtender" 
                                                                  TargetControlID="txtDate" Format="dd/MM/yyyy" 
                                        Enabled="True"/>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 18%; height: 29px;" align="right">
                                    Remarks</td>
                                <td style="width: 2%; height: 29px; font-weight: 700;" align="center">
                                    :</td>
                                <td style="height: 29px;" colspan="4">
                                    <asp:TextBox ID="txtRemarks" runat="server" Width="98%"></asp:TextBox>
                                </td>
                            </tr>
                        </table>
                    </fieldset>
                    <br/>
                    <div>
                       <asp:UpdatePanel runat="server" ID="UP1" UpdateMode="Conditional"><ContentTemplate>
   <asp:GridView runat="server" AutoGenerateColumns="False" BorderColor="LightGray" 
                    CssClass="mGrid" Font-Size="9pt" Width="100%" ID="dgPVDetailsDtl" 
                    OnRowDataBound="dgPurDtl_RowDataBound" OnRowDeleting="dgPurDtl_RowDeleting" 
                    OnRowCommand="dgPVDetailsDtl_RowCommand">
<AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
<Columns>
<asp:TemplateField><ItemTemplate>
                <asp:ImageButton ID="lbDelete" runat="server" CausesValidation="False" 
                        CommandName="Delete" ImageAlign="Middle" ImageUrl="~/img/delete.png" 
                        Text="Delete" />
                
</ItemTemplate>

<ItemStyle HorizontalAlign="Center" Font-Size="8pt" Width="4%"></ItemStyle>
</asp:TemplateField>
    <asp:BoundField DataField="GL_COA_CODE" HeaderText="GL_CODE">
    <ItemStyle Width="5%"></ItemStyle>
    </asp:BoundField>
<asp:TemplateField HeaderText="ID"><ItemTemplate>
                <asp:Label ID="lblid" runat="server" Font-Size="8pt" Width="95%" 
                    Text='<%#Eval("ID")%>'></asp:Label>
            
</ItemTemplate>

<ItemStyle HorizontalAlign="Center" Width="10%"></ItemStyle>
</asp:TemplateField>
<asp:TemplateField HeaderText="Expenses Head"><ItemTemplate>
                <asp:TextBox ID="txtItemDesc" runat="server" autocomplete="off" AutoPostBack="true" 
                        CssClass="txtVisibleAlign" Font-Size="8pt" 
                        placeHolder="Search By expenses head.."
                SkinId="tbPlain" Text='<%#Eval("ExpensesHead")%>' Height="18px" Width="95%" 
                        onFocus="this.select()" ontextchanged="txtItemDesc_TextChanged"></asp:TextBox>
                <ajaxToolkit:AutoCompleteExtender ServicePath="AutoComplete.asmx" runat="server" 
                        ID="autoComplete1" TargetControlID="txtItemDesc"
                ServiceMethod="GetItemExpensesSearch" MinimumPrefixLength="1" CompletionInterval="20" 
                        EnableCaching="true" CompletionSetCount="12"/>
                
</ItemTemplate>

<ItemStyle HorizontalAlign="Left" Width="25%"></ItemStyle>
</asp:TemplateField>
<asp:TemplateField HeaderText="Amount"><ItemTemplate>
                <asp:TextBox ID="txtItemRate" runat="server" Height="18px" AutoPostBack="True" 
                        CssClass="tbr" placeHolder="0.00" autocomplete="off"
                        Font-Size="8pt" SkinId="tbPlain" Text='<%#Eval("Amount")%>' Width="90%" 
                        onFocus="this.select()" ontextchanged="txtItemRate_TextChanged"></asp:TextBox>
                
</ItemTemplate>

<ItemStyle HorizontalAlign="Right" Width="8%"></ItemStyle>
</asp:TemplateField>
</Columns>

<HeaderStyle Font-Bold="True" Font-Size="9pt" ForeColor="White"></HeaderStyle>

<PagerStyle HorizontalAlign="Center" CssClass="pgr" ForeColor="White"></PagerStyle>

<RowStyle BackColor="White"></RowStyle>
</asp:GridView>
                           <asp:TextBox ID="txtTotal" runat="server" Visible="False"></asp:TextBox>
                </ContentTemplate></asp:UpdatePanel>
                    </div>
                </ContentTemplate>
</ajaxtoolkit:TabPanel>                                            
<ajaxtoolkit:TabPanel ID="tpVchHist" runat="server" HeaderText="Voucher History">
    <HeaderTemplate>
        Expenses History
    </HeaderTemplate>
    <ContentTemplate>
        <div></div>
    <div>

        <asp:GridView ID="dgHistory" runat="server" AutoGenerateColumns="False" 
            CssClass="mGrid" onrowdatabound="dgHistory_RowDataBound" 
            onselectedindexchanged="dgHistory_SelectedIndexChanged" Width="100%" 
            AllowPaging="True" onpageindexchanging="dgHistory_PageIndexChanging" 
            PageSize="50">
            <Columns>
                <asp:CommandField ShowSelectButton="True" />
                <asp:BoundField DataField="ID" HeaderText="ID" />
                <asp:BoundField DataField="Code" HeaderText="Code" />
                <asp:BoundField DataField="ExpDate" HeaderText="Date" />
                <asp:BoundField DataField="Total" HeaderText="Total Expenses" />
                <asp:BoundField DataField="Remarks" HeaderText="Remarks" />
            </Columns>
        </asp:GridView>

    </div>
       
    </ContentTemplate>
</ajaxtoolkit:TabPanel>
</ajaxtoolkit:TabContainer>

            </td>
            <td style="width: 20%">
                &nbsp;</td>
        </tr>
        <tr>
            <td style="width: 17%">
                &nbsp;</td>
            <td style="width: 68%">
                &nbsp;</td>
            <td style="width: 20%">
                &nbsp;</td>
        </tr>
        <tr>
            <td style="width: 17%">
                &nbsp;</td>
            <td style="width: 68%">
                &nbsp;</td>
            <td style="width: 20%">
                &nbsp;</td>
        </tr>
        <tr>
            <td style="width: 17%">
                &nbsp;</td>
            <td style="width: 68%">
                &nbsp;</td>
            <td style="width: 20%">
                &nbsp;</td>
        </tr>
        <tr>
            <td style="width: 17%">
                &nbsp;</td>
            <td style="width: 68%">
                &nbsp;</td>
            <td style="width: 20%">
                &nbsp;</td>
        </tr>
        <tr>
            <td style="width: 17%">
                &nbsp;</td>
            <td style="width: 68%">
                &nbsp;</td>
            <td style="width: 20%">
                &nbsp;</td>
        </tr>
    </table>
    </div>
</asp:Content>

