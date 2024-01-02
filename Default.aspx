<%@ Page Title="Shop." Language="C#" AutoEventWireup="true" CodeFile="default.aspx.cs" Inherits="_Default" Theme="Themes" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Login Page.</title>
<link href="css/green/screen.css" rel="stylesheet" type="text/css" media="all" />
<link href="css/black/datepicker.css" rel="stylesheet" type="text/css" media="all" />
<link href="js/visualize/visualize.css" rel="stylesheet" type="text/css" media="all" />
<link href="js/jwysiwyg/jquery.wysiwyg.css" rel="stylesheet" type="text/css" media="all" />
<link href="js/fancybox/jquery.fancybox-1.3.0.css" rel="stylesheet" type="text/css" media="all" />    
<link href="css/ie.css" rel="stylesheet" type="text/css" media="all" />
<script src="Scripts/jquery-1.4.2.js" type="text/javascript"></script>
<script type="text/javascript">
    $(document).ready(function(){
        textboxes = $("input:text");
        if ($.browser.mozilla) {
            $(textboxes).keypress(checkForEnter);
        } else {
            $(textboxes).keydown(checkForEnter);
        }
        function checkForEnter(event) {
            if (event.keyCode == 13) {
                var inputs = $(this).parents("form").eq(0).find(":input");
                if (inputs[inputs.index(this) + 1] != null) {
                    inputs[inputs.index(this) + 1].focus();
                }
                event.preventDefault();
            }
        }
    });
</script>  
   
</head>
<body class="login">
<form id="form12" runat="server">
<asp:ScriptManager ID="ScriptManager1" runat="server" AsyncPostBackTimeout="1000">
        </asp:ScriptManager>
	<!-- Begin login window -->
<p style="display:none;">
                    <asp:DropDownList SkinID="ddlPlain" ID="ddlBook" runat="server" Width="95%" Font-Size="8pt">
                     </asp:DropDownList>
</p>
<div id="login_wrapper" style="text-align:center;">		
		<br class="clear"/>
		<div id="login_top_window">
			<img src="images/green/top_login_window.png" alt="top window"/>
		</div>
		<asp:UpdatePanel ID="upLogin" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
		<!-- Begin content -->
		<div id="login_body_window">
			<div class="inner" >
				 <%--<img src="Logo/delvebd_logo.jpg" alt="logo" width="200px" height="70px"/>--%>
                 <img src="Logo/NetsoftLogo.jpg" alt="logo" width="200px" height="70px"/>
				<p>	<asp:TextBox ID="txtUserName" autocomplete="off" runat="server" Width="95%" placeholder="User Id" MaxLength="18" Font-Size="9" onkeydown="convertEnterToTabKey('<%= txtUserName.ClientID %>')"></asp:TextBox>
					</p>
					<p> 
						<asp:TextBox ID="txtPassword" runat="server" Width="95%" TextMode="Password"  Font-Size="9" placeholder="Password"
                            MaxLength="18"  onkeydown="convertEnterToTabKey('<%= txtPassword.ClientID %>')"></asp:TextBox>
					</p>
					<p>
						<asp:Button ID="LoginBtn" runat="server" ToolTip="Login" onclick="LoginBtn_Click" CssClass="Login" Text="Login"
             Width="80px" Height="25px" />
						<input type="checkbox" id="remember" name="remember" style="font-family:Verdana; font-size:8pt;"/>Remember my password
					</p>             
			</div>            
		</div>
		<!-- End content -->
        </ContentTemplate>
        </asp:UpdatePanel>         
		<div id="login_footer_window">
			<img src="images/blue/footer_login_window.png" alt="footer window"/>
		</div>
		<div id="login_reflect">
			<img src="images/blue/reflect.png" alt="window reflect"/>
		</div>
		<asp:UpdateProgress AssociatedUpdatePanelID="upLogin" ID="updateProgress" runat="server">
    <ProgressTemplate>
        <div id="progressBackgroundFilter"></div>
        <div id="processMessage" style="height:30px;padding-right:0px;"><img src="img/loading.gif" alt="" style="border:2px solid lightgray;" /></div>
    </ProgressTemplate>
    </asp:UpdateProgress>
</div>
</form>	
	<!-- End login window -->	
</body>
</html>
