namespace tontine.MVC.Models
{
    public class LigneGrandLivreViewModel
    {
        public DateTime DateEcriture     { get; set; }
        public string CodeJournal        { get; set; } = "";
        public string PieceJustificative { get; set; } = "";
        public string LibelleEcriture    { get; set; } = "";
        public string LibelleLigne       { get; set; } = "";
        public string Sens               { get; set; } = "D";
        public decimal Montant           { get; set; }
        public decimal SoldeProgressif   { get; set; }
        public string PeriodeComptable   { get; set; } = "";
    }

    public class GrandLivreViewModel
    {
        public int IdTontine            { get; set; }
        public string NomTontine        { get; set; } = "";
        public string CompteSequestre   { get; set; } = "";
        public decimal TotalDepots      { get; set; }
        public decimal TotalRetraits    { get; set; }
        public decimal SoldeActuel      { get; set; }
        public List<LigneGrandLivreViewModel> Lignes { get; set; } = new();
    }

    public class LigneEcritureDetailViewModel
    {
        public int NumeroLigne   { get; set; }
        public string Sens       { get; set; } = "";
        public string CompteOhada { get; set; } = "";
        public string LibelleCompte { get; set; } = "";
        public string LibelleLigne  { get; set; } = "";
        public decimal Montant   { get; set; }
    }

    public class EcritureDetailViewModel
    {
        public int IdEcriture            { get; set; }
        public string CodeJournal        { get; set; } = "";
        public DateTime DateEcriture     { get; set; }
        public string PeriodeComptable   { get; set; } = "";
        public int NumeroSequence        { get; set; }
        public string PieceJustificative { get; set; } = "";
        public string Libelle            { get; set; } = "";
        public decimal TotalDebit        { get; set; }
        public decimal TotalCredit       { get; set; }
        public List<LigneEcritureDetailViewModel> Lignes { get; set; } = new();
    }

    public class JournalViewModel
    {
        public string CodeJournal        { get; set; } = "";
        public string? Periode           { get; set; }
        public int NombreEcritures       { get; set; }
        public decimal TotalDebits       { get; set; }
        public decimal TotalCredits      { get; set; }
        public List<EcritureDetailViewModel> Ecritures { get; set; } = new();
    }

    public class CompteBalanceViewModel
    {
        public string Compte        { get; set; } = "";
        public string Libelle       { get; set; } = "";
        public decimal TotalDebit   { get; set; }
        public decimal TotalCredit  { get; set; }
        public decimal SoldeDebiteur { get; set; }
        public decimal SoldeCredit  { get; set; }
    }

    public class BalanceViewModel
    {
        public string Periode       { get; set; } = "";
        public decimal TotalDebits  { get; set; }
        public decimal TotalCredits { get; set; }
        public List<CompteBalanceViewModel> Comptes { get; set; } = new();
    }

    // ── Journée comptable ────────────────────────────────────────────────
    public class JourneeComptableViewModel
    {
        public int Id                  { get; set; }
        public DateOnly DateJournee    { get; set; }
        public string Statut           { get; set; } = "";
        public DateTime DateOuverture  { get; set; }
        public DateTime? DateFermeture { get; set; }
        public string OuvertPar        { get; set; } = "";
    }

    // ── Relevé de caisse ─────────────────────────────────────────────────
    public class LigneReleveCaisseViewModel
    {
        public DateTime DateEcriture { get; set; }
        public string CompteOhada   { get; set; } = "";
        public string Intitule      { get; set; } = "";
        public string Piece         { get; set; } = "";
        public string Motif         { get; set; } = "";
        public decimal Debit        { get; set; }
        public decimal Credit       { get; set; }
        public decimal Solde        { get; set; }
    }

    public class ReleveCaisseViewModel
    {
        public string? Compte      { get; set; }
        public string? DateDebut   { get; set; }
        public string? DateFin     { get; set; }
        public decimal SoldeFinal  { get; set; }
        public decimal TotalDebit  { get; set; }
        public decimal TotalCredit { get; set; }
        public List<LigneReleveCaisseViewModel> Lignes { get; set; } = new();
    }

    // ── Balance générale & client ─────────────────────────────────────────
    public class CompteBalanceDetailViewModel
    {
        public string CompteOhada          { get; set; } = "";
        public string Intitule             { get; set; } = "";
        public decimal SoldeInitialDebit   { get; set; }
        public decimal SoldeInitialCredit  { get; set; }
        public decimal MouvementDebit      { get; set; }
        public decimal MouvementCredit     { get; set; }
        public decimal SoldeFinalDebit     { get; set; }
        public decimal SoldeFinalCredit    { get; set; }
    }

    public class TotauxBalanceViewModel
    {
        public decimal TotalSoldeInitialDebit   { get; set; }
        public decimal TotalSoldeInitialCredit  { get; set; }
        public decimal TotalMouvementDebit      { get; set; }
        public decimal TotalMouvementCredit     { get; set; }
        public decimal TotalSoldeFinalDebit     { get; set; }
        public decimal TotalSoldeFinalCredit    { get; set; }
    }

    public class BalanceDetailViewModel
    {
        public string? DateDebut                { get; set; }
        public string? DateFin                  { get; set; }
        public string? PrefixeCompte            { get; set; }
        public List<CompteBalanceDetailViewModel> Comptes { get; set; } = new();
        public TotauxBalanceViewModel Totaux    { get; set; } = new();
        public TotauxBalanceViewModel? TotauxBilan { get; set; }
    }

    public class RapprochementViewModel
    {
        public string Tontine                { get; set; } = "";
        public string CompteSequestre        { get; set; } = "";
        public decimal SoldeLivreComptable   { get; set; }
        public decimal SoldeTransactions     { get; set; }
        public decimal EcartInterne          { get; set; }
        public decimal? SoldeReelBanque      { get; set; }
        public decimal? EcartBanque          { get; set; }
        public bool Equilibre                { get; set; }
        public decimal TotalCotisations      { get; set; }
        public decimal TotalVersements       { get; set; }
    }
}
