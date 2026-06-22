using Microsoft.AspNetCore.Mvc;
using tontine.MVC.Filters;
using tontine.MVC.Models;
using tontine.MVC.Services;

namespace tontine.MVC.Controllers
{
    public class PosteController : BaseController
    {
        private readonly IPosteService _service;

        public PosteController(IPosteService service) => _service = service;

        public async Task<IActionResult> Index()
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Paramètres"),
                BreadcrumbItem("Postes", isActive: true)
            );
            var postes = await _service.GetAllAsync();
            return View(postes);
        }

        public IActionResult Create()
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Paramètres"),
                BreadcrumbItem("Postes", "Poste", "Index"),
                BreadcrumbItem("Ajouter", isActive: true)
            );
            return View(new PosteViewModel());
        }

        [RoleAuthorize("Administrateur", "Gestionnaire")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PosteViewModel poste)
        {
            if (!ModelState.IsValid) return View(poste);
            var ok = await _service.CreateAsync(poste);
            if (!ok) ModelState.AddModelError("", "Erreur lors de la création.");
            return ok ? RedirectToAction(nameof(Index)) : View(poste);
        }

        public async Task<IActionResult> Edit(int id)
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Paramètres"),
                BreadcrumbItem("Postes", "Poste", "Index"),
                BreadcrumbItem("Modifier", isActive: true)
            );
            var poste = await _service.GetByIdAsync(id);
            return poste == null ? NotFound() : View(poste);
        }

        [RoleAuthorize("Administrateur", "Gestionnaire")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PosteViewModel poste)
        {
            if (!ModelState.IsValid) return View(poste);
            var ok = await _service.UpdateAsync(id, poste);
            if (!ok) ModelState.AddModelError("", "Erreur lors de la modification.");
            return ok ? RedirectToAction(nameof(Index)) : View(poste);
        }

        public async Task<IActionResult> Delete(int id)
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Paramètres"),
                BreadcrumbItem("Postes", "Poste", "Index"),
                BreadcrumbItem("Supprimer", isActive: true)
            );
            var poste = await _service.GetByIdAsync(id);
            return poste == null ? NotFound() : View(poste);
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