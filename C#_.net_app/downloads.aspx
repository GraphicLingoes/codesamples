<%@ Page Language="C#" MasterPageFile="~/portal.master" AutoEventWireup="true" CodeFile="downloads.aspx.cs" Inherits="downloads" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="bodyClass" Runat="Server">
<body id="downloads">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<!-- Start Mini Ribbon Secondary Menu -->
 <asp:ScriptManager ID="scriptManagerMemberDownloads" runat="server" />
    <div id="miniRibbon">
        <div id="miniRibbonIcons">
            <div class="miniRibbonImg"><asp:ImageButton ID="allDownloadsIB" ImageUrl="Images/miniRibbon/expand_32.png" runat="server" onclick="allDownloads" ToolTip="All Downloads" /></div>
            <div class="miniRibbonTxt"><asp:LinkButton ID="allDownloadsLB" runat="server" Text="All Downloads" onclick="allDownloads" /></div>
        </div>
    </div>
    <!-- / Mini Ribbon Secondary Menu -->
    </div>
<!-- Page Wrapper -->
<div id="pageWrapper">
    <!-- Page -->
    <div id="page">
    <div id="downloads">
    <ul>
        <li class="odd"><a href="pdf/practicesetupguide.pdf" target="_blank">Practice Setup Guide</a></li>
        <li><a href="pdf/billing.pdf" target="_blank">Billing Training Manual</a></li>
        <li class="odd"><a href="pdf/paymentguide.pdf" target="_blank">Payments Training Manual</a></li>
        <li><a href="pdf/eraTraining.pdf" target="_blank">(ERA)Electronic Remittance Advice Training Manual</a></li>
    </ul>
    </div>
    <!-- / Page -->
    </div>
<!-- / Page Wrapper -->
</div>
</asp:Content>

