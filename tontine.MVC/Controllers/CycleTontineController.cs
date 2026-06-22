using Microsoft.AspNetCore.Mvc;
using tontine.MVC.Filters;
using tontine.MVC.Models;
using tontine.MVC.Services;

namespace tontine.MVC.Controllers
{
    public class CycleTontineController : BaseController
    {
        private readonly ICycleTontineService _service;
        private readonly ICycleService _cycleService;
        private readonly ITontineService _tontineService;

        public CycleTontineController(
            ICycleTontineService service,
            ICycleService cycleService,
            ITontineService tontineService)
        {
            _service = service;
            _cycleService = cycleService;
            _tontineService = tontineService;
        }

        public async Task<IActionResult> Index()
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Paramètres"),
                BreadcrumbItem("Liaisons"),
                BreadcrumbItem("Cycles - Tontines", isActive: true)
            );
            var cycleTontines = await _service.GetAllAsync();
            await LoadDropdowns();
            return View(cycleTontines.Where(ct => ct.IdCycle == CycleId).ToList());
        }

        public async Task<IActionResult> Create()
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Paramètres"),
                BreadcrumbItem("Liaisons"),
                BreadcrumbItem("Cycles - Tontines", "CycleTontine", "Index"),
                BreadcrumbItem("Ajouter", isActive: true)
            );
            var model = new CycleTontineViewModel { IdCycle = CycleId };
            await LoadDropdowns();
            return View(model);
        }

        [RoleAuthorize("Administrateur", "Gestionnaire")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CycleTontineViewModel cycleTontine)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                return View(cycleTontine);
            }

            var (ok, erreur) = await _service.CreateWithErrorAsync(cycleTontine);
            if (!ok)
            {
                ModelState.AddModelError("", erreur ?? "Erreur lors de la création.");
                await LoadDropdowns();
                return View(cycleTontine);
            }
            TempData["Success"] = "Liaison créée avec succès.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Paramètres"),
                BreadcrumbItem("Liaisons"),
                BreadcrumbItem("Cycles - Tontines", "CycleTontine", "Index"),
                BreadcrumbItem("Modifier", isActive: true)
            );
            var cycleTontine = await _service.GetByIdAsync(id);
            if (cycleTontine == null) return NotFound();
            await LoadDropdowns();
            return View(cycleTontine);
        }

        [RoleAuthorize("Administrateur", "Gestionnaire")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CycleTontineViewModel cycleTontine)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                return View(cycleTontine);
            }

            var ok = await _service.UpdateAsync(id, cycleTontine);
            if (!ok)
            {
                ModelState.AddModelError("", "Erreur lors de la modification.");
                await LoadDropdowns();
                return View(cycleTontine);
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Paramètres"),
                BreadcrumbItem("Liaisons"),
                BreadcrumbItem("Cycles - Tontines", "CycleTontine", "Index"),
                BreadcrumbItem("Supprimer", isActive: true)
            );
            var cycleTontine = await _service.GetByIdAsync(id);
            return cycleTontine == null ? NotFound() : View(cycleTontine);
        }

        [RoleAuthorize("Administrateur", "Gestionnaire")]

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [RoleAuthorize("Administrateur", "Gestionnaire")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Repartir(int id)
        {
            var (crees, membres, erreur) = await _service.RepartirAsync(id);
            if (erreur != null)
                TempData["Error"] = erreur;
            else if (crees == 0)
                TempData["Success"] = $"Toutes les cotisations existent déjà ({membres} membre(s)).";
            else
                TempData["Success"] = $"{crees} cotisation(s) créée(s) sur {membres} membre(s) inscrit(s).";
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadDropdowns()
        {
            var cycles = await _cycleService.GetAllAsync();
            var tontines = await _tontineService.GetAllAsync();
            ViewData["Cycles"] = cycles;
            ViewData["Tontines"] = tontines;
        }
    }
}
