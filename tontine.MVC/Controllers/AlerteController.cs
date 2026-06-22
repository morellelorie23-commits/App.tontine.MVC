using Microsoft.AspNetCore.Mvc;
using tontine.MVC.Filters;
using tontine.MVC.Services;

namespace tontine.MVC.Controllers
{
    public class AlerteController : BaseController
    {
        private readonly IAlertePretService _alertePret;
        private readonly IAlerteCotisationService _alerteCotisation;

        public AlerteController(IAlertePretService alertePret, IAlerteCotisationService alerteCotisation)
        {
            _alertePret = alertePret;
            _alerteCotisation = alerteCotisation;
        }

        // Envoyer alertes prêts en retard
        [RoleAuthorize("Administrateur", "Gestionnaire")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnvoyerAlertesRetard()
        {
            var result = await _alertePret.EnvoyerAlertesPretsEnRetardAsync();

            if (result.PretsEnRetard == 0)
                TempData["Success"] = "Aucun pret en retard - aucune alerte a envoyer.";
            else
                TempData["AlerteResult"] = $"{result.EmailsEnvoyes} email(s) envoye(s) sur {result.PretsEnRetard} pret(s) en retard.";

            return RedirectToAction("Index", "Pret");
        }

        // Envoyer alertes cotisations en retard
        [RoleAuthorize("Administrateur", "Gestionnaire")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnvoyerAlertesCotisations()
        {
            var result = await _alerteCotisation.EnvoyerAlertesCotisationsEnRetardAsync();

            if (result.CotisationsEnRetard == 0)
                TempData["Success"] = "Aucune cotisation en retard - aucune alerte a envoyer.";
            else
                TempData["AlerteResult"] = $"{result.EmailsEnvoyes} email(s) envoye(s) sur {result.CotisationsEnRetard} cotisation(s) en retard.";

            return RedirectToAction("Index", "Cotisation");
        }

        // Preview : compte seulement, n'envoie pas
        [HttpGet]
        public async Task<IActionResult> PreviewAlertePrets()
        {
            var prets = await _alertePret.CompterPretsEnRetardAsync();
            return Json(new { pretsEnRetard = prets });
        }

        [HttpGet]
        public async Task<IActionResult> PreviewAlerteCotisations()
        {
            var cotis = await _alerteCotisation.CompterCotisationsEnRetardAsync();
            return Json(new { cotisationsEnRetard = cotis });
        }
    }
}
