using System.Web.Mvc;

namespace WebApp.ActionFilters
{
    public class CurrentLink: ActionFilterAttribute
    {
        private string Link { get; set; }

        public CurrentLink(string link)
        {
            Link = link;
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            dynamic viewBag = filterContext.Controller.ViewBag;
            viewBag.CurrentLink = Link;
        }

    }
}