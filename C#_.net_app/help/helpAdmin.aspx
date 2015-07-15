<%@ Page Language="C#" MasterPageFile="~/admin/admin.master" AutoEventWireup="true" CodeFile="helpAdmin.aspx.cs" Inherits="help_helpAdmin" %>
<%@ Register
    Assembly="AjaxControlToolkit"
    Namespace="AjaxControlToolkit.HTMLEditor"
    TagPrefix="HTMLEditor" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="bodyClass" Runat="Server"><body id="admin"></asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <!-- Page wrapper -->
<div id="pageWrapper">
<asp:Panel ID="panelTR1" runat="server">
<!-- Tertiary ribbon -->
<div id="tertiaryRibbon">
    <div id="tertiaryLinkButtons">
        <asp:LinkButton ID="lbNewTopic" runat="server" Text="New Help Topic" onclick="clickNewTopic" /> 
        <asp:LinkButton ID="lbManageHelpTopic" runat="server" Text="Manage Help Topics" onclick="clickManageHelpTopics" />
        <asp:LinkButton ID="lbNewHelpPage" runat="server" Text="New Help Page" onclick="clickNewHelpPage" />
        <asp:LinkButton ID="lbManageHelpPage" runat="server" Text="Manage Help Pages" onclick="clickManageHelpPages" />
    </div>
        <!--/Tertiary ribbon -->
    </div>
        </asp:Panel>
        <!-- Page -->
    <div id="page">
    <asp:Label ID="labelNotice" runat="server" />
    <asp:Label ID="labelError" runat="server" />
    <asp:Panel ID="panelNewHelpTopic" runat="server" Visible="false">
    Help Topic Name:<br />
    <asp:TextBox ID="textBoxTopicName" runat="server" />
    <asp:RequiredFieldValidator ID="rfvTopicName" runat="server"
        	                ControlToValidate="textBoxTopicName"
        	                ErrorMessage="Please enter a help topic name."
        	                SetFocusOnError="true" Display="None" ValidationGroup="newTopicName" />
        	            <atk:ValidatorCalloutExtender ID="vceTopicName" 
                            runat="server" Enabled="True" TargetControlID="rfvTopicName"
                            HighlightCssClass="fieldError">
                        </atk:ValidatorCalloutExtender>
        	            <asp:RegularExpressionValidator ID="revTopicName" runat="server"
        	                ControlToValidate="textBoxTopicName"
        	                ValidationExpression="^\s*[a-zA-Z0-9,\s,?,\,,',;,:,(,),_,!,@,.,#,\-,&]+\s*$"
        	                ErrorMessage="You have entered characters that are not allowed."
        	                Display="None" ValidationGroup="newTopicName" />
        	            <atk:ValidatorCalloutExtender ID="vceRevTopicName" 
                            runat="server" Enabled="True" TargetControlID="revTopicName"
                            HighlightCssClass="fieldError">
                        </atk:ValidatorCalloutExtender>
    <br />
    Search Keywords:<br />
    <asp:TextBox ID="textBoxKeyWords" runat="server" />
    <asp:RequiredFieldValidator ID="rfvKeyWords" runat="server"
        	                ControlToValidate="textBoxKeyWords"
        	                ErrorMessage="Please enter at least one keyword."
        	                SetFocusOnError="true" Display="None" ValidationGroup="newTopicName" />
        	            <atk:ValidatorCalloutExtender ID="vceKeyWords" 
                            runat="server" Enabled="True" TargetControlID="rfvKeyWords"
                            HighlightCssClass="fieldError">
                        </atk:ValidatorCalloutExtender>
        	            <asp:RegularExpressionValidator ID="revKeyWords" runat="server"
        	                ControlToValidate="textBoxKeyWords"
        	                ValidationExpression="^\s*[a-zA-Z0-9,\s,?,\,,',;,:,(,),_,!,@,.,#,\-,&]+\s*$"
        	                ErrorMessage="You have entered characters that are not allowed."
        	                Display="None" ValidationGroup="newTopicName" />
        	            <atk:ValidatorCalloutExtender ID="vceRevKeyWords" 
                            runat="server" Enabled="True" TargetControlID="revKeyWords"
                            HighlightCssClass="fieldError">
                        </atk:ValidatorCalloutExtender>
    <br /><br />
    <asp:Button ID="btnSaveTopic" runat="server" Text="Save" ValidationGroup="newTopicName" onClick="clickSaveHelpTopic" />&nbsp;&nbsp;<asp:Button ID="btnCancelTopic" runat="server" Text="Cancel" onClick="clickCancelHelpTopic" />
    </asp:Panel>
    <asp:Panel ID="panelManageHelpTopic" runat="server" Visible="false">
        <asp:GridView ID="gridViewMHelpTopic" runat="server"
            AutoGenerateColumns="false" AllowSorting="True" AllowPaging="true" PageSize="10"
            AutoGenerateDeleteButton="false" 
            AutoGenerateEditButton="true"
            onPageIndexChanging = "gridViewMHT_PageIndexChanging"
            onsorting="gridViewMHT_Sorting"
            onRowEditing="gridViewMHT_RowEditing"
            onRowCancelingEdit="gridViewMHT_RowCancelingEdit"
            onRowUpdating="gridViewMHT_RowUpdating">
             <Columns>
             <asp:TemplateField>
             <ItemTemplate>
               <asp:LinkButton ID="linkButtonHT" runat="server" CommandName="deleteRecord" commandArgument='<%# Bind("helpTopicID")%>' OnCommand="gridViewMHT_RowDeleting" Text="Delete" />
                <atk:ConfirmButtonExtender ID="cbeDelete" runat="server"
                        TargetControlID="linkButtonHT"
                        ConfirmText="Are you sure you wish to delete record? This action is irreversible" />
             </ItemTemplate>
             </asp:TemplateField>
                     <asp:TemplateField HeaderText="ID" HeaderStyle-HorizontalAlign="Left" SortExpression="helpTopicID">
                     <EditItemTemplate>
                            <asp:Label ID="labelEHTID" runat="server" Text='<%# Bind("helpTopicID")%>' />
                        </EditItemTemplate>
                     <ItemTemplate>
                     <asp:Label ID="labelID" runat="server" Text='<%# Bind("helpTopicID")%>' />
                     </ItemTemplate>
                     </asp:TemplateField>
                     <asp:TemplateField HeaderText="Name" HeaderStyle-HorizontalAlign="Left" SortExpression="name"> 
                        <EditItemTemplate>
                            <asp:TextBox ID="textBoxName" Text='<%# Bind("name")%>' runat="server"></asp:TextBox>
                                  <asp:RequiredFieldValidator ID="rfvName" runat="server"
        	                         ControlToValidate="textBoxName"
        	                         ErrorMessage="Please enter help topic name."
        	                         SetFocusOnError="true" Display="None" />
        	                      <atk:ValidatorCalloutExtender ID="rfvName_ValidatorCalloutExtender" 
                                     runat="server" Enabled="True" TargetControlID="rfvName"
                                     HighlightCssClass="fieldError">
                                  </atk:ValidatorCalloutExtender>
        	                      <asp:RegularExpressionValidator ID="revName" runat="server"
        	                        ControlToValidate="textBoxName"
        	                        ValidationExpression="^\s*[a-zA-Z0-9&quot;'-/\s,\?,\,',;,:,!,@,.,#,\-,&,\),\(,\{,\},\[,\]]+\s*$"
        	                        ErrorMessage="You have entered characters that are not allowed."
        	                        Display="None" />
        	                      <atk:ValidatorCalloutExtender ID="revName_ValidatorCalloutExtender" 
                                    runat="server" Enabled="True" TargetControlID="revName"
                                    HighlightCssClass="fieldError">
                                  </atk:ValidatorCalloutExtender>
                        </EditItemTemplate>
                        <ItemTemplate>
                           <asp:Label ID="labelHTname" runat="server" Text='<%# Bind("name")%>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Search Keywords" HeaderStyle-HorizontalAlign="Left" SortExpression="keyWords"> 
                        <EditItemTemplate>
                            <asp:TextBox ID="textBoxKeywords" Text='<%# Bind("keyWords")%>' runat="server"></asp:TextBox>
                                  <asp:RequiredFieldValidator ID="rfvKeyWords" runat="server"
        	                         ControlToValidate="textBoxKeywords"
        	                         ErrorMessage="Please enter search keywords."
        	                         SetFocusOnError="true" Display="None" />
        	                      <atk:ValidatorCalloutExtender ID="rfvKeyWords_ValidatorCalloutExtender" 
                                     runat="server" Enabled="True" TargetControlID="rfvKeyWords"
                                     HighlightCssClass="fieldError">
                                  </atk:ValidatorCalloutExtender>
        	                      <asp:RegularExpressionValidator ID="revKeyWords" runat="server"
        	                        ControlToValidate="textBoxKeywords"
        	                        ValidationExpression="^\s*[a-zA-Z0-9&quot;'-/\s,\?,\,',;,:,!,@,.,#,\-,&,\),\(,\{,\},\[,\]]+\s*$"
        	                        ErrorMessage="You have entered characters that are not allowed."
        	                        Display="None" />
        	                      <atk:ValidatorCalloutExtender ID="revKeyWords_ValidatorCalloutExtender" 
                                    runat="server" Enabled="True" TargetControlID="revKeyWords"
                                    HighlightCssClass="fieldError">
                                  </atk:ValidatorCalloutExtender>
                        </EditItemTemplate>
                        <ItemTemplate>
                           <asp:Label ID="labelKeyWords" runat="server" Text='<%# Bind("keyWords")%>' />
                        </ItemTemplate>
                    </asp:TemplateField>
            </Columns>
            </asp:GridView>
    </asp:Panel>
    <asp:Panel ID="panelNewHelpPage" runat="server" Visible="false">
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
                                  </atk:ValidatorCalloutExtender>
    Sort Order: <asp:TextBox ID="textBoxSortOrder" runat="server" style="width: 30px;" />&nbsp;&nbsp;
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
                                  </atk:ValidatorCalloutExtender>
    <asp:Button ID="btnSaveNewHelp" runat="server" onClick="clickSaveNewHelp" Text="Save Help Page" ValidationGroup="newHelpPage" />
   
    <br /><br />
    <HTMLEditor:Editor ID="htmlEditorNewHelp" runat="server" 
        Height="450px" 
        Width="100%"
        AutoFocus="true"/>
    </asp:Panel>
    <asp:Panel ID="PanelManageHelpPages" runat="server" Visible="false">
    <asp:GridView ID="gridViewMHelpPages" runat="server"
            AutoGenerateColumns="false" AllowSorting="True" AllowPaging="true" PageSize="20"
            AutoGenerateDeleteButton="false" 
            AutoGenerateEditButton="false"
            onPageIndexChanging = "gridViewMHP_PageIndexChanging"
            onsorting="gridViewMHP_Sorting">
             <Columns>
                     <asp:TemplateField>
                     <ItemTemplate>
                        <asp:Label ID="labelViewEdit" runat="server" Text='<%# buildLink((int)Eval("helpPageID")) %>'></asp:Label>
                        </ItemTemplate>
                     </asp:TemplateField>
                     <asp:TemplateField>
             <ItemTemplate>
               <asp:LinkButton ID="linkButtonHP" runat="server" CommandName="deleteRecordHP" commandArgument='<%# Bind("helpPageID")%>' OnCommand="gridViewMHP_RowDeleting" Text="Delete" />
                <atk:ConfirmButtonExtender ID="cbeHelpPage" runat="server"
                        TargetControlID="linkButtonHP"
                        ConfirmText="Are you sure you wish to delete record? This action is irreversible" />
             </ItemTemplate>
             </asp:TemplateField>
                     <asp:TemplateField HeaderText="ID" HeaderStyle-HorizontalAlign="Left" SortExpression="helpPageID">
                     <ItemTemplate>
                     <asp:Label ID="labelHelpPageID" runat="server" Text='<%# Bind("helpPageID") %>' />
                     </ItemTemplate>
                     </asp:TemplateField>
                     <asp:TemplateField HeaderText="Help Topic" HeaderStyle-HorizontalAlign="Left" SortExpression="helpTopicID">
                     <ItemTemplate>
                        <asp:Label ID="labelHelpTopicPages" runat="server" Text='<%# Bind("name") %>' />
                     </ItemTemplate>
                     </asp:TemplateField>
                     <asp:TemplateField HeaderText="Page Name" HeaderStyle-HorizontalAlign="Left" SortExpression="helpPageName">
                     <ItemTemplate>
                        <asp:Label ID="labelHelpPageName" runat="server" Text='<%# Bind("helpPageName") %>' />
                     </ItemTemplate>
                     </asp:TemplateField>
                      <asp:TemplateField HeaderText="Sort Order" HeaderStyle-HorizontalAlign="Left" SortExpression="sortOrder">
                     <ItemTemplate>
                        <asp:Label ID="labelSortOrder" runat="server" Text='<%# Bind("sortOrder") %>' />
                     </ItemTemplate>
                     </asp:TemplateField>
             </Columns>
             </asp:GridView>
    
    </asp:Panel>
    <!--/Page -->
    </div>
    <!--/Page Wrapper -->
</div>
<asp:ScriptManager ID="scriptManagerHelp" runat="server" />
</asp:Content>
