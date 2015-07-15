using System;
using System.Collections;
using System.Collections.Generic;
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

public partial class admin_recVidTemplates : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // Handle mini ribbon color change if menu item active
            LinkButton mpLinkButtonM = (LinkButton)Master.FindControl("membersLB");
            mpLinkButtonM.ForeColor = System.Drawing.Color.White;

            LinkButton mpLinkButtonV = (LinkButton)Master.FindControl("videosLB");
            mpLinkButtonV.ForeColor = System.Drawing.Color.White;

            LinkButton mpLinkButtonIS = (LinkButton)Master.FindControl("helpPagesLB");
            mpLinkButtonIS.ForeColor = System.Drawing.Color.White;

            LinkButton mpLinkButtonRC = (LinkButton)Master.FindControl("recVidsLB");
            mpLinkButtonRC.ForeColor = System.Drawing.ColorTranslator.FromHtml("#9CC5C9");

            LinkButton mpLinkButtonS = (LinkButton)Master.FindControl("searchLB");
            mpLinkButtonS.ForeColor = System.Drawing.Color.White;

            memberSearchGrid.Visible = true;
            bindRecommendVideos();
            //Bind template names to apply template list
            bindApplyTemplatesDropDown();
           
            
        }
        // Default Search Button
        Page.Form.DefaultButton = memberSearchBtn.UniqueID;
        // Hide all message boxes so jquery methods don't get messed up.
        searchError.Visible = false;
        noticeMessage.Visible = false;
        dbErrorMessage.Visible = false;
    }

    // Function to bind template names
    protected void bindApplyTemplatesDropDown()
    {
        SqlConnection conn;
        SqlCommand templateComm;
        SqlDataReader reader;
        string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
        conn = new SqlConnection(connectionString);
        templateComm = new SqlCommand("SELECT recTempNameID, templateName, createdDate, modifiedDate FROM recommendedTemplateName", conn);
        try
        {
            // Bind Registration States Drop Down List.
            conn.Open();
            reader = templateComm.ExecuteReader();
            ddlApplyTemp.DataSource = reader;
            ddlApplyTemp.DataValueField = "recTempNameID";
            ddlApplyTemp.DataTextField = "templateName";
            ddlApplyTemp.DataBind();
            reader.Close();
        }
        finally
        {
            ddlApplyTemp.Items.Insert(0, new ListItem("-- Select Template Here / Reset List --", "0"));
            conn.Close();
        }
    }
    // Handle apply template drop down menu
    protected void ddlApplyTemplate_SelectedIndexChange(object sender, EventArgs e)
    {
        int templateID = Convert.ToInt32(ddlApplyTemp.SelectedValue);
        bindTemplateVideos(templateID);
    }
    // Check videos off in checkbox list when "Edit Videos" is pressed from grid
    //Load videos to recommended checkbox list
    protected void bindTemplateVideos(int templateID)
    {
        int nID = templateID;
        SqlConnection conn;
        SqlCommand comm;
        DataSet dataSetTemplates = new DataSet();
        SqlDataReader reader;
        SqlDataAdapter adapter;
        string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
        conn = new SqlConnection(connectionString);
        comm = new SqlCommand("SELECT videoInfoID, videoName FROM videoInfo ORDER BY videoName ASC", conn);
        adapter = new SqlDataAdapter("SELECT videoID FROM recommendedTemplateVids WHERE recNameID=" + nID + "", conn);
        adapter.Fill(dataSetTemplates, "videoInfoIDs");
        try
        {
            conn.Open();
            reader = comm.ExecuteReader();
            recommendVidCBL.DataSource = reader;
            recommendVidCBL.DataTextField = "videoName";
            recommendVidCBL.DataValueField = "videoInfoID";
            recommendVidCBL.DataBind();
            reader.Close();
        }
        catch (SqlException ex)
        {

            messageBox box = new messageBox("error", "There has been an error trying to bind recommended videos<br /> [" + ex.Message + "]", searchError);
        }
        finally
        {
            conn.Close();
        }

        foreach (DataRow dr in dataSetTemplates.Tables[0].Rows)
        {
            for (int i = 0; i < recommendVidCBL.Items.Count; i++)
            {
                if (Convert.ToInt32(recommendVidCBL.Items[i].Value) == Convert.ToInt32(dr["videoID"]))
                {
                    recommendVidCBL.Items[i].Selected = true;
                }
            }
        }
    }

    
    // Handle tertiary link buttons
    protected void clickStartOver(object sender, EventArgs e)
    {
        Response.Redirect("recVidTemplates.aspx");
    }

    // Handle build templates
    protected void clickManageTemplates(object sender, EventArgs e)
    {
        Response.Redirect("manageTemplates.aspx");
    }

    protected void clickBackToResults(object sender, EventArgs e)
    {
        panelTR.Visible = false;
        panelSelectMember.Visible = false;
        panelSearchBox.Visible = true;
        panelSearchResults.Visible = true;
        panelRecVids.Visible = true;
        panelTR2.Visible = true;
        panelDelete.Visible = false;
        panelTR3.Visible = true;
    }
    // Reset sort order
    protected void resetSortOrder(int userRecID)
    {
        int sortCounter = 0;
        int userID = userRecID;
        SqlConnection conn;
        string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
        conn = new SqlConnection(connectionString);
        DataSet dataSet = new DataSet();
        SqlDataAdapter adapter;
        adapter = new SqlDataAdapter("SELECT recID FROM recommendedVideos WHERE userID=" + userID.ToString() + "", conn);
        adapter.Fill(dataSet, "recIDDS");
        SqlCommand setSortOrderComm;

        foreach (DataRow dr in dataSet.Tables[0].Rows)
        {
            sortCounter++;
            setSortOrderComm = new SqlCommand("UPDATE recommendedVideos SET sortID=" + sortCounter.ToString() + " WHERE recID=" + dr["recID"].ToString() + "", conn);
            try
            {
                conn.Open();
                setSortOrderComm.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                messageBox box = new messageBox("error", "There has been an error: [" + ex.Message + "]", searchError);
            }
            finally
            {
                conn.Close();
            }
        }
        messageBox successBox = new messageBox("success", "Sort order has been reset", searchError);
        bindRecMemberGrid(userID);
    }

    // Handle reset sort order button
    protected void clickResetSort(object sender, EventArgs e)
    {
        Label uID = (Label)recVidsMemberGrid.Rows[0].FindControl("userIDLbl");
        int userID = Convert.ToInt32(uID.Text);
        resetSortOrder(userID);
    }

    // Handle completed button
    protected void clickVidComplete(object sender, EventArgs e)
    {
        panelDelete.Visible = false;
        int hasRows = recVidsMemberGrid.Rows.Count;
        if (hasRows > 0)
        {
            if (globalFunctions.gridLineItemChecked(recVidsMemberGrid, "RowLevelCheckBoxM"))
            {
                for (int i = 0; i < recVidsMemberGrid.Rows.Count; i++)
                {
                    GridViewRow row = recVidsMemberGrid.Rows[i];
                    Label recordID = (Label)recVidsMemberGrid.Rows[i].FindControl("recordIDLbl");
                    bool isChecked = ((CheckBox)row.FindControl("RowLevelCheckBoxM")).Checked;
                    if (isChecked)
                    {
                        int recID = Convert.ToInt32(recordID.Text);
                        markComplete(recID);
                    }
                }
                Label uID = (Label)recVidsMemberGrid.Rows[0].FindControl("userIDLbl");
                int userID = Convert.ToInt32(uID.Text);
                bindRecMemberGrid(userID);
                messageBox boxSuccess = new messageBox("success", "Success, item(s) have been marked completed", searchError);
            }
            else
            {
                messageBox box = new messageBox("notice", "You have not selected any videos to mark complete.", searchError);
            }
        }
    }

    protected void markComplete(int recordID)
    {
        int recID = recordID;
        SqlConnection conn;
        SqlCommand comm;
        string dt = DateTime.Now.ToString();
        string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
        conn = new SqlConnection(connectionString);
        comm = new SqlCommand("UPDATE recommendedVideos SET completedDate='" + dt.ToString() + "' WHERE recID=" + recID.ToString() + "", conn);
        try
        {
            conn.Open();
            comm.ExecuteNonQuery();
        }
        catch (SqlException ex)
        {
            messageBox box = new messageBox("error", "There has been an error: [" + ex.Message + "]", searchError);
        }
        finally
        {
            conn.Close();
        }

    }
    // Handle mark incomplete
    protected void clickVidIncomplete(object sender, EventArgs e)
    {
        panelDelete.Visible = false;
        int hasRows = recVidsMemberGrid.Rows.Count;
        if (hasRows > 0)
        {
            if (globalFunctions.gridLineItemChecked(recVidsMemberGrid, "RowLevelCheckBoxM"))
            {
                for (int i = 0; i < recVidsMemberGrid.Rows.Count; i++)
                {
                    GridViewRow row = recVidsMemberGrid.Rows[i];
                    Label recordID = (Label)recVidsMemberGrid.Rows[i].FindControl("recordIDLbl");
                    bool isChecked = ((CheckBox)row.FindControl("RowLevelCheckBoxM")).Checked;
                    if (isChecked)
                    {
                        int recID = Convert.ToInt32(recordID.Text);
                        markInComplete(recID);
                    }
                }
                Label uID = (Label)recVidsMemberGrid.Rows[0].FindControl("userIDLbl");
                int userID = Convert.ToInt32(uID.Text);
                bindRecMemberGrid(userID);
                messageBox boxSuccess = new messageBox("success", "Success, item(s) have been marked incompleted", searchError);
            }
            else
            {
                messageBox box = new messageBox("notice", "You have not selected any videos to mark incomplete.", searchError);
            }
        }
    }

    protected void markInComplete(int recordID)
    {
        int recID = recordID;
        SqlConnection conn;
        SqlCommand comm;
        string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
        conn = new SqlConnection(connectionString);
        comm = new SqlCommand("UPDATE recommendedVideos SET completedDate=NULL WHERE recID=" + recID.ToString() + "", conn);
        try
        {
            conn.Open();
            comm.ExecuteNonQuery();
        }
        catch (SqlException ex)
        {
            messageBox box = new messageBox("error", "There has been an error: [" + ex.Message + "]", searchError);
        }
        finally
        {
            conn.Close();
        }

    }
    // Handle delete link button from member's recommended videos grid
    protected void clickDeleteRV(object sender, EventArgs e)
    {
        int hasRows = recVidsMemberGrid.Rows.Count;
        if (hasRows > 0)
        {
            if (globalFunctions.gridLineItemChecked(recVidsMemberGrid, "RowLevelCheckBoxM"))
            {
                panelDelete.Visible = true;
                messageBox deleteBox = new messageBox("error", "Please confirm that you wish to delete selected records", searchError);
            }
            else
            {
                panelDelete.Visible = false;
                messageBox box = new messageBox("notice", "You have not selected any videos to delete", searchError);
            }
        }
     
    }
    // Confirm delete button
    protected void clickConfirmDelete(object sender, EventArgs e)
    {
        if (globalFunctions.gridLineItemChecked(recVidsMemberGrid, "RowLevelCheckBoxM"))
        {
            for (int i = 0; i < recVidsMemberGrid.Rows.Count; i++)
            {
                GridViewRow row = recVidsMemberGrid.Rows[i];
                bool isChecked = ((CheckBox)row.FindControl("RowLevelCheckBoxM")).Checked;
                Label recordID = (Label)recVidsMemberGrid.Rows[i].FindControl("recordIDLbl");
                if (isChecked)
                {
                    int recID = Convert.ToInt32(recordID.Text);
                    deleteRecVideo(recID);
                }
            }
            Label uID = (Label)recVidsMemberGrid.Rows[0].FindControl("userIDLbl");
            int userID = Convert.ToInt32(uID.Text);
            bindRecMemberGrid(userID);
            panelDelete.Visible = false;
            messageBox successBox = new messageBox("success", "Success, records have been deleted and grid has been refreshed.", searchError);
        }
        else
        {
            messageBox box = new messageBox("notice", "You have not selected anything to delete", searchError);
        }
    }

    protected void deleteRecVideo(int recordID)
    {
        int recID = recordID;
        SqlConnection conn;
        SqlCommand comm;
        string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
        conn = new SqlConnection(connectionString);
        comm = new SqlCommand("DELETE FROM recommendedVideos WHERE recID=" + recID.ToString() + "", conn);

        try
        {
            conn.Open();
            comm.ExecuteNonQuery();
        }
        catch (SqlException ex)
        {
            messageBox box = new messageBox("error", "There has been an error [ " + ex.Message + " ]", searchError);
        }
        finally
        {
            conn.Close();
        }
    }
    // Cancel Delete
    protected void clickCancelDelete(Object sender, EventArgs e)
    {
        Label uID = (Label)recVidsMemberGrid.Rows[0].FindControl("userIDLbl");
        int userID = Convert.ToInt32(uID.Text);
        bindRecMemberGrid(userID);
        panelDelete.Visible = false;
        messageBox box = new messageBox("notice", "Deletion has been cancelled", searchError);
    }
    // Handle update link button from member's recommended videos grid
    protected void clickUpdateList(object sender, EventArgs e)
    {
        panelDelete.Visible = false;
        int hasRows = recVidsMemberGrid.Rows.Count;
        if (hasRows > 0)
        {
            if (globalFunctions.gridLineItemChecked(recVidsMemberGrid, "RowLevelCheckBoxM"))
            {
                for (int i = 0; i < recVidsMemberGrid.Rows.Count; i++)
                {
                    GridViewRow row = recVidsMemberGrid.Rows[i];
                    TextBox updateSortID = (TextBox)recVidsMemberGrid.Rows[i].FindControl("recVidSortEdit");
                    Label recordID = (Label)recVidsMemberGrid.Rows[i].FindControl("recordIDLbl");
                    
                    bool isChecked = ((CheckBox)row.FindControl("RowLevelCheckBoxM")).Checked;
                    if (isChecked)
                    {
                        int updateSort = Convert.ToInt32(updateSortID.Text);
                        int recID = Convert.ToInt32(recordID.Text);
                        updateMemberVids(recID, updateSort);
                    }
                }
                Label uID = (Label)recVidsMemberGrid.Rows[0].FindControl("userIDLbl");
                int userID = Convert.ToInt32(uID.Text);
                bindRecMemberGrid(userID);
                messageBox boxSuccess = new messageBox("success", "Recommended videos have been updated", searchError);
            }
            else
            {
                messageBox box = new messageBox("notice", "You have not selected any videos to update.", searchError);
            }
        }
    }
    // Handle update list function for member's recommended videos grid
    // loop through grid find selected items
    // get recVidSortEdit value
    // update recommendedVideos table with value
    protected void updateMemberVids(int recID, int ID)
    {
        int recordID = recID;
        int sortID = ID;
        SqlConnection conn;
        SqlCommand comm;
        string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
        conn = new SqlConnection(connectionString);
        comm = new SqlCommand("UPDATE recommendedVideos SET sortID=" + sortID.ToString() + " WHERE recID=" + recordID.ToString() + "", conn);

        try
        {
            conn.Open();
            comm.ExecuteNonQuery();
        }
        catch (SqlException ex)
        {
            messageBox box = new messageBox("error", "There has been an error: [" + ex.Message + "]", searchError);
        }
        finally
        {
            conn.Close();
        }
    }
    // Bind recommended video grid for member when selected
    protected void bindRecMemberGrid(int userID)
    {
        int id = userID;
        SqlConnection conn;
        SqlCommand comm;
        SqlCommand validateComm;
        SqlDataReader reader;
        string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
        conn = new SqlConnection(connectionString);
        validateComm = new SqlCommand("SELECT recID FROM recommendedVideos WHERE userID=" + userID.ToString() + "", conn);
        comm = new SqlCommand("SELECT [videoInfo].videoName, [recommendedVideos].recID, [recommendedVideos].userID, [recommendedVideos].sortID, " +
                                "[recommendedVideos].createdDate, [recommendedVideos].completedDate, " +
                                "[user].firstName, [user].lastName, [user].practiceName, [user].renOrgID " +
                                "FROM [recommendedVideos] " + 
                                "JOIN [videoInfo] on [recommendedVideos].videoId = [videoInfo].videoInfoID " +
                                "JOIN [user] on [recommendedVideos].userID = [user].userID " +
                                "WHERE [recommendedVideos].userID=" + id.ToString() + " ORDER BY sortID ASC", conn);

        try
        {
            conn.Open();
            reader = validateComm.ExecuteReader();
            if (!reader.Read())
            {
                recVidsMemberGrid.Visible = false;
                messageBox noticeBox = new messageBox("notice", "No videos have been recommended for this member.", searchError);
                labelTitleInfo.Text = "";
                reader.Close();
            }
            else
            {
                recVidsMemberGrid.Visible = true;
                reader.Close();
                reader = comm.ExecuteReader();
                recVidsMemberGrid.DataSource = reader;
                recVidsMemberGrid.DataBind();
                reader.Close();
                reader = comm.ExecuteReader();
                reader.Read();
                labelTitleInfo.Text = "<h3>" + reader["firstName"].ToString() + reader["lastName"].ToString() + " - " + reader["practiceName"].ToString() + "</h3>";
                reader.Close();
            }
        }
        catch (SqlException ex)
        {

            messageBox box = new messageBox("error", "There has been an error [" + ex.Message + "]", searchError);
        }
        finally
        {
            conn.Close();
        }

        
    }
    // Hanlde recVidsMemberGrid_RowEditing
    protected void recVidsMemberGrid_RowEditing(object sender, EventArgs e)
    {

    }
   
    // Handle recVidsMemberGrid_RowCancelingEdit
    protected void recVidsMemberGrid_RowCancelingEdit(object sender, EventArgs e)
    {

    }

    // Handle recVidsMemberGrid_RowUpdating
    protected void recVidsMemberGrid_RowUpdating(object sender, EventArgs e)
    {

    }

    // Handle recVidsMemberGrid_RowDeleting
    protected void recVidsMemberGrid_RowDeleting(object sender, EventArgs e)
    {

    }
    // Load videos to recommended checkbox list
    protected void bindRecommendVideos()
    {
        SqlConnection conn;
        SqlCommand comm;
        SqlDataReader reader;
        string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
        conn = new SqlConnection(connectionString);
        comm = new SqlCommand("SELECT videoInfoID, videoName FROM videoInfo ORDER BY videoName ASC", conn);

        try
        {
            conn.Open();
            reader = comm.ExecuteReader();
            recommendVidCBL.DataSource = reader;
            recommendVidCBL.DataTextField = "videoName";
            recommendVidCBL.DataValueField = "videoInfoID";
            recommendVidCBL.DataBind();
            reader.Close();
        }
        catch (SqlException ex)
        {

            searchError.Text =
                "<div class=\"errorMessageSearch\">There has been an error trying to bind recommended videos<br /> [" + ex.Message + "]</div>";
        }
        finally
        {
            conn.Close();
        }
    }

   // Handle check all checkbox search grid
    protected void HeaderLevelCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        CheckBox chk;
        foreach (GridViewRow rowItem in memberSearchGrid.Rows)
        {
            chk = (CheckBox)(rowItem.Cells[0].FindControl("RowLevelCheckBox"));
            chk.Checked = ((CheckBox)sender).Checked;
            globalFunctions.resetGridBackground(memberSearchGrid);
            recommendVidCBL.Visible = true;
            recVidCBLSelectAll.Visible = true;
        }
    }

    // Handle check all checkbox recommended vidoes member
    protected void HeaderLevelCheckBox_CheckedChangedM(object sender, EventArgs e)
    {
        CheckBox chk;
        foreach (GridViewRow rowItem in recVidsMemberGrid.Rows)
        {
            chk = (CheckBox)(rowItem.Cells[0].FindControl("RowLevelCheckBoxM"));
            chk.Checked = ((CheckBox)sender).Checked;
            globalFunctions.resetGridBackground(memberSearchGrid);
            recommendVidCBL.Visible = true;
            recVidCBLSelectAll.Visible = true;
            panelTR.Visible = true;
        }
    }

    // Handle check all videos checkbox for videos
    protected void recommendVidCBL_CheckedChanged(object sender, EventArgs e)
    {
        foreach (ListItem li in recommendVidCBL.Items)
        {
            li.Selected = ((CheckBox)sender).Checked;
        }
    }
    // Handle recommended videos submission by looping through each row in the memberSearchGrid
    // For each row loop through the recommendedVideoCBL and add selected ones
    // Invoke membersPresent and videosPresent methods to make sure something is actually checked
    // Hide / show messages to let user know what to do if nothing is checked
    // Check to see if recommneded videos already exist then hide / show message labels to let user know
    protected void recommendVids(object sender, EventArgs e)
    {
        searchError.Visible = false;
        noticeMessage.Visible = false;
        SqlConnection conn;
        SqlCommand comm;
        SqlCommand validateComm;
        SqlDataReader reader;
        bool newRecord = false;
        int userID;
        List<string> userNames = new List<string>();
        int[] videoIDs = new int[recommendVidCBL.Items.Count];
        string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
        conn = new SqlConnection(connectionString);

        int hasRows = memberSearchGrid.Rows.Count;
        if (hasRows > 0)
        {
            if(!globalFunctions.checkBoxListItemChk(recommendVidCBL) && !globalFunctions.gridLineItemChecked(memberSearchGrid, "RowLevelCheckBox"))
            {
                messageBox box = new messageBox("notice", "You must select member then videos below to recommend videos.", noticeMessage);
            }
            else if (!globalFunctions.checkBoxListItemChk(recommendVidCBL))
            {
                messageBox box = new messageBox("notice", "Please select videos", noticeMessage);
            }
            else
            {
                noticeMessage.Visible = false;
                for (int i = 0; i < memberSearchGrid.Rows.Count; i++)
                {
                    if (globalFunctions.gridLineItemChecked(memberSearchGrid, "RowLevelCheckBox"))
                    {
                        GridViewRow row = memberSearchGrid.Rows[i];
                        bool isChecked = ((CheckBox)row.FindControl("RowLevelCheckBox")).Checked;
                        if (isChecked)
                        {
                            userNames.Add(memberSearchGrid.Rows[i].Cells[6].Text);
                            for (int j = 0; j < recommendVidCBL.Items.Count; j++)
                            {
                                if (recommendVidCBL.Items[j].Selected)
                                {
                                    string dt = DateTime.Now.ToString();
                                    userID = Convert.ToInt32(memberSearchGrid.Rows[i].Cells[2].Text);
                                    videoIDs[j] = Convert.ToInt32(recommendVidCBL.Items[j].Value);
                                    validateComm = new SqlCommand("SELECT recID FROM recommendedVideos WHERE userID = " + userID.ToString() + " AND videoID = " + videoIDs[j].ToString() + "", conn);
                                    comm = new SqlCommand("INSERT INTO recommendedVideos VALUES(" + userID.ToString() + ", " + videoIDs[j].ToString() + ", NULL, '" + dt.ToString() + "', NULL)", conn);
                                   
                                    try
                                    {
                                        conn.Open();
                                        reader = validateComm.ExecuteReader();
                                        if (reader.Read())
                                        {
                                            messageBox box = new messageBox("notice", "Some or all of the videos you are trying to recommend have already been recommended. Only new recommendations have been added.", noticeMessage);
                                        }
                                        else
                                        {
                                            searchError.Visible = true;
                                            reader.Close();
                                            comm.ExecuteNonQuery();
                                            newRecord = true;
                                        }
                                    }
                                    catch (SqlException ex)
                                    {
                                        messageBox box = new messageBox("error", "There has been an error:<br /> [" + ex.Message + "] " + dt.ToString() + "", searchError);
                                        conn.Close();
                                    }
                                    finally
                                    {
                                        conn.Close();
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        messageBox box = new messageBox("notice", "Please select member(s) to associate videos to.", noticeMessage);
                    }
                }
            }
        }
        else
        {
            messageBox box = new messageBox("notice", "Please use the search feature above to find member(s) to associate videos to.", noticeMessage);
        }
        conn.Close();
        // SET SORT ORDER LOOP
        // Loop through member search gridview and execute code if member record is checked
        // Select recID coloumn from recommendedVideos and add to DataSet
        // Loop through dataset updating the sort order column each time
        for (int i = 0; i < memberSearchGrid.Rows.Count; i++)
        {
            int sortCounter = 0;
            GridViewRow row = memberSearchGrid.Rows[i];
            bool isChecked = ((CheckBox)row.FindControl("RowLevelCheckBox")).Checked;
            if (isChecked)
            {
                userID = Convert.ToInt32(memberSearchGrid.Rows[i].Cells[2].Text);
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter;
                adapter = new SqlDataAdapter("SELECT recID FROM recommendedVideos WHERE userID=" + userID.ToString() + "", conn);
                adapter.Fill(dataSet, "recIDDS");
                SqlCommand setSortOrderComm;

                foreach (DataRow dr in dataSet.Tables[0].Rows)
                {
                    sortCounter++;
                    setSortOrderComm = new SqlCommand("UPDATE recommendedVideos SET sortID=" + sortCounter.ToString() + " WHERE recID=" + dr["recID"].ToString() + "", conn);
                    try
                    {
                        conn.Open();
                        setSortOrderComm.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        messageBox box = new messageBox("error", "There has been an error: [" + ex.Message + "]", searchError);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
                
            }
        }
        // Display success message if new records are added
        if (newRecord)
        {
            string firstName = "Videos have now been recommended for the following People: ";
            foreach (string names in userNames)
            {

                firstName += names.ToString() + ", ";
            }
            string trimFirstName = firstName.Remove(firstName.Length - 2, 2);
            searchError.Text = "<div class=\"success\">" + trimFirstName + ".</div>";
            messageBox successbox = new messageBox("success", trimFirstName, searchError);
        }

    }
    
    // Handle Search btn
    protected void memberSearch(object sender, EventArgs e)
    {
        panelTR3.Visible = false;
        if (memberSearchText.Text == "@" || memberSearchText.Text == ".com")
        {
            messageBox box = new messageBox("error", "You can not search using only the @ symbol or the phrase \".com\". Please try again.", searchError);
        }
        else
        {
            bindSearchGrid();
            recommendVidCBL.Visible = true;
            recVidCBLSelectAll.Visible = true;
            panelTR2.Visible = true;
            panelRecVids.Visible = true;
        }

    }

    // Function to bind search grid
    private void bindSearchGrid()
    {
        if (IsPostBack && memberSearchText.Text != "")
        {
            noticeMessage.Visible = false;
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
                    memberSearchGrid.Visible = true;
                }
                else
                {
                    messageBox box = new messageBox("notice", "Your search did not return any members. Please try again. TIP:" +
                        "You may want to try using part of their name or email address only.", searchError);
                    reader.Close();
                }
            }
            catch (SqlException ex)
            {
                messageBox box = new messageBox("error", "There has been an error:<br /> [" + ex.Message + "]", searchError);
            }

            finally
            {
                conn.Close();
            }

        }
        else
        {
            messageBox box = new messageBox("error", "Please enter something in the <b>\"Search For\"</b> field and try your search again.", searchError);
        }
    }

    // Handle selected index change
    protected void memberSearchGrid_SelectedIndexChanged(object sender, EventArgs e)
    {

    }
    //Clear selected background color on member row checked in searchMemberGrid
    protected void styleChange_onCheck(object sender, EventArgs e)
    {

        globalFunctions.resetGridBackground(memberSearchGrid);
        recommendVidCBL.Visible = true;
        recVidCBLSelectAll.Visible = true;
    }
    // Handle view complete link in grid
    protected void memberSearchGrid_RowCommand(Object sender, GridViewCommandEventArgs e)
    {
        // If multiple buttons are used in a GridView control, use the
        // CommandName property to determine which button was clicked.
        if (e.CommandName == "viewRecVids")
        {
            // Convert the row index stored in the CommandArgument
            // property to an Integer.
            int index = Convert.ToInt32(e.CommandArgument);
            int userID = (int)memberSearchGrid.DataKeys[index].Value;

            // Retrieve the row that contains the button clicked 
            // by the user from the Rows collection.

            panelRecVids.Visible = false;
            panelSearchBox.Visible = false;
            panelSearchResults.Visible = false;
            panelTR.Visible = true;
            panelSelectMember.Visible = true;
            panelTR2.Visible = false;
            GridViewRow row = (GridViewRow)memberSearchGrid.Rows[index];
            globalFunctions.resetGridBackground(memberSearchGrid);
            row.Style.Add(HtmlTextWriterStyle.BackgroundColor, "#E1EBED");
            bindRecMemberGrid(userID);
            
        }

        if (e.CommandName == "applyTemplate")
        {
            int index = Convert.ToInt32(e.CommandArgument);
            int userID = (int)memberSearchGrid.DataKeys[index].Value;
            GridViewRow row = memberSearchGrid.Rows[index];
        }
    }
}