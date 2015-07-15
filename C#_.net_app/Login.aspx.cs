using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;

public partial class Login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //Set sign button as the default so it fires when users hits enter on keyboard.
        Page.Form.DefaultButton = submitButton.UniqueID;
        // Grab "Remember Me" info from userRememberMe Cookie
        if (!IsPostBack)
        {
            HttpCookie loginPracticeID;
            HttpCookie loginEmail;
            loginPracticeID = Request.Cookies["memberPracID"];
            loginEmail = Request.Cookies["memberEmail"];
            if (loginPracticeID == null && loginEmail == null)
            {
                practiceID.Text = "";
                emailLogin.Text = "";
            }
            else
            {
                practiceID.Text = loginPracticeID.Value;
                emailLogin.Text = loginEmail.Value;
            }

            // Check for validation complete
            if (Session["validateFailed"] != null)
            {
                loginResponse.Text = Session["validateFailed"].ToString(); 
            }
        }

    }
// Handle "Remember Me" check box.
    protected void LoginUser(Object sender, EventArgs e)
    {
        if (signInRemember.Checked)
        {
            string pracID = this.practiceID.Text.ToString();
            string userEmail = this.emailLogin.Text.ToString();
            HttpCookie memberPractID = new HttpCookie("memberPracID");
            HttpCookie memberEmail = new HttpCookie("memberEmail");
            memberPractID.Value = pracID;
            memberEmail.Value = userEmail;
            memberPractID.Expires = DateTime.Now.AddMonths(1);
            memberEmail.Expires = DateTime.Now.AddMonths(1);
            Response.Cookies.Add(memberPractID);
            Response.Cookies.Add(memberEmail);
        }


        
        System.Net.HttpWebRequest req = System.Net.HttpWebRequest.Create(ConfigurationManager.AppSettings["TrainingPortalLogin"].ToString()) as System.Net.HttpWebRequest;

        req.ContentType = "text/xml";
        req.KeepAlive = false;
        req.Method = "POST";
        req.Headers.Set("Practice", this.practiceID.Text);
        req.Headers.Set("PracticePassword", this.practicePassword.Text);
        req.Headers.Set("UserEmail", this.emailLogin.Text);
        req.Headers.Set("UserPassword", this.password.Text);

        System.IO.StreamWriter sw = new System.IO.StreamWriter(req.GetRequestStream());
        sw.Write("Login");
        sw.Close();


        System.Net.HttpWebResponse resp = req.GetResponse() as System.Net.HttpWebResponse;
        if (resp.Headers["Result"] == null)
        {
            loginResponse.Text = "<div class='errorMessageSearch'>There has been an error.</div>";
        }
        else
        {

            if (resp.Headers["Result"].ToString() != "Success")
            {
                loginResponse.Text = "<div class='errorMessageSearch'>" + resp.Headers["Result"].ToString() + "</div>";
            }
            else
            {
                bool permissionID = Convert.ToBoolean(resp.Headers["Admin"]);
                // Check to see if user is Admin in renaissance then set permission level.
                if (permissionID == true)
                {
                    Session["renUserPermissionID"] = Convert.ToInt32("5");
                }
                else
                {
                    Session["renUserPermissionID"] = Convert.ToInt32("3");
                }

                Session["renUserLoggedIn"] = resp.Headers["UserId"].ToString();
                Session["userInfoFirstName"] = resp.Headers["FirstName"].ToString();
                Session["userInfoLastName"] = resp.Headers["LastName"].ToString();
                Session["userPracticeName"] = resp.Headers["OrganizationName"].ToString();
                Session["userOrgID"] = resp.Headers["OrganizationId"].ToString();
                Session["email"] = emailLogin.Text.ToString();
                Session["lmdEditionName"] = resp.Headers["EditionName"].ToString();
                Session["lmdEditionID"] = resp.Headers["EditionId"].ToString();

                // Load user permission ID for training portal
                SqlConnection conn;
                SqlCommand comm;
                SqlDataReader reader;
                string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
                conn = new SqlConnection(connectionString);
                comm = new SqlCommand("SELECT userID, permissionID FROM [user] WHERE email='" + Session["email"] + "' AND renOrgID=" + Session["userOrgID"] + "", conn);

                conn.Open();
                reader = comm.ExecuteReader();
                if (reader.Read())
                {
                    Session["userPermissionID"] = reader["permissionID"];
                    Session["userLoggedIn"] = reader["userID"];
                    conn.Close();
                    reader.Close();
                    Response.Redirect("newDashboard.aspx");
                }
                else
                {
                    conn.Close();
                    reader.Close();
                    Session["userPermissionID"] = "3";
                    Response.Redirect("verify.aspx");
                }
            }
        }
        /* The code below is what is being sent back
        Response.Write("FirstName: " + resp.Headers["FirstName"].ToString() + "<BR>");
        Response.Write("LastName: " + resp.Headers["LastName"].ToString() + "<BR>");
        Response.Write("OrganizationID: " + resp.Headers["OrganizationId"].ToString() + "<BR>");
        Response.Write("UserId: " + resp.Headers["UserId"].ToString() + "<BR>");
        Response.Write("Result: " + resp.Headers["Result"].ToString() + "<BR>");
        Response.Write("OrganizationName: " + resp.Headers["OrganizationName"].ToString() + "<BR>");
        Response.Write("Admin: " + resp.Headers["Admin"].ToString() + "<BR>");
        Response.Write("Edition: " + resp.Headers["EditionName"].ToString() + "<BR>");
        Response.Write("EditionId: " + resp.Headers["EditionId"].ToString() + "<BR>");
         */

    }
}
