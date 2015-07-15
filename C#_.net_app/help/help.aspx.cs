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

public partial class help_helpAP : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //Set sign button as the default so it fires when users hits enter on keyboard.
        Page.Form.DefaultButton = helpPageSearchBtn.UniqueID;
        if (!IsPostBack)
        {
            //Bind Help Topics
            bindHelpTopicsGrid();
            panelHelpPage.Visible = false;
        }
    }

    protected void bindHelpTopicsGrid()
    {
        SqlConnection conn;
        SqlCommand comm;
        SqlDataReader reader;
        string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
        conn = new SqlConnection(connectionString);
        comm = new SqlCommand("SELECT * FROM helpTopics", conn);

        try
        {
            conn.Open();
            reader = comm.ExecuteReader();
            repeaterHelpTopics.DataSource = reader;
            repeaterHelpTopics.DataBind();

        }
        catch (SqlException ex)
        {
            messageBox box = new messageBox("error", "There has been an error loading help topics: [" + ex.Message + " ] ", labelError);
        }
        finally
        {
            conn.Close();
        }
    }
    protected void videoSearch(object sender, EventArgs e)
    {
        string searchCriteria = helpSearchText.Text.ToString();
        string searchfield = helpSearchBy.SelectedValue.ToString();
        SqlConnection conn;
        SqlCommand comm;
        SqlDataReader reader;
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
        conn = new SqlConnection(connectionString);
        comm = new SqlCommand("SELECT helpPages.helpPageName, helpPages.helpPageID, helpPages.helpTopicID, helpPages.content " +
                              "FROM helpPages " +
                              "JOIN helpTopics on helpPages.helpTopicID = helpTopics.helpTopicID " +
                              "WHERE " + searchfield + " LIKE '%' + @searchCriteria + '%'", conn);
        comm.Parameters.Add("@searchCriteria",SqlDbType.NVarChar,100);
        comm.Parameters["@searchCriteria"].Value = searchCriteria.ToString();

        try
        {
            conn.Open();
            reader = comm.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    // build help Page Links page links using string builder class
                    sb.Append("<li class=\"subMenu\"><a href='?hp=");
                    sb.Append(reader["helpPageID"]);
                    sb.Append("' name=\"clickSearchPage\">");
                    sb.Append(reader["helpPageName"].ToString());
                    sb.Append("</a></li>");
                }

                // set string with search results to label and make panel containing label visible
                labelDisplayHelpPage.Text = sb.ToString();
                labelNotice.Visible = false;
                panelHelpPage.Visible = true;
            }
            else
            { 
                messageBox box = new messageBox("notice", "We're sorry, your search did not return any results.", labelNotice);
                labelDisplayHelpPage.Text = "";
                labelNotice.Visible = true;
                panelHelpPage.Visible = true;
            }
        }
        catch (SqlException ex)
        {
            messageBox box = new messageBox("error", "There has been an error returning search: [ " + ex.Message + " ] ", labelError);
        }
        finally
        {
            conn.Close();
        }



    }

    protected string buildSubHelpTopics(int topicID)
    {
        SqlConnection conn;
        SqlCommand comm;
        SqlDataReader reader;
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
        conn = new SqlConnection(connectionString);
        comm = new SqlCommand("SELECT helpPageID, helpPageName, sortOrder FROM helpPages WHERE helpTopicID=@helpTopicID ORDER BY sortOrder ASC", conn);
        comm.Parameters.Add("@helpTopicID", SqlDbType.Int);
        comm.Parameters["@helpTopicID"].Value = topicID;

        try
        {
            conn.Open();
            reader = comm.ExecuteReader();
            while (reader.Read())
            {
                // build subtopic page links using string builder class
                sb.Append("<li class=\"subMenu\"><a href='?hp=");
                sb.Append(reader["helpPageID"]);
                sb.Append("' name=\"clickSubMenu\">");
                sb.Append(reader["helpPageName"].ToString());
                sb.Append("</a></li>");
            }
            reader.Close();
            reader = comm.ExecuteReader();
            if (reader.Read())
            {
                return sb.ToString();
            }
            reader.Close();

        }
        catch (SqlException ex)
        {
            messageBox box = new messageBox("error", "There has been an error loading sub categories: [ " + ex.Message + " ] ", labelError);
        }
        finally
        {
            conn.Close();
        }
        return sb.ToString();
    }
}
