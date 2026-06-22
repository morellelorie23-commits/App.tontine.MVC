using System.ComponentModel.DataAnnotations;

namespace tontine.MVC.Models
{
    public class PaiementCaisseViewModel
    {
        // ── Sélection contexte (droite haut) ──
        public int     IdTontine   { get; set; }
        public int     IdReunion   { get; set; }
        public int     IdCycle     { get; set; }
        public decimal SoldeCaisse { get; set; }   // grisé, auto-calculé

        // ── Sous-formulaire membre (droite bas, s'ouvre sur sélection réunion) ──
        [Required(ErrorMessage = "Veuillez sélectionner un membre")]
        public int     IdMembre       { get; set; }
        public decimal MontantAttendu { get; set; }  // grisé, depuis tontine
        public decimal MontantCotise  { get; set; }  // à saisir
        public decimal Reste          { get; set; }  // auto-calculé
        public decimal Penalite       { get; set; }  // à saisir
        public decimal Enchere        { get; set; }  // à saisir
        public DateTime Date          { get; set; } = DateTime.Now;

        // ── Panneau gauche : Règles caisse / Nomenclature ──
        public string  RegionCaisse      { get; set; } = "571001 — Caisse";
        public bool    MarqueCotisation  { get; set; } = true;

        [Required(ErrorMessage = "La pièce justificative est obligatoire")]
        public string  PieceJustificatif { get; set; } = "";

        public decimal MontantDomicilie  { get; set; }

        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Montant invalide")]
        public decimal PaiementEnCaisse  { get; set; }

        public string? Notes { get; set; }
    }

    public class BeneficiaireViewModel
    {
        public int     IdTontine          { get; set; }
        public int     IdCycle            { get; set; }
        public int     IdReunion          { get; set; }

        [Required(ErrorMessage = "Veuillez sélectionner un bénéficiaire")]
        public int     IdBeneficiaire     { get; set; }

        public bool    EstMandataire      { get; set; }

        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Montant invalide")]
        public decimal MontantBeneficiaire { get; set; }

        public decimal RetourEnCaisse     { get; set; }
        public DateTime Date              { get; set; } = DateTime.Now;
        public string? Notes              { get; set; }
    }
}
