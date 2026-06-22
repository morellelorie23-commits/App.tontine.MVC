using Microsoft.AspNetCore.Mvc;
using tontine.MVC.Filters;
using tontine.MVC.Services;

namespace tontine.MVC.Controllers
{
    public class RoleController : BaseController
    {
        private readonly ICompteService _compteService;

        public RoleController(ICompteService compteService)
        {
            _compteService = compteService;
        }

        public async Task<IActionResult> Index()
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Sécurité"),
                BreadcrumbItem("Rôles & Permissions", isActive: true)
            );
            var comptes = await _compteService.GetAllAsync();
            return View(comptes);
        }
    }
}
