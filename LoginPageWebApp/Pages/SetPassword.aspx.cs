using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LoginPageWebApp.Pages
{
    public partial class SetPassword : System.Web.UI.Page
    {
        protected string Token => Request.QueryString["token"];
        protected string PendingEmail
        {
            get { return Token != null ? Session["SetPasswordToken_" + Token] as string : null; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Only allow access with a valid token
                if (string.IsNullOrEmpty(Token) || PendingEmail == null)
                {
                    lblMessage.Text = "Invalid or expired token. Please request a new set password link.";
                    DisableForm();
                }
            }
        }

        protected void btnSetPassword_Click(object sender, EventArgs e)
        {
            ValidationSummary1.Controls.Clear();
            ValidationSummary1.Visible = false;
            lblMessage.Text = string.Empty;

            if (string.IsNullOrEmpty(Token) || PendingEmail == null)
            {
                lblMessage.Text = "Invalid or expired token. Please request a new set password link.";
                DisableForm();
                return;
            }

            string password = txtPassword.Text;
            string confirmPassword = txtConfirmPassword.Text;

            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                AddValidationError("Both password fields are required.");
                txtPassword.Focus();
                return;
            }

            if (password != confirmPassword)
            {
                AddValidationError("Passwords do not match.");
                txtPassword.Focus();
                return;
            }

            // TODO: Add user to DB here. For demo, just clear the session and show success.
            // Example: UserService.CreateUser(PendingEmail, password);
            Session.Remove("SetPasswordToken_" + Token);
            lblMessage.ForeColor = System.Drawing.Color.Green;
            lblMessage.Text = "Password set successfully! Your account is now active.";
            DisableForm();
        }

        private void AddValidationError(string message)
        {
            ValidationSummary1.Visible = true;
            var cv = new CustomValidator
            {
                IsValid = false,
                ErrorMessage = message,
                Display = ValidatorDisplay.None,
                EnableClientScript = false
            };
            Page.Validators.Add(cv);
        }

        private void DisableForm()
        {
            txtPassword.Enabled = false;
            txtConfirmPassword.Enabled = false;
            btnSetPassword.Enabled = false;
        }
    }
}