namespace tontine.MVC.Models
{
    public class TourViewModel
    {
        public int? Rang          { get; set; }
        public string? Matricule  { get; set; }
        public int IdMembre       { get; set; }
        public string NomMembre   { get; set; } = string.Empty;
        public string? PhotoUrl   { get; set; }
        public bool ARecu         { get; set; }
        public DateTime? DateVersement { get; set; }
        public decimal? MontantVerse   { get; set; }
        public string Statut      { get; set; } = "En attente";
    }
}
