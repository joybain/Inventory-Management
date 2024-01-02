<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmImage.aspx.cs" Inherits="frmImage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style>
        input[type=text], input[type=password], select {
            background: #ffffff url("../../images/bg_ip.png") repeat-x;
            padding: 3px;
            font-size: 10px;
            color: #000000;
            font-weight: bold;
            margin: 0;
            border: 1px solid #c0c0c0;
            height: 18px;
        }

        input[type=submit], input[type=button] {
            /*background-color:transparent;	*/
            background: #999999;
            color: Black;
            -webkit-border-radius: 4px;
            -moz-border-radius: 4px;
            border-radius: 4px;
            border: solid 1px #20538D;
            text-shadow: 0 -1px 0 rgba(0, 0, 0, 0.4);
            -webkit-box-shadow: inset 0 1px 0 rgba(255, 255, 255, 0.4), 0 1px 1px rgba(0,0, 0, 0.2);
            -moz-box-shadow: inset 0 1px 0 rgba(255, 255, 255, 0.4), 0 1px 1px rgba(0, 0, 0, 0.2);
            box-shadow: inset 0 1px 0 rgba(255, 255, 255, 0.4), 0 1px 1px rgba(0, 0, 0, 0.2);
            -webkit-transition-duration: 0.2s;
            -moz-transition-duration: 0.2s;
            transition-duration: 0.2s;
            -webkit-user-select: none;
            -moz-user-select: none;
            -ms-user-select: none;
            user-select: none;
            font-weight: bold;
        }
    </style>
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

         //    window.onload = maxWindow;

         //    function maxWindow() {
         //        window.moveTo(0, 0);


         //        if (document.all) {
         //            top.window.resizeTo(screen.availWidth, screen.availHeight);
         //        }

         //        else if (document.layers || document.getElementById) {
         //            if (top.window.outerHeight < screen.availHeight || top.window.outerWidth < screen.availWidth) {
         //                top.window.outerHeight = screen.availHeight;
         //                top.window.outerWidth = screen.availWidth;
         //            }
         //        }
         //    }
         function SetImage() {
             document.getElementById('<%=btnUpload.ClientID %>').click();
         }

</script>
</head> 
<body>
    <form id="form1" runat="server">
    <div>    
        <table style="width: 100%">           
            <tr>
                <td align="center">
                    &nbsp;</td>
            </tr>
            <tr>
                <td align="center">
                    <asp:FileUpload ID="imgUpload" runat="server" Font-Size="8pt" Height="20px" 
                        onchange="javascript:SetImage();" Size="20%" BackColor="#CCCCCC" />
                    <asp:Button ID="btnUpload" runat="server" OnClick="btnUpload_Click" 
                        style="display:none;" Text="Update" Width="120px" />
                </td>
            </tr>
            <tr>
                <td align="center" style="vertical-align: top;">
                    <asp:GridView ID="dgImage" runat="server" AutoGenerateColumns="False" 
                        CssClass="mGrid" OnRowDataBound="dgImage_RowDataBound" 
                        OnRowDeleting="dgImage_RowDeleting" PageSize="2" Width="25%">
                        <Columns>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:ImageButton ID="lbDelete" runat="server" CausesValidation="False" 
                                        CommandName="Delete" ImageAlign="Middle" ImageUrl="~/img/delete.png" 
                                        Text="Delete" />
                                </ItemTemplate>
                                <ItemStyle Font-Size="8pt" HorizontalAlign="Center" Width="2%" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Product Image">
                                <ItemTemplate>
                                    <asp:Image ID="Image1" runat="server" Height="150px" 
                                        ImageUrl='<%# "HTTPHandler.ashx?ImID="+ Eval("ImageID") %>' Width="150px" />
                                </ItemTemplate>
                                <ItemStyle Font-Size="8pt" HorizontalAlign="Center" Width="4%" />
                            </asp:TemplateField>
                            <asp:BoundField DataField="ImageID" HeaderText="ID" />
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
            <tr>
                <td align="left" style="vertical-align:top;">
                    &nbsp;</td>
            </tr>
        </table>
    
    </div>
    </form>
</body>
</html>
