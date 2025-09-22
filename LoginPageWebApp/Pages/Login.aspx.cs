using System;
using System.Web.UI;

namespace LoginPageWebApp.Pages
{
    public partial class Login : Page
    {
        protected void Page_Load(object sender, EventArgs e) { }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            ValidationSummary1.ValidatePage();

            if (ValidationSummary1.PageIsValid)
            {
                // TODO: Implement login logic
            }
        }
    }
}
