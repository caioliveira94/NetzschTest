namespace NetzschWebApp.Models
{
    public class MessageViewModel
    {
        public string Message { get; set; }
        public List<string> ConsumedMessages { get; set; }

        public MessageViewModel()
        {
            ConsumedMessages= new List<string>();
        }
    }
}
