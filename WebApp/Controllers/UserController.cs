using System;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.Profile;
using WebApp.Models;
using WebApp.ActionFilters;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;

namespace WebApp.Controllers
{
    /// <summary>
    ///  Controller handling Showing, Exporting, Editing and Disabling users 
    /// </summary> 
    [Authorize(Roles = "Admin, Default")]
    [CurrentLink("My Account")]
    public class UserController : Controller
    {
        // **************************************
        // URL: /User
        // **************************************

        [Authorize(Roles="Admin")]
        public ActionResult Index()
        {
            return View(new UserIndexViewModel(Membership.GetAllUsers().Cast<MembershipUser>().
                Where(u => !Roles.IsUserInRole(u.UserName, "Disabled")), false));
        }

        // **************************************
        // URL: /User/1
        // **************************************

        [Authorize(Roles = "Admin")]
        public ActionResult IndexDisabled()
        {
            return View(new UserIndexViewModel(Membership.GetAllUsers().Cast<MembershipUser>(), true));
        }

        // **************************************
        // URL: /User/Export
        // **************************************
        [Authorize(Roles = "Admin")]
        public ActionResult Export()
        {
            var users = Membership.GetAllUsers().Cast<MembershipUser>();
            var sb = new StringBuilder();
            sb.AppendLine("UserName, Email, EmailNotifications, Name, Affiliation, Address, Postal, City, Country, Research");
            foreach (MembershipUser user in users)
            {
                ProfileBase profile = ProfileBase.Create(user.UserName);
                var userName = user.UserName.Replace(",",".");
                var email = user.Email.Replace(",",".");
                var emailNotifications = bool.Parse(profile.GetPropertyValue("EmailNotifications").ToString());
                var name = profile.GetPropertyValue("Name").ToString().Replace(",", " ");
                var affiliation = profile.GetPropertyValue("Affiliation").ToString().Replace(",", " ");
                var address = profile.GetPropertyValue("Address").ToString().Replace(",", " ");
                var postal = profile.GetPropertyValue("Postal").ToString().Replace(","," ");
                var city = profile.GetPropertyValue("City").ToString().Replace(",", " ");
                var country = profile.GetPropertyValue("Country").ToString().Replace(",", " ");
                var research = profile.GetPropertyValue("Research").ToString().Replace(Environment.NewLine, "__").
                    Replace(",",";");
                var userString = $"{userName},{email},{emailNotifications},{name},{affiliation}," +
                                 $"{address},{postal},{city},{country},{research},{Environment.NewLine}";
                sb.Append(userString);
            }

            var str = sb.ToString();
            byte[] bytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            
            return File(bytes, System.Net.Mime.MediaTypeNames.Application.Octet, "users.csv");
        }

        // **************************************
        // URL: /User/Import
        // **************************************
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Import(HttpPostedFileBase file)
        {
            Stream inputStream;

            if (file != null && file.ContentLength > 0)
            {
                inputStream = file.InputStream;
            }
            else //load file from WebApp\Legacy directory on server
            {
                //var fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(Server.MapPath("~/Legacy/"), "Contacts Inputlog.xml");
                inputStream = new FileStream(path, FileMode.Open);
            }

            var success = Util.LegacyUserImporter.Import(inputStream);

            if (success)
                return View("ImportSuccess");
            return RedirectToAction("Index", "User");
        }

        // **************************************
        // URL: /User/Details/ID
        // **************************************

        public ActionResult Details(string id)
        {
            if (User.IsInRole("Admin") || User.Identity.Name.Equals(id))
            {
                ProfileBase profile = ProfileBase.Create(id);
                return View(new UserDetailsViewModel(Membership.GetUser(id), profile, TaskModel.FetchByUserID(id, 10)));
            }
            return RedirectToAction("Index", "Home");
        }

        // **************************************
        // URL: /User/Edit/ID
        // **************************************

        public ActionResult Edit(string id)
        {
            if (User.IsInRole("Admin") || User.Identity.Name.Equals(id))
            {
                ProfileBase profile = ProfileBase.Create(id);
                return View(new UserDetailsViewModel(Membership.GetUser(id),profile));
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult Edit(string id, UserDetailsViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (User.IsInRole("Admin") || User.Identity.Name.Equals(id))
                {
                    string mailUser = Membership.GetUserNameByEmail(model.Email);
                    if (mailUser != null && !mailUser.Equals(id))
                    {
                        ModelState.AddModelError("Email", "This email-address already exists in our database");
                        return View(model);
                    }

                    if (mailUser == null)
                    {
                        MembershipUser editedUser = Membership.GetUser(id);
                        if (editedUser != null)
                        {
                            editedUser.Email = model.Email;
                            Membership.UpdateUser(editedUser);
                        }
                    }

                    ProfileBase thisProfile = ProfileBase.Create(id);
                    thisProfile.SetPropertyValue("EmailNotifications", model.EmailNotifications);
                    thisProfile.SetPropertyValue("Name", model.Name);
                    thisProfile.SetPropertyValue("Affiliation", model.Affiliation);
                    thisProfile.SetPropertyValue("Address", model.Address);
                    thisProfile.SetPropertyValue("Postal", model.Postal);
                    thisProfile.SetPropertyValue("City", model.City);
                    thisProfile.SetPropertyValue("Country", model.Country);
                    thisProfile.SetPropertyValue("Research", model.Research);
                    thisProfile.Save();
                    return RedirectToAction("Details", new { ID = id });
                }
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        // **************************************
        // URL: /User/Delete/ID
        // **************************************

        public ActionResult Delete(string id)
        {
            if (User.IsInRole("Admin") || User.Identity.Name.Equals(id))
            {
                ViewData["ID"] = id;
                return View();
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult Delete(string id, FormCollection collection)
        {
            try
            {
                if (User.IsInRole("Admin") || User.Identity.Name.Equals(id))
                {
                    Roles.AddUserToRole(id, "Disabled");
                    Roles.RemoveUserFromRoles(id, new[]{"Default", "Admin"});
                    FormsAuthentication.SignOut();
                    if (User.IsInRole("Admin"))
                    {
                        return RedirectToAction("Details", new { ID = id });
                    }
                    return RedirectToAction("Index", "Home");
                }
                return RedirectToAction("Index", "Home");
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // **************************************
        // URL: /User/MakeAdmin/ID
        // **************************************

        [Authorize(Roles = "Admin")]
        public ActionResult MakeAdmin(string id)
        {
            ViewData["ID"] = id;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult MakeAdmin(string id, FormCollection collection)
        {
            try
            {
                Roles.AddUserToRole(id, "Admin");
                return RedirectToAction("Details", new { ID = id });
            }
            catch
            {
                return View();
            }
        }
    }
}
