using Microsoft.AspNetCore.Mvc;
using tontine.MVC.Models;
using tontine.MVC.Services;

namespace tontine.MVC.Controllers
{
    public class PosteController : Controller
    {
        private readonly IPosteService _service;

        public PosteController(IPosteService service) => _service = service;

        public async Task<IActionResult> Index()
        {
            var postes = await _service.GetAllAsync();
            return View(postes);
        }

        public IActionResult Create() => View(new PosteViewModel());

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
            var poste = await _service.GetByIdAsync(id);
            return poste == null ? NotFound() : View(poste);
        }

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
            var poste = await _service.GetByIdAsync(id);
            return poste == null ? NotFound() : View(poste);
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