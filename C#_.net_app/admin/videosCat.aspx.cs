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

public partial class admin_videosCat : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

        if(!IsPostBack)
        {
            // Bind video category grid
            BindCategoryGrid();
            h3Title.Text = "Video Category List";

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

    // Bind category grid
    private void BindCategoryGrid()
    {
        SqlConnection conn;
        SqlCommand comm;
        SqlDataReader reader;
        string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
        conn = new SqlConnection(connectionString);
        comm = new SqlCommand(globalFunctions.GenerateSQL("videoCatID, categoryName, scFolder", "videoCategory"), conn);
        try
        {
            conn.Open();
            reader = comm.ExecuteReader();
            categoryGrid.DataSource = reader;
            categoryGrid.DataBind();
            reader.Close();
        }
        finally
        {
            conn.Close();
        }

    }

    // Handle Edit Video Categories
    protected void categoryGrid_RowEditing(object sender, GridViewEditEventArgs e)
    {
        categoryGrid.EditIndex = e.NewEditIndex;
        //Bind data to the GridView control.
        BindCategoryGrid();
        h3Title.Text = "Edit Video Category";
    }
    // Handle video category cancel event
    protected void categoryGrid_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        //Reset the edit index.
        categoryGrid.EditIndex = -1;
        //Bind data to the GridView control.
        BindCategoryGrid();
        h3Title.Text = "Video Category List";
    }
    //Handle video Category Update Event
    protected void categoryGrid_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        if (categoryGrid.EditIndex != -1)
        {
            // Get videoCatId through the datakey and edit row index
            int editRowIndex = categoryGrid.EditIndex;
            int videoCatID = (int)categoryGrid.DataKeys[editRowIndex].Values["videoCatID"];
            // Find categoryNameEdit control using the edit row index and cells array
            TextBox newCatName = (TextBox)categoryGrid.Rows[editRowIndex].Cells[1].FindControl("categoryNameEdit");
            string categoryNameEditText = newCatName.Text;
            TextBox newFolderName = (TextBox)categoryGrid.Rows[editRowIndex].Cells[2].FindControl("categoryFolderEdit");
            string folderName = newFolderName.Text;

            // Start SQL commands
            SqlConnection conn;
            SqlCommand comm;
            string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
            conn = new SqlConnection(connectionString);
            comm = new SqlCommand("updateVideoCategory", conn);
            comm.CommandType = System.Data.CommandType.StoredProcedure;
            // Add Parameters
            comm.Parameters.Add("@videoCatID", System.Data.SqlDbType.Int);
            comm.Parameters["@videoCatID"].Value = videoCatID;
            comm.Parameters.Add("@NewcategoryName", System.Data.SqlDbType.NVarChar, 50);
            comm.Parameters["@NewcategoryName"].Value = categoryNameEditText;
            comm.Parameters.Add("@NewscFolder", System.Data.SqlDbType.NVarChar, 50);
            comm.Parameters["@NewscFolder"].Value = folderName;
            try
            {
                conn.Open();
                comm.ExecuteNonQuery();
            }
            finally
            {
                conn.Close();
                categoryGrid.EditIndex = -1;
                BindCategoryGrid();
                h3Title.Text = "Video Category List";
            }
        }
    }

    // Handle Category grid delete button
    protected void categoryGrid_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        int deleteRowIndex = (int)categoryGrid.DataKeys[e.RowIndex].Value;
        string tableName = "videoCategory";
        string rowName = "videoCatID";
        Session["confirmDeletion"] = deleteRowIndex;
        Session["confirmDelTable"] = tableName;
        Session["commParameter"] = rowName;
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
            BindCategoryGrid();
        }
    }
    // Handle delete cancel button
    protected void cancelDelete(object sender, EventArgs e)
    {
        deleteConfirmDiv.Style.Add("display", "none");
        Response.Redirect("videosCat.aspx");
    }
}
