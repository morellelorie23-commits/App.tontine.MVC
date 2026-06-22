using System.ComponentModel.DataAnnotations;

namespace tontine.MVC.Models
{
    public class GarantViewModel
    {
        public int IdGarant { get; set; }

        [Required(ErrorMessage = "Le membre est obligatoire")]
        public int IdMembre { get; set; }

        public string? NomMembre { get; set; }

        [Required(ErrorMessage = "Le nom est obligatoire")]
        [Display(Name = "Nom *")]
        public string Nom { get; set; } = "";

        [Required(ErrorMessage = "Le prénom est obligatoire")]
        [Display(Name = "Prénom *")]
        public string Prenom { get; set; } = "";

        [Required(ErrorMessage = "Le téléphone est obligatoire")]
        [Display(Name = "Téléphone *")]
        public string Telephone { get; set; } = "";

        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Format email invalide")]
        public string? Email { get; set; }

        [Display(Name = "Relation")]
        public string? Relation { get; set; }

        [Display(Name = "Adresse")]
        public string? Adresse { get; set; }

        public DateTime DateAjout { get; set; } = DateTime.Now;
    }

    public class EligibiliteViewModel
    {
        public bool Eligible { get; set; }
        public bool HasGarant { get; set; }
        public bool PretActif { get; set; }
        public string Message { get; set; } = "";
    }
}
