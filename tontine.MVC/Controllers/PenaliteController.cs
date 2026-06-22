using Microsoft.AspNetCore.Mvc;
using tontine.MVC.Filters;
using tontine.MVC.Models;
using tontine.MVC.Services;

namespace tontine.MVC.Controllers
{
    public class PenaliteController : BaseController
    {
        private readonly IPenaliteService _service;

        public PenaliteController(IPenaliteService service) => _service = service;

        public async Task<IActionResult> Index()
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Paramètres"),
                BreadcrumbItem("Pénalités", isActive: true)
            );
            var penalites = await _service.GetAllAsync();
            return View(penalites);
        }

        public IActionResult Create()
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Paramètres"),
                BreadcrumbItem("Pénalités", "Penalite", "Index"),
                BreadcrumbItem("Ajouter", isActive: true)
            );
            return View(new PenaliteViewModel());
        }

        [RoleAuthorize("Administrateur", "Gestionnaire")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PenaliteViewModel penalite)
        {
            if (!ModelState.IsValid) return View(penalite);
            var ok = await _service.CreateAsync(penalite);
            if (!ok) ModelState.AddModelError("", "Erreur lors de la création.");
            return ok ? RedirectToAction(nameof(Index)) : View(penalite);
        }

        public async Task<IActionResult> Edit(int id)
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Paramètres"),
                BreadcrumbItem("Pénalités", "Penalite", "Index"),
                BreadcrumbItem("Modifier", isActive: true)
            );
            var penalite = await _service.GetByIdAsync(id);
            return penalite == null ? NotFound() : View(penalite);
        }

        [RoleAuthorize("Administrateur", "Gestionnaire")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PenaliteViewModel penalite)
        {
            if (!ModelState.IsValid) return View(penalite);
            var ok = await _service.UpdateAsync(id, penalite);
            if (!ok) ModelState.AddModelError("", "Erreur lors de la modification.");
            return ok ? RedirectToAction(nameof(Index)) : View(penalite);
        }

        public async Task<IActionResult> Delete(int id)
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Paramètres"),
                BreadcrumbItem("Pénalités", "Penalite", "Index"),
                BreadcrumbItem("Supprimer", isActive: true)
            );
            var penalite = await _service.GetByIdAsync(id);
            return penalite == null ? NotFound() : View(penalite);
        }

        [RoleAuthorize("Administrateur", "Gestionnaire")]

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}