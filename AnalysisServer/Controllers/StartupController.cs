using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AnalysisServer.Controllers
{
    public class StartupController : ApiController
    {
        public HttpResponseMessage Get()
        {
            Console.WriteLine("Hello World (STARTUP)!");
            HttpResponseMessage result = Request.CreateResponse(HttpStatusCode.OK);
            return result;
        }
    }
}
