using Microsoft.AspNetCore.Mvc;
using tontine.MVC.Services;

namespace tontine.MVC.Controllers
{
    public class StatistiqueController : BaseController
    {
        private readonly IStatistiqueService _stats;

        public StatistiqueController(IStatistiqueService stats)
        {
            _stats = stats;
        }

        public async Task<IActionResult> Index()
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Statistiques", isActive: true)
            );
            var vm = await _stats.GetStatsAsync();
            return View(vm);
        }
    }
}
