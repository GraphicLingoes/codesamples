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

public partial class myImp : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Check if user is logged in else redirect them to login page
        if (!globalFunctions.validateLogin())
        {
            Response.Redirect("Logout.aspx");
        }

        if (!IsPostBack)
        {
            initalNotice.Visible = true;
            editionName.Text = Session["lmdEditionName"].ToString();
            editionId.Text = Session["lmdEditionId"].ToString();
        }
    }

    // Bind Video Grid
    private void BindImpStepGrid()
    {
        int videoEdID = Convert.ToInt32(Session["editionSelect"]);
        SqlConnection conn;
        DataSet dataSet = new DataSet();
        SqlDataAdapter adapter;
        if (ViewState["VideosImpDataSet"] == null)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
            conn = new SqlConnection(connectionString);
            adapter = new SqlDataAdapter("SELECT impSteps.impStepID, impSteps.impStepName, impSteps.Priority, impSteps.Description, " +
                                         "impStepEditions.impStepEdID, impStepEditions.videoEdID " +
                                         "FROM impStepEditions " +
                                         "JOIN impSteps ON impStepEditions.impStepID = impSteps.impStepID " +
                                         "WHERE impStepEditions.videoEdID = " + videoEdID + "ORDER BY impSteps.Priority ASC", conn);
            adapter.Fill(dataSet, "VideosImp");
            ViewState["VideosImpDataSet"] = dataSet;
        }
        else
        {
            dataSet = (DataSet)ViewState["VideosImpDataSet"];
        }
        string sortExpressionVidsImp;
        if (gridSortDirectionVidsImp == SortDirection.Ascending)
        {
            sortExpressionVidsImp = gridSortExpressionVidsImp + " ASC";
        }
        else
        {
            sortExpressionVidsImp = gridSortExpressionVidsImp + " DESC";
        }
        dataSet.Tables["VideosImp"].DefaultView.Sort = sortExpressionVidsImp;
        videoImpGrid.DataSource = dataSet.Tables["VideosImp"].DefaultView;
        videoImpGrid.DataKeyNames = new string[] { "impStepID" };
        videoImpGrid.DataBind();

    }
    // Add Paging to videos Grid View
    protected void videoImpGrid_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        int newPageIndex = e.NewPageIndex;
        videoImpGrid.PageIndex = newPageIndex;
        BindImpStepGrid();
    }
    // Handle video sorting
    protected void videoImpGrid_Sorting(object sender, GridViewSortEventArgs e)
    {
        string sortExpressionVidsImp = e.SortExpression;
        if (sortExpressionVidsImp == gridSortExpressionVidsImp)
        {
            if (gridSortDirectionVidsImp == SortDirection.Ascending)
            {
                gridSortDirectionVidsImp = SortDirection.Descending;
            }
            else
            {
                gridSortDirectionVidsImp = SortDirection.Ascending;
            }
        }
        else
        {
            gridSortDirectionVidsImp = SortDirection.Ascending;
        }
        gridSortExpressionVidsImp = sortExpressionVidsImp;
        BindImpStepGrid();
    }
    private SortDirection gridSortDirectionVidsImp
    {
        get
        {
            if (ViewState["GridSortDirectionVidsImp"] == null)
            {
                ViewState["GridSortDirectionVidsImp"] = SortDirection.Ascending;
            }
            return (SortDirection)ViewState["GridSortDirectionVidsImp"];
        }
        set
        {
            ViewState["GridSortDirectionVidsImp"] = value;
        }
    }

    private string gridSortExpressionVidsImp
    {
        get
        {
            if (ViewState["GridSortExpressionVidsImp"] == null)
            {
                ViewState["GridSortExpressionVidsImp"] = "impStepID";
            }
            return (string)ViewState["GridSortExpressionVidsImp"];
        }
        set
        {
            ViewState["GridSortExpressionVidsImp"] = value;
        }
    }

    // Bind Details to video  repeater when video selected
    protected void videoImpGrid_SelectedIndexChanged(object sender, EventArgs e)
    {
        BindguideVidsRepeater();
    }
    
    private void BindguideVidsRepeater()
    {
        guideVidsRepeater.Visible = true;
        int selectRowIndex = videoImpGrid.SelectedIndex;
        int impStepID = (int)videoImpGrid.DataKeys[selectRowIndex].Values["impStepID"];
       
        SqlConnection conn;
        SqlCommand comm;
        SqlDataReader reader;
        string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
        conn = new SqlConnection(connectionString);
        comm = new SqlCommand("SELECT videoInfo.videoInfoID, [videoInfo].videoName, videoAuthor.authorName, videoInfo.description, videoInfo.lenght, " +
                                "videoAuthor.authTitle, videoInfo.link, videoInfo.fileName, videoInfo.keyWords, impStepVids.impStepID, impSteps.impStepName FROM videoAuthor " +
                                "JOIN videoInfo ON videoAuthor.videoAuthID = videoInfo.videoAuthID " +
                                "JOIN impStepVids ON videoInfo.videoInfoID = impStepVids.videoInfoID " +
                                "JOIN impSteps ON impStepVids.impStepID = impSteps.impStepID " +
                                "WHERE impStepVids.impStepID=@impStepID", conn);
        comm.Parameters.Add("impStepID", SqlDbType.Int);
        comm.Parameters["impStepID"].Value = impStepID;
        try
        {
            conn.Open();
            reader = comm.ExecuteReader();
            guideVidsRepeater.DataSource = reader;
            guideVidsRepeater.DataBind();
            reader.Close();
            reader = comm.ExecuteReader();
            reader.Read();
            if (reader.HasRows)
            {
                Label h2Header = (Label)guideVidsRepeater.Controls[0].Controls[0].FindControl("h2Label");
                h2Header.Text = reader["impStepName"].ToString();
                reader.Close();
            }
            else
            {
                Label h2Header = (Label)guideVidsRepeater.Controls[0].Controls[0].FindControl("h2Label");
                h2Header.Text = "There are no videos associated to this step";
                reader.Close();
            }
        }
        finally
        {
            conn.Close();
        }
    }

    protected void editionSelectHandler(Object sender, EventArgs e)
    {

        guideVidsRepeater.Visible = false;
        ImageButton btn = (ImageButton)sender;
        switch (btn.CommandName)
        {
            case "standardSelect":
                ViewState["VideosImpDataSet"] = null;
                Session["editionSelect"] = btn.CommandArgument.ToString();
                initalNotice.Visible = false;
                BindImpStepGrid();
                break;
            case "claimsSelect":
                ViewState["VideosImpDataSet"] = null;
                Session["editionSelect"] = btn.CommandArgument.ToString();
                initalNotice.Visible = false;
                BindImpStepGrid();
                break;
            case "officeSelect":
                ViewState["VideosImpDataSet"] = null;
                Session["editionSelect"] = btn.CommandArgument.ToString();
                initalNotice.Visible = false;
                BindImpStepGrid();
                break;
            case "proSelect":
                ViewState["VideosImpDataSet"] = null;
                Session["editionSelect"] = btn.CommandArgument.ToString();
                initalNotice.Visible = false;
                BindImpStepGrid();
                break;
        }
    }

    // Handle mark complete link in grid
    protected void videoImpGrid_RowCommand(Object sender, GridViewCommandEventArgs e)
    {
        // If multiple buttons are used in a GridView control, use the
        // CommandName property to determine which button was clicked.
        if (e.CommandName == "markComplete")
        {
            // Convert the row index stored in the CommandArgument
            // property to an Integer.
            int index = Convert.ToInt32(e.CommandArgument);
            int impStepID = (int)videoImpGrid.DataKeys[index].Value;

            // Retrieve the row that contains the button clicked 
            // by the user from the Rows collection.
            GridViewRow row = videoImpGrid.Rows[index];
            completeConfirmDiv.Style.Clear();
            Session["completeImpStep"] = impStepID;
            btnCancelConfirm.Visible = true;
            btnCompleteConfirm.Visible = true;
            ConfirmConfirmation.Text = "<div class=\"searchNotice\">Click \"Confirm Completion\" below to add implementation step to completed list confirmation. NOTE: Completed steps will show up on the member dashboard when you log in.</div>";
        }
    }

    // Handle complete confirm button
    protected void completeTrue(object sender, EventArgs e)
    {
        string userID = Session["userLoggedIn"].ToString();
        SqlConnection conn;
        SqlCommand checkDupComm;
        SqlCommand comm;
        SqlDataReader reader;
        string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
        conn = new SqlConnection(connectionString);
        checkDupComm = new SqlCommand("SELECT impTrackingID FROM impTracking WHERE userID=" + userID + " AND impStepID=" + Session["completeImpStep"] + "", conn);
        comm = new SqlCommand("INSERT INTO impTracking (userID, impStepID, createdDate) VALUES (" + userID + ", " + Session["completeImpStep"] + ", GETDATE())", conn);

        conn.Open();
        reader = checkDupComm.ExecuteReader();
        if (reader.Read())
        {
            completeConfirmDiv.Style.Clear();
            ConfirmConfirmation.Text = "<div class=\"errorMessageSearch\">You have already added this step to your completed list. <a href=\"myImp.aspx\">Click Here</a> to return to list. or <a href=\"memberDashboard.aspx\">Click Here</a> to see a list of completed steps.</div>";
            btnCancelConfirm.Visible = false;
            btnCompleteConfirm.Visible = false;
            conn.Close();
        }
        else
        {
            try
            {
                conn.Close();
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
                completeConfirmDiv.Style.Clear();
                ConfirmConfirmation.Text = "<div class=\"success\">Item has been marked complete and can be viewed from the <a href=\"newDashboard.aspx\">Member Dashboard</a>.</div>";
                btnCancelConfirm.Visible = false;
                btnCompleteConfirm.Visible = false;
            }
        }
    }
    // Handle cancel confirm button
    protected void cancelConfirm(object sender, EventArgs e)
    {
        completeConfirmDiv.Style.Add("display", "none");
        Response.Redirect("myImp.aspx");
    }

    // Create event handler to respond to image button click of video from category view or search view.
    override protected void OnInit(EventArgs e)
    {
        base.OnInit(e);
        guideVidsRepeater.ItemCommand += new RepeaterCommandEventHandler(guideVidsRepeater_ItemCommand);
    }

    private void guideVidsRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName == "videoRedirect")
        {
            Session["videoDetailID"] = e.CommandArgument;
            Response.Redirect("memberVideos.aspx");
        }
    }

}
