namespace eShop.Application.Models
{
    public class EmailConfiguration
    {
        public int Port { get; set; }
        public string Host { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
        public bool EnableSsl { get; set; }
    }
}
