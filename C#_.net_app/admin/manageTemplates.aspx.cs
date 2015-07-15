using System;
using System.Collections;
using System.Collections.Generic;
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

public partial class admin_manageTemplates : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            messageBox box = new messageBox("notice", "Use <b>toolbar</b> above to get started.", labelNotice);
            // Bind checkbox list to select videos from
            bindEditTemplatesGrid();
        }
    }

    // Handle paging for 
    protected void gridViewEditTemplates_PageIndexChanging(object sender, GridViewPageEventArgs e) 
    { 
       gridViewEditTemplates.PageIndex = e.NewPageIndex;
       bindEditTemplatesGrid();
       labelNotice.Visible = false;
    }
    
    // Databind template name grid
    // to be able to use server side paging you have to use viewstate to bind data to grid.
    protected void bindEditTemplatesGrid()
    {
        SqlConnection conn;
        DataSet dataSet = new DataSet();
        SqlDataAdapter adapter;
        if (ViewState["editTemplateDataSet"] == null)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
            conn = new SqlConnection(connectionString);
            adapter = new SqlDataAdapter("SELECT * FROM recommendedTemplateName", conn);
            adapter.Fill(dataSet, "templates");
            ViewState["editTemplateDataSet"] = dataSet;
        }
        else
        {
            dataSet = (DataSet)ViewState["editTemplateDataSet"];
        }
        gridViewEditTemplates.DataSource = dataSet.Tables["templates"].DefaultView;
        gridViewEditTemplates.DataKeyNames = new string[] { "recTempNameID" };
        gridViewEditTemplates.DataBind();

    }
    protected void clickNewTemplate(object sender, EventArgs e)
    {
        panelCBList.Visible = true;
        panelNewTemplate.Visible = true;
        labelNotice.Visible = false;
        panelEditTemplates.Visible = false;
        bindRecommendVideosNew();
    }
    protected void clickBackToSearch(object sender, EventArgs e)
    {
        Response.Redirect("recVidTemplates.aspx");
    }
    protected void recommendVidCBL_CheckedChanged(object sender, EventArgs e)
    {
        foreach (ListItem li in recommendVidCBL.Items)
        {
            li.Selected = ((CheckBox)sender).Checked;
        }
    }

    // Check videos off in checkbox list when "Edit Videos" is pressed from grid
    //Load videos to recommended checkbox list
    protected void bindRecommendVideos(int templateID)
    {
        int nID = templateID;
        SqlConnection conn;
        SqlCommand comm;
        DataSet dataSet = new DataSet();
        SqlDataReader reader;
        SqlDataAdapter adapter;
        string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
        conn = new SqlConnection(connectionString);
        comm = new SqlCommand("SELECT videoInfoID, videoName FROM videoInfo ORDER BY videoName ASC", conn);
        adapter = new SqlDataAdapter("SELECT videoID FROM recommendedTemplateVids WHERE recNameID=" + nID + "", conn);
        adapter.Fill(dataSet, "videoInfoIDs");
        try
        {
            conn.Open();
            reader = comm.ExecuteReader();
            recommendVidCBL.DataSource = reader;
            recommendVidCBL.DataTextField = "videoName";
            recommendVidCBL.DataValueField = "videoInfoID";
            recommendVidCBL.DataBind();
            reader.Close();
        }
        catch (SqlException ex)
        {

            messageBox box = new messageBox("error", "There has been an error trying to bind recommended videos<br /> [" + ex.Message + "]", labelDBerror);
        }
        finally
        {
            conn.Close();
        }

        foreach (DataRow dr in dataSet.Tables[0].Rows)
        {
            for(int i = 0; i < recommendVidCBL.Items.Count; i++)
            {
                if(Convert.ToInt32(recommendVidCBL.Items[i].Value) == Convert.ToInt32(dr["videoID"]))
                {
                    recommendVidCBL.Items[i].Selected = true;
                }
            }
        }
    }

    // bind checkbox list for new template
     // Load videos to recommended checkbox list
    protected void bindRecommendVideosNew()
    {
        SqlConnection conn;
        SqlCommand comm;
        SqlDataReader reader;
        string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
        conn = new SqlConnection(connectionString);
        comm = new SqlCommand("SELECT videoInfoID, videoName FROM videoInfo ORDER BY videoName ASC", conn);
        try
        {
            conn.Open();
            reader = comm.ExecuteReader();
            recommendVidCBL.DataSource = reader;
            recommendVidCBL.DataTextField = "videoName";
            recommendVidCBL.DataValueField = "videoInfoID";
            recommendVidCBL.DataBind();
            reader.Close();
        }
        catch (SqlException ex)
        {

            messageBox box = new messageBox("error", "There has been an error trying to bind recommended videos<br /> [" + ex.Message + "]", labelDBerror);
        }
        finally
        {
            conn.Close();
        }
    }
    // Check to see if template name exists
    protected bool checkTempName(string name)
    {
        string templateName = name;
        SqlConnection conn;
        SqlCommand comm;
        SqlDataReader reader;
        string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
        conn = new SqlConnection(connectionString);
        comm = new SqlCommand("SELECT templateName FROM recommendedTemplateName WHERE templateName=@name", conn);
        comm.Parameters.Add("@name", SqlDbType.NVarChar, 250);
        comm.Parameters["@name"].Value = textBoxName.Text;
        try
        {
            conn.Open();
            reader = comm.ExecuteReader();
            if (reader.Read())
            {
                return true;
            }
        }
        catch (SqlException ex)
        {
            messageBox box = new messageBox("error", "There has been an error: [" + ex.Message + "]", labelDBerror);
        }
        finally
        {
            conn.Close();
        }
        
        return false;
    }
    // Handle save button
    protected void clickSaveTemplate(object sender, EventArgs e)
    {
        if(checkTempName(textBoxName.Text.ToString()))
        {
            messageBox box = new messageBox("notice", "The template name you are trying to use already exist, please use a different name to proceed.", labelNotice);
        } 
        else
        {
            if (videosPresent())
            {
                int templateID = createNewTemplate();
                int[] videoIDs = new int[recommendVidCBL.Items.Count];
                int sortOrder = 0;
                bool newRecord = false;
                SqlConnection conn;
                SqlCommand comm;
                string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
                conn = new SqlConnection(connectionString);
                //Loop through video checkbox list to find checked videos
                //Add each checked video to database under
                for (int i = 0; i < recommendVidCBL.Items.Count; i++)
                {
                    videoIDs[i] = Convert.ToInt32(recommendVidCBL.Items[i].Value);
                    if (recommendVidCBL.Items[i].Selected)
                    {
                        sortOrder++;
                        comm = new SqlCommand("INSERT INTO recommendedTemplateVids VALUES(" + templateID.ToString() + ", " + videoIDs[i].ToString() + ", " + sortOrder.ToString() + ")", conn);
                       
                        try
                        {
                            conn.Open();
                            comm.ExecuteNonQuery();
                            newRecord = true;
                        }
                        catch (SqlException ex)
                        {
                            messageBox box = new messageBox("error", "There has been an error: [" + ex.Message + "]", labelDBerror);
                        }
                        finally
                        {
                            conn.Close();
                        }
                        if (newRecord)
                        {
                            messageBox box = new messageBox("success", "Success, new template has been created.", labelNotice);
                        }
                    }
                }
            }
            else
            {
                messageBox box = new messageBox("notice", "You must select videos to create template. Please use video list below to make your choices.", labelNotice);
            }
         }
    }
    // Handle cancel button
    protected void clickCancelTemplate(object sender, EventArgs e)
    {
        Response.Redirect("manageTemplates.aspx");
    }

    //Check to see if videos are checked
    protected bool videosPresent()
    {
        for (int i = 0; i < recommendVidCBL.Items.Count; i++)
        {
            if (recommendVidCBL.Items[i].Selected)
            {
                return true;
            }
        }
        return false;
    }

    // Function to create new template called by the clickSaveTemplate method above
    protected Int32 createNewTemplate()
    {
        SqlConnection conn;
        SqlCommand comm;
        int tempNameID;
        string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
        conn = new SqlConnection(connectionString);
        comm = new SqlCommand("INSERT INTO recommendedTemplateName(templateName, createdDate, modifiedDate, templateDescription, createdBy) VALUES (@name, @date, NULL, @description, @user);SELECT SCOPE_IDENTITY();", conn);
        comm.Parameters.Add("@name", SqlDbType.NVarChar, 250);
        comm.Parameters["@name"].Value = textBoxName.Text;
        comm.Parameters.Add("@description", SqlDbType.NVarChar, 250);
        comm.Parameters["@description"].Value = textBoxDescription.Text;
        comm.Parameters.Add("@date", SqlDbType.NVarChar, 50);
        comm.Parameters["@date"].Value = DateTime.Now.ToString();
        comm.Parameters.Add("@user", SqlDbType.Int);
        comm.Parameters["@user"].Value = Convert.ToInt32(Session["userLoggedIn"]);

        try
        {
            conn.Open();
            int id = Convert.ToInt32(comm.ExecuteScalar());
            return id;
        }
        catch (SqlException ex)
        {
            labelDBerror.Visible = true;
            messageBox box = new messageBox("error", "There has been an error: [ " + ex.Message + " ] ", labelDBerror);
        }
        finally
        {
            conn.Close();
        }

        tempNameID = 0;
        return tempNameID;
    }
    protected void clickEditTemplates(object sender, EventArgs e)
    {
        panelEditTemplates.Visible = true;
        panelNewTemplate.Visible = false;
        panelCBList.Visible = false;
        labelNotice.Visible = false;
        panelTR1.Visible = false;
        panelTR2.Visible = true;
        ViewState["editTemplateDataSet"] = null;
        bindEditTemplatesGrid();
    }
    // Handle Edit Videos Link
    protected void gridViewEditTemplates_RowCommand(Object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "clickEditVideos")
        {
            // Convert the row index stored in the CommandArgument
            // property to an Integer.
            int index = Convert.ToInt32(e.CommandArgument);
            int templateID = (int)gridViewEditTemplates.DataKeys[index].Value;
            panelCBList.Visible = true;
            labelNotice.Visible = false;
            bindRecTemplateGrid(templateID);
            GridViewRow row = (GridViewRow)gridViewEditTemplates.Rows[index];
            globalFunctions.resetGridBackground(gridViewEditTemplates);
            row.Style.Add(HtmlTextWriterStyle.BackgroundColor, "#E1EBED");
            bindRecommendVideos(templateID);
        }
    }

    protected void gridViewEditTemplates_RowEditing(object sender, GridViewEditEventArgs e)
    {
        labelNotice.Visible = false;
        gridViewEditTemplates.EditIndex = e.NewEditIndex;
        //Bind data to the GridView control.
        bindEditTemplatesGrid();
    }


    protected void gridViewEditTemplates_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        labelNotice.Visible = false;
        gridViewEditTemplates.EditIndex = -1;
        //Bind data to the GridView control.
        bindEditTemplatesGrid();
    }

    protected void gridViewEditTemplates_RowUpdating(object sender, EventArgs e)
    {
        bool updateSuccess = false;
        // Get templateID through the datakey and edit row index
        int editRowIndex = gridViewEditTemplates.EditIndex;
        int templateID = (int)gridViewEditTemplates.DataKeys[editRowIndex].Values["recTempNameID"];
        // Find original name to use in validation check when saving
        Label name = (Label)gridViewEditTemplates.Rows[editRowIndex].Cells[1].FindControl("compareName");
        string compareName = name.Text;
        // Find edit info in the gridview control using the edit row index and cells array
        TextBox newTemplateName = (TextBox)gridViewEditTemplates.Rows[editRowIndex].Cells[1].FindControl("textBoxEditName");
        string templateName = newTemplateName.Text;
        TextBox newTemplateDesc = (TextBox)gridViewEditTemplates.Rows[editRowIndex].Cells[2].FindControl("textBoxEditDescription");
        string templateDesc = newTemplateDesc.Text;
        SqlConnection conn;
        SqlCommand validateComm;
        SqlCommand comm;
        SqlDataReader reader;
        string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
        conn = new SqlConnection(connectionString);
        comm = new SqlCommand("UPDATE recommendedTemplateName SET templateName=@name, modifiedDate=@date, templateDescription=@description, modifiedBy=@modifiedBy WHERE recTempNameID=@record", conn);
        validateComm = new SqlCommand("SELECT templateName FROM recommendedTemplateName WHERE templateName=@nameCheck", conn);
        //Add SQL parameters
        validateComm.Parameters.Add("@nameCheck", SqlDbType.NVarChar, 250);
        validateComm.Parameters["@nameCheck"].Value = templateName;
        comm.Parameters.Add("@record", SqlDbType.Int);
        comm.Parameters["@record"].Value = templateID;
        comm.Parameters.Add("@name", SqlDbType.NVarChar, 250);
        comm.Parameters["@name"].Value = templateName;
        comm.Parameters.Add("@date", SqlDbType.NVarChar, 50);
        comm.Parameters["@date"].Value = DateTime.Now.ToString();
        comm.Parameters.Add("@description", SqlDbType.NVarChar, 250);
        comm.Parameters["@description"].Value = templateDesc;
        comm.Parameters.Add("@modifiedBy", SqlDbType.Int);
        comm.Parameters["@modifiedBy"].Value = Convert.ToInt32(Session["userLoggedIn"]);

        try
        {
            conn.Open();
            reader = validateComm.ExecuteReader();
            if (reader.Read())
            {
                // Check to see if the name being updated is the same name that was there before.
                // Update database with row info.
                if (compareName == reader["templateName"].ToString())
                {
                    reader.Close();
                    comm.ExecuteNonQuery();
                    updateSuccess = true;
                }
                else
                {
                    // Name already exist.
                    updateSuccess = false;
                }
            }
            else
            {
                // Name does not exist and name has been modified update row.
                reader.Close();
                comm.ExecuteNonQuery();
                updateSuccess = true;
            }
        }
        catch(SqlException ex)
        {
            messageBox box = new messageBox("error", "There has been an error: [ " + ex.Message + " ]", labelDBerror);
        }
        finally
        {
            conn.Close();
        }

        if (updateSuccess)
        {
            labelNotice.Visible = true;
            messageBox successBox = new messageBox("success", "Success, template has been updated.", labelNotice);
            // Set grid view back to non edit mode.
            gridViewEditTemplates.EditIndex = -1;
            //Bind data to the GridView control.
            bindEditTemplatesGrid();
        }
        else
        {
            labelNotice.Visible = true;
            messageBox box = new messageBox("notice", "The name you are trying to use already exist for another template, please use a different name and try again.", labelNotice);
        }
    }

    protected void gridViewEditTemplates_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        int deleteIndex = e.RowIndex;
        GridViewRow row = (GridViewRow)gridViewEditTemplates.Rows[deleteIndex];
        int templateDeleteID = (int)gridViewEditTemplates.DataKeys[deleteIndex].Values["recTempNameID"];
        // Check to see if any videos are associated to template. If so show error.
        hiddenFieldTempID.Value = templateDeleteID.ToString();
        if (checkDelete(templateDeleteID))
        {
            labelNotice.Visible = true;
            panelDeleteTemplate.Visible = false;
            messageBox box = new messageBox("notice", "There are still videos associated to this template. You must delete them first to delete the template itself.", labelNotice);
            bindRecTemplateGrid(templateDeleteID);
            globalFunctions.resetGridBackground(gridViewEditTemplates);
            row.Style.Add(HtmlTextWriterStyle.BackgroundColor, "#E1EBED");
        }
        else
        {
            labelNotice.Visible = true;
            messageBox box = new messageBox("notice", "Are you sure you wish to delete this template? Once template is deleted it cannot be retrieved and must be re-created to duplicate.", labelNotice);
            panelDeleteTemplate.Visible = true;
            bindRecTemplateGrid(templateDeleteID);
            globalFunctions.resetGridBackground(gridViewEditTemplates);
            row.Style.Add(HtmlTextWriterStyle.BackgroundColor, "#E1EBED");
        }
    }
    // function to check if videos are associated to to template before deleting.
    protected bool checkDelete(int templateID)
    {
        int id = templateID;
        SqlConnection conn;
        SqlCommand checkComm;
        SqlDataReader reader;
        string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
        conn = new SqlConnection(connectionString);
        checkComm = new SqlCommand("SELECT recTempVidID FROM recommendedTemplateVids WHERE recNameID=" + templateID.ToString() + "", conn);

        try
        {
            conn.Open();
            reader = checkComm.ExecuteReader();
            if (reader.Read())
            {
                reader.Close();
                return true;
            }
        }
        catch (SqlException ex)
        {
            labelDBerror.Visible = true;
            messageBox box = new messageBox("error", "There has been an error: [ " + ex.Message + " ]", labelDBerror);
        }
        finally
        {
            conn.Close();
        }
        return false;
    }
    // confirm deletion of template
    protected void clickConfirmTemplateDelete(Object sender, EventArgs e)
    {
        int id = Convert.ToInt32(hiddenFieldTempID.Value);
        SqlConnection conn;
        SqlCommand comm;
        string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
        conn = new SqlConnection(connectionString);
        comm = new SqlCommand("DELETE FROM recommendedTemplateName WHERE recTempNameID=" + id.ToString() + "", conn);
        try
        {
            conn.Open();
            comm.ExecuteNonQuery();
            ViewState["editTemplateDataSet"] = null;
        }
        catch (SqlException ex)
        {
            messageBox box = new messageBox("error", "There has been an error: [ " + ex.Message + " ]", labelDBerror);
        }
        finally
        {
            conn.Close();
            messageBox box = new messageBox("success", "Success, template has been deleted and grid has been refreshed", labelNotice);
            panelDeleteTemplate.Visible = false;
            bindEditTemplatesGrid();
            labelTemplateID.Text = "0";
        }
    }

    // Cancel template name delete
    protected void clickCancelTemplateDelete(object sender, EventArgs e)
    {
        panelDeleteTemplate.Visible = false;
        messageBox box = new messageBox("notice", "Deletion has been cancelled", labelNotice);
        ViewState["editTemplateDataSet"] = null;
        bindEditTemplatesGrid();
    }
    // Bind template videos to list when template "edit videos" is clicked.
    protected void bindRecTemplateGrid(int templateID)
    {
        int id = templateID;
        SqlConnection conn;
        SqlCommand comm;
        SqlDataReader reader;
        string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
        conn = new SqlConnection(connectionString);
        comm = new SqlCommand("SELECT [videoInfo].videoName, [recommendedTemplateVids].recTempVidID, " +
                                "[recommendedTemplateVids].recNameID, [recommendedTemplateVids].videoID, " +
                                "[recommendedTemplateVids].sortID, [recommendedTemplateName].templateName " +
                                "FROM [recommendedTemplateVids]" +
                                "JOIN [videoInfo] on [recommendedTemplateVids].videoID = [videoInfo].videoInfoID " +
                                "JOIN [recommendedTemplateName] on [recommendedTemplateVids].recNameID = [recommendedTemplateName].recTempNameID " +
                                "WHERE [recommendedTemplateVids].recNameID=" + id.ToString() + " ORDER BY sortID ASC", conn);

        try
        {
                conn.Open();
                recVidsTemplateGrid.Visible = true;
                reader = comm.ExecuteReader();
                recVidsTemplateGrid.DataSource = reader;
                recVidsTemplateGrid.DataBind();
                reader.Close();
                labelTemplateID.Text = templateID.ToString();    
                //reader = comm.ExecuteReader();
                //reader.Read();
                //labelTitleInfo.Text = "<h3>" + reader["firstName"].ToString() + reader["lastName"].ToString() + " - " + reader["practiceName"].ToString() + "</h3>";
                //reader.Close();
        }
        catch (SqlException ex)
        {

            messageBox box = new messageBox("error", "There has been an error [" + ex.Message + "]", labelDBerror);
        }
        finally
        {
            conn.Close();
            panelTemplateVids.Visible = true;
        }

       
    }

    // Handle check all checkbox recommended vidoes member
    protected void HeaderLevelCheckBox_CheckedChangedT(object sender, EventArgs e)
    {
        CheckBox chk;
        foreach (GridViewRow rowItem in recVidsTemplateGrid.Rows)
        {
            chk = (CheckBox)(rowItem.Cells[0].FindControl("RowLevelCheckBoxT"));
            chk.Checked = ((CheckBox)sender).Checked;
        }
    }

    protected void clickStartOver(object sender, EventArgs e)
    {
        Response.Redirect("manageTemplates.aspx");
    }
   // Handle adding new videos to template when "Add Videos" button is clicked.
    protected void clickAddVideos(object sender, EventArgs e)
    {
        if (labelTemplateID.Text != "0")
        {
            int nID = Convert.ToInt32(labelTemplateID.Text);
            if (videosPresent())
            {
                int[] videoIDs = new int[recommendVidCBL.Items.Count];
                int sortOrder = 0;
                bool newRecord = false;
                SqlConnection conn;
                SqlCommand comm;
                SqlCommand deleteComm;
                string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
                conn = new SqlConnection(connectionString);
                deleteComm = new SqlCommand("DELETE FROM recommendedTemplateVids WHERE recNameID=" + nID.ToString() + "", conn);
                //delete current selections so you can you re-add them.    
                try
                {
                    conn.Open();
                    deleteComm.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    messageBox box = new messageBox("error", "There has been an error deleting: [ " + ex.Message + " ]", labelDBerror);
                }
                finally
                {
                    conn.Close();
                }
                //Loop through video checkbox list to find checked videos
                //Add each checked video to database under
                for (int i = 0; i < recommendVidCBL.Items.Count; i++)
                {
                    videoIDs[i] = Convert.ToInt32(recommendVidCBL.Items[i].Value);
                    if (recommendVidCBL.Items[i].Selected)
                    {
                        sortOrder++;
                        comm = new SqlCommand("INSERT INTO recommendedTemplateVids VALUES(" + nID.ToString() + ", " + videoIDs[i].ToString() + ", " + sortOrder.ToString() + ")", conn);

                        try
                        {
                            conn.Open();
                            comm.ExecuteNonQuery();
                            newRecord = true;
                        }
                        catch (SqlException ex)
                        {
                            messageBox box = new messageBox("error", "There has been an error: [" + ex.Message + "]", labelDBerror);
                        }
                        finally
                        {
                            conn.Close();
                        }
                        if (newRecord)
                        {
                            messageBox box = new messageBox("success", "Success, template videos have been updated.", labelNotice);
                        }
                    }
                }
            }
            else
            {
                messageBox box = new messageBox("notice", "You must select videos to create template. Please use video list below to make your choices.", labelNotice);
            }
            //rebind video grid to show changes.    
            bindRecTemplateGrid(nID);
        }
        else
        {
            labelNotice.Visible = true;
            messageBox box = new messageBox("notice", "Cannot resolve request, please select template or start over.", labelNotice);
        }
    } 

    // Handle update link button from member's recommended videos grid
    protected void clickUpdateSort(object sender, EventArgs e)
    {
       // panelDelete.Visible = false;
        int hasRows = recVidsTemplateGrid.Rows.Count;
        if (hasRows > 0)
        {
            if (globalFunctions.gridLineItemChecked(recVidsTemplateGrid, "RowLevelCheckBoxT"))
            {
                Label tID = (Label)recVidsTemplateGrid.Rows[0].FindControl("templateIDLbl");
                int templateID = Convert.ToInt32(tID.Text);
                for (int i = 0; i < recVidsTemplateGrid.Rows.Count; i++)
                {
                    GridViewRow row = recVidsTemplateGrid.Rows[i];
                    TextBox updateSortID = (TextBox)recVidsTemplateGrid.Rows[i].FindControl("recVidSortEdit");
                    Label recordID = (Label)recVidsTemplateGrid.Rows[i].FindControl("recordIDLbl");

                    bool isChecked = ((CheckBox)row.FindControl("RowLevelCheckBoxT")).Checked;
                    if (isChecked)
                    {
                        int updateSort = Convert.ToInt32(updateSortID.Text);
                        int recID = Convert.ToInt32(recordID.Text);
                        updateTemplateVids(recID, updateSort, templateID);
                    }
                }
                bindRecTemplateGrid(templateID);
                messageBox boxSuccess = new messageBox("success", "Success, template videos have been updated", labelNotice);
            }
            else
            {
                messageBox box = new messageBox("notice", "You have not selected any videos to update.", labelNotice);
            }
        }
    }
    // Handle update list function for member's recommended videos grid
    // loop through grid find selected items
    // get recVidSortEdit value
    // update recommendedVideos table with value
    protected void updateTemplateVids(int recID, int ID, int templateNameID)
    {
        int recordID = recID;
        int sortID = ID;
        int name = templateNameID;
        SqlConnection conn;
        SqlCommand comm;
        SqlCommand updateModComm;
        string dt = DateTime.Now.ToString();
        int modBy = Convert.ToInt32(Session["userLoggedIn"]);
        string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
        conn = new SqlConnection(connectionString);
        comm = new SqlCommand("UPDATE recommendedTemplateVids SET sortID=" + sortID.ToString() + " WHERE recTempVidID=" + recordID.ToString() + "", conn);
        updateModComm = new SqlCommand("UPDATE recommendedTemplateName SET modifiedDate=@date, modifiedBy=@modifiedBy WHERE recTempNameID=" + name + "", conn);
        updateModComm.Parameters.Add("@date", SqlDbType.NVarChar, 50);
        updateModComm.Parameters["@date"].Value = DateTime.Now.ToString();
        updateModComm.Parameters.Add("@modifiedBy", SqlDbType.Int);
        updateModComm.Parameters["@modifiedBy"].Value = modBy;
        string debug = "UPDATE recommendedTemplateName SET modifiedDate=" + dt + ", modifiedBy=" + modBy.ToString() + " WHERE recTempNameID=" + name.ToString();
        try
        {
            conn.Open();
            comm.ExecuteNonQuery();
            updateModComm.ExecuteNonQuery();
        }
        catch (SqlException ex)
        {
            messageBox box = new messageBox("error", "There has been an error: [" + ex.Message + "<br />" + debug + "]", labelDBerror);
        }
        finally
        {
            conn.Close();
        }
    }

    protected void clickDeleteAssoc(object sender, EventArgs e)
    {
        int hasRows = recVidsTemplateGrid.Rows.Count;
        if (hasRows > 0)
        {
            if (globalFunctions.gridLineItemChecked(recVidsTemplateGrid, "RowLevelCheckBoxT"))
            {
                panelDelete.Visible = true;
                messageBox deleteBox = new messageBox("notice", "Please confirm that you wish to delete selected records", labelNotice);
            }
            else
            {
                panelDelete.Visible = false;
                messageBox box = new messageBox("notice", "You have not selected any videos to delete", labelNotice);
            }
        }
    }
    
    protected void clickConfirmDelete(object sender, EventArgs e)
    {
        Label tID = (Label)recVidsTemplateGrid.Rows[0].FindControl("templateIDLbl");
        int templateID = Convert.ToInt32(tID.Text);
        if (globalFunctions.gridLineItemChecked(recVidsTemplateGrid, "RowLevelCheckBoxT"))
        {
            for (int i = 0; i < recVidsTemplateGrid.Rows.Count; i++)
            {
                GridViewRow row = recVidsTemplateGrid.Rows[i];
                bool isChecked = ((CheckBox)row.FindControl("RowLevelCheckBoxT")).Checked;
                Label recordID = (Label)recVidsTemplateGrid.Rows[i].FindControl("recordIDLbl");
                if (isChecked)
                {
                    int recID = Convert.ToInt32(recordID.Text);
                    deleteTemplateVideo(recID);
                }
            }
            bindRecTemplateGrid(templateID);
            panelDelete.Visible = false;
            messageBox successBox = new messageBox("success", "Success, videos have been deleted from template and grid has been refreshed.", labelNotice);
        }
        else
        {
            messageBox box = new messageBox("notice", "You have not selected anything to delete", labelDBerror);
        }
        bindRecommendVideos(templateID);
    }
    protected void deleteTemplateVideo(int recordID)
    {
        int recID = recordID;
        SqlConnection conn;
        SqlCommand comm;
        string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
        conn = new SqlConnection(connectionString);
        comm = new SqlCommand("DELETE FROM recommendedTemplateVids WHERE recTempVidID=" + recID.ToString() + "", conn);

        try
        {
            conn.Open();
            comm.ExecuteNonQuery();
        }
        catch (SqlException ex)
        {
            messageBox box = new messageBox("error", "There has been an error [ " + ex.Message + " ]", labelDBerror);
        }
        finally
        {
            conn.Close();
        }
    }
    protected void clickCancelDelete(object sender, EventArgs e)
    {
        Label tID = (Label)recVidsTemplateGrid.Rows[0].FindControl("templateIDLbl");
        int templateID = Convert.ToInt32(tID.Text);
        bindRecTemplateGrid(templateID);
        panelDelete.Visible = false;
        messageBox box = new messageBox("notice", "Deletion has been cancelled", labelNotice);
    }
    protected void clickResetSortT(object sender, EventArgs e)
    {
        if (labelTemplateID.Text == "0")
        {
            labelNotice.Visible = true;
            messageBox box = new messageBox("notice", "Cannot resolve request, please select template or start over.", labelNotice);
        }
        else
        {
            Label tID = (Label)recVidsTemplateGrid.Rows[0].FindControl("templateIDLbl");
            int recordID = Convert.ToInt32(tID.Text);
            resetSortOrderT(recordID);
        }
    }

    // Reset sort order
    protected void resetSortOrderT(int templateID)
    {
        int sortCounter = 0;
        int tID = templateID;
        SqlConnection conn;
        string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
        conn = new SqlConnection(connectionString);
        DataSet dataSet = new DataSet();
        SqlDataAdapter adapter;
        adapter = new SqlDataAdapter("SELECT recTempVidID FROM recommendedTemplateVids WHERE recNameID=" + tID.ToString() + "", conn);
        adapter.Fill(dataSet, "recTempIDDS");
        SqlCommand setSortOrderComm;

        foreach (DataRow dr in dataSet.Tables[0].Rows)
        {
            sortCounter++;
            setSortOrderComm = new SqlCommand("UPDATE recommendedTemplateVids SET sortID=" + sortCounter.ToString() + " WHERE recTempVidID=" + dr["recTempVidID"].ToString() + "", conn);
            try
            {
                conn.Open();
                setSortOrderComm.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                messageBox box = new messageBox("error", "There has been an error: [" + ex.Message + "]", labelDBerror);
            }
            finally
            {
                conn.Close();
            }
        }
        messageBox successBox = new messageBox("success", "Success, sort order has been reset", labelNotice);
        bindRecTemplateGrid(tID);

    }

}
