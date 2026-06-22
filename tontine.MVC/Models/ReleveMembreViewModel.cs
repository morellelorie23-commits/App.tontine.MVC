namespace tontine.MVC.Models
{
    public class ReleveMembreViewModel
    {
        public MembreViewModel Membre { get; set; } = new();
        public List<CotisationReleveDto> Cotisations { get; set; } = new();
        public List<VersementReleveDto>  Versements  { get; set; } = new();
        public List<PretViewModel>       Prets       { get; set; } = new();
        public decimal TotalCotisations { get; set; }
        public decimal TotalVersements  { get; set; }
        public decimal TotalPrets       { get; set; }
    }

    public class CotisationReleveDto
    {
        public int     IdCotisation   { get; set; }
        public decimal Montant        { get; set; }
        public DateTime DateCotisation { get; set; }
        public string  Statut         { get; set; } = "";
        public string? Notes          { get; set; }
        public string  LibelleTontine { get; set; } = "";
        public string  NomCycle       { get; set; } = "";
    }

    public class VersementReleveDto
    {
        public int     IdVersement    { get; set; }
        public decimal Montant        { get; set; }
        public DateTime DateVersement { get; set; }
        public string? Notes          { get; set; }
        public string  LibelleTontine { get; set; } = "";
        public string  NomCycle       { get; set; } = "";
    }
}
