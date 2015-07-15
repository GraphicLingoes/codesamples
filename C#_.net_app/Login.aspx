<%@ Page Title="LeonardoMD Training Portal - Login Page" Language="C#" MasterPageFile="~/outterPages.master" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Login" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<!-- Start signin div -->
<asp:ScriptManager ID="signInScriptManager" runat="server" />
<div style="margin:0px auto 0px auto; width:400px;"><asp:Label ID="loginResponse" runat="server" /></div>
<div id="LoginBox">
            <div style="margin-right: 50px; text-align:right;" ><h2>Sign In</h2></div>
             Practice ID<br />
        	<asp:TextBox ID="practiceID" runat="server" style="border-color: #cccccc;" />
        	<asp:RequiredFieldValidator ID="practiceIDreq" runat="server"
        	    ControlToValidate="practiceID"
        	    ErrorMessage="Pracitce ID required to login."
        	    SetFocusOnError="true" Display="None" ValidationGroup="Login" />
        	<cc1:ValidatorCalloutExtender ID="practiceIDCallOut" 
                runat="server" Enabled="True" TargetControlID="practiceIDreq"
                HighlightCssClass="fieldError">
            </cc1:ValidatorCalloutExtender>
             <asp:RegularExpressionValidator ID="regExpPracticeId" runat="server"
        	                        ControlToValidate="practiceID"
        	                        ValidationExpression="^\s*[a-zA-Z0-9,\s,?,\,,',;,:,!,@,.,#'\-,&]+\s*$"
        	                        ErrorMessage="You have entered characters that are not allowed."
        	                        Display="None" ValidationGroup="Login" />
        	                      <cc1:ValidatorCalloutExtender ID="callOutRegExpPracitceID" 
                                    runat="server" Enabled="True" TargetControlID="regExpPracticeId"
                                    HighlightCssClass="fieldError">
                                  </cc1:ValidatorCalloutExtender>
        	<br />
        	Practice Password:<br />
        	<asp:TextBox ID="practicePassword" runat="server" TextMode="Password" style="border-color: #cccccc;" /><br />
             Member Email<br />
        	<asp:TextBox ID="emailLogin" runat="server" style="border-color: #cccccc;" />
        	<asp:RequiredFieldValidator ID="emailReg" runat="server"
        	    ControlToValidate="emailLogin"
        	    ErrorMessage="Email address is required to login."
        	    SetFocusOnError="true" Display="None" ValidationGroup="Login" />
        	
        	<cc1:ValidatorCalloutExtender ID="emailReg_ValidatorCalloutExtender" 
                runat="server" Enabled="True" TargetControlID="emailReg"
                HighlightCssClass="fieldError">
            </cc1:ValidatorCalloutExtender>
        	<asp:RegularExpressionValidator ID="emailValidator" runat="server"
        	    ControlToValidate="emailLogin"
        	    ValidationExpression="^\S+@\S+\.\S+$"
        	    ErrorMessage="You must enter a valid email address to login."
        	    Display="None" ValidationGroup="Login" />
        	<cc1:ValidatorCalloutExtender ID="emailValidator_ValidatorCalloutExtender" 
                runat="server" Enabled="True" TargetControlID="emailValidator"
                HighlightCssClass="fieldError">
            </cc1:ValidatorCalloutExtender>
        	<br />
        	Member Password<br />
        	<asp:TextBox ID="password" runat="server" TextMode="Password" style="border-color: #cccccc;" />
        	<asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server"
        	    ControlToValidate="password"
        	    ErrorMessage="Password is required to login."
        	    SetFocusOnError="true" Display="None" ValidationGroup="Login" />
            <cc1:ValidatorCalloutExtender ID="RequiredFieldValidator1_ValidatorCalloutExtender" 
                runat="server" Enabled="True" TargetControlID="RequiredFieldValidator1"
                HighlightCssClass="fieldError">
            </cc1:ValidatorCalloutExtender>
            <asp:RegularExpressionValidator ID="regExpPassword" runat="server"
        	                        ControlToValidate="password"
        	                        ValidationExpression="^\s*[a-zA-Z0-9,\s,?,\,,',;,:,!,@,.,#'\-,&]+\s*$"
        	                        ErrorMessage="You have entered characters that are not allowed."
        	                        Display="None" ValidationGroup="Login" />
        	                      <cc1:ValidatorCalloutExtender ID="callOutPassword" 
                                    runat="server" Enabled="True" TargetControlID="regExpPassword"
                                    HighlightCssClass="fieldError">
                                  </cc1:ValidatorCalloutExtender>
            <br />
            <div style="margin-top: 3px; margin-bottom: 5px;"><asp:CheckBox ID="signInRemember" runat="server" />&nbsp;Remember Me
            </div>
            <a href="https://renaissance.leonardomd.com/members/resetpassword.asp" target="_blank">Forgot My Password</a>
            <div style="margin-top: 7px; margin-right: 32px; text-align: right;" >
            <asp:Button ID="submitButton" runat="server" Text="Sign In" CssClass="formBtn" OnClick="LoginUser" ValidationGroup="Login" /></div>
            
        	
<!-- End signin div -->
</div> 
</asp:Content>

