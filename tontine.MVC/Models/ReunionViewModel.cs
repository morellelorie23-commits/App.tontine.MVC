using System.ComponentModel.DataAnnotations;

namespace tontine.MVC.Models
{
    public class ReunionViewModel
    {
        public int IdReunion { get; set; }

        [Required(ErrorMessage = "Le cycle est obligatoire")]
        [Display(Name = "Cycle")]
        public int IdCycle { get; set; }

        [Required(ErrorMessage = "La tontine est obligatoire")]
        [Display(Name = "Tontine")]
        public int IdTontine { get; set; }

        [Required(ErrorMessage = "La date est obligatoire")]
        [Display(Name = "Date de la réunion")]
        public DateTime DateReunion { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "L'objet est obligatoire")]
        [Display(Name = "Objet / Ordre du jour")]
        public string Objet { get; set; } = "";

        [Display(Name = "Lieu")]
        public string? Lieu { get; set; }

        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        // Affichage
        public string NomCycle       { get; set; } = "";
        public string LibelleTontine { get; set; } = "";
    }
}
