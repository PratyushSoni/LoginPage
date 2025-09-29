using System;
using System.Net.Http;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;

namespace LoginPageWebApp.Pages
{
    public partial class SetPassword : System.Web.UI.Page
    {
        protected string Token => Request.QueryString["token"];
        protected string PendingUsername
        {
            get
            {
                var val = Token != null ? Session["SetPasswordToken_" + Token] as string : null;
                return val != null && val.Contains("|") ? val.Split('|')[0] : null;
            }
        }
        protected string PendingEmail
        {
            get
            {
                var val = Token != null ? Session["SetPasswordToken_" + Token] as string : null;
                return val != null && val.Contains("|") ? val.Split('|')[1] : null;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Only allow access with a valid token
                if (string.IsNullOrEmpty(Token) || PendingEmail == null || PendingUsername == null)
                {
                    lblMessage.Text = "Invalid or expired token. Please request a new set password link.";
                    DisableForm();
                }
            }
        }

        protected async void btnSetPassword_Click(object sender, EventArgs e)
        {
            ValidationSummary1.Controls.Clear();
            ValidationSummary1.Visible = false;
            lblMessage.Text = string.Empty;

            if (string.IsNullOrEmpty(Token) || PendingEmail == null || PendingUsername == null)
            {
                lblMessage.Text = "Invalid or expired token. Please request a new set password link.";
                DisableForm();
                return;
            }

            string password = txtPassword.Text;
            string confirmPassword = txtConfirmPassword.Text;
            string email = PendingEmail;
            string username = PendingUsername;

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

            // Call API to create user
            try
            {
                using (var client = new HttpClient())
                {
                    var apiUrl = "https://localhost:7201/api/users/create";
                    var payload = new
                    {
                        Username = username,
                        Email = email,
                        Password = password,
                        Role = "User"
                    };
                    var json = JsonConvert.SerializeObject(payload);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    // You may need to add an admin JWT if required by the API
                    var adminJwt = Session["JwtToken"] as string;
                    if (!string.IsNullOrEmpty(adminJwt))
                        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", adminJwt);

                    var response = await client.PostAsync(apiUrl, content);
                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        AddValidationError($"Could not create user. Status: {response.StatusCode}. Details: {errorContent}");
                        return;
                    }
                }

                // Success: clear session and show success
                Session.Remove("SetPasswordToken_" + Token);
                lblMessage.ForeColor = System.Drawing.Color.Green;
                lblMessage.Text = "Password set successfully! Your account is now active.";
                DisableForm();
            }
            catch (Exception ex)
            {
                AddValidationError("Could not create user: " + ex.Message);
            }
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