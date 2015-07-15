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

public partial class admin_impSteps : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Handle mini ribbon color change if menu item active
        LinkButton mpLinkButtonM = (LinkButton)Master.FindControl("membersLB");
        mpLinkButtonM.ForeColor = System.Drawing.Color.White;

        LinkButton mpLinkButtonV = (LinkButton)Master.FindControl("videosLB");
        mpLinkButtonV.ForeColor = System.Drawing.Color.White;

        LinkButton mpLinkButtonIS = (LinkButton)Master.FindControl("helpPagesLB");
        mpLinkButtonIS.ForeColor = System.Drawing.ColorTranslator.FromHtml("#9CC5C9");

        LinkButton mpLinkButtonRC = (LinkButton)Master.FindControl("recVidsLB");
        mpLinkButtonRC.ForeColor = System.Drawing.Color.White;

        LinkButton mpLinkButtonS = (LinkButton)Master.FindControl("searchLB");
        mpLinkButtonS.ForeColor = System.Drawing.Color.White;

        if (!IsPostBack)
        {
            obj_addNew.Visible = false;
            h3Title.Text = "Implementation Steps List";
            SqlConnection videoConn;
            SqlCommand videoEdComm;
            SqlDataReader Videoreader;
            string vidoeConnectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
            videoConn = new SqlConnection(vidoeConnectionString);
            videoEdComm = new SqlCommand("SELECT videoEdID, edition FROM videoEdition", videoConn);


            try
            {

                videoConn.Open();
                // Bind video Edition Drop Down List.
                Videoreader = videoEdComm.ExecuteReader();
                cbVidEdIS.DataSource = Videoreader;
                cbVidEdIS.DataTextField = "edition";
                cbVidEdIS.DataValueField = "videoEdID";
                cbVidEdIS.SelectedValue = "8";
                cbVidEdIS.DataBind();
                Videoreader.Close();
            }
            finally
            {
                videoConn.Close();
            }
   
            
            // Bind implemenation steps grid
            BindimpStepGrid();

        }
    }

    // Handle add new imp step click
    protected void clickAddNew(object sender, EventArgs e)
    {
        obj_addNew.Visible = true;
        obj_categoryGrid.Visible = false;
        newImpStepBtn.Visible = false;
        h3Title.Text = "";
    }

    // Create function for binding implementation steps
    private void BindimpStepGrid()
    {
        SqlConnection conn;
        SqlCommand comm;
        SqlCommand commImpStepEd;
        SqlDataReader reader;
        
        string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
        conn = new SqlConnection(connectionString);
        comm = new SqlCommand("SELECT impSteps.impStepID, impSteps.impStepName, impSteps.Priority, impSteps.Description " +
                   "FROM impSteps ORDER BY Priority ASC", conn);
        try
        {
            conn.Open();
            reader = comm.ExecuteReader();
            impStepGrid.DataSource = reader;
            impStepGrid.DataBind();
            reader.Close();
            
        // Loop through the grid row index to display edition impStepGrid
        // Remember you can access the datakey for each row by looking at its RowIndex after defining the GridViewRow
            foreach (GridViewRow rowItem in impStepGrid.Rows)
            {
                commImpStepEd = new SqlCommand("SELECT videoEdition.videoEdID, videoEdition.edition, impStepEditions.impStepEdID, " +
                                      "impStepEditions.impStepID FROM impStepEditions " +
                                      "JOIN videoEdition ON impStepEditions.videoEdID = videoEdition.videoEdID " +
                                      "JOIN impSteps ON impStepEditions.impStepID = impSteps.impStepID " +
                                      "WHERE impStepEditions.impStepID=@impStepID", conn);
                commImpStepEd.Parameters.Add("@impStepID", SqlDbType.Int);
                commImpStepEd.Parameters["@impStepID"].Value = impStepGrid.DataKeys[rowItem.RowIndex].Value.ToString();

                ListBox cboImpStepCbList = (ListBox)(rowItem.Cells[3].FindControl("impStepEditionLb"));
                reader = commImpStepEd.ExecuteReader();
                cboImpStepCbList.DataSource = reader;
                cboImpStepCbList.DataTextField = "edition";
                cboImpStepCbList.DataValueField = "videoEdID";
                cboImpStepCbList.DataBind();
                reader.Close();
            }
        }
        catch (SqlException ex)
        {

            sqlError.Text =
                "<div class=\"errorMessageSearch\">Error There has been an error trying to bind the implementaton steps grid. <br /> <b>[" + ex.Message + "]</b></div>";
        }
        finally
        {
            conn.Close();
        }


    }
   // Handle new imp step addition
    protected void newImpStep(Object s, EventArgs e)
    {
        if (IsPostBack)
        {

            SqlConnection conn;
            SqlCommand comm;
            SqlCommand cbNewImpComm;
            SqlDataReader reader;
            string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
            conn = new SqlConnection(connectionString);
            comm = new SqlCommand("newImpStep", conn);
            comm.CommandType = System.Data.CommandType.StoredProcedure;
            // Add Parameters
            comm.Parameters.Add("@NewimpStepName", System.Data.SqlDbType.NVarChar, 200);
            comm.Parameters["@NewimpStepName"].Value = impStepName.Text;
            comm.Parameters.Add("@NewimpStepPriority", System.Data.SqlDbType.NVarChar, 50);
            comm.Parameters["@NewimpStepPriority"].Value = impStepPriority.Text;
            comm.Parameters.Add("@newimpStepDescription", System.Data.SqlDbType.NVarChar, 2000);
            comm.Parameters["@newimpStepDescription"].Value = impStepDescription.Text;

           
            try
            {
                conn.Open();
                reader = comm.ExecuteReader();
                reader.Read();
                Session["impValue"] = reader["Value"];
                reader.Close();         
            }
            catch (SqlException ex)
            {

                newImpStepError.Style.Clear();
                newImpStepError.Text =
                    "<div class=\"errorMessageSearch\">Error There has been an error trying to create your new video <br /> [" + ex.Message + "]</div>";
            }
            finally
            {
                conn.Close();
            }

            // loop through new selected video editions so you can add new values when updating details.
            foreach (ListItem item in cbVidEdIS.Items)
            {    
                int impStepID = Convert.ToInt32(Session["impValue"]);
                cbNewImpComm = new SqlCommand("newImpStepEdition", conn);
                cbNewImpComm.CommandType = System.Data.CommandType.StoredProcedure;
                cbNewImpComm.Parameters.Add("@NewimpStepEditionsImpStepID", System.Data.SqlDbType.Int);
                cbNewImpComm.Parameters["@NewimpStepEditionsImpStepID"].Value = impStepID;
                cbNewImpComm.Parameters.Add("@NewimpStepEditionsVideoEdID", System.Data.SqlDbType.Int);
                cbNewImpComm.Parameters["@NewimpStepEditionsVideoEdID"].Value = item.Value;
                if (item.Selected)
                {
                    try
                    {
                        conn.Open();
                        cbNewImpComm.ExecuteNonQuery();
                    }
                    catch (SqlException exA)
                    {

                        newImpStepError.Style.Clear();
                        newImpStepError.Text =
                            "<div class=\"errorMessageSearch\">Error There has been an error trying to re-add imp steps <br /> [" + exA.Message + "]</div>";
                    }
                    finally
                    {
                        conn.Close();
                    }

                }
            }

            newImpStepSuccess.Style.Clear();
            newImpStepSuccess.Text = "<div class=\"success\">" + impStepName.Text + " has been added to implementation steps.</div>";
            impStepName.Text = "";
            impStepPriority.Text = "";
            impStepDescription.Text = "";
            cbVidEdIS.SelectedValue = "8";
            BindimpStepGrid();
            obj_addNew.Visible = false;
            obj_categoryGrid.Visible = true;
            newImpStepBtn.Visible = true;
            h3Title.Text = "Implementation Steps List";

        }
    }

    protected void BindimpStepGridEdit()
    {
        SqlConnection conn;
        SqlCommand comm;
        SqlCommand commImpStepEd;
        SqlDataReader reader;
        
        string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
        conn = new SqlConnection(connectionString);
        comm = new SqlCommand("SELECT impSteps.impStepID, impSteps.impStepName, impSteps.Priority, impSteps.Description " +
                   "FROM impSteps ORDER BY Priority ASC", conn);
        try
        {
            conn.Open();
            reader = comm.ExecuteReader();
            impStepGrid.DataSource = reader;
            impStepGrid.DataBind();
            reader.Close();
            
          commImpStepEd = new SqlCommand("SELECT videoEdition.videoEdID, videoEdition.edition FROM videoEdition", conn);
           int editRowIndex = impStepGrid.EditIndex;
           CheckBoxList cboImpStepCbList = (CheckBoxList)impStepGrid.Rows[editRowIndex].Cells[2].FindControl("cbVidEdIsEdit");
           reader = commImpStepEd.ExecuteReader();
           cboImpStepCbList.DataSource = reader;
           cboImpStepCbList.DataTextField = "edition";
           cboImpStepCbList.DataValueField = "videoEdID";
           cboImpStepCbList.DataBind();
           reader.Close();
        }
        catch (SqlException ex)
        {

            sqlError.Text =
                "<div class=\"errorMessageSearch\">Error There has been an error trying to bind the implementaton steps grid. <br /> <b>[" + ex.Message + "]</b></div>";
        }
        finally
        {
            conn.Close();
        }


    }
    
    
    // Handle edit imp steps grid event
    protected void impStepGrid_RowEditing(object sender, GridViewEditEventArgs e)
    {
        h3Title.Text = "Edit Implementation Step";
        impStepGrid.EditIndex = e.NewEditIndex;
        //Bind data to the GridView control.
        BindimpStepGridEdit();
        // Find edit index and set to variable
        int editRowIndex = impStepGrid.EditIndex;
        // Grab datakey id to find selected ddl value for edition
        int impStepID = (int)impStepGrid.DataKeys[editRowIndex].Values["impStepID"];
        // Find control that you are looking for using edit index and search array.
        CheckBoxList cboCbVidID = (CheckBoxList)impStepGrid.Rows[editRowIndex].Cells[2].FindControl("cbVidEdIsEdit");
        // Bind grid in edit mode
        SqlConnection conn;
        //SqlCommand cbVideoEdComm;
        SqlCommand cbSelectedComm;
        SqlDataReader reader;
        string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
        conn = new SqlConnection(connectionString);
        //cbVideoEdComm = new SqlCommand("SELECT videoEdID, edition FROM videoEdition", conn);
        cbSelectedComm = new SqlCommand("SELECT videoEdID FROM impStepEditions " +
                                      "WHERE impStepID=@impStepID", conn);
        cbSelectedComm.Parameters.Add("impStepID", SqlDbType.Int);
        cbSelectedComm.Parameters["impStepID"].Value = impStepID.ToString();
        try
        {
            conn.Open();
            //Bind edition selected values
            reader = cbSelectedComm.ExecuteReader();
            while (reader.Read())
            {
                ListItem cboListItemEd = cboCbVidID.Items.FindByValue(reader["videoEdId"].ToString());
                if (cboListItemEd != null)
                {
                    cboListItemEd.Selected = true;
                }
            }
            reader.Close(); 
        }
        catch (SqlException ex)
        {

            sqlError.Text =
                "<div class=\"errorMessageSearch\">Error There has been an error trying to bind the implementaton steps grid. <br /> <b>[" + ex.Message + "]</b></div>";
        }
        finally
        {
            conn.Close();
        }


    }
    // Handle imp steps cancel event
    protected void impStepGrid_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        //Reset the edit index.
        impStepGrid.EditIndex = -1;
        //Bind data to the GridView control.
        BindimpStepGrid();
        h3Title.Text = "Implementation Steps List";

    }
    //Handle imp steps update event
    protected void impStepGrid_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        if (impStepGrid.EditIndex != -1)
        {
            h3Title.Text = "Implementation Steps List";
            // Get impStepID through the datakey and edit row index
            int editRowIndex = impStepGrid.EditIndex;
            int impStepID = (int)impStepGrid.DataKeys[editRowIndex].Values["impStepID"];
           
            //Find checkboxes and delete previous values
            SqlConnection conn;
            SqlCommand impEdCkBoxDeleteComm;
            SqlCommand impEdCkBoxAddComm;
            SqlCommand comm;
            string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
            conn = new SqlConnection(connectionString);
           
            CheckBoxList cboImpStepEdCbList = (CheckBoxList)impStepGrid.Rows[editRowIndex].Cells[2].FindControl("cbVidEdIsEdit");
            impEdCkBoxDeleteComm = new SqlCommand("DELETE FROM impStepEditions WHERE impStepID=" + impStepID + "", conn);
            try
            {
                conn.Open();
                impEdCkBoxDeleteComm.ExecuteNonQuery();
            }
            catch (SqlException exB)
            {
                newImpStepError.Style.Clear();
                newImpStepError.Text =
                    "<div class=\"errorMessageSearch\">Error There has been an error trying to delete Imp Steps <br /> [" + exB.Message + "]</div>";
            }
            finally
            {
                conn.Close();
            }
            // loop through new selected video editions so you can add new values when updating details.
            foreach (ListItem item in cboImpStepEdCbList.Items)
            {
                impEdCkBoxAddComm = new SqlCommand("newImpStepEdition", conn);
                impEdCkBoxAddComm.CommandType = System.Data.CommandType.StoredProcedure;
                impEdCkBoxAddComm.Parameters.Add("@NewimpStepEditionsImpStepID", SqlDbType.Int);
                impEdCkBoxAddComm.Parameters["@NewimpStepEditionsImpStepID"].Value = impStepID;
                impEdCkBoxAddComm.Parameters.Add("@NewimpStepEditionsVideoEdID", SqlDbType.Int);
                impEdCkBoxAddComm.Parameters["@NewimpStepEditionsVideoEdID"].Value = item.Value;
               
                if (item.Selected)
                {
                    try
                    {
                        conn.Open();
                        impEdCkBoxAddComm.ExecuteNonQuery();

                    }
                    catch (SqlException exA)
                    {

                        newImpStepError.Style.Clear();
                        newImpStepError.Text =
                            "<div class=\"errorMessageSearch\">Error There has been an error trying to re-add imp steps <br /> [" + exA.Message + " " + impStepID + "]</div>";
                    }
                    finally
                    {
                        conn.Close();
                    }

                }
            }


            // Find imp step name, priority, description edit control using the edit row index and cells array
            TextBox newImpStepName = (TextBox)impStepGrid.Rows[editRowIndex].Cells[1].FindControl("impStepNameEdit");
            string NewimpStepName = newImpStepName.Text;
            TextBox newImpStepPriority = (TextBox)impStepGrid.Rows[editRowIndex].Cells[2].FindControl("impStepPriorityEdit");
            string NewimpStepPriority = newImpStepPriority.Text;
            TextBox newImpStepDesc = (TextBox)impStepGrid.Rows[editRowIndex].Cells[4].FindControl("impStepDescriptionEdit");
            string NewimpStepDesc = newImpStepDesc.Text;
            // Start SQL commands
           
            comm = new SqlCommand("updateImpSteps", conn);
            comm.CommandType = System.Data.CommandType.StoredProcedure;
            // Add Parameters
            comm.Parameters.Add("@impStepID", System.Data.SqlDbType.Int);
            comm.Parameters["@impStepID"].Value = impStepID;
            comm.Parameters.Add("@NewimpStepName", System.Data.SqlDbType.NVarChar, 200);
            comm.Parameters["@NewimpStepName"].Value = NewimpStepName;
            comm.Parameters.Add("@NewimpStepPriority", System.Data.SqlDbType.NVarChar, 50);
            comm.Parameters["@NewimpStepPriority"].Value = NewimpStepPriority;
            comm.Parameters.Add("@NewimpStepDescription", System.Data.SqlDbType.NVarChar, 2000);
            comm.Parameters["@NewimpStepDescription"].Value = NewimpStepDesc;
            try
            {
                conn.Open();
                comm.ExecuteNonQuery();
            }
            finally
            {
                conn.Close();
                impStepGrid.EditIndex = -1;
                BindimpStepGrid();
            }
        }
    }

    // Handle delete button
    protected void impStepGrid_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        int deleteRowIndex = (int)impStepGrid.DataKeys[e.RowIndex].Value;
        string tableName = "impSteps";
        Session["confirmDeletionPopUp"] = deleteRowIndex;
        Session["confirmDelTablePopUp"] = tableName;
        Session["commParameterPopUp"] = "impStepID";
        deleteConfirmDiv.Style.Clear();
        deleteConfirmDiv.Style.Add("clear", "both");
        deleteConfirmation.Text = "<div class=\"errorMessageSearch\">Please confirm you wish to delete this record?</div>";
    }
    // Handle delete confirm button
    protected void deleteTrue(object sender, EventArgs e)
    {
        SqlConnection conn;
        SqlCommand associatedComm;
        SqlCommand comm;
        string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
        conn = new SqlConnection(connectionString);
        associatedComm = new SqlCommand("DELETE FROM impStepEditions WHERE " + Session["commParameterPopUp"] + "=" + Session["confirmDeletionPopUp"] + "", conn);
        comm = new SqlCommand("DELETE FROM " + Session["confirmDelTablePopUp"] + " WHERE " + Session["commParameterPopUp"] + "=" + Session["confirmDeletionPopUp"] + "", conn);
        
        try
        {
            conn.Open();
            associatedComm.ExecuteNonQuery();
            comm.ExecuteNonQuery();
        }
        finally
        {
            conn.Close();
            BindimpStepGrid();
            Response.Redirect("impSteps.aspx");
        }
    }
    // Handle delete cancel button
    protected void cancelDelete(object sender, EventArgs e)
    {
        deleteConfirmDiv.Style.Add("display", "none");
        Response.Redirect("impSteps.aspx");
    }

    // Handle add new cancel button
    protected void cancelNewImpStep(object sender, EventArgs e)
    {
        
      Response.Redirect("impSteps.aspx");
        
    }


}
