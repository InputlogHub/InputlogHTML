using System.Web.Mvc;
using WebApp.Models;
using WebApp.ActionFilters;

namespace WebApp.Controllers
{
    /// <summary>
    ///  Controller handling CRUD for Analyses
    /// </summary> 
    [Authorize(Roles="Admin")]
    [CurrentLink("Administration")]
    public class AnalysisController : Controller
    {
        // **************************************
        // URL: /Analysis
        // **************************************

        public ActionResult Index()
        {
            return View(new AnalysisIndexViewModel(AnalysisModel.FetchAll()));
        }

        // **************************************
        // URL: /Analysis/Create
        // **************************************

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                string name = Request.Form["Name"];
                AnalysisModel.Insert(name);
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        // **************************************
        // URL: /Analysis/Deactivate
        // **************************************

        [HttpPost]
        public ActionResult Deactivate(int id, FormCollection collection)
        {
            try
            {
                AnalysisModel.Deactivate(id);
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        // **************************************
        // URL: /Analysis/Activate
        // **************************************

        [HttpPost]
        public ActionResult Activate(int id, FormCollection collection)
        {
            try
            {
                AnalysisModel.Activate(id);
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }
    }
}
