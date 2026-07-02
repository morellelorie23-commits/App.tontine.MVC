using Microsoft.AspNetCore.Mvc;
using tontine.MVC.Services;
using tontine.MVC.Filters;

namespace tontine.MVC.Controllers
{
    public class NotificationController : BaseController
    {
        private readonly INotificationService _service;

        public NotificationController(INotificationService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Notifications", isActive: true)
            );

            var notifs = await _service.GetAllAsync();
            await _service.MarquerToutesLuesAsync();
            return View(notifs);
        }

        [HttpPost]
        [RoleAuthorize("Administrateur", "Gestionnaire")]
        public async Task<IActionResult> MarquerLue(int id)
        {
            await _service.MarquerLueAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [RoleAuthorize("Administrateur", "Gestionnaire")]
        public async Task<IActionResult> MarquerToutesLues()
        {
            await _service.MarquerToutesLuesAsync();
            TempData["Success"] = "Toutes les notifications ont été marquées comme lues.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [RoleAuthorize("Administrateur")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            TempData["Success"] = "Notification supprimée.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [RoleAuthorize("Administrateur")]
        public async Task<IActionResult> SupprimerLues()
        {
            await _service.SupprimerLuesAsync();
            TempData["Success"] = "Notifications lues supprimées.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Count()
        {
            var count = await _service.GetNonLuesCountAsync();
            return Json(new { count });
        }
    }
}
