<%@ Page Language="C#" MasterPageFile="~/admin/admin.master" AutoEventWireup="true" CodeFile="impSteps.aspx.cs" Inherits="admin_impSteps" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="bodyClass" Runat="Server">
<body id="admin">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<asp:ScriptManager ID="newImpStepScriptManager" runat="server" />
<!-- Page wrapper -->
<div id="pageWrapper">
    <!-- Page -->
    <div id="page">
       <div style="float:left; margin-bottom: 15px;">
       <asp:Button id="newImpStepBtn" runat="server" onClick="clickAddNew" Text="New Imp Step" />
       </div>
       <div style="float: right;">
       <h3><asp:Label ID="h3Title" runat="server" /></h3>
       </div>
       <div style="clear:both;">
       <asp:Label ID="newImpStepSuccess" runat="server" style="display:none;" />
       <div id="deleteConfirmDiv" runat="server" style="display:none;"> <asp:Button ID="btnDeleteConfirm" runat="server" OnClick="deleteTrue" CssClass="formBtn" Text="Confirm Delete" /> <asp:Button ID="btnCancelDelete" runat="server" OnClick="cancelDelete" Text="Cancel" /><asp:Label ID="deleteConfirmation" runat="server" /></div>
       </div>
       <div id="obj_addNew" runat="server" >
       <h2>Add New Implementation Step</h2><br />
        <asp:Label ID="newImpStepError" runat="server" style="display:none;" />
        Name:<br />
        <asp:TextBox ID="impStepName" runat="server" />
                        <asp:RequiredFieldValidator ID="reqImpStepName" runat="server"
        	                ControlToValidate="impStepName"
        	                ErrorMessage="Please enter implemenation step name."
        	                SetFocusOnError="true" Display="None" ValidationGroup="newImpStep" />
        	            <cc1:ValidatorCalloutExtender ID="callOutImpStepName" 
                            runat="server" Enabled="True" TargetControlID="reqImpStepName"
                            HighlightCssClass="fieldError">
                        </cc1:ValidatorCalloutExtender>
        	            <asp:RegularExpressionValidator ID="regExpImpStepName" runat="server"
        	                ControlToValidate="impStepName"
        	                ValidationExpression="^\s*[a-zA-Z0-9&quot;'-/\s,\?,\,',;,:,!,@,.,#,\-,&,\),\(,\{,\},\[,\]]+\s*$"
        	                ErrorMessage="You have entered characters that are not allowed."
        	                Display="None" ValidationGroup="newImpStep" />
        	            <cc1:ValidatorCalloutExtender ID="callOutRegExpImpStepName" 
                            runat="server" Enabled="True" TargetControlID="regExpImpStepName"
                            HighlightCssClass="fieldError">
                        </cc1:ValidatorCalloutExtender>
        <br />
        Sort Order:<br />(you can use the same order number more than once)<br />
        <asp:TextBox ID="impStepPriority" runat="server" />
                        <asp:RequiredFieldValidator ID="reqImpStepPriority" runat="server"
        	                ControlToValidate="impStepPriority"
        	                ErrorMessage="Please enter priority. For example 1, 2 or 3. Multiple implemenation steps can have the same priority number."
        	                SetFocusOnError="true" Display="None" ValidationGroup="newImpStep" />
        	            <cc1:ValidatorCalloutExtender ID="callOutImpStepPriority" 
                            runat="server" Enabled="True" TargetControlID="reqImpStepPriority"
                            HighlightCssClass="fieldError">
                        </cc1:ValidatorCalloutExtender>
        	            <asp:RegularExpressionValidator ID="regExpImpStepPriority" runat="server"
        	                ControlToValidate="impStepPriority"
        	                ValidationExpression="^\s*[a-zA-Z0-9&quot;'-/\s,\?,\,',;,:,!,@,.,#,\-,&,\),\(,\{,\},\[,\]]+\s*$"
        	                ErrorMessage="You have entered characters that are not allowed."
        	                Display="None" ValidationGroup="newImpStep" />
        	            <cc1:ValidatorCalloutExtender ID="callOutRegExpImpStepPriority" 
                            runat="server" Enabled="True" TargetControlID="regExpImpStepPriority"
                            HighlightCssClass="fieldError">
                        </cc1:ValidatorCalloutExtender>
        <br /><br />Video Edition:<br />
                        <asp:checkBoxList ID="cbVidEdIS" runat="server" RepeatDirection="Horizontal" /><br />
        Description:<br />
        <asp:TextBox ID="impStepDescription" runat="server" Rows="7" TextMode="MultiLine" style="width: 300px;" />
                        <asp:RequiredFieldValidator ID="reqImpStepDescription" runat="server"
        	                ControlToValidate="impStepDescription"
        	                ErrorMessage="Please enter a implemenation step description."
        	                SetFocusOnError="true" Display="None" ValidationGroup="newImpStep" />
        	            <cc1:ValidatorCalloutExtender ID="callOutImpStepDescription" 
                            runat="server" Enabled="True" TargetControlID="reqImpStepDescription"
                            HighlightCssClass="fieldError">
                        </cc1:ValidatorCalloutExtender>
        	            <asp:RegularExpressionValidator ID="regExpImpStepDescription" runat="server"
        	                ControlToValidate="impStepDescription"
        	                ValidationExpression="^\s*[a-zA-Z0-9&quot;'-/\s,\?,\,',;,:,!,@,.,#,\-,&,\),\(,\{,\},\[,\]]+\s*$"
        	                ErrorMessage="You have entered characters that are not allowed."
        	                Display="None" ValidationGroup="newImpStep" />
        	            <cc1:ValidatorCalloutExtender ID="callOutRegExpImpStepDescription" 
                            runat="server" Enabled="True" TargetControlID="regExpImpStepDescription"
                            HighlightCssClass="fieldError">
                        </cc1:ValidatorCalloutExtender>
        <br />
        <asp:Button ID="btnNewImpStep" runat="server" Text="Add New" OnClick="newImpStep" CssClass="formBtn" ValidationGroup="newImpStep" /> <asp:Button ID="btnImpCancel" runat="server" Text="Cancel" OnClick="cancelNewImpStep" CssClass="formBtn" />
        </div>
        <div id="obj_categoryGrid" runat="server">
<!-- Start Imp Step Grid -->
              <asp:Label ID="test" runat="server" />
             <asp:Label ID="sqlError" runat="server" />
             <asp:GridView ID="impStepGrid" runat="server"
             AutoGenerateDeleteButton="true"
             AutoGenerateEditButton="true"
             AutoGenerateColumns="false"
             onRowEditing="impStepGrid_RowEditing"
             onRowCancelingEdit="impStepGrid_RowCancelingEdit"
             onRowUpdating="impStepGrid_RowUpdating"
             OnRowDeleting="impStepGrid_RowDeleting"
             datakeynames="impStepID">
                <Columns>
                    <asp:TemplateField HeaderText="Name" HeaderStyle-HorizontalAlign="Left"> 
                        <EditItemTemplate>
                            <asp:TextBox ID="impStepNameEdit" Text='<%# Bind("impStepName")%>' runat="server"></asp:TextBox>
                                  <asp:RequiredFieldValidator ID="regImpStepName" runat="server"
        	                         ControlToValidate="impStepNameEdit"
        	                         ErrorMessage="Please enter implemenation step name."
        	                         SetFocusOnError="true" Display="None" />
        	                      <cc1:ValidatorCalloutExtender ID="regImpStepName_ValidatorCalloutExtender" 
                                     runat="server" Enabled="True" TargetControlID="regImpStepName"
                                     HighlightCssClass="fieldError">
                                  </cc1:ValidatorCalloutExtender>
        	                      <asp:RegularExpressionValidator ID="regExpImpStepName" runat="server"
        	                        ControlToValidate="impStepNameEdit"
        	                        ValidationExpression="^\s*[a-zA-Z0-9&quot;'-/\s,\?,\,',;,:,!,@,.,#,\-,&,\),\(,\{,\},\[,\]]+\s*$"
        	                        ErrorMessage="You have entered characters that are not allowed."
        	                        Display="None" />
        	                      <cc1:ValidatorCalloutExtender ID="regExpImpStepName_ValidatorCalloutExtender" 
                                    runat="server" Enabled="True" TargetControlID="regExpImpStepName"
                                    HighlightCssClass="fieldError">
                                  </cc1:ValidatorCalloutExtender>
                        </EditItemTemplate>
                        <ItemTemplate>
                           <asp:Label ID="impStepNameLbl" runat="server" Text='<%# Bind("impStepName") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Priority" HeaderStyle-HorizontalAlign="Left"> 
                        <EditItemTemplate>
                            <asp:TextBox ID="impStepPriorityEdit" Text='<%# Bind("Priority")%>' runat="server"></asp:TextBox>
                                  <asp:RequiredFieldValidator ID="regImpStepPriority" runat="server"
        	                         ControlToValidate="impStepPriorityEdit"
        	                         ErrorMessage="Please enter priority. For example 1, 2 or 3. Multiple implemenation steps can have the same priority number."
        	                         SetFocusOnError="true" Display="None" />
        	                      <cc1:ValidatorCalloutExtender ID="regImpStepPriority_ValidatorCalloutExtender" 
                                     runat="server" Enabled="True" TargetControlID="regImpStepPriority"
                                     HighlightCssClass="fieldError">
                                  </cc1:ValidatorCalloutExtender>
        	                      <asp:RegularExpressionValidator ID="regExpImpStepPriority" runat="server"
        	                        ControlToValidate="impStepPriorityEdit"
        	                        ValidationExpression="^\s*[a-zA-Z0-9&quot;'-/\s,\?,\,',;,:,!,@,.,#,\-,&,\),\(,\{,\},\[,\]]+\s*$"
        	                        ErrorMessage="You have entered characters that are not allowed."
        	                        Display="None" />
        	                      <cc1:ValidatorCalloutExtender ID="regExpImpStepPriority_ValidatorCalloutExtender" 
                                    runat="server" Enabled="True" TargetControlID="regExpImpStepPriority"
                                    HighlightCssClass="fieldError">
                                  </cc1:ValidatorCalloutExtender>
                        </EditItemTemplate>
                        <ItemTemplate>
                           <asp:Label ID="impStepPriorityLbl" runat="server" Text='<%# Bind("Priority") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Edition" HeaderStyle-HorizontalAlign="Left">
                        <EditItemTemplate>
                         <asp:CheckBoxList ID="cbVidEdIsEdit" runat="server"></asp:CheckBoxList>
                        </EditItemTemplate>
                        <ItemTemplate>
                         <asp:ListBox ID="impStepEditionLb" runat="server" Enabled="false"></asp:ListBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Description" HeaderStyle-HorizontalAlign="Left"> 
                        <EditItemTemplate>
                            <asp:TextBox ID="impStepDescriptionEdit" Text='<%# Bind("Description")%>' Rows="7" TextMode="MultiLine" style="width: 300px;" runat="server" lin></asp:TextBox>
                                  <asp:RequiredFieldValidator ID="regImpStepDesc" runat="server"
        	                         ControlToValidate="impStepDescriptionEdit"
        	                         ErrorMessage="Please enter a description."
        	                         SetFocusOnError="true" Display="None" />
        	                      <cc1:ValidatorCalloutExtender ID="reqImpStepDesc_ValidatorCalloutExtender" 
                                     runat="server" Enabled="True" TargetControlID="regImpStepDesc"
                                     HighlightCssClass="fieldError">
                                  </cc1:ValidatorCalloutExtender>
        	                      <asp:RegularExpressionValidator ID="regExpImpStepDesc" runat="server"
        	                        ControlToValidate="impStepDescriptionEdit"
        	                        ValidationExpression="^\s*[a-zA-Z0-9&quot;'-/\s,\?,\,',;,:,!,@,.,#,\-,&,\),\(,\{,\},\[,\]]+\s*$"
        	                        ErrorMessage="You have entered characters that are not allowed."
        	                        Display="None" />
        	                      <cc1:ValidatorCalloutExtender ID="regExpImpStepDesc_ValidatorCalloutExtender" 
                                    runat="server" Enabled="True" TargetControlID="regExpImpStepDesc"
                                    HighlightCssClass="fieldError">
                                  </cc1:ValidatorCalloutExtender>
                        </EditItemTemplate>
                        <ItemTemplate>
                           <asp:Label ID="impStepDescLbl" runat="server" Text='<%# Bind("Description") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                 <AlternatingRowStyle cssClass="alternateRow" />
             </asp:GridView>
             </div>
    <!-- / Page -->
    </div>
<!-- / Page wrapper -->
</div>
</asp:Content>

