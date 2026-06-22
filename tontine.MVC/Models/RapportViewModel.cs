namespace tontine.MVC.Models
{
    public class RapportViewModel
    {
        public decimal TotalCotisations     { get; set; }
        public decimal TotalVersements      { get; set; }
        public decimal TotalPrets           { get; set; }
        public decimal TotalRembourse       { get; set; }
        public int     NbMembres            { get; set; }
        public int     NbCycles             { get; set; }
        public int     NbTontines           { get; set; }
        public int     CotisationsPayees    { get; set; }
        public int     CotisationsEnAttente { get; set; }
        public int     CotisationsEnRetard  { get; set; }
        public int     PretsApprouves       { get; set; }
        public int     PretsRembourses      { get; set; }
        public int     PretsEnRetard        { get; set; }

        public decimal SoldeActuel  => TotalCotisations - TotalVersements;
        public int     NbCotisations => CotisationsPayees + CotisationsEnAttente + CotisationsEnRetard;
        public double  TauxPaiement => NbCotisations > 0 ? (double)CotisationsPayees / NbCotisations * 100 : 0;

        public int NbMembresActifs   { get; set; }
        public int NbMembresEnRetard { get; set; }

        public Dictionary<string, decimal> ParTontine           { get; set; } = new();
        public Dictionary<string, decimal> ParCycle             { get; set; } = new();
        public Dictionary<string, decimal> TopMembres           { get; set; } = new();
        public Dictionary<string, decimal> CotisationsParTontine { get; set; } = new();
        public Dictionary<string, int>     RetardsParTontine     { get; set; } = new();

        public List<EvolutionMensuelleDto> EvolutionMensuelle   { get; set; } = new();
        public List<AlerteMembreDto>       MembresEnRetard       { get; set; } = new();
        public List<AlertePretDto>         PretsEnRetardDetails  { get; set; } = new();
    }

    public class EvolutionMensuelleDto
    {
        public string  Mois        { get; set; } = string.Empty;
        public decimal Cotisations { get; set; }
        public decimal Versements  { get; set; }
    }

    public class AlerteMembreDto
    {
        public string  NomMembre      { get; set; } = string.Empty;
        public int     NbRetards      { get; set; }
        public decimal MontantEnRetard { get; set; }
    }

    public class AlertePretDto
    {
        public string   NomMembre         { get; set; } = string.Empty;
        public decimal  Montant           { get; set; }
        public decimal  MontantRembourse  { get; set; }
        public DateTime DateRemboursement { get; set; }
    }
}
