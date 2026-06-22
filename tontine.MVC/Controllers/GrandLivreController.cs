using Microsoft.AspNetCore.Mvc;
using tontine.MVC.Models;
using tontine.MVC.Services;

namespace tontine.MVC.Controllers
{
    public class GrandLivreController : BaseController
    {
        private readonly IGrandLivreService _gl;
        private readonly ITontineService    _tontines;

        public GrandLivreController(IGrandLivreService gl, ITontineService tontines)
        {
            _gl       = gl;
            _tontines = tontines;
        }

        // ── Grand Livre par groupe ────────────────────────────────────────
        public async Task<IActionResult> Index(int? idTontine = null, string? periode = null)
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Comptabilité"),
                BreadcrumbItem("Grand Livre", isActive: true)
            );
            var tontines = await _tontines.GetAllAsync();
            ViewBag.Tontines = tontines;
            ViewBag.Periode  = periode ?? DateTime.Now.ToString("yyyy-MM");

            var id = idTontine ?? tontines.FirstOrDefault()?.IdTontine ?? 0;
            ViewBag.IdTontine = id;

            var gl = id > 0
                ? await _gl.GetGrandLivreAsync(id, periode)
                : new GrandLivreViewModel();

            return View(gl ?? new GrandLivreViewModel());
        }

        [HttpGet]
        public async Task<IActionResult> Data(int idTontine, string? periode = null)
        {
            var gl = await _gl.GetGrandLivreAsync(idTontine, periode);
            return Json(gl ?? new GrandLivreViewModel());
        }

        // ── Journal par code ──────────────────────────────────────────────
        public async Task<IActionResult> Journal(string code = "BQ", string? periode = null)
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Comptabilité"),
                BreadcrumbItem("Journal", isActive: true)
            );
            ViewBag.CodeJournal = code.ToUpper();
            ViewBag.Periode     = periode ?? DateTime.Now.ToString("yyyy-MM");

            var journal = await _gl.GetJournalAsync(code, periode);
            return View(journal ?? new JournalViewModel { CodeJournal = code.ToUpper() });
        }

        // ── Balance des comptes ───────────────────────────────────────────
        public async Task<IActionResult> Balance(string? periode = null)
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Comptabilité"),
                BreadcrumbItem("Balance des comptes", isActive: true)
            );
            ViewBag.Periode = periode ?? DateTime.Now.ToString("yyyy-MM");

            var balance = await _gl.GetBalanceAsync(periode);
            return View(balance ?? new BalanceViewModel());
        }

        // ── Journée comptable ─────────────────────────────────────────────
        public async Task<IActionResult> Journee()
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Comptabilité"),
                BreadcrumbItem("Journée comptable", isActive: true)
            );
            var journee = await _gl.GetJourneeCouranteAsync();
            return View(journee);
        }

        [HttpPost]
        public async Task<IActionResult> OuvrirJournee(string dateJournee)
        {
            if (DateOnly.TryParse(dateJournee, out var date))
                await _gl.OuvrirJourneeAsync(date, HttpContext.Session.GetString("user_nom") ?? "Système");
            return RedirectToAction(nameof(Journee));
        }

        [HttpPost]
        public async Task<IActionResult> FermerJournee(int id)
        {
            await _gl.FermerJourneeAsync(id);
            return RedirectToAction(nameof(Journee));
        }

        // ── Opération comptable manuelle ──────────────────────────────────
        public IActionResult OperationComptable()
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Comptabilité"),
                BreadcrumbItem("Saisie écriture", isActive: true)
            );
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> OperationComptable([FromBody] object payload)
        {
            var id = await _gl.CreerEcritureManuelleAsync(payload);
            if (id == null) return BadRequest("Erreur lors de la création de l'écriture.");
            return Ok(new { idEcriture = id });
        }

        // ── Transfert de caisse ───────────────────────────────────────────
        public IActionResult TransfertCaisse()
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Comptabilité"),
                BreadcrumbItem("Transfert de caisse", isActive: true)
            );
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> TransfertCaissePost([FromBody] object payload)
        {
            var id = await _gl.TransfertCaisseAsync(payload);
            if (id == null) return BadRequest("Erreur lors du transfert.");
            return Ok(new { idEcriture = id });
        }

        // ── Relevé de caisse ──────────────────────────────────────────────
        public async Task<IActionResult> ReleveCaisse(string? compte = null,
            string? dateDebut = null, string? dateFin = null)
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Comptabilité"),
                BreadcrumbItem("Relevé de caisse", isActive: true)
            );
            ViewBag.CompteSelectionne = compte;
            ViewBag.DateDebut = dateDebut ?? DateTime.Now.ToString("yyyy-MM-dd");
            ViewBag.DateFin   = dateFin   ?? DateTime.Now.ToString("yyyy-MM-dd");

            ReleveCaisseViewModel? releve = null;
            if (!string.IsNullOrEmpty(compte) || !string.IsNullOrEmpty(dateDebut))
                releve = await _gl.GetReleveCaisseAsync(compte, dateDebut, dateFin);

            return View(releve ?? new ReleveCaisseViewModel());
        }

        // ── Balance générale ──────────────────────────────────────────────
        public async Task<IActionResult> BalanceGenerale(string? dateDebut = null, string? dateFin = null)
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Comptabilité"),
                BreadcrumbItem("Balance générale", isActive: true)
            );
            ViewBag.DateDebut = dateDebut ?? new DateTime(DateTime.Now.Year, 1, 1).ToString("yyyy-MM-dd");
            ViewBag.DateFin   = dateFin   ?? DateTime.Now.ToString("yyyy-MM-dd");

            var balance = await _gl.GetBalanceGeneraleAsync(dateDebut, dateFin);
            return View(balance ?? new BalanceDetailViewModel());
        }

        // ── Balance client ────────────────────────────────────────────────
        public async Task<IActionResult> BalanceClient(string? dateDebut = null,
            string? dateFin = null, string? prefixeCompte = null, string? typePers = null)
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Comptabilité"),
                BreadcrumbItem("Balance client", isActive: true)
            );
            ViewBag.DateDebut     = dateDebut ?? new DateTime(DateTime.Now.Year, 1, 1).ToString("yyyy-MM-dd");
            ViewBag.DateFin       = dateFin   ?? DateTime.Now.ToString("yyyy-MM-dd");
            ViewBag.PrefixeCompte = prefixeCompte ?? "467";
            ViewBag.TypePers      = typePers ?? "";

            var balance = await _gl.GetBalanceClientAsync(dateDebut, dateFin, prefixeCompte ?? "467");
            return View(balance ?? new BalanceDetailViewModel());
        }

        // ── Rapprochement bancaire ────────────────────────────────────────
        public async Task<IActionResult> Rapprochement(int? idTontine = null,
            decimal? soldeReelBanque = null, string? periode = null)
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Comptabilité"),
                BreadcrumbItem("Rapprochement bancaire", isActive: true)
            );
            var tontines = await _tontines.GetAllAsync();
            ViewBag.Tontines = tontines;
            var id = idTontine ?? tontines.FirstOrDefault()?.IdTontine ?? 0;
            ViewBag.IdTontine        = id;
            ViewBag.SoldeReelBanque  = soldeReelBanque;

            var rapproch = id > 0
                ? await _gl.GetRapprochementAsync(id, soldeReelBanque)
                : new RapprochementViewModel();

            return View(rapproch ?? new RapprochementViewModel());
        }
    }
}
