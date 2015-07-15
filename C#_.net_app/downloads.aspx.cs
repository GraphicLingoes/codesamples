using System;
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

public partial class downloads : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Check if user is logged in else redirect them to login page
        if (!globalFunctions.validateLogin())
        {
            Response.Redirect("Logout.aspx");
        }
    
        //Highlight all downloads link
        allDownloadsLB.ForeColor = System.Drawing.ColorTranslator.FromHtml("#9CC5C9");
    }

    // Handle all downloads click event
    protected void allDownloads(object sender, EventArgs e)
    {
        Response.Redirect("downloads.aspx");
    }
}
