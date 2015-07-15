<%@ Page Language="C#" MasterPageFile="help.master" AutoEventWireup="true" CodeFile="help.aspx.cs" Inherits="help_helpAP" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="bodyClass" Runat="Server">
    <body>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <!-- Start Mini Ribbon Secondary Menu -->
    <div id="miniRibbonHelp">
        <div id="miniRibbonIcons">
            <div id="miniRibbonAdjust">
            <asp:ScriptManager ID="searchSriptManager" runat="server" />
    <asp:TextBox ID="helpSearchText" runat="server" />
                                              <asp:RequiredFieldValidator ID="rfvSearch" runat="server"
        	                                        ControlToValidate="helpSearchText"
        	                                        ErrorMessage="Please enter search criteria."
        	                                        SetFocusOnError="true" Display="None" ValidationGroup="helpSearch" />
        	                                    <atk:ValidatorCalloutExtender ID="vceSearch" 
                                                    runat="server" Enabled="True" TargetControlID="rfvSearch"
                                                    HighlightCssClass="fieldError">
                                                </atk:ValidatorCalloutExtender>
                                             <asp:RegularExpressionValidator ID="RegExpVidSearch" runat="server"
        	                                                        ControlToValidate="helpSearchText"
        	                                                        ValidationExpression="^\s*[a-zA-Z0-9,\s,?,\,,',;,:,(,),!,@,.,*,#,\-,&]+\s*$"
        	                                                        ErrorMessage="You have entered characters that are not allowed."
        	                                                        Display="None" ValidationGroup="helpSearch" />
        	                                                    <atk:validatorcalloutextender ID="callOutRegExpVidSearch" 
                                                                    runat="server" 
                                                                    Enabled="True" TargetControlID="RegExpVidSearch"                       
                                                                    HighlightCssClass="fieldError">
                                                                </atk:validatorcalloutextender>&nbsp;&nbsp;
    &nbsp;&nbsp;<asp:DropDownList ID="helpSearchBy" runat="server" CssClass="dropdownmenu">
                                                <asp:ListItem Text="Keywords" value="helpPages.content" />
                                                <asp:ListItem Text="Help Page" value="helpPages.helpPageName" />
                                                </asp:DropDownList>
                                                &nbsp;&nbsp;<asp:Button ID="helpPageSearchBtn" runat="server" OnClick="videoSearch" Text="Search" ValidationGroup="helpSearch" />
            </div>
        </div>
            <!-- / Mini Ribbon Secondary Menu -->
    </div>
     <!-- Page wrapper -->
<div id="pageWrapperHelpGlossary">
        <!-- Page -->
    <div id="pageHelpGlossary">
    <asp:Label ID="labelError" runat="server" />
    <!-- Left Div -->
    <div id="pageHelpLeftDiv">
    <div id="helpTopicMenu">
    <ul>
    <asp:Repeater ID="repeaterHelpTopics" runat="server">
    <ItemTemplate>
    <li><a runat="server" ID="anchorHelpTopicID" href='<%# Bind("helpTopicID") %>' name="helpTopicLink"><asp:Label ID="labelHelpTopicTop" runat="server" Text='<%# Bind("name") %>' /></a>
    <ul>
    <asp:Label ID="labelViewEdit" runat="server" Text='<%# buildSubHelpTopics((int)Eval("helpTopicID")) %>'></asp:Label>
    </ul>
    </li>
    </ItemTemplate>
    </asp:Repeater>
    </ul>
    </div>
    <!--/Left Div -->
    </div>
    <!-- Right Div -->
    <div id="pageHelpRightDiv">
    <div id="loadedHelpPage"></div>
    <asp:Label ID="labelSearch" runat="server" />
    <asp:Panel ID="panelHelpPage" runat="server">
    <hr />
    <b>Search Results</b> - Page(s) that may contain helpful information.
    <hr />
    <ul>
    <asp:Label ID="labelDisplayHelpPage" runat="server" Text=""></asp:Label>
    </ul>
    <asp:Label ID="labelNotice" runat="server" />
    </asp:Panel>
    <!--/Right Div -->
    </div>
     <!--/Page -->
    </div>
    <!--/Page wrapper -->
    </div>

</asp:Content>

