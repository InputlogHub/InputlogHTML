using System.Web.Mvc;
using WebApp.ActionFilters;
using WebApp.Models;
using System.Web.Helpers;
using System.Collections.Generic;
using System.Web.Security;
using WebApp.Util;
using System.Web.Profile;
using System.Configuration;

namespace WebApp.Controllers
{
    /// <summary>
    ///  Controller displaying static Admin pages
    /// </summary> 
    [Authorize(Roles = "Admin")]
    [CurrentLink("Administration")]
    public class AdminController : Controller
    {

        // **************************************
        // URL: /Admin
        // **************************************

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MailIndex()
        {
            MembershipUser admin = Membership.GetUser(User.Identity.Name);
            List<string> names = MailModel.GetAllNames();
            ViewBag.Names = names;
            if (admin != null) ViewBag.Adminmail = admin.Email;
            return View();
        }

        public ActionResult EditMail(string id)
        {
            MailModel mail = MailModel.FetchByName(id);
            if (mail != null)
            {
                ViewBag.ID = id;
                ViewBag.Content = mail.Content;
                return View();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditMail()
        {
            FormCollection form = new FormCollection(Request.Unvalidated().Form);
            string name = form["id"].Trim();
            string content = form["content"].Trim();
            MailModel.UpdateContent(name, content);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult SendTestMail()
        {
            string type = Request.Form["type"].Trim();
            string mail = Request.Form["mail"].Trim();
            MailModel mm = MailModel.FetchByName(type);
            MembershipUser user = Membership.GetUser(User.Identity.Name);
            ProfileBase profile = ProfileBase.Create(User.Identity.Name);
            string content = mm.Content;
            switch (type)
            {
                case "AccountRefusal":
                    {
                        content = content.Replace("@Username", profile.GetPropertyValue("Name").ToString());
                        break;
                    }
                case "MailWithCode":
                    {
                        string link = ConfigurationManager.AppSettings["SiteURL"] + "Account/LogOn/";
                        content = content.Replace("@Username", profile.GetPropertyValue("Name").ToString()).Replace("@LogonLink", link);
                        break;
                    }
                case "Registration":
                    {
                        if (user != null)
                        {
                            string validationURL = ConfigurationManager.AppSettings["SiteURL"]
                                                   + "Account/Validate?User=" + user.UserName + "&Code=" + "abc123";
                            content = content.Replace("@Username", user.UserName).Replace("@ValidationLink", validationURL);
                        }

                        break;
                    }
                case "ResetPassword":
                {
                    if (user != null)
                        content = content.Replace("@Username", user.UserName).Replace("@NewPass", "abc123");
                    break;
                }
                case "TaskNotification":
                    {
                        var baseURL = ConfigurationManager.AppSettings["SiteURL"];
                        string downloadLink = baseURL + "Task/Results/" + "1" + "/Results.zip";
                        if (user != null)
                            content = content.Replace("@Username", user.UserName)
                                .Replace("@DownloadLink", downloadLink);
                        break;
                    }
            }
            Mails.SendWrapped(mail, content, "Inputlog test mail");
            TempData["Message"] = "Test mail was sent to " + mail;
            return RedirectToAction("Index");
        }
    }
}
