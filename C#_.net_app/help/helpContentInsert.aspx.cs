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
using System.Data.SqlClient;

public partial class help_helpContentInsert : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            int helpPageID = Convert.ToInt32(Server.HtmlEncode(Request.QueryString["hp"]));
            bindInsertPage(helpPageID);
        }
    }

    protected void bindInsertPage(int _helpPageID)
    {
        int hpID = Convert.ToInt32(Server.HtmlEncode(Request.QueryString["hp"]));
        SqlConnection conn;
        SqlCommand comm;
        SqlDataReader reader;
        string connectionString = ConfigurationManager.ConnectionStrings["leoQuickStart"].ConnectionString;
        conn = new SqlConnection(connectionString);
        comm = new SqlCommand("SELECT helpPageName, content FROM helpPages WHERE helpPageID=@helpPageID", conn);
        comm.Parameters.Add("@helpPageID", SqlDbType.Int);
        comm.Parameters["@helpPageID"].Value = hpID;

        try
        {
            conn.Open();
            reader = comm.ExecuteReader();
            reader.Read();
            labelPageTitle.Text = "<h3>" + reader["helpPageName"].ToString() + "</h3>";
            labelPageContent.Text = reader["content"].ToString();
            reader.Close();
        }
        catch (SqlException ex)
        {
            messageBox box = new messageBox("error", "There has been an error loading help page content: [ " + ex.Message + " ]", labelInsertError);
        }
        finally
        {
            conn.Close();
        }

    }
}
