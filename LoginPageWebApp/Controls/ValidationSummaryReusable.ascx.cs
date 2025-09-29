using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LoginPageWebApp.Controls
{
    public partial class ValidationSummaryReusable : UserControl
    {
        public string TargetControlClientID { get; set; } // For JS focus

        private List<string> ErrorMessages
        {
            get
            {
                if (ViewState["ValidationSummaryErrors"] == null)
                    ViewState["ValidationSummaryErrors"] = new List<string>();
                return (List<string>)ViewState["ValidationSummaryErrors"];
            }
            set { ViewState["ValidationSummaryErrors"] = value; }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            // Rebuild the summary from ViewState
            ulValidationSummary.Controls.Clear();
            foreach (var msg in ErrorMessages)
            {
                ulValidationSummary.Controls.Add(new LiteralControl($"<li>{msg}</li>"));
            }
            pnlValidationSummary.Visible = ErrorMessages.Count > 0;

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

        public void AddError(string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(errorMessage))
                return;
            var errors = ErrorMessages;
            errors.Add(errorMessage);
            ErrorMessages = errors;
            pnlValidationSummary.Visible = true;
        }

        // Add a server-side error using the hidden CustomValidator (no ControlToValidate)
        public void AddServerError(string errorMessage)
        {
            cvServerError.IsValid = false;
            cvServerError.ErrorMessage = errorMessage;
            cvServerError.ControlToValidate = string.Empty;
            pnlValidationSummary.Visible = true;
        }

        // Force the summary to rebuild on the client
        public void TriggerSummaryBuild()
        {
            Page.ClientScript.RegisterStartupScript(
                this.GetType(),
                "BuildValidationSummary",
                "setTimeout(buildValidationSummary, 50);",
                true
            );
        }

        // Optional: Clear all errors
        public void ClearErrors()
        {
            ErrorMessages = new List<string>();
            ulValidationSummary.Controls.Clear();
            pnlValidationSummary.Visible = false;
            // Do NOT reset cvServerError here; let AddServerError set it as needed
        }
    }
}
