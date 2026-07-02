namespace tontine.MVC.Models
{
    public class PresenceViewModel
    {
        public int    IdPresence     { get; set; }
        public int    IdReunion      { get; set; }
        public int    IdMembre       { get; set; }
        public string NomMembre      { get; set; } = "";
        public string StatutPresence { get; set; } = "Présent";
        public string? Remarque      { get; set; }
    }

    public class GererPresenceViewModel
    {
        public ReunionViewModel     Reunion   { get; set; } = new();
        public List<PresenceViewModel> Presences { get; set; } = new();
    }
}
