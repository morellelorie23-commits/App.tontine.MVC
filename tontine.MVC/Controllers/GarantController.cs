using Microsoft.AspNetCore.Mvc;
using tontine.MVC.Filters;
using tontine.MVC.Models;
using tontine.MVC.Services;

namespace tontine.MVC.Controllers
{
    public class GarantController : BaseController
    {
        private readonly IGarantService _service;
        private readonly IMembreService _membreService;

        public GarantController(IGarantService service, IMembreService membreService)
        {
            _service = service;
            _membreService = membreService;
        }

        public async Task<IActionResult> Index()
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Paramètres"),
                BreadcrumbItem("Garants", isActive: true)
            );
            var garants = await _service.GetAllAsync();
            return View(garants);
        }

        public async Task<IActionResult> Create()
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Paramètres"),
                BreadcrumbItem("Garants", "Garant", "Index"),
                BreadcrumbItem("Ajouter", isActive: true)
            );
            await LoadMembres();
            return View(new GarantViewModel());
        }

        [RoleAuthorize("Administrateur", "Gestionnaire")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GarantViewModel garant)
        {
            if (!ModelState.IsValid)
            {
                await LoadMembres();
                return View(garant);
            }
            var ok = await _service.CreateAsync(garant);
            if (!ok)
            {
                ModelState.AddModelError("", "Erreur lors de l'enregistrement du garant. Vérifiez que ce membre n'a pas déjà un garant.");
                await LoadMembres();
                return View(garant);
            }
            TempData["Success"] = "Garant enregistré avec succès.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Paramètres"),
                BreadcrumbItem("Garants", "Garant", "Index"),
                BreadcrumbItem("Modifier", isActive: true)
            );
            var garant = await _service.GetByIdAsync(id);
            if (garant == null) return NotFound();
            await LoadMembres();
            return View(garant);
        }

        [RoleAuthorize("Administrateur", "Gestionnaire")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, GarantViewModel garant)
        {
            if (!ModelState.IsValid)
            {
                await LoadMembres();
                return View(garant);
            }
            var ok = await _service.UpdateAsync(id, garant);
            if (!ok)
            {
                ModelState.AddModelError("", "Erreur lors de la modification.");
                await LoadMembres();
                return View(garant);
            }
            TempData["Success"] = "Garant modifié avec succès.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Paramètres"),
                BreadcrumbItem("Garants", "Garant", "Index"),
                BreadcrumbItem("Supprimer", isActive: true)
            );
            var garant = await _service.GetByIdAsync(id);
            return garant == null ? NotFound() : View(garant);
        }

        [RoleAuthorize("Administrateur", "Gestionnaire")]

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _service.DeleteAsync(id);
            TempData["Success"] = "Garant supprimé.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> CheckEligibilite(int idMembre)
        {
            var result = await _service.CheckEligibiliteAsync(idMembre);
            return Json(result);
        }

        private async Task LoadMembres()
        {
            ViewData["Membres"] = await _membreService.GetAllAsync();
        }
    }
}
