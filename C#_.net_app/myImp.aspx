<%@ Page Title="" Language="C#" MasterPageFile="~/portal.master" AutoEventWireup="true" CodeFile="myImp.aspx.cs" Inherits="myImp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="bodyClass" Runat="Server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<!-- Start Mini Ribbon Secondary Menu -->
    <div id="miniRibbon">
        <div id="miniRibbonIcons">
            <asp:Label id="editionName" runat="server" /> <asp:Label id="editionId" runat="server" />
        </div>
    </div>
    <!-- / Mini Ribbon Secondary Menu -->
    </div>
<!-- Page Wrapper -->
<div id="pageWrapper">
    <!-- Page -->
    <div id="page">
   <asp:Label ID="errorMessage" runat="server" />
    <!-- Start header -->
       <h2>Online Setup Guide</h2>
       <div class="notice">
       <div id="initalNotice" runat="server" class="searchNotice"><b><em>Welcome to the online Setup Guide!</em></b><br />NOTICE: Setting up the LeonardoMD database is usually done by the administrator of the database or your office manager.<br />
       <br />If you are not the administrator and are just looking for training videos <b><a href="memberVideos.aspx">Click Here</a>.</b></div><br />
       <table cellpadding="9px" >
       <tr>
        <td valign="top"> 
        <b>Click Edition for Setup Guide</b><br />
            <asp:ImageButton ID="edImageBtnPro" runat="server" ImageUrl="Images/uploads/proLogo.jpg" OnClick="editionSelectHandler" CommandName="proSelect" CommandArgument='1' CssClass="formBtn" ToolTip="Click for Professional Edition Setup Guide" /><br />
            <asp:ImageButton ID="edImageBtnOffice" runat="server" ImageUrl="Images/uploads/officeLogo.jpg" OnClick="editionSelectHandler" CommandName="officeSelect" CommandArgument='2' CssClass="formBtn" ToolTip="Click for Office Edition Setup Guide" /><br />
            <asp:ImageButton ID="edImageBtnClaims" runat="server" ImageUrl="Images/uploads/claimsLogo.jpg" OnClick="editionSelectHandler" CommandName="claimsSelect" CommandArgument='3' CssClass="formBtn" ToolTip="Click for Claims Edition Setup Guide" /><br />
            <asp:ImageButton ID="edImageBtnStd" runat="server" ImageUrl="Images/uploads/standardLogo.jpg" OnClick="editionSelectHandler" CommandName="standardSelect" CommandArgument='5' CssClass="formBtn" ToolTip="Click for Standard Edition Setup Guide" />
        </td>
        <td valign="top">
            <b>Directions:</b><br />1.) Click on your edition of LeonardoMD.
            <br />2.) Click the "Select" link to see assoicated videos.
            <br />3.) Once you have completed a step click the "Mark Completed" link in the grid below.<br /><br />
            &nbsp;</td>
       </tr>
       </table>
       </div>
    <br />
    <!-- End header -->
    <!-- Start myImpBox2 -->
    <!-- Start Video Grid View -->
         <div id="completeConfirmDiv" runat="server" style="display:none;"> <asp:Label ID="ConfirmConfirmation" runat="server"></asp:Label><div align="center"><asp:Button ID="btnCompleteConfirm" runat="server" OnClick="completeTrue" CssClass="formBtn" Text="Confirm Completion" /> <asp:Button ID="btnCancelConfirm" runat="server" OnClick="cancelConfirm" Text="Cancel" /></div><br /></div>
         <asp:GridView ID="videoImpGrid" runat="server"
            AutoGenerateColumns="false" AllowSorting="True" AllowPaging="true" PageSize="50" 
            onPageIndexChanging = "videoImpGrid_PageIndexChanging"
            onsorting="videoImpGrid_Sorting"
            onSelectedIndexChanged = "videoImpGrid_SelectedIndexChanged"
            onrowcommand="videoImpGrid_RowCommand">
            <Columns>
                <asp:ButtonField HeaderText="Complete" CommandName="markComplete" Text="Mark Complete" />
                <asp:BoundField DataField="Priority" HeaderText="Sort Order" HeaderStyle-HorizontalAlign="Left" SortExpression="Priority" />
                <asp:BoundField DataField="impStepID" HeaderText="ID" HeaderStyle-HorizontalAlign="Left" SortExpression="impStepID" Visible="false" />
                <asp:BoundField DataField="impStepName" HeaderText="Implementation Step" HeaderStyle-HorizontalAlign="Left" SortExpression="impStepName" />
                <asp:BoundField DataField="Description" HeaderText="Description" HeaderStyle-HorizontalAlign="Left" SortExpression="Description" />
                <asp:ButtonField HeaderText="View Videos" CommandName="Select" Text="Select" />
            </Columns>
            <AlternatingRowStyle CssClass="alternateRow" />
       </asp:GridView> 
<!-- End Video Grid View -->
<asp:Repeater ID="guideVidsRepeater" runat="server" >
            <HeaderTemplate><br /><h2>Helpful Videos for "<asp:Label ID="h2Label" runat="server" />":</h2></HeaderTemplate>
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
        <!-- /Page -->
        </div>
        <!-- /Page Wrapper -->
        </div>
</asp:Content>

