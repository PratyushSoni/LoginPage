using System;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.Security;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.UI.WebControls;
using System.IdentityModel.Tokens.Jwt; // Only this is needed for JWT parsing
using System.Linq;

namespace LoginPageWebApp.Pages
{
    public partial class Login : Page
    {
        private static readonly HttpClient httpClient = new HttpClient();

        protected void Page_Load(object sender, EventArgs e)
        {
            // Silent login via token in URL
            string urlToken = Request.QueryString["token"];
            if (!string.IsNullOrEmpty(urlToken))
            {
                try
                {
                    // Decode JWT to get username (sub claim)
                    var handler = new JwtSecurityTokenHandler();
                    var jwt = handler.ReadJwtToken(urlToken);
                    var username = jwt.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
                    if (!string.IsNullOrEmpty(username))
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", urlToken);
                        var userInfoResponse = httpClient.GetAsync($"https://localhost:7201/api/users/{username}").Result;
                        if (userInfoResponse.IsSuccessStatusCode)
                        {
                            var userInfoJson = userInfoResponse.Content.ReadAsStringAsync().Result;
                            var userInfo = JObject.Parse(userInfoJson);
                            string user = userInfo["username"]?.ToString();
                            string email = userInfo["email"]?.ToString();
                            // Try to get roles from token first, fallback to empty
                            var roles = jwt.Claims.Where(c => c.Type == "role" || c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Select(c => c.Value).ToArray();
                            if (roles.Length == 0 && userInfo["roles"] != null)
                                roles = userInfo["roles"].ToObject<string[]>();
                            if (roles != null && Array.Exists(roles, r => r.Equals("Admin", StringComparison.OrdinalIgnoreCase)))
                            {
                                // Store in Session
                                Session["JwtToken"] = urlToken;
                                Session["Username"] = user;
                                Session["Email"] = email;
                                Session["Roles"] = roles;
                                FormsAuthentication.SetAuthCookie(user, false);
                                Response.Redirect("~/Pages/Home.aspx", false);
                                Context.ApplicationInstance.CompleteRequest();
                                return;
                            }
                        }
                    }
                }
                catch { /* Ignore and fall through to normal login */ }
            }

            // Already logged in? Redirect
            if (Session["JwtToken"] != null)
            {
                Response.Redirect("~/Pages/Home.aspx");
            }
        }

        protected async void btnLogin_Click(object sender, EventArgs e)
        {
            ValidationSummary1.ClearErrors();
            ValidationSummary1.ValidatePage();
            if (!ValidationSummary1.PageIsValid) return;

            var username = txtUsername.Text.Trim();
            var password = txtPassword.Text.Trim();

            var payload = new { Username = username, Password = password };
            var json = JsonConvert.SerializeObject(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await httpClient.PostAsync("https://localhost:7201/api/auth/login", content);

                if (!response.IsSuccessStatusCode)
                {
                    // Only add to built-in ValidationSummary
                    var cv = new CustomValidator
                    {
                        IsValid = false,
                        ErrorMessage = "Username or password is incorrect.",
                        Display = ValidatorDisplay.None,
                        EnableClientScript = false,
                        ControlToValidate = txtUsername.ID
                    };
                    Page.Validators.Add(cv);
                    txtUsername.Focus();
                    return;
                }

                var responseBody = await response.Content.ReadAsStringAsync();
                var result = JObject.Parse(responseBody);

                string token = result["token"]?.ToString();
                string user = result["username"]?.ToString();
                string email = result["email"]?.ToString();
                var roles = result["roles"] != null ? result["roles"].ToObject<string[]>() : new string[0];

                // Store in Session
                Session["JwtToken"] = token;
                Session["Username"] = user;
                Session["Email"] = email;
                Session["Roles"] = roles;

                // Mark user as authenticated for WebForms
                FormsAuthentication.SetAuthCookie(user, false);

                // Redirect safely
                Response.Redirect("~/Pages/Home.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                var cv = new CustomValidator
                {
                    IsValid = false,
                    ErrorMessage = "Error: " + ex.Message,
                    Display = ValidatorDisplay.None,
                    EnableClientScript = false,
                    ControlToValidate = txtUsername.ID
                };
                Page.Validators.Add(cv);
                txtUsername.Focus();
            }
        }
    }
}
