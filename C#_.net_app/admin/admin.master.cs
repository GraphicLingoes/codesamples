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

public partial class admin_admin : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Bind username
        if (Session["userLoggedIn"] != null)
        {
            globalFunctions.BindmemberName(this.userInfoFirstName, this.userInfoLastName);
        }
        
        // Check if user has admin status
        if (Session["userPermissionID"] != null)
        {
            string chkPermission = Session["userPermissionID"].ToString();
            if (chkPermission != "1")
            {
                Response.Redirect("~/newDashboard.aspx");
            }
        }
        else
        {
            Response.Redirect("~/Logout.aspx");
        }

        // Hide for next release
        searchIB.Visible = false;
        searchLB.Visible = false;

    }

    public void signOutAdmin(object sender, EventArgs e)
    {
        ViewState.Clear();
        Response.Redirect("~/Logout.aspx");
    }

    public void clickVideos(object sender, EventArgs e)
    {
        Response.Redirect("~/admin/videos.aspx");
    }

    public void clickImpSteps(object sender, EventArgs e)
    {
        Response.Redirect("~/admin/impSteps.aspx");
    }

    public void clickMembers(object sender, EventArgs e)
    {
        Response.Redirect("~/admin/members.aspx");
    }

    public void clickRecVids(object sender, EventArgs e)
    {
        Response.Redirect("~/admin/recVidTemplates.aspx");
    }
    public void clickSearch(object sender, EventArgs e)
    {
        Response.Redirect("~/search.aspx");
    }
    public void clickRecVids(object sender, ImageClickEventArgs e)
    {
        Response.Redirect("~/admin/recVidTemplates.aspx");
    }
    protected void launchHelp(object sender, EventArgs e)
    {
        Response.Redirect("~/help/help.aspx");
    }
    protected void clickHelpAdmin(object sender, EventArgs e)
    {
        Response.Redirect("~/help/helpAdmin.aspx");
    }
}
