using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using tontine.MVC.Filters;
using tontine.MVC.Models;
using tontine.MVC.Services;

namespace tontine.MVC.Controllers
{
    [RoleAuthorize("Administrateur", "Gestionnaire")]
    public class FondsSolidariteController : BaseController
    {
        private readonly IFondsSolidariteService  _fonds;
        private readonly IContributionFondsService _contributions;
        private readonly IDemandeAideService       _demandes;
        private readonly IMembreService            _membres;
        private readonly ICycleService             _cycles;

        public FondsSolidariteController(
            IFondsSolidariteService fondsSvc,
            IContributionFondsService contributionsSvc,
            IDemandeAideService demandesSvc,
            IMembreService membresSvc,
            ICycleService cyclesSvc)
        {
            _fonds         = fondsSvc;
            _contributions = contributionsSvc;
            _demandes      = demandesSvc;
            _membres       = membresSvc;
            _cycles        = cyclesSvc;
        }

        public async Task<IActionResult> Index()
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Fonds de solidarité", isActive: true)
            );
            var list = await _fonds.GetAllAsync();
            return View(list.Where(f => f.IdCycle == CycleId).ToList());
        }

        public IActionResult Create()
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Fonds de solidarité", "FondsSolidarite", "Index"),
                BreadcrumbItem("Nouveau fonds", isActive: true)
            );
            return View(new FondsSolidariteViewModel { IdCycle = CycleId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FondsSolidariteViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);
            vm.IdCycle = CycleId;
            await _fonds.CreateAsync(vm);
            TempData["Success"] = "Fonds de solidarité créé avec succès.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Detail(int id)
        {
            var fonds = await _fonds.GetByIdAsync(id);
            if (fonds == null) return NotFound();

            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Fonds de solidarité", "FondsSolidarite", "Index"),
                BreadcrumbItem(fonds.Nom, isActive: true)
            );

            var contributions = await _contributions.GetByFondsAsync(id);
            var demandes      = await _demandes.GetByFondsAsync(id);

            return View(new FondsDetailViewModel
            {
                Fonds         = fonds,
                Contributions = contributions,
                Demandes      = demandes
            });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var fonds = await _fonds.GetByIdAsync(id);
            if (fonds == null) return NotFound();

            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Fonds de solidarité", "FondsSolidarite", "Index"),
                BreadcrumbItem("Modifier", isActive: true)
            );
            return View(fonds);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(FondsSolidariteViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);
            await _fonds.UpdateAsync(vm);
            TempData["Success"] = "Fonds mis à jour.";
            return RedirectToAction(nameof(Detail), new { id = vm.IdFonds });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _fonds.DeleteAsync(id);
            TempData["Success"] = "Fonds supprimé.";
            return RedirectToAction(nameof(Index));
        }

        // --- Contributions ---

        public async Task<IActionResult> AjouterContribution(int idFonds)
        {
            var fonds = await _fonds.GetByIdAsync(idFonds);
            if (fonds == null) return NotFound();

            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Fonds de solidarité", "FondsSolidarite", "Index"),
                BreadcrumbItem(fonds.Nom, "FondsSolidarite", "Detail", false),
                BreadcrumbItem("Ajouter contribution", isActive: true)
            );

            await ChargerMembresSelectList();

            return View(new ContributionFondsViewModel
            {
                IdFonds = idFonds,
                Montant = fonds.MontantParMembre
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AjouterContribution(ContributionFondsViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                await ChargerMembresSelectList();
                return View(vm);
            }
            await _contributions.CreateAsync(vm);
            TempData["Success"] = "Contribution enregistrée.";
            return RedirectToAction(nameof(Detail), new { id = vm.IdFonds });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SupprimerContribution(int id, int idFonds)
        {
            await _contributions.DeleteAsync(id);
            TempData["Success"] = "Contribution supprimée.";
            return RedirectToAction(nameof(Detail), new { id = idFonds });
        }

        // --- Demandes d'aide ---

        public async Task<IActionResult> NouvelleDemandeAide(int idFonds)
        {
            var fonds = await _fonds.GetByIdAsync(idFonds);
            if (fonds == null) return NotFound();

            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Fonds de solidarité", "FondsSolidarite", "Index"),
                BreadcrumbItem(fonds.Nom, "FondsSolidarite", "Detail", false),
                BreadcrumbItem("Nouvelle demande d'aide", isActive: true)
            );

            await ChargerMembresSelectList();
            ViewBag.SoldeDisponible = fonds.Solde;

            return View(new DemandeAideViewModel { IdFonds = idFonds });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NouvelleDemandeAide(DemandeAideViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                await ChargerMembresSelectList();
                return View(vm);
            }
            await _demandes.CreateAsync(vm);
            TempData["Success"] = "Demande d'aide soumise.";
            return RedirectToAction(nameof(Detail), new { id = vm.IdFonds });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approuver(int idDemande, int idFonds, decimal montantAccorde, string? notes)
        {
            await _demandes.ApprouverAsync(idDemande, montantAccorde, notes);
            TempData["Success"] = "Demande approuvée.";
            return RedirectToAction(nameof(Detail), new { id = idFonds });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Rejeter(int idDemande, int idFonds, string? notes)
        {
            await _demandes.RejeterAsync(idDemande, notes);
            TempData["Success"] = "Demande rejetée.";
            return RedirectToAction(nameof(Detail), new { id = idFonds });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarquerPaye(int idDemande, int idFonds)
        {
            await _demandes.MarquerPayeAsync(idDemande);
            TempData["Success"] = "Demande marquée comme payée.";
            return RedirectToAction(nameof(Detail), new { id = idFonds });
        }

        // ---

        private async Task ChargerMembresSelectList()
        {
            var membres = await _membres.GetAllAsync();
            ViewBag.Membres = new SelectList(
                membres.Select(m => new { m.IdMembre, Nom = $"{m.Prenom} {m.Nom}" }),
                "IdMembre", "Nom"
            );
        }
    }
}
