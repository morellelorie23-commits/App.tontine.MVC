using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using tontine.MVC.Models;
using tontine.MVC.Services;

namespace tontine.MVC.Controllers
{
    public class ReunionController : BaseController
    {
        private readonly IReunionService           _service;
        private readonly ITontineService           _tontines;
        private readonly IEmailService             _email;
        private readonly ISmsService               _sms;
        private readonly IPresenceService          _presences;
        private readonly IMembreService            _membres;
        private readonly IMembreCycleTontineService _mct;
        private readonly PdfService                _pdf;

        public ReunionController(
            IReunionService           service,
            ITontineService           tontines,
            IEmailService             email,
            ISmsService               sms,
            IPresenceService          presences,
            IMembreService            membres,
            IMembreCycleTontineService mct,
            PdfService                pdf)
        {
            _service   = service;
            _tontines  = tontines;
            _email     = email;
            _sms       = sms;
            _presences = presences;
            _membres   = membres;
            _mct       = mct;
            _pdf       = pdf;
        }

        public async Task<IActionResult> Index()
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Réunions", isActive: true)
            );
            var list = await _service.GetAllAsync();
            return View(list.Where(r => r.IdCycle == CycleId).ToList());
        }

        public async Task<IActionResult> Create()
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Réunions", "Reunion", "Index"),
                BreadcrumbItem("Ajouter", isActive: true)
            );
            await PopulateDropdowns();
            return View(new ReunionViewModel { IdCycle = CycleId, DateReunion = DateTime.Now });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReunionViewModel vm)
        {
            if (!ModelState.IsValid) { await PopulateDropdowns(); return View(vm); }
            var ok = await _service.CreateAsync(vm);
            if (!ok) { ModelState.AddModelError("", "Erreur lors de l'enregistrement."); await PopulateDropdowns(); return View(vm); }
            TempData["Success"] = "Réunion enregistrée avec succès.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Réunions", "Reunion", "Index"),
                BreadcrumbItem("Modifier", isActive: true)
            );
            var vm = await _service.GetByIdAsync(id);
            if (vm == null) return NotFound();
            await PopulateDropdowns();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ReunionViewModel vm)
        {
            if (!ModelState.IsValid) { await PopulateDropdowns(); return View(vm); }
            var ok = await _service.UpdateAsync(id, vm);
            if (!ok) { ModelState.AddModelError("", "Erreur lors de la modification."); await PopulateDropdowns(); return View(vm); }
            TempData["Success"] = "Réunion modifiée avec succès.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Réunions", "Reunion", "Index"),
                BreadcrumbItem("Supprimer", isActive: true)
            );
            var vm = await _service.GetByIdAsync(id);
            return vm == null ? NotFound() : View(vm);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _service.DeleteAsync(id);
            TempData["Success"] = "Réunion supprimée.";
            return RedirectToAction(nameof(Index));
        }

        // Envoyer les convocations par email à tous les membres du cycle-tontine
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Convoquer(int id)
        {
            var reunion = await _service.GetByIdAsync(id);
            if (reunion == null) return NotFound();

            var tousLesInscriptions = await _mct.GetAllAsync();
            var inscrits = tousLesInscriptions
                .Where(m => m.IdCycle == reunion.IdCycle && m.IdTontine == reunion.IdTontine)
                .ToList();

            int envoyes = 0;
            var taches = inscrits.Select(async inscrit =>
            {
                var membre = await _membres.GetByIdAsync(inscrit.IdMembre);
                if (membre == null) return;
                var nomComplet = $"{membre.Prenom} {membre.Nom}";
                bool contacte = false;
                if (membre.Email != null)
                {
                    await _email.SendConvocationAsync(
                        membre.Email, nomComplet,
                        reunion.Objet, reunion.LibelleTontine,
                        reunion.DateReunion, reunion.Lieu, reunion.Notes);
                    contacte = true;
                }
                if (membre.Telephone != null)
                {
                    await _sms.SendConvocationAsync(
                        membre.Telephone, nomComplet,
                        reunion.Objet, reunion.LibelleTontine,
                        reunion.DateReunion, reunion.Lieu);
                    contacte = true;
                }
                if (contacte) Interlocked.Increment(ref envoyes);
            });

            _ = Task.WhenAll(taches);

            TempData["Success"] = $"Convocations envoyées à {inscrits.Count} membre(s) (email + SMS selon coordonnées renseignées).";
            return RedirectToAction(nameof(Index));
        }

        // Feuille de présence — GET
        public async Task<IActionResult> GererPresence(int id)
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Réunions", "Reunion", "Index"),
                BreadcrumbItem("Feuille de présence", isActive: true)
            );

            var reunion = await _service.GetByIdAsync(id);
            if (reunion == null) return NotFound();

            var tousInscrits = await _mct.GetAllAsync();
            var inscrits = tousInscrits
                .Where(m => m.IdCycle == reunion.IdCycle && m.IdTontine == reunion.IdTontine)
                .ToList();

            var existantes = await _presences.GetByReunionAsync(id);

            var liste = inscrits.Select(m =>
            {
                var found = existantes.FirstOrDefault(p => p.IdMembre == m.IdMembre);
                return found ?? new PresenceViewModel
                {
                    IdPresence     = 0,
                    IdReunion      = id,
                    IdMembre       = m.IdMembre,
                    NomMembre      = m.NomMembre ?? $"Membre #{m.IdMembre}",
                    StatutPresence = "Présent"
                };
            }).OrderBy(p => p.NomMembre).ToList();

            var vm = new GererPresenceViewModel
            {
                Reunion   = reunion,
                Presences = liste
            };
            return View(vm);
        }

        // Feuille de présence — POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GererPresence(int id, GererPresenceViewModel vm)
        {
            var ok = await _presences.BatchSaveAsync(id, vm.Presences);
            TempData[ok ? "Success" : "Error"] = ok
                ? "Présences enregistrées avec succès."
                : "Erreur lors de l'enregistrement des présences.";
            return RedirectToAction(nameof(Index));
        }

        // Générer le procès-verbal en PDF
        public async Task<IActionResult> GenererPV(int id)
        {
            var reunion  = await _service.GetByIdAsync(id);
            if (reunion == null) return NotFound();
            var presences = await _presences.GetByReunionAsync(id);

            var pdfBytes = _pdf.GenererProcesVerbalPdf(reunion, presences);
            return File(pdfBytes, "application/pdf", $"pv_reunion_{id:D5}.pdf");
        }

        private async Task PopulateDropdowns()
        {
            var tontines = await _tontines.GetAllAsync();
            ViewBag.Tontines = tontines.Select(t => new SelectListItem(t.Libelle, t.IdTontine.ToString()));

            ViewBag.Statuts = new List<SelectListItem>
            {
                new("Planifiée", "Planifiée"),
                new("En cours",  "En cours"),
                new("Terminée",  "Terminée"),
                new("Annulée",   "Annulée"),
            };
        }
    }
}
