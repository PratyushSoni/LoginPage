using System;
using System.IO;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using Newtonsoft.Json;

namespace LoginPageWebApp.Pages
{
    public partial class CreateUser : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Require login
            if (Session["JwtToken"] == null)
            {
                Response.Redirect("~/Pages/Login.aspx");
                return;
            }
            // Require Admin role
            var roles = Session["Roles"] as string[];
            if (roles == null || !roles.Contains("Admin"))
            {
                Response.Redirect("~/Pages/Home.aspx");
                return;
            }
            // Only clear errors on first load, not on every postback
            if (!IsPostBack)
            {
                ValidationSummary1.ClearErrors();
                lblMessage.Text = string.Empty;
            }
            // Always set the username and email textbox ClientIDs for JS focus
            ValidationSummary1.TargetControlClientID = txtUsername.ClientID + ',' + txtEmail.ClientID;
        }

        protected async void btnCreateUser_Click(object sender, EventArgs e)
        {
            // Clear previous validation errors
            ValidationSummary1.ClearErrors();
            ValidationSummary1.Visible = false;
            lblMessage.Text = string.Empty;

            string username = txtUsername.Text.Trim();
            string email = txtEmail.Text.Trim();

            // Basic validation
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email))
            {
                if (string.IsNullOrEmpty(username))
                    AddValidationError("Username is required.", txtUsername.ID);
                if (string.IsNullOrEmpty(email))
                    AddValidationError("Email is required.", txtEmail.ID);
                if (string.IsNullOrEmpty(username)) txtUsername.Focus();
                else if (string.IsNullOrEmpty(email)) txtEmail.Focus();
                return;
            }

            // Check if username and/or email already exist via API (with JWT token)
            try
            {
                using (var client = new HttpClient())
                {
                    var apiUrl = $"https://localhost:7201/api/users/check?username={Uri.EscapeDataString(username)}&email={Uri.EscapeDataString(email)}";
                    string jwt = Session["JwtToken"] as string;
                    if (!string.IsNullOrEmpty(jwt))
                        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);
                    var response = await client.GetAsync(apiUrl);
                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        string errorMsg = $"Could not verify username/email uniqueness. Status: {response.StatusCode}.";
                        if (!string.IsNullOrWhiteSpace(errorContent))
                            errorMsg += $" Details: {errorContent}";
                        AddValidationError(errorMsg);
                        return;
                    }
                    var result = await response.Content.ReadAsStringAsync();
                    // Assume API returns: { "usernameExists": true/false, "emailExists": true/false }
                    dynamic exists = JsonConvert.DeserializeObject(result);
                    bool usernameExists = exists.usernameExists == true;
                    bool emailExists = exists.emailExists == true;
                    if (usernameExists && emailExists)
                    {
                        AddValidationError("Username and email already exist. Please choose a different username and email.", txtUsername.ID);
                        txtUsername.Focus();
                        return;
                    }
                    else if (usernameExists)
                    {
                        AddValidationError("User already exists. Please choose a different username.", txtUsername.ID);
                        txtUsername.Focus();
                        return;
                    }
                    else if (emailExists)
                    {
                        AddValidationError("Email already exists. Please choose a different email.", txtEmail.ID);
                        txtEmail.Focus();
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                AddValidationError("Could not check user existence: " + ex.Message, txtUsername.ID);
                txtUsername.Focus();
                return;
            }

            // Generate a unique token
            string token = Guid.NewGuid().ToString();

            // Store the token and email in Session (for demo; use DB in production)
            Session["SetPasswordToken_" + token] = email;

            // Build the set password link
            string setPasswordUrl = $"{Request.Url.GetLeftPart(UriPartial.Authority)}/Pages/SetPassword.aspx?token={token}";

            // Compose the email
            var mail = new MailMessage("no-reply@yourapp.com", email)
            {
                Subject = "Set your password",
                Body = $"Hello {username},\n\nPlease set your password by clicking the link below:\n{setPasswordUrl}\n\nIf you did not request this, ignore this email."
            };

            try
            {
                using (var smtp = new SmtpClient())
                {
                    smtp.Send(mail);
                }
                lblMessage.ForeColor = System.Drawing.Color.Green;
                lblMessage.Text = "User created and email sent!";
                // Clear errors on success
                ValidationSummary1.ClearErrors();
            }
            catch (Exception ex)
            {
                // Save as .txt if email sending fails
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

        protected void AddValidationError(string message, string controlId = null)
        {
            ValidationSummary1.Visible = true;
            ValidationSummary1.AddError(message);
            if (!string.IsNullOrEmpty(controlId))
            {
                // Find the control and set focus
                Control control = FindControl(controlId);
                if (control != null)
                    ScriptManager.RegisterStartupScript(control, control.GetType(), "setFocus", "this.focus();", true);
            }
        }
    }
}