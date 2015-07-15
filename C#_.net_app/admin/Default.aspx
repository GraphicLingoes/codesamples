<%@ Page Language="C#" MasterPageFile="~/admin/admin.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="admin_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="bodyClass" Runat="Server">
    <body id="admin">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <!-- NOTE: Mini ribbon is included in master page -->
    <!-- Page Wrapper -->
    <div id="pageWrapper">
    <asp:ScriptManager ID="scriptManagerAdmin" runat="server" />
    <asp:Label ID="dbErrorMessage" runat="server" />
        <!-- Page -->
        <div id="page">
        This page is being save to act as some kind of dashboard in the future.
        <!-- / Page -->
        </div>
    <!-- / Page Wrapper -->
    </div>
</asp:Content>

