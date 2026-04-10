using System.ComponentModel.DataAnnotations;

namespace tontine.MVC.Models
{
    public class PenaliteViewModel
    {
        public int IdPenalite { get; set; }

        [Display(Name = "Libellé")]
        [Required(ErrorMessage = "Le libellé est obligatoire")]
        public string? Libelle { get; set; }

        [Display(Name = "Description")]
        public string? Description { get; set; }
    }
}