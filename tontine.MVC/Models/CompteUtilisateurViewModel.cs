using System.ComponentModel.DataAnnotations;

namespace tontine.MVC.Models
{
    public class CompteUtilisateurViewModel
    {
        public int IdCompte { get; set; }

        [Required(ErrorMessage = "Le nom est requis")]
        [Display(Name = "Nom")]
        public string Nom { get; set; } = "";

        [Required(ErrorMessage = "Le prénom est requis")]
        [Display(Name = "Prénom")]
        public string Prenom { get; set; } = "";

        [Required(ErrorMessage = "L'email est requis")]
        [EmailAddress(ErrorMessage = "Email invalide")]
        [Display(Name = "Email")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Le rôle est requis")]
        [Display(Name = "Rôle")]
        public string Role { get; set; } = "Lecteur";

        [Required(ErrorMessage = "Le statut est requis")]
        [Display(Name = "Statut")]
        public string Statut { get; set; } = "Actif";

        public DateOnly DateCreation { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe")]
        public string? MotDePasse { get; set; }

        public string? Photo { get; set; }

        [Display(Name = "Photo de profil")]
        public IFormFile? PhotoFile { get; set; }
    }
}
