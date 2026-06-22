using Microsoft.AspNetCore.Mvc;
using tontine.MVC.Filters;
using tontine.MVC.Models;
using tontine.MVC.Services;

namespace tontine.MVC.Controllers
{
    public class PretController : BaseController
    {
        private readonly IPretService _service;
        private readonly IMembreService _membreService;

        public PretController(IPretService service, IMembreService membreService)
        {
            _service = service;
            _membreService = membreService;
        }

        public async Task<IActionResult> Index()
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Paramètres"),
                BreadcrumbItem("Prêts", isActive: true)
            );
            var prets = await _service.GetAllAsync();
            return View(prets);
        }

        public async Task<IActionResult> Create()
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Paramètres"),
                BreadcrumbItem("Prêts", "Pret", "Index"),
                BreadcrumbItem("Ajouter", isActive: true)
            );
            var model = new PretViewModel { DatePret = DateTime.Now, DateRemboursement = DateTime.Now.AddMonths(3) };
            await LoadDropdowns();
            return View(model);
        }

        [RoleAuthorize("Administrateur", "Gestionnaire")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PretViewModel pret)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                return View(pret);
            }

            if (pret.DateRemboursement <= pret.DatePret)
            {
                ModelState.AddModelError("DateRemboursement", "La date de remboursement doit être ultérieure à la date de prêt.");
                await LoadDropdowns();
                return View(pret);
            }

            var (ok, erreur) = await _service.CreateWithErrorAsync(pret);
            if (!ok)
            {
                ModelState.AddModelError("", erreur ?? "Erreur lors de la création du prêt.");
                await LoadDropdowns();
                return View(pret);
            }

            TempData["Success"] = "Prêt créé avec succès.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Paramètres"),
                BreadcrumbItem("Prêts", "Pret", "Index"),
                BreadcrumbItem("Modifier", isActive: true)
            );
            var pret = await _service.GetByIdAsync(id);
            if (pret == null) return NotFound();
            await LoadDropdowns();
            return View(pret);
        }

        [RoleAuthorize("Administrateur", "Gestionnaire")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PretViewModel pret)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                return View(pret);
            }

            if (pret.DateRemboursement <= pret.DatePret)
            {
                ModelState.AddModelError("DateRemboursement", "La date de remboursement doit être ultérieure à la date de prêt.");
                await LoadDropdowns();
                return View(pret);
            }

            var ok = await _service.UpdateAsync(id, pret);
            if (!ok)
            {
                ModelState.AddModelError("", "Erreur lors de la modification du prêt.");
                await LoadDropdowns();
                return View(pret);
            }

            TempData["Success"] = "Prêt modifié avec succès.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Paramètres"),
                BreadcrumbItem("Prêts", "Pret", "Index"),
                BreadcrumbItem("Supprimer", isActive: true)
            );
            var pret = await _service.GetByIdAsync(id);
            return pret == null ? NotFound() : View(pret);
        }

        [RoleAuthorize("Administrateur", "Gestionnaire")]

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _service.DeleteAsync(id);
            TempData["Success"] = "Prêt supprimé avec succès.";
            return RedirectToAction(nameof(Index));
        }

        [RoleAuthorize("Administrateur", "Gestionnaire")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approuver(int id)
        {
            var ok = await _service.ApprouverAsync(id);
            TempData[ok ? "Success" : "Error"] = ok ? "Prêt approuvé." : "Impossible d'approuver ce prêt.";
            return RedirectToAction(nameof(Index));
        }

        [RoleAuthorize("Administrateur", "Gestionnaire")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Rembourser(int id)
        {
            var ok = await _service.RemboursaAsync(id);
            TempData[ok ? "Success" : "Error"] = ok ? "Prêt marqué comme remboursé." : "Impossible de marquer ce prêt.";
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadDropdowns()
        {
            var membres = await _membreService.GetAllAsync();
            ViewData["Membres"] = membres;
        }
    }
}
