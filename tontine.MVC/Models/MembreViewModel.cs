using System.ComponentModel.DataAnnotations;

namespace tontine.MVC.Models
{
    public class MembreViewModel
    {
        public int IdMembre { get; set; }

        [Required(ErrorMessage = "Le nom est obligatoire")]
        [Display(Name = "Nom")]
        public string Nom { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le prénom est obligatoire")]
        [Display(Name = "Prénom")]
        public string Prenom { get; set; } = string.Empty;

        [Display(Name = "Sexe")]
        public string? Sexe { get; set; }

        [Display(Name = "Date de naissance")]
        public DateOnly? DateNaissance { get; set; }

        [Display(Name = "Téléphone")]
        public string? Telephone { get; set; }

        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Email invalide")]
        public string? Email { get; set; }

        [Display(Name = "Adresse")]
        public string? Adresse { get; set; }

        [Display(Name = "Ville")]
        public string? Ville { get; set; }

        [Display(Name = "Pays")]
        public string? Pays { get; set; }

        [Display(Name = "Profession")]
        public string? Profession { get; set; }

        [Display(Name = "Numéro CNI")]
        public string? NumeroCni { get; set; }

        [Display(Name = "Photo")]
        public string? Photo { get; set; }

        [Display(Name = "Photo du membre")]
        public IFormFile? PhotoFile { get; set; }

        [Display(Name = "Date d'inscription")]
        public DateTime DateInscription { get; set; } = DateTime.Now;
    }
}