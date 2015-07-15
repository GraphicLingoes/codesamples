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

public partial class admin_videosEd : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

        if (!IsPostBack)
        {
            // Bind video edition grid
            BindvideoEditionGrid();
            h3Title.Text = "Video Edition List";

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

    // Handle back to videos main page
    protected void backToMain(object sender, EventArgs e)
    {
        Response.Redirect("videos.aspx");
    }

    // Bind video edition grid
    private void BindvideoEditionGrid()
    {
        SqlConnection conn;
        SqlCommand comm;
        SqlDataReader reader;
        string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
        conn = new SqlConnection(connectionString);
        comm = new SqlCommand(globalFunctions.GenerateSQL("videoEdID, edition, editionLogo", "videoEdition"), conn);
        try
        {
            conn.Open();
            reader = comm.ExecuteReader();
            videoEdGrid.DataSource = reader;
            videoEdGrid.DataBind();
            reader.Close();
        }
        finally
        {
            conn.Close();
        }
    }
    // Handle Edit Video Editions
    protected void editionGrid_RowEditing(object sender, GridViewEditEventArgs e)
    {
        videoEdGrid.EditIndex = e.NewEditIndex;
        //Bind data to the GridView control.
        BindvideoEditionGrid();
        int editRowIndex = videoEdGrid.EditIndex;
        Label edImageStore = (Label)videoEdGrid.Rows[editRowIndex].Cells[1].FindControl("edImageSession");
        // Add current image name to session to handle no change in image when editing in the update event.
        string sessionVariableEd = edImageStore.Text;
        Session["currentEdImage"] = sessionVariableEd;
        h3Title.Text = "Edit Edition Information";
    }
    // Handle video edition cancel event
    protected void editionGrid_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        //Reset the edit index.
        videoEdGrid.EditIndex = -1;
        //Bind data to the GridView control.
        BindvideoEditionGrid();
        Session.Remove("currentEdImage");
        h3Title.Text = "Video Edition List";

    }

    // Handle video Edition Change image button
    protected void changeEdEidtImage(object sender, EventArgs e)
    {
        int editRowIndex = videoEdGrid.EditIndex;
        HtmlGenericControl hideDiv = (HtmlGenericControl)videoEdGrid.Rows[editRowIndex].Cells[1].FindControl("divShowUpEd");
        HtmlGenericControl showDiv = (HtmlGenericControl)videoEdGrid.Rows[editRowIndex].Cells[1].FindControl("divShowStaticEd");
        Session.Remove("currentEdImage");
        showDiv.Style.Add("display", "none");
        hideDiv.Style.Clear();
    }
    //Handle video Edition Update Event
    protected void editionGrid_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        if (videoEdGrid.EditIndex != -1 && Session["currentEdImage"] == null)
        {
            // Get videoEdId through the datakey and edit row index
            int editRowIndex = videoEdGrid.EditIndex;
            int videoEdID = (int)videoEdGrid.DataKeys[editRowIndex].Values["videoEdID"];
            // Find editionNameEdit control using the edit row index and cells array
            TextBox newEdName = (TextBox)videoEdGrid.Rows[editRowIndex].Cells[1].FindControl("editionNameEdit");
            string edNameEditText = newEdName.Text;
            // Find editionImageEdit control using the edit row index and cells array
            GridViewRow row = videoEdGrid.Rows[e.RowIndex];
            FileUpload fileUpload = row.Cells[1].FindControl("fileUploadEdEdit") as FileUpload;
            string fileEdLogo = fileUpload.FileName;
            if (fileUpload != null && fileUpload.HasFile)
            {
                fileUpload.SaveAs(Server.MapPath("/Images/uploads/" + fileUpload.FileName));
                // Start SQL commands
                SqlConnection conn;
                SqlCommand comm;
                string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
                conn = new SqlConnection(connectionString);
                comm = new SqlCommand("updateVideoEdition", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                // Add Parameters
                comm.Parameters.Add("@videoEdID", System.Data.SqlDbType.Int);
                comm.Parameters["@videoEdID"].Value = videoEdID;
                comm.Parameters.Add("@Newedition", System.Data.SqlDbType.NVarChar, 50);
                comm.Parameters["@Newedition"].Value = edNameEditText;
                comm.Parameters.Add("@NeweditionLogo", System.Data.SqlDbType.NVarChar, 200);
                comm.Parameters["@NeweditionLogo"].Value = fileEdLogo;
                try
                {
                    conn.Open();
                    comm.ExecuteNonQuery();
                }
                finally
                {
                    conn.Close();
                    videoEdGrid.EditIndex = -1;
                    BindvideoEditionGrid();
                    h3Title.Text = "Video Edition List";
                }
            }
            else
            {
                HtmlGenericControl hideDiv = (HtmlGenericControl)videoEdGrid.Rows[editRowIndex].Cells[1].FindControl("divShowUpEd");
                Label error = (Label)videoEdGrid.Rows[editRowIndex].Cells[1].FindControl("editEdUploadError");
                hideDiv.Style.Clear();
                error.Text = "<div class=\"errorMessageSearch\">You must upload an image to proceed.</div>";
            }
        }
        else
        {
            // Handle update with no image change
            if (Session["currentEdImage"] != null)
            {
                // Get videoEdId through the datakey and edit row index
                int editRowIndex = videoEdGrid.EditIndex;
                int videoEdID = (int)videoEdGrid.DataKeys[editRowIndex].Values["videoEdID"];
                // Find editionNameEdit control using the edit row index and cells array
                TextBox newEdName = (TextBox)videoEdGrid.Rows[editRowIndex].Cells[1].FindControl("editionNameEdit");
                string edNameEditText = newEdName.Text;
                // Start SQL commands
                SqlConnection conn;
                SqlCommand comm;
                string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
                conn = new SqlConnection(connectionString);
                comm = new SqlCommand("updateVideoEdition", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                // Add Parameters
                comm.Parameters.Add("@videoEdID", System.Data.SqlDbType.Int);
                comm.Parameters["@videoEdID"].Value = videoEdID;
                comm.Parameters.Add("@Newedition", System.Data.SqlDbType.NVarChar, 50);
                comm.Parameters["@Newedition"].Value = edNameEditText;
                comm.Parameters.Add("@NeweditionLogo", System.Data.SqlDbType.NVarChar, 200);
                comm.Parameters["@NeweditionLogo"].Value = Convert.ToString(Session["currentEdImage"]);
                try
                {
                    conn.Open();
                    comm.ExecuteNonQuery();
                }
                finally
                {
                    conn.Close();
                    videoEdGrid.EditIndex = -1;
                    BindvideoEditionGrid();
                    Session.Remove("currentEdImage");
                    h3Title.Text = "Video Edition List";
                }
            }
        }
    }
    // Handle Edition grid delete button
    protected void edition_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        int deleteRowIndex = (int)videoEdGrid.DataKeys[e.RowIndex].Value;
        string tableName = "videoEdition";
        Session["confirmDeletion"] = deleteRowIndex;
        Session["confirmDelTable"] = tableName;
        Session["commParameter"] = "videoEdID";
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
            BindvideoEditionGrid();
        }
    }
    // Handle delete cancel button
    protected void cancelDelete(object sender, EventArgs e)
    {
        deleteConfirmDiv.Style.Add("display", "none");
        Response.Redirect("videosEd.aspx");
    }
}
