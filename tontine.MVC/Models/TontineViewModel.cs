using System.ComponentModel.DataAnnotations;

namespace tontine.MVC.Models
{
    public class TontineViewModel
    {
        public int IdTontine { get; set; }

        [Required(ErrorMessage = "Le libellé est obligatoire")]
        [Display(Name = "Libellé")]
        public string? Libelle { get; set; }

        [Required(ErrorMessage = "Le montant est obligatoire")]
        [Display(Name = "Montant (FCFA)")]
        public decimal? Montant { get; set; }

        [Display(Name = "Fréquence")]
        public string? Frequence { get; set; }
    }
}