using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace tontine.MVC.Controllers
{
    public class BaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (HttpContext.Session.GetString("user_id") == null ||
                HttpContext.Session.GetString("cycle_id") == null)
            {
                context.Result = RedirectToAction("Login", "Auth");
                return;
            }
            ViewData["CycleId"]  = CycleId;
            ViewData["CycleNom"] = CycleNom;
            base.OnActionExecuting(context);
        }

        protected int CycleId => int.Parse(HttpContext.Session.GetString("cycle_id")!);
        protected string CycleNom => HttpContext.Session.GetString("cycle_nom") ?? "";

        protected void SetBreadcrumbs(params (string Label, string Controller, string Action, bool IsActive)[] breadcrumbs)
        {
            ViewData["Breadcrumbs"] = new List<(string, string, string, bool)>(breadcrumbs);
        }

        protected (string Label, string, string, bool) BreadcrumbItem(
            string label,
            string controller = null,
            string action = null,
            bool isActive = false)
        {
            return (label, controller, action, isActive);
        }
    }
}
