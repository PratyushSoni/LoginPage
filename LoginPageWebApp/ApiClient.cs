using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq; // Add this for JObject

public static class ApiClient
{
    private static readonly HttpClient client = new HttpClient();

    public static async Task<string> LoginAsync(string username, string password)
    {
        var payload = new { Username = username, Password = password };
        var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

        var response = await client.PostAsync("https://localhost:5001/api/auth/login", content);

        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync();
        // Use JObject for compatibility with .NET Framework
        var result = JObject.Parse(json);
        return result["token"]?.ToString();
    }
}