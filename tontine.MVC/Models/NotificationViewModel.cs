namespace tontine.MVC.Models
{
    public class NotificationViewModel
    {
        public int IdNotification { get; set; }
        public string Titre { get; set; } = "";
        public string Message { get; set; } = "";
        public string Type { get; set; } = "Info";
        public bool Lue { get; set; }
        public DateTime DateCreation { get; set; }
        public int? IdCycle { get; set; }
        public int? IdTontine { get; set; }
        public string? Lien { get; set; }

        public string CouleurType => Type switch
        {
            "Alerte"       => "#C62828",
            "Avertissement" => "#E65100",
            _              => "#0F6E56"
        };

        public string IconeType => Type switch
        {
            "Alerte"       => "alert-triangle",
            "Avertissement" => "alert-circle",
            _              => "info"
        };
    }
}
