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

public partial class verify : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

        //Prevent page from being cached so user won't create another record for themselves.
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        if (Session["renUserLoggedIn"] == null)
        {
            Response.Redirect("Login.aspx");
        }
        // Check if user exist
        if (globalFunctions.userExist(this.dbErrorMessage))
        {
            Response.Redirect("Logout.aspx");
        }
        
        if (!IsPostBack)
        {
            //Load States drop down menu 
            SqlConnection conn;
            SqlCommand statesComm;
            SqlDataReader reader;
            string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
            conn = new SqlConnection(connectionString);
            statesComm = new SqlCommand("SELECT abbr, name FROM us_postal_codes", conn);
            try
            {
                // Bind Registration States Drop Down List.
                conn.Open();
                reader = statesComm.ExecuteReader();
                statesList.DataSource = reader;
                statesList.DataValueField = "abbr";
                statesList.DataTextField = "name";
                statesList.DataBind();
                reader.Close();
            }
            finally
            {
                conn.Close();
            }

            practiceName.Text = Session["userPracticeName"].ToString();
            email.Text = Session["email"].ToString();
            firstName.Text = Session["userInfoFirstName"].ToString();
            lastName.Text = Session["userInfoLastName"].ToString();
        }
        // Append blank space to drop down list
       // DropDownList ddl = (DropDownList)sender;
        ListItem emptyItem = new ListItem("", "");
        statesList.Items.Insert(0, emptyItem);
    }

    protected void register(object sender, EventArgs e)
    {
        //Register New User and check to see if email address is already in use.
        if (Page.IsValid)
        {
            SqlConnection conn;
            SqlCommand comm;
            SqlCommand permComm;
            SqlCommand validateComm;
            SqlDataReader reader;
            string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
            conn = new SqlConnection(connectionString);
            validateComm = new SqlCommand("SELECT email, renOrgID FROM [user] WHERE renUserID=" + Session["renUserLoggedIn"] + " and renOrgID=" + Session["userOrgID"] + "", conn);
            permComm = new SqlCommand("SELECT permissionID FROM [user] WHERE userID=" + Session["userLoggedIn"] + "", conn);
            comm = new SqlCommand("newMemberRegistration", conn);
            comm.CommandType = System.Data.CommandType.StoredProcedure;

            //Add Parameters
            comm.Parameters.Add("@NewfirstName", System.Data.SqlDbType.NVarChar, 100);
            comm.Parameters["@NewfirstName"].Value = firstName.Text;
            comm.Parameters.Add("@NewlastName", System.Data.SqlDbType.NVarChar, 100);
            comm.Parameters["@NewlastName"].Value = lastName.Text;
            comm.Parameters.Add("@Newaddress1", System.Data.SqlDbType.NVarChar, 100);
            comm.Parameters["@Newaddress1"].Value = address1.Text;
            comm.Parameters.Add("@Newaddress2", System.Data.SqlDbType.NVarChar, 100);
            comm.Parameters["@Newaddress2"].Value = address2.Text;
            comm.Parameters.Add("@Newcity", System.Data.SqlDbType.NVarChar, 60);
            comm.Parameters["@Newcity"].Value = city.Text;
            comm.Parameters.Add("@Newstate", System.Data.SqlDbType.NVarChar, 2);
            comm.Parameters["@Newstate"].Value = statesList.Text;
            comm.Parameters.Add("@NewzipCode", System.Data.SqlDbType.NVarChar, 10);
            comm.Parameters["@NewzipCode"].Value = zipCode.Text;
            comm.Parameters.Add("@Newemail", System.Data.SqlDbType.NVarChar, 100);
            comm.Parameters["@Newemail"].Value = email.Text;
            comm.Parameters.Add("@NewcontactPhone", System.Data.SqlDbType.NVarChar, 20);
            comm.Parameters["@NewcontactPhone"].Value = phone.Text;
            comm.Parameters.Add("@NewpracticeName", System.Data.SqlDbType.NVarChar, 200);
            comm.Parameters["@NewpracticeName"].Value = practiceName.Text;
            comm.Parameters.Add("@Newtitle", System.Data.SqlDbType.NVarChar, 50);
            comm.Parameters["@Newtitle"].Value = title.Text;
            comm.Parameters.Add("@NewrenUserID", System.Data.SqlDbType.Int);
            comm.Parameters["@NewrenUserID"].Value = Convert.ToInt32(Session["renUserLoggedIn"]);
            comm.Parameters.Add("@NewrenOrgID", System.Data.SqlDbType.Int);
            comm.Parameters["@NewrenOrgID"].Value = Convert.ToInt32(Session["userOrgID"]);
           
            try
            {
                conn.Open();
                reader = validateComm.ExecuteReader();
                reader.Read();
                // Check to see if user and org ID exist already. If not add new user.
                if(reader.Read())
                {
                    Session["validateFailed"] = "<div class='errorMessageSearch'>It appears that you have logged in before, please try your login again.</div>";
                    reader.Close();
                }
                else
                {
                    reader.Close();
                    reader = comm.ExecuteReader();
                    reader.Read();
                    Session["userLoggedIn"] = reader["Value"];
                    reader.Close();
                    reader = permComm.ExecuteReader();
                    reader.Read();
                    Session["userPermissionID"] = reader["permissionID"];
                    reader.Close();
                }
            }
            catch (SqlException ex)
            {

                dbErrorMessage.Text = "<div class=\"errorMessage\">Error submitting your registration try again later, and/or change the entered data.<br /> [" + ex.Message + "]</div>";
            }
            finally
            {
                conn.Close();
                if (Session["validateFailed"] != null)
                {
                    Response.Redirect("Login.aspx");
                }
                else
                {
                    Session["validateFailed"] = null;
                    Response.Redirect("newDashboard.aspx");
                }
            }
        }
    }
}
