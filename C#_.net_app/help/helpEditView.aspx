<%@ Page Language="C#" MasterPageFile="~/help/help.master" AutoEventWireup="true" CodeFile="helpEditView.aspx.cs" Inherits="help_helpEditView"%>
<%@ Register
    Assembly="AjaxControlToolkit"
    Namespace="AjaxControlToolkit.HTMLEditor"
    TagPrefix="HTMLEditor" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="bodyClass" Runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <!-- Page wrapper -->
<div id="pageWrapperHelp">
        <!-- Page -->
    <div id="pageHelp">
    <asp:Label ID="labelNotice" runat="server" />
    <asp:Label ID="labelError" runat="server" />
    <!--/help page Panel -->
    <asp:Panel ID="panelEditHelpPage" runat="server">
     Sort Order:&nbsp;&nbsp;<asp:TextBox ID="textBoxSortOrder" runat="server" style="width: 30px;" />&nbsp;&nbsp;
     <asp:RequiredFieldValidator ID="rfvSortOrder" runat="server"
        	                         ControlToValidate="textBoxSortOrder"
        	                         ErrorMessage="Please enter a sort order. This is the order it will appear in sub menu."
        	                         SetFocusOnError="true" Display="None" ValidationGroup="newHelpPage" />
        	                      <atk:ValidatorCalloutExtender ID="rfvSortOrderCallOut" 
                                     runat="server" Enabled="True" TargetControlID="rfvSortOrder"
                                     HighlightCssClass="fieldError">
                                  </atk:ValidatorCalloutExtender>
        	                      <asp:RegularExpressionValidator ID="revSortOrder" runat="server"
        	                        ControlToValidate="textBoxSortOrder"
        	                        ValidationExpression="^\s*[a-zA-Z0-9&quot;'-/\s,\?,\,',;,:,!,@,.,#,\-,&,\),\(,\{,\},\[,\]]+\s*$"
        	                        ErrorMessage="You have entered characters that are not allowed."
        	                        Display="None" ValidationGroup="newHelpPage" />
        	                      <atk:ValidatorCalloutExtender ID="revSortOrderCallOut" 
                                    runat="server" Enabled="True" TargetControlID="revSortOrder"
                                    HighlightCssClass="fieldError">
                                  </atk:ValidatorCalloutExtender><br />
       Help Topic:&nbsp;&nbsp;<asp:DropDownList ID="dropDownHelpTopics" runat="server"></asp:DropDownList>&nbsp;&nbsp;
    Help Page Name (Used as Title):&nbsp;&nbsp;<asp:TextBox ID="textBoxHelpPageName" runat="server" />&nbsp;&nbsp;
    <asp:RequiredFieldValidator ID="rfvHelpPageName" runat="server"
        	                         ControlToValidate="textBoxHelpPageName"
        	                         ErrorMessage="Please enter a help page name."
        	                         SetFocusOnError="true" Display="None" ValidationGroup="newHelpPage" />
        	                      <atk:ValidatorCalloutExtender ID="rfvHelpPageName_callOut" 
                                     runat="server" Enabled="True" TargetControlID="rfvHelpPageName"
                                     HighlightCssClass="fieldError">
                                  </atk:ValidatorCalloutExtender>
        	                      <asp:RegularExpressionValidator ID="revHelpPageName" runat="server"
        	                        ControlToValidate="textBoxHelpPageName"
        	                        ValidationExpression="^\s*[a-zA-Z0-9&quot;'-/\s,\?,\,',;,:,!,@,.,#,\-,&,\),\(,\{,\},\[,\]]+\s*$"
        	                        ErrorMessage="You have entered characters that are not allowed."
        	                        Display="None" ValidationGroup="newHelpPage" />
        	                      <atk:ValidatorCalloutExtender ID="revHelpPageName_callOut" 
                                    runat="server" Enabled="True" TargetControlID="revHelpPageName"
                                    HighlightCssClass="fieldError">
                                  </atk:ValidatorCalloutExtender><br />
    <br /><br />
     <HTMLEditor:Editor ID="htmlEditorEditHelp" runat="server" 
        Height="375px" 
        Width="100%"
        AutoFocus="true"/>
        <div class="center"><br /><asp:Button ID="btnEditHelp" runat="server" onClick="clickSaveNewHelp" Text="Update Page" ValidationGroup="newHelpPage" />
        &nbsp;&nbsp;<asp:Button ID="btnCancelHelp" runat="server" Text="Cancel" /></div>
    </asp:Panel>
    <asp:ScriptManager ID="scriptManagerEditHelp" runat="server" />
    <!--/Page -->
    </div>
    <!--/Page wrapper -->
    </div>
</asp:Content>

