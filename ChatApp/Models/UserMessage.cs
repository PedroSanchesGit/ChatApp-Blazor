namespace ChatApp.Models
{
    public class UserMessage
    {
        #region Properties
        public int Id { get; set; } 
        public User? User { get; set; }
        public string? Message { get; set; }
        public bool IsCurrentUser { get; set; }
        public DateTime DateSent { get; set; }
        #endregion
    }
}
