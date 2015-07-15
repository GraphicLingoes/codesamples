<%@ Page Language="C#" MasterPageFile="~/portal.master" AutoEventWireup="true" CodeFile="verify.aspx.cs" Inherits="verify" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="bodyClass" Runat="Server">
<body>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<!-- Page Wrapper -->
<div id="pageWrapper">
    <!-- Page -->
    <div id="page">
    <asp:ScriptManager ID="updateUserSriptMgr" runat="server" />
<asp:Label ID="dbErrorMessage" runat=server />
<div class="searchNotice"><b>First Login:</b> Please take a moment to verify your information. Press <b>Update My Profile</b> to continue.</div>
<!-- Start Reg Box Left -->
<div class="regBox">
    Practice Name:<br />
    <div class="readOnlyTB"><asp:Label ID="practiceName" runat="server" /></div><br />
    First Name:<br />
    <div class="readOnlyTB"><asp:Label ID="firstName" runat="server" /></div><br />
    Last Name:<br />
    <div class="readOnlyTB"><asp:Label ID="lastName" runat="server" /></div><br />
    Email:<br />
    <div class="readOnlyTB"><asp:Label ID="email" runat="server" /></div><br />
</div>
<!-- End Reg Box Left -->
<!-- Start Reg Box Right -->       	
        	<div class="regBox2">
        	Contact Phone#:<br />
        	<asp:TextBox ID="phone" runat="server" />
        	<asp:RegularExpressionValidator ID="reContactPh" runat="server"
        	                        ControlToValidate="phone"
        	                        ValidationExpression="^\s*[a-zA-Z0-9,\s,?,\,,',;,:,!,@,.,#'\-,&]+\s*$"
        	                        ErrorMessage="You have entered characters that are not allowed."
        	                        Display="None" ValidationGroup="Register" />
        	                      <cc1:ValidatorCalloutExtender ID="coReContactPh" 
                                    runat="server" Enabled="True" TargetControlID="reContactPh"
                                    HighlightCssClass="fieldError">
                                  </cc1:ValidatorCalloutExtender>
        	<asp:RequiredFieldValidator ID="phoneReg" runat="server"
        	    ControlToValidate="phone"
        	    ErrorMessage="A contact phone # is required."
        	    SetFocusOnError="true" Display="None" ValidationGroup="Register" />
                <cc1:ValidatorCalloutExtender ID="phoneReg_ValidatorCalloutExtender" 
                    runat="server" Enabled="True" TargetControlID="phoneReg"
                    HighlightCssClass="fieldError">
                </cc1:ValidatorCalloutExtender>
                <br />
                Your Title:<br />
                <asp:TextBox ID="title" runat="server" />
                <asp:RegularExpressionValidator ID="reTitle" runat="server"
        	                        ControlToValidate="title"
        	                        ValidationExpression="^\s*[a-zA-Z0-9,\s,?,\,,',;,:,!,@,.,#'\-,&]+\s*$"
        	                        ErrorMessage="You have entered characters that are not allowed."
        	                        Display="None" ValidationGroup="Register" />
        	                      <cc1:ValidatorCalloutExtender ID="coReTitle" 
                                    runat="server" Enabled="True" TargetControlID="reTitle"
                                    HighlightCssClass="fieldError">
                                  </cc1:ValidatorCalloutExtender>
                <br />
        	Address 1:<br />
        	<asp:TextBox ID="address1" runat="server" />
        	 <asp:RegularExpressionValidator ID="reAddress1" runat="server"
        	                        ControlToValidate="address1"
        	                        ValidationExpression="^\s*[a-zA-Z0-9,\s,?,\,,',;,:,!,@,.,#'\-,&]+\s*$"
        	                        ErrorMessage="You have entered characters that are not allowed."
        	                        Display="None" ValidationGroup="Register" />
        	                      <cc1:ValidatorCalloutExtender ID="coReAddress1" 
                                    runat="server" Enabled="True" TargetControlID="reAddress1"
                                    HighlightCssClass="fieldError">
                                  </cc1:ValidatorCalloutExtender>
        	<br />
        	Address 2:<br />
        	<asp:TextBox ID="address2" runat="server" />
        	<asp:RegularExpressionValidator ID="reAddress2" runat="server"
        	                        ControlToValidate="address2"
        	                        ValidationExpression="^\s*[a-zA-Z0-9,\s,?,\,,',;,:,!,@,.,#'\-,&]+\s*$"
        	                        ErrorMessage="You have entered characters that are not allowed."
        	                        Display="None" ValidationGroup="Register" />
        	                      <cc1:ValidatorCalloutExtender ID="coReAddress2" 
                                    runat="server" Enabled="True" TargetControlID="reAddress2"
                                    HighlightCssClass="fieldError">
                                  </cc1:ValidatorCalloutExtender>
        	<br />
        	City:<br />
        	<asp:TextBox ID="city" runat="server" />
        	<asp:RegularExpressionValidator ID="reCity" runat="server"
        	                        ControlToValidate="city"
        	                        ValidationExpression="^\s*[a-zA-Z0-9,\s,?,\,,',;,:,!,@,.,#'\-,&]+\s*$"
        	                        ErrorMessage="You have entered characters that are not allowed."
        	                        Display="None" ValidationGroup="Register" />
        	                      <cc1:ValidatorCalloutExtender ID="coReCity" 
                                    runat="server" Enabled="True" TargetControlID="reCity"
                                    HighlightCssClass="fieldError">
                                  </cc1:ValidatorCalloutExtender>
        	<br />
        	State: <br />
        	 <asp:DropDownList ID="statesList" DataSelectedValue="state" DataValueField = "abbr" DataTextField = "name" runat="server" CssClass="dropdownmenu" AppendDataBoundItems="True">
        	 </asp:DropDownList>
        <br />
        	Zip Code:<br />
        	<asp:TextBox ID="zipCode" runat="server" />
        	<asp:RegularExpressionValidator ID="reZipCode" runat="server"
        	                        ControlToValidate="zipCode"
        	                        ValidationExpression="^\s*[a-zA-Z0-9,\s,?,\,,',;,:,!,@,.,#'\-,&]+\s*$"
        	                        ErrorMessage="You have entered characters that are not allowed."
        	                        Display="None" ValidationGroup="Register" />
        	                      <cc1:ValidatorCalloutExtender ID="coReZipCode" 
                                    runat="server" Enabled="True" TargetControlID="reZipCode"
                                    HighlightCssClass="fieldError">
                                  </cc1:ValidatorCalloutExtender>
        	<br /><br />
                 <asp:Button ID="registerButton" runat="server" Text="Update My Profile" CssClass="formBtn" OnClick="register" ValidationGroup="Register" />       	
        	</div>
        	<div style="clear:both;">&nbsp;</div>
    <!-- /Page -->
    </div>
<!-- / Page Wrapper -->
</div>
</asp:Content>

