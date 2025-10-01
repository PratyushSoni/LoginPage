using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LoginPageWebApp.Controls
{
    public partial class ValidationSummaryReusable : UserControl
    {
        private List<(string Message, string TargetControlID)> ErrorMessages
        {
            get
            {
                if (ViewState["ValidationSummaryErrors"] == null)
                    ViewState["ValidationSummaryErrors"] = new List<(string, string)>();
                return (List<(string, string)>)ViewState["ValidationSummaryErrors"];
            }
            set { ViewState["ValidationSummaryErrors"] = value; }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            ulValidationSummary.Controls.Clear();
            foreach (var err in ErrorMessages)
            {
                var li = new LiteralControl($"<li data-target='{err.TargetControlID}'>{err.Message}</li>");
                ulValidationSummary.Controls.Add(li);
            }
            pnlValidationSummary.Visible = ErrorMessages.Count > 0;
        }

        public void AddError(string message, string targetControlID = null)
        {
            if (string.IsNullOrWhiteSpace(message)) return;

            var errors = ErrorMessages;
            errors.Add((message, targetControlID));
            ErrorMessages = errors;
            pnlValidationSummary.Visible = true;
        }

        public void ClearErrors()
        {
            ErrorMessages = new List<(string, string)>();
            ulValidationSummary.Controls.Clear();
            pnlValidationSummary.Visible = false;
        }
    }
}
