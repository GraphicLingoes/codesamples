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

public partial class memberVideos : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Check if user is logged in else redirect them to login page
        if (!globalFunctions.validateLogin())
        {
            Response.Redirect("Logout.aspx");
        }
        categoryVidsLB.ForeColor = System.Drawing.ColorTranslator.FromHtml("#9CC5C9");
        allVidsLB.ForeColor = System.Drawing.Color.White;
        confirmVideoDiv.Attributes["style"] = String.Format("visibility: hidden; height: 1px;");
        backVidsIB.Visible = false;
        backVidsLB.Visible = false;

        if (!IsPostBack)
        {   
            // Bind video info grid
            BindVideoCatGrid();
            // Determine wether to reset session or not
            string lastPageNotPB = Request.UrlReferrer.Segments[Request.UrlReferrer.Segments.Length - 1].ToString();
            if(Session["videoDetailID"] != null)
            {
            confirmVideoDiv.Attributes["style"] = String.Format("visibility: visible;");
            SqlConnection conn;
            SqlCommand comm;
            SqlDataReader reader;
            string videoInfoID = Session["videoDetailID"].ToString();
            string ConnectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
            conn = new SqlConnection(ConnectionString);
            comm = new SqlCommand("SELECT videoName FROM videoInfo WHERE videoInfoID=" + videoInfoID + "", conn);
            conn.Open();
            reader = comm.ExecuteReader();
            reader.Read();
            confirmMessage.Text = "<div class='searchNotice'>Pressing <b>Continue to Video</b> below will add the <b>" +
                reader["videoName"].ToString() + "</b> video to " +
                "your video history list. To view your video history click on the <b>Home</b> " +
                "icon on the ribbon bar above.<br /><br /><b>NOTE:</b> This action does not mark your video complete, it only keeps track of the date you watched it.</div>";
            conn.Close();
            reader.Close(); 
            videoCatGrid.Visible = false;
            categoryVidsLB.ForeColor = System.Drawing.Color.White;
            allVidsLB.ForeColor = System.Drawing.Color.White;
            
                Boolean confirmed = Convert.ToBoolean(Session["confirmVideo"]);
               //Check for confirmed button event.
                if (lastPageNotPB == "memberVideos.aspx" && confirmed)
                    {
                        confirmVideoDiv.Attributes["style"] = String.Format("visibility: hidden; height: 1px;");
                        videoDetail.Attributes["style"] = String.Format("visibility: visible; text-align: center;");
                        categoryVidsLB.ForeColor = System.Drawing.Color.White;
                        videoCatGrid.Visible = false;

                        string screenCastInfo;
                        // Load Video
                        int vidID = Convert.ToInt32(Session["videoDetailID"]);
                        screenCastInfo = globalFunctions.GenerateVideoCode(vidID);
                        screenCastVideo.Text = screenCastInfo;
                        conn = new SqlConnection(ConnectionString);
                        comm = new SqlCommand("SELECT videoName, lenght FROM videoInfo WHERE videoInfoID=" + videoInfoID + "", conn);

                        conn.Open();
                        reader = comm.ExecuteReader();
                        reader.Read();
                        videoTitle.Text = "You are watching: " + reader["videoName"].ToString() + "  |  Length: " + reader["lenght"].ToString() + "";
                        conn.Close();
                        reader.Close();
                        Session["confirmVideo"] = false;
                        Session["videoDetailID"] = null;
                    }
            }

            
        
        }
    }

    // Handle video categories button
    public void showCategories(object s, EventArgs e)
    {
        Session["videoDetailID"] = null;
        categoryVidsLB.ForeColor = System.Drawing.ColorTranslator.FromHtml("#9CC5C9");
        allVidsLB.ForeColor = System.Drawing.Color.White;
        Response.Redirect("memberVideos.aspx");
    }
    // Handle all videos button
    public void showAllVideos(object s, EventArgs e)
    {
        Session["videoDetailID"] = null;
        categoryVidsLB.ForeColor = System.Drawing.Color.White;
        allVidsLB.ForeColor = System.Drawing.ColorTranslator.FromHtml("#9CC5C9");
        expandAllVids();
    }
    // Bind Video Grid
    private void BindVideoCatGrid()
    {
        SqlConnection conn;
        DataSet dataSet = new DataSet();
        SqlDataAdapter adapter;
        if (ViewState["allVideosCatDataSet"] == null)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
            conn = new SqlConnection(connectionString);
            adapter = new SqlDataAdapter("SELECT videoCatID, categoryName FROM videoCategory ORDER BY categoryName", conn);
            adapter.Fill(dataSet, "allVideosCat");
            ViewState["allVideosCatDataSet"] = dataSet;
        }
        else
        {
            dataSet = (DataSet)ViewState["allVideosCatDataSet"];
        }
        string sortExpressionVidsCat;
        if (gridSortDirectionVidsCat == SortDirection.Ascending)
        {
            sortExpressionVidsCat = gridSortExpressionVidsCat + " ASC";
        }
        else
        {
            sortExpressionVidsCat = gridSortExpressionVidsCat + " DESC";
        }
        dataSet.Tables["allVideosCat"].DefaultView.Sort = sortExpressionVidsCat;
        videoCatGrid.DataSource = dataSet.Tables["allVideosCat"].DefaultView;
        videoCatGrid.DataKeyNames = new string[] { "videoCatID" };
        videoCatGrid.DataBind();

    }
    // Add Paging to videos Grid View
    protected void videoCatGrid_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        int newPageIndex = e.NewPageIndex;
        videoCatGrid.PageIndex = newPageIndex;
        BindVideoCatGrid();
    }
    // Handle video sorting
    protected void videoCatGrid_Sorting(object sender, GridViewSortEventArgs e)
    {
        string sortExpressionVidsCat = e.SortExpression;
        if (sortExpressionVidsCat == gridSortExpressionVidsCat)
        {
            if (gridSortDirectionVidsCat == SortDirection.Ascending)
            {
                gridSortDirectionVidsCat = SortDirection.Descending;
            }
            else
            {
                gridSortDirectionVidsCat = SortDirection.Ascending;
            }
        }
        else
        {
            gridSortDirectionVidsCat = SortDirection.Ascending;
        }
        gridSortExpressionVidsCat = sortExpressionVidsCat;
        BindVideoCatGrid();
    }
    private SortDirection gridSortDirectionVidsCat
    {
        get
        {
            if (ViewState["GridSortDirectionVidsCat"] == null)
            {
                ViewState["GridSortDirectionVidsCat"] = SortDirection.Ascending;
            }
            return (SortDirection)ViewState["GridSortDirectionVidsCat"];
        }
        set
        {
            ViewState["GridSortDirectionVidsCat"] = value;
        }
    }

    private string gridSortExpressionVidsCat
    {
        get
        {
            if (ViewState["GridSortExpressionVidsCat"] == null)
            {
                ViewState["GridSortExpressionVidsCat"] = "categoryName";
            }
            return (string)ViewState["GridSortExpressionVidsCat"];
        }
        set
        {
            ViewState["GridSortExpressionVidsCat"] = value;
        }
    }

    // Create event handler to respond to image button click of video from category view or search view.
    override protected void OnInit(EventArgs e)
    {
        base.OnInit(e);
        allVidsRepeater.ItemCommand += new RepeaterCommandEventHandler(allVidsRepeater_ItemCommand);
    }
    private void allVidsRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName == "videoRedirect")
        {
            Session["videoDetailID"] = e.CommandArgument;
            Response.Redirect("memberVideos.aspx");
        }
    }

    // Bind Details to video detail view when video selected
    protected void videoCatGrid_SelectedIndexChanged(object sender, EventArgs e)
    {
        BindallVidsRepeater();
        allVidsLB.ForeColor = System.Drawing.Color.White;
        categoryVidsLB.ForeColor = System.Drawing.Color.White;
    }

    private void BindallVidsRepeater()
    {
        videoCatGrid.Visible = false;
        allVidsRepeater.Visible = true;
        backVidsIB.Visible = true;
        backVidsLB.Visible = true;
        int selectRowIndex = videoCatGrid.SelectedIndex;
        int videoCatID = (int)videoCatGrid.DataKeys[selectRowIndex].Values["videoCatID"];
        SqlConnection conn;
        SqlCommand comm;
        SqlCommand commh2;
        SqlDataReader reader;
        string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
        conn = new SqlConnection(connectionString);
        comm = new SqlCommand("SELECT videoInfo.videoInfoID, [videoInfo].videoName, videoAuthor.authorName, videoInfo.description, videoInfo.lenght, videoCategory.categoryName, " +
                                "videoAuthor.authTitle, videoInfo.link, videoInfo.fileName, videoInfo.keyWords FROM videoAuthor " +
                                "JOIN videoInfo ON videoAuthor.videoAuthID = videoInfo.videoAuthID " +
                                "JOIN videoCategory ON videoInfo.videoCatID = videoCategory.VideoCatID " +
                                "WHERE videoInfo.videoCatID=@videoCatID", conn);
        comm.Parameters.Add("videoCatID", SqlDbType.Int);
        comm.Parameters["videoCatID"].Value = videoCatID;
        commh2 = new SqlCommand("SELECT categoryName FROM videoCategory WHERE videoCatID=@videoCatID", conn);
        commh2.Parameters.Add("videoCatID", SqlDbType.Int);
        commh2.Parameters["videoCatID"].Value = videoCatID;
        try
        {
            conn.Open();
            reader = comm.ExecuteReader();
            allVidsRepeater.DataSource = reader;
            allVidsRepeater.DataBind();
            reader.Close();
            reader = commh2.ExecuteReader();
            while(reader.Read())
            {
                categoryNameh2.Text = "Video Results for: " + Convert.ToString(reader["categoryName"]);
            }
            reader.Close();
        }
        finally
        {
            conn.Close();
        }
    }

        private void expandAllVids()
        {
        videoCatGrid.Visible = false;
        allVidsRepeater.Visible = true;
        backVidsIB.Visible = true;
        backVidsLB.Visible = true;
        SqlConnection conn;
        SqlCommand comm;
        SqlDataReader reader;
        string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
        conn = new SqlConnection(connectionString);
        comm = new SqlCommand("SELECT videoInfo.videoInfoID, [videoInfo].videoName, videoAuthor.authorName, videoInfo.description, videoInfo.lenght, videoCategory.categoryName, " +
                                "videoAuthor.authTitle, videoInfo.link, videoInfo.fileName, videoInfo.keyWords FROM videoAuthor " +
                                "JOIN videoInfo ON videoAuthor.videoAuthID = videoInfo.videoAuthID " +
                                "JOIN videoCategory ON videoInfo.videoCatID = videoCategory.VideoCatID ORDER BY [videoInfo].videoName ASC", conn);

        try
        {
            conn.Open();
            reader = comm.ExecuteReader();
            allVidsRepeater.DataSource = reader;
            allVidsRepeater.DataBind();
            reader.Close();
        }
        finally
        {
            conn.Close();
        }
    }

    // Handle confirm button
        protected void confirmVideo(object sender, EventArgs e)
        {
            if (Session["videoDetailID"] == null)
            {
                messageBox box = new messageBox("error", "There has been an error please <a href='memberVideos.aspx'>Click Here</a> to try again.", errorMessage);
            }
            else
            {
                
                string videoInfoID = Session["videoDetailID"].ToString();
                string userID = Session["userLoggedIn"].ToString();
                SqlConnection conn;
                SqlCommand comm;
                string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
                conn = new SqlConnection(connectionString);
                comm = new SqlCommand("INSERT INTO videoTracking (videoInfoID, userID, createdDate) VALUES (" + videoInfoID + ", " + userID + ", GETDATE())", conn);


                try
                {
                    conn.Open();
                    comm.ExecuteNonQuery();

                }
                catch (SqlException ex)
                {

                    errorMessage.Text =
                        "<div class=\"errorMessageSearch\">Error submitting your registration try again later, and/or change the entered data.<br /> [" + ex.Message + "]</div>";
                }
                finally
                {
                    conn.Close();
                    Session["confirmVideo"] = true;
                    Response.Redirect("memberVideos.aspx");
                }
            }
        }

    // Handle go back button    
    protected void goBackVid(object sender, EventArgs e)
        {
            Session["videoDetailID"] = null;
            Session["confirmVideo"] = false;
            Response.Redirect("memberVideos.aspx");
        }
}
