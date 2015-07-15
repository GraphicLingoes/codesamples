﻿<%@ Page Language="C#" MasterPageFile="~/admin/admin.master" AutoEventWireup="true" CodeFile="videosCat.aspx.cs" Inherits="admin_videosCat" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="bodyClass" Runat="Server">
<body id="admin">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<asp:ScriptManager ID="videoCatListScriptManager" runat="server" />
<!-- Page wrapper -->
<div id="pageWrapper">
    <!-- Page -->
    <div id="page">
    <div style="float: left;"><asp:ImageButton id="backToMainIB" runat="server" ImageUrl="../Images/miniRibbon/back_32.png" onClick="backToMain" ToolTip="Back To Videos Main" /></div>
    <div style="float: left; margin: 9px 0px 21px 6px;"><asp:LinkButton id="backToMainLB" runat="server" Text="Back To Videos Home" onClick="backToMain" /> </div>
    <div style="float: right;"><h3><asp:Label ID="h3Title" runat="server" /></h3></div>
<asp:Label id="debuging" runat="server" />
<asp:Label ID="newEdError" runat="server" style="display:none" />
<asp:Label ID="dbErrorMessage" runat="server" />
 <div id="deleteConfirmDiv" runat="server" style="display:none;"> <asp:Button ID="btnDeleteConfirm" runat="server" OnClick="deleteTrue" CssClass="formBtn" Text="Confirm Delete" /> <asp:Button ID="btnCancelDelete" runat="server" OnClick="cancelDelete" Text="Cancel" /><asp:Label ID="deleteConfirmation" runat="server" /></div>
    <!-- Start Category Grid View -->
             <asp:GridView ID="categoryGrid" runat="server"
             AutoGenerateDeleteButton="true"
             AutoGenerateEditButton="true"
             AutoGenerateColumns="false"
             onRowEditing="categoryGrid_RowEditing"
             onRowCancelingEdit="categoryGrid_RowCancelingEdit"
             onRowUpdating="categoryGrid_RowUpdating"
             OnRowDeleting="categoryGrid_RowDeleting"
             datakeynames="videoCatID">
                <Columns>
                    <asp:TemplateField HeaderText="Category" HeaderStyle-HorizontalAlign="Left"> 
                        <EditItemTemplate>
                            <asp:TextBox ID="categoryNameEdit" Text='<%# Bind("categoryName")%>' runat="server" MaxLength="50"></asp:TextBox>
                                  <asp:RequiredFieldValidator ID="editCatNameReq" runat="server"
        	                         ControlToValidate="categoryNameEdit"
        	                         ErrorMessage="Video category name required to update."
        	                         SetFocusOnError="true" Display="None" />
        	                      <cc1:ValidatorCalloutExtender ID="editCatNameReq_ValidatorCalloutExtender" 
                                     runat="server" Enabled="True" TargetControlID="editCatNameReq"
                                     HighlightCssClass="fieldError">
                                  </cc1:ValidatorCalloutExtender>
        	                      <asp:RegularExpressionValidator ID="regExpressioneditCatName" runat="server"
        	                        ControlToValidate="categoryNameEdit"
        	                        ValidationExpression="^\s*[a-zA-Z0-9,\s,?,\,,',;,),(,:,!,@,.,#,\-,&]+\s*$"
        	                        ErrorMessage="You have entered characters that are not allowed."
        	                        Display="None" />
        	                      <cc1:ValidatorCalloutExtender ID="editCategory_ValidatorCalloutExtender" 
                                    runat="server" Enabled="True" TargetControlID="regExpressioneditCatName"
                                    HighlightCssClass="fieldError">
                                  </cc1:ValidatorCalloutExtender>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:TextBox ID="categoryNameInsert" runat="server" Text='<%# Bind("categoryName") %>'></asp:TextBox>
                        </InsertItemTemplate>
                        <ItemTemplate>
                           <asp:Label ID="categoryNameLabel" runat="server" Text='<%# Bind("categoryName") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                     <asp:TemplateField HeaderText="Screencast Folder" HeaderStyle-HorizontalAlign="Left"> 
                        <EditItemTemplate>
                            <asp:TextBox ID="categoryFolderEdit" Text='<%# Bind("scFolder")%>' runat="server" MaxLength="50"></asp:TextBox>
        	                      <asp:RegularExpressionValidator ID="regExpressioneditscFolder" runat="server"
        	                        ControlToValidate="categoryFolderEdit"
        	                        ValidationExpression="^\s*[a-zA-Z0-9,\s,?,\,,',;,),(,:,!,@,.,#,\-,&]+\s*$"
        	                        ErrorMessage="You have entered characters that are not allowed."
        	                        Display="None" />
        	                      <cc1:ValidatorCalloutExtender ID="editFolder_ValidatorCalloutExtender" 
                                    runat="server" Enabled="True" TargetControlID="regExpressioneditscFolder"
                                    HighlightCssClass="fieldError">
                                  </cc1:ValidatorCalloutExtender>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:TextBox ID="categoryFolderInsert" runat="server" Text='<%# Bind("scFolder") %>'></asp:TextBox>
                        </InsertItemTemplate>
                        <ItemTemplate>
                           <asp:Label ID="categoryFolderLabel" runat="server" Text='<%# Bind("scFolder") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                 <AlternatingRowStyle cssClass="alternateRow" />
             </asp:GridView>
    <!-- / Page -->
    </div>
<!-- / Page wrapper -->
</div>
</asp:Content>

