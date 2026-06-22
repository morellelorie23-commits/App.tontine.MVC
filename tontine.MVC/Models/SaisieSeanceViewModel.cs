namespace tontine.MVC.Models
{
    public class SaisieSeancePageViewModel
    {
        public int IdTontine { get; set; }
        public int IdReunion { get; set; }
        public int IdCycle   { get; set; }

        public decimal SoldeCaisse { get; set; }

        public List<LigneSeanceViewModel>        Lignes              { get; set; } = new();
        public List<HistoriqueBeneficiaireDto>   Dejabeneficiaires   { get; set; } = new();
        public List<TontineSelectDto>            Tontines            { get; set; } = new();
        public List<ReunionSelectDto>            Reunions            { get; set; } = new();
        public List<MembreSelectDto>             Membres             { get; set; } = new();

        public int?    IdBeneficiaire   { get; set; }
        public string? Mandataires      { get; set; }
        public decimal MontantBeneficie { get; set; }
        public decimal RetourCaisse     { get; set; }
    }

    public class LigneSeanceViewModel
    {
        public int     IdMembre         { get; set; }
        public string  NomPrenom        { get; set; } = "";
        public decimal MtAttendu        { get; set; }
        public decimal MtCotise         { get; set; }
        public decimal Penalite         { get; set; }
        public decimal MtEnchere        { get; set; }
        public bool    IsGagnantEnchere  { get; set; }
    }

    public class HistoriqueBeneficiaireDto
    {
        public string  NomPrenom      { get; set; } = "";
        public string  DateVersement  { get; set; } = "";
        public decimal MontantNet     { get; set; }
    }

    public class TontineSelectDto
    {
        public int    IdTontine { get; set; }
        public string Libelle   { get; set; } = "";
    }

    public class ReunionSelectDto
    {
        public int    IdReunion     { get; set; }
        public string DateReunion   { get; set; } = "";
        public string? Objet        { get; set; }
    }

    public class MembreSelectDto
    {
        public int     IdMembre  { get; set; }
        public string  NomPrenom { get; set; } = "";
        public decimal MtAttendu { get; set; }
    }

    // DTO pour l'envoi AJAX au MVC controller
    public class SaisieSeanceSaveDto
    {
        public int IdTontine { get; set; }
        public int IdReunion { get; set; }
        public int IdCycle   { get; set; }
        public int? IdBeneficiaire   { get; set; }
        public decimal MontantBeneficie { get; set; }
        public decimal RetourCaisse     { get; set; }
        public List<LigneSeanceSaveDto> Lignes { get; set; } = new();
    }

    public class LigneSeanceSaveDto
    {
        public int     IdMembre         { get; set; }
        public decimal MtAttendu        { get; set; }
        public decimal MtCotise         { get; set; }
        public decimal Penalite         { get; set; }
        public decimal MtEnchere        { get; set; }
        public bool    IsGagnantEnchere  { get; set; }
    }
}
