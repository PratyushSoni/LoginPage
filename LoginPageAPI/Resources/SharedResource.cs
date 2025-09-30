using System.Collections.Generic;

namespace LoginPageAPI.Resources
{
    public static class SharedResource
    {
        // Maps file names to their primary programming language
        public static readonly Dictionary<string, string> FileLanguageMap = new()
        {
            { "Controllers/UsersController.cs", "C#" },
            { "Program.cs", "C#" },
            { "Pages/CreateUser.aspx", "C#" },
            { "Pages/SetPassword.aspx", "C#" }
        };
    }
}
