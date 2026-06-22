using Microsoft.AspNetCore.Mvc;
using tontine.MVC.Filters;
using tontine.MVC.Models;
using tontine.MVC.Services;

namespace tontine.MVC.Controllers
{
    public class CycleTontinePenaliteController : BaseController
    {
        private readonly ICycleTontinePenaliteService _service;
        private readonly ICycleService _cycleService;
        private readonly ITontineService _tontineService;
        private readonly IPenaliteService _penaliteService;

        public CycleTontinePenaliteController(
            ICycleTontinePenaliteService service,
            ICycleService cycleService,
            ITontineService tontineService,
            IPenaliteService penaliteService)
        {
            _service = service;
            _cycleService = cycleService;
            _tontineService = tontineService;
            _penaliteService = penaliteService;
        }

        public async Task<IActionResult> Index()
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Paramètres"),
                BreadcrumbItem("Liaisons"),
                BreadcrumbItem("Pénalités - Cycles", isActive: true)
            );
            var data = await _service.GetAllAsync();
            return View(data);
        }

        public async Task<IActionResult> Create()
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Paramètres"),
                BreadcrumbItem("Liaisons"),
                BreadcrumbItem("Pénalités - Cycles", "CycleTontinePenalite", "Index"),
                BreadcrumbItem("Ajouter", isActive: true)
            );
            var model = new CycleTontinePenaliteViewModel();
            await LoadDropdowns();
            return View(model);
        }

        [RoleAuthorize("Administrateur", "Gestionnaire")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CycleTontinePenaliteViewModel data)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                return View(data);
            }

            var ok = await _service.CreateAsync(data);
            if (!ok) ModelState.AddModelError("", "Erreur lors de la création.");
            return ok ? RedirectToAction(nameof(Index)) : View(data);
        }

        public async Task<IActionResult> Edit(int id)
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Paramètres"),
                BreadcrumbItem("Liaisons"),
                BreadcrumbItem("Pénalités - Cycles", "CycleTontinePenalite", "Index"),
                BreadcrumbItem("Modifier", isActive: true)
            );
            var data = await _service.GetByIdAsync(id);
            if (data == null) return NotFound();
            await LoadDropdowns();
            return View(data);
        }

        [RoleAuthorize("Administrateur", "Gestionnaire")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CycleTontinePenaliteViewModel data)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                return View(data);
            }

            var ok = await _service.UpdateAsync(id, data);
            if (!ok) ModelState.AddModelError("", "Erreur lors de la modification.");
            return ok ? RedirectToAction(nameof(Index)) : View(data);
        }

        public async Task<IActionResult> Delete(int id)
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Paramètres"),
                BreadcrumbItem("Liaisons"),
                BreadcrumbItem("Pénalités - Cycles", "CycleTontinePenalite", "Index"),
                BreadcrumbItem("Supprimer", isActive: true)
            );
            var data = await _service.GetByIdAsync(id);
            return data == null ? NotFound() : View(data);
        }

        [RoleAuthorize("Administrateur", "Gestionnaire")]

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadDropdowns()
        {
            var cycles = await _cycleService.GetAllAsync();
            var tontines = await _tontineService.GetAllAsync();
            var penalites = await _penaliteService.GetAllAsync();
            ViewData["Cycles"] = cycles;
            ViewData["Tontines"] = tontines;
            ViewData["Penalites"] = penalites;
        }
    }
}
