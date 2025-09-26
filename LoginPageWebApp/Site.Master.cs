using System;
using System.Web;

namespace LoginPageWebApp
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ShowAdminLink();
            }

            // Optionally set the dropdown to the current language
            if (!IsPostBack && Session["CurrentCulture"] != null)
            {
                ddlLanguage.SelectedValue = Session["CurrentCulture"].ToString();
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
    }
}
