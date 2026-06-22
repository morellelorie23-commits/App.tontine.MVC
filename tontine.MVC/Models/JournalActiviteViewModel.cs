namespace tontine.MVC.Models
{
    public class JournalActiviteViewModel
    {
        public int IdJournal { get; set; }
        public string Action { get; set; } = "";
        public string? Description { get; set; }
        public string? Utilisateur { get; set; }
        public DateTime DateAction { get; set; }
    }
}
