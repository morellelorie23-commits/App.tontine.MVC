using System.ComponentModel.DataAnnotations;

namespace tontine.MVC.Models
{
    public class VersementViewModel
    {
        public int IdVersement { get; set; }

        [Required(ErrorMessage = "Veuillez sélectionner un membre")]
        [Display(Name = "Membre bénéficiaire")]
        public int IdMembre { get; set; }

        [Required(ErrorMessage = "Veuillez sélectionner une tontine")]
        [Display(Name = "Tontine")]
        public int IdTontine { get; set; }

        [Required(ErrorMessage = "Veuillez sélectionner un cycle")]
        [Display(Name = "Cycle")]
        public int IdCycle { get; set; }

        [Required(ErrorMessage = "Le montant est obligatoire")]
        [Range(1, double.MaxValue, ErrorMessage = "Le montant doit être supérieur à 0")]
        [Display(Name = "Montant versé (FCFA)")]
        public decimal Montant { get; set; }

        [Display(Name = "Date")]
        public DateTime DateVersement { get; set; } = DateTime.Now;

        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        [Display(Name = "Déduction (part du bénéficiaire)")]
        public decimal MontantDeduction { get; set; }

        [Display(Name = "Commission")]
        public decimal MontantCommission { get; set; }

        [Display(Name = "Montant net encaissé")]
        public decimal MontantNet { get; set; }

        // Champs affichage (liste)
        public string NomMembre      { get; set; } = string.Empty;
        public string LibelleTontine { get; set; } = string.Empty;
        public string NomCycle       { get; set; } = string.Empty;
    }
}
