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


public partial class admin_videosList : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // Bind video info grid
            BindVideoGrid();
            h3Title.Text = "Videos List";
            backToVidListIB.Visible = false;
            backToVidListLB.Visible = false;

            // Handle mini ribbon color change if menu item active
            LinkButton mpLinkButtonM = (LinkButton)Master.FindControl("membersLB");
            mpLinkButtonM.ForeColor = System.Drawing.Color.White;

            LinkButton mpLinkButtonV = (LinkButton)Master.FindControl("videosLB");
            mpLinkButtonV.ForeColor = System.Drawing.ColorTranslator.FromHtml("#9CC5C9");

            LinkButton mpLinkButtonIS = (LinkButton)Master.FindControl("helpPagesLB");
            mpLinkButtonIS.ForeColor = System.Drawing.Color.White;

            LinkButton mpLinkButtonRC = (LinkButton)Master.FindControl("recVidsLB");
            mpLinkButtonRC.ForeColor = System.Drawing.Color.White;

            LinkButton mpLinkButtonS = (LinkButton)Master.FindControl("searchLB");
            mpLinkButtonS.ForeColor = System.Drawing.Color.White;
        }
        else
        {
            backToVidListIB.Visible = true;
            backToVidListLB.Visible = true;
        }
    }

   // Handle back to videos main page
    protected void backToMain(object sender, EventArgs e)
    {
        Response.Redirect("videos.aspx");
    }

    // Handle back to video list button
    protected void backToVidList(object sender, EventArgs e)
    {
        Response.Redirect("videosList.aspx");
    }
    
    // Bind Video Grid
    private void BindVideoGrid()
    {
        SqlConnection conn;
        DataSet dataSet = new DataSet();
        SqlDataAdapter adapter;
        if (ViewState["VideosDataSet"] == null)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
            conn = new SqlConnection(connectionString);
            adapter = new SqlDataAdapter("SELECT videoInfo.videoInfoID, [videoInfo].videoName, videoAuthor.authorName, videoInfo.[description], videoInfo.lenght, videoCategory.categoryName, videoInfo.link, videoInfo.[fileName], videoInfo.keyWords " +
                                            "FROM videoAuthor " +
                                            "JOIN videoInfo ON videoAuthor.videoAuthID = videoInfo.videoAuthID " +
                                            "JOIN videoCategory ON videoInfo.videoCatID = videoCategory.videoCatID " +
                                            "ORDER BY videoInfoID", conn);
            adapter.Fill(dataSet, "Videos");
            ViewState["VideosDataSet"] = dataSet;
        }
        else
        {
            dataSet = (DataSet)ViewState["VideosDataSet"];
        }
        string sortExpressionVids;
        if (gridSortDirectionVids == SortDirection.Ascending)
        {
            sortExpressionVids = gridSortExpressionVids + " ASC";
        }
        else
        {
            sortExpressionVids = gridSortExpressionVids + " DESC";
        }
        dataSet.Tables["Videos"].DefaultView.Sort = sortExpressionVids;
        videoInfoGrid.DataSource = dataSet.Tables["Videos"].DefaultView;
        videoInfoGrid.DataKeyNames = new string[] { "videoInfoID" };
        videoInfoGrid.DataBind();

    }
    // Add Paging to videos Grid View
    protected void videoInfoGrid_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        int newPageIndex = e.NewPageIndex;
        videoInfoGrid.PageIndex = newPageIndex;
        BindVideoGrid();
    }
    // Handle video sorting
    protected void videoInfoGrid_Sorting(object sender, GridViewSortEventArgs e)
    {
        string sortExpressionVids = e.SortExpression;
        if (sortExpressionVids == gridSortExpressionVids)
        {
            if (gridSortDirectionVids == SortDirection.Ascending)
            {
                gridSortDirectionVids = SortDirection.Descending;
            }
            else
            {
                gridSortDirectionVids = SortDirection.Ascending;
            }
        }
        else
        {
            gridSortDirectionVids = SortDirection.Ascending;
        }
        gridSortExpressionVids = sortExpressionVids;
        BindVideoGrid();
    }
    private SortDirection gridSortDirectionVids
    {
        get
        {
            if (ViewState["GridSortDirectionVids"] == null)
            {
                ViewState["GridSortDirectionVids"] = SortDirection.Ascending;
            }
            return (SortDirection)ViewState["GridSortDirectionVids"];
        }
        set
        {
            ViewState["GridSortDirectionVids"] = value;
        }
    }

    private string gridSortExpressionVids
    {
        get
        {
            if (ViewState["GridSortExpressionVids"] == null)
            {
                ViewState["GridSortExpressionVids"] = "videoInfoID";
            }
            return (string)ViewState["GridSortExpressionVids"];
        }
        set
        {
            ViewState["GridSortExpressionVids"] = value;
        }
    }
    // Bind Details to video detail view when video selected
    protected void videoInfoGrid_SelectedIndexChanged(object sender, EventArgs e)
    {
        videoDetail.ChangeMode(DetailsViewMode.ReadOnly);
        Session["videoEdit"] = "";
        videoInfoGrid.Visible = false;
        BindVideoDetails();
        h3Title.Text = "Video Detail Information";
    }
    private void BindVideoDetails()
    {
        int selectRowIndex = videoInfoGrid.SelectedIndex;
        int videoInfoID = (int)videoInfoGrid.DataKeys[selectRowIndex].Values["videoInfoID"];
        SqlConnection conn;
        SqlCommand comm;
        SqlCommand vidEdComm;
        SqlCommand impVidComm;
        SqlDataReader reader;
        string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
        conn = new SqlConnection(connectionString);
        comm = new SqlCommand("SELECT videoInfo.videoInfoID, [videoInfo].videoName, videoAuthor.authorName, videoInfo.description, videoInfo.lenght, videoCategory.categoryName, " +
                                "videoInfo.link, videoInfo.fileName, videoInfo.keyWords, videoInfo.width, videoInfo.height FROM videoAuthor " +
                                "JOIN videoInfo ON videoAuthor.videoAuthID = videoInfo.videoAuthID " +
                                "JOIN videoCategory ON videoInfo.videoCatID = videoCategory.VideoCatID " +
                                "WHERE videoInfoID=@videoInfoID", conn);
        comm.Parameters.Add("videoInfoID", SqlDbType.Int);
        comm.Parameters["videoInfoID"].Value = videoInfoID;
        vidEdComm = new SqlCommand("SELECT videoEditions.videoEditionsID, videoEditions.videoEdID, videoEditions.videoInfoID, videoEdition.edition FROM videoEditions " +
                                    "JOIN videoEdition ON videoEditions.videoEdID = videoEdition.videoEdID " +
                                    "WHERE videoEditions.videoInfoID=@videoInfoID", conn);
        vidEdComm.Parameters.Add("videoInfoID", SqlDbType.Int);
        vidEdComm.Parameters["videoInfoID"].Value = videoInfoID;
        impVidComm = new SqlCommand("SELECT impStepVids.impStepVidID, impStepVids.impStepID, impStepVids.videoInfoID, impSteps.impStepName FROM impStepVids " +
                                "JOIN impSteps ON impStepVids.impStepID = impSteps.impStepID " +
                                "WHERE impStepVids.videoInfoID=@videoInfoID", conn);
        impVidComm.Parameters.Add("videoInfoID", SqlDbType.Int);
        impVidComm.Parameters["videoInfoID"].Value = videoInfoID;
        try
        {
            conn.Open();
            reader = comm.ExecuteReader();
            videoDetail.DataSource = reader;
            videoDetail.DataKeyNames = new string[] { "videoInfoID" };
            videoDetail.DataBind();
            reader.Close();
            conn.Close();
            conn.Open();
            reader = vidEdComm.ExecuteReader();
            if (Session["videoEdit"] != "true")
            {
                CheckBoxList cboEdCb = (CheckBoxList)videoDetail.FindControl("videoEdDetail");
                cboEdCb.DataSource = reader;
                cboEdCb.DataTextField = "edition";
                cboEdCb.DataValueField = "videoEdID";
                cboEdCb.DataBind();
                reader.Close();
            }
            else
            {
                reader.Close();
            }
            reader = impVidComm.ExecuteReader();
            if (Session["videoEdit"] != "true")
            {
                ListBox cboImpVids = (ListBox)videoDetail.FindControl("videoImpStepsDetail");
                cboImpVids.DataSource = reader;
                cboImpVids.DataTextField = "impStepName";
                cboImpVids.DataValueField = "impStepID";
                cboImpVids.DataBind();
                reader.Close();
            }
            else
            {
                reader.Close();
            }

        }
        finally
        {
            conn.Close();
        }
    }
    //************************************************************************************* Edit mode video details grid **********************

    protected void videoDetail_ModeChanging(object sender, DetailsViewModeEventArgs e)
    {
        {
            if (!e.CancelingEdit)
            {
                videoDetail.ChangeMode(e.NewMode);
                Session["videoEdit"] = "true";
                h3Title.Text = "Edit Video Information";
                BindVideoDetails();
                int selectRowIndex = videoInfoGrid.SelectedIndex;
                int videoInfoID = (int)videoInfoGrid.DataKeys[selectRowIndex].Value;
                SqlConnection conn;
                SqlCommand comm;
                SqlCommand ddlAuthComm;
                SqlCommand cbEdComm;
                SqlCommand cbEdCKComm;
                SqlCommand ddlCatComm;
                SqlCommand impStepsComm;
                SqlCommand impStepsCKComm;
                SqlDataReader reader;
                string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
                conn = new SqlConnection(connectionString);
                comm = new SqlCommand("SELECT videoInfo.videoInfoID, [videoInfo].videoName, videoAuthor.videoAuthID, videoAuthor.authorName, videoInfo.description, videoInfo.lenght, videoCategory.videoCatID, videoCategory.categoryName, " +
                                "videoInfo.link, videoInfo.fileName, videoInfo.keyWords, videoInfo.width, videoInfo.height FROM videoAuthor " +
                                "JOIN videoInfo ON videoAuthor.videoAuthID = videoInfo.videoAuthID " +
                                "JOIN videoCategory ON videoInfo.videoCatID = videoCategory.VideoCatID " +
                                "WHERE videoInfoID=@videoInfoID", conn);
                ddlAuthComm = new SqlCommand("SELECT videoAuthID, authorName FROM videoAuthor", conn);
                cbEdComm = new SqlCommand("SELECT videoEdID, edition FROM videoEdition", conn);
                cbEdCKComm = new SqlCommand("SELECT videoEdID FROM videoEditions WHERE videoInfoID=@videoInfoID", conn);
                ddlCatComm = new SqlCommand("SELECT videoCatID, categoryName FROM videoCategory", conn);
                impStepsComm = new SqlCommand("SELECT impStepID, impStepName, Priority FROM impSteps ORDER BY Priority ASC", conn);
                impStepsCKComm = new SqlCommand("SELECT impStepID FROM impStepVids WHERE videoInfoID=@videoInfoID", conn);
                comm.Parameters.Add("videoInfoID", SqlDbType.Int);
                comm.Parameters["videoInfoID"].Value = videoInfoID;
                cbEdCKComm.Parameters.Add("videoInfoID", SqlDbType.Int);
                cbEdCKComm.Parameters["videoInfoID"].Value = videoInfoID;
                impStepsCKComm.Parameters.Add("videoInfoID", SqlDbType.Int);
                impStepsCKComm.Parameters["videoInfoID"].Value = videoInfoID;

                try
                {
                    conn.Open();
                    //Bind video author drop down list
                    reader = ddlAuthComm.ExecuteReader();
                    DropDownList cboAuthor = (DropDownList)videoDetail.FindControl("ddlVidAuthorEdit");
                    cboAuthor.DataSource = reader;
                    cboAuthor.DataValueField = "videoAuthID";
                    cboAuthor.DataTextField = "authorName";
                    cboAuthor.DataBind();
                    reader.Close();
                    // Bind video edition Check Box List
                    reader = cbEdComm.ExecuteReader();
                    CheckBoxList cboEdition = (CheckBoxList)videoDetail.FindControl("cbVidEdEdit");
                    cboEdition.DataSource = reader;
                    cboEdition.DataValueField = "videoEdID";
                    cboEdition.DataTextField = "edition";
                    cboEdition.DataBind();
                    reader.Close();
                    // Bind video Category drop down list
                    reader = ddlCatComm.ExecuteReader();
                    DropDownList cboCategory = (DropDownList)videoDetail.FindControl("ddlVidCatEdit");
                    cboCategory.DataSource = reader;
                    cboCategory.DataValueField = "videoCatID";
                    cboCategory.DataTextField = "categoryName";
                    cboCategory.DataBind();
                    reader.Close();
                    // Bind associated video edit mode checkboxes
                    reader = impStepsComm.ExecuteReader();
                    CheckBoxList cboVidDetailsImpStepsCB = (CheckBoxList)videoDetail.FindControl("videoImpStepsCBEdit");
                    cboVidDetailsImpStepsCB.DataSource = reader;
                    cboVidDetailsImpStepsCB.DataTextField = "impStepName";
                    cboVidDetailsImpStepsCB.DataValueField = "impStepID";
                    cboVidDetailsImpStepsCB.DataBind();
                    reader.Close();
                    // Bind author selected value drop down list.
                    reader = comm.ExecuteReader();
                    reader.Read();
                    cboAuthor.SelectedValue = reader["videoAuthID"].ToString();
                    reader.Close();
                    //Bind edition selected values
                    reader = cbEdCKComm.ExecuteReader();
                    while (reader.Read())
                    {
                        ListItem cboListItemEd = cboEdition.Items.FindByValue(reader["videoEdId"].ToString());
                        if (cboListItemEd != null)
                        {
                            cboListItemEd.Selected = true;
                        }
                    }
                    reader.Close();
                    //Bind category selected value drop down list
                    reader = comm.ExecuteReader();
                    reader.Read();
                    cboCategory.SelectedValue = reader["videoCatID"].ToString();
                    reader.Close();
                    //Bind imp steps selected values
                    reader = impStepsCKComm.ExecuteReader();
                    while (reader.Read())
                    {
                        ListItem cboListItem = cboVidDetailsImpStepsCB.Items.FindByValue(reader["impStepID"].ToString());
                        if (cboListItem != null)
                        {
                            cboListItem.Selected = true;
                        }
                    }
                    reader.Close();
                    conn.Close();
                }
                catch (SqlException ex)
                {

                    dbErrorMessage.Style.Clear();
                    dbErrorMessage.Text =
                        "<div class=\"errorMessageSearch\">The following Error has occurred:<br /> [" +

    ex.Message + "]</div>";
                }
                finally
                {
                    conn.Close();
                }
            }
            else
            {
                videoDetail.ChangeMode(DetailsViewMode.ReadOnly);
                Session["videoEdit"] = "";
                BindVideoDetails();
                h3Title.Text = "Video Detail Information";
            }
        }

    }
    // ***************************************************************************************** Handle update event video detail view************
    protected void videoDetail_ItemUpdating(object sender, DetailsViewUpdateEventArgs e)
    {
        Session["videoEdit"] = "";
        int selectRowIndex = videoInfoGrid.SelectedIndex;
        int videoInfoID = (int)videoInfoGrid.DataKeys[selectRowIndex].Value;

        SqlConnection conn;
        SqlCommand edCkBoxDeleteComm;
        SqlCommand edCkBoxAddComm;
        SqlCommand impCkBoxDeleteComm;
        SqlCommand impCkBoxAddComm;
        SqlCommand comm;
        string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
        conn = new SqlConnection(connectionString);
        //Find checkboxes and delete previous values
        CheckBoxList cboVidDetailsEdCB = (CheckBoxList)videoDetail.FindControl("cbVidEdEdit");
        CheckBoxList cboVidDetailsImpStepsCB = (CheckBoxList)videoDetail.FindControl("videoImpStepsCBEdit");
        edCkBoxDeleteComm = new SqlCommand("DELETE FROM videoEditions WHERE videoInfoID=" + videoInfoID + "", conn);
        impCkBoxDeleteComm = new SqlCommand("DELETE FROM impStepVids WHERE videoInfoID=" + videoInfoID + "", conn);
        try
        {
            conn.Open();
            edCkBoxDeleteComm.ExecuteNonQuery();
            impCkBoxDeleteComm.ExecuteNonQuery();
        }
        catch (SqlException exB)
        {
            newEdError.Style.Clear();
            newEdError.Text =
                "<div class=\"errorMessageSearch\">Error There has been an error trying to delete Imp Steps <br /> [" + exB.Message + "]</div>";
        }
        finally
        {
            conn.Close();
        }
        // loop through new selected video editions so you can add new values when updating details.
        foreach (ListItem item in cboVidDetailsEdCB.Items)
        {
            edCkBoxAddComm = new SqlCommand("INSERT INTO videoEditions (videoEdID, videoInfoID) VALUES (" + item.Value + ", " + videoInfoID + ")", conn);
            if (item.Selected)
            {
                try
                {
                    conn.Open();
                    edCkBoxAddComm.ExecuteNonQuery();

                }
                catch (SqlException exA)
                {

                    newEdError.Style.Clear();
                    newEdError.Text =
                        "<div class=\"errorMessageSearch\">Error There has been an error trying to re-add imp steps <br /> [" + exA.Message + "]</div>";
                }
                finally
                {
                    conn.Close();
                    h3Title.Text = "Video Detail Information";
                }

            }
        }

        // loop through new selected implementation steps so you can add new values when updating details.
        foreach (ListItem item in cboVidDetailsImpStepsCB.Items)
        {
            impCkBoxAddComm = new SqlCommand("INSERT INTO impStepVids (impStepID, videoInfoID) VALUES (" + item.Value + ", " + videoInfoID + ")", conn);
            if (item.Selected)
            {
                try
                {
                    conn.Open();
                    impCkBoxAddComm.ExecuteNonQuery();

                }
                catch (SqlException exA)
                {

                    newEdError.Style.Clear();
                    newEdError.Text =
                        "<div class=\"errorMessageSearch\">Error There has been an error trying to re-add imp steps <br /> [" + exA.Message + "]</div>";
                }
                finally
                {
                    conn.Close();
                }

            }
        }

        TextBox newVidName = (TextBox)videoDetail.FindControl("videoNameEdit");
        string NewVidName = newVidName.Text;
        TextBox newVidLenght = (TextBox)videoDetail.FindControl("videoLenghtEdit");
        string NewVidLenght = newVidLenght.Text;
        DropDownList newVidAuthor = (DropDownList)videoDetail.FindControl("ddlVidAuthorEdit");
        int NewVidAuthor = Convert.ToInt32(newVidAuthor.SelectedValue);
        DropDownList newVidCategory = (DropDownList)videoDetail.FindControl("ddlVidCatEdit");
        int NewVidCategory = Convert.ToInt32(newVidCategory.SelectedValue);
        TextBox newVidSCNumber = (TextBox)videoDetail.FindControl("videoSCVNEdit");
        string NewVidSCNumber = newVidSCNumber.Text;
        TextBox newVidSCFN = (TextBox)videoDetail.FindControl("videoSCFNEdit");
        string NewVidSCFN = newVidSCFN.Text;
        TextBox newVidKeyWords = (TextBox)videoDetail.FindControl("videokeyWrdsEdit");
        string NewVidKeyWords = newVidKeyWords.Text;
        TextBox newVidDescription = (TextBox)videoDetail.FindControl("videoDescEdit");
        string NewVidDescription = newVidDescription.Text;
        TextBox newVidWidth = (TextBox)videoDetail.FindControl("videoWidthEdit");
        string NewVidWidth = newVidWidth.Text;
        TextBox newVidHeight = (TextBox)videoDetail.FindControl("videoHeightEdit");
        string NewVidHeight = newVidHeight.Text;


        comm = new SqlCommand("updateVideoDetails", conn);
        comm.CommandType = CommandType.StoredProcedure;
        comm.Parameters.Add("@videoInfoID", SqlDbType.Int);
        comm.Parameters["@videoInfoID"].Value = videoInfoID;
        comm.Parameters.Add("@NewvidName", System.Data.SqlDbType.NVarChar, 200);
        comm.Parameters["@NewvidName"].Value = NewVidName;
        comm.Parameters.Add("@NewvidAuthID", SqlDbType.Int);
        comm.Parameters["@NewvidAuthID"].Value = NewVidAuthor;
        comm.Parameters.Add("@NewvidCatID", SqlDbType.Int);
        comm.Parameters["@NewvidCatID"].Value = NewVidCategory;
        comm.Parameters.Add("@NewvidDescription", System.Data.SqlDbType.NVarChar, 2000);
        comm.Parameters["@NewvidDescription"].Value = NewVidDescription;
        comm.Parameters.Add("@NewvidLenght", System.Data.SqlDbType.NVarChar, 50);
        comm.Parameters["@NewvidLenght"].Value = NewVidLenght;
        comm.Parameters.Add("@NewvidLink", System.Data.SqlDbType.NVarChar, 300);
        comm.Parameters["@NewvidLink"].Value = NewVidSCNumber;
        comm.Parameters.Add("@NewvidFileName", System.Data.SqlDbType.NVarChar, 150);
        comm.Parameters["@NewvidFileName"].Value = NewVidSCFN;
        comm.Parameters.Add("@NewvidKeyWords", System.Data.SqlDbType.NVarChar, 150);
        comm.Parameters["@NewvidKeyWords"].Value = NewVidKeyWords;
        comm.Parameters.Add("@NewvidWidth", System.Data.SqlDbType.NVarChar, 10);
        comm.Parameters["@NewvidWidth"].Value = NewVidWidth;
        comm.Parameters.Add("@NewvidHeight", System.Data.SqlDbType.NVarChar, 10);
        comm.Parameters["@NewvidHeight"].Value = NewVidHeight;

        try
        {
            conn.Open();
            comm.ExecuteNonQuery();
        }
        finally
        {
            conn.Close();
        }
        videoDetail.ChangeMode(DetailsViewMode.ReadOnly);
        BindVideoDetails();
        // Clear viewstate to reload member grid.
        ViewState["VideosDataSet"] = null;
        BindVideoGrid();
    }

    // Handle video delete event
    protected void videoInfoGrid_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        int deleteRowIndex = (int)videoInfoGrid.DataKeys[e.RowIndex].Value;
        string tableName = "videoInfo";
        Session["confirmDeletion"] = deleteRowIndex;
        Session["confirmDelTable"] = tableName;
        Session["commParameter"] = "videoInfoID";
        deleteConfirmDiv.Style.Clear();
        deleteConfirmDiv.Style.Add("clear", "both");
        deleteConfirmation.Text = "<div class=\"errorMessageSearch\">Please confirm you wish to delete this record?</div>";
    }

    // Handle delete confirm button
    protected void deleteTrue(object sender, EventArgs e)
    {
        SqlConnection conn;
        SqlCommand commAssocEd;
        SqlCommand commAssocImp;
        SqlCommand comm;
        string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
        conn = new SqlConnection(connectionString);
        commAssocEd = new SqlCommand("DELETE FROM videoEditions WHERE " + Session["commParameter"] + "=" + Session["confirmDeletion"] + "", conn);
        commAssocImp = new SqlCommand("DELETE FROM impStepVids WHERE " + Session["commParameter"] + "=" + Session["confirmDeletion"] + "", conn);
        comm = new SqlCommand("DELETE FROM " + Session["confirmDelTable"] + " WHERE " + Session["commParameter"] + "=" + Session["confirmDeletion"] + "", conn);
        try
        {
            conn.Open();
            commAssocEd.ExecuteNonQuery();
            commAssocImp.ExecuteNonQuery();
            comm.ExecuteNonQuery();

        }
        finally
        {
            conn.Close();
            Response.Redirect("videosList.aspx");
        }
    }
    // Handle delete cancel button
    protected void cancelDelete(object sender, EventArgs e)
    {
        deleteConfirmDiv.Style.Add("display", "none");
        Response.Redirect("videosList.aspx");
    }
}
