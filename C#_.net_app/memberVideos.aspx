<%@ Page Language="C#" MasterPageFile="~/portal.master" AutoEventWireup="true" CodeFile="memberVideos.aspx.cs" Inherits="memberVideos" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<script type="text/javascript" src="../jquery/jquery-1.4.2.min.js"></script>
<script type="text/javascript" src="../jquery/hideBoxes.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="bodyClass" Runat="Server"><body ID="videos"></asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <!-- Start Mini Ribbon Secondary Menu -->
    <asp:ScriptManager ID="scriptManagerMemberVideos" runat="server" />
    <div id="miniRibbon">
        <div id="miniRibbonIcons">
           <div class="miniRibbonImg"><asp:ImageButton ID="categoryVidsIB" ImageUrl="Images/miniRibbon/collapse_32.png" runat="server" ToolTip="Show Videos by Category" onclick="showCategories" /></div>
            <div class="miniRibbonTxt"><asp:LinkButton ID="categoryVidsLB" runat="server" Text="Video Categories" onclick="showCategories" /></div>
            <div class="miniRibbonImg"><asp:ImageButton ID="allVidsIB" ImageUrl="Images/miniRibbon/expand_32.png" runat="server" ToolTip="Show All Videos" onclick="showAllVideos" /></div>
            <div class="miniRibbonTxt"><asp:LinkButton ID="allVidsLB" runat="server" Text="All Videos" onclick="showAllVideos" /></div>
            <div class="miniRibbonImg"><asp:ImageButton ID="backVidsIB" ImageUrl="Images/miniRibbon/back_32.png" runat="server" ToolTip="Back" onclick="showCategories" /></div>
            <div class="miniRibbonTxt"><asp:LinkButton ID="backVidsLB" runat="server" Text="Back" onclick="showCategories" /></div>
        </div>
    </div>
    <!-- / Mini Ribbon Secondary Menu -->
    </div>
    <!-- Main Wrapper 100% -->
    <div id="pageWrapper">
    <div id="page">
    <!-- Video category grid -->
     <asp:GridView ID="videoCatGrid" runat="server"
            AutoGenerateColumns="false" AllowSorting="True" AllowPaging="true" PageSize="50" 
            onPageIndexChanging = "videoCatGrid_PageIndexChanging"
            onsorting="videoCatGrid_Sorting"
            onSelectedIndexChanged = "videoCatGrid_SelectedIndexChanged">
            <Columns>
                <asp:ButtonField ButtonType="link" CommandName="Select" DataTextField="categoryName" HeaderText="Video Category Name" SortExpression="categoryName" />
            </Columns>
            <AlternatingRowStyle CssClass="alternateRow" />
       </asp:GridView> 
    <!-- /Video category grid -->
    <!-- Confirm video div -->
    <asp:Label id="test" runat="server" />
    <div id="confirmVideoDiv" runat="server">
    <asp:Label ID="errorMessage" runat="server" />
    <asp:Label ID="confirmMessage" runat="server" />
    <div align="center" style="margin-top: 15px;">
        <asp:Button ID="btnConfirmVid" runat="server" OnClick="confirmVideo" CssClass="formBtn" Text="Continue to Video" />&nbsp;
    <asp:Button ID="btnGoBackVid" runat="server" OnClick="goBackVid" CssClass="formBtn" Text="Cancel" /></div>
        <!-- / Confirm video div -->
        </div>
     <!-- Start Video Grid View -->
<!-- End Video Grid View -->
<!-- Start Selected Video Category Div -->
    <div class="adminMainContainer" style="padding:0px 9px 9px 9px;">
        <h1><asp:Label ID="categoryNameh2" runat="server" /></h1><br />
        <asp:Repeater ID="allVidsRepeater" runat="server">
                <ItemTemplate>
                <div class="allVidsOff">
                <table cellpadding="9" cellspacing="0">
                <tr>
                    <td><asp:ImageButton ID="videoImageBtn" runat="server" ImageUrl="Images/playVideo.jpg" CommandName="videoRedirect" CommandArgument='<%# Eval("videoInfoID") %>' CssClass="formBtn"  /></td>
                    <td><b><%# Eval("videoName") %> <%# Eval("lenght") %></b><br /><br />
                Presented By <%# Eval("authorName") %><br />
                <em><%# Eval("authTitle") %></em><br /><br />
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
                Presented By <%# Eval("authorName") %><br />
                <em><%# Eval("authTitle") %></em><br /><br />
                <%# Eval("description") %><br /><br />
                </td>
                </tr>
                </table>
                </div>
                </AlternatingItemTemplate>
        </asp:Repeater>
        <!-- Video Detail -->
        <div id="videoDetail" runat="server">
        <b><asp:Label ID="videoTitle" runat="server" /></b><br /><br />
        <asp:Label ID="screenCastVideo" runat="server" />
        <!-- / Video Detail -->
        </div>
    </div>
 <!-- / Page -->
 </div>
 <!--/Main page wrapper -->
 </div>
</asp:Content>

