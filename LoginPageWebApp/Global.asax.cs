using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using System.Globalization;
using System.Threading;

namespace LoginPageWebApp
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            // Set culture for language selection
            if (Context.Handler is IRequiresSessionState && Session != null && Session["CurrentCulture"] != null)
            {
                var culture = Session["CurrentCulture"].ToString();
                try
                {
                    Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);
                }
                catch { }
            }
            if (Context.User?.Identity != null && Context.User.Identity.IsAuthenticated)
            {
                var roles = (Session != null) ? Session["Roles"] as string[] : null;
                if (roles != null)
                {
                    var identity = new System.Security.Principal.GenericIdentity(Context.User.Identity.Name);
                    var principal = new System.Security.Principal.GenericPrincipal(identity, roles);
                    Context.User = principal;
                }
            }
        }
    }
}