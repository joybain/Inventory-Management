<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Employee.aspx.cs" Inherits="Employee" Title="Personnel Information.-DDP"  Theme="Themes" MaintainScrollPositionOnPostback="true"%>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxtoolkit" %>
<%@ Import Namespace="System.Data" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">

    <script src='<%= ResolveUrl("~/Scripts/valideDate.js") %>' type="text/javascript"></script>
<script type="text/javascript">
    function SetImage()
    {        
        document.getElementById('<% =lbImgUpload.ClientID %>').click();
    }
    function SetSignature()
    {        
        document.getElementById('<% =lbSigUpload.ClientID %>').click();
    }
    </script>
    <script language="javascript" type="text/javascript">
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

<div id="frmMainDiv"  style="width:98.5%; background-color:transparent;">
<table  id="pageFooterWrapper">

 <tr>
	<td align="center"><asp:Button  ID="BtnDelete" runat="server"  ToolTip="Delete Record"   
            OnClick="BtnDelete_Click"  
            
            
            onclientclick="javascript:return window.confirm('are u really want to delete  these data')" Text="Delete" 
        Height="35px" Width="50%" BorderStyle="Outset"  /></td>
	<td align="center">
	<asp:Button  ID="BtnFind" runat="server"  ToolTip="Find"  
            OnClick="BtnFind_Click"  Text="Find" 
        Height="35px" Width="50%" BorderStyle="Outset" Visible="False"  />
	</td>
	<td align="center">
        <asp:Button ID="BtnSave" runat="server" ToolTip="Save or Update Record" 
            OnClick="BtnSave_Click" Text="Save"  
        Height="35px" Width="50%" BorderStyle="Outset"  /></td>
	<td align="center">
        <asp:Button ID="BtnReset" runat="server" ToolTip="Clear Form" 
            OnClick="BtnReset_Click" Text="Clear" 
        Height="35px" Width="50%" BorderStyle="Outset"  /></td>
    <td align="center">
       <asp:Button ID="btnPrint" runat="server" ToolTip="Print" 
            onclick="btnPrint_Click" Text="Print" 
        Height="35px" Width="50%" BorderStyle="Outset"  />
   </td>        
	</tr>		
	</table>
<table style="width:100%;background-color:white;" >
<tr>
<td style="width:1%;"></td>
<td style="width:98%; " align="center">

 <div style="width:100%; margin-left:auto; margin-right:auto;">
     <%--<ajaxtoolkit:TabPanel ID="TabPanel10" runat="server" HeaderText="Membership">                    
<ContentTemplate>
<table style="width:100%; font-size:8pt;">
<tr><td style="font-size:x-small;" align="left">
    &nbsp;</td></tr>
</table>
    </ContentTemplate>
</ajaxtoolkit:TabPanel>--%><%--</ContentTemplate>
</asp:UpdatePanel>--%>

<table style="width:100%; font-size:8pt; ">
<tr>
<td style="width:80%;">
<%--<asp:UpdatePanel ID="updatepanelEmpNo" runat="server" UpdateMode="Conditional">
<ContentTemplate>--%>
<table style="width:100%; border:1px solid lightgray; padding-right:8px;">
<tr>
  <td style="width: 15%; height: 27px;" align="left"><asp:Label ID="lblEmpNo" 
          runat="server" Font-Size="8pt" Width="100px" style="font-weight: 700">Employee Id </asp:Label></td>
	<td style="width: 16%; height: 27px;" align="left">
        <asp:TextBox SkinID="tbGray" ID="txtEmpNo"  CssClass="txtVisibleAlign"
            runat="server" Width="100%" AutoPostBack="false" Font-Size="8pt" 
             Height="18px" ></asp:TextBox></td>
	<td style="width:3%;" align="center">
        <asp:Label ID="Label44" runat="server" ForeColor="#CC3300" Text="*"></asp:Label>
    </td>
	<td style="width: 15%; height: 27px;" align="left"><asp:Label ID="Label39" runat="server" Font-Size="8pt" Width="100px">Personal File No</asp:Label></td>
	<td style="width: 16%; height: 27px;" align="left">
        <asp:TextBox SkinID="tbGray" ID="txtPersoneelFileNo"  CssClass="tbc" Enabled="true" Text=""
            runat="server" Width="100%" Font-Size="8pt" MaxLength="20"></asp:TextBox> </td>
	<td style="width:3%;"></td>
	<td style="width: 15%; height: 27px;" align="left"><asp:Label ID="Label22" 
            runat="server" Font-Size="8pt">National ID/Any valid ID</asp:Label></td>
	<td style="width: 16%; height: 27px;" align="left">
        <asp:TextBox SkinID="tbGray" ID="txtNationalId"  CssClass="tbc" style="text-align: left;"
            runat="server" Width="100%" AutoPostBack="False"  Enabled="true"
            Font-Size="8pt"></asp:TextBox> </td> 
</tr>
<tr>
  <td style="width: 15%; height: 27px;" align="left">
      <asp:Label ID="Label1" 
          runat="server" Font-Size="8pt" style="font-weight: 700">Name :</asp:Label>
      <asp:Label ID="lblID" 
          runat="server" Font-Size="8pt" Visible="False"></asp:Label></td>
	<td style="height: 27px;" align="left" colspan="4">
        <asp:TextBox SkinID="tbGray" ID="txtFName"  CssClass="TXT" Enabled="true"
            runat="server" Width="100%" AutoPostBack="False" 
            Font-Size="8pt" MaxLength="60"></asp:TextBox>
    </td>
    <td style="width:3%;" align="center">
        <asp:Label ID="Label45" runat="server" ForeColor="#CC3300" Text="*"></asp:Label>
    </td>
    <td style="width: 15%; height: 27px;" align="left"><asp:Label ID="Label43" 
            runat="server" Font-Size="8pt" Width="100px">Nationality</asp:Label></td>
    <td style="width: 16%; height: 27px;" align="left">
        <asp:TextBox SkinID="tbGray" ID="txtNationality"  CssClass="tbc"
            runat="server" Width="100%" AutoPostBack="False"  Enabled="true"
            Font-Size="8pt" MaxLength="20"></asp:TextBox> </td>
</tr>
<tr>
  <td style="width: 15%; height: 27px;" align="left"><asp:Label ID="Label16" 
          runat="server" Font-Size="8pt" Width="100px" style="font-weight: 700">Sex</asp:Label></td>
	<td style="width: 16%; height: 27px;" align="left">
     <asp:DropDownList SkinID="ddlPlain" ID="ddlSex" runat="server" Font-Size="8pt" 
            Width="105%" Height="18px" >
     <asp:ListItem></asp:ListItem>       
     <asp:ListItem Text="Male" Value="M"></asp:ListItem> 
     <asp:ListItem Text="Female" Value="F"></asp:ListItem>
     </asp:DropDownList></td>
	<td style="width:3%;"></td>
	<td style="width: 15%; height: 27px;" align="left"><asp:Label ID="Label17" 
            runat="server" Font-Size="8pt" Width="100px" style="font-weight: 700">DOB</asp:Label></td>
	<td style="width: 16%; height: 27px;" align="left">
        <asp:TextBox SkinID="tbGray" ID="txtEmpBirthDt"  CssClass="tbc" PlaceHolder="dd/MM/yyyy"
            runat="server" Width="100%" AutoPostBack="False"  Enabled="true"
            Font-Size="8pt" MaxLength="11"></asp:TextBox>
    <ajaxtoolkit:calendarextender runat="server" ID="Calendarextender2" TargetControlID="txtEmpBirthDt" Format="dd/MM/yyyy"/>        
    </td>
	<td style="width:3%;"></td>
	<td style="width: 15%; height: 27px;" align="left"><asp:Label ID="Label18" runat="server" Font-Size="8pt" Width="100px">Birth Place</asp:Label></td>
	<td style="width: 16%; height: 27px;" align="left">
    <asp:TextBox SkinID="tbGray" ID="txtPlaceOfBirth"  CssClass="tbc" 
            runat="server" Width="100%" AutoPostBack="False"  Enabled="true"
            Font-Size="8pt" MaxLength="20"></asp:TextBox></td>        
</tr>
<tr>
  <td style="width: 15%; height: 27px;" align="left"><asp:Label ID="Label19" 
          runat="server" Font-Size="8pt" Width="100px" style="font-weight: 700">Religion</asp:Label></td>
	<td style="width: 16%; height: 27px;" align="left">
     <asp:DropDownList SkinID="ddlPlain" ID="ddlReligionCode" runat="server" 
            Font-Size="8pt" Width="105%" Height="18px" >
     <%--<asp:ListItem></asp:ListItem>       
     <asp:ListItem Text="Islam" Value="1"></asp:ListItem> 
     <asp:ListItem Text="Hindu" Value="2"></asp:ListItem>
     <asp:ListItem Text="Christian" Value="3"></asp:ListItem>
     <asp:ListItem Text="Buddhist" Value="4"></asp:ListItem>
     <asp:ListItem Text="Others" Value="5"></asp:ListItem>
     <asp:ListItem Text="Born again" Value="6"></asp:ListItem>
     <asp:ListItem Text="Catholic" Value="7"></asp:ListItem>
     <asp:ListItem Text="Ni Christo" Value="8"></asp:ListItem>
     <asp:ListItem Text="Roman Catholic" Value="9"></asp:ListItem>
     <asp:ListItem Text="Iglesia Ni Christo" Value="10"></asp:ListItem>--%>
     </asp:DropDownList></td>
	<td style="width:3%;"></td>
	<td style="width: 15%; height: 27px;" align="left"><asp:Label ID="Label20" runat="server" Font-Size="8pt" Width="100px">Blood Group</asp:Label></td>
	<td style="width: 16%; height: 27px;" align="left">
      <asp:DropDownList SkinID="ddlPlain" ID="ddlBloodGroup" runat="server" 
            Font-Size="8pt" Width="105%" Height="18px">
     <asp:ListItem></asp:ListItem>       
     <asp:ListItem Text="A+" Value="A+"></asp:ListItem> 
     <asp:ListItem Text="B+" Value="B+"></asp:ListItem>
     <asp:ListItem Text="B-" Value="B-"></asp:ListItem>
     <asp:ListItem Text="O+" Value="O+"></asp:ListItem>
     <asp:ListItem Text="O-" Value="O-"></asp:ListItem>
     <asp:ListItem Text="AB+" Value="AB+"></asp:ListItem>
     <asp:ListItem Text="AB-" Value="AB-"></asp:ListItem>
     </asp:DropDownList></td>
	<td style="width:3%;"></td>
	<td style="width: 15%; height: 27px;" align="left"><asp:Label ID="Label21" runat="server" Font-Size="8pt" Width="100px">Marital Status</asp:Label></td>
	<td style="width: 16%; height: 27px;" align="left">
    <asp:DropDownList SkinID="ddlPlain" ID="ddlMaritalStatusCode" runat="server" 
            Font-Size="8pt" Width="105%" Height="18px" >
     <asp:ListItem></asp:ListItem>       
     <asp:ListItem Text="Maried" Value="M"></asp:ListItem> 
     <asp:ListItem Text="Single" Value="S"></asp:ListItem>
     <asp:ListItem Text="Divorced" Value="D"></asp:ListItem>
     <asp:ListItem Text="Separated" Value="P"></asp:ListItem>
     </asp:DropDownList></td>        
</tr>
<tr>
  <td style="width: 15%; height: 27px;" align="left"><asp:Label ID="Label13" runat="server" Font-Size="8pt" Width="100px">Father Name</asp:Label></td>
	<td style="width: 50%; height: 27px;" align="left" colspan="4">
        <asp:TextBox SkinID="tbGray" ID="txtFhName"  CssClass="tbl" Enabled="true"
            runat="server" Width="100%" AutoPostBack="False" 
            Font-Size="8pt" MaxLength="40"></asp:TextBox></td>
    <td style="width:3%;"></td>
  <td style="width: 15%; height: 27px;" align="left"><asp:Label ID="Label9" runat="server" Font-Size="8pt" Width="100px">Employee Type :</asp:Label></td>
	<td style="width: 16%; height: 27px;" align="left">
	<asp:DropDownList SkinID="ddlPlain" ID="ddlEmpCat" runat="server" Font-Size="8pt" Width="105%" Height="18px" >
     <asp:ListItem></asp:ListItem>  
     <asp:ListItem Text="Full-time" Value="F"></asp:ListItem>     
     <asp:ListItem Text="Part-time" Value="PT"></asp:ListItem> 
     <asp:ListItem Text="Casual" Value="C"></asp:ListItem>     
     <asp:ListItem Text="Fixed term" Value="FT"></asp:ListItem> 
     <asp:ListItem Text="Shiftworkers" Value="S"></asp:ListItem>     
     <asp:ListItem Text="Daily hire and weekly hire" Value="D"></asp:ListItem> 
     <asp:ListItem Text="Probation" Value="P"></asp:ListItem>     
     <asp:ListItem Text="Outworkers" Value="O"></asp:ListItem> 
     </asp:DropDownList> </td>          
</tr>
<tr>	
	<td style="width: 15%; height: 27px;" align="left"><asp:Label ID="Label14" runat="server" Font-Size="8pt" Width="100px">Mother Name</asp:Label></td>
	<td style="width: 50%; height: 27px;" align="left" colspan="4">
    <asp:TextBox SkinID="tbGray" ID="txtMhName"  CssClass="tbl"
            runat="server" Width="100%" AutoPostBack="False"  Enabled="true"
            Font-Size="8pt" MaxLength="30"></asp:TextBox></td>
    <td style="width:3%;"></td>
    <td style="width: 15%; height: 27px;" align="left"><asp:Label ID="Label4" 
            runat="server" Font-Size="8pt" Width="100px" style="font-weight: 700">Date of Join</asp:Label></td>
	<td style="width: 16%; height: 27px;" align="left">
        <asp:TextBox SkinID="tbGray" ID="txtJoinDate"  CssClass="tbc" PlaceHolder="dd/MM/yyyy"
            runat="server" Width="100%" AutoPostBack="false" Font-Size="8pt" 
            MaxLength="11" TabIndex="16" ></asp:TextBox>
            <ajaxtoolkit:calendarextender runat="server" ID="Calendarextender5" TargetControlID="txtJoinDate" Format="dd/MM/yyyy"/>
            </td>        
</tr>	
<tr>
	<td style="width: 15%; height: 27px;" align="left"><asp:Label ID="Label15" runat="server" Font-Size="8pt" Width="100px">Spouse Name</asp:Label></td>
	<td style="width: 50%; height: 27px;" align="left" colspan="4">
        <asp:TextBox SkinID="tbGray" ID="txtSpouseName"  CssClass="tbl"
            runat="server" Width="100%" AutoPostBack="false" Font-Size="8pt" 
            MaxLength="40"></asp:TextBox></td>
    <td style="width:3%;"></td>
    <td style="width: 15%; height: 27px;" align="left"><asp:Label ID="Label7" runat="server" Font-Size="8pt" Width="100px">Confirm Date</asp:Label></td>
	<td style="width: 16%; height: 27px;" align="left">
        <asp:TextBox SkinID="tbGray" ID="txtConfirmDate"  CssClass="tbc" PlaceHolder="dd/MM/yyyy"
            runat="server" Width="100%" AutoPostBack="false" Font-Size="8pt" 
            MaxLength="11" ></asp:TextBox>
    <ajaxtoolkit:calendarextender runat="server" ID="Calendarextender1" TargetControlID="txtConfirmDate" Format="dd/MM/yyyy"/>
    </td>         
</tr>
<tr>     	
	<td style="width: 15%; height: 27px;" align="left"><asp:Label ID="Label5" 
            runat="server" Font-Size="8pt" Width="100px" style="font-weight: 700">Designation </asp:Label></td>
	<td style="width: 50%; height: 27px;" align="left" colspan="4">
        <asp:DropDownList SkinID="ddlPlain" ID="ddlJoinDesigCode" runat="server" 
            Font-Size="8pt" Width="102%" Height="18px">
     </asp:DropDownList></td>
     <td style="width:3%;" align="center">
         <asp:Label ID="Label46" runat="server" ForeColor="#CC3300" Text="*"></asp:Label>
    </td>
    <td style="width: 15%; height: 27px;" align="left"><asp:Label ID="Label6" runat="server" Font-Size="8pt" Width="100px">Job Status</asp:Label></td>
	<td style="width: 16%; height: 27px;" align="left">
    <asp:DropDownList SkinID="ddlPlain" ID="ddlJobStatus" runat="server" Font-Size="8pt" Width="105%" Height="18px">
     <asp:ListItem></asp:ListItem>       
     <asp:ListItem Text="In Service" Value="S"></asp:ListItem> 
     <asp:ListItem Text="Retired" Value="R"></asp:ListItem>
     <asp:ListItem Text="Resigned" Value="R1"></asp:ListItem>
     <asp:ListItem Text="Terminated" Value="T"></asp:ListItem>
     <asp:ListItem Text="Transferred" Value="T1"></asp:ListItem>
     <asp:ListItem Text="Lay-off" Value="L"></asp:ListItem>
     </asp:DropDownList></td>
</tr>
<tr>	
	<td style="width: 15%; height: 27px;" align="left"><asp:Label ID="Label35" 
            runat="server" Font-Size="8pt">Passport No/Any valid ID</asp:Label></td>
	<td style="width: 16%; height: 27px;" align="left">
    <asp:TextBox SkinID="tbGray" ID="txtPassNo"  CssClass="tbc"
            runat="server" Width="100%" AutoPostBack="False"  Enabled="true"
            Font-Size="8pt"></asp:TextBox></td>        
    <td style="width:3%;"></td>
  <td style="width: 15%; height: 27px;" align="left"><asp:Label ID="Label37" 
          runat="server" Font-Size="8pt">Driving Licnse No/Any valid ID</asp:Label></td>
	<td style="width: 16%; height: 27px;" align="left">
        <asp:TextBox SkinID="tbGray" ID="txtDrivLicNo"  CssClass="tbc"
            runat="server" Width="100%" AutoPostBack="false" Font-Size="8pt"></asp:TextBox></td>
	<td style="width:3%;"></td>
	<td style="width: 15%; height: 27px;" align="left">
        <asp:Label ID="lblSupAddr8" runat="server" Font-Size="9pt" 
            style="font-weight: 700">Country</asp:Label>
    </td>
	<td style="width: 16%; height: 27px;" align="left">    
        <asp:DropDownList ID="ddlCountry" runat="server" Height="26px" Width="105%">
        </asp:DropDownList>
            </td>        
</tr>
<tr>	
	<td style="width: 15%; height: 27px;" align="left">
        <asp:Label runat="server" Font-Size="8pt" Width="100px" ID="Label11">Present Branch</asp:Label>
    </td>
	<td style="height: 27px;" align="left" colspan="4">
        <asp:DropDownList runat="server" Font-Size="8pt" Height="18px" 
            SkinID="ddlPlain" Width="102%" ID="ddlBranchKey">
        </asp:DropDownList>
    </td>        
	<td style="width:3%;">&nbsp;</td>
	<td style="width: 15%; height: 27px;" align="left">
        Basic Salary</td>
	<td style="width: 16%; height: 27px;" align="left">    
    <asp:TextBox SkinID="tbGray" ID="txtBasicSalary"  CssClass="tbc"
            runat="server" Width="100%" AutoPostBack="False"  Enabled="true"
            Font-Size="8pt" MaxLength="20"></asp:TextBox>
            </td>        
</tr>
<tr>
<td colspan="6">
    <asp:Label ID="lblTranStatus" runat="server" Font-Size="8pt" Text="" Visible="false"></asp:Label>
<asp:TextBox SkinID="tbPlain" ID="txtGlCoa" style="text-align:left;" 
        runat="server" 
        Font-Size="9pt"  CssClass="tbc" Visible="False"></asp:TextBox>
      <asp:Label ID="Label42" 
          runat="server" Font-Size="8pt" Visible="False">Segment Code</asp:Label>
    <asp:Label ID="Label38" runat="server" Font-Size="8pt" Visible="False">PF No</asp:Label>    
    <asp:TextBox SkinID="tbGray" ID="txtPfNo"  CssClass="tbc"
            runat="server" AutoPostBack="False"  Enabled="true"
            Font-Size="8pt" MaxLength="15" Visible="False"></asp:TextBox>
            </td>
    <td>
        <asp:HiddenField ID="hfEmpNo" runat="server" />
    </td>
    <td>
        &nbsp;</td>
</tr>
<tr>
<td colspan="6">
    &nbsp;</td>
    <td>
        &nbsp;</td>
    <td>
        &nbsp;</td>
</tr>
</table>
<%--</ContentTemplate>
</asp:UpdatePanel>--%>
</td>
<td style="width:20%;" align="center">
<table style="width:100%; border:1px solid lightgray;">
<tr>
<td align="right">
<asp:FileUpload ID="imgUpload" runat="server" Visible="true" Size="15" Height="20px"  onchange="javascript:SetImage();" Font-Size="8pt"/>
<asp:Button ID="lbImgUpload" runat="server" Text="Upload" Font-Size="8pt"  style="display:none;"
        Width="50px" Height="20px" onclick="lbImgUpload_Click"></asp:Button>
</td>
</tr>
<tr>
<td style="width: 200px; height: 162px; vertical-align:top;" align="right">
    <asp:Image ID="imgEmp" runat="server" Height="165px" Width="145px" BorderStyle="Solid" BackColor="#EFF3FB" BorderWidth="1px"  />  
</td>	
</tr>
<tr>
<td style="width: 200px; height: 60px; vertical-align:top;" align="right">
    <asp:Image ID="imgSig" runat="server" Height="60px" Width="145px" BorderStyle="Solid" BackColor="#EFF3FB" BorderWidth="1px" />
</td>	
</tr>
<tr>
<td>
<table style="width:100%;">
<tr>
<td style="width:100%;" align="right">
<asp:FileUpload ID="sigUpload" runat="server" Visible="true" Size="15" Height="20px" onchange="javascript:SetSignature();" Font-Size="8pt"/>
<asp:Button ID="lbSigUpload" runat="server" Text="Upload" Font-Size="8pt" style="display:none;"
        Width="50px" Height="20px" onclick="lbSigUpload_Click"></asp:Button>
</td>
</tr>
</table>
</td>
</tr>
</table>
</td>
</tr>
</table>
</div>
<img alt="" height="1px" src="img/box_bottom_filet.gif" width="100%" />

<%--<asp:UpdatePanel ID="TabUpdatePanel" runat="server" UpdateMode="Conditional">
<ContentTemplate>--%>
<div style=" width:100%;">
<ajaxtoolkit:TabContainer ID="EmpTabContainer" runat="server" Width="100%" 
         TabIndex="148" Font-Size="8pt" ActiveTabIndex="0">
<ajaxtoolkit:TabPanel ID="tab" runat="server" HeaderText="Employee General Info">                    
<ContentTemplate>
<table style="width:100%; font-size:9pt; ">
<tr><td colspan="8" style="font-size:8pt; width:100%;" align="left">Present and Permanent Address :</td></tr>
<tr><td style="width: 30%; height: 30px;" align="right" colspan="3"><asp:Label ID="Label23" 
            runat="server" Font-Size="8pt">Permanent Address</asp:Label></td>
	<td style="width: 70%; height: 30px;" align="left" colspan="5">
        <asp:TextBox SkinID="tbPlain" ID="txtPerLoc" 
            runat="server" Width="100%" Font-Size="8pt" MaxLength="200"></asp:TextBox></td>
</tr>
<tr>
<td style="width: 20%; height: 30px;" align="right" colspan="2"></td>
	
	<td style="width: 10%; height: 30px;" align="right"><asp:Label ID="lblAdmisDate" 
            runat="server" Font-Size="8pt">District</asp:Label></td>
	<td style="width: 16%; height: 30px;" align="left">
     <asp:DropDownList SkinID="ddlPlain" ID="ddlPerDistCode" runat="server" 
            Font-Size="8pt" Width="104%" Height="18px" TabIndex="32" AutoPostBack="True" 
            onselectedindexchanged="ddlPerDistCode_SelectedIndexChanged">
        </asp:DropDownList></td>
	
	<td style="width: 10%; height: 30px;" align="right"><asp:Label ID="lblRefBy" 
            runat="server" Font-Size="8pt" Width="100px">Thana</asp:Label></td>
	<td style="width: 16%; height: 30px;" align="left">
    <asp:DropDownList SkinID="ddlPlain" ID="ddlPerThanaCode" runat="server" Font-Size="8pt" Width="104%" Height="18px">
        </asp:DropDownList></td>
    
	<td style="width: 10%; height: 30px;" align="right"><asp:Label ID="lbRefOther" 
            runat="server" Font-Size="8pt" Width="100px">Zip</asp:Label></td>
	<td style="width: 16%; height: 30px;" align="left">
    <asp:TextBox SkinID="tbPlain" ID="txtZipAreaCode"  CssClass="tbc"
            runat="server" Width="100%" Font-Size="8pt" MaxLength="10"></asp:TextBox></td>
</tr>
<tr><td style="width: 30%; height: 30px;" align="right" colspan="3"><asp:Label ID="Label24" 
            runat="server" Font-Size="8pt">Present Address</asp:Label></td>
	<td style="width: 70%; height: 30px;" align="left" colspan="5">
        <asp:TextBox SkinID="tbPlain" ID="txtMailLoc" 
            runat="server" Width="100%" Font-Size="8pt" MaxLength="200"></asp:TextBox></td>
</tr>
<tr>
<td style="width: 20%; height: 30px;" align="right" colspan="2"></td>
	
	<td style="width: 10%; height: 30px;" align="right"><asp:Label ID="Label25" 
            runat="server" Font-Size="8pt">District</asp:Label></td>
	<td style="width: 16%; height: 30px;" align="left">
     <asp:DropDownList SkinID="ddlPlain" ID="ddlMailDistCode" runat="server" 
            Font-Size="8pt" Width="104%" Height="18px" AutoPostBack="True" 
            onselectedindexchanged="ddlMailDistCode_SelectedIndexChanged">
        </asp:DropDownList></td>
	
	<td style="width: 10%; height: 30px;" align="right"><asp:Label ID="Label26" 
            runat="server" Font-Size="8pt" Width="100px">Thana</asp:Label></td>
	<td style="width: 16%; height: 30px;" align="left">
    <asp:DropDownList SkinID="ddlPlain" ID="ddlMailThanaCode" runat="server" Font-Size="8pt" Width="104%" Height="18px" >
        </asp:DropDownList></td>
    
	<td style="width: 10%; height: 30px; font-size:8pt;" align="right">Post Code</td>
	<td style="width: 16%; height: 30px;" align="left">
	<asp:TextBox SkinID="tbPlain" ID="txtMailPostCode"  CssClass="tbc"
            runat="server" Width="100%" Font-Size="8pt" MaxLength="4"></asp:TextBox>
	</td>
</tr>
<tr>
<td style="width: 20%; height: 30px;" align="left" colspan="2"></td>
	
	<td style="width: 10%; height: 30px;" align="right"><asp:Label ID="Label27" 
            runat="server" Font-Size="8pt">Resident Phone</asp:Label></td>
	<td style="width: 16%; height: 30px;" align="left">
     <asp:TextBox SkinID="tbGray" ID="txtResPhNo" 
            runat="server" Width="100%" Font-Size="8pt" MaxLength="20"></asp:TextBox></td>
	
	<td style="width: 10%; height: 30px;" align="right"><asp:Label ID="Label28" 
            runat="server" Font-Size="8pt">Mobile</asp:Label></td>
	<td style="width: 16%; height: 30px;" align="left">
    <asp:TextBox SkinID="tbGray" ID="txtMobile" 
            runat="server" Width="100%" Font-Size="8pt" MaxLength="24"></asp:TextBox></td>
    
	<td style="width: 10%; height: 30px;" align="right"><asp:Label ID="Label29" 
            runat="server" Font-Size="8pt" >Email</asp:Label></td>
	<td style="width: 16%; height: 30px;" align="left"><asp:TextBox SkinID="tbGray" ID="txtEMail" 
            runat="server" Width="100%" Font-Size="8pt"></asp:TextBox></td>
</tr>
<tr><td colspan="8" style="font-size:x-small;" align="left">Bank, Salary and Designation Info :</td></tr>
<tr>
<td style="width: 10%; height: 30px;" align="right"><asp:Label ID="Label33" 
            runat="server" Font-Size="8pt">Bank Name</asp:Label></td>
	<td style="width: 36%; height: 30px;" align="left" colspan="3">
	<asp:DropDownList SkinID="ddlPlain" ID="ddlBankNo" runat="server" Font-Size="8pt" Width="102%" Height="18px"> </asp:DropDownList></td>
	
	<td style="width: 10%; height: 30px;" align="right"><asp:Label ID="Label30" 
            runat="server" Font-Size="8pt">Bank Acc. No</asp:Label></td>
	<td style="width: 16%; height: 30px;" align="left">
     <asp:TextBox SkinID="tbGray" ID="txtBankAccNo" 
            runat="server" Width="100%" Font-Size="8pt" MaxLength="20"></asp:TextBox></td>
	
	<td style="width: 10%; height: 30px;" align="right"><asp:Label ID="Label31" 
            runat="server" Font-Size="8pt">Emp Insur Dt</asp:Label></td>
	<td style="width: 16%; height: 30px;" align="left">
    <asp:TextBox SkinID="tbGray" ID="txtEmpInsureDt" 
            runat="server" Width="100%" Font-Size="8pt" MaxLength="11"></asp:TextBox>
    <ajaxtoolkit:calendarextender runat="server" ID="Calendarextender4" 
            TargetControlID="txtEmpInsureDt" Format="dd/MM/yyyy" Enabled="True"/>
    </td>

</tr>
<tr>    
	<td style="width: 10%; height: 30px;" align="right"><asp:Label ID="Label32" 
            runat="server" Font-Size="8pt">Spouse Insur Dt</asp:Label></td>
	<td style="width: 16%; height: 30px;" align="left"><asp:TextBox SkinID="tbGray" ID="txtSpouseInsDt" 
            runat="server" Width="100%" Font-Size="8pt" MaxLength="11"></asp:TextBox>
    <ajaxtoolkit:calendarextender runat="server" ID="Calendarextender3" 
            TargetControlID="txtSpouseInsDt" Format="dd/MM/yyyy" Enabled="True"/>        
    </td>

    <td style="width: 10%; height: 30px;" align="right"><asp:Label ID="Label34" 
            runat="server" Font-Size="8pt">Seniority Sl No</asp:Label></td>
	<td style="width: 16%; height: 30px;" align="left">
	<asp:TextBox SkinID="tbGray" ID="txtSenrSlNo" 
            runat="server" Width="100%" Font-Size="8pt" MaxLength="11"></asp:TextBox></td>
	
	<td style="width: 10%; height: 30px;" align="right"><asp:Label ID="Label36" 
            runat="server" Font-Size="8pt">Present Desig</asp:Label></td>
	<td style="width: 36%; height: 30px;" align="left" colspan="3">
    <asp:DropDownList SkinID="ddlPlain" ID="ddlPrstDesigCode" runat="server" Font-Size="8pt" Width="102%" Height="18px" >
        </asp:DropDownList></td>
 </tr>
<tr>
<td style="width: 10%; height: 30px;" align="right">&nbsp;</td>
	<td style="width: 36%; height: 30px;" align="left" colspan="3">
	    &nbsp;</td>  
   <td style="width: 10%; height: 30px;" align="right"><asp:Label ID="Label41" runat="server" Font-Size="8pt" Width="100px">TIN No</asp:Label></td>
	<td style="width: 16%; height: 30px;" align="left"><asp:TextBox SkinID="tbGray" 
            ID="txtTinNo"  CssClass="tbc" runat="server" Width="90px" Font-Size="8pt" MaxLength="15"></asp:TextBox></td>
            
    <td style="width: 10%; height: 30px;" align="right"><asp:Label ID="Label40" runat="server" Font-Size="8pt" Width="100px">LPR Date</asp:Label></td>
	<td style="width: 16%; height: 30px;" align="left">
        <asp:TextBox SkinID="tbGray" 
            ID="txtLprDate"  CssClass="tbc"  runat="server" Width="100%" PlaceHolder="dd/MM/yyyy"
            Font-Size="8pt" MaxLength="11"></asp:TextBox>
            <ajaxtoolkit:calendarextender runat="server" ID="Calendarextender6" 
            TargetControlID="txtLprDate" Format="dd/MM/yyyy" Enabled="True"/>
    </td>
</tr>
</table>
    </ContentTemplate>
</ajaxtoolkit:TabPanel>
<ajaxtoolkit:TabPanel ID="TabPanel1" runat="server" HeaderText="Education">                    
<ContentTemplate>
<table style="width:100%; font-size:8pt;">
<tr>
    <td style="font-size:x-small;" align="left">
        <asp:UpdatePanel ID="UP1" runat="server" UpdateMode="Conditional"><ContentTemplate>
<asp:GridView CssClass="mGrid" ID="dgEmpEdu" runat="server" AutoGenerateColumns="False" 
        AllowPaging="True" Width="100%" BackColor="White" BorderWidth="1px" BorderStyle="Solid"
        CellPadding="2" BorderColor="LightGray" Font-Size="8pt" 
        AllowSorting="True" PageSize="20" onrowcancelingedit="dgEmpEdu_RowCancelingEdit" 
        onrowdeleting="dgEmpEdu_RowDeleting" onrowediting="dgEmpEdu_RowEditing" 
        onrowupdating="dgEmpEdu_RowUpdating" 
        onrowdatabound="dgEmpEdu_RowDataBound" onrowcommand="dgEmpEdu_RowCommand">
  <HeaderStyle Font-Size="8pt" Font-Names="Arial" Font-Bold="True" BackColor="LightGray"
        HorizontalAlign="Center" ForeColor="Black"/>

  <Columns>
  <asp:TemplateField>
  <ItemTemplate> 
  <asp:LinkButton ID="lbEdit" runat="server" CausesValidation="False" CommandName="Edit" Text="Edit" ></asp:LinkButton>  
  <asp:LinkButton ID="lbDelete" runat="server" CausesValidation="False" CommandName="Delete" Text="Remove"
  onclientclick="javascript:return window.confirm('are u really want to delete  these data')"></asp:LinkButton>  
  <asp:LinkButton ID="btnAddDet" runat="server" CommandName="AddNew" Text="New"></asp:LinkButton>  
      </ItemTemplate>
  <EditItemTemplate>  
  <asp:LinkButton ID="lbUpdate" runat="server" CausesValidation="True" CommandName="Update" Text="Update" ></asp:LinkButton> 
  <asp:LinkButton ID="lbCancel" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel"></asp:LinkButton>  
      </EditItemTemplate>
  <FooterTemplate>  
  <asp:LinkButton ID="lbInsert" runat="server" CommandName="Insert" Text="Add" ></asp:LinkButton>   
  <asp:LinkButton ID="lbCancel" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel"></asp:LinkButton>    
      </FooterTemplate>
      <ItemStyle Font-Size="8pt" Width="130px" />
  </asp:TemplateField>
  
  <asp:TemplateField HeaderText="Exam">
      <ItemTemplate>
          <asp:Label ID="lblExamCode" runat="server" style="display:none" Text='<%#Eval("exam_code") %>' Width="80px" ></asp:Label> 
          <asp:Label ID="lblExamName" runat="server" Text='<%#Eval("exam_name") %>' Width="80px"></asp:Label>   
      </ItemTemplate>
      <EditItemTemplate>
      <asp:DropDownList SkinID="ddlPlain" ID="ddlExamCode" runat="server" Font-Size="8pt" Width="80px" TabIndex="61">
          </asp:DropDownList>
          </EditItemTemplate>
      <FooterTemplate>
      <asp:DropDownList SkinID="ddlPlain" ID="ddlExamCode" runat="server" Font-Size="8pt" Width="80px" TabIndex ="61">
      </asp:DropDownList>
      </FooterTemplate>
      <ItemStyle HorizontalAlign="Center" Width="80px" Height="25px" />
  </asp:TemplateField>
  
  <asp:TemplateField HeaderText="Group">
  <ItemTemplate>
  <asp:Label ID="lblGroupName" runat="server" Text='<%#Eval("group_name") %>' Width="80px"></asp:Label>  
      </ItemTemplate>
  <EditItemTemplate>
  <asp:TextBox SkinID="tbGray" ID="txtGroupName" PlaceHolder="Search Group.."
            runat="server" Width="80px" TabIndex="62" Font-Size="8pt" MaxLength="30"></asp:TextBox>
             <ajaxtoolkit:AutoCompleteExtender ID="AutoCompleteExtender2"
                                                        runat="server" CompletionInterval="20" CompletionSetCount="30"
                                                        EnableCaching="true" MinimumPrefixLength="1"
                                                        ServiceMethod="GetPMIS_EXAM_GROUP" ServicePath="~/AutoComplete.asmx"
                                                        TargetControlID="txtGroupName">
                                                    </ajaxtoolkit:AutoCompleteExtender>
      </EditItemTemplate>
  <FooterTemplate>
  <asp:TextBox SkinID="tbGray" ID="txtGroupName" PlaceHolder="Search Group.."
            runat="server" Width="80px" TabIndex="62" Font-Size="8pt" MaxLength="30"></asp:TextBox>
            <ajaxtoolkit:AutoCompleteExtender ID="AutoCompleteExtender2"
                                                        runat="server" CompletionInterval="20" CompletionSetCount="30"
                                                        EnableCaching="true" MinimumPrefixLength="1"
                                                        ServiceMethod="GetPMIS_EXAM_GROUP" ServicePath="~/AutoComplete.asmx"
                                                        TargetControlID="txtGroupName">
                                                    </ajaxtoolkit:AutoCompleteExtender>
      </FooterTemplate>
      <ItemStyle HorizontalAlign="Center" Width="80px" />
  </asp:TemplateField>
  
  <asp:TemplateField HeaderText="Institute">
  <ItemTemplate>
  <asp:Label ID="lblInstitute" runat="server" Text='<%#Eval("institute") %>'></asp:Label>  
      </ItemTemplate>
  <EditItemTemplate>
  <asp:TextBox SkinID="tbGray" ID="txtInstitute" 
            runat="server" Width="150px" TabIndex="63" Font-Size="8pt" ></asp:TextBox>
      </EditItemTemplate>
  <FooterTemplate>
  <asp:TextBox SkinID="tbGray" ID="txtInstitute" 
            runat="server" Width="150px" TabIndex="63" Font-Size="8pt" ></asp:TextBox>
      </FooterTemplate>
      <ItemStyle HorizontalAlign="Left" Width="150px" />
  </asp:TemplateField>
  
  <asp:TemplateField HeaderText="Passing Year">
  <ItemTemplate>
  <asp:Label ID="lblPassYear" runat="server" Text='<%#Eval("pass_year") %>' Width="60px"></asp:Label>  
      </ItemTemplate>
  <EditItemTemplate>
  <asp:TextBox SkinID="tbGray" ID="txtPassYear" 
            runat="server" Width="60px" TabIndex="64" Font-Size="8pt" ></asp:TextBox>
      </EditItemTemplate>
  <FooterTemplate>
  <asp:TextBox SkinID="tbGray" ID="txtPassYear" 
            runat="server" Width="60px" TabIndex="64" Font-Size="8pt" ></asp:TextBox>
      </FooterTemplate>
      <ItemStyle HorizontalAlign="Center" Width="60px" />
  </asp:TemplateField>
  
  <asp:TemplateField HeaderText="Major Subject">
  <ItemTemplate>
  <asp:Label ID="lblMainSub" runat="server" Text='<%#Eval("main_sub") %>' Font-Size="8pt"></asp:Label>  
      </ItemTemplate>
  <EditItemTemplate>
  <asp:TextBox SkinID="tbGray" ID="txtMainSub" 
            runat="server" Width="150px" TabIndex="65" Font-Size="8pt" ></asp:TextBox>
      </EditItemTemplate>
  <FooterTemplate>
  <asp:TextBox SkinID="tbGray" ID="txtMainSub" 
            runat="server" Width="150px" TabIndex="65" Font-Size="8pt" ></asp:TextBox>
      </FooterTemplate>
      <ItemStyle HorizontalAlign="Center" Width="110px" />
  </asp:TemplateField>
  
  <asp:TemplateField HeaderText="Board">
      <ItemTemplate>
          <asp:Label ID="lblDivClass" runat="server" style="display:none" Text='<%#Eval("div_class") %>' Width="90px"></asp:Label>  
          <asp:Label ID="lblDivName" runat="server" Text='<%#Eval("division_name") %>' Width="90px"></asp:Label>
      </ItemTemplate>
      <EditItemTemplate>
          <asp:DropDownList SkinID="ddlPlain" ID="ddlDivClass" runat="server" Font-Size="8pt" Width="90px" Height="18px" TabIndex="66" >
   
          </asp:DropDownList>
      </EditItemTemplate>
      <FooterTemplate>
          <asp:DropDownList SkinID="ddlPlain" ID="ddlDivClass" runat="server" Font-Size="8pt" Width="90px" Height="18px" TabIndex="66" >
   
          </asp:DropDownList>
      </FooterTemplate>
      <ItemStyle HorizontalAlign="Center" Width="90px" />
  </asp:TemplateField>
  
    </Columns>
                        <AlternatingRowStyle CssClass="alt" />
    <PagerStyle CssClass="pgr" />
    </asp:GridView>  
    </ContentTemplate></asp:UpdatePanel>
</td>
</tr>
</table>
    </ContentTemplate>
</ajaxtoolkit:TabPanel>

<ajaxtoolkit:TabPanel ID="TabPanel2" runat="server" HeaderText="Family">                    
<ContentTemplate>
<table style="width:100%; font-size:8pt;">
<tr><td style="font-size:x-small;" align="left">
         <asp:UpdatePanel ID="UP2" runat="server" UpdateMode="Conditional"><ContentTemplate>
<asp:GridView CssClass="mGrid" ID="dgEmpFam" runat="server" AutoGenerateColumns="False" 
        AllowPaging="True" Width="100%" BackColor="White" BorderWidth="1px" BorderStyle="Solid"
        CellPadding="2" BorderColor="LightGray" Font-Size="8pt" 
        AllowSorting="True" PageSize="20" onrowcancelingedit="dgEmpFam_RowCancelingEdit" 
        onrowdeleting="dgEmpFam_RowDeleting" onrowediting="dgEmpFam_RowEditing" 
        onrowupdating="dgEmpFam_RowUpdating" 
        onrowdatabound="dgEmpFam_RowDataBound" onrowcommand="dgEmpFam_RowCommand">
  <HeaderStyle Font-Size="8pt" Font-Names="Arial" Font-Bold="True" BackColor="LightGray" 
        HorizontalAlign="Center" ForeColor="Black"/>

  <Columns>
  <asp:TemplateField>
  <ItemTemplate> 
  <asp:LinkButton ID="lbEdit" runat="server" CausesValidation="False" CommandName="Edit" Text="Edit" ></asp:LinkButton>  
  <asp:LinkButton ID="lbDelete" runat="server" CausesValidation="False" CommandName="Delete" Text="Remove"
  onclientclick="javascript:return window.confirm('are u really want to delete  these data')"></asp:LinkButton>  
  <asp:LinkButton ID="btnAddDet" runat="server" CommandName="New" Text="New"></asp:LinkButton>  
      </ItemTemplate>
  <EditItemTemplate>  
  <asp:LinkButton ID="lbUpdate" runat="server" CausesValidation="True" CommandName="Update" Text="Update" ></asp:LinkButton> 
  <asp:LinkButton ID="lbCancel" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel"></asp:LinkButton>  
      </EditItemTemplate>
  <FooterTemplate>  
  <asp:LinkButton ID="lbInsert" runat="server" CommandName="Insert" Text="Add" ></asp:LinkButton>   
  <asp:LinkButton ID="lbCancel" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel"></asp:LinkButton>    
      </FooterTemplate>
      <ItemStyle Font-Size="8pt" HorizontalAlign="Center" Width="130px" />
  </asp:TemplateField>
  
  <asp:TemplateField HeaderText="Relative Name">
  <ItemTemplate>
  <asp:Label ID="lblRelName" runat="server" Text='<%#Eval("rel_name") %>' Width="200px"  Font-Size="8pt"></asp:Label>  
      </ItemTemplate>
  <EditItemTemplate>
  <asp:TextBox SkinID="tbGray" ID="txtRelName" 
            runat="server" Width="200px" TabIndex="71" Font-Size="8pt" MaxLength="25"></asp:TextBox>
      </EditItemTemplate>
  <FooterTemplate>
  <asp:TextBox SkinID="tbGray" ID="txtRelName" 
            runat="server" Width="200px" TabIndex="71" Font-Size="8pt" MaxLength="25"></asp:TextBox>
      </FooterTemplate>
      <ItemStyle HorizontalAlign="Left" Width="200px" Height="25px" />
  </asp:TemplateField>
  
  <asp:TemplateField HeaderText="Relation">
  <ItemTemplate>
  <asp:Label ID="lblRelation" runat="server" Text='<%#Eval("relation") %>' Width="100px"></asp:Label>  
      </ItemTemplate>
  <EditItemTemplate>
  <asp:DropDownList SkinID="ddlPlain" ID="ddlRelation" runat="server" Font-Size="8pt" Width="100px" Height="18px" TabIndex="72" >
     <asp:ListItem></asp:ListItem>       
     <asp:ListItem Text="Son" Value="1"></asp:ListItem> 
     <asp:ListItem Text="Daughter" Value="2"></asp:ListItem>
     <asp:ListItem Text="Brother" Value="3"></asp:ListItem>
     <asp:ListItem Text="Sister" Value="4"></asp:ListItem>
     <asp:ListItem Text="Spouse" Value="5"></asp:ListItem>
     <asp:ListItem Text="Father" Value="6"></asp:ListItem>
     <asp:ListItem Text="Mother" Value="7"></asp:ListItem>
     <asp:ListItem Text="Uncle" Value="8"></asp:ListItem>
     <asp:ListItem Text="Aunt" Value="9"></asp:ListItem>
      <asp:ListItem Text="Other" Value="10"></asp:ListItem>
      </asp:DropDownList>
      </EditItemTemplate>
  <FooterTemplate>
  <asp:DropDownList SkinID="ddlPlain" ID="ddlRelation" runat="server" Font-Size="8pt" Width="100px" Height="18px" TabIndex="72" >
     <asp:ListItem></asp:ListItem>       
     <asp:ListItem Text="Son" Value="1"></asp:ListItem> 
     <asp:ListItem Text="Daughter" Value="2"></asp:ListItem>
     <asp:ListItem Text="Brother" Value="3"></asp:ListItem>
     <asp:ListItem Text="Sister" Value="4"></asp:ListItem>
     <asp:ListItem Text="Spouse" Value="5"></asp:ListItem>
     <asp:ListItem Text="Father" Value="6"></asp:ListItem>
     <asp:ListItem Text="Mother" Value="7"></asp:ListItem>
     <asp:ListItem Text="Uncle" Value="8"></asp:ListItem>
     <asp:ListItem Text="Aunt" Value="9"></asp:ListItem>
      <asp:ListItem Text="Other" Value="10"></asp:ListItem>
      </asp:DropDownList>
      </FooterTemplate>
      <ItemStyle HorizontalAlign="Center" Width="100px" />
  </asp:TemplateField>
  
  <asp:TemplateField HeaderText="Birth Date">
  <ItemTemplate>
  <asp:Label ID="lblBirthDt" runat="server" Text='<%#Eval("birth_dt") %>'  Font-Size="8pt"></asp:Label>  
      </ItemTemplate>
  <EditItemTemplate>
  <asp:TextBox SkinID="tbGray" ID="txtBirthDt" PlaceHolder="dd/MM/yyyy"
            runat="server" Width="100px" TabIndex="73" Font-Size="8pt" MaxLength="11"></asp:TextBox>
<ajaxtoolkit:calendarextender runat="server" ID="CalendarextenderBirthDate1" TargetControlID="txtBirthDt" Format="dd/MM/yyyy"/>
      </EditItemTemplate>
  <FooterTemplate>
  <asp:TextBox SkinID="tbGray" ID="txtBirthDt" PlaceHolder="dd/MM/yyyy"
            runat="server" Width="100px" TabIndex="73" Font-Size="8pt" MaxLength="11"></asp:TextBox>
            <ajaxtoolkit:calendarextender runat="server" ID="Calendarextender1" TargetControlID="txtBirthDt" Format="dd/MM/yyyy"/>
      </FooterTemplate>
      <ItemStyle HorizontalAlign="Center" Width="100px" />
  </asp:TemplateField>
  
  <asp:TemplateField HeaderText="Mob.Number">
  <ItemTemplate>
  <asp:Label ID="lblAge" runat="server" Text='<%#Eval("age") %>' Width="80px"></asp:Label>  
      </ItemTemplate>
  <EditItemTemplate>
  <asp:TextBox SkinID="tbGray" ID="txtAge" 
            runat="server" Width="95%" TabIndex="74" Font-Size="8pt" ></asp:TextBox>
      </EditItemTemplate>
  <FooterTemplate>
  <asp:TextBox SkinID="tbGray" ID="txtAge" 
            runat="server" Width="95%" TabIndex="74" Font-Size="8pt" ></asp:TextBox>
      </FooterTemplate>
      <ItemStyle HorizontalAlign="Center" Width="80px" />
  </asp:TemplateField>
  
  <asp:TemplateField HeaderText="Occupation">
  <ItemTemplate>
  <asp:Label ID="lblOccupation" runat="server" Text='<%#Eval("occupation") %>'></asp:Label>  
      </ItemTemplate>
  <EditItemTemplate>
  <asp:TextBox SkinID="tbGray" ID="txtOccupation" 
            runat="server" Width="150px" TabIndex="75" Font-Size="8pt" ></asp:TextBox>
      </EditItemTemplate>
  <FooterTemplate>
  <asp:TextBox SkinID="tbGray" ID="txtOccupation" 
            runat="server" Width="150px" TabIndex="75" Font-Size="8pt" ></asp:TextBox>
      </FooterTemplate>
      <ItemStyle HorizontalAlign="Center" Width="150px" />
  </asp:TemplateField> 
  
    </Columns>
                        <AlternatingRowStyle CssClass="alt" />
    <PagerStyle CssClass="pgr" />
    </asp:GridView>  
     </ContentTemplate></asp:UpdatePanel>
</td></tr>
</table>
    </ContentTemplate>
</ajaxtoolkit:TabPanel>

<ajaxtoolkit:TabPanel ID="TabPanel3" runat="server" HeaderText="Experience">                    
<ContentTemplate>
<table style="width:100%; font-size:8pt;">
<tr><td style="font-size:x-small;" align="left">
Past Public Sector Experience (Outside) :
</td></tr>
<tr><td style="font-size:x-small;" align="left">
        <asp:UpdatePanel ID="UP3" runat="server" UpdateMode="Conditional"><ContentTemplate>
<asp:GridView CssClass="mGrid" ID="dgEmpExp" runat="server" AutoGenerateColumns="False" 
        AllowPaging="True" Width="100%" BackColor="White" BorderWidth="1px" BorderStyle="Solid"
        CellPadding="2" BorderColor="LightGray" Font-Size="8pt" 
        AllowSorting="True" PageSize="40" onrowcancelingedit="dgEmpExp_RowCancelingEdit" 
        onrowdeleting="dgEmpExp_RowDeleting" onrowediting="dgEmpExp_RowEditing" 
        onrowupdating="dgEmpExp_RowUpdating" 
        onrowdatabound="dgEmpExp_RowDataBound" onrowcommand="dgEmpExp_RowCommand">
  <HeaderStyle Font-Size="8pt" Font-Names="Arial" Font-Bold="True" BackColor="LightGray" 
        HorizontalAlign="Center" ForeColor="Black"/>

  <Columns>
  <asp:TemplateField>
  <ItemTemplate> 
  <asp:LinkButton ID="lbEdit" runat="server" CausesValidation="False" CommandName="Edit" Text="Edit" ></asp:LinkButton>  
  <asp:LinkButton ID="lbDelete" runat="server" CausesValidation="False" CommandName="Delete" Text="Remove"
  onclientclick="javascript:return window.confirm('are u really want to delete  these data')"></asp:LinkButton>  
  <asp:LinkButton ID="btnAddDet" runat="server" CommandName="AddNew" Text="New"></asp:LinkButton>  
      </ItemTemplate>
  <EditItemTemplate>  
  <asp:LinkButton ID="lbUpdate" runat="server" CausesValidation="True" CommandName="Update" Text="Update" ></asp:LinkButton> 
  <asp:LinkButton ID="lbCancel" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel"></asp:LinkButton>  
      </EditItemTemplate>
  <FooterTemplate>  
  <asp:LinkButton ID="lbInsert" runat="server" CommandName="Insert" Text="Add" ></asp:LinkButton>   
  <asp:LinkButton ID="lbCancel" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel"></asp:LinkButton>    
      </FooterTemplate>
      <ItemStyle Font-Size="8pt" HorizontalAlign="Center" Width="130px" />
  </asp:TemplateField>
  
  <asp:TemplateField HeaderText="Organization Name">
  <ItemTemplate>
  <asp:Label ID="lblOrgaName" runat="server" Text='<%#Eval("orga_name") %>' Width="220px" Font-Size="8pt"></asp:Label>  
      </ItemTemplate>
  <EditItemTemplate>
  <asp:TextBox SkinID="tbGray" ID="txtOrgaName" 
            runat="server" Width="220px" TabIndex="81" Font-Size="8pt" MaxLength="35"></asp:TextBox>
      </EditItemTemplate>
  <FooterTemplate>
  <asp:TextBox SkinID="tbGray" ID="txtOrgaName" 
            runat="server" Width="220px" TabIndex="81" Font-Size="8pt" MaxLength="35"></asp:TextBox>
      </FooterTemplate>
      <ItemStyle HorizontalAlign="Left" Width="220px" Height="25px" />
  </asp:TemplateField>
  
  <asp:TemplateField HeaderText="Position">
  <ItemTemplate>
  <asp:Label ID="lblPositionHeld" runat="server" Text='<%#Eval("position_held") %>' Width="150px" Font-Size="8pt"></asp:Label>  
      </ItemTemplate>
  <EditItemTemplate>
  <asp:TextBox SkinID="tbGray" ID="txtPositionHeld" 
            runat="server" Width="150px" TabIndex="82" Font-Size="8pt" MaxLength="35"></asp:TextBox>
      </EditItemTemplate>
  <FooterTemplate>
  <asp:TextBox SkinID="tbGray" ID="txtPositionHeld" 
            runat="server" Width="150px" TabIndex="82" Font-Size="8pt" MaxLength="35"></asp:TextBox>
      </FooterTemplate>
      <ItemStyle HorizontalAlign="Center" Width="150px" />
  </asp:TemplateField>
  
  <asp:TemplateField HeaderText="From Date">
  <ItemTemplate>
  <asp:Label ID="lblFromDt" runat="server" Text='<%# Eval("from_dt") %>' Width="70px"></asp:Label>  
      </ItemTemplate>
  <EditItemTemplate>
  <asp:TextBox SkinID="tbGray" ID="txtFromDt" PlaceHolder="dd/MM/yyyy"
            runat="server" Width="98%" TabIndex="83" Font-Size="8pt" MaxLength="11"></asp:TextBox>
            <ajaxtoolkit:calendarextender runat="server" ID="Calendarextender11123" TargetControlID="txtFromDt" Format="dd/MM/yyyy"/>
      </EditItemTemplate>
  <FooterTemplate>
  <asp:TextBox SkinID="tbGray" ID="txtFromDt" PlaceHolder="dd/MM/yyyy"
            runat="server" Width="98%" TabIndex="83" Font-Size="8pt" MaxLength="11"></asp:TextBox>
            <ajaxtoolkit:calendarextender runat="server" ID="Calendarextender122" TargetControlID="txtFromDt" Format="dd/MM/yyyy"/>
      </FooterTemplate>
      <ItemStyle HorizontalAlign="Center" Width="70px" />
  </asp:TemplateField>
  
  <asp:TemplateField HeaderText="To Date">
  <ItemTemplate>
  <asp:Label ID="lblToDt" runat="server" Text='<%# Eval("to_dt") %>' Width="90px"></asp:Label>  
      </ItemTemplate>
  <EditItemTemplate>
  <asp:TextBox SkinID="tbGray" ID="txtToDt" PlaceHolder="dd/MM/yyyy"
            runat="server" Width="98%" TabIndex="84" Font-Size="8pt" MaxLength="11"></asp:TextBox>
            <ajaxtoolkit:calendarextender runat="server" ID="Calendarextender111" TargetControlID="txtToDt" Format="dd/MM/yyyy"/>
      </EditItemTemplate>
  <FooterTemplate>
  <asp:TextBox SkinID="tbGray" ID="txtToDt" PlaceHolder="dd/MM/yyyy"
            runat="server" Width="98%" TabIndex="84" Font-Size="8pt" MaxLength="11"></asp:TextBox>
            <ajaxtoolkit:calendarextender runat="server" ID="Calendarextender11" TargetControlID="txtToDt" Format="dd/MM/yyyy"/>
      </FooterTemplate>
  <ItemStyle HorizontalAlign="Center" Width="70px" />
   </asp:TemplateField>
  
  <asp:TemplateField HeaderText="Pay Scale">
  <ItemTemplate>
  <asp:Label ID="lblPayScale" runat="server" Text='<%#Eval("pay_scale") %>' Font-Size="8pt"></asp:Label>  
      </ItemTemplate>
  <EditItemTemplate>
  <asp:TextBox SkinID="tbGray" ID="txtPayScale" 
            runat="server" Width="150px" TabIndex="85" Font-Size="8pt" MaxLength="40"></asp:TextBox>
      </EditItemTemplate>
  <FooterTemplate>
  <asp:TextBox SkinID="tbGray" ID="txtPayScale" 
            runat="server" Width="150px" TabIndex="85" Font-Size="8pt" MaxLength="40"></asp:TextBox>
      </FooterTemplate>
      <ItemStyle HorizontalAlign="Center" Width="150px" />
  </asp:TemplateField> 
  
    </Columns>
                        <AlternatingRowStyle CssClass="alt" />
    <PagerStyle CssClass="pgr" />
    </asp:GridView>  
    </ContentTemplate></asp:UpdatePanel>
</td></tr>
</table>
    </ContentTemplate>
</ajaxtoolkit:TabPanel>

<%--<ajaxtoolkit:TabPanel ID="TabPanel4" runat="server" HeaderText="Training">                    
<ContentTemplate>
<table style="width:100%; font-size:8pt;">
<tr><td style="font-size:8pt;" align="left">
Training Information [Local & Foreign Training Including Study Tour, Seminar and Workshops] :
</td></tr>
<tr><td style="font-size:8pt;" align="left">
    &nbsp;</td></tr>
</table>
    </ContentTemplate>
</ajaxtoolkit:TabPanel>--%>

<ajaxtoolkit:TabPanel ID="TabPanel6" runat="server" HeaderText="Transfer">                    
<ContentTemplate>
<table style="width:100%; font-size:8pt;">
<tr><td style="font-size:8pt;" align="left">
Job  Information (Starting from DWASA joining up to Present Position) :
</td></tr>
<tr><td style="font-size:8pt;" align="left">
          <asp:UpdatePanel ID="UP4" runat="server" UpdateMode="Conditional"><ContentTemplate>
<asp:GridView CssClass="mGrid" ID="dgEmpTrans" runat="server" AutoGenerateColumns="False" 
        AllowPaging="True" Width="100%" BackColor="White" BorderWidth="1px" BorderStyle="Solid"
        CellPadding="2" BorderColor="LightGray" Font-Size="8pt" 
        AllowSorting="True" PageSize="20" onrowcancelingedit="dgEmpTrans_RowCancelingEdit" 
        onrowdeleting="dgEmpTrans_RowDeleting" onrowediting="dgEmpTrans_RowEditing" 
        onrowupdating="dgEmpTrans_RowUpdating" 
        onrowdatabound="dgEmpTrans_RowDataBound" 
        onrowcommand="dgEmpTrans_RowCommand">
  <HeaderStyle Font-Size="8pt" Font-Names="Arial" Font-Bold="True" BackColor="LightGray" 
        HorizontalAlign="Center" ForeColor="Black"/>

  <Columns>
  <asp:TemplateField>
  <ItemTemplate> 
  <asp:LinkButton ID="lbEdit" runat="server" CausesValidation="False" CommandName="Edit" Text="Edit" Font-Size="8pt" ></asp:LinkButton>  
  <asp:LinkButton ID="lbDelete" runat="server" CausesValidation="False" CommandName="Delete" Text="Remove" Font-Size="8pt"
  onclientclick="javascript:return window.confirm('are u really want to delete  these data')"></asp:LinkButton>  
  <asp:LinkButton ID="btnAddDet" runat="server" CommandName="AddNew" Text="New"></asp:LinkButton>  
      </ItemTemplate>
  <EditItemTemplate>  
  <asp:LinkButton ID="lbUpdate" runat="server" CausesValidation="True" CommandName="Update" Text="Update" Font-Size="8pt" ></asp:LinkButton> 
  <asp:LinkButton ID="lbCancel" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel" Font-Size="8pt"></asp:LinkButton>  
      </EditItemTemplate>
  <FooterTemplate>  
  <asp:LinkButton ID="lbInsert" runat="server" CommandName="Insert" Text="Add" Font-Size="8pt" ></asp:LinkButton>   
  <asp:LinkButton ID="lbCancel" runat="server" CausesValidation="False" Font-Size="8pt" CommandName="Cancel" Text="Cancel"></asp:LinkButton>    
      </FooterTemplate>
      <ControlStyle Font-Size="8pt" />
      <ItemStyle Font-Size="8pt" HorizontalAlign="Center" Width="130px" />
  </asp:TemplateField>
  
  <asp:TemplateField HeaderText="Office Order No">
  <ItemTemplate>
  <asp:Label ID="lblOrderNo" runat="server" Text='<%#Eval("order_no") %>' Width="150px" Font-Size="8pt"></asp:Label>  
      </ItemTemplate>
  <EditItemTemplate>
  <asp:TextBox SkinID="tbGray" ID="txtOrderNo"
            runat="server" Width="150px" TabIndex="101" Font-Size="8pt" MaxLength="35"></asp:TextBox>
      </EditItemTemplate>
  <FooterTemplate>
  <asp:TextBox SkinID="tbGray" ID="txtOrderNo" 
            runat="server" Width="150px" TabIndex="101" Font-Size="8pt" MaxLength="35"></asp:TextBox>
      </FooterTemplate>
      <ItemStyle HorizontalAlign="Left" Width="150px" Height="25px" />
  </asp:TemplateField>
  
  <asp:TemplateField HeaderText="Date">
  <ItemTemplate>
  <asp:Label ID="lblTransDate" runat="server" Text='<%# Eval("trans_date") %>' Width="100px"></asp:Label>  
      </ItemTemplate>
  <EditItemTemplate>
  <asp:TextBox SkinID="tbGray" ID="txtTransDate" PlaceHolder="dd/MM/yyyy"
            runat="server" Width="100px" TabIndex="102" Font-Size="8pt" MaxLength="11"></asp:TextBox>
             <ajaxtoolkit:calendarextender runat="server" ID="CalendarextenderTrans1" TargetControlID="txtTransDate" Format="dd/MM/yyyy"/>
      </EditItemTemplate>
  <FooterTemplate>
  <asp:TextBox SkinID="tbGray" ID="txtTransDate" PlaceHolder="dd/MM/yyyy"
            runat="server" Width="100px" TabIndex="102" Font-Size="8pt" MaxLength="11"></asp:TextBox>
             <ajaxtoolkit:calendarextender runat="server" ID="CalendarextenderTrans11" TargetControlID="txtTransDate" Format="dd/MM/yyyy"/>
      </FooterTemplate>
      <ItemStyle HorizontalAlign="Center" Width="100px" />
  </asp:TemplateField>
  
  <asp:TemplateField HeaderText="With Promotion?">
  <ItemTemplate>
  <asp:Label ID="lblTransProm" runat="server" Text='<%#Eval("trans_prom") %>' Width="100px"></asp:Label>  
      </ItemTemplate>
  <EditItemTemplate>
  <asp:DropDownList SkinID="ddlPlain" ID="ddlTransProm" runat="server" Font-Size="8pt" Width="100px" Height="18px" TabIndex="103" >
     <asp:ListItem></asp:ListItem>       
     <asp:ListItem Text="Yes" Value="Y"></asp:ListItem> 
     <asp:ListItem Text="No" Value="N"></asp:ListItem>
      </asp:DropDownList>
      </EditItemTemplate>
  <FooterTemplate>
  <asp:DropDownList SkinID="ddlPlain" ID="ddlTransProm" runat="server" Font-Size="8pt" Width="100px" Height="18px" TabIndex="103" >
     <asp:ListItem></asp:ListItem>       
     <asp:ListItem Text="Yes" Value="Y"></asp:ListItem> 
     <asp:ListItem Text="No" Value="N"></asp:ListItem>
      </asp:DropDownList>
      </FooterTemplate>
      <ItemStyle HorizontalAlign="Center" Width="100px" />
  </asp:TemplateField>
  
  <asp:TemplateField HeaderText="Branch">
  <ItemTemplate>
  <asp:Label ID="lblBranchCode" runat="server" Text='<%#Eval("BRANCH_CODE") %>' Width="150px"></asp:Label>  
      </ItemTemplate>
  <EditItemTemplate>
  <asp:DropDownList SkinID="ddlPlain" ID="ddlBranchKey" runat="server" Font-Size="8pt" Width="150px" 
  Height="18px" TabIndex="104" >
      </asp:DropDownList>
      </EditItemTemplate>
  <FooterTemplate>
  <asp:DropDownList SkinID="ddlPlain" ID="ddlBranchKey" runat="server" Font-Size="8pt" Width="150px" 
  Height="18px" TabIndex="104" >
      </asp:DropDownList>
      </FooterTemplate>
      <ItemStyle HorizontalAlign="Center" Width="150px" />
  </asp:TemplateField>
  
  <asp:TemplateField HeaderText="Designation">
  <ItemTemplate>
  <asp:Label ID="lblDesigCode" runat="server" Text='<%#Eval("desig_code") %>' Width="150px"></asp:Label>  
      </ItemTemplate>
  <EditItemTemplate>
  <asp:DropDownList SkinID="ddlPlain" ID="ddlDesigCode" runat="server" Font-Size="8pt" Width="150px" 
  Height="18px" TabIndex="105" >
      </asp:DropDownList>
      </EditItemTemplate>
  <FooterTemplate>
  <asp:DropDownList SkinID="ddlPlain" ID="ddlDesigCode" runat="server" Font-Size="8pt" Width="150px" 
  Height="18px" TabIndex="105" >
      </asp:DropDownList>
      </FooterTemplate>
      <ItemStyle HorizontalAlign="Center" Width="150px" />
  </asp:TemplateField>
    </Columns>
                        <AlternatingRowStyle CssClass="alt" />
    <PagerStyle CssClass="pgr" />
    </asp:GridView>   
    </ContentTemplate></asp:UpdatePanel>
</td></tr>
</table>
    </ContentTemplate>
</ajaxtoolkit:TabPanel>

<ajaxtoolkit:TabPanel ID="TabPanel7" runat="server" HeaderText="Promotion">                    
<ContentTemplate>
<table style="width:100%; font-size:8pt;">
<tr><td style="font-size:8pt;" align="left">
        <asp:UpdatePanel ID="UP5" runat="server" UpdateMode="Conditional"><ContentTemplate>
<asp:GridView CssClass="mGrid" PagerStyle-CssClass="pgr"  AlternatingRowStyle-CssClass="alt" ID="dgEmpProm" runat="server" AutoGenerateColumns="False" 
        AllowPaging="True" Width="100%" BackColor="White" BorderWidth="1px" BorderStyle="Solid"
        CellPadding="2" BorderColor="LightGray" Font-Size="8pt" 
        AllowSorting="True" PageSize="20" onrowcancelingedit="dgEmpProm_RowCancelingEdit" 
        onrowdeleting="dgEmpProm_RowDeleting" onrowediting="dgEmpProm_RowEditing" 
        onrowupdating="dgEmpProm_RowUpdating" 
        onrowdatabound="dgEmpProm_RowDataBound" 
        onrowcommand="dgEmpProm_RowCommand">
  <HeaderStyle Font-Size="8pt" Font-Names="Arial" Font-Bold="True" BackColor="LightGray" 
        HorizontalAlign="Center" ForeColor="Black"/>

  <Columns>
  <asp:TemplateField>
  <ItemTemplate> 
  <asp:LinkButton ID="lbEdit" runat="server" CausesValidation="False" CommandName="Edit" Text="Edit" Font-Size="8pt" ></asp:LinkButton>  
  <asp:LinkButton ID="lbDelete" runat="server" CausesValidation="False" CommandName="Delete" Text="Remove" Font-Size="8pt"
  onclientclick="javascript:return window.confirm('are u really want to delete  these data')"></asp:LinkButton>  
  <asp:LinkButton ID="btnAddDet" runat="server" CommandName="AddNew" Text="New"></asp:LinkButton>  
      </ItemTemplate>
  <EditItemTemplate>  
  <asp:LinkButton ID="lbUpdate" runat="server" CausesValidation="True" CommandName="Update" Text="Update" Font-Size="8pt" ></asp:LinkButton> 
  <asp:LinkButton ID="lbCancel" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel" Font-Size="8pt"></asp:LinkButton>  
      </EditItemTemplate>
  <FooterTemplate>  
  <asp:LinkButton ID="lbInsert" runat="server" CommandName="Insert" Text="Add" Font-Size="8pt" ></asp:LinkButton>   
  <asp:LinkButton ID="lbCancel" runat="server" CausesValidation="False" Font-Size="8pt" CommandName="Cancel" Text="Cancel"></asp:LinkButton>    
      </FooterTemplate>
      <ControlStyle Font-Size="8pt" />
      <ItemStyle Font-Size="8pt" HorizontalAlign="Center" Width="130px" />
  </asp:TemplateField>
  
  <asp:TemplateField HeaderText="Office Order No" ItemStyle-Height="25px">
  <ItemTemplate>
  <asp:Label ID="lblOffOrdNo" runat="server" Text='<%#Eval("off_ord_no") %>' Width="120px" Font-Size="8pt"></asp:Label>  
      </ItemTemplate>
  <EditItemTemplate>
  <asp:TextBox SkinID="tbGray" ID="txtOffOrdNo"
            runat="server" Width="120px" TabIndex="111" Font-Size="8pt" MaxLength="35"></asp:TextBox>
      </EditItemTemplate>
  <FooterTemplate>
  <asp:TextBox SkinID="tbGray" ID="txtOffOrdNo" 
            runat="server" Width="120px" TabIndex="111" Font-Size="8pt" MaxLength="35"></asp:TextBox>
      </FooterTemplate>
      <ItemStyle HorizontalAlign="Left" Width="120px" />
  </asp:TemplateField>
  
  <asp:TemplateField HeaderText="Date">
  <ItemTemplate>
  <asp:Label ID="lblJoiningDate" runat="server" Text='<%# Eval("joining_date") %>' Width="70px"></asp:Label>  
  <%--Text='<%# String.Format("{0:dd/MM/yy}",((DataRowView)Container.DataItem)["joining_date"]) %>'--%>
      </ItemTemplate>
  <EditItemTemplate>
  <asp:TextBox SkinID="tbGray" ID="txtJoiningDate" PlaceHolder="dd/MM/yyyy"
            runat="server" Width="70px" TabIndex="112" Font-Size="8pt" MaxLength="11"></asp:TextBox>
             <ajaxtoolkit:calendarextender runat="server" ID="Calendarextender2Join" TargetControlID="txtJoiningDate" Format="dd/MM/yyyy"/>
      </EditItemTemplate>
  <FooterTemplate>
  <asp:TextBox SkinID="tbGray" ID="txtJoiningDate" PlaceHolder="dd/MM/yyyy"
            runat="server" Width="70px" TabIndex="112" Font-Size="8pt" MaxLength="11"></asp:TextBox>
             <ajaxtoolkit:calendarextender runat="server" ID="Calendarextender1Join" TargetControlID="txtJoiningDate" Format="dd/MM/yyyy"/>
      </FooterTemplate>
      <ItemStyle HorizontalAlign="Center" Width="70px" />
  </asp:TemplateField>  
  
  <asp:TemplateField HeaderText="Branch">
  <ItemTemplate>
  <asp:Label ID="lblJoiningBranch" runat="server" Text='<%#Eval("joining_branch") %>' Width="150px" Font-Size="8pt"></asp:Label>  
      </ItemTemplate>
  <EditItemTemplate>
  <asp:DropDownList SkinID="ddlPlain" ID="ddlJoiningBranch" runat="server" Font-Size="8pt" Width="150px" 
  Height="18px" TabIndex="113" >
      </asp:DropDownList>
      </EditItemTemplate>
  <FooterTemplate>
  <asp:DropDownList SkinID="ddlPlain" ID="ddlJoiningBranch" runat="server" Font-Size="8pt" Width="150px" 
  Height="18px" TabIndex="113" >
      </asp:DropDownList>
      </FooterTemplate>
      <ItemStyle HorizontalAlign="Center" Width="150px" />
  </asp:TemplateField>
  
  <asp:TemplateField HeaderText="Designation">
  <ItemTemplate>
  <asp:Label ID="lblJoiningDesig" runat="server" Text='<%#Eval("joining_desig") %>' Width="150px" Font-Size="8pt"></asp:Label>  
      </ItemTemplate>
  <EditItemTemplate>
  <asp:DropDownList SkinID="ddlPlain" ID="ddlJoiningDesig" runat="server" Font-Size="8pt" Width="150px" 
  Height="18px" TabIndex="114" >
      </asp:DropDownList>
      </EditItemTemplate>
  <FooterTemplate>
  <asp:DropDownList SkinID="ddlPlain" ID="ddlJoiningDesig" runat="server" Font-Size="8pt" Width="150px" 
  Height="18px" TabIndex="114" >
      </asp:DropDownList>
      </FooterTemplate>
      <ItemStyle HorizontalAlign="Center" Width="150px" />
  </asp:TemplateField>
  
  <asp:TemplateField HeaderText="Basic Pay">
  <ItemTemplate>
  <asp:Label ID="lblBasicPay" runat="server" Text='<%#Eval("basic_pay") %>' Width="50px"></asp:Label>  
      </ItemTemplate>
  <EditItemTemplate>
  <asp:TextBox SkinID="tbGray" ID="txtBasicPay" 
            runat="server" Width="50px" TabIndex="115" Font-Size="8pt" MaxLength="5"></asp:TextBox>
      </EditItemTemplate>
  <FooterTemplate>
  <asp:TextBox SkinID="tbGray" ID="txtbasicPay" 
            runat="server" Width="50px" TabIndex="115" Font-Size="8pt" MaxLength="5"></asp:TextBox>
      </FooterTemplate>
      <ItemStyle HorizontalAlign="Center" Width="50px" />
  </asp:TemplateField>
  
  <asp:TemplateField HeaderText="Pay Scale">
  <ItemTemplate>
  <asp:Label ID="lblPayScale" runat="server" Text='<%#Eval("pay_scale") %>' Width="130px"></asp:Label>  
      </ItemTemplate>
  <EditItemTemplate>
  <asp:TextBox SkinID="tbGray" ID="txtPayScale" 
            runat="server" Width="130px" TabIndex="116" Font-Size="8pt" MaxLength="30"></asp:TextBox>
      </EditItemTemplate>
  <FooterTemplate>
  <asp:TextBox SkinID="tbGray" ID="txtPayScale" 
            runat="server" Width="130px" TabIndex="116" Font-Size="8pt" MaxLength="30"></asp:TextBox>
      </FooterTemplate>
      <ItemStyle HorizontalAlign="Center" Width="130px" />
  </asp:TemplateField>
    </Columns>
<EditRowStyle BackColor="" />
                        <AlternatingRowStyle BackColor="" />
    </asp:GridView>  
    </ContentTemplate></asp:UpdatePanel> 
</td></tr>
</table>
    </ContentTemplate>
</ajaxtoolkit:TabPanel>

<ajaxtoolkit:TabPanel ID="TabPanel8" runat="server" HeaderText="Document">                    
<ContentTemplate>
<div>
            <fieldset style="vertical-align: top; border: solid .5px #8BB381;text-align:left;line-height:1.5em;"><legend style="color: maroon;">
    <b>Upload Document</b></legend>
                                    <table style="width: 100%">
                            <tr>
                                <td style="width: 15%; font-weight: 700; height: 27px;">
                                    File Description</td>
                                <td style="width: 2%; font-weight: 700; height: 27px;" align="center">
                                    :</td>
                                <td colspan="2" style="height: 27px">
                                    <asp:TextBox ID="txtFileDescription" runat="server" CssClass="tbc" Font-Size="8pt" 
                                        SkinID="tbPlain" Style="text-align: left;" Width="100%"></asp:TextBox>
                                </td>
                                <td align="center" style="width: 2%; height: 27px;">
                                    </td>
                                <td align="center" style="width: 15%; height: 27px;">
                                    </td>
                            </tr>
                                         <tr>
                                             <td style="width: 15%; font-weight: 700;">
                                                 Select File</td>
                                             <td align="center" style="width: 2%; font-weight: 700;">
                                                 :</td>
                                             <td style="width: 40%">
                                                 <asp:FileUpload ID="fileUpload1" runat="server" Width="100%" />
                                             </td>
                                             <td align="right" style="width: 15%">
                                                 <asp:Button ID="btnUploadFile" runat="server" Height="35px" 
                                                     OnClick="btnUploadFile_Click" Text="Upload" Width="80%" Font-Bold="True" />
                                             </td>
                                             <td align="center" style="width: 2%">
                                                 &nbsp;</td>
                                             <td align="center" style="width: 15%">
                                                 &nbsp;</td>
                                         </tr>
                                         <tr>
                                             <td style="width: 15%; font-weight: 700;">
                                                 &nbsp;</td>
                                             <td style="width: 2%">
                                                 &nbsp;</td>
                                             <td colspan="2">
                                                 <asp:GridView ID="dgQuestion" runat="server" AutoGenerateColumns="False" 
                                                     CssClass="mGrid" onrowcommand="dgQuestion_RowCommand" onrowdatabound="dgSubject_RowDataBound" 
                                                     Width="100%" onselectedindexchanged="dgQuestion_SelectedIndexChanged">
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
                                                          <asp:CommandField HeaderText="Delete" ShowSelectButton="True" 
                                                             SelectText="Delete">
                                                                    <ItemStyle HorizontalAlign="Center" Width="5%" Font-Bold="True" 
                                                             Font-Size="10pt" />
                                                          </asp:CommandField>
                                                     </Columns>
                                                 </asp:GridView>
                                             </td>
                                             <td align="center" style="width: 15%">
                                                 &nbsp;</td>
                                         </tr>
                                         <tr>
                                             <td style="width: 15%; font-weight: 700;">
                                                 &nbsp;</td>
                                             <td style="width: 2%">
                                                 &nbsp;</td>
                                             <td style="width: 40%">
                                                 &nbsp;</td>
                                             <td align="right" style="width: 15%">
                                                 &nbsp;</td>
                                             <td align="center" style="width: 2%">
                                                 &nbsp;</td>
                                             <td align="center" style="width: 15%">
                                                 &nbsp;</td>
                                         </tr>
                                    </table>
                                    </fieldset >
       </div>
    </ContentTemplate>
</ajaxtoolkit:TabPanel>

<%--<ajaxtoolkit:TabPanel ID="TabPanel9" runat="server" HeaderText="Suspension">                    
<ContentTemplate>
<table style="width:100%; font-size:8pt;">
<tr><td style="font-size:x-small;" align="left">
    &nbsp;</td></tr>
</table>
    </ContentTemplate>
</ajaxtoolkit:TabPanel>--%>

<%--<ajaxtoolkit:TabPanel ID="TabPanel10" runat="server" HeaderText="Membership">                    
<ContentTemplate>
<table style="width:100%; font-size:8pt;">
<tr><td style="font-size:x-small;" align="left">
    &nbsp;</td></tr>
</table>
    </ContentTemplate>
</ajaxtoolkit:TabPanel>--%>

    <ajaxtoolkit:TabPanel ID="tpEmpList" runat="server" HeaderText="Employee List">                    
<ContentTemplate>
    <div style="width:100%;">
        <asp:GridView ID="dgEmp" CssClass="mGrid"
    runat="server" AutoGenerateColumns="False" 
        AllowPaging="True" Width="100%" BackColor="White" BorderWidth="1px" BorderStyle="Solid"
        CellPadding="2" BorderColor="LightGray" Font-Size="8pt" 
        AllowSorting="True" PageSize="40" ForeColor="#333333" 
        onselectedindexchanged="dgEmp_SelectedIndexChanged" 
        onpageindexchanging="dgEmp_PageIndexChanging" 
            onrowdatabound="dgEmp_RowDataBound">
            <AlternatingRowStyle CssClass="alt" />
  <Columns>
  <asp:CommandField ShowSelectButton="True">
      <ItemStyle ForeColor="Blue" Height="21px" HorizontalAlign="Center" 
          Width="40px" />
      </asp:CommandField>
      <asp:BoundField DataField="EMP_NO" HeaderText="Employee Id" />
      <asp:BoundField DataField="Emp_name" HeaderText="Employee Name" />
      <asp:BoundField DataField="JOIN_DATE" HeaderText="Date Of Joining" />
      <asp:BoundField DataField="RELIGION_CODE" HeaderText="Religion"  />
      <asp:BoundField DataField="DESIG_NAME" HeaderText="Designation" />
          <asp:BoundField DataField="Country" HeaderText="Country"  />
           <asp:BoundField DataField="ID" HeaderText="ID" 
          SortExpression="EMP_NO" />
  </Columns>
                        <PagerStyle HorizontalAlign="Center" CssClass="pgr" />
                        <HeaderStyle Font-Bold="True" ForeColor="Black" />
  </asp:GridView>
    </div>

    </ContentTemplate>
</ajaxtoolkit:TabPanel>
</ajaxtoolkit:TabContainer>
</div>
<%--</ContentTemplate>
</asp:UpdatePanel>--%>
</td>
<td style="width:1%;"></td>
</tr>
<tr>
<td style="width:1%;">&nbsp;</td>
<td style="width:98%; " align="center">

    <asp:GridView runat="server" AllowSorting="True" AutoGenerateColumns="False" 
        CellPadding="2" PageSize="20" BackColor="White" BorderColor="LightGray" 
        BorderWidth="1px" BorderStyle="Solid" CssClass="mGrid" Font-Size="8pt" 
        Width="100%" ID="dgMember" OnRowCancelingEdit="dgMember_RowCancelingEdit" 
        OnRowDeleting="dgMember_RowDeleting" OnRowEditing="dgMember_RowEditing" 
        OnRowUpdating="dgMember_RowUpdating" OnRowDataBound="dgMember_RowDataBound" 
        OnRowCommand="dgMember_RowCommand" Visible="False">
        <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
        <Columns>
            <asp:TemplateField>
                <EditItemTemplate>
                    <asp:LinkButton ID="lbUpdate0" runat="server" CausesValidation="True" 
          CommandName="Update" Text="Update" ></asp:LinkButton>
                    <asp:LinkButton ID="lbCancel0" runat="server" CausesValidation="False" 
          CommandName="Cancel" Text="Cancel"></asp:LinkButton>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:LinkButton ID="lbInsert0" runat="server" CommandName="Insert" Text="Add" ></asp:LinkButton>
                    <asp:LinkButton ID="lbCancel1" runat="server" CausesValidation="False" 
          CommandName="Cancel" Text="Cancel"></asp:LinkButton>
                </FooterTemplate>
                <ItemTemplate>
                    <asp:LinkButton ID="lbEdit0" runat="server" CausesValidation="False" 
          CommandName="Edit" Text="Edit" ></asp:LinkButton>
                    <asp:LinkButton ID="lbDelete0" runat="server" CausesValidation="False" 
          CommandName="Delete" Text="Remove"
  
          
                        onclientclick="javascript:return window.confirm('are u really want to delete  these data')"></asp:LinkButton>
                    <asp:LinkButton ID="btnAddDet0" runat="server" CommandName="New" Text="New"></asp:LinkButton>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Font-Size="8pt" Width="130px"></ItemStyle>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Association">
                <EditItemTemplate>
                    <asp:DropDownList SkinID="ddlPlain" ID="ddlAssoId" runat="server" Font-Size="8pt" 
          Width="600px" Height="18px" TabIndex="71" >
                    </asp:DropDownList>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:DropDownList SkinID="ddlPlain" ID="ddlAssoId0" runat="server" Font-Size="8pt" 
          Width="600px" Height="18px" TabIndex="71" >
                    </asp:DropDownList>
                </FooterTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblAssoId" runat="server" Text='<%#Eval("asso_id") %>' 
          Width="600px"  Font-Size="8pt"></asp:Label>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Left" Height="25px" Width="600px"></ItemStyle>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Membership#">
                <EditItemTemplate>
                    <asp:TextBox SkinID="tbGray" ID="txtMemberNo" 
            runat="server" Width="100px" TabIndex="73" Font-Size="8pt" 
          MaxLength="20"></asp:TextBox>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:TextBox SkinID="tbGray" ID="txtMemberNo0" 
            runat="server" Width="100px" TabIndex="73" Font-Size="8pt" 
          MaxLength="20"></asp:TextBox>
                </FooterTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblMemberNo" runat="server" Text='<%#Eval("member_no") %>'  
          Font-Size="8pt"></asp:Label>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="100px"></ItemStyle>
            </asp:TemplateField>
        </Columns>
        <HeaderStyle HorizontalAlign="Center" BackColor="LightGray" Font-Size="8pt" 
            Font-Bold="True" Font-Names="Arial" ForeColor="Black"></HeaderStyle>
        <PagerStyle CssClass="pgr"></PagerStyle>
    </asp:GridView>
    <asp:GridView runat="server" AllowSorting="True" AutoGenerateColumns="False" 
        CellPadding="2" PageSize="20" BackColor="White" BorderColor="LightGray" 
        BorderWidth="1px" BorderStyle="Solid" CssClass="mGrid" Font-Size="8pt" 
        Width="100%" ID="dgEmpSusp" OnRowCancelingEdit="dgEmpSusp_RowCancelingEdit" 
        OnRowDeleting="dgEmpSusp_RowDeleting" OnRowEditing="dgEmpSusp_RowEditing" 
        OnRowUpdating="dgEmpSusp_RowUpdating" OnRowDataBound="dgEmpSusp_RowDataBound" 
        OnRowCommand="dgEmpSusp_RowCommand" Visible="False">
        <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
        <Columns>
            <asp:TemplateField>
                <EditItemTemplate>
                    <asp:LinkButton ID="lbUpdate1" runat="server" CausesValidation="True" 
          CommandName="Update" Text="Update" ></asp:LinkButton>
                    <asp:LinkButton ID="lbCancel2" runat="server" CausesValidation="False" 
          CommandName="Cancel" Text="Cancel"></asp:LinkButton>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:LinkButton ID="lbInsert1" runat="server" CommandName="Insert" Text="Add" ></asp:LinkButton>
                    <asp:LinkButton ID="lbCancel3" runat="server" CausesValidation="False" 
          CommandName="Cancel" Text="Cancel"></asp:LinkButton>
                </FooterTemplate>
                <ItemTemplate>
                    <asp:LinkButton ID="lbEdit1" runat="server" CausesValidation="False" 
          CommandName="Edit" Text="Edit" ></asp:LinkButton>
                    <asp:LinkButton ID="lbDelete1" runat="server" CausesValidation="False" 
          CommandName="Delete" Text="Remove"
  
          
                        onclientclick="javascript:return window.confirm('are u really want to delete  these data')"></asp:LinkButton>
                    <asp:LinkButton ID="btnAddDet1" runat="server" CommandName="AddNew" Text="New"></asp:LinkButton>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Font-Size="8pt" Width="130px"></ItemStyle>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Office Order No">
                <EditItemTemplate>
                    <asp:TextBox SkinID="tbGray" ID="txtOffOrderNo"
            runat="server" Width="120px" TabIndex="131" Font-Size="8pt" 
          MaxLength="35"></asp:TextBox>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:TextBox SkinID="tbGray" ID="txtOffOrderNo0" 
            runat="server" Width="120px" TabIndex="131" Font-Size="8pt" 
          MaxLength="35"></asp:TextBox>
                </FooterTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblOffOrderNo" runat="server" Text='<%#Eval("off_order_no") %>' 
          Width="120px" Font-Size="8pt"></asp:Label>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Left" Height="25px" Width="120px"></ItemStyle>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Susp. Date">
                <EditItemTemplate>
                    <asp:TextBox SkinID="tbGray" ID="txtSuspenDate" 
            runat="server" Width="70px" TabIndex="132" Font-Size="8pt" 
          MaxLength="11"></asp:TextBox>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:TextBox SkinID="tbGray" ID="txtSuspenDate0" 
            runat="server" Width="70px" TabIndex="132" Font-Size="8pt" 
          MaxLength="11"></asp:TextBox>
                </FooterTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblSuspenDate" runat="server" Text='<%# Eval("suspen_date") %>' 
          Width="70px"></asp:Label>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="70px"></ItemStyle>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Clause">
                <EditItemTemplate>
                    <asp:TextBox SkinID="tbGray" ID="txtSuspenClause" 
            runat="server" Width="150px" TabIndex="133" Font-Size="8pt" 
          MaxLength="70"></asp:TextBox>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:TextBox SkinID="tbGray" ID="txtSuspenClause0" 
            runat="server" Width="150px" TabIndex="133" Font-Size="8pt" 
          MaxLength="70"></asp:TextBox>
                </FooterTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblSuspenClause" runat="server" Text='<%#Eval("suspen_clause") %>' 
          Width="150px" Font-Size="8pt"></asp:Label>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="70px"></ItemStyle>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Withdrawn Order No">
                <EditItemTemplate>
                    <asp:TextBox SkinID="tbGray" ID="txtWithdrawOrderNo" 
            runat="server" Width="100px" TabIndex="134" Font-Size="8pt" 
          MaxLength="35"></asp:TextBox>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:TextBox SkinID="tbGray" ID="txtWithdrawOrderNo0" 
            runat="server" Width="100px" TabIndex="134" Font-Size="8pt" 
          MaxLength="35"></asp:TextBox>
                </FooterTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblWithdrawOrderNo" runat="server" 
          Text='<%#Eval("withdraw_order_no") %>' Width="100px" Font-Size="8pt"></asp:Label>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Left" Width="100px"></ItemStyle>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Withdrawn Date">
                <EditItemTemplate>
                    <asp:TextBox SkinID="tbGray" ID="txtWithDate" 
            runat="server" Width="70px" TabIndex="135" Font-Size="8pt" 
          MaxLength="11"></asp:TextBox>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:TextBox SkinID="tbGray" ID="txtWithDate0" 
            runat="server" Width="70px" TabIndex="135" Font-Size="8pt" 
          MaxLength="11"></asp:TextBox>
                </FooterTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblWithDate" runat="server" Text='<%# Eval("with_date") %>' 
          Width="70px"></asp:Label>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="70px"></ItemStyle>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Punishment">
                <EditItemTemplate>
                    <asp:TextBox SkinID="tbGray" ID="txtPunishment" 
            runat="server" Width="100px" TabIndex="136" Font-Size="8pt" 
          MaxLength="35"></asp:TextBox>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:TextBox SkinID="tbGray" ID="txtPunishment0" 
            runat="server" Width="100px" TabIndex="136" Font-Size="8pt" 
          MaxLength="35"></asp:TextBox>
                </FooterTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblPunishment" runat="server" Text='<%#Eval("punishment") %>' 
          Width="100px"></asp:Label>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="100px"></ItemStyle>
            </asp:TemplateField>
        </Columns>
        <HeaderStyle HorizontalAlign="Center" BackColor="LightGray" Font-Size="8pt" 
            Font-Bold="True" Font-Names="Arial" ForeColor="Black"></HeaderStyle>
        <PagerStyle CssClass="pgr"></PagerStyle>
    </asp:GridView>
    <asp:GridView runat="server" AllowSorting="True" AutoGenerateColumns="False" 
        CellPadding="2" PageSize="20" BackColor="White" BorderColor="LightGray" 
        BorderWidth="1px" BorderStyle="Solid" CssClass="mGrid" Font-Size="8pt" 
        Width="100%" ID="dgEmpQtr" OnRowCancelingEdit="dgEmpQtr_RowCancelingEdit" 
        OnRowDeleting="dgEmpQtr_RowDeleting" OnRowEditing="dgEmpQtr_RowEditing" 
        OnRowUpdating="dgEmpQtr_RowUpdating" OnRowDataBound="dgEmpQtr_RowDataBound" 
        OnRowCommand="dgEmpQtr_RowCommand" Visible="False">
        <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
        <Columns>
            <asp:TemplateField>
                <EditItemTemplate>
                    <asp:LinkButton ID="lbUpdate2" runat="server" CausesValidation="True" 
          CommandName="Update" Text="Update" Font-Size="8pt" ></asp:LinkButton>
                    <asp:LinkButton ID="lbCancel4" runat="server" CausesValidation="False" 
          CommandName="Cancel" Text="Cancel" Font-Size="8pt"></asp:LinkButton>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:LinkButton ID="lbInsert2" runat="server" CommandName="Insert" Text="Add" 
          Font-Size="8pt" ></asp:LinkButton>
                    <asp:LinkButton ID="lbCancel5" runat="server" CausesValidation="False" 
          Font-Size="8pt" CommandName="Cancel" Text="Cancel"></asp:LinkButton>
                </FooterTemplate>
                <ItemTemplate>
                    <asp:LinkButton ID="lbEdit2" runat="server" CausesValidation="False" 
          CommandName="Edit" Text="Edit" Font-Size="8pt" ></asp:LinkButton>
                    <asp:LinkButton ID="lbDelete2" runat="server" CausesValidation="False" 
          CommandName="Delete" Text="Remove" Font-Size="8pt"
  
          
                        onclientclick="javascript:return window.confirm('are u really want to delete  these data')"></asp:LinkButton>
                    <asp:LinkButton ID="btnAddDet2" runat="server" CommandName="AddNew" Text="New"></asp:LinkButton>
                </ItemTemplate>
                <ControlStyle Font-Size="8pt"></ControlStyle>
                <ItemStyle HorizontalAlign="Center" Font-Size="8pt" Width="130px"></ItemStyle>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Allotment Reference">
                <EditItemTemplate>
                    <asp:TextBox SkinID="tbGray" ID="txtAllotRef"
            runat="server" Width="120px" TabIndex="121" Font-Size="8pt" 
          MaxLength="35"></asp:TextBox>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:TextBox SkinID="tbGray" ID="txtAllotRef0" 
            runat="server" Width="120px" TabIndex="121" Font-Size="8pt" 
          MaxLength="35"></asp:TextBox>
                </FooterTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblAllotRef" runat="server" Text='<%#Eval("allot_ref") %>' 
          Width="120px" Font-Size="8pt"></asp:Label>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Left" Height="25px" Width="120px"></ItemStyle>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Ref. Date">
                <EditItemTemplate>
                    <asp:TextBox SkinID="tbGray" ID="txtRefDate" 
            runat="server" Width="70px" TabIndex="122" Font-Size="8pt" 
          MaxLength="11"></asp:TextBox>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:TextBox SkinID="tbGray" ID="txtRefDate0" 
            runat="server" Width="70px" TabIndex="122" Font-Size="8pt" 
          MaxLength="11"></asp:TextBox>
                </FooterTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblRefDate" runat="server" Text='<%# Eval("ref_date") %>' 
          Width="70px"></asp:Label>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="70px"></ItemStyle>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Positioning Date">
                <EditItemTemplate>
                    <asp:TextBox SkinID="tbGray" ID="txtPostDate" 
            runat="server" Width="70px" TabIndex="123" Font-Size="8pt" 
          MaxLength="11"></asp:TextBox>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:TextBox SkinID="tbGray" ID="txtPostDate0" 
            runat="server" Width="70px" TabIndex="123" Font-Size="8pt" 
          MaxLength="11"></asp:TextBox>
                </FooterTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblPostDate" runat="server" Text='<%# Eval("post_date") %>' 
          Width="70px"></asp:Label>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="70px"></ItemStyle>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Location">
                <EditItemTemplate>
                    <asp:TextBox SkinID="tbGray" ID="txtLocat" 
            runat="server" Width="70px" TabIndex="124" Font-Size="8pt" 
          MaxLength="25"></asp:TextBox>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:TextBox SkinID="tbGray" ID="txtLocat0" 
            runat="server" Width="70px" TabIndex="124" Font-Size="8pt" 
          MaxLength="25"></asp:TextBox>
                </FooterTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblLocat" runat="server" Text='<%#Eval("locat") %>' Width="70px"></asp:Label>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="70px"></ItemStyle>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Road#">
                <EditItemTemplate>
                    <asp:TextBox SkinID="tbGray" ID="txtRoad" 
            runat="server" Width="50px" TabIndex="125" Font-Size="8pt" MaxLength="5"></asp:TextBox>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:TextBox SkinID="tbGray" ID="txtRoad0" 
            runat="server" Width="50px" TabIndex="125" Font-Size="8pt" MaxLength="5"></asp:TextBox>
                </FooterTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblRoad" runat="server" Text='<%#Eval("road") %>' Width="50px"></asp:Label>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="50px"></ItemStyle>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Building#">
                <EditItemTemplate>
                    <asp:TextBox SkinID="tbGray" ID="txtBuild" 
            runat="server" Width="50px" TabIndex="126" Font-Size="8pt" 
          MaxLength="10"></asp:TextBox>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:TextBox SkinID="tbGray" ID="txtBuild0" 
            runat="server" Width="50px" TabIndex="126" Font-Size="8pt" 
          MaxLength="10"></asp:TextBox>
                </FooterTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblBuild" runat="server" Text='<%#Eval("build") %>' Width="50px"></asp:Label>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="50px"></ItemStyle>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Flat#">
                <EditItemTemplate>
                    <asp:TextBox SkinID="tbGray" ID="txtFlat" 
            runat="server" Width="50px" TabIndex="127" Font-Size="8pt" 
          MaxLength="10"></asp:TextBox>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:TextBox SkinID="tbGray" ID="txtFlat0" 
            runat="server" Width="50px" TabIndex="127" Font-Size="8pt" 
          MaxLength="10"></asp:TextBox>
                </FooterTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblFlat" runat="server" Text='<%#Eval("flat") %>' Width="50px"></asp:Label>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="50px"></ItemStyle>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Flat Type">
                <EditItemTemplate>
                    <asp:TextBox SkinID="tbGray" ID="txtFlatTyp" 
            runat="server" Width="70px" TabIndex="128" Font-Size="8pt" 
          MaxLength="15"></asp:TextBox>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:TextBox SkinID="tbGray" ID="txtFlatTyp0" 
            runat="server" Width="70px" TabIndex="128" Font-Size="8pt" 
          MaxLength="15"></asp:TextBox>
                </FooterTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblFlatTyp" runat="server" Text='<%#Eval("flat_typ") %>' 
          Width="70px"></asp:Label>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="70px"></ItemStyle>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Size">
                <EditItemTemplate>
                    <asp:TextBox SkinID="tbGray" ID="txtSizee" 
            runat="server" Width="70px" TabIndex="129" Font-Size="8pt" MaxLength="5"></asp:TextBox>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:TextBox SkinID="tbGray" ID="txtSizee0" 
            runat="server" Width="70px" TabIndex="129" Font-Size="8pt" MaxLength="5"></asp:TextBox>
                </FooterTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblSizee" runat="server" Text='<%#Eval("sizee") %>' Width="70px"></asp:Label>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="70px"></ItemStyle>
            </asp:TemplateField>
        </Columns>
        <HeaderStyle HorizontalAlign="Center" BackColor="LightGray" Font-Size="8pt" 
            Font-Bold="True" Font-Names="Arial" ForeColor="Black"></HeaderStyle>
        <PagerStyle CssClass="pgr"></PagerStyle>
    </asp:GridView>
    <asp:GridView runat="server" AllowSorting="True" AutoGenerateColumns="False" 
        CellPadding="2" PageSize="20" BackColor="White" BorderColor="LightGray" 
        BorderWidth="1px" BorderStyle="Solid" CssClass="mGrid" Font-Size="8pt" 
        Width="100%" ID="dgEmpTrain" OnRowCancelingEdit="dgEmpTrain_RowCancelingEdit" 
        OnRowDeleting="dgEmpTrain_RowDeleting" OnRowEditing="dgEmpTrain_RowEditing" 
        OnRowUpdating="dgEmpTrain_RowUpdating" OnRowDataBound="dgEmpTrain_RowDataBound" 
        OnRowCommand="dgEmpTrain_RowCommand" Visible="False">
        <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
        <Columns>
            <asp:TemplateField>
                <EditItemTemplate>
                    <asp:LinkButton ID="lbUpdate3" runat="server" CausesValidation="True" 
          CommandName="Update" Text="Update" Font-Size="8pt" ></asp:LinkButton>
                    <asp:LinkButton ID="lbCancel6" runat="server" CausesValidation="False" 
          CommandName="Cancel" Text="Cancel" Font-Size="8pt"></asp:LinkButton>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:LinkButton ID="lbInsert3" runat="server" CommandName="Insert" Text="Add" 
                        Font-Size="8pt" ></asp:LinkButton>
                    <asp:LinkButton ID="lbCancel7" runat="server" CausesValidation="False" 
          CommandName="Cancel" Text="Cancel" Font-Size="8pt"></asp:LinkButton>
                </FooterTemplate>
                <ItemTemplate>
                    <asp:LinkButton ID="lbEdit3" runat="server" CausesValidation="False" 
          CommandName="Edit" Text="Edit" Font-Size="8pt" ></asp:LinkButton>
                    <asp:LinkButton ID="lbDelete3" runat="server" CausesValidation="False" 
          CommandName="Delete" Text="Remove"
  
          
                        onclientclick="javascript:return window.confirm('are u really want to delete  these data')" 
                        Font-Size="8pt"></asp:LinkButton>
                    <asp:LinkButton ID="btnAddDet3" runat="server" CommandName="AddNew" Text="New"></asp:LinkButton>
                </ItemTemplate>
                <ControlStyle Font-Size="8pt"></ControlStyle>
                <ItemStyle HorizontalAlign="Center" Font-Size="8pt" Width="130px"></ItemStyle>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Training Title">
                <EditItemTemplate>
                    <asp:TextBox SkinID="tbGray" ID="txtTrainTitle" 
            runat="server" Width="150px" TabIndex="91" Font-Size="8pt" 
          MaxLength="150"></asp:TextBox>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:TextBox SkinID="tbGray" ID="txtTrainTitle0" 
            runat="server" Width="150px" TabIndex="91" Font-Size="8pt" 
          MaxLength="150"></asp:TextBox>
                </FooterTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblTrainTitle" runat="server" Text='<%#Eval("train_title") %>' 
          Width="150px" Font-Size="8pt"></asp:Label>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Left" Height="25px" Width="150px"></ItemStyle>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Year">
                <EditItemTemplate>
                    <asp:TextBox SkinID="tbGray" ID="txtYear" 
            runat="server" Width="50px" TabIndex="92" Font-Size="8pt" 
          MaxLength="4"></asp:TextBox>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:TextBox SkinID="tbGray" ID="txtYear0" 
            runat="server" Width="50px" TabIndex="92" Font-Size="8pt" 
          MaxLength="4"></asp:TextBox>
                </FooterTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblYear" runat="server" Text='<%#Eval("year") %>' 
          Width="50px"></asp:Label>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="50px"></ItemStyle>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Place">
                <EditItemTemplate>
                    <asp:TextBox SkinID="tbGray" ID="txtPlace" 
            runat="server" Width="100px" TabIndex="93" Font-Size="8pt" MaxLength="35"></asp:TextBox>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:TextBox SkinID="tbGray" ID="txtPlace0" 
            runat="server" Width="100px" TabIndex="93" Font-Size="8pt" MaxLength="35"></asp:TextBox>
                </FooterTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblPlace" runat="server" Text='<%#Eval("place") %>' 
                        Width="100px"></asp:Label>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="100px"></ItemStyle>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Country">
                <EditItemTemplate>
                    <asp:TextBox SkinID="tbGray" ID="txtCountry" 
            runat="server" Width="80px" TabIndex="94" Font-Size="8pt" MaxLength="25"></asp:TextBox>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:TextBox SkinID="tbGray" ID="txtCountry0" 
            runat="server" Width="80px" TabIndex="94" Font-Size="8pt" MaxLength="25"></asp:TextBox>
                </FooterTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblCountry" runat="server" Text='<%#Eval("country") %>' 
                        Width="80px"></asp:Label>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="80px"></ItemStyle>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Financed By">
                <EditItemTemplate>
                    <asp:TextBox SkinID="tbGray" ID="txtFinan" 
            runat="server" Width="90px" TabIndex="95" Font-Size="8pt" 
          MaxLength="25"></asp:TextBox>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:TextBox SkinID="tbGray" ID="txtFinan0" 
            runat="server" Width="90px" TabIndex="95" Font-Size="8pt" 
          MaxLength="25"></asp:TextBox>
                </FooterTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblFinan" runat="server" Text='<%#Eval("finan") %>' 
          Font-Size="8pt" Width="90px"></asp:Label>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="90px"></ItemStyle>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Amount">
                <EditItemTemplate>
                    <asp:TextBox SkinID="tbGray" ID="txtAmount" 
            runat="server" Width="70px" TabIndex="96" Font-Size="8pt" MaxLength="15"></asp:TextBox>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:TextBox SkinID="tbGray" ID="txtAmount0" 
            runat="server" Width="70px" TabIndex="96" Font-Size="8pt" MaxLength="15"></asp:TextBox>
                </FooterTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblAmount" runat="server" Text='<%#Eval("amount") %>'></asp:Label>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="70px"></ItemStyle>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Year">
                <EditItemTemplate>
                    <asp:TextBox SkinID="tbGray" ID="txtDuYear" 
            runat="server" Width="30px" TabIndex="97" Font-Size="8pt" MaxLength="2"></asp:TextBox>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:TextBox SkinID="tbGray" ID="txtDuYear0" 
            runat="server" Width="30px" TabIndex="97" Font-Size="8pt" MaxLength="2"></asp:TextBox>
                </FooterTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblDuYear" runat="server" Text='<%#Eval("du_year") %>' 
                        Width="30px"></asp:Label>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="30px"></ItemStyle>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Month">
                <EditItemTemplate>
                    <asp:TextBox SkinID="tbGray" ID="txtDuMonth" 
            runat="server" Width="30px" TabIndex="98" Font-Size="8pt" MaxLength="2"></asp:TextBox>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:TextBox SkinID="tbGray" ID="txtDuMonth0" 
            runat="server" Width="30px" TabIndex="98" Font-Size="8pt" MaxLength="2"></asp:TextBox>
                </FooterTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblDuMonth" runat="server" Text='<%#Eval("du_month") %>' 
          Width="30px"></asp:Label>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="30px"></ItemStyle>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Day">
                <EditItemTemplate>
                    <asp:TextBox SkinID="tbGray" ID="txtDuDay" 
            runat="server" Width="30px" TabIndex="99" Font-Size="8pt" MaxLength="3"></asp:TextBox>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:TextBox SkinID="tbGray" ID="txtDuDay0" 
            runat="server" Width="30px" TabIndex="99" Font-Size="8pt" MaxLength="3"></asp:TextBox>
                </FooterTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblDuDay" runat="server" Text='<%#Eval("du_day") %>' 
                        Width="30px"></asp:Label>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="30px"></ItemStyle>
            </asp:TemplateField>
        </Columns>
        <HeaderStyle HorizontalAlign="Center" BackColor="LightGray" Font-Size="8pt" 
            Font-Bold="True" Font-Names="Arial" ForeColor="Black"></HeaderStyle>
        <PagerStyle CssClass="pgr"></PagerStyle>
    </asp:GridView>
</td>
<td style="width:1%;">&nbsp;</td>
</tr>
<tr>
<td style="width:1%;">&nbsp;</td>
<td style="width:98%; " align="center">

    &nbsp;</td>
<td style="width:1%;">&nbsp;</td>
</tr></table>
</div>
</asp:Content>

