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

public partial class Logout : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
       if (Session["userLoggedIn"] != null)
       {
           Session.Abandon();
           ViewState.Clear();
           Response.Redirect("Login.aspx");
       }
       else
       {
           ViewState.Clear();
           Response.Redirect("Login.aspx");
       }
    }

}
