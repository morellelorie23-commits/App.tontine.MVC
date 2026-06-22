using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using tontine.MVC.Models;
using tontine.MVC.Services;

namespace tontine.MVC.Controllers
{
    public class ReunionController : BaseController
    {
        private readonly IReunionService  _service;
        private readonly ITontineService  _tontines;

        public ReunionController(IReunionService service, ITontineService tontines)
        {
            _service  = service;
            _tontines = tontines;
        }

        public async Task<IActionResult> Index()
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Réunions", isActive: true)
            );
            var list = await _service.GetAllAsync();
            return View(list.Where(r => r.IdCycle == CycleId).ToList());
        }

        public async Task<IActionResult> Create()
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Réunions", "Reunion", "Index"),
                BreadcrumbItem("Ajouter", isActive: true)
            );
            await PopulateDropdowns();
            return View(new ReunionViewModel { IdCycle = CycleId, DateReunion = DateTime.Now });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReunionViewModel vm)
        {
            if (!ModelState.IsValid) { await PopulateDropdowns(); return View(vm); }
            var ok = await _service.CreateAsync(vm);
            if (!ok) { ModelState.AddModelError("", "Erreur lors de l'enregistrement."); await PopulateDropdowns(); return View(vm); }
            TempData["Success"] = "Réunion enregistrée avec succès.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Réunions", "Reunion", "Index"),
                BreadcrumbItem("Modifier", isActive: true)
            );
            var vm = await _service.GetByIdAsync(id);
            if (vm == null) return NotFound();
            await PopulateDropdowns();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ReunionViewModel vm)
        {
            if (!ModelState.IsValid) { await PopulateDropdowns(); return View(vm); }
            var ok = await _service.UpdateAsync(id, vm);
            if (!ok) { ModelState.AddModelError("", "Erreur lors de la modification."); await PopulateDropdowns(); return View(vm); }
            TempData["Success"] = "Réunion modifiée avec succès.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Réunions", "Reunion", "Index"),
                BreadcrumbItem("Supprimer", isActive: true)
            );
            var vm = await _service.GetByIdAsync(id);
            return vm == null ? NotFound() : View(vm);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _service.DeleteAsync(id);
            TempData["Success"] = "Réunion supprimée.";
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateDropdowns()
        {
            var tontines = await _tontines.GetAllAsync();
            ViewBag.Tontines = tontines.Select(t => new SelectListItem(t.Libelle, t.IdTontine.ToString()));
        }
    }
}
