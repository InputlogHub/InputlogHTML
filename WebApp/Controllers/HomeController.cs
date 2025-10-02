using System.Web.Mvc;
using System.Web.Profile;
using System.Web.Security;
using WebApp.ActionFilters;
using WebApp.Util;

namespace WebApp.Controllers
{
    /// <summary>
    ///  Controller for static pages
    /// </summary> 
    [HandleError]
    public class HomeController : Controller
    {
        // **************************************
        // URL: /Index
        // **************************************

        [CurrentLink("Description")]
        public ActionResult Index()
        {
            return View();
        }

        // **************************************
        // URL: /About
        // **************************************

        [CurrentLink("Team")]
        public ActionResult About()
        {
            return View();
        }

        public ActionResult Error()
        {
            return View();
        }

        // **************************************
        // URL: /User validation messages
        // **************************************
        public ActionResult Thanks()
        {
            return View();
        }

        public ActionResult Unsubscribe()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult Unsubscribe(string email)
        {
            ViewData["EmailAddress"] = email;

            foreach (string adminName in Roles.GetUsersInRole("Admin"))
            {
                MembershipUser admin = Membership.GetUser(adminName);
                ProfileBase adminProfile = ProfileBase.Create(adminName);
                bool notifications = bool.Parse(adminProfile.GetPropertyValue("EmailNotifications").ToString());
                if (notifications)
                {
                    if (admin != null)
                        Mails.SendWrapped(admin.Email, "User with email address " + email
                            + " has requested to be unsubscribed from the mailing list.",
                            "InputLog mailing list: request to unsubscribe " + email);
                }

            }
            TempData["Message"] = "Your request to unsubscribe " + email
                                    + " from the mailing list was successfully submitted. Your email address wil be removed shortly.";
            return View();
        }
    }
}