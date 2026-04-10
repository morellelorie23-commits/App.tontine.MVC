using System.ComponentModel.DataAnnotations;

namespace tontine.MVC.Models
{
    public class PosteViewModel
    {
        public int IdPoste { get; set; }

        [Required(ErrorMessage = "Le libellé est obligatoire")]
        [Display(Name = "Libellé du poste")]
        public string LibellePoste { get; set; } = string.Empty;

        [Display(Name = "Description")]
        public string? Description { get; set; }
    }
}