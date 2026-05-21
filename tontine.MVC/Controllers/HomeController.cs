using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using tontine.MVC.Models;
using tontine.MVC.Services;

namespace tontine.MVC.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IStatistiqueService _stats;
        private readonly ITontineService     _tontines;

        public HomeController(IStatistiqueService stats, ITontineService tontines)
        {
            _stats    = stats;
            _tontines = tontines;
        }

        public async Task<IActionResult> Index()
        {
            SetBreadcrumbs(BreadcrumbItem("Tableau de bord", isActive: true));
            var vm       = await _stats.GetStatsAsync();
            var tontines = await _tontines.GetAllAsync();
            ViewBag.Tontines = tontines;
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> AlerteCount()
        {
            var vm = await _stats.GetStatsAsync();
            return Json(new { vm.PretsEnRetard, vm.CotisationsEnRetard, vm.CotisationsEnAttente });
        }

        [HttpGet]
        public async Task<IActionResult> TontineStats(int id)
        {
            var result = await _stats.GetStatsTontineAsync(id);
            return Json(result);
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
            => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
