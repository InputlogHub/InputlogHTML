using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;
using System.IO;
using System.Security.Cryptography;
using InputLog.Core.IO.Basic;
using InputLog.Core.IO.CSV;
using InputLog.Core.IO;
using InputLog.Core.Analyses.Copytask;
using CorpusManager;
using System.Globalization;

namespace AnalysisServer.Controllers
{
  
    public class UploadController : ApiController
    {
        // POST /upload
        public HttpResponseMessage Post()
        {
			Console.WriteLine("Hello World (UPLOAD)!");
            bool anonimize = false;

            HttpResponseMessage result = Request.CreateResponse(HttpStatusCode.OK);
            var httpRequest = HttpContext.Current.Request;

            CorpusWriter.Process(httpRequest.Form["user_path"] + "/upload/", httpRequest.Form["path"], anonimize);
            return result;
        }
    }
}
