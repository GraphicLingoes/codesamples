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

public partial class help_helpAdmin : System.Web.UI.Page
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
            //Load Intro Message
            messageBox box = new messageBox("notice", "Use menu bar above to get started.", labelNotice);
        }

    }

    // function to bind help topics to new help page drop down menu
    protected void bindHelpTopicDDL()
    {
        SqlConnection conn;
        SqlCommand comm;
        SqlDataReader reader;
        string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
        conn = new SqlConnection(connectionString);
        comm = new SqlCommand("SELECT helpTopicID, name FROM helpTopics ORDER BY helpTopicID ASC", conn);

        try
        {
            conn.Open();
            reader = comm.ExecuteReader();
            dropDownHelpTopics.DataSource = reader;
            dropDownHelpTopics.DataTextField = "name";
            dropDownHelpTopics.DataValueField = "helpTopicID";
            dropDownHelpTopics.DataBind();
            reader.Close();
        }
        catch (SqlException ex)
        {
            labelError.Visible = true;
            messageBox box = new messageBox("error", "There has been an error binding drop down list: [ " + ex.Message + " ]", labelError);
        }
        finally
        {
            conn.Close();
        }
    }


    protected void clickNewTopic(object sender, EventArgs e)
    {
        panelManageHelpTopic.Visible = false;
        PanelManageHelpPages.Visible = false;
        panelNewHelpPage.Visible = false;
        panelNewHelpTopic.Visible = true;
        labelNotice.Visible = false;
    }
    protected void clickManageHelpTopics(object sender, EventArgs e)
    {
        panelManageHelpTopic.Visible = true;
        PanelManageHelpPages.Visible = false;
        panelNewHelpPage.Visible = false;
        panelNewHelpTopic.Visible = false;
        labelNotice.Visible = false;
        // Clear view state to make sure most current records are loaded.
        ViewState["helpTopicDataSet"] = null;
        // Bind manage help topics grid view
        BindHelpTopicGrid();
    }
    protected void clickNewHelpPage(object sender, EventArgs e)
    {
        panelManageHelpTopic.Visible = false;
        PanelManageHelpPages.Visible = false;
        panelNewHelpPage.Visible = true;
        panelNewHelpTopic.Visible = false;
        labelNotice.Visible = false;
        bindHelpTopicDDL();
    }
    protected void clickManageHelpPages(object sender, EventArgs e)
    {
        ViewState["helpPagesDataSet"] = null;
        panelManageHelpTopic.Visible = false;
        PanelManageHelpPages.Visible = true;
        panelNewHelpPage.Visible = false;
        panelNewHelpTopic.Visible = false;
        labelNotice.Visible = false;
        BindHelpPagesGrid();
    }
    protected void clickSaveHelpTopic(object sender, EventArgs e)
    {
        SqlConnection conn;
        SqlCommand comm;
        string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
        conn = new SqlConnection(connectionString);
        comm = new SqlCommand("INSERT INTO helpTopics (name, keyWords) VALUES(@helpTopicName, @helpKeyWords)", conn);
        comm.Parameters.Add("@helpTopicName", SqlDbType.NVarChar, 100);
        comm.Parameters["@helpTopicName"].Value = textBoxTopicName.Text;
        comm.Parameters.Add("@helpKeyWords", SqlDbType.NVarChar, 200);
        comm.Parameters["@helpKeyWords"].Value = textBoxKeyWords.Text;

        string insertName = HttpContext.Current.Server.HtmlEncode(textBoxTopicName.Text);
        if (checkHelpTopicName(insertName))
        {
            labelNotice.Visible = true;
            messageBox box = new messageBox("notice", "There is already a help topic with the name you are trying to use. Please use a different name and try again.", labelNotice);
        }
        else
        {

            try
            {
                conn.Open();
                comm.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                messageBox box = new messageBox("error", "There has been an error: [ " + ex.Message + " ] ", labelError);
            }
            finally
            {
                conn.Close();
                labelNotice.Visible = true;
                messageBox successBox = new messageBox("success", "Success, new help topic has been added.", labelNotice);
            }
        }

    }
    // Function to check if help topic name already exsists
    protected bool checkHelpTopicName(string name)
    {
        SqlConnection conn;
        SqlCommand comm;
        SqlDataReader reader;
        string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
        conn = new SqlConnection(connectionString);
        comm = new SqlCommand("SELECT name FROM helpTopics WHERE name='" + name + "'", conn);

        try
        {
            conn.Open();
            reader = comm.ExecuteReader();
            if (reader.Read())
            {
                reader.Close();
                return true;
            }
            else
            {
                return false;
            }
        }
        finally
        {
            conn.Close();
        }
    }
    protected void clickCancelHelpTopic(object sender, EventArgs e)
    {
        Response.Redirect("~/help/helpAdmin.aspx");
    }

    // Bind help topics Grid
    private void BindHelpTopicGrid()
    {
        SqlConnection conn;
        DataSet dataSet = new DataSet();
        SqlDataAdapter adapter;
        if (ViewState["helpTopicDataSet"] == null)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
            conn = new SqlConnection(connectionString);
            adapter = new SqlDataAdapter("SELECT helpTopicID, name, keyWords FROM helpTopics", conn);
            adapter.Fill(dataSet, "helpTopics");
            ViewState["helpTopicDataSet"] = dataSet;
        }
        else
        {
            dataSet = (DataSet)ViewState["helpTopicDataSet"];
        }
        string sortExpressionHelpTopics;
        if (gridSortDirectionHelpTopics == SortDirection.Ascending)
        {
            sortExpressionHelpTopics = gridsortExpressionHelpTopics + " ASC";
        }
        else
        {
            sortExpressionHelpTopics = gridsortExpressionHelpTopics + " DESC";
        }
        dataSet.Tables["helpTopics"].DefaultView.Sort = sortExpressionHelpTopics;
        gridViewMHelpTopic.DataSource = dataSet.Tables["helpTopics"].DefaultView;
        gridViewMHelpTopic.DataKeyNames = new string[] { "helpTopicID" };
        gridViewMHelpTopic.DataBind();

    }
    // Add Paging to help topics Grid View
    protected void gridViewMHT_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        int newPageIndex = e.NewPageIndex;
        gridViewMHelpTopic.PageIndex = newPageIndex;
        BindHelpTopicGrid();
    }
    // Handle help topic sorting
    protected void gridViewMHT_Sorting(object sender, GridViewSortEventArgs e)
    {
        string sortExpressionHelpTopics = e.SortExpression;
        if (sortExpressionHelpTopics == gridsortExpressionHelpTopics)
        {
            if (gridSortDirectionHelpTopics == SortDirection.Ascending)
            {
                gridSortDirectionHelpTopics = SortDirection.Descending;
            }
            else
            {
                gridSortDirectionHelpTopics = SortDirection.Ascending;
            }
        }
        else
        {
            gridSortDirectionHelpTopics = SortDirection.Ascending;
        }
        gridsortExpressionHelpTopics = sortExpressionHelpTopics;
        BindHelpTopicGrid();
    }
    private SortDirection gridSortDirectionHelpTopics
    {
        get
        {
            if (ViewState["gridSortDirectionHelpTopics"] == null)
            {
                ViewState["gridSortDirectionHelpTopics"] = SortDirection.Ascending;
            }
            return (SortDirection)ViewState["gridSortDirectionHelpTopics"];
        }
        set
        {
            ViewState["gridSortDirectionHelpTopics"] = value;
        }
    }

    private string gridsortExpressionHelpTopics
    {
        get
        {
            if (ViewState["GridsortExpressionHelpTopics"] == null)
            {
                ViewState["GridsortExpressionHelpTopics"] = "helpTopicID";
            }
            return (string)ViewState["GridsortExpressionHelpTopics"];
        }
        set
        {
            ViewState["GridsortExpressionHelpTopics"] = value;
        }
    }

    protected void gridViewMHT_RowEditing(Object sender, GridViewEditEventArgs e)
    {
        // set grid to edit mode then bind data to work with.
        labelNotice.Visible = false;
        gridViewMHelpTopic.EditIndex = e.NewEditIndex;
        BindHelpTopicGrid();
    }

    protected void gridViewMHT_RowCancelingEdit(Object sender, EventArgs e)
    {
        // reset grid, take out of edit mode then rebind to load data.
        labelNotice.Visible = false;
        gridViewMHelpTopic.EditIndex = -1;
        BindHelpTopicGrid();
    }

    protected void gridViewMHT_RowUpdating(Object sender, GridViewUpdateEventArgs e)
    {
        if (gridViewMHelpTopic.EditIndex != -1)
        {
            // Get helpTopicID through the datakey and edit row index
            int editRowIndex = gridViewMHelpTopic.EditIndex;
            int helpTopicID = (int)gridViewMHelpTopic.DataKeys[editRowIndex].Values["helpTopicID"];

            TextBox name = (TextBox)gridViewMHelpTopic.Rows[editRowIndex].Cells[2].FindControl("textBoxName");
            string helpTopicName = name.Text;
            TextBox keyWords = (TextBox)gridViewMHelpTopic.Rows[editRowIndex].Cells[2].FindControl("textBoxKeyWords");
            string helpTopicKeyWords = keyWords.Text;

            SqlConnection conn;
            SqlCommand comm;
            SqlCommand validatComm;
            SqlDataReader reader;
            string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
            conn = new SqlConnection(connectionString);
            validatComm = new SqlCommand("SELECT name FROM helpTopics WHERE name=@helpTopicName AND helpTopicID <>" + helpTopicID.ToString() + "", conn);
            comm = new SqlCommand("UPDATE helpTopics SET name=@helpTopicNameEdit, keyWords=@helpKeyWords WHERE helpTopicID=" + helpTopicID.ToString() + "", conn);
            validatComm.Parameters.Add("@helpTopicName", SqlDbType.NVarChar, 100);
            validatComm.Parameters["@helpTopicName"].Value = helpTopicName;
            comm.Parameters.Add("@helpTopicNameEdit", SqlDbType.NVarChar, 100);
            comm.Parameters["@helpTopicNameEdit"].Value = helpTopicName;
            comm.Parameters.Add("@helpKeyWords", SqlDbType.NVarChar, 200);
            comm.Parameters["@helpKeyWords"].Value = helpTopicKeyWords;

            try
            {
                conn.Open();
                reader = validatComm.ExecuteReader();
                if (!reader.Read())
                {
                    reader.Close();
                    comm.ExecuteNonQuery();
                    labelNotice.Visible = false;
                    // Close connection, clear view state, reset grid to view mode and re-bind to refresh data.
                    conn.Close();
                    ViewState["helpTopicDataSet"] = null;
                    gridViewMHelpTopic.EditIndex = -1;
                    BindHelpTopicGrid();
                }
                else
                {
                    conn.Close();
                    reader.Close();
                    labelNotice.Visible = true;
                    messageBox boxConflict = new messageBox("notice", "The name you are trying to use is already in use, please use a different name and try again.", labelNotice);
                }
            }
            catch (SqlException ex)
            {
                messageBox box = new messageBox("error", "There has been an error updating record: [ " + ex.Message + " ]", labelError);
            }

        }

    }

    protected void gridViewMHT_RowDeleting(Object sender, CommandEventArgs e)
    {
        //check to make sure that grid view is not in edit mode.
        if (gridViewMHelpTopic.EditIndex != -1)
        {
            labelNotice.Visible = true;
            messageBox box = new messageBox("notice", "You can not delete while in edit mode, please exit and try again.", labelNotice);
        }
        else
        {
            // get record id
            int recordID = Convert.ToInt32(e.CommandArgument);
            SqlConnection conn;
            SqlCommand comm;
            string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
            conn = new SqlConnection(connectionString);
            comm = new SqlCommand("DELETE FROM helpTopics WHERE helpTopicID=" + recordID.ToString() + "", conn);

            try
            {
                conn.Open();
                comm.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                labelError.Visible = true;
                messageBox box = new messageBox("error", "There has been an error trying to delete: [ " + ex.Message + " ] ", labelError);
            }
            finally
            {
                conn.Close();
                ViewState["helpTopicDataSet"] = null;
                BindHelpTopicGrid();
            }
        }
    }
    protected void clickSaveNewHelp(object sender, EventArgs e)
    {
        int topicID = Convert.ToInt32(dropDownHelpTopics.SelectedValue);
        string helpPageName = HttpContext.Current.Server.HtmlEncode(textBoxHelpPageName.Text);
        int sortOrder = Convert.ToInt32(textBoxSortOrder.Text);
        string helpPageContent = htmlEditorNewHelp.Content.ToString();

        SqlConnection conn;
        SqlCommand comm;
        SqlCommand validateComm;
        SqlDataReader reader;
        string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
        conn = new SqlConnection(connectionString);
        validateComm = new SqlCommand("SELECT helpPageName FROM helpPages WHERE helpPageName=@helpPageName", conn);
        comm = new SqlCommand("INSERT INTO helpPages (helpTopicID, helpPageName, content, sortOrder) VALUES(@helpTopicID, @helpPageName, @helpPageContent, @sortOrder)", conn);
        comm.Parameters.Add("@helpTopicID", SqlDbType.Int);
        comm.Parameters["@helpTopicID"].Value = topicID;
        comm.Parameters.Add("@helpPageName", SqlDbType.NVarChar, 100);
        comm.Parameters["@helpPageName"].Value = helpPageName;
        comm.Parameters.Add("@helpPageContent", SqlDbType.Text);
        comm.Parameters["@helpPageContent"].Value = helpPageContent;
        comm.Parameters.Add("@sortOrder", SqlDbType.Int);
        comm.Parameters["@sortOrder"].Value = sortOrder;
        validateComm.Parameters.Add("@helpPageName", SqlDbType.NVarChar, 100);
        validateComm.Parameters["@helpPageName"].Value = helpPageName;
        try
        {
            conn.Open();
            reader = validateComm.ExecuteReader();
            reader.Read();
            string validateString = reader["helpPageName"].ToString();
            reader.Close();
            reader = validateComm.ExecuteReader();
            if (reader.Read() && validateString != "Overview")
            {
                reader.Close();
                conn.Close();
                labelNotice.Visible = true;
                messageBox validateBox = new messageBox("notice", "There is already a help page with the name you are trying to use. Please use a new name.", labelNotice);
            }
            else
            {
                reader.Close();
                comm.ExecuteNonQuery();
                conn.Close();
                labelNotice.Visible = true;
                messageBox successBox = new messageBox("success", "Success, new help page has been saved.", labelNotice);
            }
        }
        catch (SqlException ex)
        {
            labelError.Visible = true;
            messageBox box = new messageBox("error", "There has been an error saving help page: [ " + ex.Message + " ]", labelError);
        }
    }
    // Funtion called from front end to build help page edit link
    protected string buildLink(int value)
    {
        return string.Concat("<a href='helpEditView.aspx?ID=", value.ToString(), "' name=\"launchHelpEdit\">View/Edit</a>");
    }
    // Bind Help Page Grid
    private void BindHelpPagesGrid()
    {
        SqlConnection conn;
        DataSet dataSet = new DataSet();
        SqlDataAdapter adapter;
        if (ViewState["helpPagesDataSet"] == null)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
            conn = new SqlConnection(connectionString);
            adapter = new SqlDataAdapter("SELECT helpPages.helpPageID, helpPages.helpTopicID, helpPages.helpPageName, helpPages.sortOrder, helpTopics.[name] " +
                                           "FROM helpTopics " +
                                           "JOIN helpPages on helpTopics.helpTopicID = helpPages.helpTopicID", conn);
            adapter.Fill(dataSet, "helpPages");
            ViewState["helpPagesDataSet"] = dataSet;
        }
        else
        {
            dataSet = (DataSet)ViewState["helpPagesDataSet"];
        }
        string sortExpressionHelpPages;
        if (gridSortDirectionHelpPages == SortDirection.Ascending)
        {
            sortExpressionHelpPages = gridsortExpressionHelpPages + " ASC";
        }
        else
        {
            sortExpressionHelpPages = gridsortExpressionHelpPages + " DESC";
        }
        dataSet.Tables["helpPages"].DefaultView.Sort = sortExpressionHelpPages;
        gridViewMHelpPages.DataSource = dataSet.Tables["helpPages"].DefaultView;
        gridViewMHelpPages.DataKeyNames = new string[] { "helpPageID" };
        gridViewMHelpPages.DataBind();

    }
    // Add Paging to help pages Grid View
    protected void gridViewMHP_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        int newPageIndex = e.NewPageIndex;
        gridViewMHelpPages.PageIndex = newPageIndex;
        BindHelpPagesGrid();
    }
    // Handle help pages sorting
    protected void gridViewMHP_Sorting(object sender, GridViewSortEventArgs e)
    {
        string sortExpressionHelpPages = e.SortExpression;
        if (sortExpressionHelpPages == gridsortExpressionHelpPages)
        {
            if (gridSortDirectionHelpPages == SortDirection.Ascending)
            {
                gridSortDirectionHelpPages = SortDirection.Descending;
            }
            else
            {
                gridSortDirectionHelpPages = SortDirection.Ascending;
            }
        }
        else
        {
            gridSortDirectionHelpPages = SortDirection.Ascending;
        }
        gridsortExpressionHelpPages = sortExpressionHelpPages;
        BindHelpPagesGrid();
    }
    private SortDirection gridSortDirectionHelpPages
    {
        get
        {
            if (ViewState["gridSortDirectionHelpPages"] == null)
            {
                ViewState["gridSortDirectionHelpPages"] = SortDirection.Ascending;
            }
            return (SortDirection)ViewState["gridSortDirectionHelpPages"];
        }
        set
        {
            ViewState["gridSortDirectionHelpPages"] = value;
        }
    }

    private string gridsortExpressionHelpPages
    {
        get
        {
            if (ViewState["GridsortExpressionHelpPages"] == null)
            {
                ViewState["GridsortExpressionHelpPages"] = "helpPageID";
            }
            return (string)ViewState["GridsortExpressionHelpPages"];
        }
        set
        {
            ViewState["GridsortExpressionHelpPages"] = value;
        }
    }

    // Handle delete help page
    protected void gridViewMHP_RowDeleting(object sender, CommandEventArgs e)
    {
        int deleteHelpPageID = Convert.ToInt32(e.CommandArgument);
        SqlConnection conn;
        SqlCommand comm;
        string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
        conn = new SqlConnection(connectionString);
        comm = new SqlCommand("DELETE FROM helpPages WHERE helpPageID=" + deleteHelpPageID.ToString() + "", conn);

        try
        {
            conn.Open();
            comm.ExecuteNonQuery();
        }
        catch (SqlException ex)
        {
            messageBox box = new messageBox("error", "There has been an error trying to delete help page: [ " + ex.Message + " ] ", labelError);
        }
        finally
        {
            conn.Close();
            ViewState["helpPagesDataSet"] = null;
            BindHelpPagesGrid();
        }
    }
}
