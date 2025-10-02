using System.Web.Mvc;
using System.Web.Routing;

namespace WebApp
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit https://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        private static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

            // Get filename in URL for results. Important since otherwise the file would be named 20.zip or such
            routes.MapRoute(
                "Results", // Route name
                "Task/Results/{id}/{name}", // URL with parameters
                new { controller = "Task", action = "Results", id = UrlParameter.Optional, name = UrlParameter.Optional } // Parameter defaults
            );

            // Get filename in URL for results. Important since otherwise the file would be named 20.zip or such
            routes.MapRoute(
                "DownloadIdfx", // Route name
                "Idfx/Download", // URL with parameters
                new { controller = "Idfx", action = "Download" } // Parameter defaults
            );
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RegisterRoutes(RouteTable.Routes);
        }
    }
}