using System.ComponentModel.DataAnnotations;

namespace tontine.MVC.Models
{
    public class PretViewModel
    {
        public int IdPret { get; set; }

        [Required(ErrorMessage = "Le membre est obligatoire")]
        public int IdMembre { get; set; }

        public string? NomMembre { get; set; }

        [Required(ErrorMessage = "Le montant est obligatoire")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Le montant doit être supérieur à 0")]
        public decimal Montant { get; set; }

        [Required(ErrorMessage = "La taux d'intérêt est obligatoire")]
        [Range(0, 100, ErrorMessage = "Le taux doit être entre 0 et 100")]
        public decimal Taux { get; set; }

        [Required(ErrorMessage = "La date de prêt est obligatoire")]
        public DateTime DatePret { get; set; }

        [Required(ErrorMessage = "La date de remboursement est obligatoire")]
        public DateTime DateRemboursement { get; set; }

        [Required(ErrorMessage = "Le statut est obligatoire")]
        public string Statut { get; set; } = "En attente";  // En attente, Approuvé, Remboursé, En retard

        public string? Description { get; set; }

        public decimal MontantRemboursé { get; set; }
        public DateTime DateCreation { get; set; } = DateTime.Now;
    }
}
