using Microsoft.AspNetCore.Mvc;
using tontine.MVC.Filters;
using tontine.MVC.Models;
using tontine.MVC.Services;

namespace tontine.MVC.Controllers
{
    public class MembrePosteCycleController : BaseController
    {
        private readonly IMembrePosteCycleService _service;
        private readonly IMembreService _membreService;
        private readonly IPosteService _posteService;
        private readonly ICycleService _cycleService;
        private readonly ITontineService _tontineService;

        public MembrePosteCycleController(
            IMembrePosteCycleService service,
            IMembreService membreService,
            IPosteService posteService,
            ICycleService cycleService,
            ITontineService tontineService)
        {
            _service = service;
            _membreService = membreService;
            _posteService = posteService;
            _cycleService = cycleService;
            _tontineService = tontineService;
        }

        public async Task<IActionResult> Index()
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Paramètres"),
                BreadcrumbItem("Liaisons"),
                BreadcrumbItem("Attributions de rôles", isActive: true)
            );
            var data = await _service.GetAllAsync();
            return View(data.Where(m => m.IdCycle == CycleId).ToList());
        }

        public async Task<IActionResult> Create()
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Paramètres"),
                BreadcrumbItem("Liaisons"),
                BreadcrumbItem("Attributions de rôles", "MembrePosteCycle", "Index"),
                BreadcrumbItem("Ajouter", isActive: true)
            );
            var model = new MembrePosteCycleViewModel { IdCycle = CycleId };
            await LoadDropdowns();
            return View(model);
        }

        [RoleAuthorize("Administrateur", "Gestionnaire")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MembrePosteCycleViewModel data)
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
                BreadcrumbItem("Attributions de rôles", "MembrePosteCycle", "Index"),
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
        public async Task<IActionResult> Edit(int id, MembrePosteCycleViewModel data)
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
                BreadcrumbItem("Attributions de rôles", "MembrePosteCycle", "Index"),
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
            var membres = await _membreService.GetAllAsync();
            var postes = await _posteService.GetAllAsync();
            var cycles = await _cycleService.GetAllAsync();
            var tontines = await _tontineService.GetAllAsync();
            ViewData["Membres"] = membres;
            ViewData["Postes"] = postes;
            ViewData["Cycles"] = cycles;
            ViewData["Tontines"] = tontines;
        }
    }
}
