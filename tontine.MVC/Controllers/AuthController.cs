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
            if (!ModelState.IsValid)
            {
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

            var cycle = await _cycleService.GetByIdAsync(vm.IdCycle!.Value);
            if (cycle == null)
            {
                ModelState.AddModelError("IdCycle", "Cycle introuvable.");
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

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
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
