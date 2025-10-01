using System;
using System.Web;
using System.IdentityModel.Tokens.Jwt;

namespace LoginPageWebApp
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // JWT expiration check
            var token = Session["JwtToken"] as string;
            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jwt = handler.ReadJwtToken(token);
                    var exp = jwt.Payload.Exp;
                    if (exp.HasValue)
                    {
                        var expDate = DateTimeOffset.FromUnixTimeSeconds(exp.Value).UtcDateTime;
                        if (DateTime.UtcNow > expDate)
                        {
                            // Token expired: clear session and redirect
                            Session.Clear();
                            Response.Redirect("~/Pages/Login.aspx?expired=1");
                            return;
                        }
                    }
                }
                catch { /* Ignore and continue if token is invalid */ }
            }

            if (!IsPostBack)
            {
                ShowAdminLink();
            }

            // Optionally set the dropdown to the current language
            if (!IsPostBack && Session["CurrentCulture"] != null)
            {
                ddlLanguage.SelectedValue = Session["CurrentCulture"].ToString();
            }

            // Hide or disable Home link on Login page
            string currentPath = Request.AppRelativeCurrentExecutionFilePath.ToLowerInvariant();
            if (currentPath.Contains("/pages/login.aspx"))
            {
                var homeLink = FindControlRecursive(this, "navbarHomeLink") as System.Web.UI.HtmlControls.HtmlAnchor;
                if (homeLink != null)
                {
                    homeLink.HRef = "#";
                    homeLink.Attributes["style"] = "pointer-events:none;opacity:0.5;";
                }
            }
        }

        private void ShowAdminLink()
        {
            // Default: hide
            hlRoleAssignment.Visible = false;

            // Check session roles set during login
            var roles = Session["Roles"] as string[];
            if (roles != null && Array.Exists(roles, r => r.Equals("Admin", StringComparison.OrdinalIgnoreCase)))
            {
                hlRoleAssignment.Visible = true;
            }
        }

        protected void ddlLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedLang = ddlLanguage.SelectedValue;
            Session["CurrentCulture"] = selectedLang;
            Response.Redirect(Request.Url.AbsoluteUri);
        }

        // Helper to find control recursively
        private System.Web.UI.Control FindControlRecursive(System.Web.UI.Control root, string id)
        {
            if (root.ID == id) return root;
            foreach (System.Web.UI.Control c in root.Controls)
            {
                var t = FindControlRecursive(c, id);
                if (t != null) return t;
            }
            return null;
        }
    }
}
