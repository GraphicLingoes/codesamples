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

public partial class newDashboard : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Check if user is logged in else redirect them to login page
        if (!globalFunctions.validateLogin())
        {
            Response.Redirect("Logout.aspx");
        }
        // **The following code is for version two, we hide it until then**
        messagesIB.Visible = false;
        messagesLB.Visible = false;
        myImpIB.Visible = true;
        myImpLB.Visible = true;
        //Load watched video grid.
        watchedVideoGrid.Visible = false;
        gridHistory.Visible = false;
        noticeLabel.Visible = false;
    
        // Hide back to home buttons
        backToHomeIB.Visible = false;
        backToHomeLB.Visible = false;
    }
    // Handle back to home
    protected void backToHome(object sender, EventArgs e)
    {
        Response.Redirect("newDashboard.aspx");
    }
    
    // Handle show messages
    public void showMessages(object s, EventArgs e)
    {
        panelTR.Visible = false;
        noticeLabel.Visible = true;
        messagesLB.ForeColor = System.Drawing.ColorTranslator.FromHtml("#9CC5C9");
        noticeLabel.Text = "Showing Messages";
        videoHistoryLB.ForeColor = System.Drawing.Color.White;
        myImpLB.ForeColor = System.Drawing.Color.White;
    }
    // Handle show my implementation
    public void showRecommendedVids(object s, EventArgs e)
    {
        noHistory.Visible = false;
        dashboardMessage.Visible = false;
        myImpLB.ForeColor = System.Drawing.ColorTranslator.FromHtml("#9CC5C9");
        videoHistoryLB.ForeColor = System.Drawing.Color.White;
        messagesLB.ForeColor = System.Drawing.Color.White;
        backToHomeIB.Visible = true;
        backToHomeLB.Visible = true;
        panelTR.Visible = true;
        panelRecVids.Visible = true;
        BindrecVidsRepeater();
    }
    // Handle show video history
    public void showVideoHistory(object s, EventArgs e)
    {
        panelTR.Visible = false;
        noticeLabel.Visible = false;
        dashboardMessage.Visible = false;
        panelRecVids.Visible = false;
        backToHomeIB.Visible = true;
        backToHomeLB.Visible = true;
        watchedVideoGrid.Visible = true;
        videoHistoryLB.ForeColor = System.Drawing.ColorTranslator.FromHtml("#9CC5C9");
        messagesLB.ForeColor = System.Drawing.Color.White;
        myImpLB.ForeColor = System.Drawing.Color.White;
        bindWatchedVideos();
    }
    // Bind watched video grid
    // Always make sure userID is stored to session with ren org ID check. If it is not the "watchedComm" can return wrong results.
    private void bindWatchedVideos()
    {
        string userID = Session["userLoggedIn"].ToString();
        SqlConnection conn;
        SqlCommand watchedComm;
        SqlDataReader reader;
        string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
        conn = new SqlConnection(connectionString);
        watchedComm = new SqlCommand("SELECT COUNT(videoTracking.videoInfoID) AS videoCount, MIN(videoTracking.videoInfoID) AS videoInfoID, MIN(videoTracking.userID) AS userID, MIN(videoInfo.videoName) AS videoName, " +
                            "MIN(videoInfo.[description]) AS description, MIN(videoInfo.lenght) AS lenght, MIN(Replace(Convert(VarChar(10), videoTracking.createdDate, 101), '.', '/')) AS createdDate FROM videoInfo " +
                            "JOIN videoTracking ON videoInfo.videoInfoID = videoTracking.videoInfoID " +
                            "WHERE videoTracking.userID=" + userID + "GROUP BY videoTracking.videoInfoID ORDER BY videoName ASC", conn);
        try
        {

            conn.Open();
            reader = watchedComm.ExecuteReader();
            if (reader.HasRows)
            {
                watchedVideoGrid.DataKeyNames = new string[] { "videoInfoID" };
                watchedVideoGrid.DataSource = reader;
                watchedVideoGrid.DataBind();
                
            }
            else
            {
                messageBox box = new messageBox("notice", "It appears that you have not watched any videos yet. To get started either click on the videos or search icons in the ribbon bar above.", noHistory);
            }
            reader.Close();

        }
        catch (SqlException ex)
        {
            messageBox box = new messageBox("error", "Error submitting your registration try again later, and/or change the entered data.<br /> [" + ex.Message + "]", dbErrorMessage);
        }
        finally
        {
            conn.Close();
        }
    }

    // Handle watch video link from watched video grid view.
    protected void watchedVideoGrid_SelectedIndexChanged(object sender, EventArgs e)
    {
        int selectRowIndex = watchedVideoGrid.SelectedIndex;
        int videoInfoID = (int)watchedVideoGrid.DataKeys[selectRowIndex].Value;
        Session["videoDetailID"] = videoInfoID.ToString();
        Response.Redirect("memberVideos.aspx");
    }

    protected void watchedVideoGrid_RowCommand(Object sender, GridViewCommandEventArgs e)
    {
        // If multiple buttons are used in a GridView control, use the
        // CommandName property to determine which button was clicked.
        if (e.CommandName == "viewHistory")
        {
            // Convert the row index stored in the CommandArgument
            // property to an Integer.
            int index = Convert.ToInt32(e.CommandArgument);
            int videoInfoID = (int)watchedVideoGrid.DataKeys[index].Value;

            // Retrieve the row that contains the button clicked 
            // by the user from the Rows collection.
            GridViewRow row = watchedVideoGrid.Rows[index];
            bindHistory(videoInfoID);

        }
    }
    protected void recVidsRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName == "videoRedirect")
        {
            Session["videoDetailID"] = e.CommandArgument;
            Response.Redirect("memberVideos.aspx");
        }
    }
    // Handle watched video history link.
    private void bindHistory(int videoInfoID)
    {
        backToHomeIB.Visible = true;
        backToHomeLB.Visible = true;
        gridHistory.Visible = true;
        string userID = Session["userLoggedIn"].ToString();
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
            messageBox box = new messageBox("error", "Error submitting your registration try again later, and/or change the entered data.<br /> [" + ex.Message + "]", dbErrorMessage);
        }
        finally
        {
            conn.Close();
            watchedVideoGrid.Visible = true;
        }
    }
    // Bind recommend videos repeater
    // Load videoIDs into Sql DataSet(videoIDDS) by running a query on recommnedVideos table using UserID from session
    // Loop through videoIDDS and add all recommended videos to another Sql DataSet(recVidsDS)
    // Bind recommended videos repeater using recVidsDS
    protected void BindrecVidsRepeater()
    {
        recVidsRepeater.Visible = true;
        int userID = Convert.ToInt32(Session["userLoggedIn"].ToString());
        SqlConnection conn;
        DataSet recVideoDS = new DataSet();
        DataSet recVideoIdDS = new DataSet();
        SqlDataAdapter adapter;
        SqlDataAdapter adapterRV;
        string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
        conn = new SqlConnection(connectionString);
        adapter = new SqlDataAdapter("SELECT videoID, userID, sortID FROM recommendedVideos WHERE userID=" + userID.ToString() + " ORDER BY sortID", conn);
        adapter.Fill(recVideoDS, "videoIDDS");
        

        foreach (DataRow dr in recVideoDS.Tables[0].Rows)
        {
            adapterRV = new SqlDataAdapter("SELECT [recommendedVideos].[recID],[recommendedVideos].[videoID],[recommendedVideos].[userID], [recommendedVideos].[sortID], " +
                                        "[recommendedVideos].[createdDate], [recommendedVideos].[completedDate], [videoInfo].[videoInfoID], " +
                                        "[videoInfo].[videoName], [videoAuthor].[authorName], [videoInfo].[description], [videoInfo].[lenght], " +
                                        "[videoCategory].[categoryName], [videoAuthor].[authTitle], [videoInfo].[keyWords] " +
                                        "FROM [videoAuthor] " +
                                        "JOIN [videoInfo] ON [videoAuthor].[videoAuthID] = [videoInfo].[videoAuthID] " +
                                        "JOIN [recommendedVideos] on [videoInfo].[videoInfoID] = [recommendedVideos].[videoID] " +
                                        "JOIN [videoCategory] ON [videoInfo].[videoCatID] = [videoCategory].[VideoCatID] " +
                                        "WHERE [recommendedVideos].[videoID]=" + Convert.ToInt32(dr["videoID"]) + " AND [recommendedVideos].[userID]=" + Convert.ToInt32(dr["userID"]) + "", conn);
            adapterRV.Fill(recVideoIdDS, "recVideosID");
            
        }
        // Change CSS background color of tr for completed or not completed header
        if (recVideoIdDS.Tables.Count > 0)
        {
            messageBox box = new messageBox("notice", " Below you will find a list of videos that have been tailored to your training needs by your LeonardoMD implementation representative. They are also being displayed in the order you should view them.<br /><br />To keep track of videos you have completed and to let your implementation representative know you are on the right track, be sure to check off each video you complete and hit the \"Mark Complete\" button above.", noticeLabel);
            recVidsRepeater.DataSource = recVideoIdDS;
            recVidsRepeater.DataBind();
            for(int i = 0; i<recVidsRepeater.Items.Count; i++)
            {
                Label completed = (Label)recVidsRepeater.Items[i].FindControl("labelCompleted");
                Label divClass = (Label)recVidsRepeater.Items[i].FindControl("labelTableClass");
                if (completed.Text != "")
                {
                    divClass.Text = "<div class=\"tableHdrRepeaterComplete\">";
                }
                else
                {
                    divClass.Text = "<div class=\"tableHdrRepeater\">";
                }
                
            }
        }
        else
        {
            noticeLabel.Visible = true;
            messageBox box = new messageBox("notice", "No videos have been recommended at this time. Please contact your implementation representative for more information.", noticeLabel);
        }
    }
    // Check to see if anything is checked
    protected bool lineItemPresent()
    {
        for (int i = 0; i < recVidsRepeater.Items.Count; i++)
        {
            RepeaterItem item = recVidsRepeater.Items[i];
            bool isChecked = ((CheckBox)item.FindControl("rowLevelCheckBox")).Checked;
            if (isChecked)
            {
                return true;
            }
        }
        return false;
    }
    // Tertiary bar ##############################################
    // Handle mark complete
    protected void clickMarkComplete(object sender, EventArgs e)
    {
        if (lineItemPresent())
        {
            for (int i = 0; i < recVidsRepeater.Items.Count; i++)
            {
                RepeaterItem item = recVidsRepeater.Items[i];
                bool isChecked = ((CheckBox)item.FindControl("rowLevelCheckBox")).Checked;
                if (isChecked)
                {
                    int recID = Convert.ToInt32(((Label)item.FindControl("recordID")).Text);
                    markComplete(recID);
                }
            }
            BindrecVidsRepeater();
            messageBox box = new messageBox("success", "Success, the videos you selected have been marked completed.", noticeLabel);

        }
        else
        {
            messageBox box = new messageBox("notice", "You have not selected any videos.", noticeLabel);
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
            messageBox box = new messageBox("error", "There has been an error: [" + ex.Message + "]", noticeLabel);
        }
        finally
        {
            conn.Close();
        }

    }
    // Hanlde mark incomplete
    protected void clickMarkIncomplete(object sender, EventArgs e)
    {
        if (lineItemPresent())
        {
            for (int i = 0; i < recVidsRepeater.Items.Count; i++)
            {
                RepeaterItem item = recVidsRepeater.Items[i];
                bool isChecked = ((CheckBox)item.FindControl("rowLevelCheckBox")).Checked;
                if (isChecked)
                {
                    int recID = Convert.ToInt32(((Label)item.FindControl("recordID")).Text);
                    markInComplete(recID);
                }
            }
            BindrecVidsRepeater();
            messageBox box = new messageBox("success", "Success, completion date has been removed.", noticeLabel);

        }
        else
        {
            messageBox box = new messageBox("notice", "You have not selected any videos.", noticeLabel);
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
            messageBox box = new messageBox("error", "There has been an error: [" + ex.Message + "]", noticeLabel);
        }
        finally
        {
            conn.Close();
        }

    }
    // Handle hide complete
    protected void clickHideComplete(object sender, EventArgs e)
    {
        BindrecVidsRepeaterHide();
    }
    protected void BindrecVidsRepeaterHide()
    {
        recVidsRepeater.Visible = true;
        int userID = Convert.ToInt32(Session["userLoggedIn"].ToString());
        SqlConnection conn;
        DataSet recVideoDS = new DataSet();
        DataSet recVideoIdDS = new DataSet();
        SqlDataAdapter adapter;
        SqlDataAdapter adapterRV;
        SqlCommand validateComm;
        SqlDataReader reader;
        string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
        conn = new SqlConnection(connectionString);
        validateComm = new SqlCommand("SELECT recID FROM recommendedVideos WHERE userID=" + userID.ToString() + " AND completedDate IS NOT NULL", conn);

        conn.Open();
        reader = validateComm.ExecuteReader();
        if (!reader.Read())
        {
            messageBox box = new messageBox("notice", "You have not marked any videos complete yet.", noticeLabel);
            reader.Close();
            conn.Close();
        }
        else
        {
            conn.Close();
            adapter = new SqlDataAdapter("SELECT videoID, userID, sortID FROM recommendedVideos WHERE userID=" + userID.ToString() + " AND completedDate IS NULL ORDER BY sortID", conn);
            adapter.Fill(recVideoDS, "videoIDDS");

            foreach (DataRow dr in recVideoDS.Tables[0].Rows)
            {
                adapterRV = new SqlDataAdapter("SELECT [recommendedVideos].[recID],[recommendedVideos].[videoID],[recommendedVideos].[userID], [recommendedVideos].[sortID], " +
                                            "[recommendedVideos].[createdDate], [recommendedVideos].[completedDate], [videoInfo].[videoInfoID], " +
                                            "[videoInfo].[videoName], [videoAuthor].[authorName], [videoInfo].[description], [videoInfo].[lenght], " +
                                            "[videoCategory].[categoryName], [videoAuthor].[authTitle], [videoInfo].[keyWords] " +
                                            "FROM [videoAuthor] " +
                                            "JOIN [videoInfo] ON [videoAuthor].[videoAuthID] = [videoInfo].[videoAuthID] " +
                                            "JOIN [recommendedVideos] on [videoInfo].[videoInfoID] = [recommendedVideos].[videoID] " +
                                            "JOIN [videoCategory] ON [videoInfo].[videoCatID] = [videoCategory].[VideoCatID] " +
                                            "WHERE [recommendedVideos].[videoID]=" + Convert.ToInt32(dr["videoID"]) + " AND [recommendedVideos].[userID]=" + Convert.ToInt32(dr["userID"]) + "", conn);
                adapterRV.Fill(recVideoIdDS, "recVideosID");

            }

            if (recVideoIdDS.Tables.Count > 0)
            {
                messageBox box = new messageBox("notice", "Completed videos have been hidden. You can use the <b>Show All</b> button above ", noticeLabel);
                recVidsRepeater.DataSource = recVideoIdDS;
                recVidsRepeater.DataBind();
                for (int i = 0; i < recVidsRepeater.Items.Count; i++)
                {
                    Label completed = (Label)recVidsRepeater.Items[i].FindControl("labelCompleted");
                    Label divClass = (Label)recVidsRepeater.Items[i].FindControl("labelTableClass");
                    if (completed.Text != "")
                    {
                        divClass.Text = "<div class=\"tableHdrRepeaterComplete\">";
                    }
                    else
                    {
                        divClass.Text = "<div class=\"tableHdrRepeater\">";
                    }

                }

            }
            else
            {
                noticeLabel.Visible = true;
                messageBox box = new messageBox("success", "Congratulations! All videos have already been completed. If list of completed videos does not appear click the <b>Show All</b> button above. Thank you.", noticeLabel);
            }
        }
    }
    // Handle show all
    protected void clickShowAll(object sender, EventArgs e)
    {
        BindrecVidsRepeater();
    }
}
