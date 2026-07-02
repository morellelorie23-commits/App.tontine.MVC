using Microsoft.AspNetCore.Mvc;
using tontine.MVC.Models;
using tontine.MVC.Services;

namespace tontine.MVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly ICompteService _compteService;
        private readonly ICycleService _cycleService;

        public AuthController(ICompteService compteService, ICycleService cycleService)
        {
            _compteService = compteService;
            _cycleService  = cycleService;
        }

        [HttpGet]
        public async Task<IActionResult> Login(string? returnUrl = null)
        {
            if (HttpContext.Session.GetString("user_id") != null)
                return RedirectToAction("Index", "Home");
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["Cycles"] = await _cycleService.GetAllAsync();
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel vm, string? returnUrl = null)
        {
            // Valider uniquement les identifiants (le cycle est optionnel pour les membres)
            if (string.IsNullOrWhiteSpace(vm.Nom) || string.IsNullOrWhiteSpace(vm.MotDePasse))
            {
                ModelState.AddModelError("", "Nom d'utilisateur et mot de passe requis.");
                ViewData["Cycles"] = await _cycleService.GetAllAsync();
                return View(vm);
            }

            var user = await _compteService.LoginAsync(vm.Nom, vm.MotDePasse);
            if (user == null)
            {
                ModelState.AddModelError("", "Nom d'utilisateur ou mot de passe incorrect.");
                ViewData["Cycles"] = await _cycleService.GetAllAsync();
                return View(vm);
            }

            // Résolution du cycle selon le rôle
            CycleViewModel? cycle;
            if (user.Role == "Membre")
            {
                // Les membres n'ont pas besoin de choisir un cycle — on prend l'actif automatiquement
                var cycles = await _cycleService.GetAllAsync();
                cycle = vm.IdCycle.HasValue
                    ? cycles.FirstOrDefault(c => c.IdCycle == vm.IdCycle.Value)
                    : cycles.FirstOrDefault(c => c.Statut == "Actif")
                      ?? cycles.OrderByDescending(c => c.DateDebut).FirstOrDefault();
            }
            else
            {
                if (!vm.IdCycle.HasValue)
                {
                    ModelState.AddModelError("IdCycle", "Veuillez sélectionner un cycle de travail.");
                    ViewData["Cycles"] = await _cycleService.GetAllAsync();
                    return View(vm);
                }
                cycle = await _cycleService.GetByIdAsync(vm.IdCycle.Value);
            }

            if (cycle == null)
            {
                ModelState.AddModelError("IdCycle", "Aucun cycle disponible. Contactez l'administrateur.");
                ViewData["Cycles"] = await _cycleService.GetAllAsync();
                return View(vm);
            }

            HttpContext.Session.SetString("user_id",    user.IdCompte.ToString());
            HttpContext.Session.SetString("user_nom",   user.Nom + " " + user.Prenom);
            HttpContext.Session.SetString("user_role",  user.Role);
            HttpContext.Session.SetString("user_email", user.Email);
            HttpContext.Session.SetString("user_photo", user.Photo ?? "");
            HttpContext.Session.SetString("cycle_id",   cycle.IdCycle.ToString());
            HttpContext.Session.SetString("cycle_nom",  cycle.NomCycle ?? "");

            if (user.Role == "Membre" && user.IdMembre.HasValue)
            {
                HttpContext.Session.SetString("membre_id", user.IdMembre.Value.ToString());
                return RedirectToAction("Index", "PortailMembre");
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return LocalRedirect(returnUrl);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        public IActionResult Refuse()
        {
            return View();
        }
    }
}
