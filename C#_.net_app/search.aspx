<%@ Page Language="C#" MasterPageFile="~/portal.master" AutoEventWireup="true" CodeFile="search.aspx.cs" Inherits="search" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="bodyClass" Runat="server"><body ID="search"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <!-- Start Mini Ribbon Secondary Menu -->
    <div id="miniRibbon">
        <div id="miniRibbonIcons">
            <div id="miniRibbonAdjust">
            <asp:ScriptManager ID="searchSriptManager" runat="server" />
    <asp:TextBox ID="videoSearchText" runat="server" />
                                             <asp:RegularExpressionValidator ID="RegExpVidSearch" runat="server"
        	                                                        ControlToValidate="videoSearchText"
        	                                                        ValidationExpression="^\s*[a-zA-Z0-9,\s,?,\,,',;,:,(,),!,@,.,*,#,\-,&]+\s*$"
        	                                                        ErrorMessage="You have entered characters that are not allowed."
        	                                                        Display="None" ValidationGroup="vidSearch" />
        	                                                    <cc1:ValidatorCalloutExtender ID="callOutRegExpVidSearch" 
                                                                    runat="server" Enabled="True" TargetControlID="RegExpVidSearch"
                                                                    HighlightCssClass="fieldError">
                                                                </cc1:ValidatorCalloutExtender>&nbsp;&nbsp;
    &nbsp;&nbsp;<asp:DropDownList ID="videoSearchBy" runat="server" CssClass="dropdownmenu">
                                                <asp:ListItem Text="Video Description" value="videoInfo.[description]" />
                                                <asp:ListItem Text="Video Name" value="videoInfo.videoName" />
                                                <asp:ListItem Text="Video Category" value="videoCategory.categoryName" />
                                                <asp:ListItem Text="Video Key Words" value="videoInfo.keyWords" />
                                                <asp:ListItem Text="LeonardoMD Edition" value="videoEdition.edition" />
                                                </asp:DropDownList>
                                                &nbsp;&nbsp;<asp:Button ID="videoSearchBtn" runat="server" OnClick="videoSearch" Text="Search" ValidationGroup="vidSearch" />
            </div>
        </div>
    </div>
    <!-- / Mini Ribbon Secondary Menu -->
    </div>
    <!-- Main Wrapper 100% -->
    <div id="pageWrapper">
    <asp:Label ID="dbErrorMessage" runat="server" />
        <!-- Page -->
        <div id="page">
         <!-- Search box notice -->
        <asp:Label ID="searchError" runat="server" />
        <!-- Search Result Repeatrer -->
        <asp:Repeater ID="searchRepeater" runat="server">
            <HeaderTemplate><h2>Video Search Results</h2></HeaderTemplate>
                <ItemTemplate>
                <div class="allVidsOff">
                <table cellpadding="9" cellspacing="0">
                <tr>
                    <td><asp:ImageButton ID="videoImageBtn" runat="server" ImageUrl="Images/playVideo.jpg" CommandName="videoRedirect" CommandArgument='<%# Eval("videoInfoID") %>' CssClass="formBtn"  /></td>
                    <td><b><%# Eval("videoName") %> <%# Eval("lenght") %></b><br /><br />
                <%# Eval("description") %><br /><br />
                </td>
                </tr>
                </table>
                </div>
                </ItemTemplate>
                <AlternatingItemTemplate>
                <div class="allVidsOn">
                <table cellpadding="9" cellspacing="0">
                <tr>
                    <td><asp:ImageButton ID="videoImageBtn" runat="server" ImageUrl="Images/playVideo.jpg" CommandName="videoRedirect" CommandArgument='<%# Eval("videoInfoID") %>' CssClass="formBtn" /></td>
                    <td><b><%# Eval("videoName") %> <%# Eval("lenght") %></b><br /><br />
                <%# Eval("description") %><br /><br />
                </td>
                </tr>
                </table>
                </div>
                </AlternatingItemTemplate>
        </asp:Repeater>
        
        <!-- End Search Result Repeater -->
        <!-- / Page -->
        </div>
   <!-- / Main Wrapper -->
   </div>
</asp:Content>

