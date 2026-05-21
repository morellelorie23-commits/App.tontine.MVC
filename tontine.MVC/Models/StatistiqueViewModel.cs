namespace tontine.MVC.Models
{
    public class StatistiqueViewModel
    {
        public int TotalMembres    { get; set; }
        public int TotalTontines   { get; set; }
        public int TotalCycles     { get; set; }
        public int CyclesActifs    { get; set; }
        public int TotalPostes     { get; set; }
        public int TotalPenalites  { get; set; }
        public decimal MontantTotal      { get; set; }
        public decimal TotalCotisations  { get; set; }
        public decimal TotalVersements   { get; set; }
        public int PretsEnCours          { get; set; }
        public int PretsEnRetard         { get; set; }
        public int CotisationsEnRetard   { get; set; }
        public int CotisationsEnAttente  { get; set; }
        public int CotisationsPayees      { get; set; }
        public int NbCotisations          { get; set; }
        public int AmendesEnAttente       { get; set; }
        public decimal MontantAmendesEnAttente { get; set; }

        public Dictionary<string, int> ParSexe       { get; set; } = new();
        public Dictionary<string, int> ParFrequence  { get; set; } = new();
        public Dictionary<string, int> ParStatut     { get; set; } = new();
        public Dictionary<string, int> ParPays       { get; set; } = new();

        public List<InscriptionMoisDto>       InscriptionsParMois { get; set; } = new();
        public List<CotisationMoisDto>        CotisationsParMois  { get; set; } = new();
        public List<JournalActiviteViewModel> JournalRecents      { get; set; } = new();
    }

    public class InscriptionMoisDto
    {
        public string Mois  { get; set; } = string.Empty;
        public int    Total { get; set; }
    }

    public class CotisationMoisDto
    {
        public string  Mois    { get; set; } = string.Empty;
        public int     Total   { get; set; }
        public decimal Montant { get; set; }
    }
}
