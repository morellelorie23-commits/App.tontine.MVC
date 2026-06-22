using System.ComponentModel.DataAnnotations;

namespace tontine.MVC.Models
{
    public class CycleTontinePenaliteViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le cycle est obligatoire")]
        [Display(Name = "Cycle")]
        public int? IdCycle { get; set; }

        [Required(ErrorMessage = "La tontine est obligatoire")]
        [Display(Name = "Tontine")]
        public int? IdTontine { get; set; }

        [Required(ErrorMessage = "La pénalité est obligatoire")]
        [Display(Name = "Pénalité")]
        public int? IdPenalite { get; set; }

        [Display(Name = "Taux avant (%)")]
        public decimal? TauxAvant { get; set; }

        [Display(Name = "Taux après (%)")]
        public decimal? TauxApres { get; set; }

        // Propriétés d'affichage
        public string? NomCycle { get; set; }
        public string? LibelleTontine { get; set; }
        public string? LibellePenalite { get; set; }
    }
}
