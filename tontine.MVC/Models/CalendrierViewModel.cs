namespace tontine.MVC.Models
{
    public class CalendrierSlotViewModel
    {
        public int     NumeroOrdre    { get; set; }
        public int     IdMembre       { get; set; }
        public string  NomMembre      { get; set; } = "";
        public string  LibelleTontine { get; set; } = "";
        public int     IdTontine      { get; set; }
        public decimal MontantPot     { get; set; }
        public DateTime? DatePrevue   { get; set; }
        public bool    Verse          { get; set; }
        public DateTime? DateVersement { get; set; }
        public decimal MontantNet     { get; set; }
        public int?    IdVersement    { get; set; }
        public string  Statut         { get; set; } = "À venir";
        public string  Photo          { get; set; } = "";
    }

    public class CalendrierTontineViewModel
    {
        public int     IdTontine         { get; set; }
        public string  LibelleTontine    { get; set; } = "";
        public string  Frequence         { get; set; } = "";
        public decimal MontantCotisation { get; set; }
        public decimal MontantPot        { get; set; }
        public int     NombreMembres     { get; set; }
        public int     NombreVerses      { get; set; }
        public List<CalendrierSlotViewModel> Slots { get; set; } = new();
        public decimal PourcentageAvancement => NombreMembres > 0 ? 100m * NombreVerses / NombreMembres : 0;
    }

    public class CalendrierViewModel
    {
        public string    NomCycle   { get; set; } = "";
        public DateOnly? DateDebut  { get; set; }
        public DateOnly? DateFin    { get; set; }
        public List<CalendrierTontineViewModel> ParTontine { get; set; } = new();
        public int TotalSlots  => ParTontine.Sum(t => t.NombreMembres);
        public int TotalVerses => ParTontine.Sum(t => t.NombreVerses);
    }
}
