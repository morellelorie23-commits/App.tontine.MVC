namespace tontine.MVC.Models
{
    public class PortailMembreViewModel
    {
        public MembreViewModel Membre { get; set; } = new();
        public List<CotisationViewModel> MesCotisations { get; set; } = new();
        public List<VersementViewModel> MesVersements { get; set; } = new();
        public List<PretViewModel> MesPrets { get; set; } = new();
        public List<MembreCycleTontineViewModel> MesInscriptions { get; set; } = new();
        public List<PlanningTontineViewModel> PlanningParTontine { get; set; } = new();
        public List<ReunionViewModel> ProchainesReunions { get; set; } = new();

        // KPIs calculés
        public decimal TotalCotise          => MesCotisations.Where(c => c.Statut == "Payé").Sum(c => c.Montant);
        public decimal TotalRecu            => MesVersements.Sum(v => v.MontantNet);
        public int     CotisationsPayees    => MesCotisations.Count(c => c.Statut == "Payé");
        public int     CotisationsEnAttente => MesCotisations.Count(c => c.Statut == "En attente");
        public int     CotisationsEnRetard  => MesCotisations.Count(c => c.Statut == "En retard");
        public int     PretsActifs          => MesPrets.Count(p => p.Statut == "Approuvé" || p.Statut == "En retard");
        public decimal MontantDuPrets       => MesPrets
            .Where(p => p.Statut == "Approuvé" || p.Statut == "En retard")
            .Sum(p => p.Montant + Math.Round(p.Montant * p.Taux / 100, 0));
        public bool    EstAJour             => CotisationsEnRetard == 0;
    }

    public class PlanningTontineViewModel
    {
        public string  NomTontine        { get; set; } = "";
        public int     IdTontine         { get; set; }
        public decimal MontantCotisation { get; set; }
        public string  Frequence         { get; set; } = "";
        public int     MonNumeroOrdre    { get; set; }
        public decimal MesNombreParts    { get; set; } = 1;
        public decimal MontantARecevoir  { get; set; }
        public int     NombreTotal       { get; set; }
        public int     NombreARecu       { get; set; }
        public int     PersonnesAvantMoi { get; set; }
        public bool    ADejaRecu         { get; set; }
        public List<LignePlanningViewModel> Lignes { get; set; } = new();
    }

    public class LignePlanningViewModel
    {
        public int      IdMembre     { get; set; }
        public string   NomMembre    { get; set; } = "";
        public int      NumeroOrdre  { get; set; }
        public decimal  NombreParts  { get; set; } = 1;
        public bool     ARecu        { get; set; }
        public DateTime? DateReception { get; set; }
        public decimal  MontantRecu  { get; set; }
        public bool     EstMoi       { get; set; }
    }
}
