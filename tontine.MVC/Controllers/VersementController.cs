using Microsoft.AspNetCore.Mvc;
using tontine.MVC.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using tontine.MVC.Models;
using tontine.MVC.Services;

namespace tontine.MVC.Controllers
{
    public class VersementController : BaseController
    {
        private readonly IVersementService _service;
        private readonly IMembreService    _membres;
        private readonly ITontineService   _tontines;
        private readonly ICycleService     _cycles;

        public VersementController(IVersementService service, IMembreService membres,
                                    ITontineService tontines, ICycleService cycles)
        {
            _service  = service;
            _membres  = membres;
            _tontines = tontines;
            _cycles   = cycles;
        }

        public async Task<IActionResult> Index()
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Comptabilité"),
                BreadcrumbItem("Versements", isActive: true)
            );
            var list = await _service.GetAllAsync();
            return View(list.Where(v => v.IdCycle == CycleId).ToList());
        }

        public async Task<IActionResult> Create()
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Comptabilité"),
                BreadcrumbItem("Versements", "Versement", "Index"),
                BreadcrumbItem("Ajouter", isActive: true)
            );
            await PopulateDropdowns();
            return View(new VersementViewModel { IdCycle = CycleId, DateVersement = DateTime.Now });
        }

        [RoleAuthorize("Administrateur", "Gestionnaire")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VersementViewModel vm)
        {
            if (!ModelState.IsValid) { await PopulateDropdowns(); return View(vm); }
            var ok = await _service.CreateAsync(vm);
            if (!ok) { ModelState.AddModelError("", "Erreur lors de l'enregistrement."); await PopulateDropdowns(); return View(vm); }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Comptabilité"),
                BreadcrumbItem("Versements", "Versement", "Index"),
                BreadcrumbItem("Modifier", isActive: true)
            );
            var vm = await _service.GetByIdAsync(id);
            if (vm == null) return NotFound();
            await PopulateDropdowns();
            return View(vm);
        }

        [RoleAuthorize("Administrateur", "Gestionnaire")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, VersementViewModel vm)
        {
            if (!ModelState.IsValid) { await PopulateDropdowns(); return View(vm); }
            var ok = await _service.UpdateAsync(id, vm);
            if (!ok) { ModelState.AddModelError("", "Erreur lors de la modification."); await PopulateDropdowns(); return View(vm); }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Comptabilité"),
                BreadcrumbItem("Versements", "Versement", "Index"),
                BreadcrumbItem("Supprimer", isActive: true)
            );
            var vm = await _service.GetByIdAsync(id);
            return vm == null ? NotFound() : View(vm);
        }

        [RoleAuthorize("Administrateur", "Gestionnaire")]

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateDropdowns()
        {
            var membres  = await _membres.GetAllAsync();
            var tontines = await _tontines.GetAllAsync();
            var cycles   = await _cycles.GetAllAsync();

            ViewBag.Membres  = membres.Select(m => new SelectListItem(m.Nom + " " + m.Prenom, m.IdMembre.ToString()));
            ViewBag.Tontines = tontines.Select(t => new SelectListItem(t.Libelle, t.IdTontine.ToString()));
            ViewBag.Cycles   = cycles.Select(c => new SelectListItem(c.NomCycle, c.IdCycle.ToString()));
        }
    }
}
