using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;

public partial class portal : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Bind username
        if (Session["userLoggedIn"] != null)
        {
            globalFunctions.BindmemberName(this.userInfoFirstName, this.userInfoLastName);
        }
        
        // Check if user has admin status
        string chkPermission = Session["userPermissionID"].ToString();

        if (chkPermission != "1")
        {
            HtmlGenericControl menuDiv = (HtmlGenericControl)this.FindControl("adminMenu");
            menuDiv.Visible = false;
        }
        else
        {
            HtmlGenericControl menuDiv = (HtmlGenericControl)this.FindControl("adminMenu");
            menuDiv.Visible = true;
        }
    }

    public void signOut(object sender, EventArgs e)
    {
        ViewState.Clear();
        Response.Redirect("Logout.aspx");
    }

}
