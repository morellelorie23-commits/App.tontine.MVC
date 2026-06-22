using Microsoft.AspNetCore.Mvc;
using tontine.MVC.Filters;
using tontine.MVC.Models;
using tontine.MVC.Services;

namespace tontine.MVC.Controllers
{
    public class CycleController : BaseController
    {
        private readonly ICycleService _service;

        public CycleController(ICycleService service) => _service = service;

        public async Task<IActionResult> Index()
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Paramètres"),
                BreadcrumbItem("Cycles", isActive: true)
            );
            var cycles = await _service.GetAllAsync();
            return View(cycles);
        }

        public IActionResult Create()
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Paramètres"),
                BreadcrumbItem("Cycles", "Cycle", "Index"),
                BreadcrumbItem("Ajouter", isActive: true)
            );
            return View(new CycleViewModel());
        }

        [RoleAuthorize("Administrateur", "Gestionnaire")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CycleViewModel cycle)
        {
            if (!ModelState.IsValid) return View(cycle);
            var ok = await _service.CreateAsync(cycle);
            if (!ok) ModelState.AddModelError("", "Erreur lors de la création.");
            return ok ? RedirectToAction(nameof(Index)) : View(cycle);
        }

        public async Task<IActionResult> Edit(int id)
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Paramètres"),
                BreadcrumbItem("Cycles", "Cycle", "Index"),
                BreadcrumbItem("Modifier", isActive: true)
            );
            var cycle = await _service.GetByIdAsync(id);
            return cycle == null ? NotFound() : View(cycle);
        }

        [RoleAuthorize("Administrateur", "Gestionnaire")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CycleViewModel cycle)
        {
            if (!ModelState.IsValid) return View(cycle);
            var ok = await _service.UpdateAsync(id, cycle);
            if (!ok) ModelState.AddModelError("", "Erreur lors de la modification.");
            return ok ? RedirectToAction(nameof(Index)) : View(cycle);
        }

        [RoleAuthorize("Administrateur", "Gestionnaire")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Demarrer(int id)
        {
            var ok = await _service.DemarrerAsync(id);
            TempData[ok ? "Success" : "Error"] = ok ? "Cycle démarré avec succès." : "Impossible de démarrer ce cycle.";
            return RedirectToAction(nameof(Index));
        }

        [RoleAuthorize("Administrateur", "Gestionnaire")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cloturer(int id)
        {
            var ok = await _service.CloturerAsync(id);
            TempData[ok ? "Success" : "Error"] = ok ? "Cycle clôturé avec succès." : "Impossible de clôturer ce cycle.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Paramètres"),
                BreadcrumbItem("Cycles", "Cycle", "Index"),
                BreadcrumbItem("Supprimer", isActive: true)
            );
            var cycle = await _service.GetByIdAsync(id);
            return cycle == null ? NotFound() : View(cycle);
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