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

        [Display(Name = "Statut")]
        public string Statut { get; set; } = "Planifiée";

        [Display(Name = "Procès-verbal")]
        public string? ProcesVerbal { get; set; }

        // Affichage
        public string NomCycle       { get; set; } = "";
        public string LibelleTontine { get; set; } = "";

        // Compteurs de présence
        public int NombrePresents { get; set; }
        public int NombreAbsents  { get; set; }
        public int NombreExcuses  { get; set; }
        public int TotalPresences => NombrePresents + NombreAbsents + NombreExcuses;
    }
}
