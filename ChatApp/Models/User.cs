namespace ChatApp.Models
{
    public class User
    {
        #region
        public int Id { get; set; } 
        public string? Username { get; set; }
        public string? DisplayName { get; set; }
        public string? PrincipalName { get; set; }
        public string? Email { get; set; }
        public string? Title { get; set; }
        public List<string>? Role { get; set; }
        public string? Photo { get; set; }
        #endregion
    }
}
