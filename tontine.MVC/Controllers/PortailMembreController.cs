using Microsoft.AspNetCore.Mvc;
using tontine.MVC.Models;
using tontine.MVC.Services;

namespace tontine.MVC.Controllers
{
    public class PortailMembreController : Controller
    {
        private readonly IMembreService _membreService;
        private readonly ICotisationService _cotisationService;
        private readonly IVersementService _versementService;
        private readonly IPretService _pretService;
        private readonly IMembreCycleTontineService _mctService;
        private readonly IFondsSolidariteService _fondsService;
        private readonly IDemandeAideService _demandeService;
        private readonly ITontineService _tontineService;
        private readonly IReunionService _reunionService;
        private readonly PdfService _pdf;

        public PortailMembreController(
            IMembreService membreService,
            ICotisationService cotisationService,
            IVersementService versementService,
            IPretService pretService,
            IMembreCycleTontineService mctService,
            IFondsSolidariteService fondsService,
            IDemandeAideService demandeService,
            ITontineService tontineService,
            IReunionService reunionService,
            PdfService pdf)
        {
            _membreService      = membreService;
            _cotisationService  = cotisationService;
            _versementService   = versementService;
            _pretService        = pretService;
            _mctService         = mctService;
            _fondsService       = fondsService;
            _demandeService     = demandeService;
            _tontineService     = tontineService;
            _reunionService     = reunionService;
            _pdf                = pdf;
        }

        // Vérifie que l'utilisateur est connecté en tant que Membre
        private (bool ok, IActionResult? redirect, int membreId) CheckMembre()
        {
            var userId   = HttpContext.Session.GetString("user_id");
            var role     = HttpContext.Session.GetString("user_role");
            var membreId = HttpContext.Session.GetString("membre_id");

            if (userId == null)
                return (false, RedirectToAction("Login", "Auth"), 0);
            if (role != "Membre" || membreId == null)
                return (false, RedirectToAction("Index", "Home"), 0);

            return (true, null, int.Parse(membreId));
        }

        private void SetViewBag()
        {
            ViewBag.UserNom   = HttpContext.Session.GetString("user_nom") ?? "";
            ViewBag.UserPhoto = HttpContext.Session.GetString("user_photo") ?? "";
            ViewBag.CycleNom  = HttpContext.Session.GetString("cycle_nom") ?? "";
        }

        private int GetCycleId() =>
            int.TryParse(HttpContext.Session.GetString("cycle_id"), out var id) ? id : 0;

        public async Task<IActionResult> Index()
        {
            var (ok, redirect, membreId) = CheckMembre();
            if (!ok) return redirect!;
            SetViewBag();

            var membre = await _membreService.GetByIdAsync(membreId);
            if (membre == null) return RedirectToAction("Login", "Auth");

            var cycleId = GetCycleId();

            // Données de base du membre
            var cotisations  = (await _cotisationService.GetAllAsync())
                                .Where(c => c.IdMembre == membreId && c.IdCycle == cycleId).ToList();
            var versements   = (await _versementService.GetAllAsync())
                                .Where(v => v.IdMembre == membreId && v.IdCycle == cycleId).ToList();
            var prets        = await _pretService.GetByMembreAsync(membreId);
            var inscriptions = (await _mctService.GetAllAsync())
                                .Where(i => i.IdMembre == membreId && i.IdCycle == cycleId).ToList();

            // Planning du cycle — pour chaque tontine inscrite
            var tousInscriptions  = (await _mctService.GetAllAsync())
                                     .Where(i => i.IdCycle == cycleId).ToList();
            var tousVersements    = (await _versementService.GetAllAsync())
                                     .Where(v => v.IdCycle == cycleId).ToList();
            var tontines          = await _tontineService.GetAllAsync();

            var planningParTontine = new List<PlanningTontineViewModel>();
            foreach (var monIns in inscriptions)
            {
                var tontine     = tontines.FirstOrDefault(t => t.IdTontine == monIns.IdTontine);
                var montantBase = tontine?.Montant ?? 0;

                var membresTontine = tousInscriptions
                    .Where(i => i.IdTontine == monIns.IdTontine)
                    .OrderBy(i => i.NumeroOrdre ?? 999)
                    .ToList();

                var versementsT = tousVersements
                    .Where(v => v.IdTontine == monIns.IdTontine)
                    .ToDictionary(v => v.IdMembre);

                var lignes = membresTontine.Select(m => new LignePlanningViewModel
                {
                    IdMembre      = m.IdMembre,
                    NomMembre     = m.NomMembre ?? m.Matricule ?? "—",
                    NumeroOrdre   = m.NumeroOrdre ?? 0,
                    NombreParts   = m.NombreParts,
                    ARecu         = versementsT.ContainsKey(m.IdMembre),
                    DateReception = versementsT.TryGetValue(m.IdMembre, out var vd) ? vd.DateVersement : null,
                    MontantRecu   = versementsT.TryGetValue(m.IdMembre, out var vm2) ? vm2.MontantNet : 0,
                    EstMoi        = m.IdMembre == membreId
                }).OrderBy(l => l.NumeroOrdre).ToList();

                var totalParts      = membresTontine.Sum(m => m.NombreParts);
                var pot             = montantBase * totalParts;
                var monOrdre        = monIns.NumeroOrdre ?? 0;
                var personnesAvant  = lignes.Count(l => l.NumeroOrdre < monOrdre && !l.ARecu && !l.EstMoi);
                var aDejaRecu       = versementsT.ContainsKey(membreId);

                planningParTontine.Add(new PlanningTontineViewModel
                {
                    NomTontine        = monIns.LibelleTontine ?? tontine?.Libelle ?? "",
                    IdTontine         = monIns.IdTontine,
                    MontantCotisation = montantBase,
                    Frequence         = tontine?.Frequence ?? "",
                    MonNumeroOrdre    = monOrdre,
                    MesNombreParts    = monIns.NombreParts,
                    MontantARecevoir  = pot * monIns.NombreParts,
                    NombreTotal       = membresTontine.Count,
                    NombreARecu       = versementsT.Count,
                    PersonnesAvantMoi = personnesAvant,
                    ADejaRecu         = aDejaRecu,
                    Lignes            = lignes
                });
            }

            // Prochaines réunions (du cycle, à venir)
            var prochainesReunions = (await _reunionService.GetAllAsync())
                .Where(r => r.IdCycle == cycleId && r.DateReunion >= DateTime.Now)
                .OrderBy(r => r.DateReunion)
                .Take(3)
                .ToList();

            var vm = new PortailMembreViewModel
            {
                Membre              = membre,
                MesCotisations      = cotisations,
                MesVersements       = versements,
                MesPrets            = prets,
                MesInscriptions     = inscriptions,
                PlanningParTontine  = planningParTontine,
                ProchainesReunions  = prochainesReunions
            };

            return View(vm);
        }

        public async Task<IActionResult> MesCotisations()
        {
            var (ok, redirect, membreId) = CheckMembre();
            if (!ok) return redirect!;
            SetViewBag();

            var cycleId = GetCycleId();
            var tous    = await _cotisationService.GetAllAsync();
            var liste   = tous.Where(c => c.IdMembre == membreId && c.IdCycle == cycleId).ToList();
            return View(liste);
        }

        public async Task<IActionResult> MesVersements()
        {
            var (ok, redirect, membreId) = CheckMembre();
            if (!ok) return redirect!;
            SetViewBag();

            var cycleId = GetCycleId();
            var tous    = await _versementService.GetAllAsync();
            var liste   = tous.Where(v => v.IdMembre == membreId && v.IdCycle == cycleId).ToList();
            return View(liste);
        }

        public async Task<IActionResult> MesPrets()
        {
            var (ok, redirect, membreId) = CheckMembre();
            if (!ok) return redirect!;
            SetViewBag();

            var liste = await _pretService.GetByMembreAsync(membreId);
            return View(liste);
        }

        public async Task<IActionResult> MonProfil()
        {
            var (ok, redirect, membreId) = CheckMembre();
            if (!ok) return redirect!;
            SetViewBag();

            var membre = await _membreService.GetByIdAsync(membreId);
            if (membre == null) return RedirectToAction("Login", "Auth");
            return View(membre);
        }

        public async Task<IActionResult> TelechargerRecu(int id)
        {
            var (ok, redirect, membreId) = CheckMembre();
            if (!ok) return redirect!;

            var cot = await _cotisationService.GetByIdAsync(id);
            // Vérification : ce reçu appartient bien à ce membre
            if (cot == null || cot.IdMembre != membreId || cot.Statut != "Payé")
                return Forbid();

            var pdfBytes = _pdf.GenererRecuCotisationPdf(cot);
            return File(pdfBytes, "application/pdf", $"recu_cotisation_{id:D5}.pdf");
        }

        // --- Fonds de solidarité ---

        public async Task<IActionResult> MesFonds()
        {
            var (ok, redirect, membreId) = CheckMembre();
            if (!ok) return redirect!;
            SetViewBag();

            var cycleId = GetCycleId();
            var fonds   = (await _fondsService.GetAllAsync())
                           .Where(f => f.IdCycle == cycleId).ToList();

            // Toutes les demandes de ce membre, regroupées par fonds
            var mesDemandes = new Dictionary<int, List<DemandeAideViewModel>>();
            foreach (var f in fonds)
            {
                var demandes = await _demandeService.GetByFondsAsync(f.IdFonds);
                mesDemandes[f.IdFonds] = demandes.Where(d => d.IdMembre == membreId).ToList();
            }

            ViewBag.MesDemandes = mesDemandes;
            return View(fonds);
        }

        [HttpGet]
        public async Task<IActionResult> DemanderAide(int idFonds)
        {
            var (ok, redirect, membreId) = CheckMembre();
            if (!ok) return redirect!;
            SetViewBag();

            var fonds = await _fondsService.GetByIdAsync(idFonds);
            if (fonds == null) return NotFound();

            ViewBag.Fonds = fonds;
            return View(new DemandeAideViewModel
            {
                IdFonds  = idFonds,
                IdMembre = membreId
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DemanderAide(DemandeAideViewModel vm)
        {
            var (ok, redirect, membreId) = CheckMembre();
            if (!ok) return redirect!;

            vm.IdMembre = membreId; // toujours forcer le membre de la session

            if (!ModelState.IsValid)
            {
                SetViewBag();
                ViewBag.Fonds = await _fondsService.GetByIdAsync(vm.IdFonds);
                return View(vm);
            }

            await _demandeService.CreateAsync(vm);
            TempData["SuccesFonds"] = "Votre demande d'aide a bien été soumise. Elle sera examinée par l'administrateur.";
            return RedirectToAction(nameof(MesFonds));
        }

        // ---

        public IActionResult Deconnexion()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Auth");
        }
    }
}
