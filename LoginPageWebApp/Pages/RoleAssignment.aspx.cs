using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;

namespace LoginPageWebApp.Pages
{
    public partial class RoleAssignment : Page
    {
        private static readonly string apiBaseUrl = "https://localhost:7201/api/users"; // Your API URL

        protected void Page_Load(object sender, EventArgs e)
        {
            // Check if user is Admin (assume JWT is stored in Session["Token"])
            var token = Session["Token"] as string;
            if (string.IsNullOrEmpty(token) || !IsAdmin(token))
            {
                Response.Redirect("~/Pages/AccessDenied.aspx");
                return;
            }

            if (!IsPostBack)
            {
                BindUsers(token);
            }
        }

        private bool IsAdmin(string jwtToken)
        {
            // Basic JWT decoding to check roles (or implement a proper JWT parser)
            // For simplicity, just checking if token contains "Admin"
            return jwtToken.Contains("Admin");
        }

        private async void BindUsers(string token)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await client.GetAsync(apiBaseUrl);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var users = JsonConvert.DeserializeObject<List<UserDto>>(json);

                    gvUsers.DataSource = users;
                    gvUsers.DataBind();

                    // Bind roles to dropdowns
                    var rolesResponse = await client.GetAsync($"{apiBaseUrl}/roles");
                    var rolesJson = await rolesResponse.Content.ReadAsStringAsync();
                    var roles = JsonConvert.DeserializeObject<List<string>>(rolesJson);

                    foreach (GridViewRow row in gvUsers.Rows)
                    {
                        var ddlRoles = (DropDownList)row.FindControl("ddlRoles");
                        ddlRoles.DataSource = roles;
                        ddlRoles.DataBind();
                    }
                }
                else
                {
                    lblMessage.Text = "Failed to load users.";
                }
            }
        }

        protected async void btnAssign_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            var userId = btn.CommandArgument;
            var row = (GridViewRow)btn.NamingContainer;
            var ddlRoles = (DropDownList)row.FindControl("ddlRoles");
            var roleName = ddlRoles.SelectedValue;

            var token = Session["Token"] as string;

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var payload = new { UserId = userId, Role = roleName };
                var content = new StringContent(JsonConvert.SerializeObject(payload), System.Text.Encoding.UTF8, "application/json");

                var response = await client.PostAsync($"{apiBaseUrl}/assignrole", content);

                if (response.IsSuccessStatusCode)
                {
                    lblMessage.Text = $"Role '{roleName}' assigned successfully.";
                    BindUsers(token); // Refresh grid
                }
                else
                {
                    lblMessage.Text = "Failed to assign role.";
                }
            }
        }
    }

    public class UserDto
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
    }
}
