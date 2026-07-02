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
                if (!IsValidImageFile(membre.PhotoFile, out var err))
                { ModelState.AddModelError("PhotoFile", err); return View(membre); }
                var uploadsDir = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsDir)) Directory.CreateDirectory(uploadsDir);
                var ext = Path.GetExtension(membre.PhotoFile.FileName).ToLower();
                var fileName = Guid.NewGuid().ToString() + ext;
                using var stream = new FileStream(Path.Combine(uploadsDir, fileName), FileMode.Create);
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
                if (!IsValidImageFile(membre.PhotoFile, out var err))
                { ModelState.AddModelError("PhotoFile", err); return View(membre); }
                var uploadsDir = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsDir)) Directory.CreateDirectory(uploadsDir);
                var ext = Path.GetExtension(membre.PhotoFile.FileName).ToLower();
                var fileName = Guid.NewGuid().ToString() + ext;
                using var stream = new FileStream(Path.Combine(uploadsDir, fileName), FileMode.Create);
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

        private static readonly string[] _allowedImageMimes = ["image/jpeg", "image/png", "image/gif", "image/webp"];
        private static readonly string[] _allowedImageExts  = [".jpg", ".jpeg", ".png", ".gif", ".webp"];

        private static bool IsValidImageFile(IFormFile file, out string error)
        {
            error = "";
            if (file.Length > 5 * 1024 * 1024)
            { error = "La photo ne doit pas dépasser 5 Mo."; return false; }
            if (!_allowedImageMimes.Contains(file.ContentType.ToLower()))
            { error = "Type de fichier non autorisé (JPG, PNG, GIF ou WEBP uniquement)."; return false; }
            var ext = Path.GetExtension(file.FileName).ToLower();
            if (!_allowedImageExts.Contains(ext))
            { error = "Extension de fichier non autorisée."; return false; }
            return true;
        }
    }
}