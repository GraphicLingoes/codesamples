using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

/// <summary>
/// Summary description for messageBox
/// </summary>
public class messageBox
{
    private string boxType;
    private string boxMessageText;
    private string rootUrl = "http://trainingmike.leonardomd.com/";
	public messageBox(string boxType, string boxMessageText, Label messageLabel)
	{
        this.boxType = boxType;
        this.boxMessageText = boxMessageText;

        switch (boxType)
        {
            case "error":
                error(boxMessageText, messageLabel);
                break;
            case "notice":
                notice(boxMessageText, messageLabel);
                break;
            case "success":
                success(boxMessageText, messageLabel);
                break;

        }
	}

    private void error(string boxMessageText, Label messageLabel)
    {
        messageLabel.Visible = true;
        messageLabel.Text = "<div class=\"errorMessageSearch\"><div class=\"closeBox\"><a href=\"#\" name=\"closeErrorBox\"><img src=\"" + rootUrl.ToString() + "Images/closeMessageBox.png\" title='Close Box' /></a></div>" + boxMessageText + "</div>";
    }

    private void notice(string boxMessageText, Label messageLabel)
    {
        messageLabel.Visible = true;
        messageLabel.Text = "<div class=\"searchNotice\"><div class=\"closeBox\"><a href=\"#\" name=\"closeNoticeBox\"><img src=\"" + rootUrl.ToString() + "Images/closeMessageBox.png\" title='Close Box' /></a></div>" + boxMessageText + "</div>";
    }

    private void success(string boxMessageText, Label messageLabel)
    {
        messageLabel.Visible = true;
        messageLabel.Text = "<div class=\"success\"><div class=\"closeBox\"><a href=\"#\" name=\"closeSuccessBox\"><img src=\"" + rootUrl.ToString() + "Images/closeMessageBox.png\" title='Close Box' /></a></div>" + boxMessageText + "</div>";
    }
}
