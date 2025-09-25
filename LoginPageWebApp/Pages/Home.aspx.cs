using System;
using System.Linq;
using System.Web.UI;

namespace LoginPageWebApp.Pages
{
    public partial class Home : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Redirect to login if no token
            if (Session["JwtToken"] == null)
            {
                Response.Redirect("~/Pages/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                string username = Session["Username"]?.ToString();
                lblWelcome.Text = $"Welcome, {username}!";
                lblUser.Text = username;

                var roles = Session["Roles"] as string[];
                if (roles != null && roles.Contains("Admin"))
                {
                    btnCreateUser.Visible = true;
                }
            }
        }

        protected void lnkSignOut_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Pages/Login.aspx");
        }
    }
}
