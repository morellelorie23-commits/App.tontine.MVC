using System.ComponentModel.DataAnnotations;

namespace tontine.MVC.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Le nom d'utilisateur est obligatoire")]
        [Display(Name = "Nom d'utilisateur")]
        public string Nom { get; set; } = "";

        [Required(ErrorMessage = "Le mot de passe est obligatoire")]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe")]
        public string MotDePasse { get; set; } = "";

        [Display(Name = "Cycle de travail")]
        public int? IdCycle { get; set; }
    }

    public class UserSessionDto
    {
        public int IdCompte { get; set; }
        public string Nom { get; set; } = "";
        public string Prenom { get; set; } = "";
        public string Email { get; set; } = "";
        public string Role { get; set; } = "";
        public string? Photo { get; set; }
        public int? IdMembre { get; set; }
    }
}
