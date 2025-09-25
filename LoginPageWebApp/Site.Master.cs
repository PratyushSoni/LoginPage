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
    }
}
