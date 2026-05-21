namespace tontine.MVC.Models
{
    public class AmendeViewModel
    {
        public int     IdAmende          { get; set; }
        public int     IdCotisation      { get; set; }
        public int     IdMembre          { get; set; }
        public int     IdCycle           { get; set; }
        public int     IdTontine         { get; set; }
        public string  NomMembre         { get; set; } = "";
        public string  NomCycle          { get; set; } = "";
        public string  LibelleTontine    { get; set; } = "";
        public string  LibellePenalite   { get; set; } = "";
        public DateTime DateCotisation   { get; set; }
        public DateTime DateCalcul       { get; set; }
        public DateTime? DatePaiement    { get; set; }
        public decimal MontantCotisation { get; set; }
        public decimal TauxApplique      { get; set; }
        public decimal MontantAmende     { get; set; }
        public string  Statut            { get; set; } = "En attente";

        public bool EstPayee => Statut == "Payée";
    }
}
