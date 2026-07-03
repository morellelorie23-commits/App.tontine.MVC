using Microsoft.AspNetCore.Mvc;
using tontine.MVC.Filters;
using tontine.MVC.Services;

namespace tontine.MVC.Controllers
{
    [RoleAuthorize("Administrateur", "Gestionnaire")]
    public class PaiementMobileController : BaseController
    {
        private readonly IPaiementMobileService _service;

        public PaiementMobileController(IPaiementMobileService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Paiements mobiles", isActive: true)
            );
            var list = await _service.GetAllAsync(CycleId);
            return View(list);
        }
    }
}
