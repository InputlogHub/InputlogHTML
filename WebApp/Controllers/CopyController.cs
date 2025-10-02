using System;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Mvc;
using WebApp.Models;

namespace WebApp.Controllers
{
    [HandleError]
    public class CopyController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Choose(string code)
        {
            ViewData["UserCode"] = code;
            return View();
        }

        public ActionResult Try()
        {
            return View();
        }

        public ActionResult Manage(string creator, string task)
        {
            return View();
        }

        [ValidateInput(false)]
        [HttpPost]
        public string UploadCopyTask(FormCollection form)
        {
            try
            {
                string username = Request.Form["username"].Trim();
                string password = Request.Form["password"].Trim();

                if (new AccountMembershipService().ValidateUser(username, password))
                {
                    string fileName = Request.Form["filename"].Trim();
                    if (fileName == "")
                    {
                        fileName = "copyTask";
                    }
                    string extension = ".zip";

                    var formFile = Request.Form["file"];
                    byte[] contents = HttpUtility.UrlDecodeToBytes(formFile);

                    var taskDirFullPath = Path.Combine(ConfigurationManager.AppSettings["ServerTaskDirectory"], username);

                    var fullPath = Path.Combine(taskDirFullPath, fileName + extension);
                    var userDir = new DirectoryInfo(taskDirFullPath);
                    if (!userDir.Exists)
                    {
                        userDir.Create();
                    }

                    int count = 1;
                    string tempFileName = fileName;

                    while (System.IO.File.Exists(fullPath))
                    {
                        tempFileName = $"{fileName}_{count++}";
                        fullPath = Path.Combine(taskDirFullPath, tempFileName + extension);
                    }

                    System.IO.File.WriteAllBytes(fullPath, contents);

                    return tempFileName;
                }
                return "";
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}