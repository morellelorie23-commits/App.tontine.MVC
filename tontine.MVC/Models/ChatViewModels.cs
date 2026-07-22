namespace tontine.MVC.Models
{
    public class ChatMessageViewModel
    {
        public string Role { get; set; } = "user"; // "user" ou "assistant"
        public string Contenu { get; set; } = "";
    }
}
