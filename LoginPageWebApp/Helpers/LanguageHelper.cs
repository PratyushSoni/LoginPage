using System;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace LoginPageWebApp.Helpers
{
    public static class LanguageHelper
    {
        private static readonly string xmlPath = HttpContext.Current.Server.MapPath("~/App_Data/Languages.xml");

        public static string GetText(string key)
        {
            string lang = "en"; // default
            if (HttpContext.Current.Request.Cookies["CurrentLanguage"] != null)
                lang = HttpContext.Current.Request.Cookies["CurrentLanguage"].Value;

            XDocument doc = XDocument.Load(xmlPath);
            var element = doc.Descendants("Text")
                             .FirstOrDefault(x => x.Attribute("Key").Value == key);

            if (element != null)
            {
                var value = element.Element(lang);
                if (value != null)
                    return value.Value;
            }

            return key; // fallback if key not found
        }
    }
}
