<%@ Page Language="C#" MasterPageFile="~/admin/admin.master" AutoEventWireup="true" CodeFile="manageTemplates.aspx.cs" Inherits="admin_manageTemplates"%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<script type="text/javascript" src="../jquery/jquery-1.4.2.min.js"></script>
<script type="text/javascript" src="../jquery/hideBoxes.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="bodyClass" Runat="Server">
    <body id="admin">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <!-- Page wrapper -->
<div id="pageWrapper">
    <!-- Tertiary ribbon -->
    <asp:Panel ID="panelTR1" runat="server">
    <div id="tertiaryRibbon">
        <div id="tertiaryLinkButtons">
            <asp:LinkButton ID="lbBackToSearch" runat="server" Text="Back to Search" onclick="clickBackToSearch" />
            <asp:LinkButton ID="lbNewTemplate" runat="server" Text="New Template" onclick="clickNewTemplate" />
            <asp:LinkButton ID="lbEditTemplates" runat="server" Text="Edit Templates" onclick="clickEditTemplates" />
        </div>
    </div>
    </asp:Panel>
      <!-- Edit template panel ribbon nav -->
     <asp:Panel ID="panelTR2" runat="server" Visible="false">
     <div id="tertiaryRibbon">
           <div id="tertiaryLinkButtons">
           <asp:LinkButton ID="lbStartOver" runat="server" Text="Start Over" onclick="clickStartOver" />
           <asp:LinkButton ID="lbUpdateSort" runat="server" Text="Update List" onclick="clickUpdateSort" />
           <asp:LinkButton ID="lbDeleteVid" runat="server" Text="Delete Selected" onclick="clickDeleteAssoc" />
           <asp:LinkButton ID="lbRestSort" runat="server" Text="Reset Sort" onclick="clickResetSortT" />
           <asp:LinkButton ID="lbAddVideos" runat="server" Text="Add / Update Videos" onclick="clickAddVideos" />
            </div>
        </div>
     </asp:Panel>
    <!-- Page -->
    <div id="page">
        <asp:ScriptManager ID="scriptManagerTemplates" runat="server" />
        <asp:Label ID="labelNotice" runat="server" />
        <asp:Label id="labelDBerror" runat="server" />
         <asp:Panel ID="panelDeleteTemplate" runat="server" Visible="false">
            <asp:HiddenField ID="hiddenFieldTempID" runat="server" />
            <asp:Button ID="btnDeleteTempConfirm" runat="server" Text="Confirm Delete" onclick="clickConfirmTemplateDelete" />&nbsp;&nbsp;
            <asp:Button ID="btnDeleteTemplateCancel" runat="server" Text="Cancel" onclick="clickCancelTemplateDelete" />
            <br /><br />
         </asp:Panel>
         <asp:Panel ID="panelDelete" runat="server" Visible="false">
                <asp:Button ID="btnDeleteConfirm" runat="server" Text="Confirm Delete" onclick="clickConfirmDelete" />&nbsp;&nbsp;
                <asp:Button ID="btnDeleteCancel" runat="server" Text="Cancel" onclick="clickCancelDelete" />
                <br /><br />
            </asp:Panel>
        <asp:Panel ID="panelNewTemplate" runat="server" Visible="false">
        Template Name:<br />
        <asp:TextBox ID="textBoxName" runat="server" />
         <asp:RequiredFieldValidator ID="reqName" runat="server"
        	                ControlToValidate="textBoxName"
        	                ErrorMessage="Please enter a template name."
        	                SetFocusOnError="true" Display="None" ValidationGroup="newTemplate" />
        	            <atk:ValidatorCalloutExtender ID="callOutName" 
                            runat="server" Enabled="True" TargetControlID="reqName"
                            HighlightCssClass="fieldError">
                        </atk:ValidatorCalloutExtender>
        	            <asp:RegularExpressionValidator ID="regExpName" runat="server"
        	                ControlToValidate="textBoxName"
        	                ValidationExpression="^\s*[a-zA-Z0-9,\s,?,\,,',;,:,(,),_,!,@,.,#,\-,&]+\s*$"
        	                ErrorMessage="You have entered characters that are not allowed."
        	                Display="None" ValidationGroup="newTemplate" />
        	            <atk:ValidatorCalloutExtender ID="callOutName2" 
                            runat="server" Enabled="True" TargetControlID="regExpName"
                            HighlightCssClass="fieldError">
                        </atk:ValidatorCalloutExtender>
        <br /><br />
        Template Description:<br />
        <asp:TextBox ID="textBoxDescription" runat="server" Rows="7" TextMode="MultiLine" style="width: 200px;" />
                        <asp:RequiredFieldValidator ID="reqVidDesc" runat="server"
        	                ControlToValidate="textBoxDescription"
        	                ErrorMessage="Please enter a template description."
        	                SetFocusOnError="true" Display="None" ValidationGroup="newTemplate" />
        	            <atk:ValidatorCalloutExtender ID="callOutVidDesc" 
                            runat="server" Enabled="True" TargetControlID="reqVidDesc"
                            HighlightCssClass="fieldError">
                        </atk:ValidatorCalloutExtender>
        	            <asp:RegularExpressionValidator ID="regExpVidDesc" runat="server"
        	                ControlToValidate="textBoxDescription"
        	                ValidationExpression="^\s*[a-zA-Z0-9,\s,?,\,,',;,:,(,),_,!,@,.,#,\-,&]+\s*$"
        	                ErrorMessage="You have entered characters that are not allowed."
        	                Display="None" ValidationGroup="newTemplate" />
        	            <atk:ValidatorCalloutExtender ID="callOutRegExpVidDesc" 
                            runat="server" Enabled="True" TargetControlID="regExpVidDesc"
                            HighlightCssClass="fieldError">
                        </atk:ValidatorCalloutExtender><br /><br />
                        <asp:Button ID="buttonSaveTemplate" runat="server" Text="Save" ValidationGroup="newTemplate" OnClick="clickSaveTemplate" /> 
                        <asp:Button ID="buttonCancelTemplate" runat="server" Text="Cancel" OnClick="clickCancelTemplate" />
        </asp:Panel>
         <asp:Panel ID="panelEditTemplates" runat="server" Visible="false">
            <asp:GridView ID="gridViewEditTemplates" runat="server"
             AutoGenerateDeleteButton="true"
             AutoGenerateEditButton="true"
             AutoGenerateColumns="false"
             AllowPaging="true"
             PageSize="5"
             EmptyDataText="You have not created any templates yet. Use the start over button above."
             OnPageIndexChanging="gridViewEditTemplates_PageIndexChanging"
             onRowEditing="gridViewEditTemplates_RowEditing"
             onRowCancelingEdit="gridViewEditTemplates_RowCancelingEdit"
             onRowUpdating="gridViewEditTemplates_RowUpdating"
             OnRowDeleting="gridViewEditTemplates_RowDeleting"
             onRowCommand="gridViewEditTemplates_RowCommand"
            >
            <Columns>
                <asp:ButtonField CommandName="clickEditVideos" Text="Edit Videos" />
                <asp:TemplateField HeaderText="Row" HeaderStyle-HorizontalAlign="Left">
                <ItemTemplate>
                    <%# Container.DataItemIndex + 1 %>
                </ItemTemplate>
            </asp:TemplateField>
                <asp:TemplateField HeaderText="Name" HeaderStyle-HorizontalAlign="Left">
                    <EditItemTemplate>
                    Current Name: <asp:Label ID="compareName" runat="server" Text='<%# Eval("templateName") %>' /><br />
                    <asp:TextBox ID="textBoxEditName" runat="server" Text='<%# Eval("templateName") %>' />
         <asp:RequiredFieldValidator ID="reqEditName" runat="server"
        	                ControlToValidate="textBoxEditName"
        	                ErrorMessage="Please enter a template name."
        	                SetFocusOnError="true" Display="None" ValidationGroup="newTemplate" />
        	            <atk:ValidatorCalloutExtender ID="ValidatorCalloutExtender1" 
                            runat="server" Enabled="True" TargetControlID="reqEditName"
                            HighlightCssClass="fieldError">
                        </atk:ValidatorCalloutExtender>
        	            <asp:RegularExpressionValidator ID="regExpEditName" runat="server"
        	                ControlToValidate="textBoxEditName"
        	                ValidationExpression="^\s*[a-zA-Z0-9,\s,?,\,,',;,:,(,),_,!,@,.,#,\-,&]+\s*$"
        	                ErrorMessage="You have entered characters that are not allowed."
        	                Display="None" ValidationGroup="newTemplate" />
        	            <atk:ValidatorCalloutExtender ID="ValidatorCalloutExtender2" 
                            runat="server" Enabled="True" TargetControlID="regExpEditName"
                            HighlightCssClass="fieldError">
                        </atk:ValidatorCalloutExtender>
                    </EditItemTemplate>
                    <InsertItemTemplate>
                    </InsertItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lableName" runat="server" Text='<%# Eval("templateName") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Description" HeaderStyle-HorizontalAlign="Left">
                    <EditItemTemplate>
                    <asp:TextBox ID="textBoxEditDescription" runat="server" Rows="7" TextMode="MultiLine" Text='<%# Eval("templateDescription") %>' style="width: 200px;" />
                        <asp:RequiredFieldValidator ID="reqVidEditDesc" runat="server"
        	                ControlToValidate="textBoxEditDescription"
        	                ErrorMessage="Please enter a template description."
        	                SetFocusOnError="true" Display="None" ValidationGroup="newTemplate" />
        	            <atk:ValidatorCalloutExtender ID="ValidatorCalloutExtender3" 
                            runat="server" Enabled="True" TargetControlID="reqVidEditDesc"
                            HighlightCssClass="fieldError">
                        </atk:ValidatorCalloutExtender>
        	            <asp:RegularExpressionValidator ID="regExpVidEditDesc" runat="server"
        	                ControlToValidate="textBoxEditDescription"
        	                ValidationExpression="^\s*[a-zA-Z0-9,\s,?,\,,',;,:,(,),_,!,@,.,#,\-,&]+\s*$"
        	                ErrorMessage="You have entered characters that are not allowed."
        	                Display="None" ValidationGroup="newTemplate" />
        	            <atk:ValidatorCalloutExtender ID="ValidatorCalloutExtender4" 
                            runat="server" Enabled="True" TargetControlID="regExpVidEditDesc"
                            HighlightCssClass="fieldError">
                        </atk:ValidatorCalloutExtender>
                    </EditItemTemplate>
                    <InsertItemTemplate>
                    </InsertItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="labelDescription" runat="server" Text='<%# Eval("templateDescription") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            </asp:GridView>
            <asp:Label ID="labelTest" runat="server" />
        </asp:Panel>
        <br /><br />
          <!-- Template Videos panel -->
       <asp:Panel ID="panelTemplateVids" runat="server">
       <asp:Label ID="labelTemplateID" runat="server" Text="0" Visible="false" />
       <asp:GridView ID="recVidsTemplateGrid" runat="server"
             AutoGenerateColumns="false"
             AllowMultiRowEdit="True"
             EmptyDataText="There are no videos associated to this template"
             datakeynames="recTempVidID">
                <Columns>
                     <asp:TemplateField>
                <HeaderTemplate>
                    <asp:CheckBox runat="server" ID="HeaderLevelCheckBoxM" AutoPostBack="true" OnCheckedChanged="HeaderLevelCheckBox_CheckedChangedT" Text="Select All" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:CheckBox runat="server" ID="RowLevelCheckBoxT" />
                </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Record ID" HeaderStyle-HorizontalAlign="Left"> 
                        <ItemTemplate>
                           <asp:Label ID="recordIDLbl" runat="server" Text='<%# Bind("recTempVidID") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Video ID" HeaderStyle-HorizontalAlign="Left">
                        <ItemTemplate>
                            <asp:Label ID="templateIDLbl" runat="server" Text='<%# Bind("recNameID") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Video Name" HeaderStyle-HorizontalAlign="Left"> 
                        <ItemTemplate>
                           <asp:Label ID="vidoeNameLbl" runat="server" Text='<%# Bind("videoName") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Priority" HeaderStyle-HorizontalAlign="Left"> 
                        <ItemTemplate>
                          <asp:TextBox ID="recVidSortEdit" Text='<%# Bind("sortID")%>' runat="server" style="width: 50px;" ></asp:TextBox>
                                  <asp:RequiredFieldValidator ID="regRecVidSortEdit" runat="server"
        	                         ControlToValidate="recVidSortEdit"
        	                         ErrorMessage="Please enter sort Order. For example 1, 2 or 3. Multiple implemenation steps can have the same priority number."
        	                         SetFocusOnError="true" Display="None" />
        	                      <atk:ValidatorCalloutExtender ID="ValidatorCalloutExtender5" 
                                     runat="server" Enabled="True" TargetControlID="regRecVidSortEdit"
                                     HighlightCssClass="fieldError">
                                  </atk:ValidatorCalloutExtender>
        	                      <asp:RegularExpressionValidator ID="regExpVidSortPriority" runat="server"
        	                        ControlToValidate="recVidSortEdit"
        	                        ValidationExpression="^\s*[0-9]+\s*$"
        	                        ErrorMessage="You have entered characters that are not allowed."
        	                        Display="None" />
        	                      <atk:ValidatorCalloutExtender ID="ValidatorCalloutExtender6" 
                                    runat="server" Enabled="True" TargetControlID="regExpVidSortPriority"
                                    HighlightCssClass="fieldError">
                                  </atk:ValidatorCalloutExtender>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                 <AlternatingRowStyle cssClass="alternateRow" />
             </asp:GridView>
       </asp:Panel>
        <!-- Checkbox list panel -->
        <asp:Panel ID="panelCBList" runat="server" Visible="false">
         <!-- Recommend Videos List -->
         <div id="recommendVidsDiv" runat="server">
            <br /><br />
        <div class="greyBkgrdDiv"><asp:CheckBox runat="server" ID="recVidCBLSelectAll" AutoPostBack="true" OnCheckedChanged="recommendVidCBL_CheckedChanged" Text="Select All" /></div>
       </div>
        <asp:CheckBoxList id="recommendVidCBL"
            cellPadding="3"
            repeatColumns="2"
            repeatDirection="vertical"
            runat="server">
        </asp:CheckBoxList>
        </asp:Panel>
    <!-- /Page -->
    </div>
<!-- /Page wrapper -->
</div>
</asp:Content>

