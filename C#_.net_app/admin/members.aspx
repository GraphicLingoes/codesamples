<%@ Page Language="C#" MasterPageFile="~/admin/admin.master" AutoEventWireup="true" CodeFile="members.aspx.cs" Inherits="admin_members" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="bodyClass" Runat="Server">
    <body id="admin">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:ScriptManager ID="membersListScriptManager" runat="server" />
<!-- Page wrapper -->
<div id="pageWrapper">
    <!-- Page -->
    <div id="page">
    <div id="searchMember" runat="server">
       <div style="float: left; margin-bottom: 12px;">
     <asp:TextBox ID="memberSearchText" runat="server" />
                                             <asp:RegularExpressionValidator ID="RegExpMemSearch" runat="server"
        	                                                        ControlToValidate="memberSearchText"
        	                                                        ValidationExpression="^\s*[a-zA-Z0-9,\s,?,\,,',;,:,(,),!,@,.,*,#,\-,&]+\s*$"
        	                                                        ErrorMessage="You have entered characters that are not allowed."
        	                                                        Display="None" ValidationGroup="memberSearch" />
        	                                                    <cc1:ValidatorCalloutExtender ID="callOutRegExpMemberSearch" 
                                                                    runat="server" Enabled="True" TargetControlID="RegExpMemSearch"
                                                                    HighlightCssClass="fieldError">
                                                                </cc1:ValidatorCalloutExtender>&nbsp;&nbsp;
    <asp:DropDownList ID="memberSearchBy" runat="server" CssClass="dropdownmenu">
                                                <asp:ListItem Text="Email" value="[user].email" />
                                                <asp:ListItem Text="First Name" value="[user].firstName" />
                                                <asp:ListItem Text="Last Name" value="[user].lastName" />
                                                <asp:ListItem Text="Practice Name" value="[user].practiceName" />
                                                <asp:ListItem Text="Org ID" value="[user].renOrgID" />
                                                </asp:DropDownList>
                                                &nbsp;&nbsp;<asp:Button ID="memberSearchBtn" runat="server" OnClick="memberSearch" Text="Search" ValidationGroup="memberSearch" /><br />
         </div>
    </div>
    <div style="float: left;"><asp:ImageButton id="backToMembersIB" runat="server" ImageUrl="../Images/miniRibbon/back_32.png" onClick="backToMain" ToolTip="Back To List" /></div>
    <div style="float: left; margin: 9px 9px 21px 6px;"><asp:LinkButton id="backToMembersLB" runat="server" Text="Back To List" onClick="backToMain" /> </div>
    <div style="float: left;"><asp:ImageButton id="backToSearchIB" runat="server" ImageUrl="../Images/miniRibbon/back_32.png" onClick="backToSearch" ToolTip="Back To Search Results" /></div>
    <div style="float: left; margin: 9px 0px 21px 6px;"><asp:LinkButton id="backToSearchLB" runat="server" Text="Back To Search Results" onClick="backToSearch" /> </div>
    <div id="nextPrevDiv" runat="server">
    <div style="float:right; margin: 0px 0px 0px 0px;"><asp:Button ID="Btn_Next" runat="server" CommandName="Next" OnCommand="ChangePage" Text="Next" /></div>
    <div style="float:right; margin: 9px 12px 21px 0px; color:#006666;">Page:&nbsp;<asp:Label ID="lblCurrentPage" runat="server" />&nbsp;of&nbsp;<asp:Label ID="lblTotalPages" runat="server" /></div>
     <div style="float:right; margin: 0px 6px 0px 9px;"><asp:Button ID="Btn_Previous" CommandName="Previous" runat="server" OnCommand="ChangePage" Text="Previous" CssClass="formBtn" /></div>
    </div>
    <div style="float: right;"><h3><asp:Label ID="h3Title" runat="server" /></h3></div>
     <div style="clear:both;"><asp:Label ID="searchError" runat="server" />
        <asp:Label ID="dbErrorMessage" runat="server" />
        <asp:Label ID="noticeNoVideos" runat="server" />
        </div>
     <!-- Member Detail View -->
     <asp:DetailsView ID="memberAdminDetailView" runat="server" AutoGenerateRows="False"
            onmodechanging="memberAdminDetailView_ModeChanging"
            onitemupdating="memberAdminDetailView_ItemUpdating">
             <FieldHeaderStyle Width="110px" HorizontalAlign="right" BackColor="#ededed" CssClass="headerPad" />
             <Fields>
                <asp:CommandField ShowEditButton="True" ValidationGroup="Register" EditText="Edit Member Details" ButtonType="Button" ControlStyle-CssClass="formBtn" />
                    <asp:TemplateField HeaderText="Status">
                         <EditItemTemplate>
                            <asp:DropDownList ID="dropDownListStatusEdit" DataSelectedValue="name" DataValueField = "userStatusID" DataTextField = "name" runat="server" CssClass="dropdownmenu" />
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:TextBox ID="textBoxStatusInsert" runat="server" Text='<%# Bind("name") %>'></asp:TextBox>
                        </InsertItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="statusLabel" runat="server" Text='<%# Bind("name") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Permission Level">
                        <EditItemTemplate>
                            <asp:DropDownList ID="dropDownListPermissionEdit" DataSelectedValue="permissionLevel" DataValueField = "permissionID" DataTextField = "permissionLevel" runat="server" CssClass="dropdownmenu" />
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:TextBox ID="textBoxPermissionInsert" runat="server" Text='<%# Bind("permissionLevel") %>'></asp:TextBox>
                        </InsertItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="permissionLabel" runat="server" Text='<%# Bind("permissionLevel") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="First Name">
                        <EditItemTemplate>
                            <asp:TextBox ID="textBoxfNameEdit" runat="server" Text='<%# Bind("firstName") %>'></asp:TextBox>
                                <asp:RequiredFieldValidator ID="textBoxfNameEditReq" runat="server"
        	                        ControlToValidate="textBoxfNameEdit"
        	                        ErrorMessage="First Name is required."
        	                        SetFocusOnError="true" Display="None" ValidationGroup="Register" />
                                <cc1:ValidatorCalloutExtender ID="textBoxfNameEditReq_ValidatorCalloutExtender" 
                                    runat="server" Enabled="True" TargetControlID="textBoxfNameEditReq"
                                    HighlightCssClass="fieldError">
                                </cc1:ValidatorCalloutExtender>
                                <asp:RegularExpressionValidator ID="reqExpfName" runat="server"
        	                        ControlToValidate="textBoxfNameEdit"
        	                        ValidationExpression="^\s*[a-zA-Z0-9,\s,?,\,,',;,:,(,),_,!,@,.,#,\-,&]+\s*$"
        	                        ErrorMessage="You have entered illegal characters."
        	                        Display="None" ValidationGroup="Register" />
        	                    <cc1:ValidatorCalloutExtender ID="reqExpfName_ValidatorCalloutExtender" 
                                    runat="server" Enabled="True" TargetControlID="reqExpfName"
                                    HighlightCssClass="fieldError">
                                </cc1:ValidatorCalloutExtender>
                        </EditItemTemplate>
                       <InsertItemTemplate>
                            <asp:TextBox ID="textBoxfNameInsert" runat="server" Text='<%# Bind("firstName") %>'></asp:TextBox>
                       </InsertItemTemplate>
                       <ItemTemplate>
                            <asp:Label ID="fNameLabel" runat="server" Text='<%# Bind("firstName") %>'></asp:Label>
                       </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Last Name">
                        <EditItemTemplate>
                            <asp:TextBox ID="textBoxlNameEdit" runat="server" Text='<%# Bind("lastName") %>'></asp:TextBox>
                              <asp:RequiredFieldValidator ID="textBoxlNameEditReq" runat="server"
        	                        ControlToValidate="textBoxlNameEdit"
        	                        ErrorMessage="Last Name is required."
        	                        SetFocusOnError="true" Display="None" ValidationGroup="Register" />
                                <cc1:ValidatorCalloutExtender ID="textBoxlNameEditReq_ValidatorCalloutExtender" 
                                    runat="server" Enabled="True" TargetControlID="textBoxlNameEditReq"
                                    HighlightCssClass="fieldError">
                                </cc1:ValidatorCalloutExtender>
                                <asp:RegularExpressionValidator ID="reqExplName" runat="server"
        	                        ControlToValidate="textBoxlNameEdit"
        	                        ValidationExpression="^\s*[a-zA-Z0-9,\s,?,\,,',;,:,(,),_,!,@,.,#,\-,&]+\s*$"
        	                        ErrorMessage="You have entered illegal characters."
        	                        Display="None" ValidationGroup="Register" />
        	                    <cc1:ValidatorCalloutExtender ID="reqExplName_ValidatorCalloutExtender" 
                                    runat="server" Enabled="True" TargetControlID="reqExplName"
                                    HighlightCssClass="fieldError">
                                </cc1:ValidatorCalloutExtender>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:TextBox ID="textBoxlNameInsert" runat="server" Text='<%# Bind("lastName") %>'></asp:TextBox>
                        </InsertItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lNameLabel" runat="server" Text='<%# Bind("lastName") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Address 1">
                        <EditItemTemplate>
                            <asp:TextBox ID="textBoxAddress1Edit" runat="server" Text='<%# Bind("address1") %>'></asp:TextBox>
                                <asp:RegularExpressionValidator ID="reqExpAddress1" runat="server"
        	                        ControlToValidate="textBoxAddress1Edit"
        	                        ValidationExpression="^\s*[a-zA-Z0-9,\s,?,\,,',;,:,(,),_,!,@,.,#,\-,&]+\s*$"
        	                        ErrorMessage="You have enter illegal characters."
        	                        Display="None" ValidationGroup="Register" />
        	                    <cc1:ValidatorCalloutExtender ID="reqExpAddress1_ValidatorCalloutExtender" 
                                    runat="server" Enabled="True" TargetControlID="reqExpAddress1"
                                    HighlightCssClass="fieldError">
                                </cc1:ValidatorCalloutExtender>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:TextBox ID="textBoxAddress1Insert" runat="server" Text='<%# Bind("address1") %>'></asp:TextBox>
                        </InsertItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="address1Label" runat="server" Text='<%# Bind("address1") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Address 2">
                        <EditItemTemplate>
                            <asp:TextBox ID="textBoxAddress2Edit" runat="server" Text='<%# Bind("address2") %>'></asp:TextBox>
                            <asp:RegularExpressionValidator ID="reqExpAddress2" runat="server"
        	                        ControlToValidate="textBoxAddress2Edit"
        	                        ValidationExpression="^\s*[a-zA-Z0-9,\s,?,\,,',;,:,(,),_,!,@,.,#,\-,&]+\s*$"
        	                        ErrorMessage="You have enter illegal characters."
        	                        Display="None" ValidationGroup="Register" />
        	                    <cc1:ValidatorCalloutExtender ID="reqExpAddress2_ValidatorCalloutExtender" 
                                    runat="server" Enabled="True" TargetControlID="reqExpAddress2"
                                    HighlightCssClass="fieldError">
                                </cc1:ValidatorCalloutExtender>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:TextBox ID="textBoxAddress2Insert" runat="server" Text='<%# Bind("address2") %>'></asp:TextBox>
                        </InsertItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="address2Label" runat="server" Text='<%# Bind("address2") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="City">
                        <EditItemTemplate>
                            <asp:TextBox ID="textBoxCityEdit" runat="server" Text='<%# Bind("city") %>'></asp:TextBox>
                            <asp:RegularExpressionValidator ID="reqExpCity" runat="server"
        	                        ControlToValidate="textBoxCityEdit"
        	                        ValidationExpression="^\s*[a-zA-Z0-9,\s,?,\,,',;,:,(,),_,!,@,.,#,\-,&]+\s*$"
        	                        ErrorMessage="You have enter illegal characters."
        	                        Display="None" ValidationGroup="Register" />
        	                    <cc1:ValidatorCalloutExtender ID="reqExpCity_ValidatorCalloutExtender" 
                                    runat="server" Enabled="True" TargetControlID="reqExpCity"
                                    HighlightCssClass="fieldError">
                                </cc1:ValidatorCalloutExtender>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:TextBox ID="textBoxCityInsert" runat="server" Text='<%# Bind("city") %>'></asp:TextBox>
                        </InsertItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="cityLabel" runat="server" Text='<%# Bind("city") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="State">
                        <EditItemTemplate>
                            <asp:DropDownList ID="dropDownListStateEdit" DataSelectedValue="state" DataValueField = "abbr" DataTextField = "name"; runat="server" CssClass="dropdownmenu" />
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:TextBox ID="textBoxStateInsert" runat="server" Text='<%# Bind("state") %>'></asp:TextBox>
                        </InsertItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="stateLabel" runat="server" Text='<%# Bind("state") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Zip">
                        <EditItemTemplate>
                            <asp:TextBox ID="textBoxZipCodeEdit" runat="server" Text='<%# Bind("zipCode") %>'></asp:TextBox>
                            <asp:RegularExpressionValidator ID="reqExpState" runat="server"
        	                        ControlToValidate="textBoxZipCodeEdit"
        	                        ValidationExpression="^\s*[a-zA-Z0-9,\s,?,\,,',;,:,(,),_,!,@,.,#,\-,&]+\s*$"
        	                        ErrorMessage="You have enter illegal characters."
        	                        Display="None" ValidationGroup="Register" />
        	                    <cc1:ValidatorCalloutExtender ID="reqExpState_ValidatorCalloutExtender" 
                                    runat="server" Enabled="True" TargetControlID="reqExpState"
                                    HighlightCssClass="fieldError">
                                </cc1:ValidatorCalloutExtender>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:TextBox ID="textBoxZipCodeInsert" runat="server" Text='<%# Bind("zipCode") %>'></asp:TextBox>
                        </InsertItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="zipCodeLabel" runat="server" Text='<%# Bind("zipCode") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Email">
                        <EditItemTemplate>
                            <asp:TextBox ID="textBoxEmailEdit" runat="server" Text='<%# Bind("email") %>'></asp:TextBox>
                             <asp:RegularExpressionValidator ID="reqExpEmail" runat="server"
        	                        ControlToValidate="textBoxEmailEdit"
        	                        ValidationExpression="^\s*[a-zA-Z0-9,\s,?,\,,',;,:,(,),_,!,@,.,#,\-,&]+\s*$"
        	                        ErrorMessage="You have enter illegal characters."
        	                        Display="None" ValidationGroup="Register" />
        	                    <cc1:ValidatorCalloutExtender ID="reqExpEmail_ValidatorCalloutExtender" 
                                    runat="server" Enabled="True" TargetControlID="reqExpEmail"
                                    HighlightCssClass="fieldError">
                                </cc1:ValidatorCalloutExtender>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:TextBox ID="textBoxEmailInsert" runat="server" Text='<%# Bind("email") %>'></asp:TextBox>
                        </InsertItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="emailLabel" runat="server" Text='<%# Bind("email") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Phone">
                        <EditItemTemplate>
                            <asp:TextBox ID="textBoxPhoneEdit" runat="server" Text='<%# Bind("contactPhone") %>'></asp:TextBox>
                             <asp:RegularExpressionValidator ID="reqExpPhone" runat="server"
        	                        ControlToValidate="textBoxPhoneEdit"
        	                        ValidationExpression="^\s*[a-zA-Z0-9,\s,?,\,,',;,:,(,),_,!,@,.,#,\-,&]+\s*$"
        	                        ErrorMessage="You have enter illegal characters."
        	                        Display="None" ValidationGroup="Register" />
        	                    <cc1:ValidatorCalloutExtender ID="reqExpPhone_ValidatorCalloutExtender" 
                                    runat="server" Enabled="True" TargetControlID="reqExpPhone"
                                    HighlightCssClass="fieldError">
                                </cc1:ValidatorCalloutExtender>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:TextBox ID="textBoxPhoneInsert" runat="server" Text='<%# Bind("contactPhone") %>'></asp:TextBox>
                        </InsertItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="phoneLabel" runat="server" Text='<%# Bind("contactPhone") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Practice Name">
                        <EditItemTemplate>
                            <asp:TextBox ID="textBoxPracticeNameEdit" runat="server" Text='<%# Bind("practiceName") %>'></asp:TextBox>
                             <asp:RegularExpressionValidator ID="reqExpPractice" runat="server"
        	                        ControlToValidate="textBoxPracticeNameEdit"
        	                        ValidationExpression="^\s*[a-zA-Z0-9,\s,?,\,,',;,:,(,),_,!,@,.,#,\-,&]+\s*$"
        	                        ErrorMessage="You have enter illegal characters."
        	                        Display="None" ValidationGroup="Register" />
        	                    <cc1:ValidatorCalloutExtender ID="reqExpPractice_ValidatorCalloutExtender" 
                                    runat="server" Enabled="True" TargetControlID="reqExpPractice"
                                    HighlightCssClass="fieldError">
                                </cc1:ValidatorCalloutExtender>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:TextBox ID="textBoxPracticeNameInsert" runat="server" Text='<%# Bind("practiceName") %>'></asp:TextBox>
                        </InsertItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="practiceNameLabel" runat="server" Text='<%# Bind("practiceName") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Title">
                        <EditItemTemplate>
                            <asp:TextBox ID="textBoxTitleEdit" runat="server" Text='<%# Bind("title") %>'></asp:TextBox>
                              <asp:RegularExpressionValidator ID="reqExpTitle" runat="server"
        	                        ControlToValidate="textBoxTitleEdit"
        	                        ValidationExpression="^\s*[a-zA-Z0-9,\s,?,\,,',;,:,(,),_,!,@,.,#,\-,&]+\s*$"
        	                        ErrorMessage="You have enter illegal characters."
        	                        Display="None" ValidationGroup="Register" />
        	                    <cc1:ValidatorCalloutExtender ID="reqExpTitle_ValidatorCalloutExtender" 
                                    runat="server" Enabled="True" TargetControlID="reqExpTitle"
                                    HighlightCssClass="fieldError">
                                </cc1:ValidatorCalloutExtender>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:TextBox ID="textBoxTitleInsert" runat="server" Text='<%# Bind("title") %>'></asp:TextBox>
                        </InsertItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="titleLabel" runat="server" Text='<%# Bind("title") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Created Date">
                        <InsertItemTemplate>
                            <asp:TextBox ID="textBoxCreatedDate" runat="server" Text='<%# Bind("createdDate") %>'></asp:TextBox>
                        </InsertItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="CreatedDateLabel" runat="server" Text='<%# Bind("createdDate") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Fields>
                <HeaderTemplate>
                    <%#Eval("firstName") %> <%#Eval("lastName") %>
                </HeaderTemplate>
                <AlternatingRowStyle cssClass="alternateRow" />
        </asp:DetailsView>
     <asp:GridView ID="memberAdminGridView" runat="server"
       onSelectedIndexChanged = "memberAdminDetailView_SelectedIndexChanged">
           <Columns>
                <asp:ButtonField CommandName="Select" Text="Member Detail" />
            </Columns> 
            <AlternatingRowStyle CssClass="alternateRow" />
       </asp:GridView>
<asp:GridView ID="memberSearchGrid" runat="server"
            AutoGenerateColumns="false" AllowPaging="false"
            onSelectedIndexChanged = "memberSearchGrid_SelectedIndexChanged"
            onrowcommand="memberSearchGrid_RowCommand">
            <Columns>
                <asp:ButtonField CommandName="viewMemberDetail" Text="Member Detail" />
                <asp:ButtonField CommandName="videoHistory" Text="View Video History" />
                <asp:ButtonField CommandName="viewImpSteps" Text="View Completed Imp Steps" />
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
<!-- End Members List -->
<!-- Search member results -->
 <!-- Start completed implementation steps -->
     <br />
     <asp:GridView ID="completedImpGrid" runat="server"
            AutoGenerateColumns="false" AllowPaging="false">
              <Columns>
                <asp:BoundField DataField="Priority" HeaderText="Sort Order" HeaderStyle-HorizontalAlign="Left" />
                <asp:BoundField DataField="impStepID" HeaderText="ID" HeaderStyle-HorizontalAlign="Left" Visible="false" />
                <asp:BoundField DataField="impStepName" HeaderText="Implementation Step" HeaderStyle-HorizontalAlign="Left" />
                <asp:BoundField DataField="Description" HeaderText="Description" HeaderStyle-HorizontalAlign="Left" />
                <asp:BoundField DataField="createdDate" HeaderText="Date Completed" HeaderStyle-HorizontalAlign="Left" />
            </Columns>
            <AlternatingRowStyle CssClass="alternateRow" />
     </asp:GridView>
 <!-- End completed implemenation steps -->
 <!-- Watched videos detail grid -->
 <asp:GridView ID="gridHistory" runat="server" AllowPaging="false" AutoGenerateColumns="false" >
    <Columns>
    <asp:BoundField HeaderText="Video Name" DataField="videoName" />
    <asp:BoundField HeaderText="Date Viewed" DataField="createdDate" />
    </Columns>
    <AlternatingRowStyle CssClass="alternateRow" />
    </asp:GridView>
 <!-- Start Watched Video Grid -->
    <asp:GridView ID="watchedVideoGrid" runat="server" AllowPaging="false" AutoGenerateColumns="false" onSelectedIndexChanged = "watchedVideoGrid_SelectedIndexChanged" >
    <Columns>
    <asp:BoundField HeaderText="Video Name" DataField="videoName" />
    <asp:BoundField HeaderText="Description" DataField="description" />
    <asp:BoundField HeaderText="Lenght" DataField="lenght" />
    <asp:BoundField HeaderText="Number of Views" DataField="videoCount" />
    <asp:ButtonField HeaderText="Viewing History" CommandName="select" Text="Details" />
    </Columns>
    <AlternatingRowStyle CssClass="alternateRow" />
    </asp:GridView><br />
    <!-- End watched videos Grid -->  
    <!-- /Page -->
    </div>
<!-- /Page wrapper -->
</div> 
</asp:Content>

