using Microsoft.AspNetCore.Mvc;
using tontine.MVC.Services;

namespace tontine.MVC.Controllers
{
    public class RapportController : BaseController
    {
        private readonly ICotisationService _cotisationService;
        private readonly IVersementService  _versementService;
        private readonly IPretService       _pretService;
        private readonly IMembreService     _membreService;
        private readonly ICycleService      _cycleService;
        private readonly ITontineService    _tontineService;
        private readonly PdfService         _pdf;

        public RapportController(
            ICotisationService cotisationService,
            IVersementService versementService,
            IPretService pretService,
            IMembreService membreService,
            ICycleService cycleService,
            ITontineService tontineService,
            PdfService pdf)
        {
            _cotisationService  = cotisationService;
            _versementService   = versementService;
            _pretService        = pretService;
            _membreService      = membreService;
            _cycleService       = cycleService;
            _tontineService     = tontineService;
            _pdf                = pdf;
        }

        public async Task<IActionResult> Index()
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Statistiques"),
                BreadcrumbItem("Rapports détaillés", isActive: true)
            );

            var cotisations = await _cotisationService.GetAllAsync();
            var versements  = await _versementService.GetAllAsync();
            var prets       = await _pretService.GetAllAsync();
            var membres     = await _membreService.GetAllAsync();
            var cycles      = await _cycleService.GetAllAsync();
            var tontines    = await _tontineService.GetAllAsync();

            var vm = new Models.RapportViewModel
            {
                TotalCotisations        = cotisations.Sum(c => c.Montant),
                TotalVersements         = versements.Sum(v => v.Montant),
                TotalPrets              = prets.Sum(p => p.Montant),
                TotalRembourse          = prets.Sum(p => p.MontantRemboursé),
                NbMembres               = membres.Count,
                NbCycles                = cycles.Count,
                NbTontines              = tontines.Count,
                CotisationsPayees       = cotisations.Count(c => c.Statut == "Payé"),
                CotisationsEnAttente    = cotisations.Count(c => c.Statut == "En attente"),
                CotisationsEnRetard     = cotisations.Count(c => c.Statut == "En retard"),
                PretsApprouves          = prets.Count(p => p.Statut == "Approuvé"),
                PretsRembourses         = prets.Count(p => p.Statut == "Remboursé"),
                PretsEnRetard           = prets.Count(p => p.Statut == "En retard"),
                ParTontine              = versements.GroupBy(v => v.LibelleTontine ?? "?")
                                            .ToDictionary(g => g.Key, g => g.Sum(v => v.Montant)),
                ParCycle                = cotisations.GroupBy(c => c.NomCycle ?? "?")
                                            .ToDictionary(g => g.Key, g => g.Sum(c => c.Montant)),
                TopMembres              = cotisations.GroupBy(c => c.NomMembre ?? "?")
                                            .OrderByDescending(g => g.Sum(c => c.Montant))
                                            .Take(10)
                                            .ToDictionary(g => g.Key, g => g.Sum(c => c.Montant))
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> ExportPdf()
        {
            var cotisations = await _cotisationService.GetAllAsync();
            var versements  = await _versementService.GetAllAsync();
            var prets       = await _pretService.GetAllAsync();
            var membres     = await _membreService.GetAllAsync();
            var cycles      = await _cycleService.GetAllAsync();
            var tontines    = await _tontineService.GetAllAsync();

            var vm = new Models.RapportViewModel
            {
                TotalCotisations     = cotisations.Sum(c => c.Montant),
                TotalVersements      = versements.Sum(v => v.Montant),
                TotalPrets           = prets.Sum(p => p.Montant),
                TotalRembourse       = prets.Sum(p => p.MontantRemboursé),
                NbMembres            = membres.Count,
                NbCycles             = cycles.Count,
                NbTontines           = tontines.Count,
                CotisationsPayees    = cotisations.Count(c => c.Statut == "Payé"),
                CotisationsEnAttente = cotisations.Count(c => c.Statut == "En attente"),
                CotisationsEnRetard  = cotisations.Count(c => c.Statut == "En retard"),
                PretsApprouves       = prets.Count(p => p.Statut == "Approuvé"),
                PretsRembourses      = prets.Count(p => p.Statut == "Remboursé"),
                PretsEnRetard        = prets.Count(p => p.Statut == "En retard"),
                ParTontine           = versements.GroupBy(v => v.LibelleTontine ?? "?")
                                         .ToDictionary(g => g.Key, g => g.Sum(v => v.Montant)),
                ParCycle             = cotisations.GroupBy(c => c.NomCycle ?? "?")
                                         .ToDictionary(g => g.Key, g => g.Sum(c => c.Montant)),
                TopMembres           = cotisations.GroupBy(c => c.NomMembre ?? "?")
                                         .OrderByDescending(g => g.Sum(c => c.Montant))
                                         .Take(10)
                                         .ToDictionary(g => g.Key, g => g.Sum(c => c.Montant))
            };

            var pdfBytes = _pdf.GenererRapportPdf(vm);
            return File(pdfBytes, "application/pdf", $"rapport-tontine-{DateTime.Now:yyyyMMdd}.pdf");
        }
    }
}
