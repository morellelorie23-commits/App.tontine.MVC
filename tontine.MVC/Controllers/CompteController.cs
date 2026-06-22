using Microsoft.AspNetCore.Mvc;
using tontine.MVC.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using tontine.MVC.Models;
using tontine.MVC.Services;

namespace tontine.MVC.Controllers
{
    public class CompteController : BaseController
    {
        private readonly ICompteService _service;
        private readonly IWebHostEnvironment _env;

        public CompteController(ICompteService service, IWebHostEnvironment env)
        {
            _service = service;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Sécurité"),
                BreadcrumbItem("Comptes utilisateurs", isActive: true)
            );
            var list = await _service.GetAllAsync();
            return View(list);
        }

        public IActionResult Create()
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Sécurité"),
                BreadcrumbItem("Comptes utilisateurs", "Compte", "Index"),
                BreadcrumbItem("Ajouter", isActive: true)
            );
            PopulateDropdowns();
            return View(new CompteUtilisateurViewModel
            {
                DateCreation = DateOnly.FromDateTime(DateTime.Now)
            });
        }

        [RoleAuthorize("Administrateur")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CompteUtilisateurViewModel vm)
        {
            if (!ModelState.IsValid) { PopulateDropdowns(); return View(vm); }
            var ok = await _service.CreateAsync(vm);
            if (!ok) { ModelState.AddModelError("", "Erreur lors de l'enregistrement."); PopulateDropdowns(); return View(vm); }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Sécurité"),
                BreadcrumbItem("Comptes utilisateurs", "Compte", "Index"),
                BreadcrumbItem("Modifier", isActive: true)
            );
            var vm = await _service.GetByIdAsync(id);
            if (vm == null) return NotFound();
            PopulateDropdowns();
            return View(vm);
        }

        [RoleAuthorize("Administrateur")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CompteUtilisateurViewModel vm)
        {
            if (vm.PhotoFile != null && vm.PhotoFile.Length > 0)
            {
                var uploadsDir = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsDir);
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(vm.PhotoFile.FileName);
                using var stream = new FileStream(Path.Combine(uploadsDir, fileName), FileMode.Create);
                await vm.PhotoFile.CopyToAsync(stream);
                vm.Photo = fileName;

                if (HttpContext.Session.GetString("user_id") == vm.IdCompte.ToString())
                    HttpContext.Session.SetString("user_photo", fileName);
            }
            ModelState.Remove("PhotoFile");
            ModelState.Remove("Photo");
            if (!ModelState.IsValid) { PopulateDropdowns(); return View(vm); }
            var ok = await _service.UpdateAsync(vm);
            if (!ok) { ModelState.AddModelError("", "Erreur lors de la modification."); PopulateDropdowns(); return View(vm); }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Sécurité"),
                BreadcrumbItem("Comptes utilisateurs", "Compte", "Index"),
                BreadcrumbItem("Supprimer", isActive: true)
            );
            var vm = await _service.GetByIdAsync(id);
            return vm == null ? NotFound() : View(vm);
        }

        [RoleAuthorize("Administrateur")]

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Mon compte"),
                BreadcrumbItem("Changer le mot de passe", isActive: true)
            );
            return View(new ChangePasswordViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var idStr = HttpContext.Session.GetString("user_id");
            if (!int.TryParse(idStr, out int id)) return RedirectToAction("Login", "Auth");

            var compte = await _service.GetByIdAsync(id);
            if (compte == null) return NotFound();

            var (ok, erreur) = await _service.ChangePasswordAsync(id, vm.AncienMotDePasse, vm.NouveauMotDePasse);
            if (!ok)
            {
                ModelState.AddModelError("", erreur ?? "Mot de passe actuel incorrect.");
                return View(vm);
            }

            TempData["Success"] = "Mot de passe modifié avec succès.";
            return RedirectToAction("Index", "Home");
        }

        private void PopulateDropdowns()
        {
            ViewBag.Roles = new List<SelectListItem>
            {
                new SelectListItem("Administrateur", "Administrateur"),
                new SelectListItem("Gestionnaire",   "Gestionnaire"),
                new SelectListItem("Lecteur",         "Lecteur"),
            };
            ViewBag.Statuts = new List<SelectListItem>
            {
                new SelectListItem("Actif",    "Actif"),
                new SelectListItem("Inactif",  "Inactif"),
                new SelectListItem("Suspendu", "Suspendu"),
            };
        }
    }
}
