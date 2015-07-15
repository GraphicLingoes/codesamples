<%@ Page Language="C#" MasterPageFile="~/admin/admin.master" AutoEventWireup="true" CodeFile="recVidTemplates.aspx.cs" Inherits="admin_recVidTemplates"%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script type="text/javascript" src="../jquery/jquery-1.4.2.min.js"></script>
<script type="text/javascript" src="../jquery/hideBoxes.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="bodyClass" Runat="Server">
    <body id="admin">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:ScriptManager ID="recTempListScriptManager" runat="server" />
<!-- Page wrapper -->
<div id="pageWrapper">
    <!-- Tertiary Ribbon -->
     <asp:Panel ID="panelTR3" runat="server">
     <div id="tertiaryRibbon">
        <div id="tertiaryLinkButtons">
            <asp:LinkButton ID="lbManTemp" runat="server" Text="Manage Templates" onclick="clickManageTemplates" />
        </div>
     </div>
     </asp:Panel>
     <asp:Panel ID="panelTR2" runat="server" Visible="false">
        <div id="tertiaryRibbon">
           <div id="tertiaryLinkButtons">
           <asp:LinkButton ID="lbStartOver2" runat="server" Text="Start Over" onclick="clickStartOver" />
           <asp:LinkButton ID="lbRecVids" runat="server" Text="Recommend Videos" onclick="recommendVids" />
            </div>
        </div>
     </asp:Panel>
     <asp:Panel ID="panelTR" runat="server" Visible="false">
        <div id="tertiaryRibbon">
           <div id="tertiaryLinkButtons">
           <asp:LinkButton ID="lbStartOver" runat="server" Text="Start Over" onclick="clickStartOver" />
           <asp:LinkButton ID="lbBack" runat="server" Text="Back To Results" onclick="clickBackToResults" />
           <asp:LinkButton ID="lbUpdateRV" runat="server" Text="Update List" onclick="clickUpdateList" />
           <asp:LinkButton ID="lbDeleteRV" runat="server" Text="Delete Selected" onclick="clickDeleteRV" />
           <asp:LinkButton ID="lbMarkComplete" runat="server" Text="Completed" onclick="clickVidComplete" />
           <asp:LinkButton ID="lbMarkIncomplete" runat="server" Text="Incomplete" onclick="clickVidIncomplete" />
           <asp:LinkButton ID="resetOrder" runat="server" Text="Reset Sort" onclick="clickResetSort" />
            </div>
        </div>
     </asp:Panel>
    <!-- Page -->
    <div id="page">
     <div id="searchMember" runat="server">
       <div style="float: left; margin-bottom: 12px;">
     <asp:Panel ID="panelSearchBox" runat="server">
     <!-- Search Box -->
     <h3>Find User to Recommend Videos</h3>
     <asp:TextBox ID="memberSearchText" runat="server" />
        <asp:RegularExpressionValidator ID="RegExpMemSearch" runat="server"
            ControlToValidate="memberSearchText"
        	ValidationExpression="^\s*[a-zA-Z0-9,\s,?,\,,',;,:,(,),!,@,.,*,#,\-,&]+\s*$"
        	ErrorMessage="You have entered characters that are not allowed."
        	Display="None" ValidationGroup="memberSearch" />
        <atk:ValidatorCalloutExtender ID="callOutRegExpMemberSearch" 
            runat="server" Enabled="True" TargetControlID="RegExpMemSearch"
            HighlightCssClass="fieldError">
        </atk:ValidatorCalloutExtender>&nbsp;&nbsp;
    <asp:DropDownList ID="memberSearchBy" runat="server" CssClass="dropdownmenu">
                                                <asp:ListItem Text="Email" value="[user].email" />
                                                <asp:ListItem Text="First Name" value="[user].firstName" />
                                                <asp:ListItem Text="Last Name" value="[user].lastName" />
                                                <asp:ListItem Text="Practice Name" value="[user].practiceName" />
                                                <asp:ListItem Text="Org ID" value="[user].renOrgID" />
                                                </asp:DropDownList>
                                                &nbsp;&nbsp;<asp:Button ID="memberSearchBtn" runat="server" OnClick="memberSearch" Text="Search" ValidationGroup="memberSearch" /><br />
        <!-- Search Errors / Notices / DB Errors -->
        </asp:Panel>
        </div>
        <div style="clear:both;"><asp:Label ID="searchError" runat="server" />
        <asp:Label ID="dbErrorMessage" runat="server" />
        <asp:Label ID="noticeMessage" runat="server" />
        </div>
        <!-- /Search Errors / Notices / DB Errors -->
        <!-- Selected user grid view -->
       <asp:Panel ID="panelSelectMember" runat="server">
            <asp:Panel ID="panelDelete" runat="server" Visible="false">
                <asp:Button ID="btnDeleteConfirm" runat="server" Text="Confirm Delete" onclick="clickConfirmDelete" />&nbsp;&nbsp;
                <asp:Button ID="btnDeleteCancel" runat="server" Text="Cancel" onclick="clickCancelDelete" />
            </asp:Panel>
       <asp:Label ID="labelTitleInfo" runat="server" /><br />
       <asp:GridView ID="recVidsMemberGrid" runat="server"
             AutoGenerateColumns="false"
             onRowEditing="recVidsMemberGrid_RowEditing"
             onRowCancelingEdit="recVidsMemberGrid_RowCancelingEdit"
             onRowUpdating="recVidsMemberGrid_RowUpdating"
             OnRowDeleting="recVidsMemberGrid_RowDeleting"
             AllowMultiRowEdit="True"
             datakeynames="recID">
                <Columns>
                     <asp:TemplateField>
                <HeaderTemplate>
                    <asp:CheckBox runat="server" ID="HeaderLevelCheckBoxM" AutoPostBack="true" OnCheckedChanged="HeaderLevelCheckBox_CheckedChangedM" Text="Select All" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:CheckBox runat="server" ID="RowLevelCheckBoxM" />
                </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Record ID" HeaderStyle-HorizontalAlign="Left"> 
                        <ItemTemplate>
                           <asp:Label ID="recordIDLbl" runat="server" Text='<%# Bind("recID") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                     <asp:TemplateField HeaderText="User ID" HeaderStyle-HorizontalAlign="Left"> 
                        <ItemTemplate>
                           <asp:Label ID="userIDLbl" runat="server" Text='<%# Bind("userID") %>'></asp:Label>
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
        	                      <atk:ValidatorCalloutExtender ID="ValidatorCalloutExtender1" 
                                     runat="server" Enabled="True" TargetControlID="regRecVidSortEdit"
                                     HighlightCssClass="fieldError">
                                  </atk:ValidatorCalloutExtender>
        	                      <asp:RegularExpressionValidator ID="regExpVidSortPriority" runat="server"
        	                        ControlToValidate="recVidSortEdit"
        	                        ValidationExpression="^\s*[a-zA-Z0-9&quot;'-/\s,\?,\,',;,:,!,@,.,#,\-,&,\),\(,\{,\},\[,\]]+\s*$"
        	                        ErrorMessage="You have entered characters that are not allowed."
        	                        Display="None" />
        	                      <atk:ValidatorCalloutExtender ID="ValidatorCalloutExtender2" 
                                    runat="server" Enabled="True" TargetControlID="regExpVidSortPriority"
                                    HighlightCssClass="fieldError">
                                  </atk:ValidatorCalloutExtender>
                        </ItemTemplate>
                    </asp:TemplateField>
                     <asp:TemplateField HeaderText="Created Date" HeaderStyle-HorizontalAlign="Left"> 
                        <ItemTemplate>
                           <asp:Label ID="createdDateLbl" runat="server" Text='<%# Bind("createdDate") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                     <asp:TemplateField HeaderText="Completed Date" HeaderStyle-HorizontalAlign="Left"> 
                        <ItemTemplate>
                           <asp:Label ID="completedDateLbl" runat="server" Text='<%# Bind("completedDate") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                 <AlternatingRowStyle cssClass="alternateRow" />
             </asp:GridView>
             </asp:Panel>
        <br />
         <asp:Panel ID="panelSearchResults" runat="server">
         <!-- Search member results -->
<asp:GridView ID="memberSearchGrid" runat="server"
            AutoGenerateColumns="false" AllowPaging="false"
            onSelectedIndexChanged = "memberSearchGrid_SelectedIndexChanged"
            onrowcommand="memberSearchGrid_RowCommand">
            <Columns>
             <asp:TemplateField>
            <HeaderTemplate>
                <asp:CheckBox runat="server" ID="HeaderLevelCheckBox" AutoPostBack="true" OnCheckedChanged="HeaderLevelCheckBox_CheckedChanged" Text="Select All" />
            </HeaderTemplate>
            <ItemTemplate>
                <asp:CheckBox runat="server" ID="RowLevelCheckBox" AutoPostBack="true" OnCheckedChanged="styleChange_onCheck" />
            </ItemTemplate>
        </asp:TemplateField>
                <asp:ButtonField commandName="viewRecVids" Text="View Videos" />
                <asp:BoundField DataField="userID" HeaderText="User ID" HeaderStyle-HorizontalAlign="Left" />
                <asp:BoundField DataField="renUserID" HeaderText="Ren User ID" HeaderStyle-HorizontalAlign="Left" />
                <asp:BoundField DataField="renOrgID" HeaderText="Org ID" HeaderStyle-HorizontalAlign="Left" />
                <asp:BoundField DataField="practiceName" HeaderText="Practice Name" HeaderStyle-HorizontalAlign="Left" />
                <asp:BoundField DataField="firstName" HeaderText="First Name" HeaderStyle-HorizontalAlign="Left" />
                <asp:BoundField DataField="lastName" HeaderText="Last Name" HeaderStyle-HorizontalAlign="Left" />
                <asp:BoundField DataField="email" HeaderText="Email" HeaderStyle-HorizontalAlign="Left" />
                <asp:BoundField DataField="contactPhone" HeaderText="Phone #" HeaderStyle-HorizontalAlign="Left" />
                <asp:BoundField DataField="name" HeaderText="Status" HeaderStyle-HorizontalAlign="Left" />
            </Columns>
             <AlternatingRowStyle CssClass="alternateRow" />
       </asp:GridView>
         </asp:Panel>
       <br />
        <asp:Panel ID="panelRecVids" runat="server" Visible="false">
        <asp:DropDownList ID="ddlApplyTemp" runat="server" AutoPostBack="true" 
                OnSelectedIndexChanged="ddlApplyTemplate_SelectedIndexChange" >
        	 </asp:DropDownList>
        <div id="recommendVidsDiv" runat="server">
            <br /><br />
        <div class="greyBkgrdDiv"><asp:CheckBox runat="server" ID="recVidCBLSelectAll" AutoPostBack="true" OnCheckedChanged="recommendVidCBL_CheckedChanged" Text="Select All" /></div>
       <asp:Label id="selectedTest" runat="server" />
       </div>
       <!-- /Search Results -->
    </div>
    <!-- Recommend Videos List -->
    <asp:CheckBoxList id="recommendVidCBL"
         cellPadding="3"
         repeatColumns="2"
         repeatDirection="vertical"
     runat="server"></asp:CheckBoxList>
     </asp:Panel>
    <!-- /Page -->
    </div>
<!-- /Page Wrapper -->
</div>
</asp:Content>

