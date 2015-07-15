using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;
using System.Data;
using System.Web.Security;
using System.IO;
using System.Xml;
using System.Text;
using System.Security.Cryptography;
using System.Reflection;

/// <summary>
/// Summary description for globalFunctions
/// </summary>
public class globalFunctions
{
    public globalFunctions()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public static bool validateLogin()
    {
        // Check if user is logged in else redirect them to login page
        if (System.Web.HttpContext.Current.Session["userLoggedIn"] == null)
        {
            return false;
        }

        if (System.Web.HttpContext.Current.Session["userPermissionID"] == null)
        {
            return false;
        }
       
        return true;
    }

    // Check if user exist
    public static bool userExist(Label dbErrorMessage)
    {
        SqlConnection conn;
        SqlCommand comm;
        SqlDataReader reader;
        string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
        conn = new SqlConnection(connectionString);
        comm = new SqlCommand("SELECT email, renOrgID FROM [user] WHERE renUserID=" + System.Web.HttpContext.Current.Session["renUserLoggedIn"] + " and renOrgID=" + System.Web.HttpContext.Current.Session["userOrgID"] + "", conn);

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

            dbErrorMessage.Text = "<div class=\"errorMessage\">The following Error has occurred:<br /> [" + ex.Message + "<br />" + comm + "]</div>";
        }
        finally
        {
            conn.Close();

        }

        return false;
    }


    // Global function to bind user name above help box
    public static void BindmemberName(Label UserFirstName, Label UserLastName)
    {
        UserFirstName.Text = Convert.ToString(System.Web.HttpContext.Current.Session["userInfoFirstName"]);
        UserLastName.Text = Convert.ToString(System.Web.HttpContext.Current.Session["userInfoLastName"]);
    }
    // Function to load video on video detail page
    public static String GenerateVideoCode(int videoID)
    {
        SqlConnection conn;
        SqlCommand comm;
        SqlDataReader reader;
        string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
        conn = new SqlConnection(connectionString);
        comm = new SqlCommand("SELECT videoInfo.videoInfoID, [videoInfo].videoName, videoAuthor.authorName, videoAuthor.authImage, videoAuthor.authTitle, videoInfo.description, videoInfo.lenght, videoCategory.categoryName, videoCategory.scFolder, " +
                                "videoInfo.link, videoInfo.fileName, videoInfo.keyWords, videoInfo.width, videoInfo.height FROM videoAuthor " +
                                "JOIN videoInfo ON videoAuthor.videoAuthID = videoInfo.videoAuthID " +
                                "JOIN videoCategory ON videoInfo.videoCatID = videoCategory.VideoCatID " +
                                "WHERE videoInfoID=" + videoID, conn);
        try
        {
            conn.Open();
            reader = comm.ExecuteReader();
            reader.Read();
            string vidName = reader["videoName"].ToString();
            string vidAuthor = reader["authorName"].ToString();
            string vidAuthImage = reader["authImage"].ToString();
            string vidAuthTitle = reader["authTitle"].ToString();
            string vidDesc = reader["description"].ToString();
            string vidLength = reader["categoryName"].ToString();
            string vidLink = reader["link"].ToString();
            string vidScFolder = reader["scFolder"].ToString();
            string vidFileName = reader["fileName"].ToString();
            string vidWidth = reader["width"].ToString();
            string vidHeight = reader["height"].ToString();
            reader.Close();

            string screenCast = "<object classid=" + "'" + "clsid:D27CDB6E-AE6D-11cf-96B8-444553540000" + "'" + " width=" + "'" + vidWidth + "'" + " height=" + "'" + vidHeight + "'" + ">" +
                "<param name=" + "'" + "movie" + "'" + " value=" + "'" + "http://content.screencast.com/users/LeonardoMD/folders/" + vidScFolder + "/media/" + vidLink + "/bootstrap.swf" + "'" + "></param>" +
                "<param name=" + "'" + "quality" + "'" + " value=" + "'" + "high" + "'" + "></param>" +
                "<param name=" + "'" + "bgcolor" + "'" + " value=" + "'" + "#1A1A1A" + "'" + "></param>" +
                "<param name=" + "'" + "flashVars" + "'" + " value=" + "'" + "thumb=http://content.screencast.com/users/LeonardoMD/folders/" + vidScFolder + "/media/" + vidLink + "/FirstFrame.jpg&content=http://content.screencast.com/users/LeonardoMD/folders/" + vidScFolder + "/media/" + vidLink + "/" + vidFileName + "&width=640&height=498" + "'" + "></param>" +
                "<param name=" + "'" + "allowFullScreen" + "'" + " value=" + "'" + "true" + "'" + "></param>" +
                "<param name=" + "'" + "scale" + "'" + " value=" + "'" + "showall" + "'" + "></param>" +
                "<param name=" + "'" + "allowScriptAccess" + "'" + " value=" + "'" + "always" + "'" + "></param>" +
                "<embed src=" + "'" + "http://content.screencast.com/users/LeonardoMD/folders/" + vidScFolder + "/media/" + vidLink + "/bootstrap.swf" + "'" + " quality=" + "'" + "high" + "'" + " bgcolor=" + "'" + "#1A1A1A" + "'" + " width=" + "'" + vidWidth + "'" + " height=" + "'" + vidHeight + "'" + " type=" + "'" + "application/x-shockwave-flash" + "'" + " allowScriptAccess=" + "'" + "always" + "'" + " flashVars=" + "'" + "thumb=http://content.screencast.com/users/LeonardoMD/folders/" + vidScFolder + "/media/" + vidLink + "/FirstFrame.jpg&content=http://content.screencast.com/users/LeonardoMD/folders/" + vidScFolder + "/media/" + vidLink + "/" + vidFileName + "&width=640&height=498" + "'" + " allowFullScreen=" + "'" + "true" + "'" + " scale=" + "'" + "showall" + "'" + "></embed>" +
                "</object>";

            return screenCast;
      
        }
        catch (SqlException ex)
        {

            string screenCast =
                "<div class=\"errorMessage\">The following Error has occurred:<br /> [" +

ex.Message + "<br />" + comm + "]</div>";
            return screenCast;
        }
        finally
        {
            conn.Close(); 
            
        }
    }
    
    
    public static String GenerateSQL(string SQLparameters, string SQLtable)
    {
        string SQL;
        SQL = "SELECT " + SQLparameters + " FROM " + SQLtable;
        return SQL;

    }

    public static String GenerateWhereSQL(string SQLparameters, string SQLtable, string SQLArgument)
    {
        string SQL;
        SQL = "SELECT " + SQLparameters + " FROM " + SQLtable + " WHERE " + SQLArgument;
        return SQL;

    }

    //Loop through grid function to remove background color
    public static void resetGridBackground(GridView passGrid)
    {
        GridView grid = passGrid;
        int totalRecords = grid.Rows.Count;

        for (int i = 0; i < totalRecords; i++)
        {
            grid.Rows[i].Style.Remove("background-color");
        }
    }


    // function to check for line item checked in passed in Grid View
    public static bool gridLineItemChecked(GridView grid, string control)
    {
        for (int i = 0; i < grid.Rows.Count; i++)
        {
            GridViewRow row = grid.Rows[i];
            bool isChecked = ((CheckBox)row.FindControl(control)).Checked;
            if (isChecked)
            {
                return true;
            }
        }
        return false;
    }

    // function to check for checkboxes checked in passed in checkboxlist control
    public static bool checkBoxListItemChk(CheckBoxList list)
    {
        for (int i = 0; i < list.Items.Count; i++)
        {
            if (list.Items[i].Selected)
            {
                return true;
            }
        }
        return false;
    }
}
