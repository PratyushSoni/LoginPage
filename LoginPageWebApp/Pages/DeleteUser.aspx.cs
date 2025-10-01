using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.UI;
using Newtonsoft.Json.Linq;

namespace LoginPageWebApp.Pages
{
    public partial class DeleteUser : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Require login and Admin role
            if (Session["JwtToken"] == null)
            {
                Response.Redirect("~/Pages/Login.aspx");
                return;
            }
            var roles = Session["Roles"] as string[];
            if (roles == null || Array.IndexOf(roles, "Admin") == -1)
            {
                Response.Redirect("~/Pages/AccessDenied.aspx");
                return;
            }
            if (!IsPostBack)
            {
                BindUsers();
            }
        }

        private async void BindUsers()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var jwt = Session["JwtToken"] as string;
                    if (!string.IsNullOrEmpty(jwt))
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
                    var apiUrl = "https://localhost:7201/api/users/all";
                    var response = await client.GetAsync(apiUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var users = JArray.Parse(json);
                        var userList = new List<UserRow>();
                        foreach (var u in users)
                        {
                            userList.Add(new UserRow
                            {
                                Username = (string)u["username"],
                                Email = (string)u["email"],
                                Roles = u["roles"].ToObject<List<string>>()
                            });
                        }
                        rptUsers.DataSource = userList;
                        rptUsers.DataBind();
                    }
                    else
                    {
                        lblMessage.Text = "Could not load users.";
                        lblMessage.ForeColor = System.Drawing.Color.Red;
                    }
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error: " + ex.Message;
                lblMessage.ForeColor = System.Drawing.Color.Red;
            }
        }

        protected async void DeleteUser_Command(object sender, System.Web.UI.WebControls.CommandEventArgs e)
        {
            string username = e.CommandArgument as string;
            if (string.IsNullOrEmpty(username)) return;
            try
            {
                using (var client = new HttpClient())
                {
                    var jwt = Session["JwtToken"] as string;
                    if (!string.IsNullOrEmpty(jwt))
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
                    var apiUrl = $"https://localhost:7201/api/users/delete?input={Uri.EscapeDataString(username)}";
                    var response = await client.DeleteAsync(apiUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        lblMessage.Text = $"User '{username}' deleted successfully.";
                        lblMessage.ForeColor = System.Drawing.Color.Green;
                        BindUsers();
                    }
                    else
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        lblMessage.Text = $"Error: {error}";
                        lblMessage.ForeColor = System.Drawing.Color.Red;
                    }
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Exception: " + ex.Message;
                lblMessage.ForeColor = System.Drawing.Color.Red;
            }
        }

        public class UserRow
        {
            public string Username { get; set; }
            public string Email { get; set; }
            public List<string> Roles { get; set; }
        }
    }
}