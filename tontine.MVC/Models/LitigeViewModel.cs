using System.ComponentModel.DataAnnotations;

namespace tontine.MVC.Models
{
    public class LitigeViewModel
    {
        public int IdLitige { get; set; }

        [Required]
        public int IdCycle { get; set; }

        [Required(ErrorMessage = "Le membre accusé est obligatoire")]
        [Display(Name = "Membre accusé")]
        public int IdMembreAccuse { get; set; }

        [Display(Name = "Membre déclarant")]
        public int? IdMembreDeclarant { get; set; }

        [Required(ErrorMessage = "La nature du litige est obligatoire")]
        [Display(Name = "Nature")]
        public string Nature { get; set; } = "";

        [Required(ErrorMessage = "La description est obligatoire")]
        [Display(Name = "Description")]
        public string Description { get; set; } = "";

        [Display(Name = "Gravité")]
        public string Gravite { get; set; } = "Modérée";

        public string Statut { get; set; } = "Ouvert";

        public DateTime DateDeclaration { get; set; } = DateTime.Now;
        public DateTime? DateResolution { get; set; }

        [Display(Name = "Résolution")]
        public string? Resolution { get; set; }

        // Computed
        public string NomAccuse    { get; set; } = "";
        public string? PhotoAccuse { get; set; }
        public string? NomDeclarant { get; set; }
    }

    public class ExclusionViewModel
    {
        public int IdExclusion { get; set; }

        [Required]
        public int IdCycle { get; set; }

        [Required(ErrorMessage = "Le membre est obligatoire")]
        [Display(Name = "Membre")]
        public int IdMembre { get; set; }

        [Required(ErrorMessage = "Le motif est obligatoire")]
        [Display(Name = "Motif de l'exclusion")]
        public string Motif { get; set; } = "";

        public DateTime DateExclusion { get; set; } = DateTime.Now;

        [Display(Name = "Exclusion définitive")]
        public bool EstDefinitif { get; set; } = false;

        public DateTime? DateReintegration { get; set; }

        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        [Display(Name = "Litige associé")]
        public int? IdLitige { get; set; }

        // Computed
        public string NomMembre    { get; set; } = "";
        public string? PhotoMembre { get; set; }

        public bool EstReintegre => DateReintegration.HasValue;
    }

    public class LitigesIndexViewModel
    {
        public List<LitigeViewModel>    Litiges    { get; set; } = new();
        public List<ExclusionViewModel> Exclusions { get; set; } = new();
    }
}
