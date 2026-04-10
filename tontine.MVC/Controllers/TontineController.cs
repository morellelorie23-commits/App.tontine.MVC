using Microsoft.AspNetCore.Mvc;
using tontine.MVC.Models;
using tontine.MVC.Services;

namespace tontine.MVC.Controllers
{
    public class TontineController : Controller
    {
        private readonly ITontineService _service;

        public TontineController(ITontineService service) => _service = service;

        public async Task<IActionResult> Index()
        {
            var tontines = await _service.GetAllAsync();
            return View(tontines);
        }

        public IActionResult Create() => View(new TontineViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TontineViewModel tontine)
        {
            if (!ModelState.IsValid) return View(tontine);
            var ok = await _service.CreateAsync(tontine);
            if (!ok) ModelState.AddModelError("", "Erreur lors de la création.");
            return ok ? RedirectToAction(nameof(Index)) : View(tontine);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var tontine = await _service.GetByIdAsync(id);
            return tontine == null ? NotFound() : View(tontine);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TontineViewModel tontine)
        {
            if (!ModelState.IsValid) return View(tontine);
            var ok = await _service.UpdateAsync(id, tontine);
            if (!ok) ModelState.AddModelError("", "Erreur lors de la modification.");
            return ok ? RedirectToAction(nameof(Index)) : View(tontine);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var tontine = await _service.GetByIdAsync(id);
            return tontine == null ? NotFound() : View(tontine);
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