using Microsoft.AspNetCore.Mvc;
using tontine.MVC.Models;
using tontine.MVC.Services;

namespace tontine.MVC.Controllers
{
    public class CycleController : Controller
    {
        private readonly ICycleService _service;

        public CycleController(ICycleService service) => _service = service;

        public async Task<IActionResult> Index()
        {
            var cycles = await _service.GetAllAsync();
            return View(cycles);
        }

        public IActionResult Create() => View(new CycleViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CycleViewModel cycle)
        {
            if (!ModelState.IsValid) return View(cycle);
            var ok = await _service.CreateAsync(cycle);
            if (!ok) ModelState.AddModelError("", "Erreur lors de la création.");
            return ok ? RedirectToAction(nameof(Index)) : View(cycle);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var cycle = await _service.GetByIdAsync(id);
            return cycle == null ? NotFound() : View(cycle);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CycleViewModel cycle)
        {
            if (!ModelState.IsValid) return View(cycle);
            var ok = await _service.UpdateAsync(id, cycle);
            if (!ok) ModelState.AddModelError("", "Erreur lors de la modification.");
            return ok ? RedirectToAction(nameof(Index)) : View(cycle);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var cycle = await _service.GetByIdAsync(id);
            return cycle == null ? NotFound() : View(cycle);
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