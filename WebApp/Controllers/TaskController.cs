using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Profile;
using System.Web.Security;
using InputLog.Core.Util.Server;
using WebApp.ActionFilters;
using WebApp.Models;
using WebApp.Util;

namespace WebApp.Controllers
{
    /// <summary>
    /// Controller handling CRUD for Tasks
    /// </summary>
    [Authorize(Roles = "Admin, Default")]
    [CurrentLink("Project Overview")]
    public class TaskController : Controller
    {
        /// <summary>
        /// URL: /Task
        /// Shows overview of tasks for the current user
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewBag.Full = false;
            return View(new TaskIndexViewModel(TaskModel.FetchAllByUserID(User.Identity.Name)));
        }

        /// <summary>
        /// URL: /Task/FullIndex
        /// Shows overview of tasks for all users, used for admins
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        public ActionResult FullIndex()
        {
            ViewBag.Full = true;
            return View("Index", new TaskIndexViewModel(TaskModel.FetchAll()));
        }

        /// <summary>
        /// URL: /Task/SendNotifications
        /// Sends mail notifications for tasks that have not been downloaded yet
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        public ActionResult SendNotifications()
        {
            var tasks = (List<TaskModel>) TaskModel.FetchCompletedNotDownloaded();
            Dictionary<string, List<TaskModel>> toNotify = new Dictionary<string, List<TaskModel>>();
            foreach (var task in tasks)
            {
                var userName = task.UserID;
                var profile = ProfileBase.Create(userName);
                var notifications = bool.Parse(profile.GetPropertyValue("EmailNotifications").ToString());
                if (Membership.GetUser(userName) != null && notifications)
                {
                    if (!toNotify.ContainsKey(userName))
                    {
                        toNotify.Add(userName, new List<TaskModel>());
                    }
                    toNotify[userName].Add(task);
                }
            }
            foreach (string userName in toNotify.Keys)
            {
                MailModel mm = MailModel.FetchByName("TaskNotification");
                var baseURL = ConfigurationManager.AppSettings["SiteURL"];
                MembershipUser membershipUser = Membership.GetUser(userName);
                if (toNotify[userName].Count == 1)
                {
                    TaskModel task = toNotify[userName].First();
                    string downloadLink = baseURL + "Task/Results/" + task.ID + "/Results.zip";
                    string content = mm.Content.Replace("@Username", userName).Replace("@DownloadLink", downloadLink);
                    if (membershipUser != null)
                        Mails.SendWrapped(membershipUser.Email, content, "Inputlog Task completed");
                }
                else
                {
                    string downloadLink = baseURL + "Task/";
                    string content = mm.Content.Replace("@Username", userName).Replace("@DownloadLink", downloadLink);
                    if (membershipUser != null)
                        Mails.SendWrapped(membershipUser.Email, content, "Inputlog Task completed");
                }
                foreach (TaskModel task in toNotify[userName])
                {
                    TaskModel.MarkDownloaded(task.ID);
                }
            }
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// URL: /Task/Cleanup
        /// Removes old task files from server directory
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        public ActionResult Cleanup()
        {
            var tasks = (List<TaskModel>) TaskModel.FetchOutdated(31);
            string dirs = "";
            foreach (var task in tasks)
            {
                var taskDir = Path.Combine(task.UserID, task.Guid);
                var taskDirFullPath = Path.Combine(ConfigurationManager.AppSettings["ServerTaskDirectory"], taskDir);
                dirs += (" ; " + taskDirFullPath);
                try
                {
                    Directory.Delete(taskDirFullPath, true);
                }
                catch
                {
                    // Directory is probably gone already
                }
            }
            TaskModel.MarkPruned(31);
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// URL: /Task/Details/ID
        /// Presents a detailed overview of the Task
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(int id)
        {
            var data = TaskModel.GetByIDWithAnalyses(id);
            var taskDir = Path.Combine(data.Key.UserID, data.Key.Guid);
            var taskDirFullPath = Path.Combine(ConfigurationManager.AppSettings["ServerTaskDirectory"], taskDir);
            var outputPath = Path.Combine(taskDirFullPath, "Output.xml");
            return View(new TaskDetailsViewModel(data.Key, data.Value, outputPath));
        }

        /// <summary>
        /// URL: /Task/GetStatus/ID
        /// Returns the Task's status (as an int)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetStatus(int id)
        {
            var task = TaskModel.FetchByID(id);
            ViewBag.Value = task.Status;
            return View("Value");
        }

        /// <summary>
        /// URL: /Task/GetPositionInQueue/ID
        /// Returns the Task's position in the queue
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetPositionInQueue(int id)
        {
            var queue = TaskModel.GetPositionInQueue(id);
            ViewBag.Value = queue;
            return View("Value");
        }

        /// <summary>
        /// URL: /Task/Results/ID
        /// Returns a zip file with Task output
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FilePathResult Results(int id)
        {
            var task = TaskModel.FetchByID(id);
            const string fileName = "Results.zip";
            var taskDir = Path.Combine(task.UserID, task.Guid);
            var taskDirFullPath = Path.Combine(ConfigurationManager.AppSettings["ServerTaskDirectory"], taskDir);
            if (!task.Downloaded)
            {
                TaskModel.MarkDownloaded(task.ID);
            }
            return File(Path.Combine(taskDirFullPath, fileName), "application/gzip");
        }

        /// <summary>
        /// URL: /Task/Summary/ID
        /// Returns a Zip with analysis summary files
        /// Should only be used by Client (GUI)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FilePathResult Summary(int id)
        {
            var task = TaskModel.FetchByID(id);
            const string fileName = "Summaries.zip";
            var taskDir = Path.Combine(task.UserID, task.Guid);
            var taskDirFullPath = Path.Combine(ConfigurationManager.AppSettings["ServerTaskDirectory"], taskDir);
            return File(Path.Combine(taskDirFullPath, fileName), "application/gzip");
        }

        /// <summary>
        /// URL: /Task/Create
        /// Shows Task creation form.
        /// Only for use by Client (GUI) atm.
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            return View(new TaskCreateViewModel(AnalysisModel.FetchAll()));
        }

        /// <summary>
        /// URL: /Task/Create
        /// Creates a Task
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(FormCollection collection, HttpPostedFileBase file)
        {
            try
            {
                var analyses = Request.Form["analyses"].Trim();
                if (!analyses.Equals(""))
                {
                    var ans = analyses.Split(new[] {','});
                    var g = GenerateUID(ans);
                    if (file.ContentLength > 0)
                    {
                        const string fileName = "Input.zip";
                        var taskDir = Path.Combine(User.Identity.Name, g);
                        var taskDirFullPath = 
                            Path.Combine(ConfigurationManager.AppSettings["ServerTaskDirectory"],taskDir);
                        var fullPath = Path.Combine(taskDirFullPath, fileName);
                        var userDir = new DirectoryInfo(taskDirFullPath);
                        if (!userDir.Exists)
                        {
                            userDir.Create();
                        }
                        file.SaveAs(fullPath);
                        var id = TaskModel.InsertWithAnalyses(User.Identity.Name, g, TaskStatus.STATUS_UNSTARTED, ans.ToList());
                        return RedirectToAction("Details", new {ID = id});
                    }
                    TempData["ErrorMsg"] = "Please upload a file to analyze";
                }
                else
                {
                    TempData["ErrorMsg"] = "Please select at least one analysis";
                }
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                TempData["ErrorMsg"] = e.Message;
                return RedirectToAction("Index");
            }
        }

        private string GenerateUID(string[] ans)
        {
            string result = User.Identity.Name + ".";
            foreach (string a in ans)
            {
                result += (AnalysisModel.FetchByID(Int32.Parse(a)).Name.Replace(" ", string.Empty) + ".");
            }
            result += Guid.NewGuid().ToString().Substring(0, 10);
            return result;
        }

        /// <summary>
        /// URL: /Task/Delete/ID
        /// Shows confirm delete page if task is not pending/completed
        /// </summary>
        /// <param name="id"></param>
        /// <param name="full">Did we come from fullIndex?</param>
        /// <returns></returns>
        public ActionResult Delete(int id, bool full)
        {
            var task = TaskModel.FetchByID(id);
            ViewBag.Full = full;
            if (task.Done())
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        /// <summary>
        /// URL: /Task/Delete/ID
        /// Deletes a task if it is not pending/completed
        /// </summary>
        /// <param name="id"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                var task = TaskModel.FetchByID(id);
                if (!task.Done())
                {
                    TaskModel.Delete(id);
                }
                if (Request.Form["full"].Trim().Equals("0"))
                    return RedirectToAction("Index");
                else
                 return RedirectToAction("FullIndex");
            }
            catch
            {
                return View();
            }
        }

        /// <summary>
        /// URL: /Task/Reset/ID
        /// Puts a pending (or completed) task back to 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        public ActionResult Reset(int id, FormCollection collection)
        {
            try
            {
                var task = TaskModel.FetchByID(id);
                if (task.HasStarted())
                {
                    task.Queue();
                }
                return RedirectToAction("FullIndex");
            }
            catch
            {
                return View();
            }
        }
    }
}