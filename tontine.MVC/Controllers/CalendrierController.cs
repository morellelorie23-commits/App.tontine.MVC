using Microsoft.AspNetCore.Mvc;
using tontine.MVC.Models;
using tontine.MVC.Services;

namespace tontine.MVC.Controllers
{
    public class CalendrierController : BaseController
    {
        private readonly ICycleService              _cycles;
        private readonly ITontineService            _tontines;
        private readonly IMembreCycleTontineService _mct;
        private readonly IVersementService          _versements;
        private readonly IMembreService             _membres;

        public CalendrierController(
            ICycleService              cycles,
            ITontineService            tontines,
            IMembreCycleTontineService mct,
            IVersementService          versements,
            IMembreService             membres)
        {
            _cycles     = cycles;
            _tontines   = tontines;
            _mct        = mct;
            _versements = versements;
            _membres    = membres;
        }

        public async Task<IActionResult> Index()
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Calendrier des versements", isActive: true)
            );

            var cycle       = await _cycles.GetByIdAsync(CycleId);
            var toutesInscriptions = await _mct.GetAllAsync();
            var tousVersements     = await _versements.GetAllAsync();
            var toutesTontines     = await _tontines.GetAllAsync();
            var tousMembres        = await _membres.GetAllAsync();

            var inscriptionsCycle = toutesInscriptions
                .Where(m => m.IdCycle == CycleId)
                .ToList();

            var versementsCycle = tousVersements
                .Where(v => v.IdCycle == CycleId)
                .ToList();

            var membresDict = tousMembres.ToDictionary(m => m.IdMembre, m => m);

            var parTontine = inscriptionsCycle
                .GroupBy(m => m.IdTontine)
                .Select(g =>
                {
                    var tontine = toutesTontines.FirstOrDefault(t => t.IdTontine == g.Key);
                    var montantCotisation = tontine?.Montant ?? 0;

                    var totalParts  = g.Sum(m => m.NombreParts);
                    var montantPot  = montantCotisation * totalParts;

                    var slots = g
                        .OrderBy(m => m.NumeroOrdre ?? 9999)
                        .ThenBy(m => m.NomMembre)
                        .Select((m, idx) =>
                        {
                            var datePrevue = CalculerDatePrevue(cycle?.DateDebut, tontine?.Frequence, m.NumeroOrdre ?? (idx + 1));
                            var versement  = versementsCycle.FirstOrDefault(v => v.IdMembre == m.IdMembre && v.IdTontine == g.Key);

                            string statut;
                            if (versement != null)
                                statut = "Versé";
                            else if (datePrevue.HasValue && datePrevue.Value.Date < DateTime.Today)
                                statut = "En retard";
                            else
                                statut = "À venir";

                            var membre = membresDict.GetValueOrDefault(m.IdMembre);
                            string photo = membre?.Photo ?? "";

                            return new CalendrierSlotViewModel
                            {
                                NumeroOrdre   = m.NumeroOrdre ?? (idx + 1),
                                IdMembre      = m.IdMembre,
                                NomMembre     = m.NomMembre ?? $"Membre #{m.IdMembre}",
                                LibelleTontine = tontine?.Libelle ?? "",
                                IdTontine     = g.Key,
                                MontantPot    = montantPot * m.NombreParts,
                                DatePrevue    = datePrevue,
                                Verse         = versement != null,
                                DateVersement = versement?.DateVersement,
                                MontantNet    = versement?.MontantNet ?? 0,
                                IdVersement   = versement?.IdVersement,
                                Statut        = statut,
                                Photo         = photo
                            };
                        }).ToList();

                    return new CalendrierTontineViewModel
                    {
                        IdTontine         = g.Key,
                        LibelleTontine    = tontine?.Libelle ?? $"Tontine #{g.Key}",
                        Frequence         = tontine?.Frequence ?? "Mensuel",
                        MontantCotisation = montantCotisation,
                        MontantPot        = montantPot,
                        NombreMembres     = slots.Count,
                        NombreVerses      = slots.Count(s => s.Verse),
                        Slots             = slots
                    };
                })
                .OrderBy(t => t.LibelleTontine)
                .ToList();

            var vm = new CalendrierViewModel
            {
                NomCycle   = cycle?.NomCycle ?? "",
                DateDebut  = cycle?.DateDebut,
                DateFin    = cycle?.DateFin,
                ParTontine = parTontine
            };

            return View(vm);
        }

        private static DateTime? CalculerDatePrevue(DateOnly? dateDebut, string? frequence, int ordre)
        {
            if (dateDebut == null) return null;
            var dt = dateDebut.Value.ToDateTime(TimeOnly.MinValue);
            return (frequence?.ToLower() ?? "mensuel") switch
            {
                "hebdomadaire" => dt.AddDays((ordre - 1) * 7),
                "bimensuel"    => dt.AddDays((ordre - 1) * 14),
                "trimestriel"  => dt.AddMonths((ordre - 1) * 3),
                _              => dt.AddMonths(ordre - 1)
            };
        }
    }
}
