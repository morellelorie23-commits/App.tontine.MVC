using System.ComponentModel.DataAnnotations;

namespace tontine.MVC.Models
{
    public class MembrePosteCycleViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le membre est obligatoire")]
        [Display(Name = "Membre")]
        public int IdMembre { get; set; }

        [Required(ErrorMessage = "Le poste est obligatoire")]
        [Display(Name = "Poste")]
        public int IdPoste { get; set; }

        [Required(ErrorMessage = "Le cycle est obligatoire")]
        [Display(Name = "Cycle")]
        public int IdCycle { get; set; }

        [Required(ErrorMessage = "La tontine est obligatoire")]
        [Display(Name = "Tontine")]
        public int IdTontine { get; set; }

        [Display(Name = "Commentaire")]
        public string? Commentaire { get; set; }

        // Propriétés d'affichage
        public string? NomMembre { get; set; }
        public string? LibellePoste { get; set; }
        public string? NomCycle { get; set; }
        public string? LibelleTontine { get; set; }
    }
}
