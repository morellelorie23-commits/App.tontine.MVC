using Microsoft.AspNetCore.Mvc;
using tontine.MVC.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using tontine.MVC.Models;
using tontine.MVC.Services;

namespace tontine.MVC.Controllers
{
    public class CotisationController : BaseController
    {
        private readonly ICotisationService       _service;
        private readonly IMembreService           _membres;
        private readonly ITontineService          _tontines;
        private readonly ICycleService            _cycles;
        private readonly IAlerteCotisationService _alertes;
        private readonly PdfService               _pdf;
        private readonly IPaiementMobileService   _paiements;
        private readonly IReunionService          _reunions;
        private readonly IGrandLivreService       _gl;
        private readonly IVersementService        _versements;
        private readonly ExcelService             _excel;
        private readonly IEmailService            _email;

        public CotisationController(ICotisationService service, IMembreService membres,
                                     ITontineService tontines, ICycleService cycles,
                                     IAlerteCotisationService alertes, PdfService pdf,
                                     IPaiementMobileService paiements,
                                     IReunionService reunions,
                                     IGrandLivreService gl,
                                     IVersementService versements,
                                     ExcelService excel,
                                     IEmailService email)
        {
            _service    = service;
            _membres    = membres;
            _tontines   = tontines;
            _cycles     = cycles;
            _alertes    = alertes;
            _pdf        = pdf;
            _paiements  = paiements;
            _reunions   = reunions;
            _gl         = gl;
            _versements = versements;
            _excel      = excel;
            _email      = email;
        }

        public async Task<IActionResult> Index()
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Comptabilité"),
                BreadcrumbItem("Cotisations", isActive: true)
            );
            var list = await _service.GetAllAsync();
            return View(list.Where(c => c.IdCycle == CycleId).ToList());
        }

        public async Task<IActionResult> Create()
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Comptabilité"),
                BreadcrumbItem("Cotisations", "Cotisation", "Index"),
                BreadcrumbItem("Ajouter", isActive: true)
            );
            await PopulateDropdowns();
            return View(new CotisationViewModel { IdCycle = CycleId, DateCotisation = DateTime.Now });
        }

        [RoleAuthorize("Administrateur", "Gestionnaire")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CotisationViewModel vm)
        {
            if (!ModelState.IsValid) { await PopulateDropdowns(); return View(vm); }
            var ok = await _service.CreateAsync(vm);
            if (!ok) { ModelState.AddModelError("", "Erreur lors de l'enregistrement."); await PopulateDropdowns(); return View(vm); }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Comptabilité"),
                BreadcrumbItem("Cotisations", "Cotisation", "Index"),
                BreadcrumbItem("Modifier", isActive: true)
            );
            var vm = await _service.GetByIdAsync(id);
            if (vm == null) return NotFound();
            await PopulateDropdowns();
            return View(vm);
        }

        [RoleAuthorize("Administrateur", "Gestionnaire")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CotisationViewModel vm)
        {
            if (!ModelState.IsValid) { await PopulateDropdowns(); return View(vm); }
            var ok = await _service.UpdateAsync(id, vm);
            if (!ok) { ModelState.AddModelError("", "Erreur lors de la modification."); await PopulateDropdowns(); return View(vm); }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Comptabilité"),
                BreadcrumbItem("Cotisations", "Cotisation", "Index"),
                BreadcrumbItem("Supprimer", isActive: true)
            );
            var vm = await _service.GetByIdAsync(id);
            return vm == null ? NotFound() : View(vm);
        }

        [RoleAuthorize("Administrateur", "Gestionnaire")]

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [RoleAuthorize("Administrateur", "Gestionnaire")]
        [HttpPost]
        public async Task<IActionResult> PayerCash(int id)
        {
            var ok = await _service.PayerCashAsync(id);
            if (ok)
            {
                TempData["Success"] = "Paiement en espèces enregistré.";
                // Envoi de la confirmation par email (en arrière-plan, sans bloquer la réponse)
                _ = Task.Run(async () =>
                {
                    var cot = await _service.GetByIdAsync(id);
                    if (cot == null) return;
                    var membre = await _membres.GetByIdAsync(cot.IdMembre);
                    if (membre?.Email != null)
                        await _email.SendConfirmationPaiementAsync(
                            membre.Email, $"{membre.Prenom} {membre.Nom}",
                            cot.LibelleTontine, cot.NomCycle,
                            cot.Montant, cot.DateCotisation, cot.IdCotisation);
                });
            }
            else
            {
                TempData["Error"] = "Erreur lors de l'enregistrement.";
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Tours()
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Comptabilité"),
                BreadcrumbItem("Tableau des tours", isActive: true)
            );
            var tontines = await _tontines.GetAllAsync();
            ViewBag.Tontines = tontines;
            var defaultTontine = tontines.FirstOrDefault();
            var tours = defaultTontine != null
                ? await _service.GetToursAsync(CycleId, defaultTontine.IdTontine)
                : new List<tontine.MVC.Models.TourViewModel>();
            ViewBag.IdTontine = defaultTontine?.IdTontine ?? 0;
            return View(tours);
        }

        [HttpGet]
        public async Task<IActionResult> ToursData(int idTontine)
        {
            var tours = await _service.GetToursAsync(CycleId, idTontine);
            return Json(tours);
        }

        public async Task<IActionResult> LivreDeCompte()
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Comptabilité"),
                BreadcrumbItem("Livre de compte", isActive: true)
            );
            var tontines = await _tontines.GetAllAsync();
            ViewBag.Tontines = tontines;
            var defaultTontine = tontines.FirstOrDefault();
            var livre = defaultTontine != null
                ? await _service.GetLivreDeCompteAsync(CycleId, defaultTontine.IdTontine)
                : new tontine.MVC.Models.LivreDeCompteViewModel();
            ViewBag.IdTontine = defaultTontine?.IdTontine ?? 0;
            return View(livre ?? new tontine.MVC.Models.LivreDeCompteViewModel());
        }

        [HttpGet]
        public async Task<IActionResult> LivreData(int idTontine)
        {
            var livre = await _service.GetLivreDeCompteAsync(CycleId, idTontine);
            return Json(livre ?? new tontine.MVC.Models.LivreDeCompteViewModel());
        }

        [RoleAuthorize("Administrateur", "Gestionnaire")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AppliquerPenalites()
        {
            var nb = await _service.AppliquerPenalitesAsync();
            TempData["Success"] = nb > 0
                ? $"{nb} cotisation(s) marquée(s) En retard."
                : "Aucune cotisation en retard détectée.";
            return RedirectToAction(nameof(Index));
        }

        [RoleAuthorize("Administrateur", "Gestionnaire")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnvoyerAlertes()
        {
            var result = await _alertes.EnvoyerAlertesCotisationsEnRetardAsync();
            TempData["Success"] = result.EmailsEnvoyes > 0
                ? $"{result.EmailsEnvoyes} alerte(s) envoyée(s) sur {result.CotisationsEnRetard} cotisation(s) en retard."
                : result.CotisationsEnRetard == 0
                    ? "Aucune cotisation en retard."
                    : $"0 email envoyé ({result.EmailsEchoues} échec(s)).";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> RecuPdf(int id)
        {
            var cot = await _service.GetByIdAsync(id);
            if (cot == null) return NotFound();
            var pdf = _pdf.GenererRecuCotisationPdf(cot);
            return File(pdf, "application/pdf", $"recu-cotisation-{id}.pdf");
        }

        [HttpPost]
        public async Task<IActionResult> InitierPaiement([FromBody] InitierPaiementViewModel req)
        {
            var result = await _paiements.InitierAsync(req);
            return Json(new { succes = result.Succes, reference = result.Reference, message = result.Message, paymentUrl = result.PaymentUrl });
        }

        [HttpGet]
        public async Task<IActionResult> VerifierStatutPaiement(string reference)
        {
            var statut = await _paiements.VerifierStatutAsync(reference);
            return Json(statut);
        }

        [HttpPost]
        public async Task<IActionResult> SimulerPaiement(string id, [FromQuery] bool succes = true)
        {
            var ok = await _paiements.SimulerAsync(id, succes);
            return Json(new { ok });
        }

        // ── Paiement en caisse (collecte lors d'une réunion) ─────────────
        public async Task<IActionResult> PaiementCaisse(int? idTontine = null, int? idReunion = null)
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Comptabilité"),
                BreadcrumbItem("Paiement en caisse", isActive: true)
            );

            var tontines = await _tontines.GetAllAsync();
            var membres  = await _membres.GetAllAsync();
            var reunions = await _reunions.GetAllAsync();

            var selTontine = idTontine ?? tontines.FirstOrDefault()?.IdTontine ?? 0;
            var tontine    = tontines.FirstOrDefault(t => t.IdTontine == selTontine);
            var reunionsFiltrees = reunions.Where(r => r.IdTontine == selTontine).ToList();
            var selReunion = idReunion ?? reunionsFiltrees.FirstOrDefault()?.IdReunion ?? 0;

            var gl            = selTontine > 0 ? await _gl.GetGrandLivreAsync(selTontine, null) : null;
            decimal soldeCaisse   = gl?.SoldeActuel ?? 0m;
            decimal montantDom    = tontine?.Montant ?? 0m;
            var cotisations   = await _service.GetAllAsync();
            decimal mcActuel  = cotisations
                .Where(c => c.IdTontine == selTontine && c.IdCycle == CycleId && c.Statut == "Payé")
                .Sum(c => c.Montant);
            int nbMembres        = membres.Count;
            decimal montantDemande = montantDom * nbMembres;

            ViewBag.Tontines         = tontines;
            ViewBag.Reunions         = reunionsFiltrees;
            ViewBag.Membres          = membres;
            ViewBag.IdTontine        = selTontine;
            ViewBag.IdReunion        = selReunion;
            ViewBag.SoldeCaisse      = soldeCaisse;
            ViewBag.MontantDomicilie = montantDom;

            var vm = new PaiementCaisseViewModel
            {
                IdTontine        = selTontine,
                IdReunion        = selReunion,
                IdCycle          = CycleId,
                SoldeCaisse      = soldeCaisse,
                MontantAttendu   = montantDom,
                MontantDomicilie = montantDom,
                Date             = DateTime.Now
            };
            return View(vm);
        }

        [RoleAuthorize("Administrateur", "Gestionnaire")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PaiementCaisse(PaiementCaisseViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                var tontines = await _tontines.GetAllAsync();
                var membres  = await _membres.GetAllAsync();
                var reunions = await _reunions.GetAllAsync();
                ViewBag.Tontines = tontines;
                ViewBag.Reunions = reunions.Where(r => r.IdTontine == vm.IdTontine).ToList();
                ViewBag.Membres  = membres;
                ViewBag.IdTontine = vm.IdTontine;
                ViewBag.IdReunion = vm.IdReunion;
                ViewBag.SoldeCaisse = vm.SoldeCaisse;
                ViewBag.MontantDomicilie = vm.MontantDomicilie;
                return View(vm);
            }

            var details = $"Réunion #{vm.IdReunion} | Pièce: {vm.PieceJustificatif}" +
                          $" | Attendu: {vm.MontantAttendu:N0} | Reste: {vm.Reste:N0}" +
                          (vm.Penalite > 0 ? $" | Pénalité: {vm.Penalite:N0}" : "") +
                          (vm.Enchere  > 0 ? $" | Enchère: {vm.Enchere:N0}"   : "") +
                          (vm.Notes   != null ? $" | {vm.Notes}" : "");

            var cotisation = new CotisationViewModel
            {
                IdMembre       = vm.IdMembre,
                IdTontine      = vm.IdTontine,
                IdCycle        = vm.IdCycle,
                Montant        = vm.MontantCotise > 0 ? vm.MontantCotise : vm.PaiementEnCaisse,
                DateCotisation = vm.Date,
                Statut         = vm.MarqueCotisation ? "Payé" : "En attente",
                Notes          = details,
                ModePaiement   = "Cash"
            };
            var ok = await _service.CreateAsync(cotisation);
            TempData[ok ? "Success" : "Error"] = ok
                ? "Paiement en caisse enregistré avec succès."
                : "Erreur lors de l'enregistrement du paiement.";
            return RedirectToAction(nameof(PaiementCaisse), new { idTontine = vm.IdTontine, idReunion = vm.IdReunion });
        }

        [HttpGet]
        public async Task<IActionResult> PaiementCaisseData(int idTontine)
        {
            var gl          = await _gl.GetGrandLivreAsync(idTontine, null);
            var cotisations = await _service.GetAllAsync();
            var membres     = await _membres.GetAllAsync();
            var tontines    = await _tontines.GetAllAsync();
            var reunions    = await _reunions.GetAllAsync();

            decimal soldeCaisse  = gl?.SoldeActuel ?? 0m;
            decimal mcActuel     = cotisations
                .Where(c => c.IdTontine == idTontine && c.Statut == "Payé")
                .Sum(c => c.Montant);
            var tontine          = tontines.FirstOrDefault(t => t.IdTontine == idTontine);
            decimal montantDom   = tontine?.Montant ?? 0m;
            decimal montantDemande = montantDom * membres.Count;
            var reunionsFilt = reunions.Where(r => r.IdTontine == idTontine)
                .Select(r => new { r.IdReunion, Label = r.DateReunion.ToString("dd/MM/yyyy") + " — " + r.Objet })
                .ToList();

            return Json(new
            {
                soldeCaisse,
                montantCaisseActuel = mcActuel,
                montantDemande,
                montantDomicilie = montantDom,
                reunions = reunionsFilt
            });
        }

        [RoleAuthorize("Administrateur", "Gestionnaire")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AjouterBeneficiaire(BeneficiaireViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Données bénéficiaire invalides. Vérifiez les champs.";
                return RedirectToAction(nameof(PaiementCaisse), new { idTontine = vm.IdTontine, idReunion = vm.IdReunion });
            }

            var montantNet = vm.MontantBeneficiaire - vm.RetourEnCaisse;
            var notes = $"Réunion #{vm.IdReunion}"
                      + (vm.EstMandataire ? " | Paiement via mandataire" : "")
                      + (vm.RetourEnCaisse > 0 ? $" | Retour en caisse: {vm.RetourEnCaisse:N0} FCFA" : "")
                      + (vm.Notes != null ? $" | {vm.Notes}" : "");

            var versement = new VersementViewModel
            {
                IdMembre       = vm.IdBeneficiaire,
                IdTontine      = vm.IdTontine,
                IdCycle        = vm.IdCycle,
                Montant        = montantNet > 0 ? montantNet : vm.MontantBeneficiaire,
                DateVersement  = vm.Date,
                Notes          = notes
            };
            var ok = await _versements.CreateAsync(versement);
            TempData[ok ? "Success" : "Error"] = ok
                ? $"Bénéficiaire enregistré — Versement net : {montantNet:N0} FCFA."
                : "Erreur lors de l'enregistrement du bénéficiaire.";
            return RedirectToAction(nameof(PaiementCaisse), new { idTontine = vm.IdTontine, idReunion = vm.IdReunion });
        }

        private async Task PopulateDropdowns()
        {
            var membres  = await _membres.GetAllAsync();
            var tontines = await _tontines.GetAllAsync();
            var cycles   = await _cycles.GetAllAsync();

            ViewBag.Membres  = membres.Select(m => new SelectListItem(m.Nom + " " + m.Prenom, m.IdMembre.ToString()));
            ViewBag.Tontines = tontines.Select(t => new SelectListItem(t.Libelle, t.IdTontine.ToString()));
            ViewBag.Cycles   = cycles.Select(c => new SelectListItem(c.NomCycle, c.IdCycle.ToString()));
        }

        [HttpGet]
        public async Task<IActionResult> ExportExcel()
        {
            var list    = await _service.GetAllAsync();
            var donnees = list.Where(c => c.IdCycle == CycleId).ToList();
            var bytes   = _excel.ExporterCotisations(donnees, CycleNom);
            return File(bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"cotisations-{CycleNom}-{DateTime.Now:yyyyMMdd}.xlsx");
        }
    }
}
