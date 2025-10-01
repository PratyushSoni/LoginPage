using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.Web.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LoginPageWebApp.Pages
{
    public partial class CreateUser : Page
    {
        private static readonly HttpClient httpClient = new HttpClient();

        protected void Page_Load(object sender, EventArgs e)
        {
            // Require login
            if (Session["JwtToken"] == null)
            {
                Response.Redirect("~/Pages/Login.aspx");
                return;
            }

            // Require Admin or Manager role
            var roles = Session["Roles"] as string[];
            if (roles == null || (!roles.Contains("Admin") && !roles.Contains("Manager")))
            {
                Response.Redirect("~/Pages/AccessDenied.aspx");
                return;
            }

            if (!IsPostBack)
            {
                ValidationSummary1.ClearErrors();
            }
        }

        protected async void btnCreateUser_Click(object sender, EventArgs e)
        {
            // Clear previous messages and validation errors
            ValidationSummary1.ClearErrors();
            lblMessage.Text = string.Empty;

            string username = txtUsername.Text.Trim();
            string email = txtEmail.Text.Trim();

            // Server-side required field validation
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email))
            {
                if (string.IsNullOrEmpty(username))
                    ValidationSummary1.AddError(GetLocalResource("UsernameRequired", "Username is required."));
                if (string.IsNullOrEmpty(email))
                    ValidationSummary1.AddError(GetLocalResource("EmailRequired", "Email is required."));

                return;
            }

            // Check if username/email already exist via API
            try
            {
                using (var client = new HttpClient())
                {
                    string jwt = Session["JwtToken"] as string;
                    if (!string.IsNullOrEmpty(jwt))
                        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);

                    var apiUrl = $"https://localhost:7201/api/users/check?username={Uri.EscapeDataString(username)}&email={Uri.EscapeDataString(email)}";
                    var response = await client.GetAsync(apiUrl);

                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        string errorMsg = $"Could not verify username/email uniqueness. Status: {response.StatusCode}.";
                        if (!string.IsNullOrWhiteSpace(errorContent))
                            errorMsg += $" Details: {errorContent}";
                        ValidationSummary1.AddError(errorMsg);
                        return;
                    }

                    var result = await response.Content.ReadAsStringAsync();
                    dynamic exists = JsonConvert.DeserializeObject(result);

                    bool usernameExists = exists.usernameExists == true;
                    bool emailExists = exists.emailExists == true;

                    if (usernameExists && emailExists)
                    {
                        ValidationSummary1.AddError(GetLocalResource("UsernameAndEmailExist", "Username and email already exist. Please choose a different username and email."));
                        return;
                    }
                    else if (usernameExists)
                    {
                        ValidationSummary1.AddError(GetLocalResource("UsernameExists", "User already exists. Please choose a different username."));
                        return;
                    }
                    else if (emailExists)
                    {
                        ValidationSummary1.AddError(GetLocalResource("EmailExists", "Email already exists. Please choose a different email."));
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                ValidationSummary1.AddError(GetLocalResource("UserCheckException", "Could not check user existence: ") + ex.Message);
                return;
            }

            // Generate a unique token for password setup
            string token = Guid.NewGuid().ToString();
            Session["SetPasswordToken_" + token] = $"{username}|{email}";

            string setPasswordUrl = $"{Request.Url.GetLeftPart(UriPartial.Authority)}/Pages/SetPassword.aspx?token={token}";

            // Compose email
            var mail = new MailMessage("no-reply@yourapp.com", email)
            {
                Subject = GetLocalResource("SetPasswordEmailSubject", "Set your password"),
                Body = string.Format(
                    GetLocalResource(
                        "SetPasswordEmailBody",
                        "Hello {0},\n\nPlease set your password by clicking the link below:\n{1}\n\nIf you did not request this, ignore this email."),
                    username, setPasswordUrl)
            };

            try
            {
                using (var smtp = new SmtpClient())
                {
                    smtp.Send(mail);
                }

                // Save email content locally as backup
                string txtDir = Server.MapPath("~/App_Data/");
                if (!Directory.Exists(txtDir)) Directory.CreateDirectory(txtDir);
                string txtPath = Path.Combine(txtDir, $"SetPassword_{Guid.NewGuid()}.txt");
                File.WriteAllText(txtPath, $"To: {email}\r\nSubject: {mail.Subject}\r\n\r\n{mail.Body}");

                lblMessage.ForeColor = System.Drawing.Color.Green;
                lblMessage.Text = $"User created and email sent! Email content saved at: {txtPath}";

                // Clear input fields
                txtUsername.Text = txtEmail.Text = string.Empty;
                ValidationSummary1.ClearErrors();
            }
            catch (Exception ex)
            {
                // Save email locally if sending fails
                string txtDir = Server.MapPath("~/App_Data/");
                if (!Directory.Exists(txtDir)) Directory.CreateDirectory(txtDir);
                string txtPath = Path.Combine(txtDir, $"SetPassword_{Guid.NewGuid()}.txt");
                try
                {
                    File.WriteAllText(txtPath, $"To: {email}\r\nSubject: {mail.Subject}\r\n\r\n{mail.Body}");
                    lblMessage.ForeColor = System.Drawing.Color.OrangeRed;
                    lblMessage.Text = $"Email could not be sent. Email content saved at: {txtPath}";
                }
                catch (Exception fileEx)
                {
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                    lblMessage.Text = "Email could not be sent and could not be saved as text. Error: " + fileEx.Message;
                }
            }
        }


        private string GetLocalResource(string key, string fallback)
        {
            try
            {
                return (string)GetGlobalResourceObject("SharedResource", key) ?? fallback;
            }
            catch
            {
                return fallback;
            }
        }
    }
}
