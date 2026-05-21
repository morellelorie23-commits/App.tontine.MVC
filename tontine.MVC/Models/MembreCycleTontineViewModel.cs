using System.ComponentModel.DataAnnotations;

namespace tontine.MVC.Models
{
    public class MembreCycleTontineViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le membre est obligatoire")]
        [Display(Name = "Membre")]
        public int IdMembre { get; set; }

        [Required(ErrorMessage = "Le cycle est obligatoire")]
        [Display(Name = "Cycle")]
        public int IdCycle { get; set; }

        [Required(ErrorMessage = "La tontine est obligatoire")]
        [Display(Name = "Tontine")]
        public int IdTontine { get; set; }

        [Display(Name = "Matricule dans la tontine")]
        public string? Matricule { get; set; }

        [Display(Name = "N° d'ordre de bénéfice")]
        public int? NumeroOrdre { get; set; }

        [Display(Name = "Nombre de parts")]
        public decimal NombreParts { get; set; } = 1m;

        // Propriétés d'affichage
        public string? NomMembre { get; set; }
        public string? NomCycle { get; set; }
        public string? LibelleTontine { get; set; }

        public string LibelleParts => NombreParts switch {
            0.5m => "Demi-part (½)",
            2m   => "Double part (×2)",
            _    => "Part entière (×1)"
        };
    }
}
