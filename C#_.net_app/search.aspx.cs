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

public partial class search : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //Set sign button as the default so it fires when users hits enter on keyboard.
        Page.Form.DefaultButton = videoSearchBtn.UniqueID;
        // Check if user is logged in else redirect them to login page
        if (!globalFunctions.validateLogin())
        {
            Response.Redirect("Logout.aspx");
        }
    }

    // Handle Search btn
    protected void videoSearch(object sender, EventArgs e)
    {
        searchRepeater.Visible = false;

        if (IsPostBack && videoSearchText.Text != "")
        {
            string tableField = videoSearchBy.SelectedValue.ToString();
            searchError.Visible = false;
            SqlConnection conn;
            SqlCommand comm;
            SqlDataReader reader;
            string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
            conn = new SqlConnection(connectionString);
            comm = new SqlCommand("SELECT videoInfo.videoInfoID AS videoInfoID, MIN(videoInfo.videoName) AS videoName, MIN(videoInfo.videoAuthID) AS videoAuthID, " +
                                  "MIN(videoInfo.[description]) AS description,  MIN(videoInfo.lenght) AS lenght, MIN(videoInfo.videoCatID) AS videoCatID, MIN(videoInfo.keyWords) AS keyWords, " +
                                  "MIN(videoCategory.categoryName) AS categoryName, MIN(videoCategory.videoCatID) AS videoCatID, MIN(videoEditions.videoEdID) AS videoEdID, " +
                                  "MIN(videoEditions.videoInfoID) AS videoInfoID, MIN(videoEdition.edition) AS edition, MIN(videoEdition.editionLogo) AS editionLogo  FROM videoEditions " +
                                   "JOIN videoInfo ON videoEditions.videoInfoID = videoInfo.videoInfoID " +
                                   "JOIN videoEdition ON videoEditions.videoEdID = videoEdition.videoEdID " +
                                   "JOIN videoCategory ON videoInfo.videoCatID = videoCategory.videoCatID " +
                                   "WHERE " + tableField + " LIKE '%' + @searchBox + '%' GROUP BY videoInfo.videoInfoID", conn);
            //comm.CommandType = System.Data.CommandType.StoredProcedure;
            comm.Parameters.Add("@searchBox", SqlDbType.NVarChar, 100);
            comm.Parameters["@searchBox"].Value = videoSearchText.Text;

            try
            {
                conn.Open();
                reader = comm.ExecuteReader();
                if (reader.HasRows)
                {
                    searchRepeater.Visible = true;
                    searchRepeater.DataSource = reader;
                    searchRepeater.DataBind();
                    reader.Close();

                }
                else
                {
                    searchError.Visible = true;
                    searchError.Text = "<div class='searchNotice'>Your search did not return any videos. Please try again. TIP: " +
                        "You may want to try using single words search such as \"Billing\" or \"Setup\".</div>";
                    reader.Close();
                }
            }
            catch (SqlException ex)
            {
                searchRepeater.Visible = false;
                searchError.Visible = true;
                searchError.Text =
                    "<div class=\"errorMessageSearch\">There has been an error:<br /> [" + ex.Message + " ]</div>";
            }

            finally
            {
                conn.Close();
            }

        }
        else
        {
            // If search field is blank bring up videos in ascending order by name videoInfo.videoName
            if (IsPostBack && videoSearchText.Text == "")
            {
                string tableField = "videoName";
                searchError.Visible = false;
                SqlConnection conn;
                SqlCommand comm;
                SqlDataReader reader;
                string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
                conn = new SqlConnection(connectionString);
                comm = new SqlCommand("SELECT videoInfoID, videoName, [description], lenght, link FROM videoInfo ORDER BY videoName ASC", conn);
               
                try
                {
                    conn.Open();
                    reader = comm.ExecuteReader();
                    if (reader.HasRows)
                    {
                        searchRepeater.Visible = true;
                        searchError.Visible = true;
                        searchRepeater.DataSource = reader;
                        searchRepeater.DataBind();
                        reader.Close();
                        searchError.Text = "<div class='searchNotice' style='margin-bottom:15px;'>You did not enter anything is the search box above. Here is a list of <b>all videos</b> by title. <b>TIP</b> You can search by different criteria using the dropdown menu.</div>";

                    }
                    else
                    {
                        searchError.Visible = true;
                        searchError.Text = "<div class='searchNotice'>Your search did not return any videos. Please try again. TIP: " +
                            "You may want to try using single words search such as \"Billing\" or \"Setup\".</div>";
                        reader.Close();
                    }
                }
                catch (SqlException ex)
                {
                    searchRepeater.Visible = false;
                    searchError.Visible = true;
                    searchError.Text =
                        "<div class=\"errorMessageSearch\">There has been an error:<br /> [" + ex.Message + " ]</div>";
                }

                finally
                {
                    conn.Close();
                } 
            }
        }
    }

    // Create event handler to respond to image button click of video from category view or search view.
    override protected void OnInit(EventArgs e)
    {
        base.OnInit(e);
        searchRepeater.ItemCommand += new RepeaterCommandEventHandler(searchRepeater_ItemCommand);
    }

    private void searchRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName == "videoRedirect")
        {
            Session["videoDetailID"] = e.CommandArgument;
            Response.Redirect("memberVideos.aspx");
        }
    }

}
