using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;

namespace LoginPageWebApp.Pages
{
    public partial class RoleAssignment : Page
    {
        private static readonly string apiBaseUrl = "https://localhost:7201/api/users";
        private static readonly string apiRolesUrl = "https://localhost:7201/api/roles";
        private List<string> allRoles = new List<string>();

        protected void Page_Load(object sender, EventArgs e)
        {
            // Require login first
            var token = Session["JwtToken"] as string;
            if (string.IsNullOrEmpty(token))
            {
                Response.Redirect("~/Pages/Login.aspx");
                return;
            }
            // Then require Admin role
            var roles = Session["Roles"] as string[];
            if (roles == null || !(roles.Contains("Admin")))
            {
                Response.Redirect("~/Pages/AccessDenied.aspx");
                return;
            }

            if (!IsPostBack)
            {
                // Set the localized title
                litRoleAssignmentTitle.Text = GetGlobalResourceObject("SharedResource", "RoleAssignmentTitle")?.ToString() ?? "Role Assignment";
                RegisterAsyncTask(new PageAsyncTask(async () =>
                {
                    await BindUsersAndRoles(token);
                }));
            }
        }

        private async Task BindUsersAndRoles(string token)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                // Fetch all roles from API
                var rolesResponse = await client.GetAsync(apiRolesUrl);
                if (rolesResponse.IsSuccessStatusCode)
                {
                    var rolesJson = await rolesResponse.Content.ReadAsStringAsync();
                    allRoles = JsonConvert.DeserializeObject<List<string>>(rolesJson);
                }
                else
                {
                    allRoles = new List<string> { "F" };
                }
                // Fetch all users
                var response = await client.GetAsync(apiBaseUrl + "/all");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var users = JsonConvert.DeserializeObject<List<UserWithRolesDto>>(json);

                    gvUsers.DataSource = users;
                    gvUsers.DataBind();
                }
                else
                {
                    lblMessage.Text = "Failed to load users.";
                }
            }
        }

        protected void gvUsers_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var user = (UserWithRolesDto)e.Row.DataItem;
                var ddlRoles = (DropDownList)e.Row.FindControl("ddlRoles");
                ddlRoles.DataSource = allRoles;
                ddlRoles.DataBind();
                if (user.Roles != null && user.Roles.Count > 0)
                {
                    // Prefer to select Admin, then Manager, then User if present
                    var preferred = user.Roles.FirstOrDefault(r => allRoles.Contains(r));
                    if (!string.IsNullOrEmpty(preferred))
                        ddlRoles.SelectedValue = preferred;
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

            var token = Session["JwtToken"] as string;

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var payload = new { UserId = userId, Role = roleName };
                var content = new StringContent(JsonConvert.SerializeObject(payload), System.Text.Encoding.UTF8, "application/json");

                var response = await client.PostAsync(apiBaseUrl + "/assignrole", content);

                if (response.IsSuccessStatusCode)
                {
                    lblMessage.Text = $"Role '{roleName}' assigned successfully.";
                    RegisterAsyncTask(new PageAsyncTask(async () =>
                    {
                        await BindUsersAndRoles(token);
                    }));
                }
                else
                {
                    lblMessage.Text = "Failed to assign role.";
                }
            }
        }
    }

    public class UserWithRolesDto
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
    }
}
