using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Profile;
using System.Web.Routing;
using System.Web.Security;
using Postal;
using WebApp.ActionFilters;
using WebApp.Models;
using WebApp.Util;
using InputLog.Core.Util.Server;
using System.Collections.Generic;
using System.IO;
using InputLog.Core.Util;

namespace WebApp.Controllers
{
    /// <summary>
    ///  Controller handling Account actions: registration, logging in and changing/resetting passwords
    /// </summary> 
    [HandleError]
    public class AccountController : Controller
    {

        public IFormsAuthenticationService FormsService { get; set; }
        public IMembershipService MembershipService { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {
            if (FormsService == null) { FormsService = new FormsAuthenticationService(); }
            if (MembershipService == null) { MembershipService = new AccountMembershipService(); }

            base.Initialize(requestContext);
        }

        // **************************************
        // URL: /Account/LogOn
        // **************************************
        [CurrentLink("Login")]
        public ActionResult LogOn()
        {
            return View();
        }

        /// <summary>
        /// URL: /Account/Logon
        /// Logs on the user
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CurrentLink("Login")]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (Roles.IsUserInRole(model.UserName, "Disabled"))
                {
                    ModelState.AddModelError("", "This account is currently disabled. " +
                                                 "Either you haven't verified your email address, " +
                                                 "or your account has not yet been approved by an administrator");
                }
                else if (MembershipService.ValidateUser(model.UserName, model.Password))
                {
                    FormsService.SignIn(model.UserName, model.RememberMe);
                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        /// <summary>
        /// URL: /Account/LogonAjax
        /// Logs on the user through Ajax
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult LogOnAjax(FormCollection collection)
        {
            string username = Request.Form["username"].Trim();
            string password = Request.Form["password"].Trim();

            LoginStatus status = new LoginStatus();

            if (MembershipService.ValidateUser(username, password))
            {
                status.Success = true;
                status.Message = "Login attempt successful";

                FormsService.SignIn(username, true);
            }
            else
            {
                status.Success = false;
                status.Message = "Login attempt failed";
            }

            return Json(status);
        }

        public class LoginStatus
        {
            public bool Success { get; set; }
            public string Message { get; set; }
        }

        // **************************************
        // URL: /Account/LogOff
        // **************************************

        public ActionResult LogOff()
        {
            FormsService.SignOut();

            return RedirectToAction("Index", "Home");
        }

        // **************************************
        // URL: /Account/Register
        // **************************************
        [CurrentLink("Register")]
        public ActionResult Register()
        {
            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
            return View();
        }

        [HttpPost]
        [CurrentLink("Register")]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus = MembershipService.CreateUser(model.UserName, model.Password, model.Email);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    Roles.AddUserToRole(model.UserName, "Disabled");
                    string validationCode = Guid.NewGuid().ToString();
                    ProfileBase thisProfile = ProfileBase.Create(model.UserName);
                    thisProfile.SetPropertyValue("ValidationCode", validationCode);
                    thisProfile.SetPropertyValue("AdminValidation", false);
                    thisProfile.SetPropertyValue("EmailNotifications", true);
                    thisProfile.SetPropertyValue("Name", model.Name);
                    thisProfile.SetPropertyValue("Affiliation", model.Affiliation);
                    thisProfile.SetPropertyValue("Address", model.Address);
                    thisProfile.SetPropertyValue("Postal", model.Postal);
                    thisProfile.SetPropertyValue("City", model.City);
                    thisProfile.SetPropertyValue("Country", model.Country);
                    thisProfile.SetPropertyValue("Research", model.Research);
                    thisProfile.Save();
                    MailModel mm = MailModel.FetchByName("Registration");
                    string validationURL = ConfigurationManager.AppSettings["SiteURL"]
                        + "Account/Validate?User=" + model.UserName + "&Code=" + validationCode;
                    string content = mm.Content.Replace("@Username", model.UserName).Replace("@ValidationLink", validationURL);

                    if (model.IsLite)
                    {
                        List<Pair<string, string>> attachments = new List<Pair<string, string>>();
                        AccountSettings accSet = new AccountSettings(model.UserName, model.Password, true);
                        string path = Path.GetTempFileName();
                        accSet.Serialize(path);
                        attachments.Add(new Pair<string, string>("AccountConfig.dat", path));
                        Mails.SendWrapped(model.Email, content, "Inputlog registration", attachments);
                        System.IO.File.Delete(path);
                    }
                    else
                    {
                        Mails.SendWrapped(model.Email, content, "Inputlog registration");
                    }
                    return View("RegisterSuccess");
                }
                ModelState.AddModelError("", AccountValidation.ErrorCodeToString(createStatus));
            }

            // If we got this far, something failed, redisplay form
            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
            return View(model);
        }

        // **************************************
        // URL: /Account/ChangePassword
        // **************************************

        [Authorize]
        [CurrentLink("My Account")]
        public ActionResult ChangePassword()
        {
            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
            return View();
        }

        [Authorize]
        [HttpPost]
        [CurrentLink("My Account")]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                if (MembershipService.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword))
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }

                ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
            }

            // If we got this far, something failed, redisplay form
            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
            return View(model);
        }

        // **************************************
        // URL: /Account/ChangePasswordSuccess
        // **************************************
        [CurrentLink("My Account")]
        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        // **************************************
        // URL: /Account/ForgotPassword
        // **************************************
        [CurrentLink("Login")]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        // **************************************
        // URL: /Account/ResetPassword
        // **************************************

        [HttpPost]
        [CurrentLink("Login")]
        public ActionResult ResetPassword(ResetPasswordModel Model)
        {
            if (ModelState.IsValid)
            {
                string mail = Model.Email;
                string userName = Model.UserName;
                string newPass = "";
                if (!string.IsNullOrEmpty(userName))
                {
                    MembershipUser thisUser = Membership.GetUser(userName);
                    if (thisUser == null)
                    {
                        ModelState.AddModelError("", "No user found with username " + userName);
                        return View("ForgotPassword", Model);
                    }
                    mail = thisUser.Email;
                    newPass = Membership.Provider.ResetPassword(userName, "");
                }
                else if (!string.IsNullOrEmpty(mail))
                {
                    userName = Membership.GetUserNameByEmail(mail);
                    if (userName == null)
                    {
                        ModelState.AddModelError("", "No user found with e-mail " + mail);
                        return View("ForgotPassword", Model);
                    }
                    MembershipUser thisUser = Membership.GetUser(userName);
                    newPass = Membership.Provider.ResetPassword(userName, "");
                }
                else
                {
                    ModelState.AddModelError("", "User name or password is required");
                    return View("ForgotPassword", Model);
                }
                MailModel mm = MailModel.FetchByName("ResetPassword");
                var baseURL = ConfigurationManager.AppSettings["SiteURL"];
                string content = mm.Content.Replace("@Username", userName).Replace("@NewPass", newPass);
                Mails.SendWrapped(mail, content, "Inputlog password reset");
                TempData["Message"] = "A new password has been sent to " + mail;
                return RedirectToAction("LogOn");
            }
            return View("ForgotPassword", Model);
        }

        // **************************************
        // URL: /Account/AdminValidate/User
        // **************************************
        [CurrentLink("Login")]
        [Authorize(Roles = "Admin")]
        public ActionResult AdminValidate(string id)
        {
            ProfileBase thisProfile = ProfileBase.Create(id);
            Membership.GetUser(id);
            bool adminValidation = bool.Parse(thisProfile.GetPropertyValue("AdminValidation").ToString());
            if (!adminValidation)
            {
                ViewBag.User = id;
                return View();
            }
            else
            {
                try
                {
                    Roles.RemoveUserFromRole(id, "Disabled");
                    Roles.AddUserToRole(id, "Default");
                }
                catch (Exception) { };
                TempData["Message"] = "This account has already been validated";
            }
            return RedirectToAction("Index", "Home");
        }

        // **************************************
        // URL: /Account/AdminValidate/User
        // **************************************
        [CurrentLink("Login")]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AdminValidate()
        {
            string id = Request.Form["ID"].Trim();
            ProfileBase thisProfile = ProfileBase.Create(id);
            bool adminValidation = bool.Parse(thisProfile.GetPropertyValue("AdminValidation").ToString());
            if (!adminValidation)
            {
                string username = thisProfile.GetPropertyValue("Name").ToString();
                string link = ConfigurationManager.AppSettings["SiteURL"] + "Account/LogOn/";
                MailModel mm = MailModel.FetchByName("MailWithCode");
                ViewBag.ID = id;
                ViewBag.Content = mm.Content.Replace("@Username", username.Trim()).Replace("@LogonLink", link);
                ViewBag.Target = "AdminValidateSend";
                ViewBag.Action = "Validation";
                return View("MoreInfo");
            }
            else
            {
                TempData["Message"] = "This account has already been validated";
            }
            return RedirectToAction("Thanks", "Home");
        }

        // **************************************
        // URL: /Account/AdminValidate/User
        // **************************************
        [CurrentLink("Login")]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AdminValidateSend()
        {
            FormCollection form = new FormCollection(Request.Unvalidated().Form);
            string id = form["ID"].Trim();
            ProfileBase thisProfile = ProfileBase.Create(id);
            MembershipUser user = Membership.GetUser(id);
            bool adminValidation = bool.Parse(thisProfile.GetPropertyValue("AdminValidation").ToString());
            if (!adminValidation)
            {
                thisProfile.SetPropertyValue("AdminValidation", true);
                thisProfile.SetPropertyValue("ValidationCode", "");
                thisProfile.Save();
                Roles.RemoveUserFromRole(id, "Disabled");
                Roles.AddUserToRole(id, "Default");
                TempData["Message"] = "You have validated " + id + "'s account";
                Mails.SendWrapped(user?.Email, form["content"].Trim(), "Inputlog account validation");
            }
            else
            {
                TempData["Message"] = "This account has already been validated";
            }
            return RedirectToAction("Index", "Home");
        }

        // **************************************
        // URL: /Account/AdminRefuse/User
        // **************************************
        [CurrentLink("Login")]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AdminRefuse()
        {
            string ID = Request.Form["ID"].Trim();
            ProfileBase thisProfile = ProfileBase.Create(ID);
            bool adminValidation = bool.Parse(thisProfile.GetPropertyValue("AdminValidation").ToString());
            if (!adminValidation)
            {
                string username = thisProfile.GetPropertyValue("Name").ToString();
                MailModel mm = MailModel.FetchByName("AccountRefusal");
                ViewBag.ID = ID;
                ViewBag.Content = mm.Content.Replace("@Username", username.Trim());
                ViewBag.Target = "AdminRefuseSend";
                ViewBag.Action = "Refuse";
                return View("MoreInfo");
            }
            else
            {
                TempData["Message"] = "This account has already been validated";
            }
            return RedirectToAction("Index", "Home");
        }

        // **************************************
        // URL: /Account/AdminRefuseSend/User
        // **************************************
        [CurrentLink("Login")]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AdminRefuseSend()
        {
            FormCollection form = new FormCollection(Request.Unvalidated().Form);
            string ID = form["ID"].Trim();
            ProfileBase thisProfile = ProfileBase.Create(ID);
            MembershipUser user = Membership.GetUser(ID);
            MembershipUser admin = Membership.GetUser(User.Identity.Name);
            bool adminValidation = bool.Parse(thisProfile.GetPropertyValue("AdminValidation").ToString());
            if (!adminValidation)
            {
                TempData["Message"] = "You have refused " + ID + "'s account";
                Mails.SendWrapped(user?.Email, form["content"].Trim(), "Inputlog account validation");
            }
            else
            {
                TempData["Message"] = "This account has already been validated";
            }
            return RedirectToAction("Index", "Home");
        }

        // **************************************
        // URL: /Account/AdminMoreInfo/User
        // **************************************
        [CurrentLink("Login")]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AdminMoreInfo()
        {
            string ID = Request.Form["ID"].Trim();
            ProfileBase thisProfile = ProfileBase.Create(ID);
            bool adminValidation = bool.Parse(thisProfile.GetPropertyValue("AdminValidation").ToString());
            if (!adminValidation)
            {
                string username = thisProfile.GetPropertyValue("Name").ToString();
                MailModel mm = MailModel.FetchByName("AccountMoreInfo");
                ViewBag.ID = ID;
                ViewBag.Content = mm.Content.Replace("@Username", username.Trim());
                ViewBag.Target = "AdminMoreInfoSend";
                ViewBag.Action = "More Info";
                return View("MoreInfo");
            }
            else
            {
                TempData["Message"] = "This account has already been validated";
            }
            return RedirectToAction("Index", "Home");
        }

        // **************************************
        // URL: /Account/AdminMoreInfoSend/User
        // **************************************
        [CurrentLink("Login")]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AdminMoreInfoSend()
        {
            FormCollection form = new FormCollection(Request.Unvalidated().Form);
            string id = form["ID"].Trim();
            ProfileBase thisProfile = ProfileBase.Create(id);
            MembershipUser user = Membership.GetUser(id);
            MembershipUser admin = Membership.GetUser(User.Identity.Name);
            bool adminValidation = bool.Parse(thisProfile.GetPropertyValue("AdminValidation").ToString());
            if (!adminValidation)
            {
                TempData["Message"] = "You have requested more info from " + id;
                Mails.SendWrapped(user.Email, form["content"].Trim(), "Inputlog account validation");
            }
            else
            {
                TempData["Message"] = "This account has already been validated";
            }
            return RedirectToAction("Index", "Home");
        }

        // **************************************
        // URL: /Account/Validate?User=x&Code=y
        // **************************************
        [CurrentLink("Login")]
        public ActionResult Validate(string User, string Code)
        {
            ProfileBase thisProfile = ProfileBase.Create(User);
            string expectedCode = thisProfile.GetPropertyValue("ValidationCode").ToString();
            if (!string.IsNullOrEmpty(expectedCode) && expectedCode.Equals(Code))
            {
                thisProfile.SetPropertyValue("ValidationCode", "");
                thisProfile.Save();
                NotifyAdmins(User, thisProfile);  
                TempData["Message"] = "Your email-address has been validated. " +
                                      "Your account will be accessible when an administrator approves it";
            }
            return RedirectToAction("Thanks", "Home");
        }

        private void NotifyAdmins(string User, ProfileBase profile)
        {
            foreach (string adminName in Roles.GetUsersInRole("Admin"))
            {
                MembershipUser admin = Membership.GetUser(adminName);
                ProfileBase adminProfile = ProfileBase.Create(adminName);
                bool notifications = bool.Parse(adminProfile.GetPropertyValue("EmailNotifications").ToString());
                if (notifications)
                {
                    dynamic email = new Email("NewUser");
                    var stream = new WebClient().OpenRead(ConfigurationManager.AppSettings["SiteURL"] 
                        + "Content/DescriptionHeader.PNG");
                    Attachment att = new Attachment(stream ?? throw new InvalidOperationException(), "DescriptionHeader.PNG");
                    email.Attach(att);
                    email.cid = att.ContentId;
                    if (admin != null) email.To = admin.Email;
                    email.Username = User;
                    email.Fullname = profile.GetPropertyValue("Name").ToString();
                    email.Affiliation = profile.GetPropertyValue("Affiliation").ToString();
                    email.Address = profile.GetPropertyValue("Address").ToString();
                    email.Postal = profile.GetPropertyValue("Postal").ToString();
                    email.City = profile.GetPropertyValue("City").ToString();
                    email.Country = profile.GetPropertyValue("Country").ToString();
                    email.Research = profile.GetPropertyValue("Research").ToString();
                    email.AdminName = adminName;
                    email.Mail = Membership.GetUser(User)?.Email;
                    email.Link = ConfigurationManager.AppSettings["SiteURL"]
                            + "Account/AdminValidate/" + User;
                    email.Send();
                }
            }
        }

    }
}