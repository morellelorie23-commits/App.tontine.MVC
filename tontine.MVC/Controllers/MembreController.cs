using Microsoft.AspNetCore.Mvc;
using tontine.MVC.Models;
using tontine.MVC.Services;

namespace tontine.MVC.Controllers
{
    public class MembreController : Controller
    {
        private readonly IMembreService _service;
        private readonly IWebHostEnvironment _env;

        public MembreController(IMembreService service, IWebHostEnvironment env)
        {
            _service = service;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var membres = await _service.GetAllAsync();
            return View(membres);
        }

        public IActionResult Create() => View(new MembreViewModel());

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
            var membre = await _service.GetByIdAsync(id);
            return membre == null ? NotFound() : View(membre);
        }

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

        public async Task<IActionResult> Delete(int id)
        {
            var membre = await _service.GetByIdAsync(id);
            return membre == null ? NotFound() : View(membre);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}