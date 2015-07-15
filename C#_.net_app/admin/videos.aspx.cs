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


public partial class admin_videos : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

        if (!IsPostBack)
        {
            BindAssocImpSteps();
            bindCatAuthEd();
            vidWidth.Text = "640";
            vidHeight.Text = "498";

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

        // bind Video Author, Category and Edition drop down lists
    }
    protected void bindCatAuthEd()
    {
        SqlConnection videoConn;
        SqlCommand videoAuthComm;
        SqlCommand videoCatComm;
        SqlCommand videoEdComm;
        SqlDataReader Videoreader;
        string vidoeConnectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
        videoConn = new SqlConnection(vidoeConnectionString);
        videoAuthComm = new SqlCommand("SELECT videoAuthID, authorName FROM videoAuthor", videoConn);
        videoCatComm = new SqlCommand("SELECT videoCatID, categoryName FROM videoCategory", videoConn);
        videoEdComm = new SqlCommand("SELECT videoEdID, edition FROM videoEdition", videoConn);


        try
        {
            // Bind video Author Drop Down List.
            videoConn.Open();
            Videoreader = videoAuthComm.ExecuteReader();
            ddlVidAuthor.DataSource = Videoreader;
            ddlVidAuthor.DataTextField = "authorName";
            ddlVidAuthor.DataValueField = "videoAuthID";
            ddlVidAuthor.DataBind();
            Videoreader.Close();
            // Bind video Category Drop Down List.
            Videoreader = videoCatComm.ExecuteReader();
            ddlVidCat.DataSource = Videoreader;
            ddlVidCat.DataTextField = "categoryName";
            ddlVidCat.DataValueField = "videoCatID";
            ddlVidCat.DataBind();
            Videoreader.Close();
            // Bind video Edition Drop Down List.
            Videoreader = videoEdComm.ExecuteReader();
            cbVidEd.DataSource = Videoreader;
            cbVidEd.DataTextField = "edition";
            cbVidEd.DataValueField = "videoEdID";
            cbVidEd.DataBind();
            Videoreader.Close();
        }
        finally
        {
            videoConn.Close();
        }
    }
    
    // Bind associated implementation steps
    private void BindAssocImpSteps()
    {
        SqlConnection conn;
        SqlCommand impStepsComm;
        SqlDataReader reader;
        string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
        conn = new SqlConnection(connectionString);
        impStepsComm = new SqlCommand("SELECT impStepID, impStepName, Priority FROM impSteps ORDER BY Priority ASC", conn);
        conn.Open();
        reader = impStepsComm.ExecuteReader();
        newVidImpSteps.DataSource = reader;
        newVidImpSteps.DataTextField = "impStepName";
        newVidImpSteps.DataValueField = "impStepID";
        newVidImpSteps.DataBind();
        reader.Close();
        conn.Close();
    }

    //##################################################################################### Start add new video buttons
    protected void selectNewVideo(object s, EventArgs e)
    {
        string newCheck = ddlNewVideo.SelectedValue;
        // New Video Code
        if (newCheck == "newVideo")
        {

            divNewVideo.Style.Clear();
            // Rebind assoicated implementation steps to see if there are any new ones
            BindAssocImpSteps();
            // Rebind auth cat
            bindCatAuthEd();
        }
        else
        {
            divNewVideo.Style.Add("display", "none");
        }
        // New Category Code
        if (newCheck == "newCategory")
        {
            divNewCategory.Style.Clear();
            newCatSuccess.Style.Add("display", "none");
            newCatError.Style.Add("display", "none");
        }
        else
        {
            divNewCategory.Style.Add("display", "none");
        }
        // New Edition Code
        if (newCheck == "newEdition")
        {

            divNewEdition.Style.Clear();
        }
        else
        {
            divNewEdition.Style.Add("display", "none");
        }
        // New Author Code
        if (newCheck == "newAuthor")
        {

            divNewAuthor.Style.Clear();
        }
        else
        {
            divNewAuthor.Style.Add("display", "none");
        }
    }

    // Handle manage drop down menu
    protected void selectManageVideo(object sender, EventArgs e)
    {
        string manageCheck = ddlManageVideo.SelectedValue;

        if (manageCheck == "manageVideo")
        {
            Response.Redirect("videosList.aspx");
        }

        if (manageCheck == "manageCategory")
        {
            Response.Redirect("videosCat.aspx");
        }
        
        if (manageCheck == "manageEdition")
        {
            Response.Redirect("videosEd.aspx");
        }

        if (manageCheck == "manageAuthor")
        {
            Response.Redirect("videosAuth.aspx");
        }
    }
    // Handle Add New Video Button NOTE Drop down lists are load in page load event
    protected void newVideo(object sender, EventArgs e)
    {
        if (IsPostBack)
        {

            SqlConnection conn;
            SqlCommand comm;
            SqlCommand cbEdComm;
            SqlCommand impComm;
            SqlDataReader reader;
            string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
            conn = new SqlConnection(connectionString);
            comm = new SqlCommand("newVideoInfo", conn);
            comm.CommandType = System.Data.CommandType.StoredProcedure;
            // Add Parameters
            comm.Parameters.Add("@Newname", System.Data.SqlDbType.NVarChar, 200);
            comm.Parameters["@Newname"].Value = vidName.Text;
            comm.Parameters.Add("@NewvideoAuthID", System.Data.SqlDbType.Int);
            comm.Parameters["@NewvideoAuthID"].Value = ddlVidAuthor.SelectedValue;
            comm.Parameters.Add("@Newdescription", System.Data.SqlDbType.NVarChar, 2000);
            comm.Parameters["@Newdescription"].Value = videoDescription.Text;
            comm.Parameters.Add("@Newlenght", System.Data.SqlDbType.NVarChar, 50);
            comm.Parameters["@Newlenght"].Value = vidLength.Text;
            comm.Parameters.Add("@NewvideoCatID", System.Data.SqlDbType.Int);
            comm.Parameters["@NewvideoCatID"].Value = ddlVidCat.SelectedValue;
            comm.Parameters.Add("@Newlink", System.Data.SqlDbType.NVarChar, 300);
            comm.Parameters["@Newlink"].Value = vidLink.Text;
            comm.Parameters.Add("@NewfileName", System.Data.SqlDbType.NVarChar, 150);
            comm.Parameters["@NewfileName"].Value = vidFileName.Text;
            comm.Parameters.Add("@NewkeyWords", System.Data.SqlDbType.NVarChar, 150);
            comm.Parameters["@NewkeyWords"].Value = vidKeywords.Text;
            comm.Parameters.Add("@NewvidWidth", System.Data.SqlDbType.NVarChar, 10);
            comm.Parameters["@NewvidWidth"].Value = vidWidth.Text;
            comm.Parameters.Add("@NewvidHeight", System.Data.SqlDbType.NVarChar, 10);
            comm.Parameters["@NewvidHeight"].Value = vidHeight.Text;

            // Add combobox parameters
            try
            {
                conn.Open();
                reader = comm.ExecuteReader();
                reader.Read();
                Session["vidValue"] = reader["Value"];
                reader.Close();
            }
            catch (SqlException ex)
            {

                newEdError.Style.Clear();
                newEdError.Text =
                    "<div class=\"errorMessage\">Error There has been an error trying to create your new video <br /> [" + ex.Message + "]</div>";
            }
            finally
            {
                conn.Close();

            }
            // loop through new selected video editions so you can add new values when updating details.
            foreach (ListItem item in cbVidEd.Items)
            {
                cbEdComm = new SqlCommand("INSERT INTO videoEditions (videoEdID, videoInfoID) VALUES (" + item.Value + ", " + Session["vidValue"] + ")", conn);
                if (item.Selected)
                {
                    try
                    {
                        conn.Open();
                        cbEdComm.ExecuteNonQuery();

                    }
                    catch (SqlException exA)
                    {

                        newEdError.Style.Clear();
                        newEdError.Text =
                            "<div class=\"errorMessage\">Error There has been an error trying to re-add imp steps <br /> [" + exA.Message + "]</div>";
                    }
                    finally
                    {
                        conn.Close();
                    }

                }
            }

            // Handle implementation steps to be added to each video
            foreach (ListItem item in newVidImpSteps.Items)
            {
                impComm = new SqlCommand("INSERT INTO impStepVids (impStepID, videoInfoID) VALUES (" + item.Value + ", " + Session["vidValue"] + ")", conn);
                if (item.Selected)
                {
                    try
                    {
                        conn.Open();
                        impComm.ExecuteNonQuery();

                    }
                    catch (SqlException ex)
                    {

                        newEdError.Style.Clear();
                        newEdError.Text =
                            "<div class=\"errorMessage\">Error There has been an error trying to create your new video <br /> [" + ex.Message + "]</div>";
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            newEdSuccess.Style.Clear();
            divNewVideo.Style.Clear();
            newEdSuccess.Text = "<div class=\"success\">" + vidName.Text + " has been added to the videos collection.</div>";
            // Clear viewstate to reload member grid.
            ViewState["VideosDataSet"] = null;
        }
    }
    protected void newCategory(object s, EventArgs e)
    {
        string CatName = newCatName.Text;
        string scFolder = newScFolder.Text;
        SqlConnection conn;
        SqlCommand comm;
        string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
        conn = new SqlConnection(connectionString);
        comm = new SqlCommand("newVideoCategory", conn);
        comm.CommandType = System.Data.CommandType.StoredProcedure;
        // Add Parameters
        comm.Parameters.Add("@NewcategoryName", System.Data.SqlDbType.NVarChar, 50);
        comm.Parameters["@NewcategoryName"].Value = CatName;
        comm.Parameters.Add("@NewscFolder", System.Data.SqlDbType.NVarChar, 50);
        comm.Parameters["@NewscFolder"].Value = scFolder;
        try
        {
            conn.Open();
            comm.ExecuteNonQuery();
        }
        catch (SqlException ex)
        {

            newCatError.Style.Clear();
            newCatError.Text =
                "<div class=\"errorMessage\">Error There has been an error trying to create your new video category<br /> [" + ex.Message + "]</div>";
        }
        finally
        {
            conn.Close();
            newCatSuccess.Style.Clear();
            divNewCategory.Style.Clear();
            newCatSuccess.Text = "<div class=\"success\">" + CatName + " has been added to video categories.</div>";
            //BindCategoryGrid();
        }
    }

    // Handle cancel button new video
    protected void cancelNewVideo(object sender, EventArgs e)
    {
        Response.Redirect("videos.aspx");
    }

    // Handle cancel button new video category
    protected void cancelNewCat(object sender, EventArgs e)
    {
        Response.Redirect("videos.aspx");
    }

    // Handle cancel button upload video edition
    protected void cancelEdUpload(object sender, EventArgs e)
    {
        Response.Redirect("videos.aspx");
    }

    // Handle cancel button new author
    // Handle cancel button upload video Author
    protected void cancelNewAuth(object sender, EventArgs e)
    {
        Response.Redirect("videos.aspx");
    }
    // Handle new add new video edition button
    protected void newEdition(object s, EventArgs e)
    {
        if (fileUploadEd.HasFile)
        {
            // File upload for video edition
            string fileName = fileUploadEd.FileName;
            fileUploadEd.SaveAs(Server.MapPath("/Images/uploads/") + (fileName));
            divNewEdition.Style.Clear();
            // SQL and parameters for video edition
            string EdName = newEdName.Text;
            SqlConnection conn;
            SqlCommand comm;
            string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
            conn = new SqlConnection(connectionString);
            comm = new SqlCommand("newVideoEdition", conn);
            comm.CommandType = System.Data.CommandType.StoredProcedure;
            // Add Parameters
            comm.Parameters.Add("@Newedition", System.Data.SqlDbType.NVarChar, 50);
            comm.Parameters["@Newedition"].Value = EdName;
            comm.Parameters.Add("@NeweditionLogo", System.Data.SqlDbType.NVarChar, 200);
            comm.Parameters["@NeweditionLogo"].Value = fileName;
            try
            {
                conn.Open();
                comm.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {

                divNewEdition.Style.Clear();
                editionUploadLbl.Text =
                    "<div class=\"errorMessage\">Error There has been an error trying to create your new video edition<br /> [" + ex.Message + "]</div>";
            }
            finally
            {
                conn.Close();
                divNewEdition.Style.Clear();
                editionUploadLbl.Text = "<div class=\"success\">" + EdName + " has been added to video editions and " + fileName + " has been uploaded.</div>";
                //BindvideoEditionGrid();
            }

        }
        else
        {
            divNewEdition.Style.Clear();
            editionUploadLbl.Text = "<div class=\"errorMessage\">You must choose a file to upload for new edition logo!</div>";
        }
    }
    // Handle new add new video Author button
    protected void newAuthor(object s, EventArgs e)
    {
        if (fileUploadAuthor.HasFile)
        {
            // File upload for video edition
            string fileNameAuthor = fileUploadAuthor.FileName;
            fileUploadAuthor.SaveAs(Server.MapPath("/Images/uploads/") + (fileNameAuthor));
            divNewAuthor.Style.Clear();
            // SQL and parameters for video edition
            string authorName = newAuthorName.Text;
            string authorTitle = newAuthorTitle.Text;
            SqlConnection conn;
            SqlCommand comm;
            string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
            conn = new SqlConnection(connectionString);
            comm = new SqlCommand("newVideoAuthor", conn);
            comm.CommandType = System.Data.CommandType.StoredProcedure;
            // Add Parameters
            comm.Parameters.Add("@NewauthorName", System.Data.SqlDbType.NVarChar, 50);
            comm.Parameters["@NewauthorName"].Value = authorName;
            comm.Parameters.Add("@NewauthorImage", System.Data.SqlDbType.NVarChar, 200);
            comm.Parameters["@NewauthorImage"].Value = fileNameAuthor;
            comm.Parameters.Add("@NewauthTitle", System.Data.SqlDbType.NVarChar, 50);
            comm.Parameters["@NewauthTitle"].Value = authorTitle;
            try
            {
                conn.Open();
                comm.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {

                divNewAuthor.Style.Clear();
                newAuthorLbl.Text =
                    "<div class=\"errorMessage\">Error There has been an error trying to create your new video author<br /> [" + ex.Message + "]</div>";
            }
            finally
            {
                conn.Close();
                divNewAuthor.Style.Clear();
                newAuthorLbl.Text = "<div class=\"success\">" + authorName + " has been added to video authors and " + fileNameAuthor + " has been uploaded.</div>";
                //BindvideoAuthorGrid();
            }
        }
        else
        {
            divNewAuthor.Style.Clear();
            newAuthorLbl.Text = "<div class=\"errorMessage\">You must choose a file to upload for new author image!</div>";
        }
    }
}
