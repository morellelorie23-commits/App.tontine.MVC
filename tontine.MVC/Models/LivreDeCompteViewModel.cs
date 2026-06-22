namespace tontine.MVC.Models
{
    public class LigneCompteViewModel
    {
        public DateTime Date        { get; set; }
        public string Type          { get; set; } = string.Empty;
        public string Description   { get; set; } = string.Empty;
        public decimal Montant      { get; set; }
        public string NomMembre     { get; set; } = string.Empty;
        public string ModePaiement  { get; set; } = string.Empty;
        public decimal Solde        { get; set; }
    }

    public class LivreDeCompteViewModel
    {
        public decimal TotalDepots   { get; set; }
        public decimal TotalRetraits { get; set; }
        public decimal SoldeActuel   { get; set; }
        public List<LigneCompteViewModel> Lignes { get; set; } = new();
    }
}
