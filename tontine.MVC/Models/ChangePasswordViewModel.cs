using System.ComponentModel.DataAnnotations;

namespace tontine.MVC.Models
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Mot de passe actuel requis")]
        [Display(Name = "Mot de passe actuel")]
        public string AncienMotDePasse { get; set; } = "";

        [Required(ErrorMessage = "Nouveau mot de passe requis")]
        [MinLength(6, ErrorMessage = "Minimum 6 caractères")]
        [Display(Name = "Nouveau mot de passe")]
        public string NouveauMotDePasse { get; set; } = "";

        [Required(ErrorMessage = "Confirmation requise")]
        [Compare("NouveauMotDePasse", ErrorMessage = "Les mots de passe ne correspondent pas")]
        [Display(Name = "Confirmer le nouveau mot de passe")]
        public string ConfirmMotDePasse { get; set; } = "";
    }
}
