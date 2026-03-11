using System;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.IO;

namespace AnalysisServer
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void OpenTerminals()
        {
			// Open the designated backend processes (the database interface in pyhon and the two shiny instances)
			// This will be called the first time a request is sent to this server. 
			// Normally requests will be only sent to analyse files, however we can 'preload' this by sending a fake request using : https://docs.microsoft.com/en-us/iis/get-started/whats-new-in-iis-8/iis-80-application-initialization
			// This logic needs to be implemented for the IIS server only, so make sure that all paths are the correct paths on the UAntwerpen VM and not your local machine!
			
			var R_path = @" ""C:\Program Files\R\R-4.1.2\bin\Rscript.exe"" ";
			var python_path = @" ""C:\Program Files\Python310\python.exe"" ";

			var usersite_path = @"C:\inetpub\shiny\user_site.r";
			var expertsite_path = @"C:\inetpub\shiny\expert_site.r";
			var db_path = @"C:\inetpub\shiny\database\db_frontend.py";

			var startInfo = new System.Diagnostics.ProcessStartInfo
			{
				WorkingDirectory = @"C:\inetpub\shiny",
				FileName = "cmd.exe",
				Arguments = "/K" + R_path + usersite_path
			};

			var usersite_process = new System.Diagnostics.Process();
			usersite_process.StartInfo = startInfo;
			usersite_process.Start();


			startInfo.Arguments = "/K" + R_path + expertsite_path;
			var expertsite_process = new System.Diagnostics.Process();
			expertsite_process.StartInfo = startInfo;
			expertsite_process.Start();

			startInfo.Arguments = "/K" + db_path;
			var db_process = new System.Diagnostics.Process();
			db_process.StartInfo = startInfo;
			db_process.Start();
		}
        protected void Application_Start()
        {
			// OpenTerminals();
			// Console.WriteLine("Hello");
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
