using System;
using System.Web.UI;

namespace LoginPageWebApp.Controls
{
    public partial class ValidationSummaryReusable : UserControl
    {
        protected void Page_PreRender(object sender, EventArgs e)
        {
            // Ensure JS builds summary at render time
            Page.ClientScript.RegisterStartupScript(
                this.GetType(),
                "BuildValidationSummary",
                "setTimeout(buildValidationSummary, 50);",
                true
            );
        }

        public void ValidatePage()
        {
            Page.Validate();
        }

        public bool PageIsValid => Page.IsValid;
    }
}
