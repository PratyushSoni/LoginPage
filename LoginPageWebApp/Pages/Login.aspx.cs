using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Security;
using System.Web.UI;

namespace LoginPageWebApp.Pages
{
    public partial class Login : System.Web.UI.Page
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
            if (!IsPostBack)
            {
                ValidationSummary1.ClearErrors();
            }
        }

        protected async void btnLogin_Click(object sender, EventArgs e)
        {
            ValidationSummary1.ClearErrors();

            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            // Validate required fields
            bool hasError = false;

            if (string.IsNullOrEmpty(username))
            {
                ValidationSummary1.AddError(GetLocalResource("UsernameRequired", "Username is required."), txtUsername.ClientID);
                hasError = true;
            }

            if (string.IsNullOrEmpty(password))
            {
                ValidationSummary1.AddError(GetLocalResource("PasswordRequired", "Password is required."), txtPassword.ClientID);
                hasError = true;
            }

            // Focus on the first invalid field
            if (hasError)
            {
                if (string.IsNullOrEmpty(username))
                    txtUsername.Focus();
                else
                    txtPassword.Focus();
                return;
            }

            // Call API for login
            var payload = new { Username = username, Password = password };
            var json = JsonConvert.SerializeObject(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await httpClient.PostAsync("https://localhost:7201/api/auth/login", content);

                if (!response.IsSuccessStatusCode)
                {
                    ValidationSummary1.AddError(GetLocalResource("SignInFailed", "Username or password is incorrect."), txtUsername.ClientID);
                    txtUsername.Focus();
                    return;
                }

                var responseBody = await response.Content.ReadAsStringAsync();
                var result = JObject.Parse(responseBody);

                string token = result["token"]?.ToString();
                string user = result["username"]?.ToString();
                string email = result["email"]?.ToString();
                var roles = result["roles"] != null ? result["roles"].ToObject<string[]>() : new string[0];

                // Store in session
                Session["JwtToken"] = token;
                Session["Username"] = user;
                Session["Email"] = email;
                Session["Roles"] = roles;

                FormsAuthentication.SetAuthCookie(user, false);

                Response.Redirect("~/Pages/Home.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                ValidationSummary1.AddError(GetLocalResource("SignInFailed", "Error: " + ex.Message), txtUsername.ClientID);
                txtUsername.Focus();
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
