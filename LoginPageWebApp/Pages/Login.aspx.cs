using System;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.Security;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LoginPageWebApp.Pages
{
    public partial class Login : Page
    {
        private static readonly HttpClient httpClient = new HttpClient();

        protected void Page_Load(object sender, EventArgs e)
        {
            // Already logged in? Redirect
            if (Session["JwtToken"] != null)
            {
                Response.Redirect("~/Pages/Home.aspx");
            }
        }

        protected async void btnLogin_Click(object sender, EventArgs e)
        {
            ValidationSummary1.ValidatePage();
            if (!ValidationSummary1.PageIsValid) return;

            var username = txtUsername.Text.Trim();
            var password = txtPassword.Text.Trim();

            var payload = new { Username = username, Password = password };
            var json = JsonConvert.SerializeObject(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await httpClient.PostAsync("https://localhost:7201/api/users/login", content);

                if (!response.IsSuccessStatusCode)
                {
                    ValidationSummary1.AddError("Invalid username or password.");
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
                ValidationSummary1.AddError("Error: " + ex.Message);
            }
        }
    }
}
