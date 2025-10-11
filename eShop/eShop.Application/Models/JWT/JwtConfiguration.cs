namespace eShop.Application.Models.JWT
{
    public class JwtConfiguration
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Secret { get; set; }
        public int TokenExpiryInMunites { get; set; }
        public int RefreshTokenExpiryInDays { get; set; }
    }
}
