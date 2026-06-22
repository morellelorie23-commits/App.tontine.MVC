using Microsoft.AspNetCore.Mvc;
using tontine.MVC.Models;
using tontine.MVC.Services;

namespace tontine.MVC.Controllers
{
    public class SaisieSeanceController : BaseController
    {
        private readonly ISaisieSeanceService _service;
        private readonly ITontineService      _tontines;
        private readonly IMembreService       _membres;

        public SaisieSeanceController(ISaisieSeanceService service, ITontineService tontines, IMembreService membres)
        {
            _service  = service;
            _tontines = tontines;
            _membres  = membres;
        }

        public async Task<IActionResult> Index(int? idTontine = null, int? idReunion = null)
        {
            SetBreadcrumbs(
                BreadcrumbItem("Tableau de bord", "Home", "Index"),
                BreadcrumbItem("Saisie Séance", isActive: true)
            );

            var idCycle = CycleId;
            var tontines = await _tontines.GetAllAsync();
            var membres  = await _membres.GetAllAsync();

            var vm = new SaisieSeancePageViewModel
            {
                IdCycle   = idCycle,
                IdTontine = idTontine ?? tontines.FirstOrDefault()?.IdTontine ?? 0,
                IdReunion = idReunion ?? 0,
                Tontines  = tontines.Select(t => new TontineSelectDto { IdTontine = t.IdTontine, Libelle = t.Libelle ?? "" }).ToList(),
                Membres   = membres.Select(m => new MembreSelectDto { IdMembre = m.IdMembre, NomPrenom = m.Nom + " " + m.Prenom }).ToList()
            };

            // Si tontine sélectionnée, charger les réunions
            if (vm.IdTontine > 0)
            {
                vm.Reunions = await _service.GetReunionsAsync(vm.IdTontine, idCycle);

                // Si réunion sélectionnée, charger les données de la séance
                if (vm.IdReunion > 0)
                {
                    var (soldeCaisse, lignes, dejabeneficiaires) =
                        await _service.GetDataAsync(vm.IdTontine, vm.IdReunion, idCycle);
                    vm.SoldeCaisse        = soldeCaisse;
                    vm.Lignes             = lignes;
                    vm.Dejabeneficiaires  = dejabeneficiaires;
                }
            }

            return View(vm);
        }

        // AJAX : membres d'une tontine (sans réunion requise)
        [HttpGet]
        public async Task<IActionResult> GetMembres(int idTontine)
        {
            var membres = await _service.GetMembresAsync(idTontine, CycleId);
            return Json(membres);
        }

        // AJAX : réunions pour une tontine
        [HttpGet]
        public async Task<IActionResult> GetReunions(int idTontine)
        {
            var reunions = await _service.GetReunionsAsync(idTontine, CycleId);
            return Json(reunions);
        }

        // AJAX : données séance
        [HttpGet]
        public async Task<IActionResult> GetData(int idTontine, int idReunion)
        {
            var (soldeCaisse, lignes, dejabeneficiaires) =
                await _service.GetDataAsync(idTontine, idReunion, CycleId);
            return Json(new { soldeCaisse, lignes, dejabeneficiaires });
        }

        // AJAX : enregistrement séance
        [HttpPost]
        public async Task<IActionResult> Enregistrer([FromBody] SaisieSeanceSaveDto dto)
        {
            dto.IdCycle = CycleId;
            var (success, message) = await _service.EnregistrerAsync(dto);
            if (success) return Ok(new { message });
            return BadRequest(new { message });
        }
    }
}
