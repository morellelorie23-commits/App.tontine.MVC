using System.ComponentModel.DataAnnotations;

namespace tontine.MVC.Models
{
    public class FondsSolidariteViewModel
    {
        public int IdFonds { get; set; }

        [Required(ErrorMessage = "Le cycle est obligatoire")]
        public int IdCycle { get; set; }

        [Required(ErrorMessage = "Le nom est obligatoire")]
        [Display(Name = "Nom du fonds")]
        public string Nom { get; set; } = "";

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Cotisation par membre (FCFA)")]
        public decimal MontantParMembre { get; set; }

        public DateTime DateCreation { get; set; } = DateTime.Now;

        // Computed (renvoyé par l'API)
        public string NomCycle    { get; set; } = "";
        public decimal TotalCollecte { get; set; }
        public decimal TotalDepense  { get; set; }
        public decimal Solde => TotalCollecte - TotalDepense;
        public int NombreContributions     { get; set; }
        public int NombreDemandes          { get; set; }
        public int NombreDemandesEnAttente { get; set; }
    }

    public class ContributionFondsViewModel
    {
        public int IdContribution { get; set; }

        [Required]
        public int IdFonds { get; set; }

        [Required(ErrorMessage = "Le membre est obligatoire")]
        [Display(Name = "Membre")]
        public int IdMembre { get; set; }

        [Required(ErrorMessage = "Le montant est obligatoire")]
        [Range(1, double.MaxValue, ErrorMessage = "Le montant doit être supérieur à 0")]
        [Display(Name = "Montant (FCFA)")]
        public decimal Montant { get; set; }

        [Display(Name = "Date")]
        public DateTime DateContribution { get; set; } = DateTime.Now;

        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        public string NomMembre { get; set; } = "";
    }

    public class DemandeAideViewModel
    {
        public int IdDemande { get; set; }

        [Required]
        public int IdFonds { get; set; }

        [Required(ErrorMessage = "Le membre est obligatoire")]
        [Display(Name = "Membre bénéficiaire")]
        public int IdMembre { get; set; }

        [Required(ErrorMessage = "Le montant est obligatoire")]
        [Range(1, double.MaxValue, ErrorMessage = "Le montant doit être supérieur à 0")]
        [Display(Name = "Montant demandé (FCFA)")]
        public decimal MontantDemande { get; set; }

        [Required(ErrorMessage = "Le motif est obligatoire")]
        [Display(Name = "Motif de la demande")]
        public string Motif { get; set; } = "";

        public string Statut { get; set; } = "En attente";

        public DateTime DateDemande { get; set; } = DateTime.Now;
        public DateTime? DateDecision { get; set; }

        [Display(Name = "Montant accordé (FCFA)")]
        public decimal MontantAccorde { get; set; }

        [Display(Name = "Notes de décision")]
        public string? NotesDecision { get; set; }

        public string NomMembre { get; set; } = "";
    }

    public class FondsDetailViewModel
    {
        public FondsSolidariteViewModel          Fonds         { get; set; } = new();
        public List<ContributionFondsViewModel>  Contributions { get; set; } = new();
        public List<DemandeAideViewModel>        Demandes      { get; set; } = new();
    }
}
