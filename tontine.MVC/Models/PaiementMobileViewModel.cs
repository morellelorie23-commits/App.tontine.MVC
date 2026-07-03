using System.ComponentModel.DataAnnotations;

namespace tontine.MVC.Models
{
    public class PaiementMobileViewModel
    {
        public int       IdPaiement       { get; set; }
        public int?      IdCotisation     { get; set; }
        public string    Telephone        { get; set; } = string.Empty;
        public string    Operateur        { get; set; } = "MTN";
        public string    Reference        { get; set; } = string.Empty;
        public string    Statut           { get; set; } = "En attente";
        public decimal   Montant          { get; set; }
        public DateTime  DateCreation     { get; set; }
        public DateTime? DateConfirmation { get; set; }
        public string?   MessageErreur    { get; set; }
        public string?   NomMembre        { get; set; }
    }

    public class InitierPaiementViewModel
    {
        [Required]
        public int?    IdCotisation { get; set; }
        [Required]
        public string  Telephone    { get; set; } = string.Empty;
        [Required]
        public string  Operateur    { get; set; } = "MTN";
        public decimal Montant      { get; set; }
    }
}
