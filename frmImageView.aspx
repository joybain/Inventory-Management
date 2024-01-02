<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmImageView.aspx.cs" Inherits="frmImageView" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

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

</head>
<body>
    <form id="form1" runat="server">
    <div align="center">
    
    </div>
    <div align="center">
    
        <asp:Label ID="lblImage" runat="server"></asp:Label>
    
    </div>
     <div align="center">
    
    </div>
     <div align="center">
    
         <asp:Image ID="Image1" runat="server" Height="170px" Width="167px" 
             BorderColor="#CC3300" BorderStyle="Inset" />
    
    </div>
     <div>
    
    </div>
    </form>
</body>
</html>
