using System.ComponentModel.DataAnnotations;

namespace tontine.MVC.Models
{
    public class CycleViewModel
    {
        public int IdCycle { get; set; }

        [Display(Name = "Nom du cycle")]
        [Required(ErrorMessage = "Le nom du cycle est obligatoire")]
        public string? NomCycle { get; set; }

        [Display(Name = "Date de début")]
        public DateOnly? DateDebut { get; set; }

        [Display(Name = "Date de fin")]
        public DateOnly? DateFin { get; set; }

        [Display(Name = "Statut")]
        public string? Statut { get; set; }
    }
}