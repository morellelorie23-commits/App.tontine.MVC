using Microsoft.AspNetCore.Mvc;
using tontine.MVC.Services;

namespace tontine.MVC.Controllers
{
    public class JournalController : BaseController
    {
        private readonly IJournalService _service;

        public JournalController(IJournalService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Sécurité"),
                BreadcrumbItem("Journal d'activité", isActive: true)
            );
            var list = await _service.GetAllAsync();
            return View(list);
        }
    }
}
