using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Data.SqlClient;
using System.IO;
using System.Xml;
using System.Text;
using System.Security.Cryptography;
using System.Reflection;

public partial class admin_members : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //Bind Members Grid View
            BindMemberAdminGrid();
            // Apply H3 title to list
            h3Title.Text = "Members List";
            // Handle mini ribbon color change if menu item active
            LinkButton mpLinkButtonM = (LinkButton)Master.FindControl("membersLB");
            mpLinkButtonM.ForeColor = System.Drawing.ColorTranslator.FromHtml("#9CC5C9");

            LinkButton mpLinkButtonV = (LinkButton)Master.FindControl("videosLB");
            mpLinkButtonV.ForeColor = System.Drawing.Color.White;

            LinkButton mpLinkButtonIS = (LinkButton)Master.FindControl("helpPagesLB");
            mpLinkButtonIS.ForeColor = System.Drawing.Color.White;

            LinkButton mpLinkButtonRC = (LinkButton)Master.FindControl("recVidsLB");
            mpLinkButtonRC.ForeColor = System.Drawing.Color.White;

            LinkButton mpLinkButtonS = (LinkButton)Master.FindControl("searchLB");
            mpLinkButtonS.ForeColor = System.Drawing.Color.White;

            // Hide or show objects
            backToMembersIB.Visible = false;
            backToMembersLB.Visible = false;
            backToSearchIB.Visible = false;
            backToSearchLB.Visible = false;

        }

        // Default Search Button
        Page.Form.DefaultButton = memberSearchBtn.UniqueID;

    }

    // Handle back to search button
    protected void backToSearch(object sender, EventArgs e)
    {
        backToSearchIB.Visible = false;
        backToSearchLB.Visible = false;
        memberSearchGrid.Visible = true;
        memberAdminDetailView.Visible = false;
    }

    // Handle back button
    protected void backToMain(object sender, EventArgs e)
    {
        Response.Redirect("members.aspx");
    }

    // Custom Paging using stored procedure

    protected int currentPageNumber = 1;
    private const int PAGE_SIZE = 10;

    private int CalculateTotalPages(double totalRows)
    {
        int totalPages = (int)Math.Ceiling(totalRows / PAGE_SIZE);
        return totalPages;
    }

    private void BindMemberAdminGrid()
    {
        SqlConnection conn;
        SqlCommand comm;
        string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
        conn = new SqlConnection(connectionString);
        comm = new SqlCommand("sp_GetMembersAdmin", conn);
        comm.CommandType = CommandType.StoredProcedure;
        comm.Parameters.AddWithValue("@startRowIndex", currentPageNumber);
        comm.Parameters.AddWithValue("@maximumRows", PAGE_SIZE);
        comm.Parameters.Add("@totalRows", SqlDbType.Int, 4);
        comm.Parameters["@totalRows"].Direction = ParameterDirection.Output;

        SqlDataAdapter ad = new SqlDataAdapter(comm);
        DataSet ds = new DataSet();
        ad.Fill(ds);

        memberAdminGridView.DataSource = ds;
        memberAdminGridView.DataKeyNames = new string[] { "User ID" };
        memberAdminGridView.DataBind();

        //get total rows
        double totalRows = (int)comm.Parameters["@totalRows"].Value;

        lblTotalPages.Text = CalculateTotalPages(totalRows).ToString();
        lblCurrentPage.Text = currentPageNumber.ToString();

        if (currentPageNumber == 1)
        {
            Btn_Previous.Enabled = false;
            if (Int32.Parse(lblTotalPages.Text) > 0)
            {
                Btn_Next.Enabled = true;
            }
            else
            {
                Btn_Next.Enabled = false;
            }
        }
        else
        {
            Btn_Previous.Enabled = true;
            if (currentPageNumber == Int32.Parse(lblTotalPages.Text))
            {
                Btn_Next.Enabled = false;
            }
            else
            {
                Btn_Next.Enabled = true;
            }
        }

    }

    // Method to handle the navigation / paging index
    protected void ChangePage(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "Previous":
                currentPageNumber = Int32.Parse(lblCurrentPage.Text) - 1;
                break;
            case "Next":
                currentPageNumber = Int32.Parse(lblCurrentPage.Text) + 1;
                break;
        }
        BindMemberAdminGrid();
    }

    // Bind Details to memberAdminDetailView when member selected
    protected void memberAdminDetailView_SelectedIndexChanged(object sender, EventArgs e)
    {
        searchError.Visible = false;
        int selectRowIndex = memberAdminGridView.SelectedIndex;
        int userID = (int)memberAdminGridView.DataKeys[selectRowIndex].Value;
        Session["adminSelectUserID"] = userID.ToString();
        BindmemberDetails();
        memberAdminGridView.Visible = false;
        h3Title.Text = "Member Detail View";
        // Hide or show objects
        backToMembersIB.Visible = true;
        backToMembersLB.Visible = true;
        nextPrevDiv.Visible = false;
        searchMember.Visible = false;
    }
    private void BindmemberDetails()
    {
        memberAdminDetailView.Visible = true;
        int userID = Convert.ToInt32(Session["adminSelectUserID"]);
        SqlConnection conn;
        SqlCommand comm;
        SqlDataReader reader;
        string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
        conn = new SqlConnection(connectionString);
        comm = new SqlCommand("SELECT[user].userID, permission.permissionLevel, [user].firstName, [user].lastName, [user].address1, [user].address2, " +
                                "[user].city, [user].state, [user].zipCode, [user].email, [user].contactPhone, [user].practiceName, " +
                                "[user].title, Replace(Convert(VarChar(10), [user].createdDate, 101), '.', '/') AS createdDate, userStatus.userStatusID, userStatus.name FROM permission " +
                                "JOIN [user] ON permission.permissionID = [user].permissionID " +
                                "JOIN [userStatus] ON [user].userStatusID = [userStatus].userStatusID " +
                                "WHERE userID=@userID", conn);
        comm.Parameters.Add("userID", SqlDbType.Int);
        comm.Parameters["userID"].Value = userID;
        try
        {
            conn.Open();
            reader = comm.ExecuteReader();
            memberAdminDetailView.DataSource = reader;
            memberAdminDetailView.DataKeyNames = new string[] { "userID" };
            memberAdminDetailView.DataBind();
            reader.Close();
        }
        finally
        {
            conn.Close();
        }
    }


    // *******************************************************************Handle the edit event and load states drop down menu in edit mode.
    protected void memberAdminDetailView_ModeChanging(object sender, DetailsViewModeEventArgs e)
    {
        if (!e.CancelingEdit)
        {
            memberAdminDetailView.ChangeMode(e.NewMode);
            BindmemberDetails();
            int userID = Convert.ToInt32(Session["adminSelectUserID"]);
            Session["adminSelectUserID"] = userID.ToString();
            SqlConnection conn;
            SqlCommand statusComm;
            SqlCommand permissionComm;
            SqlCommand statesComm;
            SqlDataReader reader;
            string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
            conn = new SqlConnection(connectionString);
            statusComm = new SqlCommand("SELECT userStatusID, name FROM userStatus", conn);
            statusComm.Parameters.Add("userID", SqlDbType.Int);
            statusComm.Parameters["userID"].Value = userID;
            permissionComm = new SqlCommand("SELECT permissionID, permissionLevel FROM permission", conn);
            permissionComm.Parameters.Add("userID", SqlDbType.Int);
            permissionComm.Parameters["userID"].Value = userID;
            statesComm = new SqlCommand("SELECT abbr, name FROM us_postal_codes", conn);
            statesComm.Parameters.Add("userID", SqlDbType.Int);
            statesComm.Parameters["userID"].Value = userID;

            try
            {
                conn.Open();
                //Bind status table to edit mode of memberAdminDetailView
                reader = statusComm.ExecuteReader();
                DropDownList cboStatus = (DropDownList)memberAdminDetailView.FindControl("dropDownListStatusEdit");
                cboStatus.DataSource = reader;
                cboStatus.DataValueField = "userStatusID";
                cboStatus.DataTextField = "name";
                cboStatus.DataBind();
                reader.Close();
                //Bind permission table to edit mode of memberAdminDetailView
                reader = permissionComm.ExecuteReader();
                DropDownList cboPermission = (DropDownList)memberAdminDetailView.FindControl("dropDownListPermissionEdit");
                cboPermission.DataSource = reader;
                cboPermission.DataValueField = "permissionID";
                cboPermission.DataTextField = "permissionLevel";
                cboPermission.DataBind();
                reader.Close();
                //Bind states table to edit mode of memberAdminDetailView
                reader = statesComm.ExecuteReader();
                DropDownList cboState = (DropDownList)memberAdminDetailView.FindControl("dropDownListStateEdit");
                cboState.DataSource = reader;
                cboState.DataValueField = "abbr";
                cboState.DataTextField = "name";
                cboState.DataBind();
                reader.Close();
                conn.Close();
                //Create Queries to find exact data for Status, Permission and State for unique user
                statusComm = new SqlCommand("SELECT userStatusID FROM [user] WHERE userID=" + userID, conn);
                permissionComm = new SqlCommand("SELECT permissionID FROM [user] WHERE userID=" + userID, conn);
                statesComm = new SqlCommand("SELECT [state] FROM [user] WHERE userID=" + userID, conn);
                conn.Open();
                // Bind unique informatin to Status for user
                reader = statusComm.ExecuteReader();
                reader.Read();
                cboStatus.SelectedValue = reader["userStatusID"].ToString();
                reader.Close();
                // Bind unique informatin to Permission for user
                reader = permissionComm.ExecuteReader();
                reader.Read();
                cboPermission.SelectedValue = reader["permissionID"].ToString();
                reader.Close();
                // Bind unique informatin to State for user
                reader = statesComm.ExecuteReader();
                reader.Read();
                cboState.SelectedValue = reader["state"].ToString();
                reader.Close();

            }
            catch (SqlException ex)
            {

                dbErrorMessage.Text =
                    "<div class=\"errorMessage\">The following Error has occurred:<br /> [" +

ex.Message + "]</div>";
            }
            finally
            {
                conn.Close();
                h3Title.Text = "Edit Details";
            }
        }
        else
        {
            memberAdminDetailView.ChangeMode(DetailsViewMode.ReadOnly);
            BindmemberDetails();
            BindMemberAdminGrid();
            h3Title.Text = "Member Detail View";
        }
    }


    //********************************************************************************************** Handle update members event.
    protected void memberAdminDetailView_ItemUpdating(object sender, DetailsViewUpdateEventArgs e)
    {
        int userID = Convert.ToInt32(Session["adminSelectUserID"]);
        DropDownList newStatusDropDownList = (DropDownList)memberAdminDetailView.FindControl("dropDownListStatusEdit");
        string newStatus = newStatusDropDownList.Text;
        DropDownList newPermissionDropDownList = (DropDownList)memberAdminDetailView.FindControl("dropDownListPermissionEdit");
        string newPermission = newPermissionDropDownList.Text;
        TextBox newFirstNameTextBox = (TextBox)memberAdminDetailView.FindControl("textBoxfNameEdit");
        string newFirstName = newFirstNameTextBox.Text;
        TextBox newLastNameTextBox = (TextBox)memberAdminDetailView.FindControl("textBoxlNameEdit");
        string newLastName = newLastNameTextBox.Text;
        TextBox newAddress1TextBox = (TextBox)memberAdminDetailView.FindControl("textBoxAddress1Edit");
        string newAddress1 = newAddress1TextBox.Text;
        TextBox newAddress2TextBox = (TextBox)memberAdminDetailView.FindControl("textBoxAddress2Edit");
        string newAddress2 = newAddress2TextBox.Text;
        TextBox newCityTextBox = (TextBox)memberAdminDetailView.FindControl("textBoxCityEdit");
        string newCity = newCityTextBox.Text;
        DropDownList newStateDropDownList = (DropDownList)memberAdminDetailView.FindControl("dropDownListStateEdit");
        string newState = newStateDropDownList.Text;
        TextBox newZipCodeTextBox = (TextBox)memberAdminDetailView.FindControl("textBoxZipCodeEdit");
        string newZipCode = newZipCodeTextBox.Text;
        TextBox newEmailTextBox = (TextBox)memberAdminDetailView.FindControl("textBoxEmailEdit");
        string newEmail = newEmailTextBox.Text;
        TextBox newPhoneTextBox = (TextBox)memberAdminDetailView.FindControl("textBoxPhoneEdit");
        string newPhone = newPhoneTextBox.Text;
        TextBox newPracticeNameTextBox = (TextBox)memberAdminDetailView.FindControl("textBoxPracticeNameEdit");
        string newPracticeName = newPracticeNameTextBox.Text;
        TextBox newTitleTextBox = (TextBox)memberAdminDetailView.FindControl("textBoxTitleEdit");
        string newTitle = newTitleTextBox.Text;
        SqlConnection conn;
        SqlCommand comm;
        string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
        conn = new SqlConnection(connectionString);
        comm = new SqlCommand("updateAdminMemberDetails", conn);
        comm.CommandType = CommandType.StoredProcedure;
        comm.Parameters.Add("@userID", SqlDbType.Int);
        comm.Parameters["@userID"].Value = userID;
        comm.Parameters.Add("@NewuserStatusID", System.Data.SqlDbType.Int);
        comm.Parameters["@NewuserStatusID"].Value = newStatus;
        comm.Parameters.Add("@NewpermissionID", System.Data.SqlDbType.Int);
        comm.Parameters["@NewpermissionID"].Value = newPermission;
        comm.Parameters.Add("@NewfirstName", SqlDbType.NVarChar, 100);
        comm.Parameters["@NewfirstName"].Value = newFirstName;
        comm.Parameters.Add("@NewlastName", System.Data.SqlDbType.NVarChar, 100);
        comm.Parameters["@NewlastName"].Value = newLastName;
        comm.Parameters.Add("@Newaddress1", System.Data.SqlDbType.NVarChar, 100);
        comm.Parameters["@Newaddress1"].Value = newAddress1;
        comm.Parameters.Add("@Newaddress2", System.Data.SqlDbType.NVarChar, 100);
        comm.Parameters["@Newaddress2"].Value = newAddress2;
        comm.Parameters.Add("@Newcity", System.Data.SqlDbType.NVarChar, 60);
        comm.Parameters["@Newcity"].Value = newCity;
        comm.Parameters.Add("@Newstate", System.Data.SqlDbType.NVarChar, 2);
        comm.Parameters["@Newstate"].Value = newState;
        comm.Parameters.Add("@NewzipCode", System.Data.SqlDbType.NVarChar, 10);
        comm.Parameters["@NewzipCode"].Value = newZipCode;
        comm.Parameters.Add("@Newemail", System.Data.SqlDbType.NVarChar, 100);
        comm.Parameters["@Newemail"].Value = newEmail;
        comm.Parameters.Add("@NewcontactPhone", System.Data.SqlDbType.NVarChar, 20);
        comm.Parameters["@NewcontactPhone"].Value = newPhone;
        comm.Parameters.Add("@NewpracticeName", System.Data.SqlDbType.NVarChar, 200);
        comm.Parameters["@NewpracticeName"].Value = newPracticeName;
        comm.Parameters.Add("@Newtitle", System.Data.SqlDbType.NVarChar, 50);
        comm.Parameters["@Newtitle"].Value = newTitle;
        try
        {
            conn.Open();
            comm.ExecuteNonQuery();
        }
        finally
        {
            conn.Close();
        }
        memberAdminDetailView.ChangeMode(DetailsViewMode.ReadOnly);
        BindmemberDetails();
        // Clear view state to reload member's grid after member update.
        ViewState["MembersDataSet"] = null;
        BindMemberAdminGrid();
        h3Title.Text = "Member Detail View";

    }
    // Handle Search btn
    protected void memberSearch(object sender, EventArgs e)
    {
        if (memberSearchText.Text == "@" || memberSearchText.Text == ".com")
        {
            searchError.Visible = true;
            searchError.Text = "<div class='errorMessageSearch'>You can not search using only the @ symbol or the phrase \".com\". Please try again.</div>";
        }
        else
        {
            bindSearchGrid();
            h3Title.Text = "Member Search Results";
        }

    }

    // Function to bind search grid
    private void bindSearchGrid()
    {
        if (IsPostBack && memberSearchText.Text != "")
        {
            noticeNoVideos.Visible = false;
            watchedVideoGrid.Visible = false;
            gridHistory.Visible = false;
            string tableField = memberSearchBy.SelectedValue.ToString();
            searchError.Visible = false;
            SqlConnection conn;
            SqlCommand comm;
            SqlDataReader reader;
            string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
            conn = new SqlConnection(connectionString);
            comm = new SqlCommand("SELECT[user].userID, min([user].renUserID) AS renUserID, min([user].renOrgID) AS renOrgID, min(permission.permissionLevel) AS permissionLevel, min([user].firstName) AS firstName, min([user].lastName) AS lastName, min([user].address1) AS address1, min([user].address2) AS address2, " +
                                "min([user].city) AS city, min([user].state) AS state, min([user].zipCode) AS zipCode, min([user].email) AS email, min([user].contactPhone) AS contactPhone, min([user].practiceName) AS practiceName, " +
                                "min([user].title) AS title, min([user].createdDate) AS createdDate, min(userStatus.userStatusID) AS userStatusID, min(userStatus.name) AS name FROM permission " +
                                "JOIN [user] ON permission.permissionID = [user].permissionID " +
                                "JOIN [userStatus] ON [user].userStatusID = [userStatus].userStatusID " +
                                "WHERE " + tableField + " LIKE '%' + @searchBox + '%' GROUP BY [user].userID", conn);
            comm.Parameters.Add("@searchBox", SqlDbType.NVarChar, 100);
            comm.Parameters["@searchBox"].Value = memberSearchText.Text;

            try
            {

                DataTable dt = new DataTable();
                memberSearchGrid.DataSource = dt;
                memberSearchGrid.DataKeyNames = new string[] { "userID" };
                memberSearchGrid.DataBind();
                conn.Open();
                reader = comm.ExecuteReader();
                if (reader.HasRows)
                {
                    memberSearchGrid.DataSource = reader;
                    memberSearchGrid.DataBind();
                    reader.Close();
                    memberAdminGridView.Visible = false;
                    searchMember.Visible = false;
                    backToMembersIB.Visible = true;
                    backToMembersLB.Visible = true;
                    nextPrevDiv.Visible = false;
                }
                else
                {
                    searchError.Visible = true;
                    nextPrevDiv.Visible = true;
                    backToMembersLB.Visible = false;
                    backToMembersIB.Visible = false;
                    searchMember.Visible = true;
                    searchError.Text = "<div class='searchNotice'>Your search did not return any members. Please try again. TIP: " +
                        "You may want to try using part of their name or email address only.<br />You can also page through the list " +
                        "of members below.</div>";
                    reader.Close();
                }
            }
            catch (SqlException ex)
            {
                searchError.Visible = true;
                nextPrevDiv.Visible = true;
                backToMembersLB.Visible = false;
                backToMembersIB.Visible = false;
                searchMember.Visible = true;
                searchError.Text =
                    "<div class=\"errorMessageSearch\">There has been an error:<br /> [" + ex.Message + " " + comm + " ]</div>";
            }

            finally
            {
                conn.Close();
            }

        }
        else
        {
            searchError.Visible = true;
            nextPrevDiv.Visible = true;
            backToMembersLB.Visible = false;
            backToMembersIB.Visible = false;
            searchMember.Visible = true;
            searchError.Text = "<div class='errorMessageSearch'>Please enter something in the <b>\"Search For\"</b> field and try your search again.</div>";
        }
    }

    // Handle binding info to detail view when selected from search results.
    protected void memberSearchGrid_SelectedIndexChanged(object sender, EventArgs e)
    {
        
    }

    // Handle binding info to history when details link clicked from watchedVideoGrid.
    protected void watchedVideoGrid_SelectedIndexChanged(object sender, EventArgs e)
    {
        bindHistory();
    }

    // Bind watched video grid.
    private void bindWatchedVideos(int userID)
    {
        completedImpGrid.Visible = false;
        watchedVideoGrid.Visible = true;
        gridHistory.Visible = false;
        Session["adminSelectUserID"] = userID.ToString();
        SqlConnection conn;
        SqlCommand watchedComm;
        SqlDataReader reader;
        string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
        conn = new SqlConnection(connectionString);
        watchedComm = new SqlCommand("SELECT COUNT(videoTracking.videoInfoID) AS videoCount, MIN(videoTracking.videoInfoID) AS videoInfoID, MIN(videoTracking.userID) AS userID, MIN(Replace(Convert(VarChar(10), videoTracking.createdDate, 101), '.', '/')) AS createdDate, MIN(videoInfo.videoName) AS videoName, " +
                            "MIN(videoInfo.[description]) AS description, MIN(videoInfo.lenght) AS lenght FROM videoInfo " +
                            "JOIN videoTracking ON videoInfo.videoInfoID = videoTracking.videoInfoID " +
                            "WHERE videoTracking.userID=" + userID + "GROUP BY videoTracking.videoInfoID ORDER BY videoName ASC", conn);
        try
        {

            conn.Open();
            reader = watchedComm.ExecuteReader();
            if (reader.HasRows)
            {
                watchedVideoGrid.Visible = true;
                noticeNoVideos.Visible = false;
                watchedVideoGrid.DataKeyNames = new string[] { "videoInfoID" };
                watchedVideoGrid.DataSource = reader;
                watchedVideoGrid.DataBind();
                reader.Close();
            }
            else
            {
                noticeNoVideos.Visible = true;
                watchedVideoGrid.Visible = false;
                noticeNoVideos.Text = "<div class='searchNotice'>Member has not watched any videos.</div>";
                reader.Close();
            }

        }
        catch (SqlException ex)
        {

            dbErrorMessage.Text =
                "<div class=\"errorMessage\">Error submitting your registration try again later, and/or change the entered data.<br /> [" + ex.Message + "]</div>";
        }
        finally
        {
            conn.Close();

        }
    }
    // Bind watched video history to see date video watched.
    private void bindHistory()
    {
        gridHistory.Visible = true;
        int selectRowIndex = watchedVideoGrid.SelectedIndex;
        int videoInfoID = (int)watchedVideoGrid.DataKeys[selectRowIndex].Value;
        string userID = Session["adminSelectUserID"].ToString();
        SqlConnection conn;
        SqlCommand watchedComm;
        SqlDataReader reader;
        string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
        conn = new SqlConnection(connectionString);
        watchedComm = new SqlCommand("SELECT videoTracking.videoInfoID, videoTracking.userID, videoInfo.videoName, " +
                            "videoInfo.[description], videoInfo.lenght, Replace(Convert(VarChar(10), videoTracking.createdDate, 101), '.', '/') AS createdDate FROM videoInfo " +
                            "JOIN videoTracking ON videoInfo.videoInfoID = videoTracking.videoInfoID " +
                            "WHERE videoTracking.userID=" + userID + "AND videoTracking.videoInfoID=" + videoInfoID + "ORDER BY videoTracking.createdDate DESC", conn);
        try
        {

            conn.Open();
            reader = watchedComm.ExecuteReader();
            gridHistory.DataKeyNames = new string[] { "videoInfoID" };
            gridHistory.DataSource = reader;
            gridHistory.DataBind();
            reader.Close();

        }
        catch (SqlException ex)
        {

            dbErrorMessage.Text =
                "<div class=\"errorMessage\">Error submitting your registration try again later, and/or change the entered data.<br /> [" + ex.Message + "]</div>";
        }
        finally
        {
            conn.Close();

        }
    }
    // Handle view complete link in grid
    protected void memberSearchGrid_RowCommand(Object sender, GridViewCommandEventArgs e)
    {
        // If multiple buttons are used in a GridView control, use the
        // CommandName property to determine which button was clicked.
        if (e.CommandName == "viewImpSteps")
        {
            // Convert the row index stored in the CommandArgument
            // property to an Integer.
            int index = Convert.ToInt32(e.CommandArgument);
            int userID = (int)memberSearchGrid.DataKeys[index].Value;

            // Retrieve the row that contains the button clicked 
            // by the user from the Rows collection.
            GridViewRow row = memberSearchGrid.Rows[index];
            bindCompletedSteps(userID);
        }

        if (e.CommandName == "videoHistory")
        {
            int index = Convert.ToInt32(e.CommandArgument);
            int userID = (int)memberSearchGrid.DataKeys[index].Value;
            GridViewRow row = memberSearchGrid.Rows[index];
            bindWatchedVideos(userID);
        }
        
        if (e.CommandName == "viewMemberDetail")
        {
            int index = Convert.ToInt32(e.CommandArgument);
            int userID = (int)memberSearchGrid.DataKeys[index].Value;
            GridViewRow row = memberSearchGrid.Rows[index];
            BindsearchDetails(userID);
            Session["adminSelectUserID"] = userID.ToString();
        }
    }

    // Bind member detail when selected from search grid view
    private void BindsearchDetails(int userID)
    {
        completedImpGrid.Visible = false;
        watchedVideoGrid.Visible = false;
        gridHistory.Visible = false;
        memberAdminDetailView.Visible = true;
        noticeNoVideos.Visible = false;
        memberSearchGrid.Visible = false;
        backToSearchIB.Visible = true;
        backToSearchLB.Visible = true;
        SqlConnection conn;
        SqlCommand comm;
        SqlDataReader reader;
        string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
        conn = new SqlConnection(connectionString);
        comm = new SqlCommand("SELECT[user].userID, permission.permissionLevel, [user].firstName, [user].lastName, [user].address1, [user].address2, " +
                                "[user].city, [user].state, [user].zipCode, [user].email, [user].contactPhone, [user].practiceName, " +
                                "[user].title, Replace(Convert(VarChar(10), [user].createdDate, 101), '.', '/') AS createdDate, userStatus.userStatusID, userStatus.name FROM permission " +
                                "JOIN [user] ON permission.permissionID = [user].permissionID " +
                                "JOIN [userStatus] ON [user].userStatusID = [userStatus].userStatusID " +
                                "WHERE userID=" + userID + "", conn);

        try
        {
            conn.Open();
            reader = comm.ExecuteReader();
            memberAdminDetailView.DataSource = reader;
            memberAdminDetailView.DataKeyNames = new string[] { "userID" };
            memberAdminDetailView.DataBind();
            reader.Close();
        }
        finally
        {
            conn.Close();
            h3Title.Text = "Member Detail View";
        }
    }
    // Bind completed steps
    private void bindCompletedSteps(int userID)
    {
        SqlConnection conn;
        SqlCommand comm;
        SqlDataReader reader;
        string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
        conn = new SqlConnection(connectionString);
        comm = new SqlCommand("SELECT impTracking.impTrackingID, impTracking.userID, impTracking.impStepID, Replace(Convert(VarChar(10), impTracking.createdDate, 101), '.', '/') AS createdDate, " +
                                "impSteps.impStepName, impSteps.Priority, impSteps.[Description] FROM impSteps " +
                                "JOIN impTracking ON impSteps.impStepID = impTracking.impStepID " +
                                "WHERE impTracking.UserID=" + userID + "ORDER BY impSteps.Priority ASC", conn);

        try
        {

            conn.Open();
            reader = comm.ExecuteReader();
            if (reader.HasRows)
            {
                completedImpGrid.Visible = true;
                noticeNoVideos.Visible = false;
                completedImpGrid.DataKeyNames = new string[] { "impStepID" };
                completedImpGrid.DataSource = reader;
                completedImpGrid.DataBind();
                reader.Close();
            }
            else
            {
                noticeNoVideos.Visible = true;
                noticeNoVideos.Text = "<div class='searchNotice'>Member has not completed any implementation steps.</div>";
                reader.Close();
            }
        }
        catch (SqlException ex)
        {

            dbErrorMessage.Text =
                "<div class=\"errorMessage\">Error submitting your registration try again later, and/or change the entered data.<br /> [" + ex.Message + "]</div>";
        }
        finally
        {
            conn.Close();
            completedImpGrid.Visible = true;
            watchedVideoGrid.Visible = false;
            gridHistory.Visible = false;

        }
    }
    protected void memberSearchBy_SelectedIndexChanged(object sender, EventArgs e)
    {

    }
}
