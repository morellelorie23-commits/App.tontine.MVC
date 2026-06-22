using Microsoft.AspNetCore.Mvc;
using tontine.MVC.Filters;
using tontine.MVC.Models;
using tontine.MVC.Services;

namespace tontine.MVC.Controllers
{
    public class MembreCycleTontineController : BaseController
    {
        private readonly IMembreCycleTontineService _service;
        private readonly IMembreService _membreService;
        private readonly ICycleService _cycleService;
        private readonly ITontineService _tontineService;

        public MembreCycleTontineController(
            IMembreCycleTontineService service,
            IMembreService membreService,
            ICycleService cycleService,
            ITontineService tontineService)
        {
            _service = service;
            _membreService = membreService;
            _cycleService = cycleService;
            _tontineService = tontineService;
        }

        public async Task<IActionResult> Index()
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Paramètres"),
                BreadcrumbItem("Liaisons"),
                BreadcrumbItem("Participations", isActive: true)
            );
            var data = await _service.GetAllAsync();
            return View(data.Where(m => m.IdCycle == CycleId).ToList());
        }

        public async Task<IActionResult> Create()
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Participations", "MembreCycleTontine", "Index"),
                BreadcrumbItem("Nouvelle participation", isActive: true)
            );

            var tontines = await _tontineService.GetAllAsync();
            var membres  = await _membreService.GetAllAsync();

            var vm = new SaisieSeancePageViewModel
            {
                IdCycle   = CycleId,
                IdTontine = 0,
                IdReunion = 0,
                Tontines  = tontines.Select(t => new TontineSelectDto
                {
                    IdTontine = t.IdTontine,
                    Libelle   = t.Libelle ?? ""
                }).ToList(),
                Membres   = membres.Select(m => new MembreSelectDto
                {
                    IdMembre  = m.IdMembre,
                    NomPrenom = m.Nom + " " + m.Prenom
                }).ToList()
            };

            return View(vm);
        }

        public async Task<IActionResult> Edit(int id)
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Paramètres"),
                BreadcrumbItem("Liaisons"),
                BreadcrumbItem("Participations", "MembreCycleTontine", "Index"),
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
        public async Task<IActionResult> Edit(int id, MembreCycleTontineViewModel data)
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
                BreadcrumbItem("Participations", "MembreCycleTontine", "Index"),
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

        // ── Saisie Séance — accessible depuis le module Participation ──
        public async Task<IActionResult> SaisieSeance(int? idTontine = null, int? idReunion = null)
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Participations", "MembreCycleTontine", "Index"),
                BreadcrumbItem("Saisie Séance", isActive: true)
            );

            var tontines = await _tontineService.GetAllAsync();
            var membres  = await _membreService.GetAllAsync();

            var vm = new SaisieSeancePageViewModel
            {
                IdCycle   = CycleId,
                IdTontine = idTontine ?? 0,
                IdReunion = idReunion ?? 0,
                Tontines  = tontines.Select(t => new TontineSelectDto
                {
                    IdTontine = t.IdTontine,
                    Libelle   = t.Libelle ?? ""
                }).ToList(),
                Membres   = membres.Select(m => new MembreSelectDto
                {
                    IdMembre  = m.IdMembre,
                    NomPrenom = m.Nom + " " + m.Prenom
                }).ToList()
            };

            // Réutilise la vue premium existante
            return View("~/Views/SaisieSeance/Index.cshtml", vm);
        }

        private async Task LoadDropdowns()
        {
            var membres = await _membreService.GetAllAsync();
            var cycles = await _cycleService.GetAllAsync();
            var tontines = await _tontineService.GetAllAsync();
            ViewData["Membres"] = membres;
            ViewData["Cycles"] = cycles;
            ViewData["Tontines"] = tontines;
        }
    }
}
