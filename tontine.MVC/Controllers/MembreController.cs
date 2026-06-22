using Microsoft.AspNetCore.Mvc;
using tontine.MVC.Filters;
using tontine.MVC.Models;
using tontine.MVC.Services;

namespace tontine.MVC.Controllers
{
    public class MembreController : BaseController
    {
        private readonly IMembreService _service;
        private readonly IWebHostEnvironment _env;
        private readonly PdfService _pdfService;

        public MembreController(IMembreService service, IWebHostEnvironment env, PdfService pdfService)
        {
            _service = service;
            _env = env;
            _pdfService = pdfService;
        }

        public async Task<IActionResult> Index()
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Paramètres"),
                BreadcrumbItem("Membres", isActive: true)
            );
            var membres = await _service.GetAllAsync();
            var scores  = await _service.GetAllScoresAsync();
            ViewBag.Scores = scores.ToDictionary(s => s.IdMembre);
            return View(membres);
        }

        public IActionResult Create()
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Paramètres"),
                BreadcrumbItem("Membres", "Membre", "Index"),
                BreadcrumbItem("Ajouter", isActive: true)
            );
            return View(new MembreViewModel());
        }

        [RoleAuthorize("Administrateur", "Gestionnaire")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MembreViewModel membre)
        {
            if (membre.PhotoFile != null && membre.PhotoFile.Length > 0)
            {
                var uploadsDir = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsDir))
                    Directory.CreateDirectory(uploadsDir);
                var fileName = Guid.NewGuid().ToString() +
                               Path.GetExtension(membre.PhotoFile.FileName);
                var filePath = Path.Combine(uploadsDir, fileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await membre.PhotoFile.CopyToAsync(stream);
                membre.Photo = fileName;
            }
            ModelState.Remove("PhotoFile");
            ModelState.Remove("Photo");
            if (!ModelState.IsValid) return View(membre);
            var ok = await _service.CreateAsync(membre);
            if (!ok) ModelState.AddModelError("", "Erreur lors de la création.");
            return ok ? RedirectToAction(nameof(Index)) : View(membre);
        }

        public async Task<IActionResult> Edit(int id)
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Paramètres"),
                BreadcrumbItem("Membres", "Membre", "Index"),
                BreadcrumbItem("Modifier", isActive: true)
            );
            var membre = await _service.GetByIdAsync(id);
            return membre == null ? NotFound() : View(membre);
        }

        [RoleAuthorize("Administrateur", "Gestionnaire")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MembreViewModel membre)
        {
            if (membre.PhotoFile != null && membre.PhotoFile.Length > 0)
            {
                var uploadsDir = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsDir))
                    Directory.CreateDirectory(uploadsDir);
                var fileName = Guid.NewGuid().ToString() +
                               Path.GetExtension(membre.PhotoFile.FileName);
                var filePath = Path.Combine(uploadsDir, fileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await membre.PhotoFile.CopyToAsync(stream);
                membre.Photo = fileName;
            }
            ModelState.Remove("PhotoFile");
            ModelState.Remove("Photo");
            if (!ModelState.IsValid) return View(membre);
            var ok = await _service.UpdateAsync(id, membre);
            if (!ok) ModelState.AddModelError("", "Erreur lors de la modification.");
            return ok ? RedirectToAction(nameof(Index)) : View(membre);
        }

        public async Task<IActionResult> Releve(int id)
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Paramètres"),
                BreadcrumbItem("Membres", "Membre", "Index"),
                BreadcrumbItem("Relevé", isActive: true)
            );
            var releve = await _service.GetReleveAsync(id);
            return releve == null ? NotFound() : View(releve);
        }

        public async Task<IActionResult> ExportPdf(int id)
        {
            var releve = await _service.GetReleveAsync(id);
            if (releve == null) return NotFound();

            var pdfBytes = _pdfService.GenererReleveMembrePdf(releve);
            var fileName = $"releve_{releve.Membre.Nom}_{releve.Membre.Prenom}_{DateTime.Now:yyyyMMdd}.pdf";
            return File(pdfBytes, "application/pdf", fileName);
        }

        public async Task<IActionResult> Delete(int id)
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Paramètres"),
                BreadcrumbItem("Membres", "Membre", "Index"),
                BreadcrumbItem("Supprimer", isActive: true)
            );
            var membre = await _service.GetByIdAsync(id);
            return membre == null ? NotFound() : View(membre);
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