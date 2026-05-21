using Microsoft.AspNetCore.Mvc;
using tontine.MVC.Services;
using tontine.MVC.Filters;

namespace tontine.MVC.Controllers
{
    public class AmendeController : BaseController
    {
        private readonly IAmendeService _amendeService;

        public AmendeController(IAmendeService amendeService)
        {
            _amendeService = amendeService;
        }

        public async Task<IActionResult> Index()
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Comptabilité"),
                BreadcrumbItem("Amendes", isActive: true)
            );

            var amendes = await _amendeService.GetAllAsync();
            return View(amendes);
        }

        [HttpPost]
        [RoleAuthorize("Administrateur", "Gestionnaire")]
        public async Task<IActionResult> Generer()
        {
            int generees = await _amendeService.GenererAsync(CycleId);

            TempData["Success"] = generees > 0
                ? $"{generees} amende(s) générée(s) avec succès."
                : "Aucune nouvelle amende à générer pour ce cycle.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [RoleAuthorize("Administrateur", "Gestionnaire")]
        public async Task<IActionResult> MarquerPayee(int id)
        {
            await _amendeService.MarquerPayeeAsync(id);
            TempData["Success"] = "Amende marquée comme payée.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [RoleAuthorize("Administrateur")]
        public async Task<IActionResult> Delete(int id)
        {
            await _amendeService.DeleteAsync(id);
            TempData["Success"] = "Amende supprimée.";
            return RedirectToAction(nameof(Index));
        }
    }
}
