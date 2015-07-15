<%@ Page Language="C#" MasterPageFile="~/portal.master" AutoEventWireup="true" CodeFile="newDashboard.aspx.cs" Inherits="newDashboard" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="content3" ContentPlaceHolderID="bodyClass" runat="server"><body ID="home"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <!-- Start Mini Ribbon Secondary Menu -->
    <asp:ScriptManager ID="scriptManagerDashboard" runat="server" />
    <div id="miniRibbon">
        <div id="miniRibbonIcons">
            <div class="miniRibbonImg"><asp:ImageButton ID="myImpIB" ImageUrl="Images/miniRibbon/paper&pencil_48.png" runat="server" onclick="showRecommendedVids" ToolTip="Recommended Videos" /></div>
            <div class="miniRibbonTxt"><asp:LinkButton ID="myImpLB" runat="server" Text="Recommended Videos" onclick="showRecommendedVids" /></div>
            <div class="miniRibbonImg"><asp:ImageButton ID="videoHistoryIB" ImageUrl="Images/miniRibbon/accepted_48.png" runat="server" onclick="showVideoHistory" ToolTip="Video History" /></div>
            <div class="miniRibbonTxt"><asp:LinkButton ID="videoHistoryLB" runat="server" Text="Video History" onclick="showVideoHistory" /></div>
            <div class="miniRibbonImg"><asp:ImageButton ID="backToHomeIB" ImageUrl="Images/miniRibbon/back_32.png" runat="server" ToolTip="Back" onclick="backToHome" /></div>
            <div class="miniRibbonTxt"><asp:LinkButton ID="backToHomeLB" runat="server" Text="Back" onclick="backToHome" /></div>
            <div class="miniRibbonImg"><asp:ImageButton ID="messagesIB" ImageUrl="Images/miniRibbon/mail_48.png" runat="server" onclick="showMessages" ToolTip="Messages" /></div>
            <div class="miniRibbonTxt"><asp:LinkButton ID="messagesLB" runat="server" Text="Messages" onclick="showMessages" /></div>
        </div>
    </div>
    <!-- / Mini Ribbon Secondary Menu -->
    </div>
    <!-- Main Wrapper 100% -->
    <div id="pageWrapper">
    <!-- Tertiary Menu -->
     <asp:Panel ID="panelTR" runat="server" Visible="false">
        <div id="tertiaryRibbon">
           <div id="tertiaryLinkButtons">
           <asp:LinkButton ID="lbMarkComplete" runat="server" Text="Mark Complete" onclick="clickMarkComplete" />
           <asp:LinkButton ID="lbMarkInComplete" runat="server" Text="Mark Incomplete" onclick="clickMarkIncomplete" />
           <asp:LinkButton ID="lbHideComplete" runat="server" Text="Hide Completed" onclick="clickHideComplete" />
           <asp:LinkButton ID="lbShowComplete" runat="server" Text="Show All" onclick="clickShowAll" />
            </div>
        </div>
     </asp:Panel>
    <asp:Label ID="dbErrorMessage" runat="server" />
        <!-- Page -->
        <div id="page">
        <div id="dashboardMessage" runat="server">
        <div id="dashboard_Text">
        <h1>Welcome to the LeonardoMD Video Training Portal</h1>
        <hr />
        <h3>Get Started</h3>
        <p>To start using the video portal simply use the menu icons located above in the ribbon and mini ribbon bars. Click away to get the hang of things. Don't worry you won't hurt anything!</p>
        <br /><br />
        <h3>What's New</h3>
        <p>
        <b>Recommended Videos</b> - Click the recommended videos link to see a personal list of videos tailored 
            specially for you by your LeonardoMD implementation representative.<br />
        <em>NOTE: This list will not be created until you have your inital implementation meeting.</em><br /><br />
        <b>Video History</b> - Click the video history link in the mini ribbon bar above to view a list of videos you have watched and what date you view them. 
        <br /><br />
        <b>Search</b> - We know it can be difficult to find what you are looking for when trying to sift through a list of videos. 
            If you don&#39;t like change click the videos icon in the ribbon bar above to find 
            videos just like the old video portal. However, if you are adventurous you can use the search feature by clicking search on the ribbon bar above. Give it a try!<br /><br />
        <b>Downloads</b> - Click the downloads icon in the ribbon bar above to find helpful pdf manuals and other documentation.
        </p>
        </div>
        </div>
        <div style="clear: both;">
        <asp:Label ID="noHistory" runat="server" />
        <asp:Label ID="noticeLabel" runat="server" />
        </div>
        <asp:Panel ID="panelRecVids" runat="server" Visible="false">
        <img src="Images/keyNotComplete.gif" title="Video Not Complete" />&nbsp;Not Complete&nbsp;&nbsp;<img src="Images/keyComplete.gif" title="Completed" />&nbsp;Completed
        <!-- Recommended Videos Grid -->
       <asp:Repeater ID="recVidsRepeater" runat="server" onitemcommand="recVidsRepeater_ItemCommand">
                <ItemTemplate>
                <div class="allVidsOff">
                <table cellpadding="9" cellspacing="0">
                <tr><asp:Label ID="labelTableClass" runat="server" /><b>#<%# Eval("sortID") %></b> | Recommended On: <%# Eval("createdDate") %>  |  <b>Completed On: <asp:Label ID="labelCompleted" runat="server" Text='<%# Eval("completedDate") %>' /></b></div></tr>
                <tr>
                    <td><asp:CheckBox ID="rowLevelCheckBox" runat="server" /></td>
                    <td><asp:ImageButton ID="videoImageBtn" runat="server" ImageUrl="Images/playVideo.jpg" CommandName="videoRedirect" CommandArgument='<%# Eval("videoInfoID") %>' CssClass="formBtn"  /></td>
                    <td width="100%"><b><%# Eval("videoName") %> <%# Eval("lenght") %></b><br />
                    #1<asp:Label id="recordID" runat="server" Text=<%# Eval("recID") %> />
                    <br />
                    <br />
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
                 <tr><asp:Label ID="labelTableClass" runat="server" /><b>#<%# Eval("sortID") %></b> | Recommended On: <%# Eval("createdDate") %>  |  <b>Completed On: <asp:Label ID="labelCompleted" runat="server" Text='<%# Eval("completedDate") %>' /></b></div></tr>
                <tr>
                    <td><asp:CheckBox ID="rowLevelCheckBox" runat="server" /></td>
                    <td><asp:ImageButton ID="videoImageBtn" runat="server" ImageUrl="Images/playVideo.jpg" CommandName="videoRedirect" CommandArgument='<%# Eval("videoInfoID") %>' CssClass="formBtn" /></td>
                    <td width="100%"><b><%# Eval("videoName") %> <%# Eval("lenght") %></b><br />
                     #1<asp:Label id="recordID" runat="server" Text=<%# Eval("recID") %> />
                    <br />
                    <br />
                Presented By <%# Eval("authorName") %><br />
                <em><%# Eval("authTitle") %></em><br /><br />
                <%# Eval("description") %><br /><br />
                </td>
                </tr>
                </table>
                </div>
                </AlternatingItemTemplate>
        </asp:Repeater>
         </asp:Panel>
         <!-- Watched videos grid -->
            <asp:GridView ID="gridHistory" runat="server" AllowPaging="false" AutoGenerateColumns="false" onSelectedIndexChanged = "watchedVideoGrid_SelectedIndexChanged" onrowcommand="watchedVideoGrid_RowCommand">
            <Columns>
            <asp:BoundField HeaderText="Video Name" DataField="videoName" />
            <asp:BoundField HeaderText="Date Viewed" DataField="createdDate" />
            </Columns>
            <AlternatingRowStyle CssClass="alternateRow" />
            </asp:GridView><br />
            <asp:GridView ID="watchedVideoGrid" runat="server" AllowPaging="false" AutoGenerateColumns="false" onSelectedIndexChanged = "watchedVideoGrid_SelectedIndexChanged" onrowcommand="watchedVideoGrid_RowCommand"
         >
            <Columns>
            <asp:BoundField HeaderText="Video Name" DataField="videoName" />
            <asp:BoundField HeaderText="Description" DataField="description" />
            <asp:BoundField HeaderText="Length" DataField="lenght" />
            <asp:BoundField HeaderText="Number of Views" DataField="videoCount" />
            <asp:ButtonField HeaderText="Viewing History" CommandName="viewHistory" Text="Details" />
            <asp:ButtonField HeaderText="Watch Again" CommandName="Select" Text="Watch Video" />
            </Columns>
            <AlternatingRowStyle CssClass="alternateRow" />
            </asp:GridView>
            <!-- / Watched Video Grid -->
        <!-- / Page -->
        </div>
    <!-- / Main Wrapper 100% -->
    </div>
</asp:Content>

