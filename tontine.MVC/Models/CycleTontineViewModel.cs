using System.ComponentModel.DataAnnotations;

namespace tontine.MVC.Models
{
    public class CycleTontineViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le cycle est obligatoire")]
        [Display(Name = "Cycle")]
        public int IdCycle { get; set; }

        [Required(ErrorMessage = "La tontine est obligatoire")]
        [Display(Name = "Tontine")]
        public int IdTontine { get; set; }

        // Propriétés d'affichage (non mappées)
        public string? NomCycle { get; set; }
        public string? LibelleTontine { get; set; }
    }
}
