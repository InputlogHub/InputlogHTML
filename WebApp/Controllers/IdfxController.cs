using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using ICSharpCode.SharpZipLib.Zip;
using InputLog.Core.Util;
using WebApp.ActionFilters;

namespace WebApp.Controllers
{
    /// <summary>
    /// Controller for uploading/administering idfx files on server
    /// </summary>
    [Authorize(Roles = "Admin, Default")]
    [CurrentLink("Project Overview")]
    public class IdfxController : Controller
    {
        /// <summary>
        /// URL: /Idfx/Upload
        /// Shows idfx upload form.
        /// Only for use by Client (Lite GUI) atm.
        /// </summary>
        /// <returns></returns>
        public ActionResult Upload()
        {
            return View();
        }

        /// <summary>
        /// URL: /Idfx/UploadAjax
        /// Uploads an idfx file to the server through Ajax
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public bool UploadAjax(FormCollection collection)
        {
            try{
                // Sanitize unvalidated input.
                string fileExtension = ".idfx";
                string copytaskName = StringUtils.CoerceValidFileName(Request.Form["copyTask"]);
                string session = StringUtils.CoerceValidFileName(Request.Form["session"]);
                string participantName = StringUtils.CoerceValidFileName(Request.Form["participant"]);
                string formFile = Request.Form["file"];
                string fileContents = HttpUtility.UrlDecode(formFile);
                string timestamp = DateTime.Now.ToString("dd-MM-yyyy");

                // Sanitize file name, restrict max length
                var fileName = string.Join("_", participantName, copytaskName, timestamp);
                fileName = StringUtils.CoerceValidFileName(fileName);
                
                // Determine path to save file
                string[] pathElements;
                if (string.IsNullOrWhiteSpace(session))
                {
                    pathElements = new[] { copytaskName, timestamp };
                }
                else
                {
                    pathElements = new[] { copytaskName, timestamp, session };
                }
                string savePath = Path.Combine(pathElements);

                var saveDirFullPath = Path.Combine(ConfigurationManager.AppSettings["ServerIdfxDirectory"], savePath);

                //uploaded idfx files will temporarily be saved in the Website\CopyTask directory for easier retrieval by FTP
                //var saveDirFullPath = Path.Combine(@"C:\inetpub\wwwroot\WebSite\CopyTask\uploadedIdfx", savePath);

                //string saveDirFullPath = savePath;
                var fullPathNoExtension = Path.Combine(saveDirFullPath, fileName);
                fullPathNoExtension = fullPathNoExtension.Truncate(StringUtils.FILENAME_MAX_LENGTH - fileExtension.Length); 

                if (!Directory.Exists(saveDirFullPath))
                {
                    Directory.CreateDirectory(saveDirFullPath);
                }

                // Uniquify file name
                int count = 1;
                string fullPath = fullPathNoExtension + fileExtension;
                while (System.IO.File.Exists(fullPath))
                {
                    fullPath = $"{fullPathNoExtension}({count++}){fileExtension}";
                }
                System.IO.File.WriteAllText(fullPath, fileContents);

                // Backup procedure
                //string filenameWithExt = fileName + fileExtension;
                //this.SendIdfxByMail(filenameWithExt, session, fullPath);
                    
                return true;
            }
            catch (Exception)
            {
                
                return false;
            }
        }

        /// <summary>
        /// 21/02/2022 EVH This backup procedure is out-commented as requested by LVW.
        /// </summary>
        /// <param name="filenameWithExt"></param>
        /// <param name="session"></param>
        /// <param name="fullPath"></param>
        private void SendIdfxByMail(string filenameWithExt, string session, string fullPath)
        {
            MailMessage mail = new MailMessage {From = new MailAddress("inputlog.copy@uantwerpen.be")};
            mail.To.Add("inputlog.copy@uantwerpen.be");
            mail.Subject = session;
            mail.Body = "#copyTask";

            Attachment attachment = new Attachment(fullPath);
            mail.Attachments.Add(attachment);

            SmtpClient smtpServer = new SmtpClient("mail.uantwerpen.be")
            {
                Port = 587, Credentials = new NetworkCredential("inputlog.copy", "Npynpy702"), EnableSsl = true
            };

            smtpServer.Send(mail);
        }

        /// <summary>
        /// URL: /Idfx/Upload
        /// Uploads an idfx file to the server
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Upload(FormCollection collection, HttpPostedFileBase file)
        {
            try
            {
                string proj = Request.Form["project"].Trim();
                string document = Request.Form["document"].Trim();
                string comment = Request.Form["comment"].Trim();
                if (file.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(file.FileName);
                    var taskDir = Path.Combine(User.Identity.Name, proj, document);
                    var taskDirFullPath =
                        Path.Combine(ConfigurationManager.AppSettings["ServerIdfxDirectory"], taskDir);
                    if (fileName != null)
                    {
                        var fullPath = Path.Combine(taskDirFullPath, fileName);
                        var userDir = new DirectoryInfo(taskDirFullPath);
                        if (!userDir.Exists)
                        {
                            userDir.Create();
                        }
                        if (!string.IsNullOrWhiteSpace(comment))
                        {
                            System.IO.File.WriteAllText(Path.ChangeExtension(fullPath, null) + "_comment.txt", comment);
                        }
                        file.SaveAs(fullPath);
                    }
                }
                return RedirectToAction("Index", "Home");
            }
            catch (Exception e)
            {
                TempData["ErrorMsg"] = e.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// URL: /Idfx/Download
        /// Downloads all user's idfx files the server
        /// </summary>
        /// <returns></returns>
        public FilePathResult Download()
        {
            var taskDir = Path.Combine(ConfigurationManager.AppSettings["ServerIdfxDirectory"], User.Identity.Name);
            string filename = User.Identity.Name + "_idfxs.zip";
            string outfile = Path.Combine(taskDir, filename);
            var outZip = new FastZip();
            const string outFilter = @".*\.idfx";
            outZip.CreateZip(outfile, taskDir, true, outFilter);
            return File(outfile, "application/gzip", filename);
        }
    }
}