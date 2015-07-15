<%@ Page Language="C#" MasterPageFile="~/admin/admin.master" AutoEventWireup="true" CodeFile="videos.aspx.cs" Inherits="admin_videos" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="bodyClass" Runat="Server">
<body id="admin">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<asp:ScriptManager ID="videoScriptManager" runat="server" /> 
<!-- Page wrapper -->
<div id="pageWrapper">
    <!-- Page -->
    <div id="page">
<div class="adminMainContainer">
<!-- Secondary Menu -->
<!-- Start Add New Video Items -->
                <div style="float: left;">
                <h3>Add New</h3>
                <asp:DropDownList ID="ddlNewVideo" runat="server">
                    <asp:ListItem Selected="True" Text="--Choose Item--" Value="null" />
                    <asp:ListItem Value="newVideo" Text="Video" />
                    <asp:ListItem Value="newCategory" Text="Video Category" />
                    <asp:ListItem Value="newEdition" Text="LMD Edition" />
                    <asp:ListItem Value="newAuthor" Text="Video Author" />
                </asp:DropDownList>&nbsp;&nbsp;<asp:Button ID="btnNewVideo" runat="server" OnClick="selectNewVideo" Text="GO" /><br />
</div>
    <div style="float: left; margin-left: 21px; margin-bottom: 21px;">
    <h3>Manage List</h3>
                <asp:DropDownList ID="ddlManageVideo" runat="server" 
            style="margin-bottom: 0px">
                    <asp:ListItem Selected="True" Text="--Choose Item--" Value="null" />
                    <asp:ListItem Value="manageVideo" Text="Video List" />
                    <asp:ListItem Value="manageCategory" Text="Category List" />
                    <asp:ListItem Value="manageEdition" Text="Edition List" />
                    <asp:ListItem Value="manageAuthor" Text="Author List" />
                </asp:DropDownList>&nbsp;&nbsp;<asp:Button ID="btnManageVideo" runat="server" OnClick="selectManageVideo" Text="GO" /><br />
    </div>
    <div style="clear:both; height: 1px;">&nbsp;</div>
<!-- Add New Video-->
               <div id="divNewVideo" runat="server" style="display:none;" >
                <asp:Label ID="newEdSuccess" runat="server" style="display:none;" />
                <asp:Label ID="newEdError" runat="server" style="display:none;" />
                    <div class="contentBox2">
                    Video Name:<br />
                    <asp:TextBox ID="vidName" runat="server" />
                        <asp:RequiredFieldValidator ID="reqVidName" runat="server"
        	                ControlToValidate="vidName"
        	                ErrorMessage="Please enter a video Name."
        	                SetFocusOnError="true" Display="None" ValidationGroup="newVideo" />
        	            <cc1:ValidatorCalloutExtender ID="callOutreqVidName" 
                            runat="server" Enabled="True" TargetControlID="reqVidName"
                            HighlightCssClass="fieldError">
                        </cc1:ValidatorCalloutExtender>
        	            <asp:RegularExpressionValidator ID="RegExpVidName" runat="server"
        	                ControlToValidate="vidName"
        	                ValidationExpression="^\s*[a-zA-Z0-9,\s,?,\,,',;,:,(,),_,!,@,.,#,\-,&]+\s*$"
        	                ErrorMessage="You have entered characters that are not allowed."
        	                Display="None" ValidationGroup="newVideo" />
        	            <cc1:ValidatorCalloutExtender ID="callOutreqVidName2" 
                            runat="server" Enabled="True" TargetControlID="RegExpVidName"
                            HighlightCssClass="fieldError">
                        </cc1:ValidatorCalloutExtender>
                        <br />
                        Video Length:<br />
                    <asp:TextBox ID="vidLength" runat="server" />
                        <asp:RequiredFieldValidator ID="reqFieldVidLenght" runat="server"
        	                ControlToValidate="vidLength"
        	                ErrorMessage="Please enter a video lenght."
        	                SetFocusOnError="true" Display="None" ValidationGroup="newVideo" />
        	            <cc1:ValidatorCalloutExtender ID="callOutVidLenght" 
                            runat="server" Enabled="True" TargetControlID="reqFieldVidLenght"
                            HighlightCssClass="fieldError">
                        </cc1:ValidatorCalloutExtender>
        	            <asp:RegularExpressionValidator ID="regExpVidLenght" runat="server"
        	                ControlToValidate="vidLength"
        	                ValidationExpression="^\s*[a-zA-Z0-9,\s,?,\,,',;,:,(,),_,!,@,.,#,\-,&]+\s*$"
        	                ErrorMessage="You have entered characters that are not allowed."
        	                Display="None" ValidationGroup="newVideo" />
        	            <cc1:ValidatorCalloutExtender ID="callOutRegExpLenght" 
                            runat="server" Enabled="True" TargetControlID="regExpVidLenght"
                            HighlightCssClass="fieldError">
                        </cc1:ValidatorCalloutExtender>
                        <br /><br />
                        Video Box Width:<br />
                    <asp:TextBox ID="vidWidth" runat="server" />
                        <asp:RequiredFieldValidator ID="reqVidWidth" runat="server"
        	                ControlToValidate="vidWidth"
        	                ErrorMessage="Please enter a video box width. If you are not sure enter 640."
        	                SetFocusOnError="true" Display="None" ValidationGroup="newVideo" />
        	            <cc1:ValidatorCalloutExtender ID="callOutVidWidth" 
                            runat="server" Enabled="True" TargetControlID="reqVidWidth"
                            HighlightCssClass="fieldError">
                        </cc1:ValidatorCalloutExtender>
        	            <asp:RegularExpressionValidator ID="regExpVidWidth" runat="server"
        	                ControlToValidate="vidWidth"
        	                ValidationExpression="^\s*[a-zA-Z0-9,\s,?,\,,',;,:,(,),_,!,@,.,#,\-,&]+\s*$"
        	                ErrorMessage="You have entered characters that are not allowed."
        	                Display="None" ValidationGroup="newVideo" />
        	            <cc1:ValidatorCalloutExtender ID="callOutVidWidthReg" 
                            runat="server" Enabled="True" TargetControlID="regExpVidWidth"
                            HighlightCssClass="fieldError">
                        </cc1:ValidatorCalloutExtender><br /><br />
                        Video Box Height:<br />
                    <asp:TextBox ID="vidHeight" runat="server" />
                        <asp:RequiredFieldValidator ID="reqFieldVidHeight" runat="server"
        	                ControlToValidate="vidHeight"
        	                ErrorMessage="Please enter a video box height. If you are not sure enter 498."
        	                SetFocusOnError="true" Display="None" ValidationGroup="newVideo" />
        	            <cc1:ValidatorCalloutExtender ID="callOutReqVidHeight" 
                            runat="server" Enabled="True" TargetControlID="reqFieldVidHeight"
                            HighlightCssClass="fieldError">
                        </cc1:ValidatorCalloutExtender>
        	            <asp:RegularExpressionValidator ID="regExpVidHeight" runat="server"
        	                ControlToValidate="vidHeight"
        	                ValidationExpression="^\s*[a-zA-Z0-9,\s,?,\,,',;,:,(,),_,!,@,.,#,\-,&]+\s*$"
        	                ErrorMessage="You have entered characters that are not allowed."
        	                Display="None" ValidationGroup="newVideo" />
        	            <cc1:ValidatorCalloutExtender ID="callOutRegExpVidHeight" 
                            runat="server" Enabled="True" TargetControlID="regExpVidHeight"
                            HighlightCssClass="fieldError">
                        </cc1:ValidatorCalloutExtender><br /><br />
                        Video Author:<br />
                        <asp:DropDownList ID="ddlVidAuthor" runat="server" CssClass="dropdownmenu"/>
                         <br /><br />
                        Application Edition:<br />
                        <asp:CheckBoxList ID="cbVidEd" runat="server" />
                        <br />              
                    </div>
                    <div class="contentBox2">
                        ScreenCast Video Number:<br />
                    <asp:TextBox ID="vidLink" runat="server" />
                        <asp:RequiredFieldValidator ID="reqFieldVidLink" runat="server"
        	                ControlToValidate="vidLink"
        	                ErrorMessage="Please enter a video link."
        	                SetFocusOnError="true" Display="None" ValidationGroup="newVideo" />
        	            <cc1:ValidatorCalloutExtender ID="callOutVidLink" 
                            runat="server" Enabled="True" TargetControlID="reqFieldVidLink"
                            HighlightCssClass="fieldError">
                        </cc1:ValidatorCalloutExtender>
        	            <asp:RegularExpressionValidator ID="regExpVidLink" runat="server"
        	                ControlToValidate="vidLink"
        	                ValidationExpression="^\s*[a-zA-Z0-9,\s,?,\,,',;,:,(,),_,!,@,.,#,\-,&]+\s*$"
        	                ErrorMessage="You have entered characters that are not allowed."
        	                Display="None" ValidationGroup="newVideo" />
        	            <cc1:ValidatorCalloutExtender ID="callOutRegExpVidLink" 
                            runat="server" Enabled="True" TargetControlID="regExpVidLink"
                            HighlightCssClass="fieldError">
                        </cc1:ValidatorCalloutExtender>
                        <br />
                            ScreenCast Video File: <br />
                    <asp:TextBox ID="vidFileName" runat="server" />
                        <asp:RequiredFieldValidator ID="regFieldVidFile" runat="server"
        	                ControlToValidate="vidFileName"
        	                ErrorMessage="Please enter a video file name."
        	                SetFocusOnError="true" Display="None" ValidationGroup="newVideo" />
        	            <cc1:ValidatorCalloutExtender ID="callOutVidFile" 
                            runat="server" Enabled="True" TargetControlID="regFieldVidFile"
                            HighlightCssClass="fieldError">
                        </cc1:ValidatorCalloutExtender>
        	            <asp:RegularExpressionValidator ID="regExpVidFile" runat="server"
        	                ControlToValidate="vidFileName"
        	                ValidationExpression="^\s*[a-zA-Z0-9,\s,?,\,,',;,:,(,),_,!,@,.,#,\-,&]+\s*$"
        	                ErrorMessage="You have entered characters that are not allowed."
        	                Display="None" ValidationGroup="newVideo" />
        	            <cc1:ValidatorCalloutExtender ID="callOutRegExpVidFile" 
                            runat="server" Enabled="True" TargetControlID="regExpVidFile"
                            HighlightCssClass="fieldError">
                        </cc1:ValidatorCalloutExtender>
                        <br />
                              Search Key Words<br />
                    <asp:TextBox ID="vidKeywords" runat="server" />
                        <asp:RequiredFieldValidator ID="reqFieldVidKeywords" runat="server"
        	                ControlToValidate="vidKeywords"
        	                ErrorMessage="Please enter a video keywords."
        	                SetFocusOnError="true" Display="None" ValidationGroup="newVideo" />
        	            <cc1:ValidatorCalloutExtender ID="callOutVidKeywords" 
                            runat="server" Enabled="True" TargetControlID="reqFieldVidKeywords"
                            HighlightCssClass="fieldError">
                        </cc1:ValidatorCalloutExtender>
        	            <asp:RegularExpressionValidator ID="regExpVidKeyWords" runat="server"
        	                ControlToValidate="vidKeywords"
        	                ValidationExpression="^\s*[a-zA-Z0-9,\s,?,\,,',;,:,(,),_,!,@,.,#,\-,&]+\s*$"
        	                ErrorMessage="You have entered characters that are not allowed."
        	                Display="None" ValidationGroup="newVideo" />
        	            <cc1:ValidatorCalloutExtender ID="callOutRegExpVidKeywords" 
                            runat="server" Enabled="True" TargetControlID="regExpVidKeyWords"
                            HighlightCssClass="fieldError">
                        </cc1:ValidatorCalloutExtender>
                        <br />
                        Video Description:<br />
                    <asp:TextBox ID="videoDescription" runat="server" Rows="7" TextMode="MultiLine" style="width: 200px;" />
                        <asp:RequiredFieldValidator ID="reqVidDesc" runat="server"
        	                ControlToValidate="videoDescription"
        	                ErrorMessage="Please enter a video description."
        	                SetFocusOnError="true" Display="None" ValidationGroup="newVideo" />
        	            <cc1:ValidatorCalloutExtender ID="callOutVidDesc" 
                            runat="server" Enabled="True" TargetControlID="reqVidDesc"
                            HighlightCssClass="fieldError">
                        </cc1:ValidatorCalloutExtender>
        	            <asp:RegularExpressionValidator ID="regExpVidDesc" runat="server"
        	                ControlToValidate="videoDescription"
        	                ValidationExpression="^\s*[a-zA-Z0-9,\s,?,\,,',;,:,(,),_,!,@,.,#,\-,&]+\s*$"
        	                ErrorMessage="You have entered characters that are not allowed."
        	                Display="None" ValidationGroup="newVideo" />
        	            <cc1:ValidatorCalloutExtender ID="callOutRegExpVidDesc" 
                            runat="server" Enabled="True" TargetControlID="regExpVidDesc"
                            HighlightCssClass="fieldError">
                        </cc1:ValidatorCalloutExtender>
                        <br />
                        </div>
                        <div class="contentBox2">
                        <br />
                         Video Category:<br />
                        <asp:DropDownList ID="ddlVidCat" runat="server" CssClass="dropdownmenu" /><br /><br />   
                        Associate Implementation Steps<br />(Hold down Ctrl to select mulitple Steps)<br /><br />
                        <asp:ListBox id="newVidImpSteps" SelectionMode="Multiple" runat="server" Rows="12" CssClass="dropdownmenu"></asp:ListBox>
                        <br /><br />
                        <asp:Button ID="btnAddNewVideo" runat="server" Text="Add New" OnClick="newVideo" ValidationGroup="newVideo" CssClass="formBtn" /> <asp:Button ID="btnCancelNewVideo" runat="server" Text="Cancel" OnClick="cancelNewVideo" CssClass="formBtn" />
            <asp:Label ID="testImp" runat="server" />
            </div>
             </div><div style="clear:both;">&nbsp;</div>
<!-- New Category -->
             <div id="divNewCategory" runat="server" style="display:none;">
            <br />
                <asp:Label ID="newCatSuccess" runat="server" style="display:none" />
                <asp:Label ID="newCatError" runat="server" style="display:none" />
                <asp:Label ID="newCatHdr" runat="server" Text="Video Category Name" /><br />
                <asp:TextBox ID="newCatName" runat="server" />
                <asp:RequiredFieldValidator ID="newCatNameReq" runat="server"
        	    ControlToValidate="newCatName"
        	    ErrorMessage="Please enter a category."
        	    SetFocusOnError="true" Display="None" ValidationGroup="newCategory" />
        	<cc1:ValidatorCalloutExtender ID="myTextBoxReq_ValidatorCalloutExtender" 
                runat="server" Enabled="True" TargetControlID="newCatNameReq"
                HighlightCssClass="fieldError">
            </cc1:ValidatorCalloutExtender>
        	<asp:RegularExpressionValidator ID="regExpressionmynewCatName" runat="server"
        	    ControlToValidate="newCatName"
        	    ValidationExpression="^\s*[a-zA-Z0-9,\s,?,\,,',;,:,(,),_,!,@,.,#,\-,&]+\s*$"
        	    ErrorMessage="You have entered characters that are not allowed."
        	    Display="None" ValidationGroup="newCategory" />
        	<cc1:ValidatorCalloutExtender ID="newCategory_ValidatorCalloutExtender" 
                runat="server" Enabled="True" TargetControlID="regExpressionmynewCatName"
                HighlightCssClass="fieldError">
            </cc1:ValidatorCalloutExtender><br /><br />
            Screencast Folder Name:<br />
            <asp:TextBox ID="newScFolder" runat="server" />
        	<asp:RegularExpressionValidator ID="regExpressionmynewScFolder" runat="server"
        	    ControlToValidate="newScFolder"
        	    ValidationExpression="^\s*[a-zA-Z0-9,\s,?,\,,',;,:,(,),_,!,@,.,#,\-,&]+\s*$"
        	    ErrorMessage="You have entered characters that are not allowed."
        	    Display="None" ValidationGroup="newCategory" />
        	<cc1:ValidatorCalloutExtender ID="callout_ScFolder" 
                runat="server" Enabled="True" TargetControlID="regExpressionmynewScFolder"
                HighlightCssClass="fieldError">
            </cc1:ValidatorCalloutExtender>
                <br />
                <asp:Button ID="btnNewCategory" runat="server" OnClick="newCategory" Text="Add New" ValidationGroup="newCategory" /> <asp:Button ID="btnCancelNewCat" runat="server" Text="Cancel" OnClick="cancelNewCat" CssClass="formBtn" />
             </div>
<!-- End Add New Category -->
<!-- Start Add New Edition -->
             <div id="divNewEdition" runat="server" style="display:none;">
             Video Edition Name:<br />
             <asp:TextBox ID="newEdName" runat="server" />
                <asp:RequiredFieldValidator ID="reqFieldNewEd" runat="server"
        	        ControlToValidate="newEdName"
        	        ErrorMessage="Please enter edition name."
        	        SetFocusOnError="true" Display="None" ValidationGroup="newEdition" />
        	    <cc1:ValidatorCalloutExtender ID="valitatorCalloutNewEdName1" 
                    runat="server" Enabled="True" TargetControlID="reqFieldNewEd"
                    HighlightCssClass="fieldError">
                </cc1:ValidatorCalloutExtender>
        	    <asp:RegularExpressionValidator ID="regExpNewEdName" runat="server"
        	        ControlToValidate="newEdName"
        	        ValidationExpression="^\s*[a-zA-Z0-9,\s,?,\,,',;,:,!,@,#,.,\-,&]+\s*$"
        	        ErrorMessage="You have entered characters that are not allowed."
        	        Display="None" ValidationGroup="newEdition" />
        	    <cc1:ValidatorCalloutExtender ID="valitatorCalloutNewEdName2" 
                    runat="server" Enabled="True" TargetControlID="regExpNewEdName"
                    HighlightCssClass="fieldError">
                </cc1:ValidatorCalloutExtender>
             <br />
             Video Edition Logo:<br />
             <asp:FileUpload ID="fileUploadEd" runat="server"/><br />
                <br />
                <asp:Label ID="editionUploadLbl" runat="server"></asp:Label>
                <asp:Button ID="btnNewEdition" runat="server" OnClick="newEdition" Text="Add New" ValidationGroup="newEdition" /> <asp:Button ID="btnCancelEdUpload" runat="server" Text="Cancel" OnClick="cancelEdUpload" CssClass="formBtn" />
             </div>
<!-- End Add New Edition -->
<!-- Start new Video Author -->
             <div id="divNewAuthor" runat="server" style="display:none;">
              Video Author Name:<br />
             <asp:TextBox ID="newAuthorName" runat="server" />
                <asp:RequiredFieldValidator ID="reqNewAuthor" runat="server"
        	        ControlToValidate="newAuthorName"
        	        ErrorMessage="Please enter new author's name."
        	        SetFocusOnError="true" Display="None" ValidationGroup="newAuthor" />
        	    <cc1:ValidatorCalloutExtender ID="newAuthorCallOut" 
                    runat="server" Enabled="True" TargetControlID="reqNewAuthor"
                    HighlightCssClass="fieldError">
                </cc1:ValidatorCalloutExtender>
        	    <asp:RegularExpressionValidator ID="regExpNewAuthor" runat="server"
        	        ControlToValidate="newAuthorName"
        	        ValidationExpression="^\s*[a-zA-Z0-9,\s,?,\,,',;,:,(,),_,!,@,.,#,\-,&]+\s*$"
        	        ErrorMessage="You have entered characters that are not allowed."
        	        Display="None" ValidationGroup="newAuthor" />
        	    <cc1:ValidatorCalloutExtender ID="regNewAuthorCallOut" 
                    runat="server" Enabled="True" TargetControlID="regExpNewAuthor"
                    HighlightCssClass="fieldError">
                </cc1:ValidatorCalloutExtender>
             <br /><br />
               Video Author Title:<br />
             <asp:TextBox ID="newAuthorTitle" runat="server" />
        	    <asp:RegularExpressionValidator ID="expressValAuthTitle" runat="server"
        	        ControlToValidate="newAuthorTitle"
        	        ValidationExpression="^\s*[a-zA-Z0-9,\s,?,\,,',;,:,(,),_,!,@,.,#,\-,&]+\s*$"
        	        ErrorMessage="You have entered characters that are not allowed."
        	        Display="None" ValidationGroup="newAuthor" />
        	    <cc1:ValidatorCalloutExtender ID="callOutAuthTitle" 
                    runat="server" Enabled="True" TargetControlID="expressValAuthTitle"
                    HighlightCssClass="fieldError">
                </cc1:ValidatorCalloutExtender>
             <br /><br />
             Upload Author Picture:<br />
             <asp:FileUpload ID="fileUploadAuthor" runat="server"/><br />
                <br />
                <asp:Label ID="newAuthorLbl" runat="server"></asp:Label>
                <asp:Button ID="btnNewAuthor" runat="server" OnClick="newAuthor" Text="Add New" ValidationGroup="newAuthor" /> <asp:Button ID="btnCancelAuth" runat="server" Text="Cancel" OnClick="cancelNewAuth" CssClass="formBtn" />
             </div>
<!-- End new Video Author -->
</div>
    <!-- /Page -->
    </div>
<!-- /Page wrapper -->
</div>
</asp:Content>

