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

public partial class admin_videosAuth : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

        if (!IsPostBack)
        {
            // Bind video author grid
            BindvideoAuthorGrid();
            h3Title.Text = "Video Author List";

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
    }
    // Handle back to main video page
    protected void backToMain(object sender, EventArgs e)
    {
        Response.Redirect("videos.aspx");
    }
    // Bind video author grid
    private void BindvideoAuthorGrid()
    {
        SqlConnection conn;
        SqlCommand comm;
        SqlDataReader reader;
        string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
        conn = new SqlConnection(connectionString);
        comm = new SqlCommand(globalFunctions.GenerateSQL("videoAuthID, authorName, authImage, authTitle", "videoAuthor"), conn);
        try
        {
            conn.Open();
            reader = comm.ExecuteReader();
            authorGrid.DataSource = reader;
            authorGrid.DataBind();
            reader.Close();
        }
        finally
        {
            conn.Close();
        }

    }
    // Handle Edit Video Authors
    protected void authorGrid_RowEditing(object sender, GridViewEditEventArgs e)
    {
        authorGrid.EditIndex = e.NewEditIndex;
        //Bind data to the GridView control.
        BindvideoAuthorGrid();
        int editRowIndex = authorGrid.EditIndex;
        Label authorImageStore = (Label)authorGrid.Rows[editRowIndex].Cells[1].FindControl("authorImageSession");
        // Add current image name to session to handle no change in image when editing in the update event.
        string sessionVariable = authorImageStore.Text;
        Session["currentAuthImage"] = sessionVariable;
        h3Title.Text = "Edit Video Author";
    }
    // Handle video authors cancel event
    protected void authorGrid_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        //Reset the edit index.
        authorGrid.EditIndex = -1;
        //Bind data to the GridView control.
        BindvideoAuthorGrid();
        Session.Remove("currentAuthImage");
        h3Title.Text = "Video Author List";
    }

    // Handle video Author Change image button
    protected void changeAuthEidtImage(object sender, EventArgs e)
    {
        int editRowIndex = authorGrid.EditIndex;
        HtmlGenericControl hideDiv = (HtmlGenericControl)authorGrid.Rows[editRowIndex].Cells[1].FindControl("divShowUpAuth");
        HtmlGenericControl showDiv = (HtmlGenericControl)authorGrid.Rows[editRowIndex].Cells[1].FindControl("divShowStaticAuth");
        Session.Remove("currentAuthImage");
        showDiv.Style.Add("display", "none");
        hideDiv.Style.Clear();

    }

    //Handle video author Update Event
    protected void authorGrid_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        if (authorGrid.EditIndex != -1 && Session["currentAuthImage"] == null)
        {
            // Get videoAuthId through the datakey and edit row index
            int editRowIndex = authorGrid.EditIndex;
            int videoAuthID = (int)authorGrid.DataKeys[editRowIndex].Values["videoAuthID"];
            // Find authorNameEdit control using the edit row index and cells array
            TextBox newAuthName = (TextBox)authorGrid.Rows[editRowIndex].Cells[1].FindControl("authorNameEdit");
            string authorNameEditText = newAuthName.Text;
            TextBox newAuthTitle = (TextBox)authorGrid.Rows[editRowIndex].Cells[3].FindControl("authorTitleEdit");
            string authorTitleEditText = newAuthTitle.Text;
            // Find authorImageEdit control using the edit row index and cells array
            GridViewRow row = authorGrid.Rows[e.RowIndex];
            FileUpload fileUpload = row.Cells[1].FindControl("fileUploadAuthorEdit") as FileUpload;
            string fileAuthorName = fileUpload.FileName;
            if (fileUpload != null && fileUpload.HasFile)
            {
                fileUpload.SaveAs(Server.MapPath("/Images/uploads/" + fileUpload.FileName));
                // Start SQL commands
                SqlConnection conn;
                SqlCommand comm;
                string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
                conn = new SqlConnection(connectionString);
                comm = new SqlCommand("updateVideoAuthor", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                // Add Parameters
                comm.Parameters.Add("@videoAuthID", System.Data.SqlDbType.Int);
                comm.Parameters["@videoAuthID"].Value = videoAuthID;
                comm.Parameters.Add("@NewauthorName", System.Data.SqlDbType.NVarChar, 50);
                comm.Parameters["@NewauthorName"].Value = authorNameEditText;
                comm.Parameters.Add("@NewauthImage", System.Data.SqlDbType.NVarChar, 200);
                comm.Parameters["@NewauthImage"].Value = fileAuthorName;
                comm.Parameters.Add("@NewauthTitle", System.Data.SqlDbType.NVarChar, 100);
                comm.Parameters["@NewauthTitle"].Value = authorTitleEditText;

                try
                {
                    conn.Open();
                    comm.ExecuteNonQuery();
                }
                finally
                {
                    conn.Close();
                    authorGrid.EditIndex = -1;
                    BindvideoAuthorGrid();
                    h3Title.Text = "Video Author List";
                }
            }
            else
            {
                HtmlGenericControl hideDiv = (HtmlGenericControl)authorGrid.Rows[editRowIndex].Cells[1].FindControl("divShowUpAuth");
                Label error = (Label)authorGrid.Rows[editRowIndex].Cells[1].FindControl("editUploadError");
                hideDiv.Style.Clear();
                error.Text = "<div class=\"errorMessageSearch\">You must upload an image to proceed.</div>";
            }
        }
        else
        {
            // Handle update with no image change
            if (Session["currentAuthImage"] != null)
            {
                // Get videoAuthId through the datakey and edit row index
                int editRowIndex = authorGrid.EditIndex;
                int videoAuthID = (int)authorGrid.DataKeys[editRowIndex].Values["videoAuthID"];
                // Find authorNameEdit control using the edit row index and cells array
                TextBox newAuthName = (TextBox)authorGrid.Rows[editRowIndex].Cells[1].FindControl("authorNameEdit");
                string authorNameEditText = newAuthName.Text;
                TextBox newAuthTitle = (TextBox)authorGrid.Rows[editRowIndex].Cells[3].FindControl("authorTitleEdit");
                string authorTitleEditText = newAuthTitle.Text;
                // Start SQL commands
                SqlConnection conn;
                SqlCommand comm;
                string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
                conn = new SqlConnection(connectionString);
                comm = new SqlCommand("updateVideoAuthor", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                // Add Parameters
                comm.Parameters.Add("@videoAuthID", System.Data.SqlDbType.Int);
                comm.Parameters["@videoAuthID"].Value = videoAuthID;
                comm.Parameters.Add("@NewauthorName", System.Data.SqlDbType.NVarChar, 50);
                comm.Parameters["@NewauthorName"].Value = authorNameEditText;
                comm.Parameters.Add("@NewauthImage", System.Data.SqlDbType.NVarChar, 200);
                comm.Parameters["@NewauthImage"].Value = Session["currentAuthImage"];
                comm.Parameters.Add("@NewauthTitle", System.Data.SqlDbType.NVarChar, 100);
                comm.Parameters["@NewauthTitle"].Value = authorTitleEditText;
                try
                {
                    conn.Open();
                    comm.ExecuteNonQuery();
                }
                finally
                {
                    conn.Close();
                    authorGrid.EditIndex = -1;
                    BindvideoAuthorGrid();
                    Session.Remove("currentAuthImage");
                    h3Title.Text = "Video Author List";
                }
            }
        }
    }

    // Handle Author grid delete button
    protected void author_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        int deleteRowIndex = (int)authorGrid.DataKeys[e.RowIndex].Value;
        string tableName = "videoAuthor";
        Session["confirmDeletion"] = deleteRowIndex;
        Session["confirmDelTable"] = tableName;
        Session["commParameter"] = "videoAuthID";
        deleteConfirmDiv.Style.Clear();
        deleteConfirmDiv.Style.Add("clear", "both");
        deleteConfirmation.Text = "<div class=\"errorMessageSearch\">Please confirm you wish to delete this record?</div>";
    }

    // Handle delete confirm button
    protected void deleteTrue(object sender, EventArgs e)
    {
        SqlConnection conn;
        SqlCommand comm;
        string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
        conn = new SqlConnection(connectionString);
        comm = new SqlCommand("DELETE FROM " + Session["confirmDelTable"] + " WHERE " + Session["commParameter"] + "=" + Session["confirmDeletion"] + "", conn);
        try
        {
            conn.Open();
            comm.ExecuteNonQuery();
        }
        finally
        {
            conn.Close();
            deleteConfirmDiv.Style.Clear();
            deleteConfirmDiv.Style.Add("display", "none");
            BindvideoAuthorGrid();
        }
    }
    // Handle delete cancel button
    protected void cancelDelete(object sender, EventArgs e)
    {
        deleteConfirmDiv.Style.Add("display", "none");
        Response.Redirect("videosAuth.aspx");
    }
}
