using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using tontine.MVC.Filters;
using tontine.MVC.Models;
using tontine.MVC.Services;

namespace tontine.MVC.Controllers
{
    [RoleAuthorize("Administrateur", "Gestionnaire")]
    public class LitigeController : BaseController
    {
        private readonly ILitigeService    _litiges;
        private readonly IExclusionService _exclusions;
        private readonly IMembreService    _membres;

        public LitigeController(
            ILitigeService litigesSvc,
            IExclusionService exclusionsSvc,
            IMembreService membresSvc)
        {
            _litiges    = litigesSvc;
            _exclusions = exclusionsSvc;
            _membres    = membresSvc;
        }

        public async Task<IActionResult> Index()
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Litiges & Exclusions", isActive: true)
            );
            var vm = new LitigesIndexViewModel
            {
                Litiges    = await _litiges.GetByCycleAsync(CycleId),
                Exclusions = await _exclusions.GetByCycleAsync(CycleId)
            };
            return View(vm);
        }

        // --- Litiges ---

        public async Task<IActionResult> SignalerLitige()
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Litiges & Exclusions", "Litige", "Index"),
                BreadcrumbItem("Signaler un litige", isActive: true)
            );
            await ChargerMembresSelectList();
            return View(new LitigeViewModel { IdCycle = CycleId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignalerLitige(LitigeViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                await ChargerMembresSelectList();
                return View(vm);
            }
            vm.IdCycle = CycleId;
            await _litiges.CreateAsync(vm);
            TempData["Success"] = "Litige signalé avec succès.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Resoudre(int id, string resolution)
        {
            await _litiges.ResoudreAsync(id, resolution);
            TempData["Success"] = "Litige marqué comme résolu.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Classer(int id)
        {
            await _litiges.ClasserAsync(id);
            TempData["Success"] = "Litige classé sans suite.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SupprimerLitige(int id)
        {
            await _litiges.DeleteAsync(id);
            TempData["Success"] = "Litige supprimé.";
            return RedirectToAction(nameof(Index));
        }

        // --- Exclusions ---

        public async Task<IActionResult> ExclureMembre(int? idLitige = null)
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Litiges & Exclusions", "Litige", "Index"),
                BreadcrumbItem("Exclure un membre", isActive: true)
            );
            await ChargerMembresSelectList();

            // Pré-remplir depuis un litige si fourni
            LitigeViewModel? litige = null;
            if (idLitige.HasValue)
                litige = await _litiges.GetByIdAsync(idLitige.Value);

            return View(new ExclusionViewModel
            {
                IdCycle  = CycleId,
                IdMembre = litige?.IdMembreAccuse ?? 0,
                Motif    = litige != null ? $"Suite au litige : {litige.Nature}" : "",
                IdLitige = idLitige
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExclureMembre(ExclusionViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                await ChargerMembresSelectList();
                return View(vm);
            }
            vm.IdCycle = CycleId;
            await _exclusions.CreateAsync(vm);
            TempData["Success"] = "Membre exclu avec succès.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reintegrer(int id)
        {
            await _exclusions.ReintegrerAsync(id);
            TempData["Success"] = "Membre réintégré.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SupprimerExclusion(int id)
        {
            await _exclusions.DeleteAsync(id);
            TempData["Success"] = "Exclusion supprimée.";
            return RedirectToAction(nameof(Index));
        }

        // ---

        private async Task ChargerMembresSelectList()
        {
            var membres = await _membres.GetAllAsync();
            ViewBag.Membres = new SelectList(
                membres.Select(m => new { m.IdMembre, Nom = $"{m.Prenom} {m.Nom}" }),
                "IdMembre", "Nom"
            );
        }
    }
}
