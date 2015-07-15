using System;
using System.Windows;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Data.SqlClient;

public partial class help_helpEditView : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //Set update button as the default so it fires when users hits enter on keyboard.
        Page.Form.DefaultButton = btnEditHelp.UniqueID;
        
        if (!IsPostBack)
        {
            bindHelpTopicDDL();
            bindSelectedHelpPage();
            btnCancelHelp.Attributes.Add("onclick", "window.close();");
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
    
    // function to load selected help page info
    protected void bindSelectedHelpPage()
    {
        string helpPageID = HttpContext.Current.Server.HtmlEncode(Request.QueryString["ID"].ToString());
        SqlConnection conn;
        SqlCommand comm;
        SqlDataReader reader;
        string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
        conn = new SqlConnection(connectionString);
        comm = new SqlCommand("SELECT helpPages.helpPageID, helpPages.helpTopicID, helpPages.helpPageName, helpPages.content, helpPages.sortOrder, helpTopics.[name] " +
                              "FROM helpTopics " +
                              "JOIN helpPages on helpTopics.helpTopicID = helpPages.helpTopicID WHERE helpPages.helpPageID=" + helpPageID.ToString() + "", conn);
        try
        {
            conn.Open();
            reader = comm.ExecuteReader();
            reader.Read();
            textBoxSortOrder.Text = reader["sortOrder"].ToString();
            dropDownHelpTopics.SelectedValue = reader["helpTopicID"].ToString();
            textBoxHelpPageName.Text = reader["helpPageName"].ToString();
            htmlEditorEditHelp.Content = reader["content"].ToString();
            reader.Close();
        }
        catch (SqlException ex)
        {
            messageBox box = new messageBox("error", "There has been an error loading page: [ " + ex.Message + " ]", labelError);
        }
        finally
        {
            conn.Close();
        }
    }
    
    
    protected void clickSaveNewHelp(object sender, EventArgs e)
    {
        int topicID = Convert.ToInt32(dropDownHelpTopics.SelectedValue);
        string helpPageID = HttpContext.Current.Server.HtmlEncode(Request.QueryString["ID"].ToString());
        string helpPageName = HttpContext.Current.Server.HtmlEncode(textBoxHelpPageName.Text);
        string helpPageEditContent = htmlEditorEditHelp.Content.ToString();
        int sortOrder = Convert.ToInt32(textBoxSortOrder.Text);
        bool update = false;

        SqlConnection conn;
        SqlCommand comm;
        SqlCommand validateComm;
        SqlDataReader reader;
        string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
        conn = new SqlConnection(connectionString);
        validateComm = new SqlCommand("SELECT helpPageName FROM helpPages WHERE helpPageName=@helpPageName AND helpPageID !=@helpPageIDValidate", conn);
        comm = new SqlCommand("UPDATE helpPages SET helpTopicID=@helpTopicID, helpPageName=@helpPageName, content=@helpPageEditContent, sortOrder=@sortOrder WHERE helpPageID=@helpPageID", conn);
        comm.Parameters.Add("@sortOrder", SqlDbType.Int);
        comm.Parameters["@sortOrder"].Value = sortOrder;
        comm.Parameters.Add("@helpTopicID", SqlDbType.Int);
        comm.Parameters["@helpTopicID"].Value = topicID;
        comm.Parameters.Add("@helpPageName", SqlDbType.NVarChar, 100);
        comm.Parameters["@helpPageName"].Value = helpPageName;
        comm.Parameters.Add("@helpPageEditContent", SqlDbType.Text);
        comm.Parameters["@helpPageEditContent"].Value = helpPageEditContent.ToString();
        comm.Parameters.Add("@helpPageID", SqlDbType.Int);
        comm.Parameters["@helpPageID"].Value = Convert.ToInt32(helpPageID);
        validateComm.Parameters.Add("@helpPageIDValidate", SqlDbType.Int);
        validateComm.Parameters["@helpPageIDValidate"].Value = Convert.ToInt32(helpPageID);
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
                update = false;
                messageBox validateBox = new messageBox("notice", "There is already a help page with the name you are trying to use. Please use a new name.", labelNotice);
            }
            else
            {
                reader.Close();
                comm.ExecuteNonQuery();
                conn.Close();
                update = true;
            }
        }
        catch (SqlException ex)
        {
            labelError.Visible = true;
            messageBox box = new messageBox("error", "There has been an error saving help page: [ " + ex.Message + " ]", labelError);
        }
        finally
        {
            conn.Close();
        }
        if (update)
        {
            labelNotice.Visible = true;
            messageBox box = new messageBox("success", "Success, help page has been updated", labelNotice);
        }
    }
}
