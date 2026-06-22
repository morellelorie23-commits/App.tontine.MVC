using System.ComponentModel.DataAnnotations;

namespace tontine.MVC.Models
{
    public class CotisationViewModel
    {
        public int IdCotisation { get; set; }

        [Required(ErrorMessage = "Veuillez sélectionner un membre")]
        [Display(Name = "Membre")]
        public int IdMembre { get; set; }

        [Required(ErrorMessage = "Veuillez sélectionner une tontine")]
        [Display(Name = "Tontine")]
        public int IdTontine { get; set; }

        [Required(ErrorMessage = "Veuillez sélectionner un cycle")]
        [Display(Name = "Cycle")]
        public int IdCycle { get; set; }

        [Required(ErrorMessage = "Le montant est obligatoire")]
        [Range(1, double.MaxValue, ErrorMessage = "Le montant doit être supérieur à 0")]
        [Display(Name = "Montant (FCFA)")]
        public decimal Montant { get; set; }

        [Display(Name = "Date")]
        public DateTime DateCotisation { get; set; } = DateTime.Now;

        [Display(Name = "Statut")]
        public string Statut { get; set; } = "En attente";

        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        [Display(Name = "Mandataire (en cas d'absence)")]
        public int? IdMandataire { get; set; }

        [Display(Name = "Mode de paiement")]
        public string ModePaiement { get; set; } = "Cash";

        // Champs affichage (liste)
        public string NomMembre      { get; set; } = string.Empty;
        public string LibelleTontine { get; set; } = string.Empty;
        public string NomCycle       { get; set; } = string.Empty;
        public string NomMandataire  { get; set; } = string.Empty;
    }
}
