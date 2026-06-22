namespace tontine.MVC.Models
{
    public class ScoreMembreViewModel
    {
        public int    IdMembre      { get; set; }
        public double Score         { get; set; }
        public string Niveau        { get; set; } = "Moyen";
        public string Couleur       { get; set; } = "#F57F17";
        public double Ponctualite   { get; set; }
        public double Regularite    { get; set; }
        public double Solvabilite   { get; set; }
        public int    NbCotisations { get; set; }
        public int    NbPrets       { get; set; }

        public string Emoji => Score >= 80 ? "★" : Score >= 60 ? "◆" : Score >= 40 ? "▲" : "●";
    }
}
